using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityBotEnemy : GuardEnemy
{
    public SecurityBotEnemy(GameObject go, Material fovMaterial)
        : base(go, fovMaterial)
    {

    }

    private bool CheckIsPlayerInFOV()
    {
        Vector3 direction = (m_PlayerObject.transform.position - m_GameObject.transform.position);
        if (direction.sqrMagnitude > m_ViewDistance * m_ViewDistance)
        {
            return false;
        }

        direction.Normalize();
        Ray ray = new Ray(m_GameObject.transform.position, direction);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            return m_PlayerObject == hitInfo.transform.gameObject;
        }

        return false;
    }

    public override void UpdateAlertness()
    {
        m_IsPlayerInFOV = CheckIsPlayerInFOV();

        if (m_IsPlayerInFOV && !m_IsStunned)
        {
            IncreaseAlertess();
            if (m_AlertLevel >= 1.0f && !isAltered)
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
            m_MeshMaterial.color = Color.LerpUnclamped(Color.white, Color.red, m_AlertLevel);
        }
    }
}

public class SecurityBotEnemyBehavior : MonoBehaviour
{
    public Material fovMaterial;
    public SecurityBotEnemy m_Enemy;

    // Start is called before the first frame update
    void Start()
    {
        m_Enemy = new SecurityBotEnemy(gameObject, fovMaterial);
        m_Enemy.m_Fov = 360.0f;
        m_Enemy.m_ViewDistance = 10.0f;
        m_Enemy.Start();
    }

    // Update is called once per frame
    void Update()
    {
        m_Enemy.Update();
    }
}
