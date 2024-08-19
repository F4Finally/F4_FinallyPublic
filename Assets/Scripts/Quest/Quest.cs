using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
public enum QuestType
{
    MonsterKill,
    StageProgress,
    StatUpgrade,
    ETC
}

public enum SubType
{
    None = -1,
    Health = 0,
    Attack = 1,
    Defense = 2,
    Companion = 3, // 동료 
    Village = 4, // 마을 
    Weapon = 5, // 장비 
    Dungeon = 6, // 던전
    ECompanion = 7, // 동료 장착
    EWeapon =8 // 무기 장착 
}
public class Quest
{
    public int ID;
    public string Title;
    public string Description;
    public int Objective;
    public int Reward;
    public int nextQuest;
    public SubType subType;
    public QuestType Type { get; set; }

    public Quest(Dictionary<string, string> data)
    {
        ID = int.Parse(data["ID"]);
        Title = data["Title"];
        Description = data["Description"];
        Objective = int.Parse(data["Objective"]);
        Reward = int.Parse(data["Reward"]);
        nextQuest = int.Parse(data["NextQuest"]);
        // 퀘스트 타입 생성
        if (data["Type"] == "MonsterKill")
            Type = QuestType.MonsterKill;
        else if (data["Type"] == "StageProgress")
            Type = QuestType.StageProgress;
        else if (data["Type"] == "StatUpgrade")
            Type = QuestType.StatUpgrade;
        else
            Type = QuestType.ETC;
        // 스탯 구분
        subType = (SubType)int.Parse(data["SubType"]);

    }
}
