using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CloseBtn : MonoBehaviour
{
    [SerializeField] Toggle[] keyToggles;

    public void ClosePopup()
    {
        for (int i = 0; i < keyToggles.Length; i++) keyToggles[i].isOn = false;
    }
}
