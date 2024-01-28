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

		if (m_Go.TryGetComponent(out GuardEnemyBehavior geb))
		{
			m_Enemy = geb.m_Enemy;
		}
		else if (m_Go.TryGetComponent(out SecurityBotEnemyBehavior sbeb))
		{
			m_Enemy = sbeb.m_Enemy;
		}
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

		//Look for nearby paintings and see if they're stolen
		foreach (Collider c in Physics.OverlapSphere(m_Enemy.m_GameObject.transform.position, 10f, LayerMask.GetMask("Interactable")))
		{
			if (c.gameObject.TryGetComponent(out Artwork artwork))
			{
				bool inLos = true;
				Vector3 ePos = m_Enemy.m_GameObject.transform.position;
				if (Physics.Linecast(ePos, c.gameObject.transform.position, out RaycastHit h))
				{
					if (h.collider.gameObject != c.gameObject)
						inLos = false;
				}

				if (inLos && artwork.IsStolen)
				{
					Debug.DrawLine(m_Enemy.m_GameObject.transform.position, c.gameObject.transform.position, Color.red);
					if (artwork.artState == Artwork.ArtState.STOLEN || artwork.replicaLevel < artwork.artLevel)
					{
						m_Enemy.IncreaseAlertess();
					}
				}
			}
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

	float repathCooldown = 0f;
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

		repathCooldown = 0f;
	}

	public override void OnUpdate()
	{
		//if (!reachedAlerteredDestination)
		//{
		//	if (agent.remainingDistance <= 0.5f)
		//	{
		//		reachedAlerteredDestination = true;
		//	}
		//	return;
		//}

		//if (reachedAlerteredDestination && m_Enemy.m_IsPlayerInFOV)
		//{
		//	agent.speed = 4.0f * GuardEnemyBehavior.speedMult;
		//	agent.SetDestination(m_Player.transform.position);
		//}
		//else
		//{
		//	m_Enemy.DecreaseAlertess();
		//	if (m_Enemy.m_AlertLevel == 0.0f)
		//	{
		//		m_Sm.ChangeState(new GuardPatrollingState(m_Sm, m_Go));
		//	}
		//}

		agent.speed = 4.0f * GuardEnemyBehavior.speedMult;
		repathCooldown -= Time.deltaTime;
		if (repathCooldown <= 0f)
		{
			agent.SetDestination(m_Player.transform.position);
			repathCooldown = 0.2f;
		}

		if (!m_Enemy.m_IsPlayerInFOV)
		{
			m_Enemy.DecreaseAlertess();
		}
		if (m_Enemy.m_AlertLevel == 0.0f)
		{
			m_Sm.ChangeState(new GuardPatrollingState(m_Sm, m_Go));
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

	float distractedTime = 0f;
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

		if (distractedTime <= 0f)
		{
			if ((agent.destination - agent.gameObject.transform.position).sqrMagnitude < 2f)
			{
				distractedTime = 2f;
			}
		}
		else
		{
			distractedTime -= Time.deltaTime;
			if (distractedTime <= 0f)
			{
				m_Sm.ChangeState(new GuardPatrollingState(m_Sm, m_Go));
			}
		}
	}

	public override void OnExit()
	{

	}
}
public class GuardEnemy : Enemy
{
	public GuardEnemy(GameObject go, EnemyAlertBar enemyAlertBar, GameObject fovPrebInstance)
		: base(go, enemyAlertBar, fovPrebInstance)
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
		m_IsPlayerInFOV = false;
		DrawFOVCone();

		if (m_IsStunned)
		{
			GuardEnemyBehavior.speedMult = 0.0f;
			m_StunnedDuration -= Time.deltaTime;
			m_StunnedDuration = Mathf.Max(0.0f, m_StunnedDuration);
			m_IsStunned = (m_StunnedDuration != 0.0f);
			if (!m_IsStunned)
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
		Enemy enemy = null;

		if (m_GameObject.TryGetComponent(out GuardEnemyBehavior geb))
		{
			enemy = geb.m_Enemy;
		}

		if (m_GameObject.TryGetComponent(out SecurityBotEnemyBehavior sbeb))
		{
			enemy = sbeb.m_Enemy;
		}
		if (!enemy.isAltered)
		{
			m_EnemyStateMachine.ChangeState(new GuardAlertedState(m_EnemyStateMachine, m_GameObject, alerter, enemy));
		}
	}
}

public class GuardEnemyBehavior : MonoBehaviour
{
	public GuardEnemy m_Enemy;

	public EnemyAlertBar alertBar;
	public GameObject fovPrebInstance;

	public static float speedMult = 1.0f;

    public float m_Fov = 40.0f;
    public float m_ViewDistance = 5.0f;
    public float m_AlertIncreaseStep = 0.001f;
    public float m_AlertDecreaseStep = 0.0005f; // decrease half as fast as increase
    public float m_AlertGracePeriod = 2.0f;
    void Start()
	{
		GameObject newFovGO = Instantiate(fovPrebInstance,Vector3.zero,Quaternion.identity);
		m_Enemy = new GuardEnemy(gameObject, alertBar, newFovGO);
        m_Enemy.m_Fov = m_Fov;
        m_Enemy.m_ViewDistance = m_ViewDistance;
        m_Enemy.m_AlertIncreaseStep = m_AlertIncreaseStep;
        m_Enemy.m_AlertDecreaseStep = m_AlertDecreaseStep;
        m_Enemy.m_AlertGracePeriod = m_AlertGracePeriod;
        m_Enemy.Start();
	}

	// Update is called once per frame
	void Update()
	{
		m_Enemy.Update();
		if (m_Enemy.isAltered)
		{
			//if (childGameObject.TryGetComponent(out TextBubble tex))
			//{
			//	tex.TriggerTextSwitch(true);
			//}
		}
		else
		{
			//if (childGameObject.TryGetComponent(out TextBubble tex))
			//{
			//	tex.TriggerTextSwitch(false);
			//}
		}
	}

	public void NotifyDistraction(GameObject distraction)
	{
		Debug.Log($"Guard {gameObject.name} was notified of distraction {distraction.name}.");
		m_Enemy.NotifyDistraction(distraction);
	}
}
