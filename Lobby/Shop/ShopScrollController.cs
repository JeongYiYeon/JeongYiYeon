using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using UnityEngine.UI;

public class ShopScrollController : BaseScrollController<ShopData>
{
    [SerializeField]
    private EnhancedScroller scrollView = null;

    [SerializeField]
    private EnhancedScrollerCellView shopItemGroup = null;

    public int cellsPerRow = 2;

    public void RefreshScrollview()
    {
        scrollView.ReloadData();
    }

    public void SetShopInfo(EnumShop.EShopSubCategory subCategory)
    {
        dataList = LobbyManager.Instance.Shop.GetShopItemInfo(subCategory);
        item = shopItemGroup;

        scrollView.Delegate = this;
        scrollView.ReloadData();
    }

    public override EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        ShopItemGroup item = scroller.GetCellView(shopItemGroup) as ShopItemGroup;

        int cnt = dataIndex * cellsPerRow;

        item.SetData(ref dataList, cnt);
        return item;
    }

    public override float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return shopItemGroup.GetComponent<GridLayoutGroup>().cellSize.y;
    }

    public override int GetNumberOfCells(EnhancedScroller scroller)
    {
        return Mathf.CeilToInt((float)dataList.Count / (float)cellsPerRow);
    }
}
