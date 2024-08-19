using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleInteractable : MonoBehaviour
{

    [SerializeField] private Toggle myToggle; // 인스펙터뷰에서 직접 할당하여 캐싱
    [SerializeField] private Image myOnIcon;
    public void OnToggleChanged(bool isOn)
    {
        if(isOn)
        {
            //GetComponent<Toggle>().interactable = false; 겟컴포넌트는 실시간으로 반복 연산을 하기 때문에 연산량에 피해를 줄 수 있다. 연산량이 많아지는 원인이 된다.
            myToggle.interactable = false;
            myOnIcon.enabled = true;
        }
        else
        {
            //GetComponent<Toggle>().interactable = true;
            myToggle.interactable = true;
            myOnIcon.enabled = false;
        }
    }
}
