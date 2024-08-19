using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class CompanionCard : MonoBehaviour
{
    [Header("Companion Information")]
    public Image positionImage;
    public TextMeshProUGUI levelNumText;
    public Image companionImage;
    public TextMeshProUGUI companionNameText;
    
    [Header("Grade Color")]
    public Image gradeColorImage; // 등급 색상을 적용할 이미지

    [Header("Companion UI")]
    public Image companionIconOff;
    public Button cardButton;

    [Header("Companion Overlap UI")]
    public TextMeshProUGUI dupeCountText;
    public Slider overlapNumSlider;
    public Image notificationPointIcon;

    [Header("Companion Star")]
    public GameObject[] starParents; // 5개의 빈 별 부모 오브젝트
    public Image[] filledStars; // 각 부모 오브젝트의 자식인 노란 별 이미지

    public CompanionData companionData;


    public  void InitializeCard(CompanionData data)
    {
        companionData = data;
        UpdateCard();
    }

    public void UpdateCard()
    {

        if (companionData != null)
        {
            string companionId = companionData.dataSO.companionId;
            positionImage.sprite = CompanionUIManager.Instance.GetPositionSprite(companionData.position);

            levelNumText.text = $"Lv. {companionData.GetLevel()}";
            companionImage.sprite = companionData.dataSO.image;
            companionNameText.text = companionData.dataSO.dataName;


            cardButton.onClick.RemoveAllListeners();
            cardButton.onClick.AddListener(OnCardClicked);

            UpdateStars(companionData.starRating);
            UpdateOverlapUI();
            UpdateGradeColor();
            SetIconOffActive(!companionData.isAcquired);

            cardButton.interactable = true;
            
        }
        else
        {
            SetIconOffActive(true);
           
        }
    }

    private void OnCardClicked()
    {
        CompanionUIManager.Instance.ShowCompanionDetail(companionData);
    }

    public void SetIconOffActive(bool active)
    {
        companionIconOff.gameObject.SetActive(active);
    }

    public void UpdateOverlapUI()
    {

        int maxDupeCount = companionData.GetMaxDupeCount(companionData.dataSO.grade);
        dupeCountText.text = $"{companionData.dupeCount}/{maxDupeCount}";

        overlapNumSlider.maxValue = maxDupeCount;
        overlapNumSlider.value = companionData.dupeCount;

        notificationPointIcon.gameObject.SetActive(companionData.dupeCount >= maxDupeCount);

    }

    protected void UpdateStars(int starRating)
    {
        for (int i = 0; i < starParents.Length; i++)
        {
            starParents[i].SetActive(i < companionData.maxStars);
            if (i < filledStars.Length)
            {
                filledStars[i].gameObject.SetActive(i < starRating);
            }
        }
    }
   
    private void UpdateGradeColor()
    {
        if (gradeColorImage != null && companionData != null)
        {
            if (gradeColors.TryGetValue(companionData.dataSO.grade, out Color color))
            {
                gradeColorImage.color = color;
            }
        }
    }




    private static readonly Dictionary<Grade, Color> gradeColors = new Dictionary<Grade, Color>
    {
        { Grade.Common, Color.white },
        { Grade.Rare, Color.green },
        { Grade.Epic, new Color(0.7f, 0, 1.0f) },
    };

}
