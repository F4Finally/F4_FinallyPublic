using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyUI : MonoBehaviour
{
    [Tooltip("KeyUI눌렀을 때 비활성화해줄 팝업을 등록합니다.")]
    [Header("This Object Active False")]
    [SerializeField] private GameObject inventoryPopup;
    [SerializeField] private GameObject weaponDetailsPopup;
    [SerializeField] private GameObject colleagueDetailsPopup;
    [SerializeField] private GameObject generalInfoPopup;
    [SerializeField] private GameObject deadlyInfoPopup;
    [SerializeField] private GameObject specialInfoPopup;
    [SerializeField] private GameObject ComGachaResultPopup;
    [SerializeField] private GameObject WeaponResultPopup;

    public void MainUIActiveOffControl()
    {
        inventoryPopup.SetActive(false);
        weaponDetailsPopup.SetActive(false);

        colleagueDetailsPopup.SetActive(false);

        generalInfoPopup.SetActive(false);
        deadlyInfoPopup.SetActive(false);
        specialInfoPopup.SetActive(false);

        ComGachaResultPopup.SetActive(false);
        WeaponResultPopup.SetActive(false);

    }
}
