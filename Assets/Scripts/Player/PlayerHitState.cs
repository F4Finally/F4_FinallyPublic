using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerHitState : PlayerBaseState
{
    private float hitStateDuration = 0.5f; // 지속 시간
    private float timer;
    public PlayerHitState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.player.animationData.HitParameterHash);
        timer = 0f;
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.player.animationData.HitParameterHash);
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer >= hitStateDuration)
        {
            stateMachine.ChangeState(stateMachine.playerBaseAttackState);
        }
    }
}
