using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttendanceItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text labelDay = null;
    [SerializeField]
    private BaseItem item = null;
    [SerializeField]
    private Image imgRecive = null;

    private AttendanceItemData data = null;

    public void SetData(AttendanceItemData data)
    {
        this.data = data;        

        labelDay.text = $"{data.Day}";

        item.InitItem(data.ItemData);

        imgRecive.gameObject.SetActive(data.IsRecive);
    }
}
