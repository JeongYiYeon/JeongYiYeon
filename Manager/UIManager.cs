using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField]
    private Transform canvasTf;

    [SerializeField]
    private UtilPlayEffect effect;

    private const string PopupPath = "Prefab/Popup/";

    private Dictionary<BasePopup.EPopupType, BasePopup> popupDic = new Dictionary<BasePopup.EPopupType, BasePopup>();

    public Transform CanvasTf => canvasTf;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 effectPos = CameraManager.Instance.GetCamera(CameraManager.ECamera.UI).
                 WorldToScreenPoint(effect.transform.position);

            effect.transform.position =
            CameraManager.Instance.GetCamera(CameraManager.ECamera.UI).ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, effectPos.z));

            effect.PlayEffect(1f, "TouchEffect", isForce: true);
        }
    }

    private string GetPopupPath(BasePopup.EPopupType popupType)
    {
        return PopupPath + popupType.ToString();
    }

    private void AllDeActivePopup()
    {
        foreach(BasePopup popup in popupDic.Values)
        {
            if (popup != null)
            {
                popup.DeActive();
            }
        }
    }

    public BasePopup GetPopup(BasePopup.EPopupType popupType)
    {
        if (popupDic.TryGetValue(popupType, out var popup) == false)
        {
            GameObject go = AddressableManager.Instance.Instantiate(popupType.ToString(), canvasTf.transform);

            if(go == null)
            {
                go = Instantiate(Resources.Load<GameObject>(GetPopupPath(popupType)), canvasTf.transform);
            }

            popup = go.GetComponent<BasePopup>();

            go.SetActive(false);

            popupDic.Add(popupType, popup);
        }

        if(popup == null)
        {
            popupDic.Remove(popupType);
        }

        return popup;
    }    

    public void GetChargePopup(UserData.EGoodsType goodsType)
    {
        string title = "";
        string desc = "";

        AlramPopup popup = GetPopup(BasePopup.EPopupType.Alram) as AlramPopup;

        switch (goodsType)
        {
            case UserData.EGoodsType.CASH:
                title = "현금 부족!";
                desc = "현금이 부족합니다.\n 충전 하시겠습니까?";
               
                popup.SetConfirmCallBack(() =>
                {
                    BaseEnum baseEnum = new BaseEnum();
                    baseEnum.SetMenuCategory(BaseEnum.EMenuCategory.Shop);
                    EnumShop shopEnum = new EnumShop();
                    shopEnum.SetShopSubCategory(EnumShop.EShopSubCategory.Cash);

                    AllDeActivePopup();

                    LobbyManager.Instance.Shop.SetShopSubCategory(shopEnum.ShopSubCategory);
                    LobbyManager.Instance.Shop.RefreshToggle();                    
                });

                break;
            //case UserData.EGoodsType.GOLD:
            //    title = "게임돈 부족!";
            //    desc = "게임돈이 부족합니다.\n 충전 하시겠습니까?";

            //    popup.SetConfirmCallBack(() =>
            //    {
            //        BaseEnum baseEnum = new BaseEnum();
            //        baseEnum.SetMenuCategory(BaseEnum.EMenuCategory.Shop);
            //        EnumShop shopEnum = new EnumShop();
            //        shopEnum.SetShopSubCategory(EnumShop.EShopSubCategory.Gold);

            //        AllDeActivePopup();
            //        LobbyManager.Instance.SetMenu(baseEnum.MenuCategory, shopEnum.ShopSubCategory);
            //    });

            //    break;
        }

        popup.SetTitle(title);
        popup.SetDesc(desc);
        popup.SetButtonType(BasePopup.EButtonType.Two);
        popup.SetConfirmBtLabel("충전하기");
        popup.SetCancelBtLabel("충전안함");

        popup.Active();
    }
}
