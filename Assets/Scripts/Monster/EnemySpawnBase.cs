using UnityEngine;
using System.Collections.Generic;

public abstract class EnemySpawnBase : MonoBehaviour
{
    public List<Enemy> activeMonsters = new List<Enemy>(); // 현재 활성화된 몬스터 리스트

    public abstract void OnEnemyDeath(Enemy enemy);

}