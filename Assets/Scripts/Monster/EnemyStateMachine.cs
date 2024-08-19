using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyStateMachine : StateMachine
{
    public Enemy enemy;
    public GameObject Target { get; private set; } // 플레이어
    public List<Companion> AliveCompanions { get; private set; }
    public GameObject Player { get; private set; }
    public EnemyIdleState enemyIdleState {  get; private set; }  // 기본 상태가 추적상태 이게 플레이어 추적 chasing >> 일단 빼고 
    public EnemyRunState enemyRunState { get; private set; } // 범위 안에 발견하면 뛰고 
    public EnemyAttackState enemyAttackState { get; private set; } // 공격 범위 안에 들어오면 공격하기 
    public EnemyHitState enemyHitState { get; private set; }    
    public EnemyStateMachine(Enemy enemy)
    {
        this.enemy = enemy;
        Player = GameObject.FindGameObjectWithTag("Player");
        UpdateAliveCompanions();
        UpdateCurrentTarget();
        enemyIdleState = new EnemyIdleState(this);
        enemyRunState = new EnemyRunState(this);
        enemyAttackState = new EnemyAttackState(this);
        enemyHitState = new EnemyHitState(this);

        enemyAttackState.TypeRangeSet(this.enemy.EnemyTypeRange);
          
    }


    public void UpdateAliveCompanions()
    {
        AliveCompanions = new List<Companion>(CompanionManager.Instance.GetAliveCompanions());
    }

    public void UpdateCurrentTarget()
    {
        UpdateAliveCompanions();
        if (AliveCompanions.Count > 0)
        {
            Target = GetNearestCompanion().gameObject;
        }
        else
        {
            Target = Player;
        }
        
    }

    private Companion GetNearestCompanion()
    {
        return AliveCompanions.OrderBy(c => Vector3.Distance(enemy.transform.position, c.transform.position)).FirstOrDefault();
    }
    public void PeriodicTargetUpdate()
    {
        UpdateCurrentTarget();
    }


}
