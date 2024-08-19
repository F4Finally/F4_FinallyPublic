using UnityEngine;

public enum Position { RangedDPS, MeleeDPS, Buffer, Tank }
public enum StatType { Attack, Defense, Health }

public static class StatTypeExtensions
{
    public static string ToKorean(this StatType statType)
    {
        switch (statType)
        {
            case StatType.Attack:
                return "공격력";
            case StatType.Defense:
                return "방어력";
            case StatType.Health:
                return "체력";
            default:
                return statType.ToString();
        }
    }
}
[CreateAssetMenu(fileName = "NewCompanionData", menuName = "Game Data/Data/Companion")]
public class CompanionDataSO : DataSO
{
    [Header("Companion Info")]
    public Position position;
    public string companionId;
    public float attackRange;

    public SkillData uniqueSkillData;
    
}
