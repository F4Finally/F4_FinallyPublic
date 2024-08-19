using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompanionStatusUI : MonoBehaviour
{
    [System.Serializable]
    public class CompanionSlotUI
    {
        public Image companionImage;
        public TextMeshProUGUI cooldownText;
        public Image cooldownFillImage;
    }

    public CompanionSlotUI[] companionSlots;

    private void Update()
    {
        UpdateCompanionSlots();
    }

    private void UpdateCompanionSlots()
    {
        var placedCompanions = CompanionManager.Instance.GetPlacedCompanions();

        for (int i = 0; i < companionSlots.Length; i++)
        {
            if (i < placedCompanions.Count)
            {
                Companion companion = placedCompanions[i];
                UpdateSlot(companionSlots[i], companion);
            }
            else
            {
                ClearSlot(companionSlots[i]);
            }
        }
    }

    private void UpdateSlot(CompanionSlotUI slot, Companion companion)
    {
        slot.companionImage.sprite = companion.data.dataSO.image;
        slot.companionImage.color = Color.white;

        float cooldownTime = GetUniqueSkillCooldownTime(companion);
        float maxCooldownTime = GetUniqueSkillMaxCooldownTime(companion);
        slot.cooldownFillImage.fillAmount = 1 - (cooldownTime / maxCooldownTime);
        slot.cooldownText.text = cooldownTime > 0 ? $"{cooldownTime:F1}" : "Ready";
    }

    private void ClearSlot(CompanionSlotUI slot)
    {
        slot.cooldownFillImage.fillAmount = 0;
        slot.cooldownText.text = "";
    }

    private float GetUniqueSkillCooldownTime(Companion companion)
    {
        var uniqueSkill = companion.skillApplies.FirstOrDefault(s => s is not NormalAttack && s is not CriticalAttack);
        if (uniqueSkill != null)
        {
            return Mathf.Max(0, uniqueSkill.myskilldata.coolDown - uniqueSkill.waitTime);
        }
        return 0f;
    }

    private float GetUniqueSkillMaxCooldownTime(Companion companion)
    {
        var uniqueSkill = companion.skillApplies.FirstOrDefault(s => s is not NormalAttack && s is not CriticalAttack);
        if (uniqueSkill != null)
        {
            return uniqueSkill.myskilldata.coolDown;
        }
        return 1f; // 기본값으로 1을 반환하여 0으로 나누는 오류 방지
    }
}