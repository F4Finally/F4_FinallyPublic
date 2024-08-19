using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

[System.Serializable]
public class GameData
{
    // 어떤 데이터를 저장하는 지를 여기에서 한 번에 관리 하기

    // 돈 ,씨앗
    // 장착 무기 , 획득 무기 , 각 무기 업그레이드 단계, 각 무기 레벨업 >> 딕셔너리로 
    // ex > public Dictionary<int, int> weaponData; 각 무기들의 아이디를 키로 잡아주고 value 값을 그 무기의 업그레이드 정보로 , 무기를 소유하지 않았다면 키값 조차 x 
    // public int nowWeaponIndex >> 지금 장착한 무기 
    // 장착 동료 , 획득 동료
    // 스테이지 정보 
    // 마을 정보 
    // 플레이어 공방체

    public PlayerData playerData;
    public int UnlockedSlots;
    public GameData(PlayerData playerData, Dictionary<int,QuestSaveData> questData)
    {
        this.playerData = playerData;
        this.QuestSaveDatas = questData;
        this.UnlockedSlots = 1;
    }

    public Dictionary<int,QuestSaveData> QuestSaveDatas = new Dictionary<int,QuestSaveData>();
}

[System.Serializable]
public class QuestSaveData
{
    public int myID;
    public bool IsAccepted;
    public bool IsCompleted;
    public int nowProgress;

    public QuestSaveData()
    {

    }

    public QuestSaveData(int myID)
    {
        this.myID = myID;   
        IsAccepted = false;
        IsCompleted = false;
        nowProgress = 0;
    }
}

public class SaveManager : MonoBehaviour
{
    // 불러오고 저장하는 것

    string path;

    private void Start()
    {
        GameManager.Instance.saveManager = this;
    }

    // 빅인티저면 그 뒤에 ABC 뭐 이런거까지 한꺼번에 저장하고 수치값에 뒤에 글자 있는 지 없는 지 체크하고 
    // 글자랑 숫자랑 쪼객...내 머리.. 쪼객...

    public void Init()
    {
        path = Path.Combine(Application.persistentDataPath, "gameData.json"); // Application.dataPath > 플레이하는 기기 경로를 알아서 지정해줌 
        Debug.Log(path);
    }

    public void SavaData(GameData data)
    {
        string jsonData = JsonConvert.SerializeObject(data); // data 를 json 데이터 형태의 문자열로 변환해줌 
        File.WriteAllText(path,jsonData);  // 경로랑 
    }

    public bool TryLoadData(out GameData data)
    {
        if(File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            data = JsonConvert.DeserializeObject<GameData>(jsonData);
            return true;
        }
        else
        {
            // 데이터가 없을 때
            data = null;    
            return false;
        }
    }
}
