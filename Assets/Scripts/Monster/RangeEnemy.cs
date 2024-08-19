using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemy : Enemy
{
    public GameObject projectilePrefab; // 투사체 프리팹
    
    

    public override bool IsInAttackRange(EnemyStateMachine stateMachine)
    {
        return Vector3.SqrMagnitude(stateMachine.enemy.transform.position - stateMachine.Target.transform.position) <= data.attackRange * data.attackRange;
    }

    public override void PerformAttack(EnemyStateMachine stateMachine)
    {
        // 투사체 생성 및 발사
        GameObject projectile = Instantiate(projectilePrefab, stateMachine.enemy.transform.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.Initialize(stateMachine);
        }
    }

    public void Attack()
    {
        // 투사체 생성 및 발사
        GameObject projectile = Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);
        projectile.transform.parent = this.transform;

        Projectile projectileScript = projectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.Initialize(stateMachine);
        }
    }


    public override void IsInPlayerRange(EnemyStateMachine stateMachine)
    {
        base.IsInPlayerRange(stateMachine);
        float distanceToPlayer = Vector3.Distance(transform.position, stateMachine.Target.transform.position);

        if (distanceToPlayer < data.attackRange)
        {
            // 공격 상태로 전환
            stateMachine.ChangeState(stateMachine.enemyAttackState);
        }
 
    }
}
