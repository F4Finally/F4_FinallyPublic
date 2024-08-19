using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Game Data/Data/Weapon")]
public class WeaponDataSO : DataSO
{
    [Header("Weapon Information")]
    public string weaponName;  // 무기 이름

    [Header("Weapon Identification")]
    public int weaponID;  // 무기를 고유하게 식별할 ID

    [Header("Weapon Grade Details")]
    public int subGrade; // 세부등급 (1, 2, 3, 4)

    [Header("Weapon Level Details")]
    public const int MaxLevel = 100; // 최대 레벨

    [Header("Stats")]
    public Stats levelUpStats; // 레벨업 시 증가할 스탯


    [Header("WeaponSaveData")]
    public WeaponSaveData myWeaponSaveData;

    public int level // 무기의 현재 레벨
    {
        get => myWeaponSaveData.level;
        set => myWeaponSaveData.level = value;
    }
    public int currentExp // 무기의 현재 경험치
    {
        get => myWeaponSaveData.currentExp;
        set => myWeaponSaveData.currentExp = value;
    }
    public int maxExp // 다음 레벨업까지 필요한 경험치
    {
        get => myWeaponSaveData.maxExp;
        set => myWeaponSaveData.maxExp = value;
    }
    public bool isAcquired  // 장비가 획득되었는지 여부를 표시
    {
        get => myWeaponSaveData.isAcquired;
        set => myWeaponSaveData.isAcquired = value;
    }
    public bool isEquipped //장비 장착
    {
        get => myWeaponSaveData.isEquipped;
        set => myWeaponSaveData.isEquipped = value;
    }

    public int overlapCount //장비 중복
    {
        get => myWeaponSaveData.overlapCount;
        set => myWeaponSaveData.overlapCount = value;
    }

    public void WeaponLoad(WeaponSaveData saveData)
    {
        myWeaponSaveData = saveData;
    }
}