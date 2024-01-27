using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

using Debug = UnityEngine.Debug;

public class GuardIdleState : State
{
    private readonly float m_IdleDuration = 2.0f;
    private float m_IdleTime = 0.0f;

    public GuardIdleState(StateMachine sm, GameObject go)
    {
        m_Sm = sm;
        m_Go = go;
    }

    public override void OnEnter()
    {
        Debug.Log("Guard idle state on enter");
    }

    public override void OnUpdate()
    {
        m_IdleTime += Time.deltaTime;
        if (m_IdleTime >= m_IdleDuration)
        {
            m_Sm.ChangeState(new GuardPatrollingState(m_Sm, m_Go));
        }
    }

    public override void OnExit()
    {
        Debug.Log("Guard idle state on exit");
    }
}

public class GuardPatrollingState : State
{
    public NavMeshAgent agent;

    private GameObject painting1;
    private GameObject painting2;

    public GuardPatrollingState(StateMachine sm, GameObject go)
    {
        m_Go = go;
        m_Sm = sm;
        agent = go.GetComponent<NavMeshAgent>();

        painting1 = GameObject.Find("Painting");
        painting2 = GameObject.Find("Painting (1)");
    }

    public override void OnEnter()
    {
        if (painting1 != null && painting2 != null)
        {
            Vector3 p1 = painting1.transform.position;
            Vector3 p2 = painting2.transform.position;

            Vector3 p = m_Go.GetComponent<Transform>().position;
            float sqrLen1 = (p1 - p).sqrMagnitude;
            float sqrLen2 = (p2 - p).sqrMagnitude;
            agent.speed = 1.0f;

            // For now always path to the further of the two paintings
            if (sqrLen1 > sqrLen2)
            {
                agent.SetDestination(p1);
            }
            else
            {
                agent.SetDestination(p2);
            }
        }
    }

    public override void OnUpdate()
    {
        agent.speed = 1.0f * GuardEnemyBehavior.speedMult;

        if (agent.remainingDistance <= 1.0f)
        {
            m_Sm.ChangeState(new GuardIdleState(m_Sm, m_Go));
        }
    }

    public override void OnExit()
    {

    }
}

public class GuardAlertedState : State
{
    public NavMeshAgent agent;
    public GameObject m_Alterer;

    public GuardAlertedState(StateMachine sm, GameObject go, GameObject alterer)
    {
        m_Go = go;
        m_Sm = sm;
        agent = go.GetComponent<NavMeshAgent>();
        m_Alterer = alterer;
    }
    public override void OnEnter()
    {
        // Maybe shouldn't path there
        if (m_Alterer != null)
        {
            agent.speed = 8.0f;
            agent.SetDestination(m_Alterer.GetComponent<Transform>().transform.position);
        }
    }

    public override void OnUpdate()
    {
        // dk
    }

    public override void OnExit()
    {

    }
}
public class GuardDistractedState : State
{
    public NavMeshAgent agent;
    public GameObject m_Alterer;

    public GuardDistractedState(StateMachine sm, GameObject go, GameObject alterer)
    {
        m_Go = go;
        m_Sm = sm;
        agent = go.GetComponent<NavMeshAgent>();
        m_Alterer = alterer;
    }
    public override void OnEnter()
    {
        if (m_Alterer != null)
        {
            agent.speed = 8.0f;
            agent.SetDestination(m_Alterer.GetComponent<Transform>().transform.position);
        }
    }

    public override void OnUpdate()
    {
        agent.speed = 8.0f * GuardEnemyBehavior.speedMult;
    }

    public override void OnExit()
    {

    }
}
public class GuardEnemy : Enemy
{
    public GuardEnemy(GameObject go, Material fovMaterial, GameObject m_AlertLevelUIPrefab) 
        : base(go, fovMaterial, m_AlertLevelUIPrefab)
    {
        m_EnemyStateMachine = new StateMachine();
        m_GameObject = go;
    }
    public override void Start()
    {
        m_EnemyStateMachine.ChangeState(new GuardPatrollingState(m_EnemyStateMachine, m_GameObject));
    }

    public override void Update()
    {
        IsPlayerInFOV = false;
        m_EnemyStateMachine.Update();
        DrawFOVCone();
        UpdateAlertness();
    }
    public override void NotifyDistraction(GameObject distraction)
    {
        m_EnemyStateMachine.ChangeState(new GuardDistractedState(m_EnemyStateMachine, m_GameObject, distraction));
    }
    public override void Alert(GameObject alerter)
    {
        m_EnemyStateMachine.ChangeState(new GuardAlertedState(m_EnemyStateMachine, m_GameObject, alerter));
    }
    public override void Noise(float noiselevel)
    {

    }
    public override void Jailbreak(int playerLevel, float stunTime)
    {

    }
}


public class GuardEnemyBehavior : MonoBehaviour
{
    public GameObject m_AlertLevelUI;
    public Material fovMaterial;
    public GuardEnemy m_Enemy;

    public static float speedMult = 1.0f;

	void Start()
    {
        m_Enemy = new GuardEnemy(gameObject, fovMaterial, m_AlertLevelUI);
        m_Enemy.Start();
    }

    // Update is called once per frame
    void Update()
    {
        m_Enemy.Update();
        m_Enemy.DrawFOVCone();
        m_Enemy.UpdateAlertness();
        //string alertLevel = "Guard alert: " + m_Enemy.m_AlertLevel;
        //text.SetText(alertLevel);
    }

	public void NotifyDistraction(GameObject distraction)
	{
		Debug.Log($"Guard {gameObject.name} was notified of distraction {distraction.name}.");
		m_Enemy.NotifyDistraction(distraction);
	}
}
