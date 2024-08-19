using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ComLevelupManager : MonoBehaviour
{
    [SerializeField] private ItemListManager _itemListManager;
    [SerializeField] private CompanionDetailCard _companionDetailCard;
    [SerializeField] private Button levelUpButton;
    public LevelupPrefabs[] levelupPrefabs;
    public LevelupPrefabs[] smallLevelupPrefabs;

    private CompanionData currentCompanion;
    private int controlExp = 0;



    private void OnEnable()
    {

        if (_itemListManager.inventory.Count != 0)
        {
            CheckItemData();
        }

    }

    public void SetCurrentCompanion(CompanionData companion)
    {
        currentCompanion = companion;
        if (currentCompanion != null)
        {
            Debug.Log($"Setting current companion: {currentCompanion.dataSO.dataName}");
            if (_companionDetailCard != null)
            {
                _companionDetailCard.ShowDetail(currentCompanion);
                ResetLevelupItems();
                UpdateExpUI();
            }
            else
            {
                //Debug.LogError("_companionDetailCard is null in SetCurrentCompanion");
            }
        }
        else
        {
            //Debug.LogError("Trying to set null companion");
        }
    }

    private void CheckItemData()
    {
        for (int i = 0; i < levelupPrefabs.Length; i++)
        {
            if (_itemListManager.inventory.ContainsKey(levelupPrefabs[i].Index))
            {
                levelupPrefabs[i].GetItemData(_itemListManager.inventory[levelupPrefabs[i].Index]);
            }
        }
    }

    // 작은 아이템 클릭 시 모든 아이템을 레벨업 재료로 추가
    public void AddAllItems(int index)
    {
        if (levelupPrefabs[index].amount > 0)
        {
            smallLevelupPrefabs[index].GetItemData(smallLevelupPrefabs[index].amount + levelupPrefabs[index].amount);
            controlExp += _itemListManager.itemData[levelupPrefabs[index].Index].Exp * levelupPrefabs[index].amount;
            levelupPrefabs[index].GetItemData(0);

          
            UpdateExpUI();
        }
    }

    // 큰 아이템 클릭 시 한 개의 아이템을 레벨업 재료로 추가
    public void AddOneItem(int index)
    {
        if (currentCompanion == null)
        {
            Debug.LogError("AddOneItem: currentCompanion is null");
            return;
        }

        if (levelupPrefabs[index].amount > 0)
        {
            smallLevelupPrefabs[index].GetItemData(smallLevelupPrefabs[index].amount + 1);
            controlExp += _itemListManager.itemData[levelupPrefabs[index].Index].Exp;
            levelupPrefabs[index].GetItemData(levelupPrefabs[index].amount - 1);

            Debug.Log($"AddOneItem: controlExp = {controlExp}, currentCompanion = {currentCompanion.dataSO.dataName}");
            UpdateExpUI();
        }
    }

    // 레벨업 버튼 클릭 시 호출
    public void LevelUpCompanion()
    {
        if (currentCompanion != null && controlExp > 0)
        {
            currentCompanion.AddExperience(controlExp);
            UpdateInventory();
            _companionDetailCard.ShowDetail(currentCompanion);
            CompanionManager.Instance.SaveCompanionData();
            ResetLevelupItems();

            UpdateExpUI();
        }
    }

    private void UpdateInventory()
    {
        for (int i = 0; i < levelupPrefabs.Length; i++)
        {
            if (_itemListManager.inventory.ContainsKey(levelupPrefabs[i].Index))
            {
                _itemListManager.inventory[levelupPrefabs[i].Index] = levelupPrefabs[i].amount;
            }
        }
        _itemListManager.DataUpdate();
    }

    private void ResetLevelupItems()
    {
        controlExp = 0;
        for (int i = 0; i < smallLevelupPrefabs.Length; i++)
        {
            smallLevelupPrefabs[i].GetItemData(0);
        }
        CheckItemData();
        UpdateExpUI();
    }
    private void UpdateExpUI()
    {
        Debug.Log($"UpdateExpUI called: controlExp = {controlExp}, currentCompanion = {currentCompanion?.dataSO.dataName ?? "null"}");
        if (currentCompanion == null)
        {
            Debug.LogError("currentCompanion is null in UpdateExpUI");
            return;
        }
        if (_companionDetailCard == null)
        {
            Debug.LogError("_companionDetailCard is null in UpdateExpUI");
            return;
        }

        float totalExp = currentCompanion.GetCurrentExp() + controlExp;
        float requiredExp = currentCompanion.GetRequiredExp();
        Debug.Log($"Updating UI: Total Exp = {totalExp}, Required Exp = {requiredExp}");
        _companionDetailCard.UpdateExpUI(totalExp, requiredExp);
    }
}