using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AnotherSceneManager : MonoBehaviour
{
    public ItemListManager itemListManager;

    private VillageData villageDataBasic;
    private Dictionary<int, int> inventory;
    private Dictionary<int, ItemData> itemData;

    private void Start()
    {
        LoadVillageData();
        
    }


    private void LoadVillageData()
    {
        string filePath = Application.persistentDataPath + "/villageData.json";
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            VillageData villageData = JsonConvert.DeserializeObject<VillageData>(json);
            villageDataBasic = villageData;
            inventory = villageData.Inventory;
            itemData = new Dictionary<int, ItemData>();

            var data = CSVReader.Read(file: "Item");
            for (int i = 0; i < data.Count; i++)
            {
                var target = new ItemData(data[i]);
                itemData.Add(target.Index, target);
            }

            itemListManager.CreateItemList(inventory, itemData);
        }
        else
        {
            Debug.Log("마을데이터 없음");
        }
    }

    public void SaveVillageData(Dictionary<int, int> invenData)
    {
        VillageData villageData = new VillageData
        {
            FacilityLevel_Farm = villageDataBasic.FacilityLevel_Farm,
            CurrentFillAmount_Farm = villageDataBasic.CurrentFillAmount_Farm,

            FacilityLevel_Mine = villageDataBasic.FacilityLevel_Mine,
            CurrentFillAmount_Mine = villageDataBasic.CurrentFillAmount_Mine,

            ItemInstances = new List<ItemSaveData>(),
            Inventory = inventory
        };

        //foreach (var kvp in invenData)
        //{
        //    villageDataBasic.ItemInstances[kvp.Key].CurrentAmount = kvp.Value;
        //}

        for (int i = 0; i < villageDataBasic.ItemInstances.Count; i++)
        {
            villageData.ItemInstances.Add(villageDataBasic.ItemInstances[i]);
        }

        string json = JsonConvert.SerializeObject(villageData, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/villageData.json", json); // 수정된 경로
    }
}
