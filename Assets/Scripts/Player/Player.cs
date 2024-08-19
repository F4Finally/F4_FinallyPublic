using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // 애니메이션 먼저
    [field: Header("Animations")]
    [field: SerializeField] public PlayerAnimationData animationData { get; private set; }
    public Animator animator { get; private set; }
    public PlayerStateMachine stateMachine;

    [field: Header("PlayerStats")]
    public float AttackWeight = 1.0f;
    public float DefenseWeight = 0.5f;
    public float HealthWeight = 0.2f;

    // 동료 보유효과 추가를 위한 변수
    private float attackBonus = 1f;
    private float defenseBonus = 1f;
    private float healthBonus = 1f;

    private WeaponDataSO equippedWeaponData; // 무기 데이터 저장 필드 추가

    public int AttackLevel => GameManager.Instance.CurplayerData.AttackLevel;
    public int DefenseLevel => GameManager.Instance.CurplayerData.DefenseLevel;
    public int HealthLevel => GameManager.Instance.CurplayerData.HealthLevel;

    public BigInteger BaseAttack => StatCalculator.CalculateAttack(GameManager.Instance.CurplayerData.AttackLevel) * (BigInteger)(attackBonus * 100) / 100 + GetWeaponAttackBonus();
    public BigInteger BaseDefense => StatCalculator.CalculateDefense(GameManager.Instance.CurplayerData.DefenseLevel) * (BigInteger)(defenseBonus * 100) / 100 + GetWeaponDefenseBonus();
    public BigInteger BaseHealth => StatCalculator.CalculateHealth(GameManager.Instance.CurplayerData.HealthLevel) * (BigInteger)(healthBonus * 100) / 100 + GetWeaponHealthBonus();
    public BigInteger TotalPower => (BaseAttack * (BigInteger)(AttackWeight * 100) / 100) + (BaseDefense * (BigInteger)(DefenseWeight * 100) / 100) + (BaseHealth * (BigInteger)(HealthWeight * 100) / 100);
    public Health health { get; private set; }
    // 유저 프로필
    //public string NickName {  get; set; }
    //public int Rank { get; private set; }
    //public int Level { get; private set; } = 1;

    //플레이어 장비

    [field: Header("PlayerEquipment")]

    public GachaWeapon EquippedItem { get; private set; }  // 장비 불러오기 
    public GameObject ultimateEffectPrefab;

    [field: Header("PlayerMoney")]
    // 플레이어 재화
    public BigInteger Coin
    {
        get => GameManager.Instance.CurplayerData.Coin;
        set
        {
            GameManager.Instance.CurplayerData.Coin = value;
        }
    }
    public BigInteger Seed
    {
        get => GameManager.Instance.CurplayerData.Seed;
        set
        {
            GameManager.Instance.CurplayerData.Seed = value;
        }
    }

    [Header("UI")]
    [SerializeField] private Slider ultimateGaugeSlider;
    [SerializeField] private Slider healthSlider;
    public float UltimateGauge { get; set; } = 1f;
    [SerializeField] private TextMeshProUGUI AttackText;
    [SerializeField] private TextMeshProUGUI DefenseText;
    [SerializeField] private TextMeshProUGUI HealthText;
    [SerializeField] private TextMeshProUGUI TotalText;
    [SerializeField] private TextMeshProUGUI TotalText2;
    [SerializeField] private TextMeshProUGUI healthText;

    private EnemySpawn enemySpawn;

    public void Awake()
    {
        animationData.Initialize();
        animator = GetComponentInParent<Animator>();
        health = GetComponent<Health>();
        enemySpawn = FindObjectOfType<EnemySpawn>();
        stateMachine = new PlayerStateMachine(this);
        GameManager.Instance.player = this;
    }

    private void Start()
    {
        stateMachine.ChangeState(stateMachine.playerIdleState);
        health.Init();
        health.OnDeath += OnDeath;
        health.OnHealthChanged += UpdateHealthUI;
        UpdateHealthUI(health.curhealth, health.Maxhealth);
        
    }

    private void Update()
    {
        stateMachine.Update();
        UpdateTextUI();
        health.UpdateMaxHealth();
    }
    void OnDeath()
    {
        animator.SetTrigger("Death");

        StartCoroutine(RestartStage());
    }

    private IEnumerator RestartStage()
    {

        yield return StartCoroutine(StageManager.Instance.fadeController.FadeOut());


        GameManager.Instance.CurplayerData.SubStage = StageManager.Instance.CurSubStage;
        GameManager.Instance.CurplayerData.MainStage = StageManager.Instance.CurStage;


        StageManager.Instance.CurSubStage = GameManager.Instance.CurplayerData.SubStage;
        StageManager.Instance.CurStage = GameManager.Instance.CurplayerData.MainStage;


        StageManager.Instance.UpdateStageState();


        yield return StartCoroutine(StageManager.Instance.fadeController.FadeIn());


        StageManager.Instance.UpdateStageUI();

   
        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        // 플레이어 체력 초기화
        health.Init();

        // 플레이어 위치 초기화 (기본 위치로 설정)
        transform.position = this.transform.position;

        // 상태초기화 
        animator.ResetTrigger("Death");
        animator.Play("Idle");
        stateMachine.ChangeState(stateMachine.playerIdleState);
        // 몬스터 초기화 및 리스폰
        if (enemySpawn != null)
        {
            enemySpawn.RespawnMonsters();
        }

    }

    // 궁극기 게이지 UI 
    public void UpdateUltimateGaugeUI()
    {
        if (ultimateGaugeSlider != null)
        {
            ultimateGaugeSlider.value = UltimateGauge;
        }
    }

    // 체력 슬라이더 UI 
    public void UpdateHealthUI(BigInteger current, BigInteger max)
    {
        // 플레이어의 현재 체력과 최대 체력
        BigInteger totalCurrent = current;
        BigInteger totalMax = max;

        // 스폰된 동료들의 체력을 더함
        var spawnedCompanions = CompanionManager.Instance.GetAliveCompanions();
        foreach (var companion in spawnedCompanions)
        {
            totalCurrent += (BigInteger)companion.CurrentHealth;
            totalMax += (BigInteger)companion.MaxHealth;
        }

        float healthPercentage = (float)(totalCurrent / totalMax);
        healthSlider.value = healthPercentage;
        healthText.text = BigIntegerUtils.FormatBigInteger(totalCurrent);
    }

    // 무기 보너스 스탯을 계산하는 메서드 추가
    private BigInteger GetWeaponAttackBonus()
    {
        return equippedWeaponData != null ? (BigInteger)equippedWeaponData.baseStats.attack : BigInteger.Zero;
    }

    private BigInteger GetWeaponDefenseBonus()
    {
        return equippedWeaponData != null ? (BigInteger)equippedWeaponData.baseStats.defense : BigInteger.Zero;
    }

    private BigInteger GetWeaponHealthBonus()
    {
        return equippedWeaponData != null ? (BigInteger)equippedWeaponData.baseStats.hp : BigInteger.Zero;
    }


    // 무기 데이터를 적용하고 스탯을 업데이트하는 메서드 추가
    public void ApplyWeaponStats(WeaponDataSO weaponData)
    {
        equippedWeaponData = weaponData;
        UpdateTextUI();
    }

    // 무기 데이터를 제거하고 스탯을 업데이트하는 메서드 추가
    public void RemoveWeaponStats(WeaponDataSO weaponData)
    {
        if (equippedWeaponData == weaponData)
        {
            equippedWeaponData = null;
            UpdateTextUI();
        }
    }
    // 동료 보유 효과를 적용하는 메서드 추가
    public void ApplyPassiveEffect(StatType statType, float bonusPercentage)
    {
        switch (statType)
        {
            case StatType.Attack:
                attackBonus *= (1 + bonusPercentage / 100);
                break;
            case StatType.Defense:
                defenseBonus *= (1 + bonusPercentage / 100);
                break;
            case StatType.Health:
                healthBonus *= (1 + bonusPercentage / 100);
                break;
        }
        UpdateTextUI();
        health.UpdateMaxHealth();
    }


    // 전체적인 Text UI
    private void UpdateTextUI()
    {
        AttackText.text = BigIntegerUtils.FormatBigInteger(BaseAttack);
        DefenseText.text = BigIntegerUtils.FormatBigInteger(BaseDefense);
        HealthText.text = BigIntegerUtils.FormatBigInteger(BaseHealth);
        TotalText.text = BigIntegerUtils.FormatBigInteger(TotalPower);
        TotalText2.text = BigIntegerUtils.FormatBigInteger(TotalPower);
    }
}

