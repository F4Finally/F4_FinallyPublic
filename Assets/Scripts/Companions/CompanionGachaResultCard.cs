using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompanionGachaResultCard : MonoBehaviour
{

    public Image companionImage;
    public TextMeshProUGUI companionNameText;
    public Image gradeColorImage;

    public void ShowGachaResult(GachaCompanion gachaCompanion)
    {
        if (gachaCompanion != null)
        {
           
            UpdateUI(gachaCompanion);
        }
    }


    private void UpdateUI(GachaCompanion gachaCompanion)
    {
        companionNameText.text = gachaCompanion.Name;
        companionImage.sprite = gachaCompanion.Data.image;
        if (gradeColors.TryGetValue(gachaCompanion.Grade, out Color gradeColor))
        {
            gradeColorImage.color = gradeColor;
        }
    }

    private static readonly Dictionary<Grade, Color> gradeColors = new Dictionary<Grade, Color>
    {
        { Grade.Common, Color.white },
        { Grade.Rare, Color.green },
        { Grade.Epic, new Color(0.7f, 0, 1.0f) },
    };
}
