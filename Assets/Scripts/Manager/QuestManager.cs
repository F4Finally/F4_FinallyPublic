using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : Singleton<QuestManager>
{
    public Dictionary<int, Quest> questData;
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private TextMeshProUGUI questObjectiveText;
    [SerializeField] private TextMeshProUGUI questProgressText;
    [SerializeField] private Toggle villageButton; // 마을 버튼
    [SerializeField] private Toggle dungeonButton; // 던전 버튼 
    [SerializeField] private GameObject villageUI; // 자물쇠 같은 UI 
    [SerializeField] private GameObject dungeonUI;
    [SerializeField] private Button completeQuestButton;
    [SerializeField] private GameObject villagePanel; // 마을 해금
    [SerializeField] private GameObject dungeonPanel; // 던전 해금 
    private bool isVillageUnLocked = false;
    private bool isDungeonUnLocked = false;
    private bool isVillageGuideShown = false; 
    private bool isDungeonGuideShown = false;  
    private Color originalTextColor; // 원래 텍스트 색상 저장용
    private Quest currentQuest;


    private void Start() 
    {
        GameManager.Instance.questManager = this;
        originalTextColor = questDescriptionText.color; // 원래 색상 저장
        if (GameManager.Instance.CurQuestSaveData[13].IsCompleted == true)
        {
            ApplyVillageUnlock(false); // 마을 해금
            villageUI.SetActive(false);
            villageButton.interactable = true;
        }
        if (GameManager.Instance.CurQuestSaveData[45].IsCompleted == true) 
        {
            ApplyDungeonUnlock(false);// 던전 해금
            dungeonUI.SetActive(false);
            dungeonButton.interactable = true;
        }
    }
    public void AwakeQuestManager()
    {
        var data = CSVReader.Read(file: "Quest");

        questData = new Dictionary<int, Quest>(data.Count);

        for (int i = 0; i < data.Count; i++)
        {
            var target = new Quest(data[i]);
            questData.Add(target.ID, target);
        }

        Initialize(questData);

    }

    private void Update()
    {
        var activeQuests = GetActiveQuests();
        if (activeQuests.Count > 0)
        {
            currentQuest = activeQuests[0];
            UpdateQuestUI(activeQuests[0]);
        }
    }

    public void Initialize(Dictionary<int, Quest> questData)
    {
        this.questData = questData;
    }

    public List<Quest> GetActiveQuests()
    {
        List<Quest> activeQuests = new List<Quest>();

        foreach (var quest in questData.Values)
        {
            QuestSaveData nowData = GameManager.Instance.CurQuestSaveData[quest.ID];
            if (nowData.IsAccepted && !nowData.IsCompleted) // 수락이 된 것만 완료 된 것도 안가져오기
            {
                activeQuests.Add(quest);
            }
        }

        return activeQuests;
    }

    public void UpdateQuestProgress(int questID, int progressValue)
    {
        if (questData.TryGetValue(questID, out var quest))
        {
            // 퀘스트 타입에 따라 다르게 처리
            switch (quest.Type)
            {
                case QuestType.MonsterKill:
                    UpdateMonsterKillQuest(quest, progressValue);
                    break;

                case QuestType.StageProgress:
                    UpdateStageProgressQuest(quest, progressValue);
                    break;

                case QuestType.StatUpgrade:
                    UpdateStatUpgradeQuest(quest, progressValue);
                    break;

                case QuestType.ETC:
                    UpdateETCQuest(quest, progressValue);
                    break;
            }
        }
    }
    
    // 몬스터 킬 
    private void UpdateMonsterKillQuest(Quest quest, int progressValue)
    {
        QuestSaveData nowData = GameManager.Instance.CurQuestSaveData[quest.ID];
        nowData.nowProgress += progressValue;
        // 목표 달성 여부 확인
        if (nowData.nowProgress >= quest.Objective)
        {
            questDescriptionText.color = Color.red;
            completeQuestButton.interactable = true;
        }
        else
        {
            questDescriptionText.color = originalTextColor;
            completeQuestButton.interactable = false;
        }
    }

    // 스테이지 진행 퀘스트 
    private void UpdateStageProgressQuest(Quest quest, int progressValue)
    {
        QuestSaveData nowData = GameManager.Instance.CurQuestSaveData[quest.ID];
        int currentMainStage = GameManager.Instance.CurplayerData.MainStage;
        int currentSubStage = GameManager.Instance.CurplayerData.SubStage;
        int currentProgress = currentMainStage * 1000 + currentSubStage;

        nowData.nowProgress = Mathf.Max(nowData.nowProgress, currentProgress);
        // 목표 달성 여부 확인
        if (nowData.nowProgress >= quest.Objective)
        {
            questDescriptionText.color = Color.red;
            completeQuestButton.interactable = true;
        }
        else
        {
            questDescriptionText.color = originalTextColor;
            completeQuestButton.interactable = false;
        }
    }

    // 스탯 업글 퀘스트 
    private void UpdateStatUpgradeQuest(Quest quest, int progressValue)
    {
        QuestSaveData nowData = GameManager.Instance.CurQuestSaveData[quest.ID];
        // 현재 스탯 레벨 가져옴 
        progressValue = GetCurrentStatLevel(quest.subType);
        nowData.nowProgress = progressValue;
        // 목표 달성 여부 확인
        if (nowData.nowProgress >= quest.Objective)
        {
            // 진행도가 목표보다 높으면 이미 클리어 
            questDescriptionText.color = Color.red;
            completeQuestButton.interactable = true;
        }
        else
        { 
            questDescriptionText.color = originalTextColor;
            completeQuestButton.interactable = false;
        }
    }

    // 기타 퀘스트
    private void UpdateETCQuest(Quest quest, int progressValue)
    {
        QuestSaveData nowData = GameManager.Instance.CurQuestSaveData[quest.ID];
        nowData.nowProgress = progressValue;
        // 목표 달성 여부 확인
        if (nowData.nowProgress >=quest.Objective)
        {
            // 진행도가 목표보다 높으면 이미 클리어 
            questDescriptionText.color = Color.red;
            completeQuestButton.interactable = true;
        }
        else
        {
            questDescriptionText.color = originalTextColor;
            completeQuestButton.interactable = false;
        }
    }

    private int GetCurrentStatLevel(SubType subType)
    {
        switch (subType)
        {
            case SubType.Health:
                return GameManager.Instance.CurplayerData.HealthLevel; 
            case SubType.Attack:
                return GameManager.Instance.CurplayerData.AttackLevel; 
            case SubType.Defense:
                return GameManager.Instance.CurplayerData.DefenseLevel; 
            default:
                return -1;
        }
    }

    public void CompleteQuest(Quest quest)
    {
        // 플레이어에게 보상 지급
        GiveReward(quest.Reward);
        // 퀘스트 상태 업데이트
        QuestSaveData nowData = GameManager.Instance.CurQuestSaveData[quest.ID];
        nowData.IsAccepted = false;
        nowData.IsCompleted = true;
        Firebase.Analytics.FirebaseAnalytics.LogEvent($"current_quest_{currentQuest.ID}"); // 파이어베이스 이벤트 로그 
        Quest nextQuest = null;
        if (quest.nextQuest != -1) // 다 깼을 때
        {
            GameManager.Instance.CurQuestSaveData[quest.nextQuest].IsAccepted = true;
            nextQuest = questData[quest.nextQuest]; // 다음 퀘스트의 정보를 받아오는 것 
        }

        if (quest.ID == 13) 
        {
            UnlockVillageUI();
        }
        else if (quest.ID == 45) 
        {
            UnlockDungeonUI();
        }
    }
    public void CheckNowQuest()
    {
        switch (currentQuest.Type)
        {
            case QuestType.StageProgress:
                int currentMainStage = GameManager.Instance.CurplayerData.MainStage;
                int currentSubStage = GameManager.Instance.CurplayerData.SubStage;
                int CurrentStage = currentMainStage * 1000 + currentSubStage; // 현재 스테이지 1001 > 이런식으로 나타낼 것 

                if (CurrentStage >= currentQuest.Objective)
                {
                    // 넘었으면 퀘스트 완료 
                    GameManager.Instance.CurQuestSaveData[currentQuest.ID].nowProgress = currentQuest.Objective;
                }
                else
                {
                    GameManager.Instance.CurQuestSaveData[currentQuest.ID].nowProgress = CurrentStage;
                }

                break;
            case QuestType.StatUpgrade:
                int currentLevel = 0;
                switch (currentQuest.subType)
                {
                    case SubType.Attack:
                        currentLevel = GameManager.Instance.CurplayerData.AttackLevel;
                        break;
                    case SubType.Defense:
                        currentLevel = GameManager.Instance.CurplayerData.DefenseLevel;
                        break;
                    case SubType.Health:
                        currentLevel = GameManager.Instance.CurplayerData.HealthLevel;
                        break;
                }
                GameManager.Instance.CurQuestSaveData[currentQuest.ID].nowProgress = currentLevel;
                break;
        }
    }
    public void CompleteButton()
    {
        if (currentQuest != null)
        {
            CompleteQuest(currentQuest);
            if (currentQuest.nextQuest != -1)
            {
                currentQuest = questData[currentQuest.nextQuest];
                CheckNowQuest();
                UpdateQuestUI(currentQuest);
            }
            else
            {
                currentQuest = null;
                UpdateQuestUI(null);
            }
        }
        GameManager.Instance.SaveGame();    
    }

    private void GiveReward(BigInteger reward)
    {
        GameManager.Instance.CurplayerData.Seed += reward;
    }

    public void UpdateQuestUI(Quest quest)
    {
        QuestSaveData nowData = GameManager.Instance.CurQuestSaveData[quest.ID];
        if (quest != null)
        {
            questTitleText.text = quest.Title;
            questDescriptionText.text = quest.Description;
            questObjectiveText.text = quest.Reward.ToString();

            switch (quest.Type)
            {
                case QuestType.MonsterKill:
                    questProgressText.text = $"({nowData.nowProgress}/{quest.Objective})";
                    break;
                case QuestType.StageProgress:
                    questProgressText.text = $"";
                    break;
                case QuestType.StatUpgrade:
                    int currentLevel = 0;
                    switch (quest.subType)
                    {
                        case SubType.Attack:
                            currentLevel = GameManager.Instance.CurplayerData.AttackLevel;
                            break;
                        case SubType.Defense:
                            currentLevel = GameManager.Instance.CurplayerData.DefenseLevel;
                            break;
                        case SubType.Health:
                            currentLevel = GameManager.Instance.CurplayerData.HealthLevel;
                            break;
                    }
                    questProgressText.text = $"({currentLevel}/{quest.Objective})";
                    break;
                default:
                    questProgressText.text = $"";
                    break;
            }
            bool isCompleted = nowData.nowProgress >= quest.Objective;
            questDescriptionText.color = isCompleted ? Color.red : originalTextColor;
            completeQuestButton.interactable = isCompleted;
        }
    }
        // 마을 UI 자물쇠 해제 
        private void UnlockVillageUI()
    {
        if (!isVillageUnLocked)
        {
            isVillageUnLocked = true;
            ApplyVillageUnlock(true);
        }
        
    }

    // 던전 UI 자물쇠 해제 
    private void UnlockDungeonUI()
    {
        if (!isDungeonUnLocked)
        {
            isDungeonUnLocked = true;
            ApplyDungeonUnlock(true);
        }
    }
    private void ApplyVillageUnlock(bool showPanel)
    {
        villageUI.SetActive(false);
        villageButton.interactable = true;
        if (showPanel)
        {
            StartCoroutine(ShowUnlockPanel(villagePanel));
        }
    }

    private void ApplyDungeonUnlock(bool showPanel)
    {
        dungeonUI.SetActive(false);
        dungeonButton.interactable = true;
        if (showPanel)
        {
            StartCoroutine(ShowUnlockPanel(dungeonPanel));
        }
    }

    // 코루틴: 해금 패널을 1초간 활성화 후 비활성화
    IEnumerator ShowUnlockPanel(GameObject panel)
    {
        // 패널을 활성화(true)
        panel.SetActive(true);

        // 1초 대기
        yield return new WaitForSeconds(1f);

        // 패널을 비활성화(false)
        panel.SetActive(false);
    }

}

