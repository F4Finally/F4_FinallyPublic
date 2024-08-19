using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyRunState : EnemyBaseState
{
    public EnemyRunState(EnemyStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.enemy.animationData.RunParameterHash);
   
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.enemy.animationData.RunParameterHash);
    }

    public override void Update()
    {
        base.Update();
        
        if (stateMachine.enemy.IsInAttackRange(stateMachine))
        {
            stateMachine.ChangeState(stateMachine.enemyAttackState);
        }
        else
        {
            stateMachine.enemy.IsInPlayerRange(stateMachine);
        }
    }

}
