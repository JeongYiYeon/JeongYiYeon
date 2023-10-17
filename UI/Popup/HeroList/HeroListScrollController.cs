using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroListScrollController : BaseScrollController<HeroListItemData>
{
    [SerializeField]
    private EnhancedScroller scrollView = null;

    [SerializeField]
    private EnhancedScrollerCellView heroListItemGroup = null;

    public int cellsPerRow = 2;

    public void SetHeroListItems(List<HeroListItemData> heroListItemDatas)
    {
        dataList = heroListItemDatas;
        item = heroListItemGroup;

        scrollView.Delegate = this;
        scrollView.ReloadData();
    }

    public override EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        HeroListItemGroup item = scroller.GetCellView(heroListItemGroup) as HeroListItemGroup;

        int cnt = dataIndex * cellsPerRow;

        item.SetData(ref dataList, cnt);
        return item;
    }

    public override float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return heroListItemGroup.GetComponent<GridLayoutGroup>().cellSize.y;
    }

    public override int GetNumberOfCells(EnhancedScroller scroller)
    {
        return Mathf.CeilToInt((float)dataList.Count / (float)cellsPerRow);
    }
}
