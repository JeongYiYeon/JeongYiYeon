using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using UnityEngine.EventSystems;
using System;

public class Shop : MonoBehaviour
{
    [SerializeField]
    private UtilState state = null;    

    [SerializeField]
    private EquipmentGacha equipmentGacha = null;

    [SerializeField]
    private ShopScrollController scrollViewController = null;
    [SerializeField]
    private ShopPackageScrollController packageScrollViewController = null;

    [SerializeField]
    private ToggleGroup shopSubCategoryToggle = null;

    private EnumShop.EShopSubCategory curSubCategory = EnumShop.EShopSubCategory.None;

    private Dictionary<EnumShop.EShopSubCategory, List<ShopData>> dicShopInfo = new Dictionary<EnumShop.EShopSubCategory, List<ShopData>>();

    private void OnEnable()
    {
        StartCoroutine(ToggleOn());

        InitShopData();
    }

    private void Start()
    {
        SetShopSubCategory(curSubCategory);
        RefreshToggle();
    }

    private void InitShopData()
    {
        SetShopCashData();
        SetShopPackageData();
    }

    private void SetShopCashData()
    {
        if (dicShopInfo.ContainsKey(EnumShop.EShopSubCategory.Cash) == false)
        {
            List<DataShop> dataShopList =
                DataManager.Instance.DataHelper.Shop.FindAll(x => x.EShopSubCategory == EnumShop.EShopSubCategory.Cash);

            if(dataShopList.Count == 0)
            {
                return;
            }

            List<ShopData> list = new List<ShopData>();

            for (int i = 0; i < dataShopList.Count; i++)
            {
                ShopGoodsData shopData = new ShopGoodsData();
                shopData.SetUID(dataShopList[i].UID);
                shopData.SetTitle(DataManager.Instance.GetLocalization(dataShopList[i].GOODS_NAME));
                shopData.SetAtlasPath(dataShopList[i].ITEM_ICON_ATLAS);
                shopData.SetItemImgPath(dataShopList[i].GOODS_ICON);
                shopData.SetItemCount(dataShopList[i].REWARD_ITEM_COUNT);
                shopData.SetCashCount(dataShopList[i].PAYMENT_PRICE);
                //shopData.SetCallback( () =>
                //{
                    //AlramPopup popup = UIManager.Instance.GetPopup(BasePopup.EPopupType.Alram) as AlramPopup;
                    //popup.SetButtonType(BasePopup.EButtonType.Two);
                    //popup.SetTitle("현질");
                    //popup.SetDesc($"{shopData.CashCount}원으로 {shopData.ItemCount} 캐쉬 살꺼에요?");
                    //popup.SetConfirmBtLabel("구매");
                    //popup.SetCancelBtLabel("안삼");
                    //popup.SetConfirmCallBack(async () =>
                    //{
                    //    await NetworkPacketShop.Instance.TaskBuyShopItem(shopData.UID,
                    //        successCb: () =>
                    //        {
                    //            LobbyManager.Instance.RefreshGoodsLabel();
                    //            popup.DeActive();
                    //        });

                    //});
                    //popup.Active();
                //});

                list.Add(shopData);
            }

            dicShopInfo.Add(EnumShop.EShopSubCategory.Cash, list);
        }
    }

    private void SetShopPackageData()
    {
        if (dicShopInfo.ContainsKey(EnumShop.EShopSubCategory.Package) == false)
        {
            List<DataShop> dataShopList =
                DataManager.Instance.DataHelper.Shop.FindAll(x => x.EShopSubCategory == EnumShop.EShopSubCategory.Package);

            if (dataShopList.Count == 0)
            {
                return;
            }


            List<ShopData> list = new List<ShopData>();

            for (int i = 0; i < dataShopList.Count; i++)
            {
                if (dataShopList[i].REWARD_LINK_POSSIBLE == true)
                {
                    if (dataShopList[i].EEquipmentGachaCount != EnumShop.EEquipmentGachaCount.None)
                    {
                        List<DataRewardLink> gachaRewardItemList =
                        DataManager.Instance.DataHelper.RewardLink.FindAll(x => x.REWARD_GROUP == dataShopList[i].REWARD_LINK_GROUP);

                        List<BaseItemData> gachaItemList = new List<BaseItemData>();

                        if (gachaRewardItemList.Count == 0)
                        {
                            continue;
                        }

                        for (int j = 0; j < gachaRewardItemList.Count; j++)
                        {
                            BaseItemData item = new BaseItemData();

                            DataItem itemData = DataManager.Instance.DataHelper.Item.Find(x => x.UID == gachaRewardItemList[j].REWARD_ITEM);

                            if (itemData == null)
                            {
                                continue;
                            }
                            item.SetDataItem(itemData);
                            item.SetItemUID(itemData.UID);
                            item.SetTitle(DataManager.Instance.GetLocalization(itemData.ITEM_NAME));
                            item.SetDesc(DataManager.Instance.GetLocalization(itemData.ITEM_DESC));
                            item.SetItemCategory(Enum.Parse<BaseItem.EItemCategory>(itemData.ITEM_CATEGORY));
                            item.SetItemGrade(itemData.ITEM_GRADE);
                            item.SetRewardItemRate((float)gachaRewardItemList[j].REWARD_ITEM_RATE);
                            item.SetAtlasPath(itemData.ITEM_ICON_ATLAS);
                            item.SetImgItemPath(itemData.ITEM_ICON);

                            item.SetItemCount(gachaRewardItemList[j].REWARD_ITEM_COUNT);

                            gachaItemList.Add(item);
                        }

                        if (dataShopList[i].EEquipmentGachaCount == EnumShop.EEquipmentGachaCount.One)
                        {
                            equipmentGacha.SetOneGachaData(dataShopList[i]);
                            equipmentGacha.SetOneGachaPrice(dataShopList[i].BUY_ITEM_COUNT);
                            //equipmentGacha.SetGachaItemDic(EnumShop.EEquipmentGachaCount.One.ToString(), gachaItemList);
                        }
                        else if(dataShopList[i].EEquipmentGachaCount == EnumShop.EEquipmentGachaCount.Ten)
                        {
                            equipmentGacha.SetTenGachaData(dataShopList[i]);
                            equipmentGacha.SetTenGachaPrice(dataShopList[i].BUY_ITEM_COUNT);
                            //equipmentGacha.SetGachaItemDic(EnumShop.EEquipmentGachaCount.Ten.ToString(), gachaItemList);
                        }

                        continue;
                    }

                    PackageData packageData = new PackageData();
                    packageData.SetUID(dataShopList[i].UID);
                    packageData.SetTitle(DataManager.Instance.GetLocalization(dataShopList[i].GOODS_NAME));
                    packageData.SetDesc(DataManager.Instance.GetLocalization(dataShopList[i].GOODS_DESC));


                    List<DataRewardLink> rewardItemList =
                        DataManager.Instance.DataHelper.RewardLink.FindAll(x => x.REWARD_GROUP == dataShopList[i].REWARD_LINK_GROUP);

                    if (rewardItemList.Count == 0)
                    {
                        continue;
                    }

                    List<BaseItemData> itemList = new List<BaseItemData>();

                    for (int j = 0; j < rewardItemList.Count; j++)
                    {
                        BaseItemData item = new BaseItemData();

                        DataItem itemData = DataManager.Instance.DataHelper.Item.Find(x => x.UID == rewardItemList[j].REWARD_ITEM);

                        if (itemData == null)
                        {
                            continue;
                        }

                        item.SetDataItem(itemData);
                        item.SetItemUID(itemData.UID);
                        item.SetTitle(DataManager.Instance.GetLocalization(itemData.ITEM_NAME));
                        item.SetDesc(DataManager.Instance.GetLocalization(itemData.ITEM_DESC));
                        item.SetItemCategory(Enum.Parse<BaseItem.EItemCategory>(itemData.ITEM_CATEGORY));
                        item.SetItemGrade(itemData.ITEM_GRADE);
                        item.SetAtlasPath(itemData.ITEM_ICON_ATLAS);
                        item.SetImgItemPath(itemData.ITEM_ICON);
                        item.SetViewItemCount(rewardItemList[j].REWARD_ITEM_COUNT);

                        itemList.Add(item);
                    }

                    packageData.SetItemList(itemList);

                    packageData.SetCashCount(dataShopList[i].PAYMENT_PRICE);

                    list.Add(packageData);
                }
                else
                {
                    continue;
                }
            }

            dicShopInfo.Add(EnumShop.EShopSubCategory.Package, list);

        }
    }

    public List<ShopData> GetShopItemInfo(EnumShop.EShopSubCategory category)
    {
        List<ShopData> shopData = null;

        if (dicShopInfo.TryGetValue(category, out shopData) == true)
        {
            return shopData;
        }

        return null;
    }

    private IEnumerator ToggleOn()
    {
        yield return new WaitForEndOfFrame();

        Transform tf = shopSubCategoryToggle.transform.Find(curSubCategory.ToString());
       
        if (tf != null)
        {
            tf.GetComponent<Toggle>().SetIsOnWithoutNotify(true);
        }
    }

    public void SetShopSubCategory(EnumShop.EShopSubCategory category)
    {
        if (category == EnumShop.EShopSubCategory.None)
        {
            category = EnumShop.EShopSubCategory.Cash;
        }

        curSubCategory = category;

        state.ActiveState(curSubCategory.ToString());

        switch(curSubCategory)
        {
            case EnumShop.EShopSubCategory.Cash:
                scrollViewController.SetShopInfo(curSubCategory);
                break;
            case EnumShop.EShopSubCategory.Package:
                packageScrollViewController.SetShopInfo(curSubCategory);
                break;
            case EnumShop.EShopSubCategory.Gacha:
                break;
        }
    }

    public void RefreshToggle()
    {
        StartCoroutine(ToggleOn());
    }

    public void OnClickShopSubCategory(UtilEnumSelect category)
    {
        SetShopSubCategory(category.ShopSubCategory);
    }
}
