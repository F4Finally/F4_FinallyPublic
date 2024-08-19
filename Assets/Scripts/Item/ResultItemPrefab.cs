using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultItemPrefab : MonoBehaviour
{
    [SerializeField] Image iCon;
    [SerializeField] TMP_Text resultNum;

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void UiSet(Sprite image, int num)
    {
        iCon.sprite = image;
        resultNum.text = num.ToString();
    }
}
