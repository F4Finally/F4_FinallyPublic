using System.Collections.Generic;

public enum GachaType
{
    Equipment,
    Companion
}

public abstract class GachaItem
{
    public string Name { get; protected set; }
    public Grade Grade { get; protected set; }
}

public class GachaWeapon : GachaItem
{
    public WeaponDataSO Data { get; private set; }
    public int Level { get; private set; } = 1;

    public GachaWeapon(WeaponDataSO data)
    {
        Data = data;
        Name = data.weaponName;
        Grade = data.grade; // Grade를 WeaponDataSO의 grade로 설정
    }

    // SubGrade 속성 추가
    public int SubGrade => Data.subGrade;

    public Stats GetBaseStats() => Data.levelUpStats; // 무기 스탯 가져오기
    public int GetLevel() => Data.level; // 현재 레벨 가져오기
    public int GetCurrentExp() => Data.currentExp; // 현재 경험치 가져오기
    public int GetMaxExp() => Data.maxExp; // 최대 경험치 가져오기
    public string GetWeaponName() => Data.weaponName; // 무기 이름 가져오기
}

public class GachaCompanion : GachaItem
{
    public CompanionDataSO Data { get; private set; }
    public GachaCompanion(CompanionDataSO data)
    {
        Data = data;
        Name = data.dataName;
        Grade = data.grade;
    }
   
}
