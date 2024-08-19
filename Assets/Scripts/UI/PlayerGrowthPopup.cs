using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;


public class PlayerGrowthPopup : MonoBehaviour
{
    [SerializeField] Image moveImage;
    [SerializeField] Button[] selectBtn;
    [SerializeField] float duration = 1.0f; // 1초 동안 이동

    private void Start()
    {
        foreach (Button button in selectBtn)
        {
            button.onClick.AddListener(() => StartCoroutine(MoveToButtonPosition(button.transform.position.x)));
        }
    }

    IEnumerator MoveToButtonPosition(float targetX)
    {
        Vector3 startPosition = moveImage.transform.position; // 이미지의 시작 위치
        float startY = startPosition.y; // 고정할 y 값
        float startZ = startPosition.z; // 고정할 z 값
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 경과 시간을 기준으로 비율 계산
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Lerp를 이용해 x 값을 선형적으로 보간
            float newX = Mathf.Lerp(startPosition.x, targetX, t);

            // 새로운 위치를 설정 (y와 z는 고정)
            moveImage.transform.position = new Vector3(newX, startY, startZ);

            // 다음 프레임까지 대기
            yield return null;
        }

        // 이동을 정확하게 목표 위치로 설정 (경과 시간에 의한 오차 방지)
        moveImage.transform.position = new Vector3(targetX, startY, startZ);
    }
}
