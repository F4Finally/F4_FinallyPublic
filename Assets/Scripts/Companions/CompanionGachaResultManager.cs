using System.Collections.Generic;
using UnityEngine;

public class CompanionGachaResultManager : Singleton<CompanionGachaResultManager>
{
    public Transform cardParent; 
    public GameObject companionGachaResultPrefab;

    private List<CompanionGachaResultCard> cardList = new List<CompanionGachaResultCard>();

    public void DisplayGachaResults(List<GachaCompanion> gachaCompanion)
    {
        ClearPreviousResults();

        foreach (var companion in gachaCompanion)
        {
            GameObject cardObj = Instantiate(companionGachaResultPrefab, cardParent);
            CompanionGachaResultCard card = cardObj.GetComponent<CompanionGachaResultCard>();

            if (card != null)
            {
                card.ShowGachaResult(companion); // 각 슬롯에 뽑힌 장비 데이터 전달
                cardList.Add(card);
            }
        }
    }


    private void ClearPreviousResults()
    {
        foreach (var card in cardList)
        {
            Destroy(card.gameObject);
        }
        cardList.Clear();
    }
}
