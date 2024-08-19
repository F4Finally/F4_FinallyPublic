using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{
    [field: Header("DungeonMain")]
    [SerializeField] private TextMeshProUGUI dungeonMainName;
    [SerializeField] private TextMeshProUGUI keyCountMainText;
    [SerializeField] private Button enterMainButton; // 던전 리스트에 있는 입장하기 버튼
    [SerializeField] private TextMeshProUGUI timeText;


    [field: Header("DungeonPopup")]
    [SerializeField] private TextMeshProUGUI dungeonNameText;
    public TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI keyCountText;
    [SerializeField] private Button decreaseLevelButton;
    [SerializeField] private Button increaseLevelButton;
    [SerializeField] private Button sweepButton;
    [SerializeField] private Button enterButton;

    [SerializeField] private List<DungeonInfo> dungeons = new List<DungeonInfo>();
    private int currentDungeonIndex = 0;

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        UpdateKeyTimer();
        UpdateUI();

        UpdateButtonCheck();
    }

    public void UpdateUI()
    {
        DungeonInfo currentDungeon = DungeonManager.Instance.GetCurrentDungeonInfo();
        dungeonMainName.text = currentDungeon.name;
        dungeonNameText.text = currentDungeon.name;
        levelText.text = DungeonManager.Instance.GetCurrentDungeon().CurrentLevel.ToString();
        keyCountMainText.text = $"{GameManager.Instance.CurplayerData.DungeonKeys}/{currentDungeon.maxKeyCount}"; // 플레이어가 갖고 있는 키/던전에서 가질 수 있는 최대 열쇠
        keyCountText.text = $"{GameManager.Instance.CurplayerData.DungeonKeys}/{currentDungeon.maxKeyCount}";
        UpdateKeyTimer();
    }


    //// 난이도 감소 
    //public void DecreaseLevel()
    //{
    //    DungeonManager.Instance.DecreaseDungeonLevel();
    //    UpdateLevelText();
    //}

    //// 난이도 증가
    //public void IncreaseLevel()
    //{
    //    DungeonManager.Instance.IncreaseDungeonLevel();
    //    UpdateLevelText();
    //}

    public void SweepDungeon()
    {
        // 소탕 입장
        DungeonManager.Instance.SweepDungeon();
    }

    public void EnterDungeon()
    {
        // 던전 입장
        DungeonManager.Instance.StartDungeon();
    }

    //private void UpdateLevelText()
    //{
    //    int currentLevel = DungeonManager.Instance.GetCurrentLevel();
    //    levelText.text = $"{currentLevel}";
    //    // 던전 레벨 2이상이면 버튼 활성화
    //    increaseLevelButton.interactable = GameManager.Instance.CurplayerData.DungeonLevel >= 2;
    //}

    void UpdateButtonCheck()
    {
        int currentLevel = DungeonManager.Instance.GetCurrentLevel();

        if(currentLevel <= 1)
        {
            decreaseLevelButton.interactable = false;
        }
        else
        {
            decreaseLevelButton.interactable = true;
        }

        if(currentLevel >= GameManager.Instance.CurplayerData.DungeonLevel)
        {
            increaseLevelButton.interactable = false;
        }
        else
        {
            increaseLevelButton.interactable = true;
        }

        // Debug.Log("최고 레벨 " + GameManager.Instance.CurplayerData.DungeonLevel + "현재 레벨 " + currentLevel);
        if (currentLevel < GameManager.Instance.CurplayerData.DungeonLevel)
        {
            sweepButton.interactable = true;
        }
        else
        {
            sweepButton.interactable = false;
        }
    }


    // 열쇠 메서드
    private void UpdateKeyTimer()
    {
        DungeonInfo currentDungeon = DungeonManager.Instance.GetCurrentDungeonInfo();

        if (GameManager.Instance.CurplayerData.DungeonKeys < currentDungeon.maxKeyCount)
        {
            TimeSpan timeSpan = DateTime.Now - GameManager.Instance.CurplayerData.LastKeyTime;
            double totalSeconds = timeSpan.TotalSeconds;
            int keysToGenerate = (int)(totalSeconds / (6 * 3600)); // 12시간마다 키 생성

            if (keysToGenerate > 0)
            {
                int newKeyCount = Mathf.Min(GameManager.Instance.CurplayerData.DungeonKeys + keysToGenerate, currentDungeon.maxKeyCount);
                int actuallyAddedKeys = newKeyCount - GameManager.Instance.CurplayerData.DungeonKeys;

                GameManager.Instance.CurplayerData.DungeonKeys = newKeyCount;

                if (actuallyAddedKeys > 0)
                {
                    // 마지막 키 생성 시간 업데이트
                    GameManager.Instance.CurplayerData.LastKeyTime = DateTime.Now - TimeSpan.FromSeconds(totalSeconds % (6 * 3600));
                }

                UpdateUI();
            }

            // 다음 키 생성까지 남은 시간 계산
            TimeSpan timeUntilNextKey = TimeSpan.FromHours(6) - (DateTime.Now - GameManager.Instance.CurplayerData.LastKeyTime);
            GameManager.Instance.SaveGame();
            timeText.text = $"{timeUntilNextKey:hh\\:mm\\:ss}";
        }
        else
        {
            timeText.text = "";
        }
    }
}
