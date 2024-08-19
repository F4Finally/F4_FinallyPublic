using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePopup : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(HideNoOneSyntheticPanel(gameObject, 1f)); // 1초 후 패널 비활성화
    }
    private IEnumerator HideNoOneSyntheticPanel(GameObject NoOneSyntheticPanel, float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

}

