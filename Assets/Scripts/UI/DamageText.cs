using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class DamageText : MonoBehaviour
{
    public TextMeshPro damageText; // 데미지 텍스트 
    public Vector3 offset = new Vector3(0, 0.5f, 0); // 플레이어 머리 위 오프셋
    float time; // 나오는 시간 
    float fadeTime = 1f;

    // 테스트  
    private float minFontSize = 5f; // critical = 1.2
    private float sizeChangeSpeed = 1.5f; // 2 

    private float moveSpeed = 0.15f;
    private float alphaSpeed = 1.5f;
    public float destroyTime = 0.4f;

    private Color textColor;

    private void Start()
    {
        textColor = damageText.color;
    }

    public void ShowText(float damage, Vector3 position,bool critical)
    {
        damageText.text = damage.ToString("N0");
        transform.position = position + offset;

        damageText.fontSize = minFontSize;
        textColor.a = 1f;
        damageText.color = textColor;

        if (critical)
        {
            damageText.fontSize *= 1.2f;
            damageText.color = Color.red;
        }

        time = 0;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        //time += Time.deltaTime;
        //// 페이드 아웃 효과
        //float alpha = Mathf.Lerp(1f, 0f, time / fadeTime);

        //if (time >= fadeTime)
        //{
        //    gameObject.SetActive(false);
        //}
        time += Time.deltaTime;

        // 위로 이동
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));

        // 폰트 크기 조절
        float targetFontSize = Mathf.Lerp(1f, minFontSize, time / destroyTime);
        damageText.fontSize = targetFontSize;

        // 알파값 조절
        textColor.a = Mathf.Lerp(1f, 0f, time / destroyTime);
        damageText.color = textColor;

        // destroyTime 이후 비활성화
        if (time >= destroyTime)
        {
            gameObject.SetActive(false);
        }

    }
}
