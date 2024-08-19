using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    protected float minAttackRange = 0.3f;
    protected float maxAttackRange = 5f;
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.player.animationData.IdleParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.player.animationData.IdleParameterHash);
    }

    public override void Update()
    {
        base.Update();
        Enemy targetEnemy = FindEnemy();
        if(targetEnemy != null)
        {
            stateMachine.ChangeState(stateMachine.playerBaseAttackState);
        }
    }

    private Enemy FindEnemy()
    {
        float playerX = stateMachine.player.transform.position.x;
        float minX = playerX + minAttackRange;
        float maxX = playerX + maxAttackRange;

        foreach (Enemy enemy in MainScene.Instance.returnNowActiveEnemies())
        {
            if (enemy == null) continue;
            float enemyX = enemy.transform.position.x;
            if (enemyX >= minX && enemyX <= maxX)
            {
                return enemy; // 범위 내의 첫 번째 적을 반환
            }
        }
        return null; // 범위 내에 적이 없으면 null을 반환
    }
}
