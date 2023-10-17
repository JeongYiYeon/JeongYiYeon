using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class LobbyManager : MonoSingletonInScene<LobbyManager>
{
    private enum EBottomMenu
    { 
        HeroStat = -1,
        Hero,
        HeroPos,
        Challenge,
        Shop,
        Quest
    }


    [SerializeField]
    private Canvas[] canvas;

    [SerializeField]
    private Animator animator = null;

    [SerializeField]
    private TMP_Text labelCash = null;
    [SerializeField]
    private TMP_Text labelGold = null;

    [SerializeField]
    private UtilState state = null;

    [SerializeField]
    private RectTransform menuRt = null;

    [SerializeField]
    private UtilState[] bottomMenuState = null;

    [SerializeField]
    private Shop shop = null;
    [SerializeField]
    private Hero hero = null;
    public Shop Shop => shop;

    private Sequence menuSeq = null;
    private bool isMenuOpen = false;

    private void Awake()
    {
        CameraManager.Instance.SetCanvasCamera(canvas);

        RefreshGoodsLabel();

        menuRt.sizeDelta = Vector2.zero;
        menuRt.gameObject.SetActive(false);

        animator.SetTrigger("Open");
    }

    public void RefreshGoodsLabel()
    {
        SetCashLabel();
        SetGoldLabel();
    }

    public void RefreshHeroMenu(BaseCharacter.CHARACTER_TYPE type = BaseCharacter.CHARACTER_TYPE.NONE)
    {
        hero.SetCharacterType(type);
        //hero.RefreshToggle();
    }

    private void SetCashLabel()
    {
        labelCash.text = UserData.Instance.user.Goods.GetGoodsString(UserData.EGoodsType.CASH);
    }
    public void SetGoldLabel()
    {
        labelGold.text = UserData.Instance.user.Goods.GetGoodsString(UserData.EGoodsType.GOLD);
    }

    public async void SetMenu(BaseEnum.EMenuCategory mainCategory, params object[] enumValue)
    {
        switch (mainCategory)
        {
            case BaseEnum.EMenuCategory.AdBuff:
                AdBuffPopup adBuffPopup =
                    UIManager.Instance.GetPopup(BasePopup.EPopupType.AdBuff) as AdBuffPopup;
                adBuffPopup.Active();
                break;

            case BaseEnum.EMenuCategory.Setting:
                SettingPopup setting = UIManager.Instance.GetPopup(BasePopup.EPopupType.Setting) as SettingPopup;

                EnumSetting.ESettingCategory settingCategory = 
                    (EnumSetting.ESettingCategory)Enum.Parse(typeof(EnumSetting.ESettingCategory), enumValue[0].ToString());

                setting.SetCancelCallback(() =>
                {
                    if(setting.CsSetting.isActivePolicy == true)
                    {
                        setting.CsSetting.DeActivePolicy();
                    }
                    else if (setting.CsSetting.isCustomerSupport == true)
                    {
                        setting.CsSetting.DeActiveCS();
                    }
                    else
                    {
                        setting.DeActive();
                    }
                });

                setting.Active();
                setting.SetSettingCategory(settingCategory);
                break;

            case BaseEnum.EMenuCategory.Hero:

                if (state.ActiveStateName() == mainCategory.ToString())
                {
                    bottomMenuState[(int)EBottomMenu.Hero].ActiveState("Off");
                    state.DeActiveState(mainCategory.ToString());
                    state.ActiveState(EBottomMenu.HeroStat.ToString());
                }
                else
                {
                    ResetBottomMenuState();

                    bottomMenuState[(int)EBottomMenu.Hero].ActiveState("On");

                    state.ActiveState(mainCategory.ToString());
                    RefreshHeroMenu(BaseCharacter.CHARACTER_TYPE.HERO);
                }
                break;
            case BaseEnum.EMenuCategory.HeroPos:

                if (state.ActiveStateName() == mainCategory.ToString())
                {
                    bottomMenuState[(int)EBottomMenu.HeroPos].ActiveState("Off");

                    state.DeActiveState(mainCategory.ToString());
                    state.ActiveState(EBottomMenu.HeroStat.ToString());
                }
                else
                {
                    ResetBottomMenuState();

                    bottomMenuState[(int)EBottomMenu.HeroPos].ActiveState("On");

                    state.ActiveState(mainCategory.ToString());

                    HeroPosition.Instance.InitHeroPosition();
                }
                break;
            case BaseEnum.EMenuCategory.Shop:

                if (state.ActiveStateName() == mainCategory.ToString())
                {
                    bottomMenuState[(int)EBottomMenu.Shop].ActiveState("Off");

                    state.DeActiveState(mainCategory.ToString());
                    state.ActiveState(EBottomMenu.HeroStat.ToString());
                }
                else
                {
                    ResetBottomMenuState();

                    bottomMenuState[(int)EBottomMenu.Shop].ActiveState("On");

                    state.ActiveState(mainCategory.ToString());

                    if (enumValue.Length > 0)
                    {
                        EnumShop.EShopSubCategory tmpCategory =
                            (EnumShop.EShopSubCategory)Enum.Parse(typeof(EnumShop.EShopSubCategory), enumValue[0].ToString());

                        shop.SetShopSubCategory(tmpCategory);
                        shop.RefreshToggle();
                    }
                }
                break;

            case BaseEnum.EMenuCategory.Challenge:

                if (state.ActiveStateName() == mainCategory.ToString())
                {
                    bottomMenuState[(int)EBottomMenu.Challenge].ActiveState("Off");

                    state.DeActiveState(mainCategory.ToString());
                    state.ActiveState(EBottomMenu.HeroStat.ToString());
                }
                else
                {
                    ResetBottomMenuState();

                    bottomMenuState[(int)EBottomMenu.Challenge].ActiveState("On");

                    state.ActiveState(mainCategory.ToString());
                }
                break;

            case BaseEnum.EMenuCategory.Mail:
                MailPopup mail = UIManager.Instance.GetPopup(BasePopup.EPopupType.Mail) as MailPopup;
                mail.Active();

                mail.InitMailData();
                break;
            case BaseEnum.EMenuCategory.Attendance:
                AttendancePopup attendancePopup =
                    UIManager.Instance.GetPopup(BasePopup.EPopupType.Attendance) as AttendancePopup;

                EnumAttendance.EAttendance category =
                  (EnumAttendance.EAttendance)Enum.Parse(typeof(EnumAttendance.EAttendance), enumValue[0].ToString());

                attendancePopup.Active();
                attendancePopup.SetCategory(category);
                break;

            case BaseEnum.EMenuCategory.Quest:
                if (state.ActiveStateName() == mainCategory.ToString())
                {
                    bottomMenuState[(int)EBottomMenu.Quest].ActiveState("Off");

                    state.DeActiveState(mainCategory.ToString());
                    state.ActiveState(EBottomMenu.HeroStat.ToString());
                }
                else
                {
                    await NetworkPacketQuest.Instance.TaskQuestList((questList) =>
                    {
                        ResetBottomMenuState();
                        bottomMenuState[(int)EBottomMenu.Quest].ActiveState("On");

                        state.ActiveState(mainCategory.ToString());
                        Quest.Instance.InitQuestData(questList);
                        LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
                    });
                }               
                break;

            case BaseEnum.EMenuCategory.GamePass:
                GamePassPopup gamePassPopup =
                    UIManager.Instance.GetPopup(BasePopup.EPopupType.GamePass) as GamePassPopup;

                gamePassPopup.Active();
                break;

            case BaseEnum.EMenuCategory.Inventory:
                InventoryPopup inventoryPopup =
                    UIManager.Instance.GetPopup(BasePopup.EPopupType.Inventory) as InventoryPopup;

                inventoryPopup.Active();
                break;

            case BaseEnum.EMenuCategory.ConnectReward:
                ConnectRewardPopup connectRewardPopup =
                    UIManager.Instance.GetPopup(BasePopup.EPopupType.ConnectReward) as ConnectRewardPopup;

                //보상 받을 정보 있으면 셋팅
                //connectRewardPopup.SetRewardItemDataList();
                connectRewardPopup.SetConfirmCallBack(() => 
                {
                    //받음처리
                    connectRewardPopup.DeActive();
                });
                connectRewardPopup.Active();
                break;

            default:
                break;
        }
    }

    private void ResetBottomMenuState()
    {
        for(int i = 0; i < bottomMenuState.Length; i++)
        {
            bottomMenuState[i].ActiveState("Off");
        }
    }

    #region OnClick

    public void OnClickMenu(UtilEnumSelect category)
    {
        if (category.ShopSubCategory != EnumShop.EShopSubCategory.None)
        {
            SetMenu(category.MenuCategory, category.ShopSubCategory);
        }
        else if (category.SelectSettingMenu != EnumSetting.ESelectSettingMenu.None)
        {
            SetMenu(category.MenuCategory, category.SettingCategory);
        }
        else if (category.AttendanceType != EnumAttendance.EAttendance.None)
        {
            SetMenu(category.MenuCategory, category.AttendanceType);
        }
        else
        {
            SetMenu(category.MenuCategory);
        }

    }

    public void OnClickOpenMenu()
    {
        if (menuSeq != null)
        {
            menuSeq.Kill();
            menuSeq = null;
        }

        menuSeq = DOTween.Sequence();

        if (isMenuOpen == false)
        {
            menuRt.gameObject.SetActive(true);

            menuSeq.Append(menuRt.DOSizeDelta(new Vector2(250f, 300f), 0.15f).SetEase(Ease.OutQuad));            
            menuSeq.AppendCallback(() =>
            {
                isMenuOpen = true;
            });
            menuSeq.Play();
        }
        else
        {
            menuSeq.Append(menuRt.DOSizeDelta(Vector2.zero, 0.15f).SetEase(Ease.OutQuad));
            menuSeq.AppendCallback(() =>
                {
                    menuRt.gameObject.SetActive(false);
                    isMenuOpen = false;
                }
            );
            menuSeq.Play();
        }
    }
    #endregion
}
