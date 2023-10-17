using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using UnityEngine.EventSystems;
using EnhancedUI.EnhancedScroller;

public class Package : EnhancedScrollerCellView
{
    [SerializeField]
    private TMP_Text labelTitle = null;
    [SerializeField]
    private TMP_Text labelPrice = null;
    [SerializeField]
    private BaseItem[] packageItems = null;

    [SerializeField]
    private GameObject prePackageBtGo = null;
    [SerializeField]
    private GameObject nextPackageBtGo = null;

    //private List<ShopData> packageDataList = null;

    private PackageData packageData = null;

    //private int packageIdx = 0;

    //private void Awake()
    //{
        //InitPackage();

        //if (packageDataList != null && packageDataList.Count > 0)
        //{
        //    SetPackage(packageDataList[0]);
        //}
    //}

    //private void InitPackage()
    //{
        //packageDataList = LobbyManager.Instance.Shop.GetShopItemInfo(EnumShop.EShopSubCategory.Package);
    //}

    public void SetPackage(ShopData packageData, int dataIndex)
    {
        this.packageData = (PackageData)packageData;
        this.dataIndex = dataIndex;

        labelTitle.text = this.packageData.Title;
        labelPrice.text = this.packageData.CashCount.ToString("#,##0");

        ResetPackageItems();
        SetPackageItems(this.packageData.ItemList);

        //prePackageBtGo.SetActive(packageIdx > 0);
        //nextPackageBtGo.SetActive(packageIdx < packageDataList.Count - 1);
    }

    private void SetPackageItems(List<BaseItemData> itemList)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            packageItems[i].gameObject.SetActive(true);
            packageItems[i].InitItem(itemList[i]);
        }
    }

    private void ResetPackageItems()
    {
        for (int i = 0; i < packageItems.Length; i++)
        {
            packageItems[i].gameObject.SetActive(false);
        }
    }

    #region OnClick
    public void OnClickPurchase()
    {
        if (UserData.Instance.IsEnoughGoods(UserData.EGoodsType.CASH, packageData.CashCount) == true)
        {
            AlramPopup popup = UIManager.Instance.GetPopup(BasePopup.EPopupType.Alram) as AlramPopup;
            popup.SetButtonType(BasePopup.EButtonType.Two);
            popup.SetTitle("현질");
            popup.SetDesc($"{packageData.CashCount}원으로 패키지 살꺼에요?");
            popup.SetConfirmBtLabel("구매");
            popup.SetCancelBtLabel("안삼");
            popup.SetConfirmCallBack(() =>
            {
                NetworkManager.Instance.SendProtocol(NetworkManager.ESOCKET_PROTOCOL.SHOP_BUY, packageData.UID);
                popup.DeActive();
                //await NetworkPacketShop.Instance.TaskBuyShopItem(packageData.UID,
                //    successCb: () =>
                //    {
                //        LobbyManager.Instance.RefreshGoodsLabel();
                //        popup.DeActive();
                //    });
            });
            popup.Active();
        }
        else
        {
            UIManager.Instance.GetChargePopup(UserData.EGoodsType.CASH);

        }
    }
    #endregion
}
