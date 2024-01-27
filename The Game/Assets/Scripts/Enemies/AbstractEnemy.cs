using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public abstract class Enemy
{
    public StateMachine m_EnemyStateMachine;
    public GameObject m_GameObject;

    public LineRenderer m_LineRenderer;
    public float m_Fov = 30.0f;
    public float m_ViewDistance = 5.0f;

    public float m_AlertLevel = 0.0f;
    public readonly float m_AlertGracePeriod = 2.0f;
    public float m_AlertGraceElapsedTime = 0.0f;
    public float m_AlertStepAmount = 0.001f;
    public Material m_MeshMaterial;

    public GameObject m_PlayerObject;
    public bool IsPlayerInFOV = false;

    protected Enemy(GameObject gameObject, Material fovMaterial, GameObject alertLevelPrefab)
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
    }

    public abstract void Start();
    public abstract void Update();

    // Gameplay enemy interface
    public abstract void NotifyDistraction(GameObject distraction);// - will be called by distractions the player activated.
    public abstract void Alert(GameObject alerter);// - will be called by other enemies when they are alerted to also alert this enemy.
    public abstract void Noise(float noiselevel);// - will be called by the player when nearby this enemy and making noise, refer to mechanics on what this does.
    public abstract void Jailbreak(int playerLevel, float stunTime);// - stuns the enemy for specified stun time if the playerLevel is >= enemy level.


    public virtual void UpdateAlertness()
    {
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
    }

    public virtual void IncreaseAlertess(float amount)
    {
        m_AlertLevel += amount;
    }

    public virtual void DecreaseAlertess(float amount)
    {
        m_AlertLevel -= amount;
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
                p1 = hitInfo1.point;
                IsPlayerInFOV |= (hitInfo1.transform.gameObject == m_PlayerObject);
            }
        }
        Ray r2 = new Ray(m_GameObject.transform.position, rotatedVector2);
        if (Physics.Raycast(r2, out RaycastHit hitInfo2))
        {
            if (hitInfo2.distance < m_ViewDistance)
            {
                p2 = hitInfo2.point;
                IsPlayerInFOV |= (hitInfo2.transform.gameObject == m_PlayerObject);
            }
        }

        m_LineRenderer.SetPosition(0, m_GameObject.transform.position);
        m_LineRenderer.SetPosition(1, p1);
        m_LineRenderer.SetPosition(2, p2);
        m_LineRenderer.SetPosition(3, m_GameObject.transform.position);
    }
}

