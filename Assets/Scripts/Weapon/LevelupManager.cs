using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelupManager : MonoBehaviour
{
    [SerializeField] WeaponPopupManager _weaponPopupManager;
    [SerializeField] ItemListManager _itemListManager;
    public LevelupPrefabs[] levelupPrefabs;
    public LevelupPrefabs[] SmallLeverupPrefabs;

    public GameObject NoStonePanel;
    public GameObject NoGaugePanel;

    int controlExp = 0;

    private void Awake()
    {
        Reset();
    }

    private void OnEnable()
    {
        if(_itemListManager.inventory != null && _itemListManager.inventory.Count != 0)
        {
            CheckItemData();
        }
        else
        {
            Reset();
        }
    }

    private void OnDisable()
    {
        expReset();

        Reset();
    }

    void expReset()
    {
        if (controlExp > 0)
        {
            _weaponPopupManager.ExpDown(controlExp);
        }
        else if (controlExp < 0)
        {
            _weaponPopupManager.ExpUp(controlExp);
        }

        controlExp = 0;
    }

    private void CheckItemData()
    {
        for(int i = 0; i < levelupPrefabs.Length; i++)
        {
            if(_itemListManager.inventory.ContainsKey(levelupPrefabs[i].Index))
            {
                levelupPrefabs[i].GetItemData(_itemListManager.inventory[levelupPrefabs[i].Index]);
            }
        }
    }

    private void Reset()
    {
        for(int i = 0; i < levelupPrefabs.Length; i++)
        {
            levelupPrefabs[i].GetItemData(0);
            SmallLeverupPrefabs[i].GetItemData(0);
        }
    }

    public void AddLevel(int num)
    {
        if (levelupPrefabs[num].amount > 0)
        {
            levelupPrefabs[num].GetItemData(levelupPrefabs[num].amount - 1);
            SmallLeverupPrefabs[num].GetItemData(SmallLeverupPrefabs[num].amount + 1);

            int exp = (num + 1) * 10;
            _weaponPopupManager.ExpUp(exp);
            controlExp += exp;
        }
    }

    public void RemoveLevel(int num)
    {
        if(SmallLeverupPrefabs[num].amount > 0)
        {
            levelupPrefabs[num].GetItemData(levelupPrefabs[num].amount + 1);
            SmallLeverupPrefabs[num].GetItemData(SmallLeverupPrefabs[num].amount - 1);

            int exp = (num + 1) * 10;
            _weaponPopupManager.ExpDown(exp);
            controlExp -= exp;
        }
    }

    public void LevelReset()
    {
        for (int i = 0; i < levelupPrefabs.Length; i++)
        {
            levelupPrefabs[i].GetItemData(levelupPrefabs[i].amount + SmallLeverupPrefabs[i].amount);
        }

        for (int i = 0; i < SmallLeverupPrefabs.Length; i++)
        {
            SmallLeverupPrefabs[i].GetItemData(0);
        }

        expReset();
    }

    public void AllLevelClick()
    {
        int exp = 0;

        for (int i = 0; i < levelupPrefabs.Length; i++)
        {
            if (levelupPrefabs[i].amount > 0)
            {
                exp += ((i + 1) * 10) * levelupPrefabs[i].amount;

                SmallLeverupPrefabs[i].GetItemData(SmallLeverupPrefabs[i].amount + levelupPrefabs[i].amount);
                levelupPrefabs[i].GetItemData(0);
            }
        }

        if (exp == 0 )
        {
            NoStonePanel.SetActive(true); // 광석이 없어요
        }
        _weaponPopupManager.ExpUp(exp);
        controlExp += exp;
    }


    public void LevelBtClick()
    {
        controlExp = 0;

        bool canLevelUp = false; // 레벨업 조건을 확인할 변수

        // 레벨업 가능 여부를 확인하는 루프 
        for (int i = 0; i < levelupPrefabs.Length; i++)
        {
            if (SmallLeverupPrefabs[i].amount > 0)
            {
                canLevelUp = true;
                break; // 레벨업이 가능하면 더 이상 확인할 필요 없음
            }
        }

        // 레벨업 가능 여부에 따른 처리
        if (canLevelUp)
        {
            for (int i = 0; i < levelupPrefabs.Length; i++)
            {
                if (_itemListManager.inventory.ContainsKey(levelupPrefabs[i].Index))
                {
                    _itemListManager.inventory[levelupPrefabs[i].Index] = levelupPrefabs[i].amount;
                }
            }

            for (int i = 0; i < SmallLeverupPrefabs.Length; i++)
            {
                SmallLeverupPrefabs[i].GetItemData(0);
            }

            _itemListManager.DataUpdate();
        }
        else
        {
            NoGaugePanel.SetActive(true); // 레벨업이 불가능하면 NoGaugePanel을 활성화 
        }
    }

    //기존 레벨업 코드
    //public void LevelBtClick()
    //{
    //    controlExp = 0;

    //    for (int i = 0; i < levelupPrefabs.Length; i++)
    //    {
    //        if (_itemListManager.inventory.ContainsKey(levelupPrefabs[i].Index))
    //        {
    //            _itemListManager.inventory[levelupPrefabs[i].Index] = levelupPrefabs[i].amount;
    //        }
    //    }

    //    for (int i = 0; i < SmallLeverupPrefabs.Length; i++)
    //    {
    //        SmallLeverupPrefabs[i].GetItemData(0);
    //    }

    //    _itemListManager.DataUpdate();
    //}

}
