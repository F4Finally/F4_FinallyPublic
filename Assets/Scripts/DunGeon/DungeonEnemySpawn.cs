using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEnemySpawn : EnemySpawnBase
{
    public ObjectPool objectPool;
    private float spawnInterval = 2f;
    private int maxMonstersAlive = 10;

   
    public List<string> dungeonEnemyTags; // 던전에서 스폰할 적의 태그 리스트
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval && activeMonsters.Count < maxMonstersAlive)
        {
            SpawnDungeonMonster();
            timer = 0f;
        }
    }

    public void SpawnDungeonMonster()
    {
        if(dungeonEnemyTags.Count == 0 || activeMonsters.Count > 0) // 1마리만
            return;

        string randomTag = dungeonEnemyTags[Random.Range(0, dungeonEnemyTags.Count)];
        GameObject enemyObj = objectPool.SpawnFromPool(randomTag);
        if (enemyObj == null)
            return;

        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.SetSpawnManagerDungeon(this);

        int statMultiplier = 2; // 던전 전용 스탯 배수
        float randomY = Random.Range(0.8f, 2.5f);
        Vector3 spawnPosition = new Vector3(2.6f, randomY, 0); // 던전 전용 위치 설정
        enemy.transform.position = spawnPosition;

        // 몬스터 초기화 및 상태 설정
        enemy.Init(statMultiplier);
        activeMonsters.Add(enemy);
    }

    public override void OnEnemyDeath(Enemy enemy)
    {
        activeMonsters.Remove(enemy);
    }


    public void ClearActiveMonsters()
    {
        foreach (var enemy in activeMonsters)
        {
            enemy.gameObject.SetActive(false);
        }
        activeMonsters.Clear();
    }
}
