using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Pool;

public abstract class Enemy : MonoBehaviour
{
    [field: Header("Animations")]
    [field: SerializeField] public PlayerAnimationData animationData { get; private set; }
    public Animator animator { get; private set; }
    protected EnemyStateMachine stateMachine;
    private float targetUpdateInterval = 1f; // 1초마다 타겟 업데이트
    public BigInteger CurrentHealth;

    private EnemySpawn enemySpawn;
    private DungeonEnemySpawn dungeonEnemySpawn;

    public EnemyDataSO data; // 기본 데이터
    public EnemyDataSO enemyData; // 적용 데이터
    public event Action OnDeath;
    public Transform DamageTextParent;
    public Queue<DamageText> DamageTextQueue;
    public bool EnemyTypeRange = false;


    protected void EnemyTypeSet(bool type)
    {
        EnemyTypeRange = type;
    }

    private void Awake()
    {
        animationData.Initialize();
        animator = GetComponentInChildren<Animator>();
        stateMachine = new EnemyStateMachine(this);
        DamageTextParent = GameObject.Find("DamageText").transform;

    }

    private void Start()
    {

        InitDamageText();
        InvokeRepeating("UpdateTarget", 0f, targetUpdateInterval);
    }

    private void OnEnable()
    {
        OnDeath -= OnDeathEnemy;
        stateMachine.ChangeState(stateMachine.enemyRunState);
        OnDeath += OnDeathEnemy;
    }

    public void SetSpawnManager(EnemySpawn spawnManager)
    {
        enemySpawn = spawnManager;
    }

    public void SetSpawnManagerDungeon(DungeonEnemySpawn spawnManager)
    {
        dungeonEnemySpawn = spawnManager;
    }
    public void Init(float statMultiplier)
    {
        enemyData = ScriptableObject.CreateInstance<EnemyDataSO>();

        // 데이터 복사
        enemyData.attackDamage = data.attackDamage;
        enemyData.moveSpeed = data.moveSpeed;
        enemyData.maxHealth = data.maxHealth;
        enemyData.stageLevel = data.stageLevel;

        enemyData.maxHealth *= (BigInteger)statMultiplier;
        enemyData.attackDamage *= (BigInteger)statMultiplier;
        //enemyData.moveSpeed *= statMultiplier;
        CurrentHealth = enemyData.maxHealth;
        stateMachine.ChangeState(stateMachine.enemyRunState);
    }

    public void BossInit(float statMultiplier)
    {
        Init(statMultiplier * 5);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    public bool TakeDamage(BigInteger damage)
    {
        if (CurrentHealth <= 0)
        {
            return false; // 이미 죽은 적은 데미지를 받지 않기
        }
        bool isCritical = false; // 일단 .. 이렇게 두고 나중에 고쳐보자.. 
        CurrentHealth -= damage;
        CurrentHealth = BigInteger.Max(CurrentHealth, 0);
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            OnDeath?.Invoke();
        }
        stateMachine.ChangeState(stateMachine.enemyHitState);
        ShowDamage(damage, isCritical);
        return true;
    }

    void OnDeathEnemy()
    {
        animator.SetTrigger("Death");
        MoneyManager.Instance.AddCoin(1000);
        MoneyManager.Instance.AddSeed(2);
        DungeonManager.Instance.OnMonsterDefeated(this);
        Invoke("OnDestroyEnemy", 0.5f);  // 1초 후에 사라지게 
    }

    public void OnDestroyEnemy()
    {
        MainScene.Instance.returnNowEnemySpawn().OnEnemyDeath(this);
        gameObject.SetActive(false);
    }

    public abstract bool IsInAttackRange(EnemyStateMachine stateMachine);
    public abstract void PerformAttack(EnemyStateMachine stateMachine);

    public virtual void IsInPlayerRange(EnemyStateMachine stateMachine)
    {
        UnityEngine.Vector3 direction = (stateMachine.Target.transform.position - transform.position).normalized;
        transform.position += direction * data.moveSpeed * Time.deltaTime;
    }

    public void InitDamageText()
    {
        DamageTextQueue = new Queue<DamageText>();
        for (int i = 0; i < DamageTextParent.childCount; i++)
        {
            DamageTextQueue.Enqueue(DamageTextParent.GetChild(i).GetComponent<DamageText>());
        }
    }

    void ShowDamage(BigInteger damage, bool isCritical)
    {
        DamageText now = DamageTextQueue.Dequeue(); // 하나 나오고
        now.ShowText((int)damage, transform.position, isCritical);
        DamageTextQueue.Enqueue(now);
    }
    private void UpdateTarget()
    {
        stateMachine.PeriodicTargetUpdate();
    }
}
