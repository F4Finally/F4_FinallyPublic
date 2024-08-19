using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    protected float lastAttackTime;
    protected Player player;
    bool isRange = false;

    public EnemyAttackState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
    {
  
    }

    public void TypeRangeSet(bool range)
    {
        isRange = range;
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.enemy.animationData.BaseAttackParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.enemy.animationData.BaseAttackParameterHash);
    }

    public override void Update()
    {
        base.Update();

        // 공격 범위에 있으면 공격 
        // 공격 범위에 있고 쿨다운이 끝났으면 공격
        if (isRange)
        {
            return;
        }

        if (Time.time >= lastAttackTime + stateMachine.enemy.data.attackSpeed)
        {
            stateMachine.enemy.PerformAttack(stateMachine);
            lastAttackTime = Time.time;
        }
    }
}
