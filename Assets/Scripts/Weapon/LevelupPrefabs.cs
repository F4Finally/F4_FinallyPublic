using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class LevelupPrefabs : MonoBehaviour
{
    public TextMeshProUGUI CropsNumTxt;
    public int Index;
    public int amount = 0;

    [SerializeField] UnityEvent pressEvent;
    float clickTime;
    float minClickTime = 1;
    float pressCoolTime = 0.1f;
    bool isClick = false;

    public void GetItemData(int amount)
    {
        this.amount = amount;
        CropsNumTxt.text = amount.ToString();
    }


    private void Update()
    {
        if(isClick)
        {
            clickTime += Time.deltaTime;

            if(clickTime >= minClickTime)
            {
                PressCool();
            }
        }
    }

    void PressCool()
    {
        if(clickTime >= minClickTime + pressCoolTime)
        {
            pressEvent.Invoke();
            clickTime -= pressCoolTime;
        }
    }

    public void ButtonDown()
    {
        isClick = true;
    }

    public void ButtonUp()
    {
        isClick= false;
    }
}
