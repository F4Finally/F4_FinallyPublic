using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public static class BigIntegerUtils
{
    public static string[] suffixes = { "", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    public static string FormatBigInteger(BigInteger value)
    {
        if (value < 1000)
            return value.ToString();
        int suffixIndex = 0;
        decimal adjustedValue = (decimal)value;
        while (adjustedValue >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            adjustedValue /= 1000;
            suffixIndex++;
        }
        return string.Format("{0:0.##}{1}", Math.Round(adjustedValue, 2), suffixes[suffixIndex]);
    }
}
