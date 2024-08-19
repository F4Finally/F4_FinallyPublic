using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemPrefab : MonoBehaviour
{
    public Image Icon; // 아이템 아이콘 이미지
    public TextMeshProUGUI ItemNumTxt; // 아이템 수량을 표시하는 텍스트
    public GameObject LockPanel; // 잠금 패널

    private ItemData itemData; // 아이템 데이터
    private ItemSaveData itemSaveData;
    

    // 아이템 데이터와 시설 레벨을 기반으로 초기화
    public void Initialize(ItemData itemData)
    {
        this.itemData = itemData; // 아이템 데이터 저장
        Icon.sprite = Resources.Load<Sprite>(itemData.mySprPath); // 아이콘 이미지 로드
    }

    // 수량 텍스트 업데이트
    public void UpdateItemNumTxt(int currentAmount, int maxAmount)
    {
        ItemNumTxt.text = $"{currentAmount}/{maxAmount}"; // 현재 수량/최대 수량 표시
    }

    // 잠금 패널 업데이트
    public  void SetLockPanel(bool On)
    {
        LockPanel.SetActive(On); // 활성화 상태에 따라 잠금 패널 표시
    }
}