using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;

public class MoneyManager : Singleton<MoneyManager>
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI seedText;

    public override void Awake()
    {
        base.Awake();

        
    }
    private void Start()
    {
        UpdateCurrencyText();
    }
    private void Update()
    {
        UpdateCurrencyText();
    }

    public void AddCoin(int amount)
    {
        GameManager.Instance.CurplayerData.Coin += amount;  // 게임매니저.데이터.코인 값 뭐 이렇게
        UpdateCurrencyText();
    }

    public void RemoveCoin(BigInteger amount)
    {
        GameManager.Instance.CurplayerData.Coin = BigInteger.Max(0, GameManager.Instance.CurplayerData.Coin - amount);
        UpdateCurrencyText();
    }

    public void AddSeed(int amount)
    {
        GameManager.Instance.CurplayerData.Seed += amount;
        UpdateCurrencyText();
    }

    public void RemoveSeed(BigInteger amount)
    {
        GameManager.Instance.CurplayerData.Seed = BigInteger.Max(0, GameManager.Instance.CurplayerData.Seed - amount);
        UpdateCurrencyText();
    }

    private void UpdateCurrencyText()
    {
        coinText.text = BigIntegerUtils.FormatBigInteger(GameManager.Instance.CurplayerData.Coin);
        seedText.text = BigIntegerUtils.FormatBigInteger(GameManager.Instance.CurplayerData.Seed);
    }
}

// 게임 매니저가 gameData 클래스 만들어서 플레이어 스탯, 재화 