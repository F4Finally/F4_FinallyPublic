using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;

public static class StatCalculator
{

    public static BigInteger CalculateAttack(int level)
    {
        return 5 + (level - 1) * 15;  // 소수점을 피하기 위해 10을 곱함
    }

    public static BigInteger NextAttack(int level)
    {
        return (level - 1) * 15;  // 소수점을 피하기 위해 10을 곱함
    }

    public static BigInteger CalculateDefense(int level)
    {
        return 5 + (level - 1) * 12;  // 소수점을 피하기 위해 10을 곱함
    }

    public static BigInteger NextDefense(int level)
    {
        return (level - 1) * 12;  // 소수점을 피하기 위해 10을 곱함
    }

    public static BigInteger CalculateHealth(int level)
    {
        return 50 + (level - 1) * 100;  // 소수점을 피하기 위해 10을 곱함
    }

    public static BigInteger NextHealth(int level)
    {
        return (level - 1) * 100;  // 소수점을 피하기 위해 10을 곱함
    }

    public static BigInteger CalculateUpgradeCost(int level, int baseCost = 10, double rate = 1.2)
    {
        return (BigInteger)(baseCost * Math.Pow(rate, level - 1));
    }
}

// 테이블을 참조해서 함.. 
// 10f >> 변수로 둬서 하기 할거면 