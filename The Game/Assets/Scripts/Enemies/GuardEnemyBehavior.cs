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
    
    // Probably better to have something like a blackboard.
    private Enemy m_Enemy;

    public GuardPatrollingState(StateMachine sm, GameObject go)
    {
        m_Go = go;
        m_Sm = sm;
        agent = go.GetComponent<NavMeshAgent>();

        painting1 = GameObject.Find("Painting");
        painting2 = GameObject.Find("Painting (1)");

        m_Enemy = m_Go.GetComponent<GuardEnemyBehavior>().m_Enemy;
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
            agent.speed = 1.0f * GuardEnemyBehavior.speedMult;

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

        if (m_Enemy.m_AlertLevel >= 1.0f)
        {
            m_Sm.ChangeState(new GuardAlertedState(m_Sm, m_Go, m_Go, m_Enemy));
        }

        else if (agent.remainingDistance <= 1.0f)
        {
            m_Sm.ChangeState(new GuardIdleState(m_Sm, m_Go));
        }
    }

    public override void OnExit()
    {

    }
}

// Should really be called "Pursuit State" I think..
public class GuardAlertedState : State
{
    public NavMeshAgent agent;
    public GameObject m_Alterer;
    private Enemy m_Enemy;

    public GameObject m_Player;

    private bool reachedAlerteredDestination = false;
    public GuardAlertedState(StateMachine sm, GameObject go, GameObject alterer, Enemy enemy)
    {
        m_Go = go;
        m_Sm = sm;
        agent = go.GetComponent<NavMeshAgent>();
        m_Alterer = alterer;
        m_Enemy = enemy;
    }
    public override void OnEnter()
    {
        float range = 10.0f;
        m_Enemy.AlertGuardsInVicinity(m_Alterer, range);

        // Maybe shouldn't path there
        if (m_Alterer != null)
        {
            agent.speed = 8.0f * GuardEnemyBehavior.speedMult;
            agent.SetDestination(m_Alterer.transform.position);
        }

        m_Enemy.m_AlertLevel = 1.0f; // lol
        m_Enemy.isAltered = true;
        m_Player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void OnUpdate()
    {
        if (!reachedAlerteredDestination)
        {
            if (agent.remainingDistance <= 0.5f)
            {
                reachedAlerteredDestination = true;
            }
            return;
        }

        if (reachedAlerteredDestination && m_Enemy.IsPlayerInFOV)
        {
            agent.speed = 4.0f * GuardEnemyBehavior.speedMult;
            agent.SetDestination(m_Player.transform.position);
        }
        else
        {
            m_Enemy.DecreaseAlertess();
            if (m_Enemy.m_AlertLevel == 0.0f)
            {
                m_Sm.ChangeState(new GuardPatrollingState(m_Sm, m_Go));
            }
        }
    }

    public override void OnExit()
    {
        m_Enemy.isAltered = false;
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
            agent.speed = 8.0f * GuardEnemyBehavior.speedMult;
            agent.SetDestination(m_Alterer.transform.position);
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
    public GuardEnemy(GameObject go, Material fovMaterial) 
        : base(go, fovMaterial)
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
        DrawFOVCone();

        if (m_IsStunned)
        {
            GuardEnemyBehavior.speedMult = 0.0f;
            m_StunnedDuration -= Time.deltaTime;
            m_StunnedDuration = Mathf.Max(0.0f, m_StunnedDuration);
            m_IsStunned = (m_StunnedDuration != 0.0f);
            if(!m_IsStunned)
            {
                GuardEnemyBehavior.speedMult = 1.0f;
            }
        }

        UpdateAlertness();
        m_EnemyStateMachine.Update();
    }
    public override void NotifyDistraction(GameObject distraction)
    {
        m_EnemyStateMachine.ChangeState(new GuardDistractedState(m_EnemyStateMachine, m_GameObject, distraction));
    }
    public override void Alert(GameObject alerter)
    {
        Enemy enemy = m_GameObject.GetComponent<GuardEnemyBehavior>().m_Enemy; // lol
        m_EnemyStateMachine.ChangeState(new GuardAlertedState(m_EnemyStateMachine, m_GameObject, alerter, enemy));
    }
}

public class GuardEnemyBehavior : MonoBehaviour
{
    public Material fovMaterial;
    public GuardEnemy m_Enemy;

    public static float speedMult = 1.0f;

	void Start()
    {
        m_Enemy = new GuardEnemy(gameObject, fovMaterial);
        m_Enemy.Start();
    }

    // Update is called once per frame
    void Update()
    {
        m_Enemy.Update();
    }

	public void NotifyDistraction(GameObject distraction)
	{
		Debug.Log($"Guard {gameObject.name} was notified of distraction {distraction.name}.");
		m_Enemy.NotifyDistraction(distraction);
	}
}
