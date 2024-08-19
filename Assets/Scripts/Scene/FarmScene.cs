using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FarmScene : MonoBehaviour
{

    public TextMeshProUGUI VillageName;

    public TMP_InputField villageNameInput; // 마을 이름 입력 필드
    public TMP_InputField playerNameInput; // 닉네임 입력 필드
    public Button saveButton; // 저장 버튼

    public GameObject tutorialPanel; // 튜토리얼 패널


    private void Start()
    {
        // PlayerPrefs에서 데이터 불러오기
        string villageName = PlayerPrefs.GetString("VillageName", "Unknown Village");
        string playerName = PlayerPrefs.GetString("NickName", "Unknown Player");

        // GameManager의 데이터 업데이트
        if (GameManager.Instance != null && GameManager.Instance.CurplayerData != null)
        {
            GameManager.Instance.CurplayerData.VillageName = villageName;
            GameManager.Instance.CurplayerData.NickName = playerName;
        }
      

        // UI 업데이트
        VillageName.text = villageName;
        villageNameInput.text = villageName;
        playerNameInput.text = playerName;

        // BGM 재생
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayAudio(Sound.Bgm, "Village");
        }
        else
        {
            Debug.LogWarning("SoundManager instance is not available.");
        }

        // 저장 버튼 클릭 이벤트 등록
        saveButton.onClick.AddListener(SaveChanges);

        // 입력 필드와 저장 버튼을 활성화 상태로 시작
        villageNameInput.gameObject.SetActive(true);
        playerNameInput.gameObject.SetActive(true);
        saveButton.gameObject.SetActive(true);

        // 튜토리얼 패널 표시 여부 결정
        ShowTutorialIfNeeded();
    }

    private void ShowTutorialIfNeeded()
    {
        // 튜토리얼을 이미 본 적이 있는지 확인
        if (!PlayerPrefs.HasKey("VillageTutorial"))
        {
            // 튜토리얼 패널을 보여줌
            tutorialPanel.SetActive(true);

            // 튜토리얼 본 적이 있다고 기록
            PlayerPrefs.SetInt("VillageTutorial", 1);
            PlayerPrefs.Save();
        }
        else
        {
            // 이미 튜토리얼을 본 적이 있다면 패널을 비활성화
            tutorialPanel.SetActive(false);
        }
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
        VillageName.text = newVillageName;
    }

}
