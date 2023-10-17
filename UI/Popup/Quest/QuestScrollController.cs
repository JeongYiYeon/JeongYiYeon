using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using System.Dynamic;

public class QuestScrollController : BaseScrollController<QuestData>
{
    [SerializeField]
    private EnhancedScroller scrollView = null;

    [SerializeField]
    private EnhancedScrollerCellView questItem = null;

    public void SetQuestDataInfo(List<QuestData> questDataList)
    {
        dataList = questDataList;
        item = questItem;

        scrollView.Delegate = this;
        scrollView.ReloadData();
    }

    public void ScrollViewRefresh()
    {
        scrollView.ReloadData();
    }

    public override EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        QuestItem item = scroller.GetCellView(questItem) as QuestItem;

        item.SetData(dataList[dataIndex], dataIndex);

        return item;
    }
}
