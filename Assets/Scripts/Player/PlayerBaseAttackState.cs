using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerBaseAttackState : PlayerBaseState
{
    protected float attackCount = 0.1f;
    protected float ultimate = 1f; // 슬라이더 1일 때 궁게이지 0 <> 슬라이더 0일 때 궁사용가능
    protected Player player;
    protected float minAttackRange = 0.3f;
    protected float maxAttackRange = 5f;  // 2.5
    private bool hasAttacked;
    private bool animationFinished; // 애니메이션이 끝났는지 체크하는 플래그


    public PlayerBaseAttackState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        player = stateMachine.player;
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.player.animationData.BaseAttackParameterHash);
        animationFinished = false;
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.player.animationData.BaseAttackParameterHash);
        hasAttacked = false;
        animationFinished = false;
    }

    public override void Update()
    {
        base.Update();

        if (player.animator.GetCurrentAnimatorStateInfo(0).IsName("attack") == true)
        {
            // 원하는 애니메이션이라면 플레이 중인지 체크
            float animTime = player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            animTime -= (int)animTime;
            if (hasAttacked == false)
            {
                if (animTime >= 0.5f)
                {
                    hasAttacked = true;
                    PerformAttack();
                }
            }
            else
            {
                if (animTime <= 0.5f)
                    hasAttacked = false;
            }
        }

        // 애니메이션 시작 타이밍에 애니메이션 시간 이후에 데미지가 들어가게 코루틴 써서 피격 되도 데미지가 입게 

        // 애니메이션이 완료되었는지 확인
        if (player.animator.GetCurrentAnimatorStateInfo(0).IsName("attack") && player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !animationFinished)
        {
            animationFinished = true;
            player.stateMachine.ChangeState(stateMachine.playerIdleState);
        }
    }

    // 몬스터가 나타날 때 attack상태로 
    public void PerformAttack()  // 1.애니메이션에 이벤트로 심어주기 , 2. idle 기본 > attack 할때만 attack 상태로 
    {
        player.UltimateGauge -= attackCount; // 이전 프레임부터 지금 프레임까지 걸린 시간을 곱해서 같은 속도를 유지하게

        if (player.UltimateGauge <= 0f)
        {
            player.UltimateGauge = ultimate; // 게이지 최대로 리셋
            stateMachine.ChangeState(stateMachine.playerUltimateAttackState); // 궁극기 상태로 변환
        }
        else
        {
            //// 평타 뎀지 구현
            //Enemy targetEnemy = FindEnemy();
            //if (targetEnemy != null)
            //{
            //    BigInteger damage = player.BaseAttack;
            //    targetEnemy.TakeDamage(damage);
            //    Debug.Log("플레이어 평타");
            //}
            Enemy targetEnemy = FindEnemy();
            if (targetEnemy != null)
            {
                BigInteger damage = player.BaseAttack;
                bool damageApplied = targetEnemy.TakeDamage(damage);
                if (damageApplied)
                {

                }
                else
                {
                    // 데미지가 적용되지 않았다면 (적이 이미 죽어있다면) 다른 적을 찾아 공격
                    targetEnemy = FindEnemy();
                    if (targetEnemy != null)
                    {
                        targetEnemy.TakeDamage(damage);
                    }
                }
            }

            player.UpdateUltimateGaugeUI();
        }
    }


    public Enemy FindEnemy()
    {
        float playerX = player.transform.position.x;
        float minX = playerX + minAttackRange;
        float maxX = playerX + maxAttackRange;
        Enemy closestEnemy = null;
        float closestDistance = float.MaxValue; // 초기에는 가장 큰 값을 설정하여 최소 거리로 비교

        foreach (Enemy enemy in MainScene.Instance.returnNowActiveEnemies())
        {
            if (enemy == null || enemy.CurrentHealth <= 0) continue;

            float enemyX = enemy.transform.position.x;
            if (enemyX >= minX && enemyX <= maxX)
            {
                float distanceSqr = (playerX - enemyX) * (playerX - enemyX);
                if (distanceSqr < closestDistance)
                {
                    closestDistance = distanceSqr;
                    closestEnemy = enemy;
                }
            }
        }
        return closestEnemy; // 범위 내에 적이 없으면 null을 반환
    }



}
