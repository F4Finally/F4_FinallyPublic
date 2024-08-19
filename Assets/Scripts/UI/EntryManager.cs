using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EntryManager : MonoBehaviour
{
    [SerializeField] private Toggle dungeonToggle;  // 던전 입장 토글
    [SerializeField] private Toggle villageToggle;  // 마을 입장 토글

    [SerializeField] private GameObject dungeonNoOpenPanel;  // 던전 입장 불가 시 표시할 패널
    [SerializeField] private GameObject villageNoOpenPanel;  // 마을 입장 불가 시 표시할 패널

    private void Start()
    {
        // 각 토글에 클릭 이벤트 추가
        AddClickListenerToToggle(dungeonToggle, dungeonNoOpenPanel);
        AddClickListenerToToggle(villageToggle, villageNoOpenPanel);
    }

    private void AddClickListenerToToggle(Toggle toggle, GameObject noOpenPanel)
    {
        // EventTrigger를 사용하여 비활성화된 토글에도 이벤트 리스너를 추가
        EventTrigger trigger = toggle.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((eventData) =>
        {
            if (!toggle.interactable)
            {
                noOpenPanel.SetActive(true);  // 패널 표시
            }
        });
        trigger.triggers.Add(entry);
    }
}
