using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System.Numerics;

public class CompanionUIManager : Singleton<CompanionUIManager>
{
    private Dictionary<string, CompanionCard> companionCards = new Dictionary<string, CompanionCard>();


    [SerializeField] private GameObject companionCardPrefab;
    [SerializeField] private Transform companionCardContainer;
    [SerializeField] private CompanionDetailCard companionDetailCard;

    [Header("Button")]
    [SerializeField] private List<FormationSlot> formationSlots;
    [SerializeField] private Button autoEquipButton;
    public Button promoteAllButton;

    [Header("Position Icons")]
    [SerializeField] private Sprite rangedDPSSprite;
    [SerializeField] private Sprite meleeDPSSprite;
    [SerializeField] private Sprite bufferSprite;
    [SerializeField] private Sprite tankSprite;

    [Header("Skill Icons")]
    public Sprite normalAttackIcon;
    public Sprite criticalAttackIcon;

    [Header("Toggle")]
    [Tooltip("동료슬롯을 어떻게 보여줄지 정하는 토글")]
    public Toggle showAllToggle;

    private bool showAllCompanions = true;
    private FormationSlot currentSelectedSlot;

    private HashSet<string> placedCompanionIds = new HashSet<string>();

    public Sprite GetPositionSprite(Position position)
    {
        return position switch
        {
            Position.RangedDPS => rangedDPSSprite,
            Position.MeleeDPS => meleeDPSSprite,
            Position.Buffer => bufferSprite,
            Position.Tank => tankSprite,
            _ => null
        };
    }

    private void Start()
    {
        showAllToggle.isOn = showAllCompanions;
        showAllToggle.onValueChanged.AddListener(ToggleShowAll);
        CreateAllCompanionCards();
        promoteAllButton.onClick.AddListener(PromoteAllCompanions);
        InitializeFormationSlots();
        autoEquipButton.onClick.AddListener(AutoEquipCompanions);

        RefreshUIState();
    }

    public void CreateAllCompanionCards()
    {
        // 기존 카드 제거
        foreach (Transform child in companionCardContainer)
        {
            Destroy(child.gameObject);
        }
        companionCards.Clear();

        // 모든 동료에 대해 카드 생성
        var allCompanions = CompanionManager.Instance.GetAllCompanions();

        foreach (var companionData in CompanionManager.Instance.GetAllCompanions())
        {
            if (companionData != null)
            {
                CreateCompanionCard(companionData);
            }
        }
        UpdateCompanionCardVisibility();
    }

    private void CreateCompanionCard(CompanionData data)
    {

        GameObject cardObject = Instantiate(companionCardPrefab, companionCardContainer);
        CompanionCard card = cardObject.GetComponent<CompanionCard>();
        if (card != null)
        {

            card.InitializeCard(data);
            companionCards[data.dataSO.companionId] = card;

        }
    }

    public void UpdateCompanionCard(string companionId)
    {
        if (companionCards.TryGetValue(companionId, out CompanionCard card))
        {
            card.UpdateCard();
        }
        else
        {
            Debug.LogWarning($"Card for companion {companionId} not found.");
        }
    }

    public void ShowCompanionDetail(CompanionData data)
    {
        if (companionDetailCard != null)
        {
            data.UpdateStats();
            companionDetailCard.ShowDetail(data);
        }
    }

    public void AcquireCompanion(string companionId)
    {
        CompanionData companionData = CompanionManager.Instance.GetCompanionById(companionId);
        if (companionData != null)
        {
            bool wasAlreadyAcquired = companionData.isAcquired;
            companionData.dupeCount++;
            if (!wasAlreadyAcquired)
            {
                companionData.isAcquired = true;
                CompanionManager.Instance.ApplyCompanionPassiveEffect(companionId);
            }
            UpdateCompanionCard(companionId);
        }
        CompanionManager.Instance.SaveCompanionData();
    }


    public void ToggleShowAll(bool showAll)
    {
        showAllCompanions = showAll;
        UpdateCompanionCardVisibility();
    }
    private void UpdateCompanionCardVisibility()
    {
        foreach (var card in companionCards.Values)
        {
            bool shouldShow = showAllCompanions || card.companionData.isAcquired;
            card.gameObject.SetActive(shouldShow);
        }
    }

    private void PromoteAllCompanions()
    {
        var promotableCompanions = GetPromotableCompanions();
        foreach (var companion in promotableCompanions)
        {
            companion.Promote();
            UpdateCompanionCard(companion.dataSO.companionId);
        }

        CreateAllCompanionCards();
        CompanionManager.Instance.SaveCompanionData();
    }

    private List<CompanionData> GetPromotableCompanions()
    {
        return CompanionManager.Instance.GetAllCompanions()
            .Where(c => c.CanPromote())
            .ToList();
    }


    private void InitializeFormationSlots()
    {
        for (int i = 0; i < formationSlots.Count; i++)
        {
            int index = i; // 클로저를 위한 로컬 변수
            formationSlots[i].slotButton.onClick.AddListener(() => OnFormationSlotClicked(formationSlots[index]));
        }
        UpdateFormationSlots();
    }

    public void UpdateFormationSlots()
    {
        int unlockedSlots = StageManager.Instance.UnlockedSlots;
        var placedCompanions = CompanionManager.Instance.placedCompanions;
        for (int i = 0; i < formationSlots.Count; i++)
        {
            bool isUnlocked = i < unlockedSlots;
            formationSlots[i].slotButton.interactable = isUnlocked;

            if (placedCompanions.TryGetValue(i, out string companionId) && isUnlocked)
            {
                PlaceCompanionInSlot(companionId, formationSlots[i]);
            }
            else if (isUnlocked)
            {
                // 해금된 빈 슬롯 처리
                formationSlots[i].companionId = null;
                UpdateFormationSlotUI(formationSlots[i]);
            }
            else
            {
                // 잠긴 슬롯 처리
                formationSlots[i].companionId = null;
                UpdateFormationSlotUI(formationSlots[i]);
                // 잠긴 슬롯의 UI를 표시하는 추가 로직
            }
        }
    }


    private void OnFormationSlotClicked(FormationSlot slot)
    {
        int slotIndex = formationSlots.IndexOf(slot);
        if (slotIndex < StageManager.Instance.UnlockedSlots)
        {
            currentSelectedSlot = slot;
            ShowCompanionSelectionUI(slot);
        }
    }

    private void ShowCompanionSelectionUI(FormationSlot targetSlot)
    {
        List<CompanionData> availableCompanions = CompanionManager.Instance.GetAcquiredCompanions();

        CreateAllCompanionCards();

        foreach (var card in companionCards.Values)
        {
            bool isAvailable = availableCompanions.Any(c => c.dataSO.companionId == card.companionData.dataSO.companionId);
            card.gameObject.SetActive(isAvailable);


            card.cardButton.onClick.RemoveAllListeners();
            card.cardButton.onClick.AddListener(() => OnCompanionCardClicked(card));
        }
    }



    public void PlaceCompanionInSlot(string companionId, FormationSlot slot)
    {
        if (string.IsNullOrEmpty(companionId) || placedCompanionIds.Contains(companionId))
        {
            return;
        }

        // 기존에 슬롯에 배치된 동료가 있다면 제거
        if (!string.IsNullOrEmpty(slot.companionId))
        {
            RemoveCompanionFromSlot(slot);
        }

        slot.companionId = companionId;
        UpdateFormationSlotUI(slot);
        CompanionManager.Instance.PlaceCompanion(companionId, formationSlots.IndexOf(slot));
        CompanionManager.Instance.SpawnCompanionInGame(companionId, formationSlots.IndexOf(slot));
        placedCompanionIds.Add(companionId);
    }
    private void RemoveCompanionFromSlot(FormationSlot slot)
    {
        if (!string.IsNullOrEmpty(slot.companionId))
        {
            CompanionManager.Instance.RemoveCompanion(slot.companionId);
            CompanionManager.Instance.DespawnCompanionInGame(slot.companionId);
            placedCompanionIds.Remove(slot.companionId);
            slot.companionId = null;
            UpdateFormationSlotUI(slot);
        }
    }





    private void UpdateFormationSlotUI(FormationSlot slot)
    {
        int slotIndex = formationSlots.IndexOf(slot);
        bool isUnlocked = slotIndex < StageManager.Instance.UnlockedSlots;

        if (!isUnlocked)
        {
            // 해금되지 않은 슬롯은 기존 이미지를 유지
            return;
        }

        if (string.IsNullOrEmpty(slot.companionId))
        {
            slot.companionImage.sprite = null;
            slot.companionImage.color = new Color(1, 1, 1, 0);
        }
        else
        {
            CompanionData companionData = CompanionManager.Instance.GetCompanionById(slot.companionId);
            if (companionData != null)
            {
                slot.companionImage.sprite = companionData.dataSO.image;
                slot.companionImage.color = Color.white;
            }
            else
            {
                Debug.LogWarning($"Companion with ID {slot.companionId} not found.");
                slot.companionId = null;
                slot.companionImage.sprite = null;
                slot.companionImage.color = new Color(1, 1, 1, 0);
            }
        }
    }

    private void AutoEquipCompanions()
    {
        ClearAllSlots();

        List<CompanionData> sortedCompanions = CompanionManager.Instance.GetAcquiredCompanions()
            .OrderByDescending(c => c.GetCurrentStats().combatPower)
            .Take(StageManager.Instance.UnlockedSlots)
            .ToList();

        for (int i = 0; i < sortedCompanions.Count; i++)
        {
            PlaceCompanionInSlot(sortedCompanions[i].dataSO.companionId, formationSlots[i]);
        }

        // 모든 슬롯의 UI를 업데이트
        foreach (var slot in formationSlots)
        {
            UpdateFormationSlotUI(slot);
        }

        CreateAllCompanionCards();
    }

    private void ClearAllSlots()
    {
        foreach (var slot in formationSlots)
        {
            RemoveCompanionFromSlot(slot);
        }
        placedCompanionIds.Clear();
    }
    private void OnCompanionCardClicked(CompanionCard selectedCard)
    {
        if (currentSelectedSlot != null)
        {
            // 기존 동료 제거
            RemoveCompanionFromSlot(currentSelectedSlot);

            // 새 동료 배치
            PlaceCompanionInSlot(selectedCard.companionData.dataSO.companionId, currentSelectedSlot);

            // UI 업데이트
            UpdateFormationSlotUI(currentSelectedSlot);
            CreateAllCompanionCards(); // 모든 카드 상태 업데이트

            currentSelectedSlot = null;

            foreach (var quest in QuestManager.Instance.GetActiveQuests())
            {
                if (quest.Type == QuestType.ETC && quest.subType == SubType.ECompanion)
                {
                    QuestManager.Instance.UpdateQuestProgress(quest.ID, 1);
                }
            }
        }
    }


    public void RefreshUIState()
    {
        UpdateFormationSlots();
        CreateAllCompanionCards();
    }






}
