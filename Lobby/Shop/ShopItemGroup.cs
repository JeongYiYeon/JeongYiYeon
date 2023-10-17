using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemGroup : EnhancedScrollerCellView
{
    [SerializeField]
    public ShopItem[] rowCellViews;

    public void SetData(ref List<ShopData> data, int startingIndex)
    {
        for (var i = 0; i < rowCellViews.Length; i++)
        {
            rowCellViews[i].SetData(startingIndex + i < data.Count ? data[startingIndex + i] : null);
        }
    }
}
