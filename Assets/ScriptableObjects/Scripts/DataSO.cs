using System.Collections.Generic;
using UnityEngine;

public enum Grade { Common, Rare, Epic, Unique, Legend, Mystic }

public class DataSO : ScriptableObject
{
    [Header("Data Info")]
    public Sprite image;
    public string dataName;   //이름
    public Grade grade;    //등급

    
    public Stats baseStats;  //기본 스텟

  
    public PassiveEffect passiveEffect;  //보유효과
}
