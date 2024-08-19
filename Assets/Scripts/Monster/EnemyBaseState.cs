using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseState : IState
{
    protected EnemyStateMachine stateMachine;
    protected Enemy enemy;
    protected GameObject target;

    //protected float detectionRange = 10f;
    //protected float attackRange = 10f;
    //protected float DetectionRange => detectionRange;
    //protected float AttackRange => attackRange;

    public EnemyBaseState(EnemyStateMachine enemyStateMachine)
    {
        stateMachine = enemyStateMachine;
        target = stateMachine.Target;
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void PhysicsUpdate()
    {
    }

    public virtual void Update()
    {

    }

    protected void StartAnimation(int animatorHash)
    {
        stateMachine.enemy.animator.SetBool(animatorHash, true);
    }

    protected void StopAnimation(int animatorHash)
    {
        stateMachine.enemy.animator.SetBool(animatorHash, false);
    }

    //public bool IsPlayerInRange()
    //{
    //    if (stateMachine.Target == null) return false;
    //    return Vector3.Distance(stateMachine.enemy.transform.position, stateMachine.Target.transform.position) <= detectionRange;
    //}



}
