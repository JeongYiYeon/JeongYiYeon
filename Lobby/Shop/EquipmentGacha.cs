using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class EquipmentGacha : MonoBehaviour
{
    [SerializeField]
    private TMP_Text labelOneGachaPrice = null;

    [SerializeField]
    private TMP_Text labelTenGachaPrice = null;

    private int oneGachaPrice = 0;
    private int tenGachaPrice = 0;

    private DataShop oneGachaData = null;
    private DataShop tenGachaData = null;

    private void Awake()
    {
        InitEquipmentGacha();
    }

    public void SetOneGachaData(DataShop oneGachaData)
    {
        this.oneGachaData = oneGachaData;
    }
    public void SetTenGachaData(DataShop tenGachaData)
    {
        this.tenGachaData = tenGachaData;
    }

    public void SetOneGachaPrice(int oneGachaPrice)
    {
        this.oneGachaPrice = oneGachaPrice;
    }
    public void SetTenGachaPrice(int tenGachaPrice)
    {
        this.tenGachaPrice = tenGachaPrice;
    }

    private void InitEquipmentGacha()
    {
        labelOneGachaPrice.text = oneGachaPrice.ToString("#,##0");
        labelTenGachaPrice.text = tenGachaPrice.ToString("#,##0");
    }

    #region OnClick
    public void OnClickGacha(UtilEnumSelect category)
    {
        DataShop shopData = null;
        int price = 0;

        switch (category.EquipmentGachaCount)
        {
            case EnumShop.EEquipmentGachaCount.One:
                shopData = oneGachaData;
                price = oneGachaPrice;
                break;
            case EnumShop.EEquipmentGachaCount.Ten:
                shopData = tenGachaData;
                price = tenGachaPrice;
                break;
        }

        if (category.EquipmentGachaCount != EnumShop.EEquipmentGachaCount.AdTen)
        {
            if (UserData.Instance.IsEnoughGoods(UserData.EGoodsType.CASH, price) == true)
            {
                AlramPopup popup = UIManager.Instance.GetPopup(BasePopup.EPopupType.Alram) as AlramPopup;
                popup.SetButtonType(BasePopup.EButtonType.Two);
                popup.SetTitle("가챠");
                popup.SetDesc($"{price}캐쉬로 가챠 할꺼에요?");
                popup.SetConfirmBtLabel($"{category.EquipmentGachaCount}회 장비 뽑기");
                popup.SetCancelBtLabel("안삼");
                popup.SetConfirmCallBack(() =>
                {
                    NetworkManager.Instance.SendProtocol(NetworkManager.ESOCKET_PROTOCOL.SHOP_BUY, shopData.UID);
                    popup.DeActive();
                    //await NetworkPacketShop.Instance.TaskBuyShopItem(shopData.UID,
                    //successCb: () =>
                    //{
                    //    LobbyManager.Instance.RefreshGoodsLabel();
                    //    popup.DeActive();
                    //});
                });
                popup.Active();
            }
            else
            {
                UIManager.Instance.GetChargePopup(UserData.EGoodsType.CASH);
            }
        }
        else
        {
            AdmobManager.Instance.OnClickShowRewardAD(async () =>
            {
                await UniTask.Delay(200);
                NetworkManager.Instance.SendProtocol(NetworkManager.ESOCKET_PROTOCOL.AD, "7");
            });
        }
    }
    #endregion
}
