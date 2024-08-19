using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    private EnemyStateMachine stateMachine;
    private Vector3 direction;
    public void Initialize(EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;

        // 발사 방향 설정
        direction = (stateMachine.Target.transform.position - stateMachine.enemy.transform.position).normalized;
    }

    private void Update()
    {
        if (stateMachine.Target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, stateMachine.Target.transform.position) < 0.5f)
        {
            Hit();
        }
    }

    public void Hit()
    {
        Health playerHealth = stateMachine.Target.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.OnTakeDamage((int)stateMachine.enemy.data.attackDamage);
        }
        Destroy(gameObject);
    }
}


public class CompanionRangeProjectile : MonoBehaviour
{
    public float speed = 5f;
    private CompanionStateMachine stateMachine;
    private Vector3 direction;
   /* public void Initialize(CompanionStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;

        // 발사 방향 설정
        direction = (stateMachine.Target.transform.position - stateMachine.enemy.transform.position).normalized;
    }

    private void Update()
    {
        if (stateMachine.Target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, stateMachine.Target.transform.position) < 0.5f)
        {
            Hit();
        }
    }

    public void Hit()
    {
        Health playerHealth = stateMachine.Target.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.OnTakeDamage((int)stateMachine.enemy.data.attackDamage);
        }
        Destroy(gameObject);
    }*/
}
