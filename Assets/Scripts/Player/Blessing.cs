using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Blessing : MonoBehaviour
{
    public float coinBoost = 1.0f;
    public float attackPowerBoost = 1.0f;
    public float skillCooldownBoost = 1.0f;

    public float blessingDuration = 600.0f; // 10분
    //private WaitForSeconds blessing;
    private Coroutine blessingCoroutine;

    [SerializeField] private Button blessingButton; 
    [SerializeField] private TextMeshProUGUI blessingStatusText;

    private float blessingEndTime;

    private void Awake()
    {
        //blessing = new WaitForSeconds(blessingDuration);
    }
    private void Update()
    {
        if (blessingCoroutine != null)
        {
            UpdateUI();
        }
    }

    public void ShowAd()
    {
       // 버튼 누르면 광고 나오고 축복 시작하게 
       // TODO : 광고 로직도 들어가야함
        StartBlessing();
    }

    // 전체 축복 
    private void StartBlessing()
    {
        if (blessingCoroutine != null)
        {
            StopCoroutine(blessingCoroutine);
        }
        coinBoost = 2.0f;
        attackPowerBoost = 1.5f;
        skillCooldownBoost = 0.7f;
        blessingEndTime = Time.time + blessingDuration;

        // 10분 후 버프 비활성화
        blessingCoroutine = StartCoroutine(BlessingCoroutine());
    }

    private IEnumerator BlessingCoroutine()
    {
        while (Time.time < blessingEndTime)
        {
            yield return null; 
        }
        StopBlessing();
    }

    private void StopBlessing()
    {
        coinBoost = 1.0f;
        attackPowerBoost = 1.0f;
        skillCooldownBoost = 1.0f;
        blessingCoroutine = null;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (blessingCoroutine != null)
        {
            float remainingTime = Mathf.Max(0, blessingEndTime - Time.time);
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            blessingStatusText.text = $"축복 효과 활성화 (남은 시간: {minutes:00}:{seconds:00})";
        }
        else
        {
            blessingStatusText.text = "";
        }
    }
}
