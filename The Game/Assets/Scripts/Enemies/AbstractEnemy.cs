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

    public LineRenderer m_LineRenderer;
    public float m_Fov = 30.0f;
    public float m_ViewDistance = 5.0f;
    private int FOVLayerExcludeMask = 0;

    public float m_AlertLevel = 0.0f;
    public readonly float m_AlertGracePeriod = 2.0f;
    public float m_AlertGraceElapsedTime = 0.0f;
    public float m_AlertStepAmount = 0.001f;
    public Material m_MeshMaterial;

    public GameObject m_PlayerObject;
    public bool IsPlayerInFOV = false;

    protected Enemy(GameObject gameObject, Material fovMaterial)
    {
        m_GameObject = gameObject;
        m_LineRenderer = m_GameObject.GetComponent<LineRenderer>();

        if(m_LineRenderer == null)
        {
            m_GameObject.AddComponent<LineRenderer>();
        }

        m_LineRenderer.useWorldSpace = true;
        m_LineRenderer.positionCount = 4;
        m_LineRenderer.widthMultiplier = 0.04f;
        m_LineRenderer.material = fovMaterial;

        m_MeshMaterial = m_GameObject.GetComponent<MeshRenderer>().material;
        m_PlayerObject = GameObject.FindGameObjectWithTag("Player");

        FOVLayerExcludeMask |= LayerMask.NameToLayer("Guards");
        FOVLayerExcludeMask |= LayerMask.NameToLayer("Player");
    }

    public abstract void Start();
    public abstract void Update();

    // Gameplay enemy interface
    public abstract void NotifyDistraction(GameObject distraction);// - will be called by distractions the player activated.
    public abstract void Alert(GameObject alerter);// - will be called by other enemies when they are alerted to also alert this enemy.
    public abstract void Noise(float noiselevel);// - will be called by the player when nearby this enemy and making noise, refer to mechanics on what this does.
    public abstract void Jailbreak(int playerLevel, float stunTime);// - stuns the enemy for specified stun time if the playerLevel is >= enemy level.
    
    public virtual void AlertGuardsInVicinity(GameObject alerter, float radius)
    {
        foreach (Collider c in Physics.OverlapSphere(m_GameObject.transform.position, radius, LayerMask.GetMask("Guards")))
        {
            GameObject o = c.gameObject;
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
                IsPlayerInFOV |= (m_PlayerObject == hitInfo.transform.gameObject);
            }
        }

        if (IsPlayerInFOV)
        {
            IncreaseAlertess(m_AlertStepAmount);
        }
        else
        {
            if (m_AlertGraceElapsedTime >= m_AlertGracePeriod)
            {
                DecreaseAlertess(m_AlertStepAmount);
            }
            m_AlertGraceElapsedTime += Time.deltaTime;
        }

        m_AlertLevel = Math.Clamp(m_AlertLevel, 0.0f, 1.0f);
        m_MeshMaterial.color= Color.LerpUnclamped(Color.white, Color.red, m_AlertLevel);

        if (m_AlertLevel >= 1.0f)
        {
            Alert(m_GameObject);
        }
    }

    public virtual void IncreaseAlertess(float amount)
    {
        m_AlertLevel += amount;
        m_AlertGraceElapsedTime = 0.0f;
    }

    public virtual void DecreaseAlertess(float amount)
    {
        m_AlertLevel -= amount;
    }

    private bool IsValidFOVHitTarget(int hitLayer)
    {
        if (hitLayer == 0)
            return true;

        return (FOVLayerExcludeMask & hitLayer) == 1;
    }

    public virtual void DrawFOVCone()
    {
        // Doesn't really work well for angles > 180..
        Vector3 foward = m_GameObject.transform.forward;
        float rotationAngle = m_Fov / 2.0f; 

        Quaternion rotationQuaternion1 = Quaternion.Euler(0f, -rotationAngle, 0f);
        Quaternion rotationQuaternion2 = Quaternion.Euler(0f, rotationAngle, 0f);

        Vector3 rotatedVector1 = rotationQuaternion1 * foward;
        Vector3 rotatedVector2 = rotationQuaternion2 * foward;

        rotatedVector1.Scale(new Vector3(m_ViewDistance, m_ViewDistance, m_ViewDistance));
        rotatedVector2.Scale(new Vector3(m_ViewDistance, m_ViewDistance, m_ViewDistance));

        Vector3 p1 = m_GameObject.transform.position + rotatedVector1;
        Vector3 p2 = m_GameObject.transform.position + rotatedVector2;


        Ray r1 = new Ray(m_GameObject.transform.position, rotatedVector1);
        if (Physics.Raycast(r1, out RaycastHit hitInfo1))
        {
            if (hitInfo1.distance < m_ViewDistance)
            {
                bool IsHitPlayer = (hitInfo1.transform.gameObject == m_PlayerObject);
                IsPlayerInFOV |= IsHitPlayer;

                bool isValidHit = IsValidFOVHitTarget(hitInfo1.transform.gameObject.layer);
                if (isValidHit) // Weird to clip cone to player I think
                {
                    p1 = hitInfo1.point;
                }
            }
        }
        Ray r2 = new Ray(m_GameObject.transform.position, rotatedVector2);
        if (Physics.Raycast(r2, out RaycastHit hitInfo2))
        {
            if (hitInfo2.distance < m_ViewDistance)
            {
                bool IsHitPlayer = (hitInfo2.transform.gameObject == m_PlayerObject);
                IsPlayerInFOV |= IsHitPlayer;

                bool isValidHit = IsValidFOVHitTarget(hitInfo2.transform.gameObject.layer);
                if (isValidHit) // Weird to clip cone to player I think
                {
                    p2 = hitInfo2.point;
                }
            }
        }

        m_LineRenderer.SetPosition(0, m_GameObject.transform.position);
        m_LineRenderer.SetPosition(1, p1);
        m_LineRenderer.SetPosition(2, p2);
        m_LineRenderer.SetPosition(3, m_GameObject.transform.position);
    }
}

