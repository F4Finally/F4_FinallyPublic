using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    static string nextScene;
    [SerializeField] Image progressBar;
    [SerializeField] Slider progressBarHandle;

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");

        TouchEffectOfParticle.Instance.gameObject.SetActive(false); // 씬 전환 될 때 엑티브 펄스
    }

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextScene); // LoadSceneAsync 함수가 AsyncOperation 타입으로 반환
        asyncOperation.allowSceneActivation = false; // 씬을 비동기로 불러들일 때 씬의 로딩이 끝나면 자동으로 불러온 씬으로 이동할 것인지 설정, 페이크 로딩
        float timer = 0.0f; // 시간 측정에 쓰임
        while (!asyncOperation.isDone) // 씬 로딩이 끝나지 않은 상태라면 계속 반복
        {
            yield return null;
            timer += Time.deltaTime;
            if (asyncOperation.progress < 0.7f) // 로딩바를 70퍼까지만 채우고 멈춤
            {
                progressBarHandle.value = Mathf.Lerp(progressBarHandle.value, asyncOperation.progress, timer);
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, asyncOperation.progress, timer);
                if (progressBar.fillAmount >= asyncOperation.progress)
                {
                    timer = 0f;
                }
            }
            else // 나머지 10%를 1초간 채우게 만드는 조건
            {
                timer += Time.unscaledDeltaTime;

                progressBarHandle.value = Mathf.Lerp(progressBarHandle.value, 3f, timer); // 벨류가 0.9에서 1이 되기까지 1초
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 3f, timer); // 필어마운트가 0.9에서 1이 되기까지 1초

                if (progressBar.fillAmount >= 1.0f)
                {
                    asyncOperation.allowSceneActivation = true;

                    TouchEffectOfParticle.Instance.gameObject.SetActive(true); // 전환 되고 나면 다시 활성화

                    yield break;
                }
            }
        }
    }
}
