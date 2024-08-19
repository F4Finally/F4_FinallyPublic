using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour
{
    [Header("Weapon Information")]
    public Image weaponImage;
    public Image gradeIconImage;
    public TextMeshProUGUI levelNumText; // 레벨 표시 텍스트

    [Header("Weapon UI")]
    public Button weaponButton;
    public Image equipIcon; // 장착 아이콘
    public Image weaponIconOff; // 비활성화 아이콘

    [Header("Weapon Overlap UI")]
    public TextMeshProUGUI overlapNumTxt;
    public Slider overlapNumSlider;
    public Image notificationPointIcon;

    [Header("Weapon Star")]
    public GameObject starfull1;
    public GameObject starfull2;
    public GameObject starfull3;
    public GameObject starfull4;

    [Header("Weapon Data")]
    private WeaponDataSO weaponData;

    public void InitializeSlot(WeaponDataSO data, Sprite gradeIcon)
    {
        weaponData = data;
        UpdateSlot(gradeIcon);
    }

    public void UpdateSlot(Sprite gradeIcon)
    {
        if (weaponData != null)
        {
            weaponImage.sprite = weaponData.image;
            gradeIconImage.sprite = gradeIcon;
            levelNumText.text = "Lv. " + weaponData.level.ToString(); // 레벨 표시
            weaponButton.onClick.RemoveAllListeners();
            weaponButton.onClick.AddListener(ShowWeaponDetails);

            UpdateStars(weaponData.subGrade);

            // 장비 소유 여부에 따라 비활성화 아이콘 상태 업데이트
            SetIconOffActive(!weaponData.isAcquired); // 장비를 소유하고 있으면 비활성화 아이콘을 비활성화
            UpdateOverlapUI(); // 중복 UI 업데이트
        }
        else
        {
            // 장비가 없는 경우 비활성화 아이콘만 표시
            SetIconOffActive(true);
        }
    }

    public void UpdateOverlapUI()
    {
        if (weaponData != null)
        {
            overlapNumTxt.text = $"{weaponData.overlapCount}/10";
            overlapNumSlider.maxValue = 10;
            overlapNumSlider.value = weaponData.overlapCount;

            // 슬라이더가 꽉 찼을 때 NotificationPointIcon 활성화
            notificationPointIcon.gameObject.SetActive(weaponData.overlapCount >= 10);
        }
    }
    public void SetIconOffActive(bool active)
    {
        weaponIconOff.gameObject.SetActive(active);
    }

    public int GetWeaponID()
    {
        return weaponData != null ? weaponData.weaponID : -1;
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

    public void SetEquipIconActive(bool active)
    {
        equipIcon.gameObject.SetActive(active);
    }

    private void ShowWeaponDetails()
    {
        WeaponPopupManager.Instance.ShowPopup(weaponData);
    }
}