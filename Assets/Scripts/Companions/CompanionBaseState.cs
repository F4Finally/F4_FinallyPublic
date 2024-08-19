using System.Numerics;
using UnityEngine;

public class CompanionBaseState : IState
{
    protected CompanionStateMachine stateMachine;

    public CompanionBaseState(CompanionStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void PhysicsUpdate()
    {
        // 스테이지 클리어 시 움직이게 
    }

    public virtual void Update()
    {

    }

    protected void StartAnimation(int animatorHash)
    {
        stateMachine.companion.Animator.SetBool(animatorHash, true);
    }

    protected void StopAnimation(int animatorHash)
    {
        stateMachine.companion.Animator.SetBool(animatorHash, false);
    }

    public void TriggerAnimation(int triggerHash)
    {
        stateMachine.companion.Animator.SetTrigger(triggerHash);
    }
}

public class CompanionIdleState : CompanionBaseState
{
    private float attackRange;
    public CompanionIdleState(CompanionStateMachine stateMachine) : base(stateMachine)
    {
        attackRange = stateMachine.companion.dataSO.attackRange;
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.companion.AnimData.IdleParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.companion.AnimData.IdleParameterHash);
    }


    float nowAttackDelay;
    float targetAttackDelay = 0.5f;
    public override void Update()
    {
        base.Update();
        nowAttackDelay += Time.deltaTime;
        if (nowAttackDelay < targetAttackDelay)
        {
            return;
        }
        Enemy targetEnemy = FindEnemy();
        if (targetEnemy != null)
        {

            nowAttackDelay = 0;
            stateMachine.ChangeState(stateMachine.companionAttackState);
        }

    }

    private Enemy FindEnemy()
    {
        float playerX = stateMachine.companion.transform.position.x;
        float maxX = playerX + attackRange;

        foreach (Enemy enemy in MainScene.Instance.returnNowActiveEnemies())
        {
            if (enemy == null) continue;
            float enemyX = enemy.transform.position.x;
            if (enemyX <= maxX)
            {
                return enemy; // 범위 내의 첫 번째 적을 반환

            }
        }
        return null; // 범위 내에 적이 없으면 null을 반환
    }
}

public class CompanionAttackState : CompanionBaseState
{
    protected Companion companion;

    private float attackRange;
    private bool hasAttacked;

    public CompanionAttackState(CompanionStateMachine stateMachine) : base(stateMachine)
    {
        companion = stateMachine.companion;
        attackRange = companion.dataSO.attackRange;
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.companion.AnimData.AttackParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.companion.AnimData.AttackParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if (companion.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") == true)
        {
            // 원하는 애니메이션이라면 플레이 중인지 체크
            float animTime = companion.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            if (hasAttacked == false)
            {

                if (animTime > 0.7f)
                {
                    hasAttacked = true;
                    PerformAttack();
                }
            }
            else
            {

                if (animTime < 0.7f)
                    hasAttacked = false;
                else if (animTime >= 0.9f)
                    stateMachine.ChangeState(stateMachine.companionIdleState);
            }

        }
    }

    // 몬스터가 나타날 때 attack상태로 
    public void PerformAttack()  // 1.애니메이션에 이벤트로 심어주기 , 2. idle 기본 > attack 할때만 attack 상태로 
    {
        // 평타 뎀지 구현
        Enemy targetEnemy = FindEnemy();
        if (targetEnemy != null)
        {
            float damage = companion.data.dataSO.baseStats.attack;
        
            targetEnemy.TakeDamage((BigInteger)damage);
        }

    }

    private Enemy FindEnemy()
    {
        float companionX = companion.transform.position.x;
        float maxX = companionX + attackRange;

        foreach (Enemy enemy in MainScene.Instance.returnNowActiveEnemies())
        {
            if (enemy == null) continue;
            float enemyX = enemy.transform.position.x;
            if (enemyX <= maxX)
            {
                return enemy;
            }
        }

        return null;
    }

}

public class CompanionDieState : CompanionBaseState
{
    protected Companion companion;

    public CompanionDieState(CompanionStateMachine stateMachine) : base(stateMachine)
    {
        companion = stateMachine.companion;
    }

    public override void Enter()
    {
        base.Enter();
        TriggerAnimation(stateMachine.companion.AnimData.DieParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (companion.Animator.GetCurrentAnimatorStateInfo(0).IsName("Die") == true)
        {
            // 원하는 애니메이션이라면 플레이 중인지 체크
            float animTime = companion.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;


            if (animTime >= 0.9f)
                CompanionManager.Instance.DespawnCompanionInGame(companion.dataSO.companionId);

        }


    }
}





