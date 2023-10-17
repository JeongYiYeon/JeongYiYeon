using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttendanceItemData
{
    private int day;
    private BaseItemData itemData;
    private bool isRecive;

    public int Day => day;
    public BaseItemData ItemData => itemData;
    public bool IsRecive => isRecive;

    public void SetDay(int day)
    {
        this.day = day;
    }

    public void SetItemData(BaseItemData itemData)
    {
        this.itemData = itemData;
    }

    public void SetRecive(bool isRecive) 
    {
        this.isRecive = isRecive;
    }
}
