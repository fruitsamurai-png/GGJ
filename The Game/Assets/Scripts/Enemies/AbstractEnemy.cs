using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Enemy
{
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
        m_LineRenderer.widthMultiplier = 0.4f;
        m_LineRenderer.material = fovMaterial;
    }

    public abstract void Start();
    public abstract void Update();

    // Gameplay enemy interface
    public abstract void NotifyDistraction(GameObject distraction);// - will be called by distractions the player activated.
    public abstract void Alert(GameObject alerter);// - will be called by other enemies when they are alerted to also alert this enemy.
    public abstract void Noise(float noiselevel);// - will be called by the player when nearby this enemy and making noise, refer to mechanics on what this does.
    public abstract void Jailbreak(int playerLevel, float stunTime);// - stuns the enemy for specified stun time if the playerLevel is >= enemy level.

    public void DrawFOVCone()
    {
        Vector3 foward = m_GameObject.transform.forward;

        float rotationAngle = m_Fov / 2.0f; 

        Quaternion rotationQuaternion1 = Quaternion.Euler(0f, -rotationAngle, 0f);
        Quaternion rotationQuaternion2 = Quaternion.Euler(0f, rotationAngle, 0f);

        Vector3 rotatedVector1 = rotationQuaternion1 * foward;
        Vector3 rotatedVector2 = rotationQuaternion2 * foward;

        rotatedVector1.Scale(new Vector3(m_ViewDistance, m_ViewDistance, m_ViewDistance));
        rotatedVector2.Scale(new Vector3(m_ViewDistance, m_ViewDistance, m_ViewDistance));

        m_LineRenderer.SetPosition(0, m_GameObject.transform.position);
        m_LineRenderer.SetPosition(1, m_GameObject.transform.position + rotatedVector1);
        m_LineRenderer.SetPosition(2, m_GameObject.transform.position + rotatedVector2);
        m_LineRenderer.SetPosition(3, m_GameObject.transform.position);

        m_ElapsedTime += Time.deltaTime;

        float r = MathF.Sin(m_ElapsedTime);
        float g = MathF.Cos(m_ElapsedTime);

        m_LineRenderer.material.color = new Color(r, g, r);
    }

    public StateMachine m_EnemyStateMachine;
    public GameObject m_GameObject;

    public LineRenderer m_LineRenderer;
    public float m_Fov = 45.0f;
    public float m_ViewDistance = 5.0f;

    // Test
    private float m_ElapsedTime = 0f;
}

