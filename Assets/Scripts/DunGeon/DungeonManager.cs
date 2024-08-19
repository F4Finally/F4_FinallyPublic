using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;

[System.Serializable]
public class DungeonInfo
{
    public string name;
    public int CurrentLevel = 1;
    public int MaxLevel = 15; // 난이도 최고 
    public int keyCount = 3;
    public int maxKeyCount = 3;
    public DateTime lastKeyUsageTime;
}
public class DungeonManager : Singleton<DungeonManager>
{
    public float TimeLimit = 60f;
    private float remainingTime;
    [SerializeField] private DungeonEnemySpawn dungeonEnemySpawn;
    [SerializeField] private EnemySpawn enemySpawn;
    private DungeonInfo currentDungeon;
    [SerializeField] private List<DungeonInfo> dungeons = new List<DungeonInfo>();
    private int currentDungeonIndex = 0;
    public bool isDungeonActive = false;
    private int monstersKilled = 0;
    private int totalCoins = 0;

    public event Action<int, int> OnDungeonComplete; // 스테이지, 획득 코인
    public event Action<int> OnMonsterKilled; // 현재 처치한 몬스터 수

    [field: Header("UI")]
    [SerializeField] private GameObject dungeonSelectPopup; // 던전 난이도 고르는 팝업 
    [SerializeField] private GameObject combatCanvas;  // 던전 들어가서 남은 시간 뜨는 팝업
    [SerializeField] private GameObject rewardPopup; // 던전 깼을 때 보상 팝업
    [SerializeField] private GameObject questUI; // 퀘스트 UI 
    [SerializeField] private TextMeshProUGUI timeText; // 남은 시간
    [SerializeField] private TextMeshProUGUI selectRewardText; // 선택했을 때 보상 텍스트
    [SerializeField] private TextMeshProUGUI rewardText; // 던전 보상 텍스트 
    [SerializeField] private GameObject stage; // 메인 스테이지바 UI 
    [SerializeField] private GameObject dungeonStage; // 던전 스테이지 UI
    [SerializeField] private TextMeshProUGUI dungeonStageText; // 던전 레벨
    [SerializeField] private TextMeshProUGUI progressText; // 던전 진행도 


    private void Start()
    {
        currentDungeon = dungeons[currentDungeonIndex];
        currentDungeon.CurrentLevel = GameManager.Instance.CurplayerData.DungeonLevel;
        dungeonEnemySpawn.enabled = false;
        selectRewardText.text = (CalculateCoinsFromMonster() * 10).ToString();
        dungeonStageText.text = $"난이도 {(currentDungeon.CurrentLevel * 10).ToString()}";
    }

    private void Update()
    {
        if (isDungeonActive)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimeText(remainingTime);

            // 시간이 끝나거나 몬스터 10마리 죽이면 던전 끝
            if (remainingTime <= 0 || monstersKilled >= 10)
            {
                EndDungeon();
                isDungeonActive = false;
            }
            // 몬스터가 0마리면 1마리만 생성
            else if (dungeonEnemySpawn.activeMonsters.Count == 0)
            {
                SpawnMonster();
            }
        }

        UpdateRewardText();
        progressText.text = $"{monstersKilled}/{10}";
    }

    public void StartDungeon()
    {
        if (!CanEnterDungeon())
        {
            return;
        }

        if (GameManager.Instance.CurplayerData.UseDungeonKey())
        {
            isDungeonActive = true;
            monstersKilled = 0;
            totalCoins = 0;
            remainingTime = TimeLimit;
            // 던전 캔버스 활성화 , 던전 select 팝업 비활성화
            dungeonSelectPopup.SetActive(false);
            questUI.SetActive(false);
            stage.SetActive(false);

            dungeonStageText.text = $"난이도 {currentDungeon.CurrentLevel.ToString()}";
            combatCanvas.SetActive(true);
            dungeonStage.SetActive(true);
            // 기존 전투 몬스터 클리어
            enemySpawn.enabled = false;
            dungeonEnemySpawn.enabled = true;
            enemySpawn.ClearActiveMonsters();
            SpawnMonster(); // 몬스터 소환

            // 던전 입장 퀘스트
            foreach (var quest in QuestManager.Instance.GetActiveQuests())
            {
                if (quest.Type == QuestType.ETC && quest.subType == SubType.Dungeon)
                {
                    QuestManager.Instance.UpdateQuestProgress(quest.ID, 1);
                }
            }
            GameManager.Instance.SaveGame();
        }
    }
    private void SpawnMonster()
    {
        dungeonEnemySpawn.SpawnDungeonMonster();
        Enemy monster = dungeonEnemySpawn.activeMonsters[0]; // 1마리씩 나올거니까
        AdjustMonsterStats(monster);
    }

    private void AdjustMonsterStats(Enemy monster)
    {
        float multiplier = 1 + (currentDungeon.CurrentLevel - 1) * 0.1f;
        monster.enemyData.maxHealth = monster.enemyData.maxHealth * (int)multiplier;
        monster.enemyData.attackDamage = monster.enemyData.attackDamage * (int)multiplier;
        monster.CurrentHealth = monster.enemyData.maxHealth;
    }

    public DungeonInfo GetCurrentDungeonInfo()
    {
        return dungeons[currentDungeonIndex];
    }

    public void OnMonsterDefeated(Enemy enemy)
    {
        monstersKilled++;
        int coinsFromMonster = CalculateCoinsFromMonster();
        totalCoins += coinsFromMonster;
        OnMonsterKilled?.Invoke(monstersKilled);

        if (monstersKilled >= 10)
        {
            EndDungeon();
        }
    }

    private void EndDungeon()
    {
        if (!isDungeonActive) // 던전이 이미 끝났다면 다시 실행되지 않도록 방지
            return;
        isDungeonActive = false;
        // 던전 몬스터 모두 제거 
        dungeonEnemySpawn.ClearActiveMonsters();
        remainingTime = TimeLimit;
        // 메인 캔버스로 전환 
        combatCanvas.SetActive(false);
        dungeonStage.SetActive(false);
        questUI.SetActive(true);
        stage.SetActive(true);
        enemySpawn.enabled = true;
        dungeonEnemySpawn.enabled = false;

        GameManager.Instance.CurplayerData.Coin += totalCoins;
        // 보상 팝업
        RewardDungeon();
        // 잡은 몬스터가 10마리가 넘으면 
        if (monstersKilled >= 10)
        {
            AdvanceDungeonStage();
        }
        OnDungeonComplete?.Invoke(currentDungeon.CurrentLevel, totalCoins);
        GameManager.Instance.SaveGame();
    }

    private int CalculateCoinsFromMonster()
    {
        return (int)(10000 * (1 + (currentDungeon.CurrentLevel - 1) * 0.4f));
    }

    private void AdvanceDungeonStage()
    {

        if (dungeons[currentDungeonIndex].CurrentLevel < dungeons[currentDungeonIndex].MaxLevel)
        {
            dungeons[currentDungeonIndex].CurrentLevel++;
            GameManager.Instance.CurplayerData.DungeonLevel = dungeons[currentDungeonIndex].CurrentLevel;
            GameManager.Instance.SaveGame();
        }
    }

    public bool CanSweep()
    {
        return currentDungeon.CurrentLevel > 1;
    }

    public void SweepDungeon()
    {
        //if (dungeons[currentDungeonIndex].CurrentLevel >= dungeons[currentDungeonIndex].MaxLevel)
        //{
        //    return;
        //}

        if (GameManager.Instance.CurplayerData.UseDungeonKey())
        {
            int sweepRewards = CalculateCoinsFromMonster() * 10;
            totalCoins = sweepRewards;
            GameManager.Instance.CurplayerData.Coin += sweepRewards;
            RewardDungeon();
            OnDungeonComplete?.Invoke(currentDungeon.CurrentLevel, totalCoins);
            GameManager.Instance.SaveGame();
        }
    }

    private int CalculateSweepRewards()
    {
        return currentDungeon.CurrentLevel * 100;
    }

    public void ResetDungeonStageUI()
    {
        currentDungeon.CurrentLevel = GameManager.Instance.CurplayerData.DungeonLevel;
        UpdateRewardText();
    }

    public void UpdateRewardText()
    {
        //selectRewardText.text = (CalculateCoinsFromMonster() * 10).ToString();
        selectRewardText.text = BigIntegerUtils.FormatBigInteger(CalculateCoinsFromMonster() * 10);
    }

    public bool CanEnterDungeon()
    {
        return GameManager.Instance.CurplayerData.DungeonKeys > 0;
    }


    public void IncreaseDungeonLevel()
    {
        if (dungeons[currentDungeonIndex].CurrentLevel < dungeons[currentDungeonIndex].MaxLevel)
        {
            dungeons[currentDungeonIndex].CurrentLevel++;
        }

        UpdateRewardText();
    }

    public void DecreaseDungeonLevel()
    {
        if (dungeons[currentDungeonIndex].CurrentLevel > 1)
        {
            dungeons[currentDungeonIndex].CurrentLevel--;
        }

        UpdateRewardText();
    }

    public DungeonInfo GetCurrentDungeon()
    {
        return currentDungeon;
    }

    public int GetCurrentLevel()
    {
        return currentDungeon.CurrentLevel;
    }
    // 던전 시작하고 남은 시간 표시 
    private void UpdateTimeText(float timeRemaining)
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timeText.text = $"{minutes:00}:{seconds:00}";
    }

    // 도망 가기 
    public void EscapeDungeon()
    {
        if (!isDungeonActive) return;

        isDungeonActive = false;
        enemySpawn.enabled = true;
        dungeonEnemySpawn.enabled = false;

        GameManager.Instance.CurplayerData.DungeonKeys++;

        // 메인 캔버스로 전환
        combatCanvas.SetActive(false);
        questUI.SetActive(true);
        rewardPopup.SetActive(false);

        dungeonStage.SetActive(false);
        stage.SetActive(true);

        // 던전 종료 처리 (결과 팝업 표시 없이)
        remainingTime = TimeLimit;
        dungeonEnemySpawn.ClearActiveMonsters();
        OnDungeonComplete?.Invoke(currentDungeon.CurrentLevel, totalCoins);
        GameManager.Instance.SaveGame();
    }

    private void RewardDungeon()
    {
        rewardPopup.SetActive(true);
        //rewardText.text = totalCoins.ToString();
        rewardText.text = BigIntegerUtils.FormatBigInteger(totalCoins);

        // 3초 후에 결과 팝업을 자동으로 닫음
        StartCoroutine(CloseRewardPopupAfterDelay(3f));
    }

    private IEnumerator CloseRewardPopupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        rewardPopup.SetActive(false);
    }

}