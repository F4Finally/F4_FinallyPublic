using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatUpgrade : MonoBehaviour
{
    [System.Serializable]
    private class StatUpgradeUI
    {
        public Button upgradeButton;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI statText;
        public TextMeshProUGUI costText;
    }

    [SerializeField] private Button x1Button;
    [SerializeField] private Button x10Button;
    [SerializeField] private Button x100Button;

    [field: Header("Attack")]
    [SerializeField] private StatUpgradeUI attackUI;

    [field: Header("Health")]
    [SerializeField] private StatUpgradeUI healthUI;

    [field: Header("Defense")]
    [SerializeField] private StatUpgradeUI defenseUI;

    [SerializeField] private Player player;

    private int currentMultiplier = 1;

    private void Start()
    {
        x1Button.onClick.AddListener(() => SetMultiplier(1));
        x10Button.onClick.AddListener(() => SetMultiplier(10));
        x100Button.onClick.AddListener(() => SetMultiplier(100));

        attackUI.upgradeButton.onClick.AddListener(() => UpgradeStat("Attack"));
        healthUI.upgradeButton.onClick.AddListener(() => UpgradeStat("Health"));
        defenseUI.upgradeButton.onClick.AddListener(() => UpgradeStat("Defense"));

        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    private void SetMultiplier(int multiplier)
    {
        currentMultiplier = multiplier; // 1배로 초기화 
        UpdateUI();
    }

    // 스탯 업글 
    private void UpgradeStat(string statType)
    {
        int currentLevel = GetStatLevel(statType);
        BigInteger totalCost = CalculateTotalCost(currentLevel, currentMultiplier);

        if (GameManager.Instance.CurplayerData.Coin >= totalCost)
        {
            MoneyManager.Instance.RemoveCoin(totalCost);
            SetStatLevel(statType, currentLevel + currentMultiplier);
            if (statType == "Health")
            {
                player.health.UpdateMaxHealth();
            }
            StatQuest(statType);
            GameManager.Instance.SaveGame();
            UpdateUI();
        }
    }

    private int GetStatLevel(string statType)
    {
        switch (statType)
        {
            case "Attack": return GameManager.Instance.CurplayerData.AttackLevel; 
            case "Health": return GameManager.Instance.CurplayerData.HealthLevel; 
            case "Defense": return GameManager.Instance.CurplayerData.DefenseLevel; 
            default: return 0;
        }
    }

    private void SetStatLevel(string statType, int level)
    {
        switch (statType)
        {
            case "Attack": GameManager.Instance.CurplayerData.AttackLevel = level; break;
            case "Health": GameManager.Instance.CurplayerData.HealthLevel = level; break;
            case "Defense": GameManager.Instance.CurplayerData.DefenseLevel = level; break;
        }

    }

    private void UpdateUI()
    {
        UpdateStatUI("Attack", attackUI);
        UpdateStatUI("Health", healthUI);
        UpdateStatUI("Defense", defenseUI);
    }

    private void UpdateStatUI(string statType, StatUpgradeUI ui)
    {
        int level = GetStatLevel(statType);
        ui.levelText.text = $"Lv. {level}";
        ui.statText.text = $"+ {BigIntegerUtils.FormatBigInteger(GetNextStatValue(statType, level) * currentMultiplier)}";

        BigInteger cost = CalculateTotalCost(level, currentMultiplier);
        ui.costText.text = BigIntegerUtils.FormatBigInteger(cost);
        // 코인이 부족하면 버튼 안눌리게 
        ui.upgradeButton.interactable = GameManager.Instance.CurplayerData.Coin >= cost;
    }

    private BigInteger GetNextStatValue(string statType, int level)
    {
        switch (statType)
        {
            case "Attack": return StatCalculator.NextAttack(level);
            case "Health": return StatCalculator.NextHealth(level);
            case "Defense": return StatCalculator.NextDefense(level);
            default: return BigInteger.Zero;
        }
    }
    // 돈 계산 
    private BigInteger CalculateTotalCost(int currentLevel, int upgradeCount)
    {
        BigInteger totalCost = BigInteger.Zero;
        for (int i = 0; i < upgradeCount; i++)
        {
            totalCost += StatCalculator.CalculateUpgradeCost(currentLevel + i);
        }
        return totalCost;
    }
    void StatQuest(string statType)
    {
        SubType upgradedStatType = SubType.None;
        switch (statType)
        {
            case "Attack":
                upgradedStatType = SubType.Attack;
                break;
            case "Health":
                upgradedStatType = SubType.Health;
                break;
            case "Defense":
                upgradedStatType = SubType.Defense;
                break;
        }

        foreach (var quest in QuestManager.Instance.GetActiveQuests())
        {
            if (quest.Type == QuestType.StatUpgrade && quest.subType == upgradedStatType)
            {
                QuestManager.Instance.UpdateQuestProgress(quest.ID, 0);
            }
        }
    }

}