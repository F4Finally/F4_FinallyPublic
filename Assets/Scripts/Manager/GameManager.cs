using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Player player;
    public PlayerData CurplayerData;
    public Dictionary<int, QuestSaveData> CurQuestSaveData;
    public SaveManager saveManager;
    public QuestManager questManager;
    public StageManager stageManager;
    public GameData nowData;

    public override void Awake()
    {
        base.Awake();
        questManager.AwakeQuestManager();
        saveManager.Init();
        LoadGame();
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        CompanionManager.Instance.ApplyAllCompanionPassiveEffects();

    }

    public void SaveGame()
    {
        UpdatePlayerDataBeforeSave();
        GameData gameData = new GameData(CurplayerData, CurQuestSaveData);
        gameData.UnlockedSlots = StageManager.Instance.UnlockedSlots; 
        saveManager.SavaData(gameData);
    }



    public bool LoadGame()
    {
        
        if (saveManager.TryLoadData(out GameData data))
        {
            CurplayerData = data.playerData;
            CurQuestSaveData = data.QuestSaveDatas;
            StageManager.Instance.SetUnlockedSlots(data.UnlockedSlots);
            return true;
        }
        else
        {
            CurplayerData = new PlayerData();
            CurQuestSaveData = new Dictionary<int, QuestSaveData>();
            foreach (var nowQuest in QuestManager.Instance.questData)
            {
                CurQuestSaveData.Add(nowQuest.Key, new QuestSaveData(nowQuest.Key));
            }
            CurQuestSaveData[1].IsAccepted = true;
            return false;
        }

    }

    private void UpdatePlayerDataBeforeSave()
    {
        if (player != null)
        {
            CurplayerData.AttackLevel = player.AttackLevel;
            CurplayerData.DefenseLevel = player.DefenseLevel;
            CurplayerData.HealthLevel = player.HealthLevel;
            CurplayerData.MainStage = StageManager.Instance.CurStage;
            CurplayerData.SubStage = StageManager.Instance.CurSubStage;
            CurplayerData.Coin = player.Coin;
            CurplayerData.Seed = player.Seed;
        }
    }

}
