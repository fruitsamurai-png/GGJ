using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

using Debug = UnityEngine.Debug;

public abstract class State
{
    public StateMachine m_Sm;
    public GameObject m_Go;

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}

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

    public GuardPatrollingState(StateMachine sm, GameObject go)
    {
        m_Go = go;
        m_Sm = sm;
        agent = go.GetComponent<NavMeshAgent>();
    }

    public override void OnEnter()
    {
        GameObject painting1 = GameObject.Find("Painting");
        GameObject painting2 = GameObject.Find("Painting (1)");

        // Check if the GameObjects are found before accessing their transforms
        if (painting1 != null && painting2 != null)
        {
            // Access the Transform components
            Vector3 p1 = painting1.transform.position;
            Vector3 p2 = painting2.transform.position;

            Vector3 p = m_Go.GetComponent<Transform>().position;
            float sqrLen1 = (p1 - p).sqrMagnitude;
            float sqrLen2 = (p2 - p).sqrMagnitude;

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
        Debug.Log("Agent path status: " + agent.pathStatus + "Distance remaining: " + agent.remainingDistance);
        if (agent.remainingDistance <= 1.0f)
        {
            m_Sm.ChangeState(new GuardIdleState(m_Sm, m_Go));
        }
    }

    public override void OnExit()
    {
        Debug.Log("Guard patrolling state on exit!");
    }
}
public class StateMachine
{
    public void Update()
    {
        if (m_ActiveState != null)
        {
            m_ActiveState.OnUpdate();
        }
    }

    public void ChangeState(State newState)
    {
        if (m_ActiveState != null)
        {
            m_ActiveState.OnExit();
        }
        
        m_ActiveState = newState;

        if (m_ActiveState != null)
        {
            m_ActiveState.OnEnter();
        }
    }

    private State m_ActiveState;
}

public class EnemyBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        m_EnemyStateMachine = new StateMachine();
        m_EnemyStateMachine.ChangeState(new GuardPatrollingState(m_EnemyStateMachine, gameObject));
    }

    // Update is called once per frame
    void Update()
    {
        m_EnemyStateMachine.Update();
    }

    StateMachine m_EnemyStateMachine;
}
