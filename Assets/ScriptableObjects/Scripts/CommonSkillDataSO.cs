using UnityEngine;

[CreateAssetMenu(fileName = "CommonSkillData", menuName = "Companion/Common Skill Data")]
public class CommonSkillDataSO : ScriptableObject
{
    public SkillData normalAttackData;
    public SkillData criticalAttackData;
}
