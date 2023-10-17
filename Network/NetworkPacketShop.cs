using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPacketShop : Singleton<NetworkPacketShop>
{
    public enum EWatchAD
    {
        NONE,
        AD,
    }

    private const string requestPath = "/shop";

    private const string BuyShopItem = requestPath + "/buy";
    private const string BuyHero = requestPath + "/hero";
    private const string BuyEnergy = requestPath + "/energy";

    private class ShopRequestParameter
    {
        private int uid;
        private bool isAD;

        public int UID => uid;
        public bool isAd => isAD;

        public void SetUID(int uid)
        {
            this.uid = uid;
        }

        public void SetAD(EWatchAD ad)
        {
            switch (ad)
            {
                case EWatchAD.NONE:
                    this.isAD = false;
                    break;
                case EWatchAD.AD:
                    this.isAD = true;
                    break;
            }
        }
    }

    public async UniTask TaskBuyShopItem(int shopItemUID, Action successCb = null)
    {
        ShopRequestParameter parameter = new ShopRequestParameter();
        parameter.SetUID(shopItemUID);

        await NetworkManager.Instance.SendRequest<JObject>(BuyShopItem,
            form: NetworkManager.Instance.SerializeObject(parameter),
            _successCb: (jsonData) =>
            {
                if(jsonData != null)
                {
                    List<BaseItemData> rewardItemList = new List<BaseItemData>();

                    JArray getRewardList = jsonData.Value<JArray>("SELECTED_LIST");

                    if(getRewardList != null && getRewardList.Count > 0)
                    {
                        foreach (JObject rewardData in getRewardList)
                        {
                            DataItem itemData = DataManager.Instance.DataHelper.Item.Find(x => x.UID == rewardData.Value<int>("IID"));

                            BaseItemData tmpItemData = new BaseItemData();

                            tmpItemData.SetDataItem(itemData);
                            tmpItemData.SetItemUID(itemData.UID);
                            tmpItemData.SetTitle(DataManager.Instance.GetLocalization(itemData.ITEM_NAME));
                            tmpItemData.SetDesc(DataManager.Instance.GetLocalization(itemData.ITEM_DESC));
                            tmpItemData.SetItemCategory(Enum.Parse<BaseItem.EItemCategory>(itemData.ITEM_CATEGORY));
                            tmpItemData.SetItemGrade(itemData.ITEM_GRADE);
                            tmpItemData.SetAtlasPath(itemData.ITEM_ICON_ATLAS);
                            tmpItemData.SetImgItemPath(itemData.ITEM_ICON);
                            tmpItemData.SetViewItemCount(rewardData.Value<int>("COUNT"));

                            rewardItemList.Add(tmpItemData);
                        }
                    }

                    JObject goodsInfo = jsonData.Value<JObject>("TOTAL");

                    if (goodsInfo != null)
                    {
                        JObject getMoney = goodsInfo.Value<JObject>("MONEY");

                        if (getMoney != null)
                        {
                            UserData.Instance.user.SetGoods(
                                NetworkManager.Instance.CopyData<UserData.UserGoods>(getMoney, UserData.Instance.user.Goods));
                        }

                        JArray itemArray = goodsInfo.Value<JArray>("ITEM");

                        if (itemArray != null && itemArray.Count > 0)
                        {
                            foreach (JObject item in itemArray)
                            {
                                UserData.Instance.user.Item.SetItem(item);
                            }
                        }
                    }

                    if (successCb != null)
                    {
                        successCb.Invoke();
                    }

                    ShowRewardItemsPopup showRewardItemsPopup = UIManager.Instance.GetPopup(BasePopup.EPopupType.ShowRewardItems) as ShowRewardItemsPopup;
                    showRewardItemsPopup.SetTitle("획득 아이템");
                    showRewardItemsPopup.SetDesc("여기 누르면 창 꺼짐!");
                    showRewardItemsPopup.SetRewardItemDataList(rewardItemList);
                    showRewardItemsPopup.Active();
                }
            });
    }

    public async UniTask TaskBuyHero(int characterShopUID, Action successCb = null)
    {
        ShopRequestParameter parameter = new ShopRequestParameter();
        parameter.SetUID(characterShopUID);

        await NetworkManager.Instance.SendRequest<JObject>(
            BuyHero,
            form: NetworkManager.Instance.SerializeObject(parameter),
            _successCb: (jsonData) =>
            {
                if (jsonData != null)
                {
                    UserData.Instance.user.SetGoods(
                        NetworkManager.Instance.CopyData<UserData.UserGoods>(jsonData.Value<JObject>("MONEY"), UserData.Instance.user.Goods));

                    JArray characterArray = jsonData.Value<JArray>("CHARACTER");

                    if (characterArray != null && characterArray.Count > 0)
                    {
                        List<CharacterData> haveCharacterList = new List<CharacterData>();

                        foreach (JObject character in characterArray)
                        {
                            BaseCharacter.CHARACTER_TYPE type = BaseCharacter.CHARACTER_TYPE.NONE;

                            if (Enum.TryParse<BaseCharacter.CHARACTER_TYPE>(character.Value<string>("CHA_TYPE"), out type))
                            {
                                if (type != BaseCharacter.CHARACTER_TYPE.NONE)
                                {
                                    CharacterData characterData = new CharacterData();

                                    DataCharacter dataCharacter = DataManager.Instance.DataHelper.Character.Find(x => x.UID == character.Value<int>("UID"));

                                    if (dataCharacter == null)
                                    {
                                        Debug.LogError("캐릭터 정보 오류");
                                        continue;
                                    }

                                    characterData.SetDataCharacter(dataCharacter);
                                    characterData.SetUpgradeCnt(character.Value<int>("ENCHANTED_COUNT"));
                                    characterData.SetPositionIdx(character.Value<int>("POSITION"));
                                    //characterData.SetHeroEquipment()

                                    UserData.Instance.user.Character.SetAddCharacterData(characterData);
                                }
                            }
                        }
                    }

                    if (successCb != null)
                    {
                        successCb.Invoke();
                    }
                }
                
            });

    }

    public async UniTask TaskBuyEnergy(EWatchAD ad, Action successCb = null)
    {
        ShopRequestParameter parameter = new ShopRequestParameter();
        parameter.SetAD(ad);

        await NetworkManager.Instance.SendRequest<JObject>(
            BuyEnergy,
            form: NetworkManager.Instance.SerializeObject(parameter),
            _successCb: (jsonData) =>
            {
                if (jsonData != null)
                {
                    JObject goodsInfo = jsonData.Value<JObject>("TOTAL");

                    UserData.Instance.user.SetGoods(
                        NetworkManager.Instance.CopyData<UserData.UserGoods>(goodsInfo, UserData.Instance.user.Goods));

                    if (successCb != null)
                    {
                        successCb.Invoke();
                    }
                }

            });

    }


}
