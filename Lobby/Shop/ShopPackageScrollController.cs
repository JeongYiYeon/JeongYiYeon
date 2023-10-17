using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPackageScrollController : BaseScrollController<ShopData>
{
    [SerializeField]
    private EnhancedScroller scrollView = null;

    [SerializeField]
    private EnhancedScrollerCellView packageItem = null;

    public void RefreshScrollview()
    {
        scrollView.ReloadData();
    }

    public void SetShopInfo(EnumShop.EShopSubCategory subCategory)
    {
        dataList = LobbyManager.Instance.Shop.GetShopItemInfo(subCategory);
        item = packageItem;

        scrollView.Delegate = this;
        scrollView.ReloadData();
    }

    public override EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        Package item = scroller.GetCellView(packageItem) as Package;

        item.SetPackage(dataList[dataIndex], dataIndex);

        return item;
    }
}
