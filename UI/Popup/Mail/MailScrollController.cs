using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using System;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class MailScrollController : BaseScrollController<MailData>
{
    [SerializeField]
    private EnhancedScroller scrollView;

    [SerializeField]
    private EnhancedScrollerCellView mailItem;

    private bool lastPadderActive;
    private float lastPadderSize;

    public void SetMailInfo(List<MailData> mailInfo)
    {
        dataList = mailInfo;
        item = mailItem;

        scrollView.Delegate = this;
        scrollView.ReloadData();
    }

    public void ScrollViewRefresh()
    {
        scrollView.ReloadData();
    }

    private void InitializeTween(int dataIndex, int cellViewIndex)
    {
        dataList[dataIndex].isExpanded = !dataList[dataIndex].isExpanded;

        for (var i = 0; i < dataList.Count; i++)
        {
            if (i != dataIndex)
            {
                if (((dataIndex + 2) % 3 == 0) || ((i + 2) % 3 == 0))
                {
                    dataList[i].isExpanded = false;
                }
            }
        }

        var cellPosition = scrollView.GetScrollPositionForCellViewIndex(cellViewIndex, EnhancedScroller.CellViewPositionEnum.Before);

        var tweenCellOffset = cellPosition - scrollView.ScrollPosition;

        scrollView.IgnoreLoopJump(true);

        scrollView.ReloadData();

        cellPosition = scrollView.GetScrollPositionForCellViewIndex(cellViewIndex, EnhancedScroller.CellViewPositionEnum.Before);

        scrollView.SetScrollPositionImmediately(cellPosition - tweenCellOffset);

        scrollView.IgnoreLoopJump(false);

        if (dataList[dataIndex].tweenType == Tween.TweenType.immediate)
        {
            return;
        }

        lastPadderActive = scrollView.LastPadder.IsActive();
        lastPadderSize = scrollView.LastPadder.minHeight;

        if (dataList[dataIndex].isExpanded)
        {
            scrollView.LastPadder.minHeight += dataList[dataIndex].SizeDifference;
        }
        else
        {
            scrollView.LastPadder.minHeight -= dataList[dataIndex].SizeDifference;
        }

        scrollView.LastPadder.gameObject.SetActive(true);

      
        var cellViewTween = scrollView.GetCellViewAtDataIndex(dataIndex) as MailItem;

        cellViewTween.BeginTween();
    }
    private void TweenUpdated(int dataIndex, int cellViewIndex, float newValue, float delta)
    {
        scrollView.LastPadder.minHeight -= delta;

    }
    private void TweenEnd(int dataIndex, int cellViewIndex)
    {
        scrollView.LastPadder.gameObject.SetActive(lastPadderActive);

        scrollView.LastPadder.minHeight = lastPadderSize;
    }
    public override EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        MailItem item = scroller.GetCellView(mailItem) as MailItem;

        item.SetInitTween(InitializeTween);
        item.SetUpdateTween(TweenUpdated);
        item.SetEndTween(TweenEnd);
        item.SetData(dataList[dataIndex], dataIndex);

        return item;
    }

    public override float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return dataList[dataIndex].Size;
    }
}
