using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Coffee.UIExtensions;
using UnityEngine.U2D;
using EnhancedUI.EnhancedScroller;
using UnityEngine.Purchasing;

public class ShopItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text labelTitle = null;

    [SerializeField]
    private AtlasImage imgItem = null;
    [SerializeField]
    private TMP_Text labelCount = null;

    [SerializeField]
    private AtlasImage imgCash = null;
    [SerializeField]
    private TMP_Text labelCashCount = null;

    [SerializeField]
    private CodelessIAPButton iapBt = null;

    private ShopGoodsData shopData = null;
    private Action onCallback = null;

    public virtual void SetData(ShopData shopData)
    {
        if(shopData == null)
        {
            this.gameObject.SetActive(false);
            return;
        }

        this.shopData = (ShopGoodsData)shopData;

        labelTitle.text = this.shopData.Title;
        labelCount.text = this.shopData.ItemCount.ToString();
        labelCashCount.text = this.shopData.CashCount.ToString();

        if (!string.IsNullOrEmpty(this.shopData.AtlasPath))
        {
            AtlasManager.Instance.SetSprite(imgItem, AtlasManager.Instance.Atlas[this.shopData.AtlasPath], this.shopData.ItemImgPath);
        }

        if (!string.IsNullOrEmpty(this.shopData.CashAtlasPath))
        {
            AtlasManager.Instance.SetSprite(imgCash, AtlasManager.Instance.Atlas[this.shopData.CashAtlasPath], this.shopData.CashImgPath);
        }

        onCallback = this.shopData.Callback;        

        this.gameObject.SetActive(true);


        
        //임시
        iapBt.onPurchaseComplete.RemoveAllListeners();
        iapBt.productId = "test_001";
        iapBt.onPurchaseComplete.AddListener((product) =>
        {
            Debug.LogError(product.transactionID);
            Debug.LogError(product.receipt);

            NetworkManager.Instance.SendProtocol(NetworkManager.ESOCKET_PROTOCOL.SHOP_BUY, shopData.UID);
            LoadingManager.Instance.Reset(LoadingManager.EState.Loading);

            //await NetworkPacketShop.Instance.TaskBuyShopItem(shopData.UID,
            //    successCb: () =>
            //    {
            //        LobbyManager.Instance.RefreshGoodsLabel();
            //        LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
            //    });
        });

    }

    public virtual void OnClickShopItem()
    {
        if(shopData == null)
        {
            return;
        }

        if (onCallback != null)
        {
            onCallback.Invoke();
        }
    }
}
