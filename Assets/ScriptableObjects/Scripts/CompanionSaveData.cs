[System.Serializable]
public class CompanionSaveData
{
    public string companionId;
    public bool isAcquired;
    public int nowPosition;// -1이면 배치x 

    public int level;
    public float currentExp;
    public int dupeCount;
    public int starRating;

    public CompanionSaveData()
    {

    }
    public CompanionSaveData(string id)
    {
        companionId = id;
        isAcquired = false;
        nowPosition = -1;
        level = 1;
        currentExp = 0;
        dupeCount = 0;
        starRating = 0;
    }



}