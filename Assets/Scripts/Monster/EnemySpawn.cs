using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawn : EnemySpawnBase
{
    public ObjectPool objectPool;
    private float spawnInterval = 1.5f;
    private int maxMonstersAlive = 15;

    public List<string> enemyTags;
    public List<string> bossTags;
    private float timer;
    private Player player;

    private void OnEnable()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnStageStateChanged += OnStageStateChanged;
        }
        else
        {

        }
    }
    private void OnDisable()
    {
        StageManager.Instance.OnStageStateChanged -= OnStageStateChanged;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval && activeMonsters.Count < maxMonstersAlive) // 최대 10마리까지만 
        {
            if (StageManager.Instance.GetCurrentState() == StageState.Normal)
            {
                SpawnMonster();
            }
            else if (StageManager.Instance.GetCurrentState() == StageState.Boss)
            {
                SpawnBoss();
            }
            timer = 0f;
        }
    }

    public void SpawnMonster()
    {
        if (enemyTags.Count == 0)
            return;

        string randomTag = enemyTags[Random.Range(0, enemyTags.Count)];
        GameObject enemyObj = objectPool.SpawnFromPool(randomTag);
        if (enemyObj == null)
            return;

        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.SetSpawnManager(this);

        float statMultiplier = StageManager.Instance.GetStatMultiplier();

        float randomY = Random.Range(0.8f, 2.5f);
        Vector3 spawnPosition = new Vector3(2.6f, randomY,0);
        enemy.transform.position = spawnPosition;
        enemy.transform.localScale = Vector3.one * 3.5f;
        // 몬스터 초기화 및 상태 설정
        enemy.Init(statMultiplier);
        activeMonsters.Add(enemy);  
    }


    private void SpawnBoss()
    {
        if (enemyTags.Count == 0 || activeMonsters.Count > 0) // 1마리만
            return;

        string bossTag = bossTags[Random.Range(0, bossTags.Count)]; 
        GameObject bossObj = objectPool.SpawnFromPool(bossTag);
        if (bossObj == null)
            return;

        Enemy boss = bossObj.GetComponent<Enemy>();
        boss.SetSpawnManager(this);

        float statMultiplier = StageManager.Instance.GetStatMultiplier();

        float randomY = Random.Range(0.8f, 2.5f);
        Vector3 spawnPosition = new Vector3(2.6f, randomY, 0);
        boss.transform.position = spawnPosition;
        // 보스 크기를 2배로 증가
        boss.transform.localScale = Vector3.one * 8;
        boss.BossInit(statMultiplier);
        activeMonsters.Add(boss);
    }


    private void OnStageStateChanged(StageState stageState)
    {
        if (stageState == StageState.Normal)
        {
            foreach (var enemy in activeMonsters)
            {
            }
        }
        Debug.Log("되니?");
        // 다음 스테이지 넘어갈 때 모든 거 비활성화
        ClearActiveMonsters();
    }

    public override void OnEnemyDeath(Enemy enemy)
    {
        activeMonsters.Remove(enemy);
        StageManager.Instance.OnMonsterKilled();
    }

    public void ClearActiveMonsters()
    {
        foreach (var enemy in activeMonsters)
        {
            enemy.gameObject.SetActive(false);
        }
        activeMonsters.Clear();
    }

    public void RespawnMonsters()
    {
        ClearActiveMonsters();
        timer = 0f;
    }
}
