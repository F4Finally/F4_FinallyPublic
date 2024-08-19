using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections;

public class WeaponManager : Singleton<WeaponManager>
{
    public GameObject weaponSlotPrefab; // Inspector에서 비활성화된 프리팹을 설정
    public Transform weaponSlotParent;  // UI에 무기 슬롯을 배치할 부모 오브젝트

    public List<WeaponDataSO> allWeaponData; // Inspector에서 설정할 수 있도록 public으로 선언
    List<WeaponSaveData> allWeaponSaveData;

    [Header("Grade Icons")]
    public Sprite commonIcon;
    public Sprite rareIcon;
    public Sprite epicIcon;
    public Sprite uniqueIcon;
    public Sprite legendIcon;
    public Sprite mysticIcon;

    [Header("UI Btn")]
    public Button autoEquipBtn; // 자동 장착 버튼
    public Button autoSyntheticBtn; // 자동 승급 버튼
    public Button weaponIconPromotionBtn; // 개별 승급 버튼

    [Header("Icon")]
    public Image equipWeaponIcon; // 장착 중인 무기 아이콘을 표시할 UI 이미지
    public Sprite defaultEquipIcon; // 장착 중이 아닐 때 표시할 기본 이미지

    [Header("No Panel")]
    public GameObject NoWeaponPanel;
    public GameObject NoSyntheticPanel;

    private Dictionary<int, WeaponSlot> weaponSlots = new Dictionary<int, WeaponSlot>(); // 무기 슬롯을 관리할 딕셔너리
    private WeaponDataSO equippedWeapon; // 현재 장착된 무기 데이터

    string path = "";

    private void Start()
    {
        path = Path.Combine(Application.persistentDataPath, "weaponData.json");
        CreateWeaponUI();
        autoEquipBtn.onClick.AddListener(AutoEquipWeapon); // AutoEquipWeapon 메서드를 추가
        autoSyntheticBtn.onClick.AddListener(AutoPromoteAllWeapons); // 자동 승급 버튼 이벤트 등록

        LoadWeaponData();
        UpdateAllWeaponSlots();
    }

    List<WeaponSaveData> WeaponDataOsToWeaponSaveData(List<WeaponDataSO> data)
    {
        List<WeaponSaveData> saveDatas = new List<WeaponSaveData>();

        for (int i = 0; i < data.Count; i++)
        {
            WeaponSaveData weaponSavedata = new WeaponSaveData(data[i], false);

            if (equippedWeapon != null)
            {
                if (equippedWeapon.weaponID == weaponSavedata.weaponID)
                {
                    weaponSavedata.isEquipped = true;
                }
            }

            saveDatas.Add(weaponSavedata);
        }

        return saveDatas;
    }

    void SaveWeaponData(List<WeaponSaveData> data)
    {
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            }); // data 를 json 데이터 형태의 문자열로 변환해줌 
        File.WriteAllText(path, jsonData);  // 경로랑 

        allWeaponSaveData.Clear();
    }

    void LoadWeaponData()
    {
        if (File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            Debug.Log(jsonData);
            allWeaponSaveData = JsonConvert.DeserializeObject<List<WeaponSaveData>>(jsonData);

            //for(int i = 0; i < allWeaponSaveData.Count; i++)
            //{
            //    allWeaponSaveData[i].DataLog();
            //}

            for (int i = 0; i < allWeaponSaveData.Count; i++)
            {
                int foundNuber = allWeaponData.FindIndex(n => n.weaponID == allWeaponSaveData[i].weaponID);

                allWeaponData[foundNuber].WeaponLoad(allWeaponSaveData[i]);

                if (allWeaponData[foundNuber].isEquipped)
                {
                    EquipWeapon(allWeaponData[foundNuber]);
                }
            }

            allWeaponSaveData.Clear();
        }

        else
        {
            foreach (WeaponDataSO data in allWeaponData)
            {
                data.WeaponLoad(new WeaponSaveData(data.weaponID));
            }
        }
    }

    void OnDisable()
    {
        SaveWeapon();
    }

    public void SaveWeapon()
    {
        allWeaponSaveData = WeaponDataOsToWeaponSaveData(allWeaponData);
        SaveWeaponData(allWeaponSaveData);
    }

    public void CreateWeaponUI()
    {
        foreach (var weaponData in allWeaponData)
        {
            GameObject weaponSlot = Instantiate(weaponSlotPrefab, weaponSlotParent);
            weaponSlot.SetActive(true); // 생성된 슬롯을 활성화
            WeaponSlot weaponSlotScript = weaponSlot.GetComponent<WeaponSlot>();

            if (weaponSlotScript != null)
            {
                weaponSlotScript.InitializeSlot(weaponData, GetGradeIcon(weaponData.grade));
                weaponSlots[weaponData.weaponID] = weaponSlotScript; // 슬롯을 딕셔너리에 저장
            }
        }
    }

    public Sprite GetGradeIcon(Grade grade)
    {
        switch (grade)
        {
            case Grade.Common:
                return commonIcon;
            case Grade.Rare:
                return rareIcon;
            case Grade.Epic:
                return epicIcon;
            case Grade.Unique:
                return uniqueIcon;
            case Grade.Legend:
                return legendIcon;
            case Grade.Mystic:
                return mysticIcon;
            default:
                return null;
        }
    }

    public WeaponDataSO GetWeaponDataByID(int id)
    {
        return allWeaponData.Find(weapon => weapon.weaponID == id);
    }

    public void UpdateWeaponSlot(int weaponID)
    {
        if (weaponSlots.TryGetValue(weaponID, out WeaponSlot weaponSlot))
        {
            WeaponDataSO weaponData = GetWeaponDataByID(weaponID);
            if (weaponData != null)
            {
                weaponSlot.UpdateSlot(GetGradeIcon(weaponData.grade));
            }
        }
    }

    public void AcquireWeapon(WeaponDataSO weaponData)
    {
        if (weaponData == null) return;

        // 중복 횟수 증가 및 UI 업데이트
        weaponData.isAcquired = true;
        weaponData.overlapCount++;
        UpdateAllWeaponSlots();
    }

    public void UpdateAllWeaponSlots()
    {
        foreach (var weaponSlot in weaponSlots.Values)
        {
            var weaponData = GetWeaponDataByID(weaponSlot.GetWeaponID());
            if (weaponData != null)
            {
                weaponSlot.UpdateSlot(GetGradeIcon(weaponData.grade));
                // 장비가 장착된 상태와 비장착 상태에 따라 아이콘 상태 업데이트
                bool isEquipped = equippedWeapon != null && equippedWeapon.weaponID == weaponData.weaponID;
                weaponSlot.SetEquipIconActive(isEquipped);
                weaponSlot.SetIconOffActive(!weaponData.isAcquired); // 장비를 소유하면 비활성화 아이콘을 비활성화

                // 중복 횟수와 관련된 UI 업데이트
                weaponSlot.UpdateOverlapUI();
            }

            else
            {
                UpdateAllWeaponSlots();
            }
        }
    }

    public void AutoEquipWeapon()
    {
        // 공격력이 가장 높은 소유한 무기를 찾기
        WeaponDataSO bestWeapon = allWeaponData
            .Where(w => w.isAcquired) // 소유한 무기만 고려
            .OrderByDescending(w => w.baseStats.attack)
            .FirstOrDefault();

        if (bestWeapon != null)
        {
            // 현재 장착된 무기와 동일한 무기를 찾았을 때는 장착 상태를 유지하고 아무 작업도 하지 않음
            if (equippedWeapon == bestWeapon)
            {
                return; // 장착 해제를 방지하기 위해 메서드를 종료
            }

            // 새로운 무기를 장착
            EquipWeapon(bestWeapon);
        }
        else
        {
            // 소유한 무기가 없을 때 NoWeaponPanel을 활성화
            if (NoWeaponPanel != null)
            {
                NoWeaponPanel.SetActive(true);
                StartCoroutine(HideNoWeaponPanel(NoWeaponPanel, 1f)); // 1초 후에 비활성화
            }
        }
    }

    private IEnumerator HideNoWeaponPanel(GameObject NoWeaponPanel, float delay)
    {
        yield return new WaitForSeconds(delay);
        NoWeaponPanel.SetActive(false);
    }

    public void EquipWeapon(WeaponDataSO weaponData)
    {
        Player player = FindObjectOfType<Player>(); 

        // 장착 해제 여부를 확인
        if (equippedWeapon == weaponData)
        {
            // 이미 장착된 무기를 다시 클릭하면 장착 해제
            equippedWeapon = null;
            weaponData.isEquipped = false;
            WeaponSlot previousSlot;
            if (weaponSlots.TryGetValue(weaponData.weaponID, out previousSlot))
            {
                previousSlot.SetEquipIconActive(false);
            }
            UpdateEquipWeaponIcon(); // 장착 아이콘 업데이트
            WeaponPopupManager.Instance.UpdateEquipButtonText(false); // 장착 버튼 텍스트 업데이트
            Debug.Log($"{weaponData.dataName}가 장착 해제되었습니다");

            // 플레이어의 스텟에서 무기 스텟 제거
            player.RemoveWeaponStats(weaponData);
            return;
        }

        // 이전 장착된 무기의 장착 아이콘 비활성화
        if (equippedWeapon != null)
        {
            equippedWeapon.isEquipped = false;
            WeaponSlot previousSlot;
            if (weaponSlots.TryGetValue(equippedWeapon.weaponID, out previousSlot))
            {
                previousSlot.SetEquipIconActive(false);
            }
        }

        // 새로 장착할 무기 설정
        equippedWeapon = weaponData;
        weaponData.isEquipped = true;
        WeaponSlot newSlot;
        if (weaponSlots.TryGetValue(weaponData.weaponID, out newSlot))
        {
            newSlot.SetEquipIconActive(true);
        }

        // 장착된 무기 정보를 로그로 출력
        Debug.Log($"{weaponData.dataName}가 장착되었습니다");
        UpdateEquipWeaponIcon(); // 장착 아이콘 업데이트
        WeaponPopupManager.Instance.UpdateEquipButtonText(true); // 장착 버튼 텍스트 업데이트

        // 플레이어의 스텟에 무기 스텟 추가
        player.ApplyWeaponStats(weaponData);
        SaveWeapon();
    }

    private void UpdateEquipWeaponIcon()
    {
        if (equippedWeapon != null)
        {
            equipWeaponIcon.sprite = equippedWeapon.image;
            equipWeaponIcon.gameObject.SetActive(true);
        }
        else
        {
            equipWeaponIcon.sprite = defaultEquipIcon;
            equipWeaponIcon.gameObject.SetActive(true);
        }
    }

    public WeaponDataSO GetEquippedWeapon()
    {
        return equippedWeapon;
    }

    // 공개 메서드 추가: WeaponSlot에 대한 접근
    public WeaponSlot GetWeaponSlot(int weaponID)
    {
        return weaponSlots.TryGetValue(weaponID, out WeaponSlot weaponSlot) ? weaponSlot : null;
    }

    // 공개 메서드 추가: 모든 WeaponSlot에 대한 접근
    public Dictionary<int, WeaponSlot> GetAllWeaponSlots()
    {
        return weaponSlots;
    }

    public void PromoteWeapon(WeaponDataSO weaponData)
    {
        if (weaponData == null)
        {
            Debug.Log("승급할 수 있는 상태가 아닙니다.");
            return;
        }

        while (weaponData.overlapCount >= 10)
        {
            // 10개의 중복을 사용하여 승급
            weaponData.overlapCount -= 10;

            // 다음 등급으로 승급
            WeaponDataSO nextGradeWeapon = GetNextGradeWeapon(weaponData);
            if (nextGradeWeapon != null)
            {
                nextGradeWeapon.overlapCount += 1;
                nextGradeWeapon.isAcquired = true; // 승급된 무기는 획득된 상태로 설정

                Debug.Log($"{weaponData.weaponName}이(가) {nextGradeWeapon.weaponName}(으)로 승급되었습니다.");
            }
            else
            {
                Debug.Log("다음 등급의 무기를 찾을 수 없습니다.");
                break;
            }
        }

        // UI 업데이트
        UpdateAllWeaponSlots();
    }

    // 다음 등급 무기 데이터 가져오기
    private WeaponDataSO GetNextGradeWeapon(WeaponDataSO currentWeaponData)
    {
        int currentGradeIndex = (int)currentWeaponData.grade;
        int nextGradeIndex = currentGradeIndex;
        int nextSubGrade = currentWeaponData.subGrade + 1;

        if (nextSubGrade > 4)
        {
            nextSubGrade = 1;
            nextGradeIndex++;
        }

        // 가장 높은 등급이면 승급 불가 메시지 출력
        if (nextGradeIndex >= System.Enum.GetValues(typeof(Grade)).Length)
        {
            Debug.Log("가장 높은 등급의 무기입니다.");
            return null;
        }

        Grade nextGrade = (Grade)nextGradeIndex;
        return allWeaponData.FirstOrDefault(weapon => weapon.grade == nextGrade && weapon.subGrade == nextSubGrade);
    }

    // 자동 승급 메서드
    public void AutoPromoteAllWeapons()
    {
        bool anyWeaponPromoted = false; // 승급 가능한 무기가 있는지 추적

        foreach (var weaponSlot in weaponSlots.Values)
        {
            WeaponDataSO weaponData = GetWeaponDataByID(weaponSlot.GetWeaponID());
            if (weaponData != null && weaponData.overlapCount >= 10)
            {
                PromoteWeapon(weaponData);
                anyWeaponPromoted = true; // 승급이 가능한 무기가 있음을 표시
            }
        }

        // 만약 승급 가능한 무기가 없었다면 NoSyntheticPanel을 표시
        if (!anyWeaponPromoted && NoSyntheticPanel != null)
        {
            NoSyntheticPanel.SetActive(true);
            StartCoroutine(HideNoSyntheticPanel(NoSyntheticPanel, 1f)); // 1초 후에 패널을 비활성화
        }
    }

    private IEnumerator HideNoSyntheticPanel(GameObject NoSyntheticPanel, float delay)
    {
        yield return new WaitForSeconds(delay);
        NoSyntheticPanel.SetActive(false);
    }
}

