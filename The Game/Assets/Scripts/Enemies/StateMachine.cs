using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public StateMachine m_Sm;
    public GameObject m_Go;

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
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
