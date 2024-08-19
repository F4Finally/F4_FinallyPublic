using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int AttackLevel;
    public int DefenseLevel;
    public int HealthLevel;
    public string FormattedCoin;
    public string FormattedSeed;
    public int MainStage;
    public int SubStage;
    public int DungeonLevel;
    public string NickName { get; set; }
    public string VillageName { get; set; }

    [NonSerialized]
    private BigInteger _coin;
    [NonSerialized]
    private BigInteger _seed;

    public int DungeonKeys; // 현재 보유 중인 던전 키 수
    public int MaxDungeonKeys = 3; // 최대 보유 가능한 던전 키 수
    public DateTime LastKeyTime; // 게임 나갔을 때 열쇠 생성 중이던 시간 

    public BigInteger Coin
    {
        get
        {
            if (_coin == default(BigInteger))
            {
                _coin = ParseFormattedValue(FormattedCoin);
            }
            return _coin;
        }
        set
        {
            _coin = value;
            FormattedCoin = BigIntegerUtils.FormatBigInteger(value);
        }
    }

    public BigInteger Seed
    {
        get
        {
            if (_seed == default(BigInteger))
            {
                _seed = ParseFormattedValue(FormattedSeed);
            }
            return _seed;
        }
        set
        {
            _seed = value;
            FormattedSeed = BigIntegerUtils.FormatBigInteger(value);
        }
    }

    public PlayerData()
    {
        AttackLevel = 1;
        DefenseLevel = 1;
        HealthLevel = 1;
        Coin = 1500;
        Seed = 50;
        MainStage = 1;
        SubStage = 1;
        DungeonKeys = MaxDungeonKeys; // 초기에는 최대 키를 가지고 시작
        LastKeyTime = DateTime.Now;
        DungeonLevel = 1;
    }
    // 유저 프로필
    //public string NickName = "";
    //public int Level = 1;
    //public int Rank;

    private BigInteger ParseFormattedValue(string formattedValue)
    {
        if (string.IsNullOrEmpty(formattedValue))
            return BigInteger.Zero;

        string numberPart = new string(formattedValue.TakeWhile(c => char.IsDigit(c) || c == '.').ToArray());
        string suffixPart = formattedValue.Substring(numberPart.Length);

        if (!decimal.TryParse(numberPart, out decimal number))
            return BigInteger.Zero;

        int multiplier = Array.IndexOf(BigIntegerUtils.suffixes, suffixPart);
        if (multiplier < 0)
            multiplier = 0;

        BigInteger result = (BigInteger)(number * (decimal)Math.Pow(1000, multiplier));
        return result;
    }

    // 던전 키 사용 메서드
    public bool UseDungeonKey()
    {
        if (DungeonKeys > 0)
        {

            if(DungeonKeys == MaxDungeonKeys)
                LastKeyTime = DateTime.Now; // 열쇠 사용 시 시간 갱신

            DungeonKeys--;
            return true;
        }
        return false;
    }
}
