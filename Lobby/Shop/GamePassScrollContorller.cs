using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using System;

public class GamePassScrollContorller : BaseScrollController<GamePassItemData>
{
    [SerializeField]
    private EnhancedScroller scrollView = null;

    [SerializeField]
    private EnhancedScrollerCellView gamePassFreeItem = null;

    public int GetStartCellViewIdx 
    {
        get 
        {
            return scrollView.StartCellViewIndex;
        }
    }

    public int GetEndCellViewIdx
    {
        get
        {
            return scrollView.EndCellViewIndex + 1;
        }
    }
    public void SetGPItem(List<GamePassItemData> gpItemList, Action<bool> callback = null, bool isFree = false)
    {
        dataList = gpItemList;
        item = gamePassFreeItem;

        scrollView.Delegate = this;
        scrollView.ReloadData();

        if(callback!= null) 
        {
            callback.Invoke(isFree);
        }
    }

    public override EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        GamePassItem item = scroller.GetCellView(gamePassFreeItem) as GamePassItem;
    
        item.SetData(dataList[dataIndex], dataIndex);

        return item;
    }
}
