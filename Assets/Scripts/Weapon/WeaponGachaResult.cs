using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponGachaResult : MonoBehaviour
{
    [Header("UI Elements")]
    public Image gradeIcon;
    public Image weaponIcon;
    public GameObject starfull1;
    public GameObject starfull2;
    public GameObject starfull3;
    public GameObject starfull4;

    public void ShowGachaResult(GachaWeapon gachaWeapon)
    {
        if (gachaWeapon != null)
        {
            // 뽑힌 장비의 데이터에 따라 UI 업데이트
            UpdateUI(gachaWeapon);
        }
    }

    private void UpdateUI(GachaWeapon gachaWeapon)
    {
        // 등급 아이콘 설정
        gradeIcon.sprite = WeaponManager.Instance.GetGradeIcon(gachaWeapon.Data.grade);

        // 무기 아이콘 설정
        weaponIcon.sprite = gachaWeapon.Data.image;

        // 별 개수 설정 (subGrade에 따라 별 개수 설정)
        UpdateStars(gachaWeapon.Data.subGrade);
    }

    private void UpdateStars(int subGrade)
    {
        // 모든 별 아이콘을 비활성화
        starfull1.SetActive(false);
        starfull2.SetActive(false);
        starfull3.SetActive(false);
        starfull4.SetActive(false);

        // 세부등급에 따라 별 아이콘을 활성화
        if (subGrade >= 1) starfull1.SetActive(true);
        if (subGrade >= 2) starfull2.SetActive(true);
        if (subGrade >= 3) starfull3.SetActive(true);
        if (subGrade >= 4) starfull4.SetActive(true);
    }
}