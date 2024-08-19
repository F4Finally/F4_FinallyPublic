using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum StageState
{
    Normal,
    Boss,
    Transitioning
}

public class StageManager : Singleton<StageManager>
{
    private int _unlockedSlots = 1;
    public int UnlockedSlots
    {
        get
        {
            UpdateUnlockedSlots();
            return _unlockedSlots;
        }
        private set
        {
            if (_unlockedSlots != value)
            {
                _unlockedSlots = value;
                CompanionUIManager.Instance.UpdateFormationSlots();
            }
        }
    }
    public int CurStage = 1;
    public int CurSubStage = 1;

    private int SubStages = 10;  // 각 스테이지 마다 서브 스테이지 
    private int BossSubStage1 = 5; // 보스 나오는 스테이지 
    private int BossSubStage2 = 10; // 보스 나오는 스테이지 
    private StageState currentState = StageState.Normal;

    public event Action<StageState> OnStageStateChanged;
    private int killedMonsters = 0;



    [field: Header("UI")]
    public FadeController fadeController;

    private void Start()
    {
        UpdateStageState();

        CurStage = GameManager.Instance.CurplayerData.MainStage;
        CurSubStage = GameManager.Instance.CurplayerData.SubStage;
        GameManager.Instance.stageManager = this;


    }

    private void Update()
    {
        UpdateStageState();

    }

    public void UpdateUnlockedSlots()
    {
        int previousUnlockedSlots = _unlockedSlots;
        if (CurStage == 1 && CurSubStage >= 5)
        {
            UnlockedSlots = 2;
        }
        // 추후 3, 4번째 슬롯 해금 조건을 여기에 추가
                
    }
    public void SetUnlockedSlots(int slots)
    {
        UnlockedSlots = slots;
    }


    public void AdvanceSubStage()
    {
        CurSubStage++;
        killedMonsters = 0; // 몬스터 초기화
        if (CurSubStage > SubStages)
        {
            CurSubStage = 1;
            AdvanceStage();
        }
        GameManager.Instance.CurplayerData.SubStage = CurSubStage;
        UpdateStageProgressQuest();
        GameManager.Instance.SaveGame();
        UpdateStageState();
        UpdateUnlockedSlots();
        CompanionUIManager.Instance.UpdateFormationSlots();
    }

    public void AdvanceStage()
    {
        CurStage++;
        killedMonsters = 0;
        CurSubStage = 1;
        GameManager.Instance.CurplayerData.MainStage = CurStage;
        GameManager.Instance.CurplayerData.SubStage = CurSubStage;
        GameManager.Instance.SaveGame();
        UpdateStageState();

    }

    private void UpdateStageProgressQuest()
    {
        int CurrentStage = CurStage * 1000 + CurSubStage;

        foreach (var quest in QuestManager.Instance.GetActiveQuests())
        {
            if (quest.Type == QuestType.StageProgress)
            {
                if (CurrentStage >= quest.Objective)
                {
                    QuestManager.Instance.UpdateQuestProgress(quest.ID, quest.Objective);
                }
                else
                {
                    QuestManager.Instance.UpdateQuestProgress(quest.ID, CurrentStage);
                }
            }
        }
    }
    public float GetStatMultiplier()
    {
        // 서브 스테이지마다 2%씩 증가, 메인 스테이지마다 10%씩 증가
        float multiplier = 1 + (CurStage - 1) * 0.3f + (CurSubStage - 1) * 0.05f;
        return multiplier;
    }

    public void UpdateStageState()
    {
        if (CurSubStage == BossSubStage1 || CurSubStage == BossSubStage2)
        {
            SetStageState(StageState.Boss);
        }
        else
        {
            SetStageState(StageState.Normal);
        }
    }

    private void SetStageState(StageState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            OnStageStateChanged?.Invoke(currentState);

            if (newState == StageState.Boss)
            {
                StartCoroutine(BossStageTransition());
            }
        }
    }

    private IEnumerator BossStageTransition()
    {
        // 페이드 아웃
        yield return StartCoroutine(fadeController.FadeOut());

        // 페이드 인
        yield return StartCoroutine(fadeController.FadeIn());
    }


    public StageState GetCurrentState()
    {
        return currentState;
    }

    public int GetMonstersToKill()
    {
        if (currentState == StageState.Boss)
        {
            return 1;
        }
        else
        {
            if (CurSubStage >= 6 && CurSubStage <= 9)
            {
                return (CurSubStage - 5) * 10;
            }
            else
            {
                return CurSubStage * 10;
            }
        }
    }


    public void OnMonsterKilled()
    {
        killedMonsters++;

        foreach (var quest in QuestManager.Instance.GetActiveQuests())
        {
            if (quest.Type == QuestType.MonsterKill)
            {
                QuestManager.Instance.UpdateQuestProgress(quest.ID, 1);
            }
        }

        if (currentState == StageState.Boss)
        {
            // 보스 스테이지에서 보스를 처치하면 다음 스테이지로
            AdvanceSubStage();
        }
        else
        {
            // 일반 스테이지에서 모든 몬스터를 처치했을 때 다음 서브 스테이지로
            if (killedMonsters >= GetMonstersToKill())
            {
                AdvanceSubStage();
            }
        }
    }

    // 스테이지 UI
    public string GetStageText()
    {
        return $"{CurStage}-{CurSubStage}";
    }

    public void UpdateStageUI()
    {
        MainScene.Instance.stageSlider.maxValue = 5;
        MainScene.Instance.stageSlider.value = CurSubStage;
        if (CurSubStage >= 6)
        {
            MainScene.Instance.stageSlider.value = CurSubStage - 5;
        }

        float fillAmount = (float)CurSubStage / 5;
        if (CurSubStage >= 6)
        {
            fillAmount = ((float)CurSubStage - 5) / 5;
        }
        MainScene.Instance.fillImage.fillAmount = fillAmount;

        // 1-5 서브 스테이지에서 슬라이더와 fill이 꽉 차도록 설정
        if (CurSubStage == 5)
        {
            MainScene.Instance.stageSlider.value = 100;
            MainScene.Instance.fillImage.fillAmount = 1f;
        }
        // 1-6 서브 스테이지에서 다시 0이 되도록 설정
        else if (CurSubStage == 6)
        {
            MainScene.Instance.stageSlider.value = 0;
            MainScene.Instance.fillImage.fillAmount = 0f;
        }
        // 1-10 서브 스테이지에서 다시 꽉 차게 설정
        else if (CurSubStage == 10)
        {
            MainScene.Instance.stageSlider.value = 100;
            MainScene.Instance.fillImage.fillAmount = 1f;
        }
    }

}
