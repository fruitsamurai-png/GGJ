using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Enemy
{
    public abstract void Start();
    public abstract void Update();

    // Gameplay enemy interface
    public abstract void NotifyDistraction(GameObject distraction);// - will be called by distractions the player activated.
    public abstract void Alert(GameObject alerter);// - will be called by other enemies when they are alerted to also alert this enemy.
    public abstract void Noise(float noiselevel);// - will be called by the player when nearby this enemy and making noise, refer to mechanics on what this does.
    public abstract void Jailbreak(int playerLevel, float stunTime);// - stuns the enemy for specified stun time if the playerLevel is >= enemy level.

    public StateMachine m_EnemyStateMachine;
}

