using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    public override bool IsInAttackRange(EnemyStateMachine stateMachine)
    {
        return Vector3.SqrMagnitude(stateMachine.enemy.transform.position - stateMachine.Target.transform.position) <= data.attackRange * data.attackRange;
    }

    public override void PerformAttack(EnemyStateMachine stateMachine)
    {
        Health playerHealth = stateMachine.Target.GetComponent<Health>();
        if (stateMachine.Target.TryGetComponent(out Companion companion))
        {
            companion.TakeDamage((float)stateMachine.enemy.data.attackDamage);
            
        }
        else if (stateMachine.Target.TryGetComponent(out Player player))
        {
            playerHealth.OnTakeDamage((int)stateMachine.enemy.data.attackDamage);
            Debug.Log($"Attacked player for {stateMachine.enemy.data.attackDamage} damage");
        }
       
    }

}
