using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;
using UnityEngine.U2D;

public class IntroScene : MonoBehaviour
{
    public TMP_InputField playerNameInput;
    public TMP_InputField villageNameInput;
    public TextMeshProUGUI warningText; // 검증 텍스트 
    public GameObject infoPanel;
    private SceneLoad sceneLoad;

    [Header("MovieObj")]
    [SerializeField] GameObject movieObj;
    [SerializeField] VideoPlayer introMovie;

    [Header("LogoObj")]
    [SerializeField] GameObject logoObj;
    [SerializeField] float logoEndTime = 2.0f;

    private void Start()
    {
        //PlayerPrefs.DeleteAll();  // 데이터 지우고 확인 하시려면 이거 주석 풀면 됩니다
        sceneLoad = GetComponent<SceneLoad>();
        //SoundManager.Instance.PlayAudio(Sound.Bgm, "Intro");

        StartCoroutine("MovieStartDelay");

        introMovie.loopPointReached += CheckOverMovie;
    }

    IEnumerator MovieStartDelay()
    {
        yield return new WaitForSeconds(logoEndTime);

        logoObj.SetActive(false);

        if (PlayerPrefs.HasKey("NickName") && PlayerPrefs.HasKey("VillageName"))
        {
            SkipMovie();
        }
        else
        { 
            MovieStart(); 
        }
    }

    public void SkipMovie()
    {
        movieObj.SetActive(false);
        SoundManager.Instance.PlayAudio(Sound.Bgm, "Intro");
    }

    public void MovieStart() // 영상 실행 시키는 메서드
    {
        movieObj.SetActive(true);

        introMovie.Play();
        SoundManager.Instance.StopBGM();
    }

    void CheckOverMovie(VideoPlayer vp) // 영상이 끝났는지 확인하는 용도의 메서드
    {
        movieObj.SetActive(false);
        SoundManager.Instance.PlayAudio(Sound.Bgm, "Intro");
    }

    public void OnStartGame()
    {
        // 입력 필드가 비어 있는지 확인
        if (string.IsNullOrWhiteSpace(playerNameInput.text) || string.IsNullOrWhiteSpace(villageNameInput.text))
        {
            warningText.text = "전부 입력해 주세요!";
            return;
        }

        // 플레이어 이름과 마을 이름을 PlayerData에 저장하려고 했지만 게임매니저 넣으면 너무나 많은 오류 생김.. 
        PlayerPrefs.SetString("NickName", playerNameInput.text);
        PlayerPrefs.SetString("VillageName", villageNameInput.text);
        PlayerPrefs.Save();
        sceneLoad.LoadMain();
    }

    public void SetActiveTrue()
    {
        if (PlayerPrefs.HasKey("NickName") && PlayerPrefs.HasKey("VillageName"))
        {
            // 이미 이름과 마을 이름이 저장되어 있다면 바로 게임 시작
            sceneLoad.LoadMain();
        }
        else
        {
            infoPanel.SetActive(true);
        }

    }
}
