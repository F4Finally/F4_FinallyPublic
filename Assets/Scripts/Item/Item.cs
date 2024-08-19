using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Item : MonoBehaviour //딕셔너리로 변경한 버전
{

    public Dictionary<int, ItemData> itemData;
    void Start()
    {
        var data = CSVReader.Read(file: "Item");

        itemData = new Dictionary<int, ItemData>(data.Count);

        for (int i = 0; i < data.Count; i++)
        {
            var target = new ItemData(data[i]);
            itemData.Add(target.Index, target);
        }
    }
}

[System.Serializable]

public class ItemData
{
    public int Index;
    public string Name;
    public string Description;
    public int Output;
    public int Progress;
    public int Exp;
    public string mySprPath;
    public bool isFarmProduct;

    public ItemData(Dictionary<string, string> data)
    {
        Index = int.Parse(data["Index"]);
        Name = data["Name"];
        Description = data["Description"];
        Output = int.Parse(data["Output"]);
        Progress = int.Parse(data["Progress"]);
        Exp = int.Parse(data["Exp"]);
        mySprPath = $"Sprites/Items/{data["Path"]}";
        isFarmProduct = int.Parse(data["isFarmProduct"])== 1; //띄워쓰기 빼야해요
    }

}

[System.Serializable]
public class VillageData
{
    public int FacilityLevel_Farm;  // 농장 시설 레벨
    public float CurrentFillAmount_Farm;  // 농장 현재 게이지 값
    public BigInteger StrengtheningCost_Farm; // 농장 강화 비용
    public float MaxFillAmount_Farm;  // 농장 최대 게이지 값

    public int FacilityLevel_Mine;  // 광산 시설 레벨
    public float CurrentFillAmount_Mine;  // 광산 현재 게이지 값
    public BigInteger StrengtheningCost_Mine; // 광산 강화 비용
    public float MaxFillAmount_Mine;  // 광산 최대 게이지 값

    public List<ItemSaveData> ItemInstances;  // 아이템 인스턴스 데이터
    public Dictionary<int, int> Inventory;  // 인벤토리 데이터

    public VillageData()
    {
        this.ItemInstances = new List<ItemSaveData>();
        this.Inventory = new Dictionary<int, int>();
    }

    public VillageData(int FacilityLevel_Farm, float CurrentFillAmount_Farm, BigInteger StrengtheningCost_Farm, float MaxFillAmount_Farm,
                       int FacilityLevel_Mine, float CurrentFillAmount_Mine, BigInteger StrengtheningCost_Mine, float MaxFillAmount_Mine,
                       List<ItemSaveData> itemSaveDatas)
    {
        this.FacilityLevel_Farm = FacilityLevel_Farm;
        this.CurrentFillAmount_Farm = CurrentFillAmount_Farm;
        this.StrengtheningCost_Farm = StrengtheningCost_Farm;
        this.MaxFillAmount_Farm = MaxFillAmount_Farm;

        this.FacilityLevel_Mine = FacilityLevel_Mine;
        this.CurrentFillAmount_Mine = CurrentFillAmount_Mine;
        this.StrengtheningCost_Mine = StrengtheningCost_Mine;
        this.MaxFillAmount_Mine = MaxFillAmount_Mine;

        ItemInstances = itemSaveDatas;
        Inventory = new Dictionary<int, int>();
    }
}

[System.Serializable]
public class ItemSaveData
{
    public int Index;  // 아이템 인덱스
    public int CurrentAmount;  // 현재 아이템 수량
    public DateTime LastStackTime; //마지막으로 아이템이 생성된 시간
    public bool StackStarted;

    public ItemSaveData(int index, int currentAmount)
    {
        this.Index = index;
        this.CurrentAmount = currentAmount;
        LastStackTime = DateTime.Now;
        StackStarted = false;
    }

    public ItemSaveData() { }

    public ItemSaveData(int index)
    {
       this.Index = index;
       this.CurrentAmount = 0;
       LastStackTime = DateTime.Now;
       StackStarted = false;
    }
}



