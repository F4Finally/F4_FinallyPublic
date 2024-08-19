using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

public class CompanionManager : Singleton<CompanionManager>
{

    [SerializeField] private List<CompanionDataSO> companionDataSO;
    public CommonSkillDataSO commonSkillData;
    public CommonSkillDataSO GetCommonSkillData() => commonSkillData;
    public Dictionary<string, CompanionData> companionDataDict = new Dictionary<string, CompanionData>();
    public Dictionary<int, string> placedCompanions = new Dictionary<int, string>();
    private Dictionary<string, GameObject> companionPrefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, Companion> spawnedCompanions = new Dictionary<string, Companion>();
    private string path;
    public List<CompanionData> GetAllCompanions()
    {
        return new List<CompanionData>(companionDataDict.Values);
    }
    public List<CompanionData> GetAcquiredCompanions()
    {
        return companionDataDict.Values.Where(c => c.isAcquired).ToList();
    }
    public List<Companion> GetPlacedCompanions()
    {
        return spawnedCompanions.Values.ToList();
    }
    public void Start()
    {
        LoadCompanionData();
    }
    public override void Awake()
    {
        path = Path.Combine(Application.persistentDataPath, "companionData.json");
        base.Awake();
        InitializeCompanions();
        LoadCompanionPrefabs();

    }
    private void OnDisable()
    {
        SaveCompanionData();
    }
    public void InitializeCompanions()
    {
        foreach (var dataSO in companionDataSO)
        {
            if (!companionDataDict.ContainsKey(dataSO.companionId))
            {
                CompanionData data = new CompanionData(dataSO);
                companionDataDict.Add(dataSO.companionId, data);
            }
        }
    }

    public CompanionData GetCompanionById(string companionId)
    {
        if (string.IsNullOrEmpty(companionId))
        {
            return null;
        }

        if (companionDataDict.TryGetValue(companionId, out CompanionData companionData))
        {
            return companionData;
        }

        Debug.LogWarning($"Companion with ID {companionId} not found.");
        return null;
    }
    public void ApplyCompanionPassiveEffect(string companionId)
    {
        CompanionData companionData = GetCompanionById(companionId);
        if (companionData != null && companionData.isAcquired)
        {
            Player player = GameManager.Instance.player;
            if (player != null)
            {
                companionData.ApplyPassiveEffect(player);
            }
            else
            {
                Debug.LogError("Player reference not found in GameManager.");
            }
        }
    }

    public void ApplyAllCompanionPassiveEffects()
    {
        Player player = GameManager.Instance.player;
        if (player != null)
        {
            foreach (var companionData in companionDataDict.Values)
            {
                if (companionData.isAcquired)
                {
                    companionData.ApplyPassiveEffect(player);
                    Debug.Log($"Applied passive effect of companion {companionData.dataSO.companionId} to player.");
                }
            }
        }
        else
        {
            Debug.LogError("Player reference not found in GameManager.");
        }
    }



    public void LoadCompanionPrefabs()
    {

        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/Companions");

        foreach (GameObject prefab in prefabs)
        {
            Companion companionComponent = prefab.GetComponent<Companion>();
            if (companionComponent != null)
            {
                companionPrefabs[companionComponent.dataSO.companionId] = prefab;
            }
        }

    }

    public void SpawnCompanionInGame(string companionId, int slotIndex)
    {
        if (spawnedCompanions.ContainsKey(companionId))
        {
            return;
        }

        if (companionPrefabs.TryGetValue(companionId, out GameObject prefab))
        {
            Vector3 spawnPosition = prefab.transform.position; // 프리펩의 초기 위치를 사용
            GameObject spawnedCompanion = Instantiate(prefab, spawnPosition, Quaternion.identity);
            Companion companionComponent = spawnedCompanion.GetComponent<Companion>();

            if (companionComponent != null && companionDataDict.TryGetValue(companionId, out CompanionData data))
            {
                companionComponent.data = data;
                companionComponent.InitializeCompanion();
                spawnedCompanions[companionId] = companionComponent;


                UpdatePlayerHealthUI();
            }

        }

    }


    public void DespawnCompanionInGame(string companionId)
    {
        if (spawnedCompanions.TryGetValue(companionId, out Companion spawnedCompanion))
        {
            Destroy(spawnedCompanion.gameObject);
            spawnedCompanions.Remove(companionId);
            Debug.Log($"Despawned companion {companionId}");

            UpdatePlayerHealthUI();
        }
    }
    public bool IsCompanionSpawned(string companionId)
    {
        return spawnedCompanions.ContainsKey(companionId);
    }

    public bool PlaceCompanion(string companionId, int slotIndex)
    {
        if (placedCompanions.ContainsKey(slotIndex))
        {
            RemoveCompanion(placedCompanions[slotIndex]);
        }
        if (placedCompanions.TryAdd(slotIndex, companionId))
        {
            placedCompanions[slotIndex] = companionId;
        }

        SpawnCompanionInGame(companionId, slotIndex);
        SaveCompanionData();
        return true;
    }

    public void RemoveCompanion(string companionId)
    {
        int slotIndex = placedCompanions.FirstOrDefault(x => x.Value == companionId).Key;
        placedCompanions.Remove(slotIndex);
        SaveCompanionData();
    }
    public void UpdatePlayerHealthUI()
    {
        Player player = GameManager.Instance.player;
        if (player != null)
        {
            player.UpdateHealthUI(player.health.curhealth, player.health.Maxhealth);
        }
    }
    public int returnCompanionPosition(string compaionId)
    {
        foreach (var companion in placedCompanions)
        {
            if (companion.Value == compaionId)
            {
                return companion.Key;
            }
        }
        return -1;
    }

    public void SaveCompanionData()
    {
        List<CompanionSaveData> nowCompanion = new List<CompanionSaveData>();

        foreach (var companion in companionDataDict.Values)
        {
            nowCompanion.Add(new CompanionSaveData
            {
                companionId = companion.dataSO.companionId,
                isAcquired = companion.isAcquired,
                level = companion.GetLevel(),
                currentExp = companion.GetCurrentExp(),
                dupeCount = companion.dupeCount,
                starRating = companion.starRating,
                nowPosition = returnCompanionPosition(companion.dataSO.companionId)

            });
        }
        string jsonData = JsonConvert.SerializeObject(nowCompanion, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            }); // data 를 json 데이터 형태의 문자열로 변환해줌 
        File.WriteAllText(path, jsonData);
        
    }

    public void LoadCompanionData()
    {
        List<CompanionSaveData> companionSaveDatas;
        if (!File.Exists(path))
        {
            companionSaveDatas = new List<CompanionSaveData> { };
            foreach (var targert in companionDataSO)
            {
                companionSaveDatas.Add(new CompanionSaveData(targert.companionId));
            }
        }
        else
        {
            string jsonData = File.ReadAllText(path);
            companionSaveDatas = JsonConvert.DeserializeObject<List<CompanionSaveData>>(jsonData);
          
        }

        placedCompanions = new Dictionary<int, string> { };
        foreach (var companionSaveData in companionSaveDatas)
        {
            if (companionDataDict.TryGetValue(companionSaveData.companionId, out CompanionData companionData))
            {
                companionData.isAcquired = companionSaveData.isAcquired;

                companionData.SetLevel(companionSaveData.level);
                companionData.SetCurrentExp(companionSaveData.currentExp);
                companionData.dupeCount = companionSaveData.dupeCount;
                companionData.starRating = companionSaveData.starRating;
            }
            if (companionSaveData.nowPosition != -1)
            {
                placedCompanions.Add(companionSaveData.nowPosition, companionSaveData.companionId);
              
            }

        }
        ApplyPlacedCompanions();

    }


    public void ApplyPlacedCompanions()
    {
        int unlockedSlots = StageManager.Instance.UnlockedSlots;
        foreach (var kvp in placedCompanions.ToList())
        {
            if (kvp.Key < unlockedSlots)
            {
                SpawnCompanionInGame(kvp.Value, kvp.Key);
            }
            else
            {
                placedCompanions.Remove(kvp.Key);
            }
        }
        CompanionUIManager.Instance.UpdateFormationSlots();
    }


    public List<Companion> GetAliveCompanions()
    {
        var aliveCompanions = spawnedCompanions.Values.Where(c => c.CurrentHealth > 0).ToList();
        return aliveCompanions;
    }

    public void UpdateEnemyTargets()
    {
        // 모든 적의 타겟을 업데이트
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.GetComponent<EnemyStateMachine>().UpdateAliveCompanions();
            enemy.GetComponent<EnemyStateMachine>().UpdateCurrentTarget();
        }
    }
}