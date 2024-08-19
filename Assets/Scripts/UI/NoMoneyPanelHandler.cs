using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NoMoneyPanelHandler : MonoBehaviour
{
    [SerializeField] private GameObject noMoneyPanel;

    [SerializeField] private Button atkButton;  // ATK 버튼
    [SerializeField] private Button defButton;  // DEF 버튼
    [SerializeField] private Button hpButton;   // HP 버튼

    private void Start()
    {
        // 버튼들에 클릭 이벤트 트리거 추가
        AddClickListenerToButton(atkButton);
        AddClickListenerToButton(defButton);
        AddClickListenerToButton(hpButton);
    }

    // 버튼이 비활성화된 상태에서 클릭 이벤트를 처리하는 메서드
    private void AddClickListenerToButton(Button button)
    {
        if (button != null)
        {
            // EventTrigger 컴포넌트 추가
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            // 버튼이 비활성화된 상태에서 클릭되면 noMoneyPanel 표시
            entry.callback.AddListener((eventData) =>
            {
                if (!button.interactable)
                {
                    noMoneyPanel.SetActive(true);
                }
            });
            trigger.triggers.Add(entry);
        }
    }

}
