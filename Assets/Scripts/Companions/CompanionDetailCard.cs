using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class CompanionDetailCard : MonoBehaviour
{
    [Header("Companion Detail Info")]
    public TextMeshProUGUI positionText;                    //
    public TextMeshProUGUI gradeText;                       //
    public TextMeshProUGUI expText;
    public Slider levelGaugeNumSlider;
    public TextMeshProUGUI combatPowerText;                 //
    public TextMeshProUGUI attackText;                      //
    public TextMeshProUGUI defenseText;                     //
    public TextMeshProUGUI healthText;                      //
    public TextMeshProUGUI passiveEffectText;               //
    public TextMeshProUGUI changeLevelNumTxt;

    [Header("Companion Information")]
    public Image positionImage;                             //
    public TextMeshProUGUI levelNumText;                    //
    public Image companionImage;                            //
    public TextMeshProUGUI companionNameText;               //

    [Header("Companion Overlap UI")]
    public TextMeshProUGUI dupeCountText;                   //
    public Slider overlapNumSlider;                         //
    // public Image notificationPointIcon; ui에서 추가 필요

    [Header("Companion Star")]
    public GameObject[] starParents; // 5개의 빈 별 부모 오브젝트
    public Image[] filledStars; // 각 부모 오브젝트의 자식인 노란 별 이미지

    [Header("UI Elements")]
    public Button closeBtn;               //
    public Button levelUpBtn;
    public Button autoSelBtn;
    public Button promotionBtn;           //

    [Header("Skills")]
    public List<SkillUIElement> skillUIElements;

    [System.Serializable]
    public class SkillUIElement
    {
        public Toggle skillToggle;
        public GameObject skillDescriptionPanel;
        public TextMeshProUGUI skillDescriptionText;
    }
    private string GetKoreanGrade(Grade grade)
    {
        return grade switch
        {
            Grade.Common => "노말",
            Grade.Rare => "레어",
            Grade.Epic => "에픽",
            Grade.Unique => "유니크",
            Grade.Legend => "레전드",
            Grade.Mystic => "미스틱",
            _ => "알 수 없음"
        };
    }

    private CompanionData companionData;

    private void Start()
    {
        closeBtn.onClick.AddListener(() => gameObject.SetActive(false));
        //levelUpBtn.onClick.AddListener(); //구현필요 
        promotionBtn.onClick.AddListener(Promote);
        //autoSelBtn.onClick.AddListener();//구현필요 

    }

    public void ShowDetail(CompanionData companionData)
    {
        this.companionData = companionData;
        UpdateUI();
        gameObject.SetActive(true);
        UpdateExpUI(companionData.GetCurrentExp(), companionData.GetRequiredExp());
    }

    private void UpdateUI()
    {
        if (companionData == null) return;
        companionData.UpdateStats();
        positionImage.sprite = CompanionUIManager.Instance.GetPositionSprite(companionData.position);   //
        levelNumText.text = $"Lv. {companionData.GetLevel()}";              //
        companionImage.sprite = companionData.dataSO.image;                 //
        companionNameText.text = companionData.dataSO.dataName;             //
        UpdateStars(companionData.starRating);
        UpdateOverlapUI();

        positionText.text = companionData.dataSO.position.ToString();       //
        gradeText.text = GetKoreanGrade(companionData.dataSO.grade);             //

        Stats currentStats = companionData.GetCurrentStats();
        healthText.text = currentStats.hp.ToString("F0");                      //
        attackText.text = currentStats.attack.ToString("F0");                  //
        defenseText.text = currentStats.defense.ToString("F0");                //
        combatPowerText.text = currentStats.combatPower.ToString("F0");        //


        passiveEffectText.text = companionData.dataSO.passiveEffect.GetDescription(companionData.GetLevel());
        UpdateSkillInfo(companionData);
        UpdatePromotionBtn();
        (positionImage.sprite, positionText.text) = companionData.position switch
        {
            Position.RangedDPS => (CompanionUIManager.Instance.GetPositionSprite(Position.RangedDPS), "원거리형"),
            Position.MeleeDPS => (CompanionUIManager.Instance.GetPositionSprite(Position.MeleeDPS), "근거리형"),
            Position.Buffer => (CompanionUIManager.Instance.GetPositionSprite(Position.Buffer), "지원형"),
            Position.Tank => (CompanionUIManager.Instance.GetPositionSprite(Position.Tank), "방어형"),
            _ => (positionImage.sprite, "Unknown")
        };
    }


    private void UpdateOverlapUI()
    {
        int maxDupeCount = companionData.GetMaxDupeCount(companionData.dataSO.grade);
        dupeCountText.text = $"{companionData.dupeCount}/{maxDupeCount}";

        overlapNumSlider.maxValue = maxDupeCount;
        overlapNumSlider.value = companionData.dupeCount;

        // UI에 notificationPointIcon이 추가되면 아래 코드의 주석을 해제
        // notificationPointIcon.gameObject.SetActive(companionData.dupeCount >= maxDupeCount);
    }

    private void UpdateStars(int starRating)
    {
        for (int i = 0; i < starParents.Length; i++)
        {
            starParents[i].SetActive(i < companionData.maxStars);
            if (i < filledStars.Length)
            {
                filledStars[i].gameObject.SetActive(i < companionData.starRating);
            }
        }
    }
    private void UpdatePromotionBtn()
    {
        if (companionData != null)
        {
            bool canPromote = companionData.CanPromote();
            promotionBtn.interactable = canPromote;
        }
    }
    private void Promote()
    {
        if (companionData != null && companionData.CanPromote())
        {
            companionData.Promote();
            UpdateUI();
            CompanionUIManager.Instance.UpdateCompanionCard(companionData.dataSO.companionId);
        }
    }

    private void UpdateSkillInfo(CompanionData companionData)
    {
        UpdateSkillInfoSlot(0, CompanionManager.Instance.commonSkillData.normalAttackData);
        UpdateSkillInfoSlot(1, CompanionManager.Instance.commonSkillData.criticalAttackData);
        UpdateSkillInfoSlot(2, companionData.dataSO.uniqueSkillData);

    }

    void UpdateSkillInfoSlot(int slotindex, SkillData targetData)
    {
        SkillUIElement uiElement = skillUIElements[slotindex];

        Image skillIcon = uiElement.skillToggle.image;
        skillIcon.sprite = targetData.skillIcon;
        uiElement.skillDescriptionText.text = $"{targetData.skillName}\n\n{targetData.description}";
    }

    public void UpdateExpUI(float currentExp, float requiredExp)
    {
        Debug.Log($"CompanionDetailCard.UpdateExpUI called: currentExp = {currentExp}, requiredExp = {requiredExp}");
        if (levelGaugeNumSlider != null && expText != null)
        {
            levelGaugeNumSlider.value = Mathf.Clamp01(currentExp / requiredExp);
            expText.text = $"{Mathf.Floor(currentExp)}/{Mathf.Floor(requiredExp)}";
            Debug.Log($"UI Updated: Slider value = {levelGaugeNumSlider.value}, Text = {expText.text}");
        }
        else
        {
            Debug.LogError("levelGaugeNumSlider or expText is null");
        }
    }
}
