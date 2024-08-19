using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionStateMachine : StateMachine
{
    public Companion companion { get; }
    public CompanionIdleState companionIdleState { get; private set; }
    public CompanionAttackState companionAttackState { get; private set; }
    public CompanionDieState companionDieState { get; private set; }
    public CompanionStateMachine(Companion companion)
    {
        this.companion = companion;
        companionIdleState = new CompanionIdleState(this);
        companionAttackState = new CompanionAttackState(this);
        companionDieState = new CompanionDieState(this);
    }



}
