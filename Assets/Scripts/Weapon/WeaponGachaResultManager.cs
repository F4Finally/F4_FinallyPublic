using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGachaResultManager : MonoBehaviour
{
    public static WeaponGachaResultManager Instance { get; private set; }

    public Transform slotParent;  // 슬롯들이 배치될 부모 객체
    public GameObject weaponGachaResultPrefab;  // 슬롯 프리팹

    private List<WeaponGachaResult> slotList = new List<WeaponGachaResult>();

    private void Awake()
    {
        // 싱글톤 인스턴스 할당
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("WeaponGachaResultManager 인스턴스가 두 개 이상 존재합니다. 중복 인스턴스를 삭제합니다.");
            Destroy(gameObject);
        }
    }

    public void DisplayGachaResults(List<GachaWeapon> gachaWeapons)
    {
        ClearPreviousResults();

        // 무기들을 Grade와 SubGrade 기준으로 정렬
        gachaWeapons.Sort((a, b) =>
        {
            int gradeComparison = a.Grade.CompareTo(b.Grade);
            if (gradeComparison != 0)
            {
                return gradeComparison;
            }
            return a.SubGrade.CompareTo(b.SubGrade);
        });

        foreach (var weapon in gachaWeapons)
        {
            GameObject slotObj = Instantiate(weaponGachaResultPrefab, slotParent);
            WeaponGachaResult slot = slotObj.GetComponent<WeaponGachaResult>();

            if (slot != null)
            {
                slot.ShowGachaResult(weapon); // 각 슬롯에 뽑힌 장비 데이터 전달
                slotList.Add(slot);
            }
        }
    }

    private void ClearPreviousResults()
    {
        foreach (var slot in slotList)
        {
            Destroy(slot.gameObject);
        }
        slotList.Clear();
    }
}