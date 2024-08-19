using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Health : MonoBehaviour
{
    private Player player;
    public BigInteger Maxhealth;
    public BigInteger curhealth;
    public event Action OnDeath;
    public event Action<BigInteger,BigInteger> OnHealthChanged; // 현재 체력, 최대 체력
    public Transform DamageTextParent;
    public Queue<DamageText> DamageTextQueue;

    private void Start()
    {
        player = GetComponent<Player>();
        InitDamageText();
    }


    public void Init()
    {
        Maxhealth = StatCalculator.CalculateHealth(GameManager.Instance.CurplayerData.HealthLevel);
        curhealth = Maxhealth;
        OnHealthChanged?.Invoke(curhealth, Maxhealth);
    }

    public void UpdateMaxHealth()
    {
        BigInteger oldMaxHealth = Maxhealth;  // 현재 체력 저장
        Maxhealth = player.BaseHealth;

        if (Maxhealth != oldMaxHealth) // 현재 체력과 다르다면
        {
            if (curhealth > 0)
            {
                BigInteger healthPercentage = curhealth * 100 / oldMaxHealth;  // 현재 체력의 비율을 계산 (100을 곱해 정밀도 향상)
                curhealth = Maxhealth * healthPercentage / 100;  // 새로운 체력에 맞춰 현재 체력 조정
            }
            else
            {
                curhealth = Maxhealth; // 처음 시작하거나 부활할 때
            }

            OnHealthChanged?.Invoke(curhealth, Maxhealth);
        }
    }

    public void OnTakeDamage(BigInteger damage)
    {
        bool isCritical =false; // 일단 .. 이렇게 두고 나중에 고쳐보자.. 
        curhealth = BigInteger.Max(BigInteger.Zero, curhealth - damage);
        //Debug.Log($"{curhealth}/{Maxhealth}");
        OnHealthChanged?.Invoke(curhealth, Maxhealth);
        if (curhealth <= 0)
        {
            curhealth = 0;
            OnDeath?.Invoke();
        }
        player.stateMachine.ChangeState(player.stateMachine.playerHitState);
        ShowDamage(damage, isCritical);
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
        now.ShowText((int)damage, player.transform.position, isCritical);
        DamageTextQueue.Enqueue(now);
    }

}
