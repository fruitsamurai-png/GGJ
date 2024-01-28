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
            m_MeshMaterial.color = Color.LerpUnclamped(m_OriginalMaterialColor, Color.red, m_AlertLevel);
        }
        UpdateAlertbar();
    }
}

public class SecurityBotEnemyBehavior : MonoBehaviour
{
    public Material fovMaterial;
    public SecurityBotEnemy m_Enemy;
    GameObject childGameObject;
    public EnemyAlertBar alertBar;
    public GameObject fovPrefab;
    // Start is called before the first frame update
    void Start()
    {
        GameObject go = Instantiate(fovPrefab, Vector3.zero, Quaternion.identity);
        m_Enemy = new SecurityBotEnemy(gameObject,alertBar, go);
        m_Enemy.m_Fov = 360.0f;
        m_Enemy.m_ViewDistance = 10.0f;
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

        if (m_Enemy.isAltered)
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
    //void OnGUI()
    //{
    //    GUI.TextArea(new Rect(10, 10, 250, 100), 
    //        "Alert: " + m_Enemy.m_AlertLevel + 
    //        "In range: " + m_Enemy.InRange +
    //        "Hit with ray: " + m_Enemy.RayHit);
    //}
}
