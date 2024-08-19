using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoad : MonoBehaviour
{
    private GameObject quitPopup;

    private void Awake()
    {
        SoundManager.Instance.StopBGM(); // 이전 씬 BGM 멈춰주기
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SoundManager.Instance.PlayAudio(Sound.Effect, "NomalClick");
        }

#if UNITY_ANDROID // 얘가 안드로이드 일때만 실행해주세요.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            quitPopup.SetActive(true);
        }
#endif

    }

    public void QuitBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

    public void LoadVillage()
    {

        LoadingSceneManager.LoadScene("VillageScene");
        foreach (var quest in QuestManager.Instance.GetActiveQuests())
        {
            if (quest.Type == QuestType.ETC && quest.subType == SubType.Village)
            {
                QuestManager.Instance.UpdateQuestProgress(quest.ID, 1);
            }
        }
    }
    public void LoadMain()
    {
        LoadingSceneManager.LoadScene("MainScene");
    }
}
