using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FarmProducer : VillageProducerBase
{
    public override bool ShouldActivateItem(int itemIndex, int facilityLevel)
    {
        return itemIndex >= 1 && itemIndex <= facilityLevel + 1; // 레벨에 따라 아이템 활성화
    }
}