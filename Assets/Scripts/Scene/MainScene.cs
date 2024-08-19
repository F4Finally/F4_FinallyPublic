using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : BaseScene
{
    public static MainScene Instance { get; private set; }

    [SerializeField] private EnemySpawn enemySpawn;
    [SerializeField] private DungeonEnemySpawn dungeonEnemySpawn;

    public TMP_InputField villageNameInput; // 마을 이름 입력 필드
    public TMP_InputField playerNameInput; // 닉네임 입력 필드
    public Button saveButton; // 저장 버튼

    public GameObject tutorialPanel; // 튜토리얼 패널

    public EnemySpawnBase returnNowEnemySpawn()
    {
        if (DungeonManager.Instance.isDungeonActive)
        {
            return dungeonEnemySpawn;
        }
        else
        {
            return enemySpawn;
        }
    }

    public List<Enemy> returnNowActiveEnemies ()
    {
        if (DungeonManager.Instance.isDungeonActive)
        {
            return dungeonEnemySpawn.activeMonsters;
        }
        else
        {
            return enemySpawn.activeMonsters;
        }
    }
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI NickName;
    public Slider stageSlider;
    public Image fillImage;
    protected override void Init()
    {
        SceneType = Scene.Game;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            //Destroy(gameObject);
        }

    }

    private void Start()
    {
        UpdateStageText();
        StageManager.Instance.UpdateStageUI();
        string villageName = PlayerPrefs.GetString("VillageName", "Unknown Village");
        string nickName = PlayerPrefs.GetString("NickName", "Unknown");
        GameManager.Instance.CurplayerData.NickName = nickName;
        GameManager.Instance.CurplayerData.VillageName = villageName;

        NickName.text = GameManager.Instance.CurplayerData.NickName;

        SoundManager.Instance.PlayAudio(Sound.Bgm, "Main");

        // UI 업데이트
        NickName.text = nickName;
        villageNameInput.text = villageName;
        playerNameInput.text = nickName;

        // 저장 버튼 클릭 이벤트 등록
        saveButton.onClick.AddListener(SaveChanges);

        // 입력 필드와 저장 버튼을 활성화 상태로 시작
        villageNameInput.gameObject.SetActive(true);
        playerNameInput.gameObject.SetActive(true);
        saveButton.gameObject.SetActive(true);

        // 튜토리얼 패널 표시 여부 결정
        ShowTutorialIfNeeded();
    }

    private void Update()
    {
        UpdateStageText();
        StageManager.Instance.UpdateStageUI();
    }

    private void UpdateStageText()
    {
        if (stageText != null)
        {
            stageText.text = StageManager.Instance.GetStageText();
        }
    }


    public override void Clear()
    {

    }

    // 수정된 데이터를 저장하는 메서드
    private void SaveChanges()
    {
        string newVillageName = villageNameInput.text.Trim();
        string newPlayerName = playerNameInput.text.Trim();

        // 입력값 검증
        if (string.IsNullOrEmpty(newVillageName))
        {
            Debug.LogWarning("Village name is empty.");
            return;
        }

        if (string.IsNullOrEmpty(newPlayerName))
        {
            Debug.LogWarning("Player name is empty.");
            return;
        }

        // PlayerPrefs에 저장
        PlayerPrefs.SetString("VillageName", newVillageName);
        PlayerPrefs.SetString("NickName", newPlayerName);
        PlayerPrefs.Save();

        // GameManager의 데이터 업데이트
        if (GameManager.Instance != null && GameManager.Instance.CurplayerData != null)
        {
            GameManager.Instance.CurplayerData.VillageName = newVillageName;
            GameManager.Instance.CurplayerData.NickName = newPlayerName;
        }

        // UI 업데이트
        NickName.text = newPlayerName;
    }

    private void ShowTutorialIfNeeded()
    {
        // 튜토리얼을 이미 본 적이 있는지 확인
        if (!PlayerPrefs.HasKey("MainTutorial"))
        {
            // 튜토리얼 패널을 보여줌
            tutorialPanel.SetActive(true);
            Time.timeScale = 0f;
            // 튜토리얼 본 적이 있다고 기록
            PlayerPrefs.SetInt("MainTutorial", 1);
            PlayerPrefs.Save();
        }
        else
        {
            // 이미 튜토리얼을 본 적이 있다면 패널을 비활성화
            tutorialPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
    public void CloseTutorial()
    {
        // 게임 시간 다시 시작
        Time.timeScale = 1f;
    }
    // 씬 전환시 처리할 메서드들 다 해놔야함
    // 씬 왔을 때 초기화 될 메서드들도 
    // 여러개의 컴포넌트들을 초기화를 해야할 때도 여기서 > 순서의존성이 있는 

}
