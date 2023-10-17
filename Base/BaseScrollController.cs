using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScrollController<T> : MonoBehaviour, IEnhancedScrollerDelegate
{
    private protected List<T> dataList = new List<T>();

    private protected EnhancedScrollerCellView item = null;

    public virtual EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        return null;
    }

    public virtual float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return item.GetComponent<RectTransform>().rect.height;
    }

    public virtual int GetNumberOfCells(EnhancedScroller scroller)
    {
        return dataList.Count;
    }
}
