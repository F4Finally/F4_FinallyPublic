using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponSaveData
{
    public int weaponID;  // 무기를 고유하게 식별할 ID
    public int level;
    public int currentExp;
    public int maxExp;
    public int overlapCount;

    public bool isAcquired;
    public bool isEquipped;

    public WeaponSaveData() { }

    public WeaponSaveData(int weaponID)
    {
        this.weaponID = weaponID;
        this.level = 1;
        this.currentExp = 0;
        this.maxExp = 1000;
        this.overlapCount = 0;
        this.isAcquired = false;
        this.isEquipped = false;

    }

    public WeaponSaveData(int weaponID, int level, int currentExp, int maxExp, int overlapCount, bool isAcquired, bool isEquipped)
    {
        this.weaponID = weaponID;
        this.level = level;
        this.currentExp = currentExp;
        this.maxExp = maxExp;
        this.overlapCount = overlapCount;
        this.isAcquired = isAcquired;
        this.isEquipped = isEquipped;
    }

    public WeaponSaveData(WeaponDataSO data, bool isEquip)
    {
        this.weaponID = data.weaponID;
        this.level = data.level;
        this.currentExp = data.currentExp;
        this.maxExp = data.maxExp;
        this.overlapCount = data.overlapCount;
        this.isAcquired = data.isAcquired;
        this.isEquipped = isEquip;
    }

    public void DataLog()
    {
        Debug.Log(weaponID + " , " + level + " , " + currentExp + " , " + maxExp + " , " + overlapCount + " , " + isAcquired + " , " + isEquipped);
    }
}

