using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PlayerUltimateAttackState : PlayerBaseState
{

    protected float ultimateDuration = 3f; // 궁극기 지속 시간
    protected float ultimateTimer;
    protected Player player;
    protected float minAttackRange = 0.3f;
    protected float maxAttackRange = 5f;
    protected EnemySpawn enemySpawn;
    private WaitForSeconds delay;

    public PlayerUltimateAttackState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        player = stateMachine.player;

    }

    public override void Enter()
    {
        base.Enter();
        PerformUltimateAttack();
        StartAnimation(stateMachine.player.animationData.UltimateAttackParameterHash);
        ultimateTimer = 0f;
        delay = new WaitForSeconds(2f);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.player.animationData.UltimateAttackParameterHash);
    }

    public override void Update()
    {
        base.Update();
        ultimateTimer += Time.deltaTime;

        if (ultimateTimer >= ultimateDuration)
        {
            stateMachine.ChangeState(stateMachine.playerIdleState);
        }
    }

    private void PerformUltimateAttack()
    {
        // 플레이어의 현재 위치
        UnityEngine.Vector2 playerPosition = stateMachine.player.transform.position;

        UnityEngine.Vector2 forwardDirection = stateMachine.player.transform.forward;

        // x축으로 이동할 오프셋
        //float xOffset = 2.0f;
        //UnityEngine.Vector2 spawnPosition = playerPosition + forwardDirection + new UnityEngine.Vector2(xOffset, 0);

        //GameObject ultimateEffect = Object.Instantiate(stateMachine.player.ultimateEffectPrefab, spawnPosition, UnityEngine.Quaternion.identity); // 궁극기 이펙트 생성 >> 파티클로.
        GameObject ultimateEffect = stateMachine.player.ultimateEffectPrefab;
        stateMachine.player.StartCoroutine(DeactivateEffectAfterDelay(ultimateEffect, 3f));

    }

    private IEnumerator DeactivateEffectAfterDelay(GameObject effect, float delay)
    {

       effect = stateMachine.player.ultimateEffectPrefab;
       effect.SetActive(true);

        // 뎀지 로직 추가 
        BigInteger ultimateDamage = 100;
        List<Enemy> enemiesInRange = FindEnemies();
        foreach (Enemy enemy in enemiesInRange)
        {
            // 적에게 데미지 적용
            enemy.TakeDamage(ultimateDamage);
        }

        yield return new WaitForSeconds(delay);

        effect.SetActive(false);
    }

    private List<Enemy> FindEnemies()
    {

        float playerX = player.transform.position.x;
        float minX = playerX + minAttackRange;
        float maxX = playerX + maxAttackRange;


        List<Enemy> enemiesInRange = new List<Enemy>();

        foreach (Enemy enemy in MainScene.Instance.returnNowActiveEnemies())
        {
            float enemyX = enemy.transform.position.x;
            if (enemyX >= minX && enemyX <= maxX)
            {
                enemiesInRange.Add(enemy);
            }
        }

        return enemiesInRange;
    }

}
