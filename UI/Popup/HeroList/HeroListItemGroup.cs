using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;

public class HeroListItemGroup : EnhancedScrollerCellView
{
    [SerializeField]
    public HeroListItem[] rowCellViews;

    public void SetData(ref List<HeroListItemData> data, int startingIndex)
    {
        for (var i = 0; i < rowCellViews.Length; i++)
        {
            rowCellViews[i].SetData(startingIndex + i < data.Count ? data[startingIndex + i] : null);
        }
    }
}
