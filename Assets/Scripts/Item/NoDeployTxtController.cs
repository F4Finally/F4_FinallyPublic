using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoDeployTxtController : MonoBehaviour
{
    public GameObject NoDeployTxt; // NoDeployTxt를 참조하는 변수

    public void ShowNoDeployText()
    {
        StartCoroutine(ShowTextCoroutine());
    }

    private IEnumerator ShowTextCoroutine()
    {
        NoDeployTxt.SetActive(true); // NoDeployTxt를 활성화
        yield return new WaitForSeconds(1f); // 1초 대기
        NoDeployTxt.SetActive(false); // NoDeployTxt를 비활성화
    }
}
