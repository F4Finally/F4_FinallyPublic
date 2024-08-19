using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Scene
{
    Unknown, // 디폴트
    Intro, // 인트로
    Loading, // 로딩 
    Game, // 인게임 
    Farm // 농장
}
public abstract class BaseScene : MonoBehaviour
{
    public Scene SceneType;

    protected virtual void Awake()
    {
        Init();
    }


    protected abstract void Init();
    public abstract void Clear();
}
