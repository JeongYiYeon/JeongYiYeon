using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using UnityEngine.UI;

public class EquipmentItemScrollController : BaseScrollController<EquipItemData>
{
    [SerializeField]
    private EnhancedScroller scrollView = null;

    [SerializeField]
    private EnhancedScrollerCellView equipmentItemGroup = null;

    public int cellsPerRow = 6;

    public void SetEquipmentItems(List<EquipItemData> equipItems)
    {
        dataList = equipItems;
        item = equipmentItemGroup;

        scrollView.Delegate = this;
        scrollView.ReloadData();
    }

    public override EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        EquipmentItemGroup item = scroller.GetCellView(equipmentItemGroup) as EquipmentItemGroup;

        int cnt = dataIndex * cellsPerRow;

        item.SetData(ref dataList, cnt);
        return item;
    }

    public override float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return equipmentItemGroup.GetComponent<GridLayoutGroup>().cellSize.y;
    }

    public override int GetNumberOfCells(EnhancedScroller scroller)
    {
        return Mathf.CeilToInt((float)dataList.Count / (float)cellsPerRow);
    }
}
