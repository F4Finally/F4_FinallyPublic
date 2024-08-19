using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Logo : MonoBehaviour
{
    [SerializeField] Image logoImage;
    [SerializeField] float duration = 2.0f; // 페이드 인 아웃의 시간
    [SerializeField] float fadeOutDelay = 1.0f; // 페이드 인이 끝난 후 페이드 아웃 전 대기 시간
    [SerializeField] float initialDelay = 0.2f; // 시작 전 딜레이


    private void Start()
    {
        if (logoImage == null)
        {
            Debug.LogError("로고 이미지가 없어요");
            return;
        }
        StartCoroutine(FadeInAndOut());
    }

    IEnumerator FadeInAndOut()
    {
        yield return new WaitForSeconds(initialDelay);

        // 페이드 인
        yield return StartCoroutine(LerpColor(0f, 1f, duration));

        // 페이드 아웃 전 대기
        yield return new WaitForSeconds(fadeOutDelay);

        // 페이드 아웃
        yield return StartCoroutine(LerpColor(1f, 0f, duration));
    }

    IEnumerator LerpColor(float startAlpha, float endAlpha, float duration)
    {
        Color color = logoImage.color;

        color.a = startAlpha; // 시작값
        logoImage.color = color; // 0으로 초기화

        float elapsedTime = 0f; // 시작 시간

        while (elapsedTime < duration)
        {
            // 경과한 시간의 비율을 계산
            elapsedTime += Time.deltaTime;
            float time = Mathf.Clamp01(elapsedTime / duration);

            // 알파 값 업데이트
            color.a = Mathf.Lerp(startAlpha, endAlpha, time);
            logoImage.color = color;

            yield return null; // 다음 프레임까지 대기
        }

        color.a = endAlpha; // 종료 값
        logoImage.color = color;
    }

}
