using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponPopupManager : Singleton<WeaponPopupManager>
{
    [Header("Weapon Information")]
    public TextMeshProUGUI weaponGradeNameTxt;
    public TextMeshProUGUI weaponNameTxt;
    public Image weaponIconImage;  // 무기 아이콘을 표시할 Image
    public TextMeshProUGUI hpPointNum; //무기 체력
    public TextMeshProUGUI atkPointNum; //무기 공격력
    public TextMeshProUGUI defPointNum; //무기 방어력

    [Header("Weapon Level")]
    public TextMeshProUGUI weaponLevelNumTxt;
    public TextMeshProUGUI changeLevelNumTxt;
    public Slider levelGaugeNumSlider;
    public TextMeshProUGUI levelGaugeNumTxt;

    [Header("Weapon PassiveEffect")]
    public TextMeshProUGUI affectedStatContentTxt; // 영향을 받는 스텟 내용 텍스트
    public TextMeshProUGUI basePercentageBonusContentTxt; // 기본 보너스 내용 텍스트
    public TextMeshProUGUI bonusPerTenLevelsTitleTxt; // 10레벨당 보너스 제목 텍스트
    public TextMeshProUGUI bonusPerTenLevelsContentTxt; // 10레벨당 보너스 내용 텍스트

    [Header("Weapon UI")]
    public Button levelUpBtn;
    public Button equipBtn; // 장착 버튼
    public GameObject detailOff;
    public Button weaponIconPromotionBtn; // 무기 승급 버튼
    public Button AutoSelectionBtn; //자동선택 버튼
    public Button EmptyMineralBubbleBtn; // 전체선택 버튼
    public Image OneChoiceBubble; // 하나만 선택
    public Image OneChoiceTail;
    public TextMeshProUGUI oneChoiceBubbleTxt;

    [Header("Weapon Overlap UI")]
    public TextMeshProUGUI overlapNumTxt;
    public Slider overlapNumSlider;
    public Image notificationPointIcon;
    public Image notificationPointIcon2;

    [Header("Weapon Data")]
    private WeaponDataSO currentWeaponData;
    private bool wasEquipped; // 장착 여부 저장

    [Header("No Panel")]
    public GameObject NoOneSyntheticPanel;
    

    private void Start()
    {
        // levelUpBtn.onClick.AddListener(LevelUp);
        equipBtn.onClick.AddListener(EquipWeapon);
        weaponIconPromotionBtn.onClick.AddListener(PromoteCurrentWeapon);
    }

    public void ShowPopup(WeaponDataSO weaponData)
    {
        currentWeaponData = weaponData;

        weaponGradeNameTxt.text = weaponData.dataName;
        weaponNameTxt.text = weaponData.weaponName;  // 무기 이름
        weaponLevelNumTxt.text = "Lv. " + weaponData.level.ToString();
        changeLevelNumTxt.text = weaponData.level < WeaponDataSO.MaxLevel
            ? "Lv. " + (weaponData.level + 1).ToString()
            : "Max Level";
        levelGaugeNumSlider.maxValue = weaponData.maxExp;
        levelGaugeNumSlider.value = weaponData.currentExp;
        levelGaugeNumTxt.text = weaponData.currentExp + "/" + weaponData.maxExp;
        weaponIconImage.sprite = weaponData.image;  // 무기 아이콘 설정

        hpPointNum.text = NumberFormatter.FormatNumber(weaponData.baseStats.hp);
        atkPointNum.text = NumberFormatter.FormatNumber(weaponData.baseStats.attack);
        defPointNum.text = NumberFormatter.FormatNumber(weaponData.baseStats.defense);

        // 보유효과 정보를 텍스트 형식으로 표시
        var passiveEffect = weaponData.passiveEffect;
        affectedStatContentTxt.text = passiveEffect.affectedStat.ToKorean();
        basePercentageBonusContentTxt.text = $"{passiveEffect.basePercentageBonus:F2}% 증가";
        bonusPerTenLevelsTitleTxt.text = "10레벨당 보너스";
        bonusPerTenLevelsContentTxt.text = $"{passiveEffect.bonusPerTenLevels:F2}%";

        // DetailOff UI 및 장착 버튼 상태 업데이트
        bool isAcquired = weaponData.isAcquired;
        detailOff.SetActive(!isAcquired); // 장비를 획득하지 않은 경우 DetailOff 활성화
        UpdateEquipButtonText(isAcquired);

        UpdateOverlapUI();
        gameObject.SetActive(true);
    }

    private void UpdateOverlapUI()
    {
        if (currentWeaponData != null)
        {
            overlapNumTxt.text = $"{currentWeaponData.overlapCount}/10";
            overlapNumSlider.maxValue = 10;
            overlapNumSlider.value = currentWeaponData.overlapCount;

            // 슬라이더가 꽉 찼을 때 NotificationPointIcon 활성화
            notificationPointIcon.gameObject.SetActive(currentWeaponData.overlapCount >= 10);
            notificationPointIcon2.gameObject.SetActive(currentWeaponData.overlapCount >= 10);
        }
    }

    private void EquipWeapon()
    {
        if (currentWeaponData != null && currentWeaponData.isAcquired) // 장비를 획득한 경우에만 장착 가능
        {
            WeaponManager.Instance.EquipWeapon(currentWeaponData);
            wasEquipped = true;
            UpdateEquipButtonText(wasEquipped);
            foreach (var quest in QuestManager.Instance.GetActiveQuests())
            {
                if (quest.Type == QuestType.ETC && quest.subType == SubType.EWeapon)
                {
                    QuestManager.Instance.UpdateQuestProgress(quest.ID, 1);
                }
            }
            gameObject.SetActive(false); // 팝업 창 닫기
        }
    }

    public void UpdateEquipButtonText(bool isAcquired)
    {
        // 장비를 획득한 경우에만 장착 버튼 활성화
        equipBtn.gameObject.SetActive(isAcquired);
        levelUpBtn.gameObject.SetActive(isAcquired);
        AutoSelectionBtn.gameObject.SetActive(isAcquired);
        EmptyMineralBubbleBtn.gameObject.SetActive(isAcquired);
        OneChoiceBubble.gameObject.SetActive(isAcquired);
        oneChoiceBubbleTxt.gameObject.SetActive(isAcquired);
        OneChoiceTail.gameObject.SetActive(isAcquired);

        if (isAcquired)
        {
            wasEquipped = WeaponManager.Instance.GetEquippedWeapon() == currentWeaponData;
            equipBtn.GetComponentInChildren<TextMeshProUGUI>().text = wasEquipped ? "해제" : "장착";
        }
    }

    private void LevelUp()
    {

    }

    public void ExpUp(int addExp)
    {
        if (currentWeaponData == null || currentWeaponData.level >= WeaponDataSO.MaxLevel)
            return;

        currentWeaponData.currentExp += addExp;

        do
        {
            if (currentWeaponData.currentExp >= currentWeaponData.maxExp)
            {
                currentWeaponData.currentExp -= currentWeaponData.maxExp;
                currentWeaponData.level++;

                // 레벨업 할 때마다 maxExp 증가
                currentWeaponData.maxExp = 1000 + (currentWeaponData.level / 5) * 250;

                // 레벨업 시 스탯 증가
                Stats previousStats = currentWeaponData.baseStats.Clone(); // 이전 스탯 저장
                currentWeaponData.baseStats.IncreaseStats(currentWeaponData.levelUpStats);
            }

            // 레벨이 최대 레벨을 초과하지 않도록 보장
            if (currentWeaponData.level >= WeaponDataSO.MaxLevel)
            {
                currentWeaponData.level = WeaponDataSO.MaxLevel;
                currentWeaponData.currentExp = currentWeaponData.maxExp;
            }
        } while (currentWeaponData.currentExp >= currentWeaponData.maxExp);

        UpdateUI();
        WeaponManager.Instance.UpdateWeaponSlot(currentWeaponData.weaponID); // 슬롯 UI 업데이트 호출
    }

    public void ExpDown(int Exp)
    {
        if (currentWeaponData == null || currentWeaponData.level <= 0)
            return;

        currentWeaponData.currentExp -= Exp;
        Debug.Log("현재 경험치 " + currentWeaponData.currentExp);

        if (currentWeaponData.currentExp < 0)
        {
            do
            {
                currentWeaponData.currentExp = currentWeaponData.maxExp + currentWeaponData.currentExp;
                currentWeaponData.level--;

                // 레벨업 할 때마다 maxExp 증가
                currentWeaponData.maxExp = 1000 + (currentWeaponData.level / 5) * 250;

                // 레벨업 시 스탯 증가
                Stats previousStats = currentWeaponData.baseStats.Clone(); // 이전 스탯 저장
                currentWeaponData.baseStats.DecreaseStats(currentWeaponData.levelUpStats);

                // 레벨이 최대 레벨을 초과하지 않도록 보장
                if (currentWeaponData.level < 1)
                {
                    currentWeaponData.level = 1;
                    currentWeaponData.currentExp = 0;
                }
            } while (currentWeaponData.currentExp < 0);
        }

        UpdateUI();
        WeaponManager.Instance.UpdateWeaponSlot(currentWeaponData.weaponID); // 슬롯 UI 업데이트 호출
    }

    private void PromoteCurrentWeapon()
    {
        if (currentWeaponData != null)
        {
            if (currentWeaponData.overlapCount >= 10)
            {
                WeaponManager.Instance.PromoteWeapon(currentWeaponData);
                UpdateUI(); // 승급 후 UI 업데이트
            }
            else
            {
                NoOneSyntheticPanel.SetActive(true);
               
            }
        }
    }


    private void UpdateUI()
    {
        if (currentWeaponData != null)
        {
            weaponLevelNumTxt.text = "Lv. " + currentWeaponData.level.ToString();
            changeLevelNumTxt.text = currentWeaponData.level < WeaponDataSO.MaxLevel
                ? "Lv. " + (currentWeaponData.level + 1).ToString()
                : "Max Level";
            levelGaugeNumSlider.maxValue = currentWeaponData.maxExp;
            levelGaugeNumSlider.value = currentWeaponData.currentExp;
            levelGaugeNumTxt.text = currentWeaponData.currentExp + "/" + currentWeaponData.maxExp;

            // 무기 스탯 UI 업데이트
            hpPointNum.text = NumberFormatter.FormatNumber(currentWeaponData.baseStats.hp);
            atkPointNum.text = NumberFormatter.FormatNumber(currentWeaponData.baseStats.attack);
            defPointNum.text = NumberFormatter.FormatNumber(currentWeaponData.baseStats.defense);

            // 보유 효과 정보 업데이트
            var passiveEffect = currentWeaponData.passiveEffect;
            affectedStatContentTxt.text = passiveEffect.affectedStat.ToKorean();
            basePercentageBonusContentTxt.text = $"{passiveEffect.basePercentageBonus:F2}% 증가";
            bonusPerTenLevelsTitleTxt.text = "10레벨당 보너스";
            bonusPerTenLevelsContentTxt.text = $"{passiveEffect.bonusPerTenLevels:F2}%";

            // 장착 상태에 따른 버튼 텍스트 업데이트
            wasEquipped = WeaponManager.Instance.GetEquippedWeapon() == currentWeaponData;
            UpdateEquipButtonText(currentWeaponData.isAcquired);

            UpdateOverlapUI(); // 겹침 수 UI 업데이트
        }
    }

    public static class NumberFormatter
    {
        private static readonly char[] units = { ' ', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        public static string FormatNumber(float number)
        {
            int unitIndex = 0;

            // 1000 단위로 나눌 때마다 unitIndex 증가
            while (number >= 1000 && unitIndex < units.Length - 1)
            {
                number /= 1000;
                unitIndex++;
            }

            if (unitIndex == 0)
            {
                // 1000 이하인 경우 그냥 숫자만 출력
                return $"{number:0}";
            }
            else
            {
                // 1000 이상인 경우 단위를 붙여서 출력
                return $"{number:0.#}{units[unitIndex]}";
            }
        }
    }

}






