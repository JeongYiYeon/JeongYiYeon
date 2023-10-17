using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSort : MonoBehaviour
{
    [SerializeField]
    private GameObject upgradeSortBtGo = null;

    [SerializeField]
    private UtilState gradeState = null;
    [SerializeField]
    private UtilState upGradeState = null;

    private bool isGradeUpSort = true;
    private bool isUpGradeUpSort = true;

    private Action gradeSortCb = null;
    private Action upGradeSortCb = null;

    public void SetGradeSortCb(Action gradeSortCb)
    {
        this.gradeSortCb = gradeSortCb;
    }
    public void SetUpGradeSortCb(Action upGradeSortCb)
    {
        this.upGradeSortCb = upGradeSortCb;
    }

    public void ActiveUpgradeSortBt(bool isActive)
    {
        upgradeSortBtGo.SetActive(isActive);
    }

    public void OnClickGradeSort()
    {
        if (isGradeUpSort == true)
        {
            isGradeUpSort = false;
            gradeState.ActiveState("Down");
        }
        else
        {
            isGradeUpSort = true;
            gradeState.ActiveState("Up");
        }

        if(gradeSortCb != null)
        {
            gradeSortCb.Invoke();
        }
    }

    public void OnClickUpGradeSort()
    {
        if(isUpGradeUpSort == true)
        {
            isUpGradeUpSort = false;
            upGradeState.ActiveState("Down");
        }
        else
        {
            isUpGradeUpSort = true;
            upGradeState.ActiveState("Up");
        }

        if(upGradeSortCb != null)
        {
            upGradeSortCb.Invoke();
        }
    }

    public List<EquipItemData> GetSortGradeEquipmentList(List<EquipItemData> filterEquipmentList)
    {
        if(filterEquipmentList == null || filterEquipmentList.Count == 0)
        {
            return null;
        }

        if (isGradeUpSort == true)
        {
            filterEquipmentList.Sort((x, y) => y.ItemGrade.CompareTo(x.ItemGrade));
        }
        else
        {
            filterEquipmentList.Sort((x, y) => x.ItemGrade.CompareTo(y.ItemGrade));
        }

        return filterEquipmentList;
    }

    public List<EquipItemData> GetSortUpGradeEquipmentList(List<EquipItemData> filterEquipmentList)
    {
        if (filterEquipmentList == null || filterEquipmentList.Count == 0)
        {
            return null;
        }

        if (isGradeUpSort == true)
        {
            filterEquipmentList.Sort((x, y) => y.UpgradeCnt.CompareTo(x.UpgradeCnt));
        }
        else
        {
            filterEquipmentList.Sort((x, y) => x.UpgradeCnt.CompareTo(y.UpgradeCnt));
        }

        return filterEquipmentList;
    }

    public List<BaseItemData> GetSortItemList(List<BaseItemData> itemList)
    {
        if (itemList == null || itemList.Count == 0)
        {
            return null;
        }

        if (isGradeUpSort == true)
        {
            itemList.Sort((x, y) => y.ItemGrade.CompareTo(x.ItemGrade));
        }
        else
        {
            itemList.Sort((x, y) => x.ItemGrade.CompareTo(y.ItemGrade));
        }

        return itemList;
    }
}
