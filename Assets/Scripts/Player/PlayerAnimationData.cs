using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerAnimationData
{
    [SerializeField] private string IdleParameterName = "Idle"; // Idle 상태가 아니라 평타상태에서 그냥 때리는 애니메를 넣어도 멈춰서 평타 날리지 않을까?
    [SerializeField] private string RunParameterName = "Run";

    [SerializeField] private string BaseAttackParameterName = "BaseAttack"; // 평타
    [SerializeField] private string UltimateAttackParameterName = "UltimateAttack"; // 궁극기 
    [SerializeField] private string HitParameterName = "Hit"; 

    public int IdleParameterHash { get; private set; }
    public int RunParameterHash { get; private set; }
    public int BaseAttackParameterHash { get; private set; }  
    public int UltimateAttackParameterHash { get; private set;}
    public int HitParameterHash { get; private set; }

    public void Initialize()
    {
        IdleParameterHash = Animator.StringToHash(IdleParameterName);
        RunParameterHash = Animator.StringToHash(RunParameterName);
        BaseAttackParameterHash = Animator.StringToHash(BaseAttackParameterName);   
        UltimateAttackParameterHash = Animator.StringToHash(UltimateAttackParameterName);
        HitParameterHash = Animator.StringToHash(HitParameterName);
        
    }

}