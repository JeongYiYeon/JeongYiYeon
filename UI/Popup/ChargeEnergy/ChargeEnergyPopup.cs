using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeEnergyPopup : BasePopup
{
    private enum ChrageType
    {
        Energy,
        Ad,
        TapjoyAd,
    }

    [SerializeField]
    private Charge[] charges = null;

    private void OnEnable()
    {
        //SetType(EPopupType.ChargeEnergy);
    }

    public override void Init()
    {
        base.Init();

        //charges[(int)ChrageType.Energy].SetCharge(
        //    UserData.Instance.user.Config.BuyEnergyCashRewardEnergy,
        //    UserData.Instance.user.Goods.EnergyBuyCount,
        //    UserData.Instance.user.Config.BuyEnergyCash);

        //charges[(int)ChrageType.Energy].SetPurchase("Common_Icon_Gem", UserData.Instance.user.Config.BuyEnergyCashCost);
        //charges[(int)ChrageType.Energy].SetConfirmCallback(
        //    () =>
        //    {
        //        if (UserData.Instance.user.Goods.EnergyBuyCount < UserData.Instance.user.Config.BuyEnergyCash)
        //        {
        //            AlramPopup popup = UIManager.Instance.GetPopup(BasePopup.EPopupType.Alram) as AlramPopup;
        //            popup.SetButtonType(BasePopup.EButtonType.Two);
        //            popup.SetTitle("스테미너 구매");
        //            popup.SetDesc($"{UserData.Instance.user.Config.BuyEnergyCashCost}캐쉬로 {UserData.Instance.user.Config.BuyEnergyCashRewardEnergy} 에너지 살꺼에요?");
        //            popup.SetConfirmBtLabel("구매");
        //            popup.SetCancelBtLabel("안삼");
        //            popup.SetConfirmCallBack(async () =>
        //            {
        //                if (UserData.Instance.IsEnoughGoods(UserData.EGoodsType.CASH, UserData.Instance.user.Config.BuyEnergyCashCost))
        //                {
        //                    await NetworkPacketShop.Instance.TaskBuyEnergy(NetworkPacketShop.EWatchAD.NONE,
        //                        successCb: () =>
        //                        {
        //                            charges[(int)ChrageType.Energy].SetCharge(
        //                                UserData.Instance.user.Config.BuyEnergyCashRewardEnergy,
        //                                UserData.Instance.user.Goods.EnergyBuyCount,
        //                                UserData.Instance.user.Config.BuyEnergyCash);

        //                            LobbyManager.Instance.RefreshGoodsLabel();

        //                            popup.DeActive();
        //                        });
        //                }
        //                else
        //                {
        //                    UIManager.Instance.GetChargePopup(UserData.EGoodsType.CASH);
        //                }
        //            });
        //            popup.Active();
        //        }
        //        else
        //        {
        //            Debug.LogError("충전 횟수 초과");
        //        }
        //    });

        //charges[(int)ChrageType.Ad].SetCharge(
        //    UserData.Instance.user.Config.BuyEnergyAdRewardEnergy,
        //    UserData.Instance.user.Goods.EnergyAdBuyCount,
        //    UserData.Instance.user.Config.BuyEnergyAd);

        //charges[(int)ChrageType.Ad].SetConfirmCallback(
        //    () =>
        //    {
        //        if (UserData.Instance.user.Goods.EnergyAdBuyCount < UserData.Instance.user.Config.BuyEnergyAd)
        //        {
        //            AdmobManager.Instance.OnClickShowRewardAD(async () =>
        //            {
        //                await NetworkPacketShop.Instance.TaskBuyEnergy(NetworkPacketShop.EWatchAD.AD,
        //                    successCb: () =>
        //                    {
        //                        charges[(int)ChrageType.Ad].SetCharge(
        //                            UserData.Instance.user.Config.BuyEnergyAdRewardEnergy,
        //                            UserData.Instance.user.Goods.EnergyAdBuyCount,
        //                            UserData.Instance.user.Config.BuyEnergyAd);

        //                        LobbyManager.Instance.RefreshGoodsLabel();
        //                    });
        //            });
        //        }
        //        else
        //        {
        //            Debug.LogError("충전 횟수 초과");
        //        }

        //    });

        //charges[(int)ChrageType.TapjoyAd].SetCharge(
        //    UserData.Instance.user.Config.BuyEnergyAdRewardEnergy,
        //    UserData.Instance.user.Goods.EnergyAdBuyCount,
        //    UserData.Instance.user.Config.BuyEnergyAd);

        //charges[(int)ChrageType.TapjoyAd].SetConfirmCallback(
        //    () =>
        //    {
        //        TapjoyManager.Instance.OnClickShowTapjoy();                
        //    });
    }

    public override void Active()
    {
        base.Active();
    }
}
