using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScrollController : BaseScrollController<BaseItemData>
{
    [SerializeField]
    private EnhancedScroller scrollView = null;

    [SerializeField]
    private EnhancedScrollerCellView itemGroup = null;

    public int cellsPerRow = 5;

    public void SetItemDataInfo(List<BaseItemData> itemDataList)
    {
        dataList = itemDataList;
        item = itemGroup;

        scrollView.Delegate = this;
        scrollView.ReloadData();
    }

    public void ScrollViewRefresh()
    {
        scrollView.ReloadData();
    }

    public override EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        InventoryItemGroup item = scroller.GetCellView(itemGroup) as InventoryItemGroup;

        int cnt = dataIndex * cellsPerRow;

        item.SetData(ref dataList, cnt);

        return item;
    }

    public override float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return itemGroup.GetComponent<GridLayoutGroup>().cellSize.y;
    }

    public override int GetNumberOfCells(EnhancedScroller scroller)
    {
        return Mathf.CeilToInt((float)dataList.Count / (float)cellsPerRow);
    }
}
