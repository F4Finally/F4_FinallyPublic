using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;




public class Companion : MonoBehaviour
{
    [field: Header("Animations")]
    [field: SerializeField] public CompanionAnimData AnimData { get; private set; }
    public Animator Animator { get; private set; }
    public CompanionStateMachine stateMachine;
    public CompanionData data;
    public CompanionDataSO dataSO;
    public List<SkillApply> skillApplies;
    public Queue<DamageText> DamageTextQueue;
    public Transform DamageTextParent;
    public float currentHealth;
    public float maxHealth;
    public bool isInvulnerable = false;
    private void Awake()
    {
        AnimData.Initialize();
        Animator = GetComponentInParent<Animator>();
        stateMachine = new CompanionStateMachine(this);
        InitializeCompanion();
        InitializeSkill();
        DamageTextParent = GameObject.Find("DamageText").transform;

    }
    private void Start()
    {
        stateMachine.ChangeState(stateMachine.companionIdleState);
        InitDamageText();
    }
    private void Update()
    {
        stateMachine.Update();
        checkAllSkill();
    }

    public void InitializeCompanion()
    {
        data = new CompanionData(dataSO);
        data.UpdateStats();
    }
    public void InitializeSkill()
    {
        skillApplies = new List<SkillApply>();
        skillApplies.Add(CompanionManager.Instance.commonSkillData.normalAttackData.returnMySkillApply(this));
        skillApplies.Add(CompanionManager.Instance.commonSkillData.criticalAttackData.returnMySkillApply(this));
        skillApplies.Add(dataSO.uniqueSkillData.returnMySkillApply(this));
    }
   
    private void checkAllSkill()
    {
        foreach (SkillApply skillApply in skillApplies)
        {
            skillApply.checkExecute();
        }
    }


    public void LevelUp()
    {
        data.LevelUp();
    }

    public void AddExperience(float exp)
    {
        data.AddExperience(exp);
    }
    public float CurrentHealth => data.GetCurrentStats().hp;
    public float MaxHealth => data.GetCurrentStats().hp;
    public void TakeDamage(float damage)
    {
        if (isInvulnerable)
        {
            
            return; // 무적 상태일 경우 데미지를 받지 않음
        }
        data.OnTakeDamage(damage, ShowDamage);

        if (data.GetCurrentStats().hp <= 0)
        {
            OnDeath();
            CompanionManager.Instance.UpdateEnemyTargets();
        }
        CompanionManager.Instance.UpdatePlayerHealthUI();
    }

    void OnDeath()
    {
        stateMachine.ChangeState(stateMachine.companionDieState);
        CompanionManager.Instance.RemoveCompanion(data.dataSO.companionId);
    }

    public void InitDamageText()
    {
        DamageTextQueue = new Queue<DamageText>();
        for (int i = 0; i < DamageTextParent.childCount; i++)
        {
            DamageTextQueue.Enqueue(DamageTextParent.GetChild(i).GetComponent<DamageText>());
        }
    }
    public void ShowDamage(float damage, bool isCritical)
    {
        if (DamageTextQueue.Count > 0)
        {
            DamageText damageText = DamageTextQueue.Dequeue();
            damageText.ShowText(damage, transform.position, isCritical);
            DamageTextQueue.Enqueue(damageText);
        }
    }


}

public class CompanionData
{
    public CompanionDataSO dataSO;
    private Stats currentStats;
    private int level = 1;
    public int maxLevel = 50;
    public int dupeCount = 0;
    private float currentExp = 0;
    private float requiredExp;

    public bool isAcquired = false;
    public int starRating = 0;
    public int maxStars = 5;
    public Position position;
    public Grade grade;
    public Dictionary<StatType, float> temporaryBuffs = new Dictionary<StatType, float>();
    
    public CompanionData(CompanionDataSO data)
    {
        this.dataSO = data;
        this.currentStats = data.baseStats.Clone();
        this.grade = data.grade;
        this.position = data.position;
        SetInitialRequiredExp();
        UpdateStats();

    }


    private void SetRequiredExp()
    {
        // 레벨에 따라 요구 경험치 증가
        requiredExp *= Mathf.Pow(1.1f, level - 1);
    }
   


    public void LevelUp()
    {
        if (level < maxLevel)
        {
            level++;
            UpdateStats();
            SetRequiredExp();
        }
    }

    public void UpdateStats()
    {
        float levelMultiplier = 1 + (level - 1) * 0.1f; // 레벨당 10% 증가
        currentStats = dataSO.baseStats.Clone();
        currentStats.MultiplyStats(levelMultiplier);
    }

    public void AddExperience(float exp)
    {
        currentExp += exp;
        while (currentExp >= requiredExp && level < maxLevel)
        {
            currentExp -= requiredExp;
            LevelUp();
        }
    }
  


    public void ApplyPassiveEffect(Player player)
    {
        float currentBonus = dataSO.passiveEffect.GetPercentageBonus(level);
        player.ApplyPassiveEffect(dataSO.passiveEffect.affectedStat, currentBonus);
    }

    public bool CanPromote()
    {
        return dupeCount >= GetMaxDupeCount(grade) && starRating < maxStars;
    }

    public void Promote()
    {
        if (CanPromote())
        {
            dupeCount -= GetMaxDupeCount(grade);
            starRating++;
            ApplyPromotionBonus();
        }
    }

    private void ApplyPromotionBonus()
    {
        float promotionMultiplier = 1.15f; // 15% 증가
        dataSO.baseStats.MultiplyStats(promotionMultiplier);
        UpdateStats();
    }


    public int GetLevel() => level;
    public float GetCurrentExp() => currentExp;
    public float GetRequiredExp() => requiredExp;

    public void OnTakeDamage(float damage, Action<float, bool> showDamageAction)
    {
        bool isCritical = false;
        float damageMultiplier = (100 - currentStats.defense) / 100f;
        float actualDamage = damage * damageMultiplier;
        currentStats.hp = Mathf.Max(currentStats.hp - actualDamage, 0);
        showDamageAction(actualDamage, isCritical);

    }
   


    public Stats GetCurrentStats()
    {
        Stats baseStats = currentStats.Clone();
        foreach (var buff in temporaryBuffs)
        {
            switch (buff.Key)
            {
                case StatType.Attack:
                    baseStats.attack += buff.Value;
                    break;
                case StatType.Defense:
                    baseStats.defense += buff.Value;
                    break;
                case StatType.Health:
                    baseStats.hp += buff.Value;
                    break;
            }
        }
        return baseStats;
    }


    public int GetMaxDupeCount(Grade grade) => grade switch
    {
        Grade.Common => 75,
        Grade.Rare => 50,
        Grade.Epic => 25,
        Grade.Unique => 7,
        Grade.Legend => 3,
        Grade.Mystic => 1,
        _ => 0
    };
    private void SetInitialRequiredExp()
    {
        requiredExp = grade switch
        {
            Grade.Common => 100,
            Grade.Rare => 150,
            Grade.Epic => 200,
            Grade.Unique => 300,
            Grade.Legend => 500,
            Grade.Mystic => 1000,
            _ => 100
        };
    }
    public void SetLevel(int newLevel)
    {
        level = Mathf.Clamp(newLevel, 1, maxLevel);
        UpdateStats();
    }

    public void SetCurrentExp(float newExp)
    {
        currentExp = Mathf.Clamp(newExp, 0, requiredExp);
    }
}

