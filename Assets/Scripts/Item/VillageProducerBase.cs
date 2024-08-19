using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class VillageProducerBase : MonoBehaviour
{
    [SerializeField]
    private VillageManager villageManager;
    public GameManager gameManager;
    public bool isFarmProducer;

    [Header("UI")]
    public Button StrengtheningBtn; // 시설 강화 버튼
    public Button GatherInBtn;      // 물품 수확 버튼
    public GameObject StrengtheningPopup;
    public TextMeshProUGUI StrengtheningPopupText; // 강화 팝업의 텍스트 추가

    [Header("No Panel")]
    public GameObject NoGaugePanel; // 시설게이지 부족
    public GameObject NoMoneyPanel; // 돈부족

    public Transform ItemContainer;  // 아이템을 배치할 컨테이너
    public Transform InvenContainer; // 인벤토리 컨테이너

    [Header("Level")]
    public TextMeshProUGUI FacilityLvNum;      // 시설 레벨 텍스트
    public RectTransform FillArea;             // 시설 레벨 게이지
    public TextMeshProUGUI ProductionPercent;  // 시설 레벨 게이지 텍스트
    public float maxFillAmount { get; set; } = 10000f;      // 최대 게이지 값
    public float currentFillAmount = 0f;    // 현재 게이지 값
    public int facilityLevel = 1;           // 초기 시설 레벨
    public BigInteger baseCost = new BigInteger(200000); // 초기 비용


    public abstract bool ShouldActivateItem(int itemIndex, int facilityLevel);

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // 시설 강화 버튼 클릭 시 호출
    public void OnStrengtheningBtnClicked() //시설 강화버튼 - 게이지MAX / 강화팝업 -> 돈도 있어야함
    {
        BigInteger requiredMoney = baseCost + (new BigInteger(50000) * (facilityLevel - 1));

        if (currentFillAmount >= maxFillAmount) // 최대 게이지 값을 초과하면
        {
            // 강화 팝업 텍스트 업데이트
            StrengtheningPopupText.text = $"골드 {BigIntegerUtils.FormatBigInteger(requiredMoney)}를 소모해\n시설을 강화하시겠습니까?";


            StrengtheningPopup.SetActive(true);
        }
        else
        {
            NoGaugePanel.SetActive(true); // 강화에 필요한 게이지가 부족합니다.
        }

        UpdateUI(); // UI 업데이트
    }



    public void OnStrengtheningPopupYesClicked() // 시설강화팝업 YES
    {
        BigInteger requiredMoney = baseCost + (new BigInteger(50000) * (facilityLevel - 1));

        if (gameManager.CurplayerData.Coin >= requiredMoney)
        {
            gameManager.CurplayerData.Coin -= requiredMoney; // 돈 차감

            LevelUpFacility(); // 레벨업 수행
            villageManager.UpdateCoinText();  // UI 업데이트

            StrengtheningPopup.SetActive(false); // 팝업 닫기
        }
        else
        {
            NoMoneyPanel.SetActive(true); // 강화에 필요한 게이지가 필요합니다

            StrengtheningPopup.SetActive(false); // 팝업 닫기
        }


    }


    // 시설 레벨업
    public void LevelUpFacility() // 마참내 시설 레벨업
    {
        facilityLevel++; // 시설 레벨 증가
        currentFillAmount = currentFillAmount - maxFillAmount; // 레벨업 하면 현재 게이지값-맥스값

        maxFillAmount += 5000f; // maxFillAmount 증가


        // 패널 활성화 로직 추가
        villageManager.ActivateEquipPanel();

        // 모든 아이템 프리팹의 최대 수량 업데이트 및 활성화 상태 재설정
        foreach (var kvp in villageManager.itemInstances)
        {
            var itemData = villageManager.itemData[kvp.Key];
            if (itemData.isFarmProduct != isFarmProducer) continue;

            var itemPrefab = kvp.Value;
            var _itemSaveData = villageManager.itemSaveData[kvp.Key];
            itemPrefab.UpdateItemNumTxt(_itemSaveData.CurrentAmount, villageManager.ReturnMaxAmount(kvp.Key, facilityLevel));

            SetStackItemActive(kvp.Key, itemPrefab, _itemSaveData);
        }


        villageManager.UpdateCoinText();
        UpdateUI();
        villageManager.SaveVillageData();

    }

    
    void SetStackItemActive (int itemId, ItemPrefab prefab, ItemSaveData data)
    {
        if (ShouldActivateItem(itemId, facilityLevel))
        {
            prefab.SetLockPanel(false);
            if (data.StackStarted==false)
            {
                data.StackStarted = true;
                data.LastStackTime = System.DateTime.Now; //해금 당시부터 시간가게
            }
        }
        else
        {
            prefab.SetLockPanel(true);
        }
    }

    public void UpdateUI ()
    {
        float fillPercentage = currentFillAmount / maxFillAmount; // 게이지 퍼센트 계산
        if (fillPercentage > 1f)
        {
            fillPercentage = 1f; // 최대치를 넘지 않도록 조정
        }
        FillArea.anchorMax = new UnityEngine.Vector2(fillPercentage, FillArea.anchorMax.y); // 게이지 바 업데이트

        ProductionPercent.text = $"{currentFillAmount}/{maxFillAmount}"; // 생산 퍼센트 텍스트 업데이트
        FacilityLvNum.text = $"Lv. {facilityLevel}"; // 시설 레벨 텍스트 업데이트

    }

    // 아이템 프리팹 생성
    public void CreateItemPrefabs()
    {
        foreach (var kvp in villageManager.itemData)
        {
            if (kvp.Value.isFarmProduct != isFarmProducer) continue;
            var itemObject = Instantiate(villageManager.ItemPrefab, ItemContainer);
            var itemPrefab = itemObject.GetComponent<ItemPrefab>();
            var itemSaveData = villageManager.itemSaveData[kvp.Key];

            itemPrefab.Initialize(kvp.Value);
            villageManager.itemInstances.Add(kvp.Key, itemPrefab);
            itemPrefab.UpdateItemNumTxt(itemSaveData.CurrentAmount, villageManager.ReturnMaxAmount(kvp.Key, facilityLevel));
            SetStackItemActive(kvp.Key, itemPrefab,itemSaveData);
        }
    }

    // 아이템 수확 버튼 클릭 시 호출
    public void OnGatherInBtnClicked()
    {
        villageManager.OnGatherResultPopupOpen(isFarmProducer);

        // 인벤토리 업데이트
        foreach (var kvp in villageManager.itemSaveData)
        {
            if (villageManager.itemData[kvp.Key].isFarmProduct != isFarmProducer) continue;
            int itemId = kvp.Key;
            int amount = kvp.Value.CurrentAmount;

            if (amount > 0)
            {
                if (villageManager.inventory.ContainsKey(itemId))
                {
                    villageManager.inventory[itemId] += amount; // 기존 수량에 추가
                }
                else
                {
                    villageManager.inventory[itemId] = amount; // 새 아이템 추가
                }

                // 수확한 아이템의 Progress 값을 시설 레벨업 게이지에 추가
                currentFillAmount += villageManager.itemData[itemId].Progress * amount;
              
                UpdateUI();

                kvp.Value.CurrentAmount = 0; // 아이템 수량 리셋
                villageManager.itemInstances[itemId].UpdateItemNumTxt(0, villageManager.ReturnMaxAmount(itemId, facilityLevel));

                
            }
        }

        UpdateInventoryUI(); // 인벤토리 UI 업데이트
        villageManager.SaveVillageData();
    }

    // 인벤토리 UI 업데이트
    public void UpdateInventoryUI()
    {
        // 기존 인벤토리 아이템 제거
        foreach (Transform child in InvenContainer)
        {
            Destroy(child.gameObject);
        }

        // 인벤토리 슬롯 생성 및 업데이트
        foreach (var kvp in villageManager.inventory)
        {
            int itemId = kvp.Key;
            int amount = kvp.Value;

            var itemData = villageManager.itemData[itemId]; // 아이템 데이터 가져오기
            if (itemData.isFarmProduct != isFarmProducer) continue;
            var invenObject = Instantiate(villageManager.InvenListPrefab, InvenContainer); // 인벤토리 슬롯 인스턴스화
            var invenPrefab = invenObject.GetComponent<InvenListPrefab>();

            invenPrefab.Icon.sprite = Resources.Load<Sprite>(itemData.mySprPath); // 아이콘 이미지 로드
            invenPrefab.InvenNum.text = amount.ToString(); // 아이템 수량 텍스트 업데이트
        }
    }

}
