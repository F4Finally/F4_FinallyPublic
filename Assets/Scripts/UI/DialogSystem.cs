using System.Collections;
using TMPro;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    [System.Serializable]
    public class DialogData
    {
        // 대사
        public string dialog;
    }

    [SerializeField]
    // 대화창 panel
    private GameObject panelUI;
    [SerializeField]
    // 대사 출력 UI
    private TextMeshProUGUI dialogText;
    [SerializeField]
    private bool isAutoStart = true;
    [SerializeField]
    private DialogData[] dialogs;
    [SerializeField]
    private GameObject npcChar;
    //[SerializeField]
    //private string finalMessage1 = "주인공은 바로 {0}이야! 행운을 빌게!";
    // 최초 1회만 실행 
    private bool isFirst = true;
    private int curDialogIndex = -1;
    private float typingSpeed = 0.08f;
    private bool isTyping = false;

    // 유저이름 
    private string userName;

    private Coroutine coroutine;

    private void Start()
    {
        UpdateDialog();
    }
    //클릭을 마지막 대사에서 막기 
    // 코루틴을 사용할 때는 변수에 담아서 하는 게 좋다!

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("클릭되니?");
            if (isTyping)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }

                isTyping = false;
                dialogText.text = string.Format(dialogs[curDialogIndex].dialog, userName);
            }
            // 다음 대사가 남아 있으면 
            else if (dialogs.Length > curDialogIndex + 1)
            {
                SetNextDialog();
            }

            else
            {
                panelUI.SetActive(false);
                npcChar.SetActive(false);
            }
        }
    }


    public bool UpdateDialog()
    {
        // 대사 진행
        // 최초 진행일 때만 대화 나오게 하기 
        if (isFirst)
        {
            if (isAutoStart)
            {

                SetNextDialog();
            }
            isFirst = false;
        }
        return false;
    }

    private void SetNextDialog()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        curDialogIndex++;
        Debug.Log(curDialogIndex);
        coroutine = StartCoroutine(OnTyping());
    }

    private IEnumerator OnTyping()
    {
        int index = 0;
        isTyping = true;
        string final = string.Format(dialogs[curDialogIndex].dialog, userName);
        Debug.Log(final);

        while (index < final.Length)
        {
            dialogText.text = final.Substring(0, index);

            index++;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}
