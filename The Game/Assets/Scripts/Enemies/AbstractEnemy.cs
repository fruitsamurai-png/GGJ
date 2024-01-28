using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public abstract class Enemy
{
    public StateMachine m_EnemyStateMachine;
    public GameObject m_GameObject;

	public string m_animationString = "idle";

    public GameObject m_PlayerObject;
    public bool m_IsPlayerInFOV = false;
	public bool IsNoticingStolenPainting = false;
	public float m_GroundLevel = 1.0f;

    public float m_IdleDuration = 0.0f;

    // Speed
    public float m_PatrolSpeed = 1.0f;
    public float m_DistractedSpeed = 8.0f;
    public float m_PursuitSpeed = 12.0f;

    // FOV
    public float m_Fov = 60.0f;
    public float m_ViewDistance = 5.0f;

    // Alert
#if UNITY_EDITOR
    public static bool m_IgnoreAlert = false;
#endif
    public bool m_IsAlerted = false;
    public float m_AlertLevel = 0.0f;
    public float m_AlertGraceElapsedTime = 0.0f;
    public float m_AlertIncreaseStep = 0.001f;
    public float m_AlertDecreaseStep = 0.0005f; // decrease half as fast as increase
    public float m_AlertGracePeriod = 2.0f;
    public Material m_MeshMaterial;
    public Color m_OriginalMaterialColor;
    public EnemyAlertBar m_AlertBar;


    // Stunned
    public int m_Level = 1;
    public bool m_IsStunned = false;
    public float m_StunnedDuration = 1.0f;

    private Mesh fovMesh;
    private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();
    protected Enemy(GameObject gameObject, EnemyAlertBar enemyAlertBar , GameObject fovGameObject)
    {
        m_GameObject = gameObject;

        m_AlertBar = enemyAlertBar;
        fovMesh = fovGameObject.GetComponent<MeshFilter>().mesh;

        m_MeshMaterial = m_GameObject.GetComponent<MeshRenderer>().material;
        m_OriginalMaterialColor = m_MeshMaterial.color;
        m_PlayerObject = GameObject.FindGameObjectWithTag("Player");
    }

    public abstract void Start();
    public abstract void Update();

    // Gameplay enemy interface
    public abstract void NotifyDistraction(GameObject distraction);// - will be called by distractions the player activated.
    public abstract void Alert(GameObject alerter);// - will be called by other enemies when they are alerted to also alert this enemy.
    
    // Virtual so can override for specialized behavior, unsure if needed
    public virtual void Jailbreak(int playerLevel, float stunTime)// - stuns the enemy for specified stun time if the playerLevel is >= enemy level.
    {
        if (playerLevel >= m_Level)
        {
            m_IsStunned = true;
            // += here in case we can stack stun durations
            m_StunnedDuration += stunTime;
        }
    }
    public virtual void Noise(float noiselevel)
    {
        // - will be called by the player when nearby this enemy and making noise, refer to mechanics on what this does.
        m_AlertLevel += noiselevel;
    }
    public virtual void AlertGuardsInVicinity(GameObject alerter, float radius)
    {
        foreach (Collider c in Physics.OverlapSphere(m_GameObject.transform.position, radius, LayerMask.GetMask("Guards")))
        {
            GameObject o = c.gameObject;

            // Don't alert self
            if (o == m_GameObject)
                continue;

            if (o.TryGetComponent(out GuardEnemyBehavior guardEnemyBehavior))
            {
                guardEnemyBehavior.m_Enemy.Alert(alerter);
            }

            // Do we inform other cameras?
            else if (o.TryGetComponent(out CameraEnemyBehavior cameraEnemyBehavior))
            {
                cameraEnemyBehavior.m_Enemy.Alert(alerter);
            }
        }
    }

    public virtual void UpdateAlertness()
    {
        // 2 ray cast on edges of cone done when drawing fov cone.
        Ray ray = new Ray(m_GameObject.transform.position, m_GameObject.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            if (hitInfo.distance < m_ViewDistance)
            {
                m_IsPlayerInFOV |= (m_PlayerObject == hitInfo.transform.gameObject);
            }
        }

        if (m_IsPlayerInFOV && !m_IsStunned)
        {
            IncreaseAlertess();
            if (m_AlertLevel >= 1.0f && !m_IsAlerted)
            {
                Alert(m_GameObject);
            }
        }
        else
        {
            if (m_AlertGraceElapsedTime >= m_AlertGracePeriod)
            {
                DecreaseAlertess();
            }
            m_AlertGraceElapsedTime += Time.deltaTime;
        }

        if (m_IsStunned)
        {
            m_MeshMaterial.color = Color.yellow;
        }
        else
        {
            m_MeshMaterial.color = Color.LerpUnclamped(m_OriginalMaterialColor, Color.red, m_AlertLevel);
        }

        UpdateAlertbar();
    }
    public virtual void UpdateAlertbar()
    {
        m_AlertBar.alertValue = m_AlertLevel;
        m_AlertBar.gameObject.SetActive(m_AlertLevel > 0.01f);
    }

    public virtual void IncreaseAlertess()
    {
#if UNITY_EDITOR
     if (m_IgnoreAlert)
         return; 
#endif
        m_AlertLevel += m_AlertIncreaseStep;
        m_AlertGraceElapsedTime = 0.0f;
        m_AlertLevel = Math.Clamp(m_AlertLevel, 0.0f, 1.0f);
    }

    public virtual void DecreaseAlertess()
    {
#if UNITY_EDITOR
        if (m_IgnoreAlert)
            return;
#endif
        m_AlertLevel -= m_AlertDecreaseStep;
        m_AlertLevel = Math.Clamp(m_AlertLevel, 0.0f, 1.0f);
    }
    public virtual void DrawFOVCone()
    {
        newVertices.Clear();
        newTriangles.Clear();

        m_Fov = Mathf.Clamp(m_Fov, 0, 360);

        int segments = m_Fov > 180 ? 48 : 12; 
     
        Vector3 origin = m_GameObject.transform.position + m_GameObject.transform.forward * 0.5f;
        origin.y = m_GroundLevel;
        //origin
        newVertices.Add(origin);
        float angleInterval = m_Fov / segments;
        float startAngle = -m_Fov * 0.5f;
        int i;
        for ( i = 0; i <= segments ; ++i)
        {
            //Debug.Log(startAngle);
            Quaternion rot = Quaternion.Euler(0.0f, startAngle, 0.0f);
            Vector3 dir = rot * m_GameObject.transform.forward ;
            Ray ray = new Ray(m_GameObject.transform.position, dir);
            Vector3 pt = origin + dir * m_ViewDistance;
            if (Physics.Raycast(ray, out RaycastHit hit, m_ViewDistance, ~LayerMask.GetMask("Guards")))
            {
                pt = hit.point;
                m_IsPlayerInFOV |= (hit.transform.gameObject == m_PlayerObject);
            }
            startAngle += angleInterval;
            pt.y = m_GroundLevel;
            newVertices.Add(pt);
        }
      
        for ( i = 0; i < segments; ++i)
        {
            newTriangles.Add(0);
            newTriangles.Add(i+1);
            newTriangles.Add(i+2);
        }
        if (m_Fov == 360)
        {
            newVertices.Add(newVertices[0]);

            int last = newTriangles[newTriangles.Count - 1];
            int second = newTriangles[1];
            newTriangles.Add(0);
            newTriangles.Add(last);
            newTriangles.Add(second);

        }

        fovMesh.Clear();
        fovMesh.vertices = newVertices.ToArray();
        fovMesh.triangles = newTriangles.ToArray();
    }
}

