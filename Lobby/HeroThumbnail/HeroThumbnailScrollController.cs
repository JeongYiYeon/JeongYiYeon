using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using UnityEngine.UI;
using System;

public class HeroThumbnailScrollController : BaseScrollController<HeroListItemData>
{
    [SerializeField]
    private EnhancedScroller scrollView = null;

    [SerializeField]
    private EnhancedScrollerCellView heroPositionThumbnail = null;

    public void ScrollViewRefresh()
    {
        scrollView.ReloadData();
    }

    public void SetThumbnailData(List<HeroListItemData> dataList)
    {
        this.dataList = dataList;
        item = heroPositionThumbnail;

        scrollView.Delegate = this;
        scrollView.ReloadData();
    }

    public override EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        HeroThumbnail item = scroller.GetCellView(heroPositionThumbnail) as HeroThumbnail;

        item.SetData(dataList[dataIndex], dataIndex);

        return item;
    }
}
