[System.Serializable]
public class PassiveEffect
{
    public StatType affectedStat; // 영향을 받는 스텟 종류
    public float basePercentageBonus; // 기본 보너스 퍼센트
    public float bonusPerTenLevels = 0.1f; // 10레벨당 추가 보너스 퍼센트 (기본값 0.1%)


    public float GetPercentageBonus(int level)
    {
        int bonusTiers = (level - 1) / 10;
        return basePercentageBonus + (bonusPerTenLevels * bonusTiers);
    } // 레벨에 따른 퍼센트 보너스 계산

    public string GetDescription(int level)
    {
        string statName = affectedStat.ToKorean();
        float currentBonus = GetPercentageBonus(level);
        int bonusTiers = (level - 1) / 10;
        return $"{statName} 증가 \n{currentBonus:F1}%";
    } // 패시브 효과 설명 생성

   
}