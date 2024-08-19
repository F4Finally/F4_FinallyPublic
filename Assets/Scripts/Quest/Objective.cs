using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective 
{
    private int currentProgress;
    private int targetProgress;

    public Objective(int targetProgress)
    {
        this.targetProgress = targetProgress;
        this.currentProgress = 0;
    }

    public void UpdateProgress(int progressValue)
    {
        currentProgress++;
        currentProgress = progressValue; // 현재 진행 상황 업데이트
    }

    public bool IsAchieved()
    {
        return currentProgress >= targetProgress;
    }
}
