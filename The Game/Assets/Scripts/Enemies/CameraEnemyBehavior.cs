using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Debug = UnityEngine.Debug;

public class CameraIdleState : State
{
    //private readonly float m_IdleDuration = 2.0f;
    //private float m_IdleTime = 0.0f;

    public CameraIdleState(StateMachine sm, GameObject go)
    {
        m_Sm = sm;
        m_Go = go;
    }

    public override void OnEnter()
    {

    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {

    }
}

public class CameraPatrollingState : State
{
    private float m_ElapsedTime = 0.0f;

    private readonly float m_DurationBeforeSwitching = 0.5f;
    private readonly float m_RotationOffset = 90.0f;

    private Vector3[] m_FowardVectors;
    private int m_AngleIndex = 0;

    private Vector3 m_InitialFoward;

    private CameraEnemy m_CameraEnemy; // lol
    public CameraPatrollingState(StateMachine sm, GameObject go, Vector3 forward)
    {
        m_Go = go;
        m_Sm = sm;
        m_FowardVectors = new Vector3[3];
        m_InitialFoward = forward;
    }

    public override void OnEnter()
    {
        m_AngleIndex = 0;

        for(int i = -1; i < 2; i++)
        {
            float rotationAngle = (m_RotationOffset * i) / 2.0f;
            Quaternion rotationQuaternion1 = Quaternion.Euler(0f, rotationAngle, 0f);
            Vector3 rotatedVector1 = rotationQuaternion1 * m_InitialFoward;
            m_FowardVectors[i + 1] = rotatedVector1;
        }
        m_Go.transform.forward = m_FowardVectors[m_AngleIndex];

        m_CameraEnemy = m_Go.GetComponent<CameraEnemyBehavior>().m_Enemy;
    }

    public override void OnUpdate()
    {
        if (!m_CameraEnemy.IsPlayerInFOV)
        {
            m_ElapsedTime += Time.deltaTime;
            if (m_ElapsedTime >= m_DurationBeforeSwitching)
            {
                m_ElapsedTime = 0.0f;
                ChangeAngle();
            }
        }
        else
        {
            m_ElapsedTime = 0.0f;
        }

    }

    public override void OnExit()
    {

    }

    private void ChangeAngle()
    {
        ++m_AngleIndex;
        m_AngleIndex = m_AngleIndex % 3;
        m_Go.transform.forward = m_FowardVectors[m_AngleIndex];
    }
}

public class CameraAlertedState : State
{
    public GameObject m_Alterer;

    private GameObject m_PlayerObject;
    private Enemy m_Enemy;

    
    public CameraAlertedState(StateMachine sm, GameObject go, GameObject alterer, Enemy enemy)
    {
        m_Go = go;
        m_Sm = sm;
        m_Alterer = alterer;
        m_Enemy = enemy;
    }
    public override void OnEnter()
    {
        // Does not pursue the player when alerted,
        // but has a large radius of notifying nearby guards when alerted.
        float range = 40.0f;
        m_Enemy.AlertGuardsInVicinity(m_Alterer, range);
        m_PlayerObject = GameObject.FindWithTag("Player");
    }

    public override void OnUpdate()
    {
        // Just keep looking at player?
        m_Go.transform.LookAt(m_PlayerObject.transform);
    }

    public override void OnExit()
    {

    }
}
public class CameraDistractedState : State
{
    private GameObject m_Alterer;
    private float m_ElapsedTime = 0.0f;
    private readonly float m_DistractedDuration = 5.0f;
    public CameraDistractedState(StateMachine sm, GameObject go, GameObject alterer)
    {
        m_Go = go;
        m_Sm = sm;
        m_Alterer = alterer;
    }
    public override void OnEnter()
    {
        m_Go.transform.LookAt(m_Alterer.transform);
    }

    public override void OnUpdate()
    {
        m_ElapsedTime += Time.deltaTime;
        if (m_ElapsedTime > m_DistractedDuration)
        {
            CameraEnemy cameraEnemy = m_Go.GetComponent<CameraEnemyBehavior>().m_Enemy;
            m_Sm.ChangeState(new CameraAlertedState(m_Sm, m_Go, m_Go, cameraEnemy));
        }
    }

    public override void OnExit()
    {

    }
}
public class CameraEnemy : Enemy
{
    public Vector3 m_InitialFoward;

    public CameraEnemy(GameObject go, Material fovMaterial) 
        : base(go, fovMaterial)
    {
        m_EnemyStateMachine = new StateMachine();
        m_GameObject = go;
        m_InitialFoward = m_GameObject.transform.forward;
    }
    public override void Start()
    {
        m_EnemyStateMachine.ChangeState(new CameraPatrollingState(m_EnemyStateMachine, m_GameObject, m_InitialFoward));
    }

    public override void Update()
    {
        IsPlayerInFOV = false;
        DrawFOVCone();
        UpdateAlertness();
        m_EnemyStateMachine.Update();
    }
    public override void NotifyDistraction(GameObject distraction)
    {
        Vector3 distractionSource = distraction.transform.position;
        float squaredViewDistance = m_ViewDistance * m_ViewDistance;

        if ((distractionSource - m_GameObject.transform.position).sqrMagnitude > squaredViewDistance)
        {
            m_EnemyStateMachine.ChangeState(new CameraDistractedState(m_EnemyStateMachine, m_GameObject, distraction));
        }
    }
    public override void Alert(GameObject alerter)
    {
        m_EnemyStateMachine.ChangeState(new CameraAlertedState(m_EnemyStateMachine, m_GameObject,
            alerter, m_GameObject.GetComponent<CameraEnemyBehavior>().m_Enemy));
    }
    public override void Noise(float noiselevel)
    {

    }
    public override void Jailbreak(int playerLevel, float stunTime)
    {

    }
}

public class CameraEnemyBehavior : MonoBehaviour
{
    public Material fovMaterial;

    public CameraEnemy m_Enemy;
    void Start()
    {
        m_Enemy = new CameraEnemy(gameObject, fovMaterial);
        m_Enemy.Start();
    }

    // Update is called once per frame
    void Update()
    {
        m_Enemy.Update();
    }

}
