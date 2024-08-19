using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemListManager : MonoBehaviour
{
    [SerializeField] AnotherSceneManager _anotherSceneManager;
    List<GameObject> ConsumeList = new List<GameObject>();
    public GameObject ItemListPrefab;
    public Transform ListContainer;
    public Dictionary<int, int> inventory;
    public Dictionary<int, ItemData> itemData;

    public void CreateItemList(Dictionary<int, int> inventory, Dictionary<int, ItemData> itemData)
    {
        foreach (Transform child in ListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var kvp in inventory)
        {
            var itemObject = Instantiate(ItemListPrefab, ListContainer);
            var itemListPrefab = itemObject.GetComponent<ItemListPrefab>();
            var data = itemData[kvp.Key];

            itemListPrefab.Icon.sprite = Resources.Load<Sprite>(data.mySprPath);
            itemListPrefab.ItemName.text = data.Name;
            itemListPrefab.ItemAmount.text = kvp.Value.ToString();
            itemListPrefab.key = kvp.Key;

            ConsumeList.Add(itemObject);
        }

        this.inventory = inventory;
        this.itemData = itemData;
    }

    void ConsumeUpdate()
    {
        for(int i = 0; i < ConsumeList.Count; i++)
        {
            ConsumeList[i].GetComponent<ItemListPrefab>().ItemAmount.text = inventory[ConsumeList[i].GetComponent<ItemListPrefab>().key].ToString();
        }
    }

    public void DataUpdate()
    {
        ConsumeUpdate();
        _anotherSceneManager.SaveVillageData(inventory);
    }
}