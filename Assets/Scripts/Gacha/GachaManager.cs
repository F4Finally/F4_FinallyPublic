using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GachaManager : MonoBehaviour
{
    [SerializeField] private List<WeaponDataSO> weaponDataPool;
    [SerializeField] private List<CompanionDataSO> companionDataPool;

    [SerializeField] private GameObject comGachaResultPopup;  // 동료 결과 팝업 프리팹
    [SerializeField] private GameObject weaponResultPopup;    // 장비 결과 팝업 프리팹

    [SerializeField] private GameObject WeaponNoGachaTxt;       // 씨앗 부족해요 글씨
    [SerializeField] private GameObject CompanionNoGachaTxt;    

    public GachaResult PerformGacha(GachaType type, int count)
    {
        List<GachaItem> results = new List<GachaItem>();

        for (int i = 0; i < count; i++)
        {
            GachaItem item = type == GachaType.Equipment ?
                GetRandomWeapon() : GetRandomCompanion();
            results.Add(item);
            Debug.Log($"뽑기 {i + 1}회차: {item.Name} (등급: {item.Grade})");
        }
        // 퀘스트 업데이트
        UpdateGachaQuests(type);
        return new GachaResult(results);
    }

    private void UpdateGachaQuests(GachaType type)
    {
        foreach (var quest in QuestManager.Instance.GetActiveQuests())
        {
            if (quest.Type == QuestType.ETC)
            {
                // 동료 소환 퀘스트 처리
                if (type == GachaType.Companion && quest.subType == SubType.Companion)
                {
                    QuestManager.Instance.UpdateQuestProgress(quest.ID, 1);
                }
                // 무기 소환 퀘스트 처리
                else if (type == GachaType.Equipment && quest.subType == SubType.Weapon)
                {
                    QuestManager.Instance.UpdateQuestProgress(quest.ID, 1);
                }
            }
        }
    }

    private GachaWeapon GetRandomWeapon()
    {
        float randomValue = Random.value;

        // 등급별 확률 설정
        Dictionary<Grade, float> gradeProbabilities = new Dictionary<Grade, float>
    {
        { Grade.Common, 0.50f },
        { Grade.Rare, 0.30f },
        { Grade.Epic, 0.14f },
        { Grade.Unique, 0.04f },
        { Grade.Legend, 0.019f },
        { Grade.Mystic, 0.001f }
    };

        Grade selectedGrade = Grade.Common;
        float cumulativeProbability = 0f;

        foreach (var gradeProbability in gradeProbabilities)
        {
            cumulativeProbability += gradeProbability.Value;
            if (randomValue <= cumulativeProbability)
            {
                selectedGrade = gradeProbability.Key;
                break;
            }
        }

        Debug.Log($"선택된 등급: {selectedGrade}");

        // 선택된 등급에 맞는 무기 리스트
        List<WeaponDataSO> eligibleWeapons = weaponDataPool.FindAll(w => w.grade == selectedGrade);

        if (eligibleWeapons.Count > 0)
        {
            WeaponDataSO selectedData = eligibleWeapons[Random.Range(0, eligibleWeapons.Count)];
            Debug.Log($"선택된 무기: {selectedData.dataName}, 등급: {selectedData.grade}");
            return new GachaWeapon(selectedData);
        }
        else
        {
            Debug.LogWarning($"등급 {selectedGrade}에 해당하는 무기가 없습니다. 전체 풀에서 선택합니다.");
            WeaponDataSO randomData = weaponDataPool[Random.Range(0, weaponDataPool.Count)];
            return new GachaWeapon(randomData);
        }
    }

    private GachaCompanion GetRandomCompanion()
    {
        float randomValue = Random.value;

        Dictionary<Grade, float> gradeProbabilities = new Dictionary<Grade, float>
        {
            { Grade.Common, 0.65f },
            { Grade.Rare, 0.30f },
            { Grade.Epic, 0.05f },
            //{ Grade.Unique, 0.04f },
            //{ Grade.Legend, 0.019f },
            //{ Grade.Mystic, 0.001f }
        };

        Grade selectedGrade = Grade.Common;
        float cumulativeProbability = 0f;

        foreach (var gradeProbability in gradeProbabilities)
        {
            cumulativeProbability += gradeProbability.Value;
            if (randomValue <= cumulativeProbability)
            {
                selectedGrade = gradeProbability.Key;
                break;
            }
        }

        List<CompanionDataSO> eligibleCompanions = companionDataPool.FindAll(c => c.grade == selectedGrade);

        if (eligibleCompanions.Count > 0)
        {
            CompanionDataSO selectedData = eligibleCompanions[Random.Range(0, eligibleCompanions.Count)];
            return new GachaCompanion(selectedData);
        }
        else
        {
            Debug.LogWarning($"No companions found for grade {selectedGrade}. Selecting from entire pool.");
            CompanionDataSO randomData = companionDataPool[Random.Range(0, companionDataPool.Count)];
            return new GachaCompanion(randomData);
        }
    }

    public void PerformEquipmentGachaSingle()
    {
        int seedCost = 10;

        if (GameManager.Instance.CurplayerData.Seed < seedCost)
        {
            Debug.Log("씨앗이 부족합니다.");
            WeaponNoGachaTxt.SetActive(true); // 씨앗 부족 메시지 표시
            StartCoroutine(HideNoGachaText(WeaponNoGachaTxt, 1f)); // 1초 후에 비활성화
            return;
        }

        MoneyManager.Instance.RemoveSeed(seedCost);
        GameManager.Instance.SaveGame();

        GachaResult result = PerformGacha(GachaType.Equipment, 1);

        if (result.Items.Count > 0)
        {
            DisplayEquipmentResults(result);
        }
    }

    public void PerformEquipmentGachaMulti()
    {
        int seedCost = 50;

        if (GameManager.Instance.CurplayerData.Seed < seedCost)
        {
            Debug.Log("씨앗이 부족합니다.");
            WeaponNoGachaTxt.SetActive(true); // 씨앗 부족 메시지 표시
            StartCoroutine(HideNoGachaText(WeaponNoGachaTxt, 1f)); // 1초 후에 비활성화
            return;
        }

        MoneyManager.Instance.RemoveSeed(seedCost);
        GameManager.Instance.SaveGame();

        GachaResult result = PerformGacha(GachaType.Equipment, 5);

        if (result.Items.Count > 0)
        {
            DisplayEquipmentResults(result);
        }
    }

    public void PerformCompanionGachaSingle()
    {
        int seedCost = 10;

        if (GameManager.Instance.CurplayerData.Seed < seedCost)
        {
            Debug.Log("씨앗이 부족합니다.");
            CompanionNoGachaTxt.SetActive(true); // 씨앗 부족 메시지 표시
            StartCoroutine(HideNoGachaText(CompanionNoGachaTxt, 1f)); // 1초 후에 비활성화
            return;
        }

        MoneyManager.Instance.RemoveSeed(seedCost);
        GameManager.Instance.SaveGame();

        GachaResult result = PerformGacha(GachaType.Companion, 1);

        if (result.Items.Count > 0)
        {
            DisplayCompanionResults(result);
        }
    }

    public void PerformCompanionGachaMulti()
    {
        int seedCost = 50;

        if (GameManager.Instance.CurplayerData.Seed < seedCost)
        {
            Debug.Log("씨앗이 부족합니다.");
            CompanionNoGachaTxt.SetActive(true); // 씨앗 부족 메시지 표시
            StartCoroutine(HideNoGachaText(CompanionNoGachaTxt, 1f)); // 1초 후에 비활성화
            return;
        }

        MoneyManager.Instance.RemoveSeed(seedCost);
        GameManager.Instance.SaveGame();

        GachaResult result = PerformGacha(GachaType.Companion, 5);

        if (result.Items.Count > 0)
        {
            DisplayCompanionResults(result);
        }
    }

    private void DisplayEquipmentResults(GachaResult result)
    {
        List<GachaWeapon> gachaWeapons = new List<GachaWeapon>();
        foreach (var item in result.Items)
        {
            if (item is GachaWeapon gachaWeapon)
            {
                gachaWeapons.Add(gachaWeapon);
                AcquireWeapon(gachaWeapon);
            }
        }

        // GachaWeapon 결과를 WeaponGachaResultManager에 전달하여 표시
        WeaponGachaResultManager.Instance.DisplayGachaResults(gachaWeapons);

        // 결과 팝업 활성화
        weaponResultPopup.SetActive(true);
    }

    private void DisplayCompanionResults(GachaResult result)
    {
        List<GachaCompanion> gachaCompanions = new List<GachaCompanion>();
        foreach (var item in result.Items)
        {
            if (item is GachaCompanion gachaCompanion)
            {
                gachaCompanions.Add(gachaCompanion);
                AcquireCompanion(gachaCompanion);
            }
        }

        CompanionGachaResultManager.Instance.DisplayGachaResults(gachaCompanions);

        // 결과 팝업 활성화
        comGachaResultPopup.SetActive(true);
    }

    private void AcquireCompanion(GachaCompanion gachaCompanion)
    {
        string companionId = gachaCompanion.Data.companionId;
        CompanionData companionData = CompanionManager.Instance.GetCompanionById(companionId);
        if (companionData != null)
        {
            CompanionUIManager.Instance.AcquireCompanion(companionId);
            Debug.Log($" 동료 획득: {gachaCompanion.Name}, 등급: {gachaCompanion.Grade}, 중복 횟수 증가");

        }
       
    }

    private void AcquireWeapon(GachaWeapon gachaWeapon)
    {
        WeaponDataSO weaponData = WeaponManager.Instance.GetWeaponDataByID(gachaWeapon.Data.weaponID);

        if (weaponData != null)
        {
            WeaponManager.Instance.AcquireWeapon(weaponData);

            Debug.Log($"획득한 장비: {weaponData.dataName}, 등급: {gachaWeapon.Grade}, 레벨: {gachaWeapon.GetLevel()}");

            // UI 갱신
            WeaponManager.Instance.UpdateAllWeaponSlots();
        }
        else
        {
            Debug.LogError($"Weapon with ID {gachaWeapon.Data.weaponID} not found.");
        }
    }

    private IEnumerator HideNoGachaText(GameObject noGachaText, float delay)
    {
        yield return new WaitForSeconds(delay);
        noGachaText.SetActive(false);
    }
}