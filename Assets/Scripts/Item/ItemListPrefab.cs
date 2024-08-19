using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemListPrefab : MonoBehaviour
{
    public Image Icon; // 아이템 아이콘 이미지
    public TextMeshProUGUI ItemName; // 아이템 이름 텍스트
    public TextMeshProUGUI ItemAmount; // 아이템 수량 텍스트
    public int key = 0;

    // 아이템 정보를 초기화하는 메서드
    public void Initialize(Sprite icon, string itemName, int amount)
    {
        Icon.sprite = icon;
        ItemName.text = itemName;
        ItemAmount.text = amount.ToString();
    }
}