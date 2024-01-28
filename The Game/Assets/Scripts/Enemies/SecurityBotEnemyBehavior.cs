using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityBotEnemy : GuardEnemy
{
    public bool InRange = false;
    public bool RayHit = false;

    public SecurityBotEnemy(GameObject go, EnemyAlertBar enemyAlertBar, GameObject fovGO)
        : base(go, enemyAlertBar, fovGO)
    {
    }

    private bool CheckIsPlayerInFOV()
    {
        Vector3 direction = (m_PlayerObject.transform.position - m_GameObject.transform.position);

        InRange = direction.sqrMagnitude < m_ViewDistance * m_ViewDistance;
        if (!InRange)
        {
            return false;
        }

        direction.Normalize();
        Ray ray = new Ray(m_GameObject.transform.position, direction);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            RayHit = m_PlayerObject == hitInfo.transform.gameObject;
            return RayHit;
        }

        return false;
    }

    public override void UpdateAlertness()
    {
        m_IsPlayerInFOV = CheckIsPlayerInFOV();

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
}

public class SecurityBotEnemyBehavior : MonoBehaviour
{
    public SecurityBotEnemy m_Enemy;
    GameObject childGameObject;
    public EnemyAlertBar alertBar;
    public GameObject fovPrefab;

    // FOV
    public float m_Fov = 360.0f;
    public float m_ViewDistance = 10.0f;

    // Alert
    public float m_AlertGraceElapsedTime = 0.0f;
    public float m_AlertIncreaseStep = 0.001f;
    public float m_AlertDecreaseStep = 0.0005f; // decrease half as fast as increase
    public float m_AlertGracePeriod = 2.0f;
    public float m_AlterRadius = 10.0f;

    // Speed
    public float m_PatrolSpeed = 1.0f;
    public float m_DistractedSpeed = 8.0f;
    public float m_PursuitSpeed = 12.0f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = Instantiate(fovPrefab, Vector3.zero, Quaternion.identity);
        m_Enemy = new SecurityBotEnemy(gameObject,alertBar, go);

        m_Enemy.m_Fov = m_Fov;
        m_Enemy.m_ViewDistance = m_ViewDistance;
        m_Enemy.m_AlertIncreaseStep = m_AlertIncreaseStep;
        m_Enemy.m_AlertDecreaseStep = m_AlertDecreaseStep;
        m_Enemy.m_AlertGracePeriod = m_AlertGracePeriod;
        m_Enemy.m_PatrolSpeed = m_PatrolSpeed;
        m_Enemy.m_DistractedSpeed = m_DistractedSpeed;
        m_Enemy.m_PursuitSpeed = m_PursuitSpeed;
        m_Enemy.m_AlterRadius = m_AlterRadius;

        m_Enemy.Start();
        // Get the child Transform
        Transform childTransform = gameObject.transform.GetChild(1);

        // Access the child GameObject
        childGameObject = childTransform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        m_Enemy.Update();

        if (m_Enemy.m_IsAlerted)
        {
            if (childGameObject.TryGetComponent(out TextBubble tex))
            {
                tex.TriggerTextSwitch(true);
            }
        }
        else
        {
            if (childGameObject.TryGetComponent(out TextBubble tex))
            {
                tex.TriggerTextSwitch(false);
            }
        }
    }
}
