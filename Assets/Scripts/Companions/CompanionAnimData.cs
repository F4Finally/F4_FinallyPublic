using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class CompanionAnimData
{
    [SerializeField] private string IdleParameterName = "Idle";
    [SerializeField] private string AttackParameterName = "Attack";
    [SerializeField] private string DieParameterName = "Die";

    public int IdleParameterHash { get; private set; }
    public int AttackParameterHash { get; private set; }
    public int DieParameterHash { get; private set;}   

    public void Initialize()
    {
        IdleParameterHash = Animator.StringToHash(IdleParameterName);
        AttackParameterHash = Animator.StringToHash(AttackParameterName);
        DieParameterHash = Animator.StringToHash(DieParameterName);
    }

}

