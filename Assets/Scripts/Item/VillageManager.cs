using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using System.Numerics;

public class VillageManager : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject ItemPrefab;    // 아이템 프리팹   
    public GameObject InvenListPrefab; // 인벤토리 슬롯 프리팹

    public Dictionary<int, ItemData> itemData; // 아이템 데이터 사전
    public Dictionary<int, ItemPrefab> itemInstances; // 생성된 아이템 프리팹 사전
    public Dictionary<int, int> inventory; // 인벤토리 아이템 데이터
    public Dictionary<int, ItemSaveData> itemSaveData;

    public VillageProducerBase farmProducer;
    public VillageProducerBase mineProducer;

    [SerializeField] GameObject G_ResultPopup;
    public GameObject NoFarmResultPanel;
    public GameObject NoMineResultPanel;

    [Header("Money")]
    public TextMeshProUGUI CoinTxt_Farm;
    public TextMeshProUGUI CoinTxt_Mine;
    public TextMeshProUGUI CoinTxt_Inven;
    public GameManager gameManager;


    [Header("ActiveCheck")]
    public GameObject EquipPanel1; //농장
    public GameObject EquipPanel2;
    public GameObject EquipPanel3;
    public GameObject EquipPanel4;

    public GameObject EquipPanel5; //광산
    public GameObject EquipPanel6;
    public GameObject EquipPanel7;
    public GameObject EquipPanel8;

    [Header("ActiveLock")]
    public GameObject LockPanel1; //농장
    public GameObject LockPanel2;
    public GameObject LockPanel3;
    public GameObject LockPanel4;

    public GameObject LockPanel5; //광산
    public GameObject LockPanel6;
    public GameObject LockPanel7;
    public GameObject LockPanel8;

    [SerializeField] GameObject farmPoint;
    [SerializeField] GameObject minePoint;

    bool isFarmPerfect = false;
    bool isMinePerfect = false;

    private void Start()
    {
        itemInstances = new Dictionary<int, ItemPrefab>();
        inventory = new Dictionary<int, int>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        UpdateCoinText();

        farmPoint.SetActive(isFarmPerfect);
        minePoint.SetActive(isMinePerfect);

        LoadItemData(); // 아이템 데이터 로드

        LoadVillageData();// 마을 데이터를 로드
        Debug.Log($"Farm Max Fill Amount: {farmProducer.maxFillAmount}");
        Debug.Log($"Mine Max Fill Amount: {mineProducer.maxFillAmount}");

        CreateItemPrefabs(); // 아이템 프리팹 생성

        farmProducer.UpdateUI();
        mineProducer.UpdateUI();

        ActivateEquipPanel();

        SaveVillageData();

    }

    // CSV 파일에서 아이템 데이터 로드
    private void LoadItemData()
    {
        var data = CSVReader.Read(file: "Item");
        itemData = new Dictionary<int, ItemData>(data.Count);

        for (int i = 0; i < data.Count; i++)
        {
            var target = new ItemData(data[i]);
            itemData.Add(target.Index, target);
        }
    }

    // 아이템 프리팹 생성
    public void CreateItemPrefabs()
    {
        farmProducer.CreateItemPrefabs();
        mineProducer.CreateItemPrefabs();
    }


    // 인벤토리 UI 업데이트
    void UpdateInventoryUI()
    {
        farmProducer.UpdateInventoryUI();
        mineProducer.UpdateInventoryUI();
        SaveVillageData();
    }

    // 마을 데이터를 JSON 파일로 저장
    public void SaveVillageData()
    {
        VillageData villageData = new VillageData
        {
            FacilityLevel_Farm = farmProducer.facilityLevel,
            CurrentFillAmount_Farm = farmProducer.currentFillAmount,
            StrengtheningCost_Farm = farmProducer.baseCost + (new BigInteger(50000) * (farmProducer.facilityLevel - 1)),
            MaxFillAmount_Farm = farmProducer.maxFillAmount,

            FacilityLevel_Mine = mineProducer.facilityLevel,
            CurrentFillAmount_Mine = mineProducer.currentFillAmount,
            StrengtheningCost_Mine = mineProducer.baseCost + (new BigInteger(50000) * (mineProducer.facilityLevel - 1)),
            MaxFillAmount_Mine = mineProducer.maxFillAmount,

            ItemInstances = new List<ItemSaveData>(),
            Inventory = inventory
        };

        foreach (var kvp in itemSaveData)
        {
            villageData.ItemInstances.Add(kvp.Value);
        }

        string json = JsonConvert.SerializeObject(villageData, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/villageData.json", json);
    }

    public void OnTransitionToMainScene()
    {
        SaveVillageData();
        SceneManager.LoadScene("MainScene");
    }

    // JSON 파일에서 마을 데이터 로드
    public void LoadVillageData()
    {
        string filePath = Application.persistentDataPath + "/villageData.json";
        VillageData villageData;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            villageData = JsonConvert.DeserializeObject<VillageData>(json);
        }
        else
        {
            // 기본 값으로 초기화
            List<ItemSaveData> items = new List<ItemSaveData>();
            foreach (var itemData in itemData)
            {
                items.Add(new ItemSaveData(itemData.Key));
            }

            villageData = new VillageData
            {
                FacilityLevel_Farm = 1,
                CurrentFillAmount_Farm = 0,
                MaxFillAmount_Farm = 10000f, 
                FacilityLevel_Mine = 1,
                CurrentFillAmount_Mine = 0,
                MaxFillAmount_Mine = 10000f, 
                ItemInstances = items,
                Inventory = new Dictionary<int, int>()
            };
        }

        farmProducer.facilityLevel = villageData.FacilityLevel_Farm;
        farmProducer.currentFillAmount = villageData.CurrentFillAmount_Farm;
        farmProducer.maxFillAmount = villageData.MaxFillAmount_Farm;

        mineProducer.facilityLevel = villageData.FacilityLevel_Mine;
        mineProducer.currentFillAmount = villageData.CurrentFillAmount_Mine;
        mineProducer.maxFillAmount = villageData.MaxFillAmount_Mine;

        inventory = villageData.Inventory;

        itemSaveData = new Dictionary<int, ItemSaveData>();
        foreach (var itemData in villageData.ItemInstances)
        {
            itemSaveData.Add(itemData.Index, itemData);
        }

        UpdateInventoryUI();
    }

    private void Update()
    {
        StackItems();
    }

    void StackItems()
    {
        foreach(var NowSlot in itemInstances)
        {
            ItemData NowData = itemData[NowSlot.Key];
            int NowFacilityLevel;
            if (NowData.isFarmProduct)
            {
                if (!farmProducer.ShouldActivateItem(NowSlot.Key, farmProducer.facilityLevel)) continue;
                NowFacilityLevel = farmProducer.facilityLevel; 
            }
            else
            {
                if (!mineProducer.ShouldActivateItem(NowSlot.Key, mineProducer.facilityLevel)) continue;
                NowFacilityLevel = mineProducer.facilityLevel;
            }

            //둘 중 하나 true면 생산 
            if(!mineProducer.ShouldActivateItem(NowSlot.Key, NowFacilityLevel) && !farmProducer.ShouldActivateItem(NowSlot.Key, NowFacilityLevel)) { continue; }

            ItemSaveData NowSaveData = itemSaveData[NowSlot.Key];
            

            TimeSpan timeSpan = DateTime.Now - NowSaveData.LastStackTime;
            double totalSecond = timeSpan.TotalSeconds;
            int StackCount = (int)(totalSecond / NowData.Output);
            if(StackCount > 0)
            {
                NowSaveData.CurrentAmount += StackCount;
                int NowMax = ReturnMaxAmount(NowSlot.Key, NowFacilityLevel);
                if(NowSaveData.CurrentAmount > NowMax)
                {
                    NowSaveData.CurrentAmount = NowMax;
                }
                if(NowData.isFarmProduct && !isFarmPerfect)
                {
                    if (NowSaveData.CurrentAmount >= NowMax)
                    {
                        isFarmPerfect = true;
                        farmPoint.SetActive(true);
                    }
                }
                if (!NowData.isFarmProduct && !isMinePerfect)
                {
                    if (NowSaveData.CurrentAmount >= NowMax)
                    {
                        isMinePerfect = true;
                        minePoint.SetActive(true);  
                    }
                }

                NowSaveData.LastStackTime = DateTime.Now;
                itemInstances[NowSlot.Key].UpdateItemNumTxt(NowSaveData.CurrentAmount, NowMax);
            }
        }
    }

    // 아이템 인덱스와 시설 레벨을 기반으로 최대 수량 설정
    public int ReturnMaxAmount(int itemIndex, int facilityLevel)
    {
        int baseAmount = 80 + 40 * (facilityLevel - 1);
        int decrement = ((itemIndex - 1) % 6) * 5; // 인덱스에 따라 5씩 감소
        return baseAmount - decrement;
    }

    public void OnGatherResultPopupOpen(bool isFarmProducer)
    {
        List<ResultData> resultData = new List<ResultData>();

        foreach (var kvp in itemSaveData)
        {
            if (itemData[kvp.Key].isFarmProduct != isFarmProducer) continue;
            int itemId = kvp.Key;
            int amount = kvp.Value.CurrentAmount;

            if (amount > 0)
            {
                resultData.Add(new ResultData(itemId, amount));
            }
        }

        if (resultData.Count > 0)
        {
            G_ResultPopup.SetActive(true);
            G_ResultPopup.GetComponent<ResultPopup>().ResultSet(resultData);
        }
        else
        {
            if (isFarmProducer)
            {
                NoFarmResultPanel.SetActive(true);
            }
            else
            {
                NoMineResultPanel.SetActive(true);
            }
        }
        
        if(isFarmProducer)
        {
            isFarmPerfect = false;
            farmPoint.SetActive(false);
        }
        else
        {
            isMinePerfect = false;
            minePoint.SetActive(false);
        }
    }

    public void UpdateCoinText()
    {
        CoinTxt_Farm.text = BigIntegerUtils.FormatBigInteger(gameManager.CurplayerData.Coin);
        CoinTxt_Mine.text = BigIntegerUtils.FormatBigInteger(gameManager.CurplayerData.Coin);
        CoinTxt_Inven.text = BigIntegerUtils.FormatBigInteger(gameManager.CurplayerData.Coin);

        Debug.Log(CoinTxt_Inven.text);
    }

    // 패널 활성화 로직
    public void ActivateEquipPanel()
    {
        if (farmProducer.facilityLevel >= 2)
        {
            EquipPanel1?.SetActive(true);
            LockPanel1?.SetActive(false);
        }
        if (farmProducer.facilityLevel >= 3)
        {
            EquipPanel2?.SetActive(true);
            LockPanel2?.SetActive(false);
        }
        if (farmProducer.facilityLevel >= 4)
        {
            EquipPanel3?.SetActive(true);
            LockPanel3?.SetActive(false);
        }
        if (farmProducer.facilityLevel >= 5)
        {
            EquipPanel4?.SetActive(true);
            LockPanel4?.SetActive(false);
        }
        
        if (mineProducer.facilityLevel >= 2)
        {
            EquipPanel5?.SetActive(true);
            LockPanel5?.SetActive(false);
        }
        if (mineProducer.facilityLevel >= 3)
        {
            EquipPanel6?.SetActive(true);
            LockPanel6?.SetActive(false);
        }
        if (mineProducer.facilityLevel >= 4)
        {
            EquipPanel7?.SetActive(true);
            LockPanel7?.SetActive(false);
        }
        if (mineProducer.facilityLevel >= 5)
        {
            EquipPanel8?.SetActive(true);
            LockPanel8?.SetActive(false);
        }
        
    }

}