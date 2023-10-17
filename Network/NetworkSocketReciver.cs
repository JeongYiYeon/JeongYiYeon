using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkSocketReciver
{ 
    public class Ping
    {
        public string message;

        public Ping()
        {

        }
    }

    public class Connect
    {
        public JObject MONEY;
        public JObject STAT;
        public JArray CHARACTER;
        public DateTime LOGIN_DATE;
        public JArray ITEM;
        public JObject STAGE;
        public JObject RE_SESSION_TOKEN;

        public Connect()
        {
           
        }

        public void SetData()
        {
            if (MONEY != null)
            {
                UserData.Instance.user.SetGoods(JsonConvert.DeserializeObject<UserData.UserGoods>(MONEY.ToString()));
            }

            if (STAT != null)
            {
                UserData.Instance.user.SetStat(JsonConvert.DeserializeObject<UserData.UserStat>(STAT.ToString()));
            }

            if (CHARACTER != null)
            {
                if (CHARACTER.Count > 0)
                {
                    List<CharacterData> characterList = new List<CharacterData>();

                    foreach (JObject character in CHARACTER)
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

                                characterList.Add(characterData);
                            }
                        }
                    }

                    UserData.Instance.user.Character.SetCharacterDatas(characterList);
                }
            }

            if (LOGIN_DATE != null)
            {
                UserData.Instance.user.SetLoginTime(LOGIN_DATE);
            }

            if (ITEM != null)
            {
                if (ITEM.Count > 0)
                {
                    foreach (JObject item in ITEM)
                    {
                        UserData.Instance.user.Item.SetItem(item);
                    }
                }
            }

            if (STAGE != null)
            {
                UserData.Instance.user.SetStage(JsonConvert.DeserializeObject<UserData.UserStage>(STAGE.ToString()));
            }
            else
            {
                UserData.Instance.user.SetStage(new UserData.UserStage());
            }

            if (RE_SESSION_TOKEN != null)
            {
                UserData.Instance.user.SetReSessionToken(JsonConvert.DeserializeObject<string>(RE_SESSION_TOKEN.ToString()));
            }

        }
    }

    public class Stage
    {
        public JObject STAGE;

        public void SetData()
        {
            if (STAGE != null)
            {
                UserData.Instance.user.SetStage(JsonConvert.DeserializeObject<UserData.UserStage>(STAGE.ToString()));
            }
            else
            {
                UserData.Instance.user.SetStage(new UserData.UserStage());
            }
        }
    }

    public class EnemyDropCoin
    {
        public JArray SELECTED_LIST;
        public JObject TOTAL;

        public void SetData()
        {
            if(TOTAL != null)
            {
                UserData.Instance.user.SetGoods(NetworkManager.Instance.CopyData<UserData.UserGoods>(TOTAL.Value<JObject>("MONEY"), UserData.Instance.user.Goods));

                JArray itemArray = TOTAL.Value<JArray>("ITEM");

                if (itemArray != null)
                {
                    if (itemArray.Count > 0)
                    {
                        foreach (JObject item in itemArray)
                        {
                            UserData.Instance.user.Item.SetItem(item);
                        }
                    }
                }
            }
        }
    }

    public class StatUp
    {
        public JArray SELECTED_LIST;
        public JObject TOTAL;
        public JObject STAT;

        public void SetData()
        {
            if (TOTAL != null)
            {
                UserData.Instance.user.SetGoods(NetworkManager.Instance.CopyData<UserData.UserGoods>(TOTAL.Value<JObject>("MONEY"), UserData.Instance.user.Goods));
            }

            if (STAT != null) 
            {
                UserData.Instance.user.SetStat(NetworkManager.Instance.CopyData<UserData.UserStat>(STAT, UserData.Instance.user.Stat));
            }
        }
    }

    public class ADGacha
    {
        public JObject AD;
        public JArray SELECTED_LIST;
        public JObject TOTAL;

        public void SetData()
        {
            if (TOTAL != null)
            {
                JArray itemArray = TOTAL.Value<JArray>("ITEM");

                if (itemArray != null && itemArray.Count > 0)
                {
                    foreach (JObject item in itemArray)
                    {
                        UserData.Instance.user.Item.SetItem(item);
                    }
                }
            }

            if (SELECTED_LIST != null)
            {
                List<BaseItemData> rewardItemList = new List<BaseItemData>();

                if (SELECTED_LIST.Count > 0)
                {
                    foreach (JObject rewardData in SELECTED_LIST)
                    {
                        if (rewardData.Value<int>("COUNT") > 0)
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

                    if (rewardItemList.Count > 0)
                    {
                        ShowRewardItemsPopup showRewardItemsPopup = UIManager.Instance.GetPopup(BasePopup.EPopupType.ShowRewardItems) as ShowRewardItemsPopup;
                        showRewardItemsPopup.SetTitle("획득 아이템");
                        showRewardItemsPopup.SetDesc("여기 누르면 창 꺼짐!");
                        showRewardItemsPopup.SetRewardItemDataList(rewardItemList);
                        showRewardItemsPopup.Active();
                    }
                }
            }
            if (TOTAL != null)
            {
                UserData.Instance.user.SetGoods(NetworkManager.Instance.CopyData<UserData.UserGoods>(TOTAL.Value<JObject>("MONEY"), UserData.Instance.user.Goods));
            }
        }
    }

    public class ConnectReward
    {
        public JObject CONNECT_REWARD;
        public JArray SELECTED_LIST;
        public JObject TOTAL;

        public void SetData()
        {
            if (CONNECT_REWARD != null)
            {
                //CONNECT_REWARD.Value<int>("CONNECT_TIME");
                //CONNECT_REWARD.Value<bool>("REWARDED");
            }

            if (SELECTED_LIST != null)
            {
                List<BaseItemData> rewardItemList = new List<BaseItemData>();

                if (SELECTED_LIST.Count > 0)
                {
                    foreach (JObject rewardData in SELECTED_LIST)
                    {
                        if (rewardData.Value<int>("COUNT") > 0)
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

                    if (rewardItemList.Count > 0)
                    {
                        ShowRewardItemsPopup showRewardItemsPopup = UIManager.Instance.GetPopup(BasePopup.EPopupType.ShowRewardItems) as ShowRewardItemsPopup;
                        showRewardItemsPopup.SetTitle("획득 아이템");
                        showRewardItemsPopup.SetDesc("여기 누르면 창 꺼짐!");
                        showRewardItemsPopup.SetRewardItemDataList(rewardItemList);
                        showRewardItemsPopup.Active();
                    }
                }
            }
            if (TOTAL != null)
            {
                UserData.Instance.user.SetGoods(NetworkManager.Instance.CopyData<UserData.UserGoods>(TOTAL.Value<JObject>("MONEY"), UserData.Instance.user.Goods));

                JArray itemArray = TOTAL.Value<JArray>("ITEM");

                if (itemArray != null)
                {
                    if (itemArray.Count > 0)
                    {
                        foreach (JObject item in itemArray)
                        {
                            UserData.Instance.user.Item.SetItem(item);
                        }
                    }
                }
            }
        }
    }

    public class Shop
    {
        public JArray SELECTED_LIST;
        public JObject TOTAL;
        public JArray CHARACTER;

        public void SetData()
        {
            if (SELECTED_LIST != null)
            {
                List<BaseItemData> rewardItemList = new List<BaseItemData>();

                if (SELECTED_LIST.Count > 0)
                {
                    foreach (JObject rewardData in SELECTED_LIST)
                    {
                        if (rewardData.Value<int>("COUNT") > 0)
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

                    if (rewardItemList.Count > 0)
                    {
                        ShowRewardItemsPopup showRewardItemsPopup = UIManager.Instance.GetPopup(BasePopup.EPopupType.ShowRewardItems) as ShowRewardItemsPopup;
                        showRewardItemsPopup.SetTitle("획득 아이템");
                        showRewardItemsPopup.SetDesc("여기 누르면 창 꺼짐!");
                        showRewardItemsPopup.SetRewardItemDataList(rewardItemList);
                        showRewardItemsPopup.Active();
                    }
                }
            }

            if (TOTAL != null)
            {
                UserData.Instance.user.SetGoods(NetworkManager.Instance.CopyData<UserData.UserGoods>(TOTAL.Value<JObject>("MONEY"), UserData.Instance.user.Goods));

                JArray itemArray = TOTAL.Value<JArray>("ITEM");

                if (itemArray != null)
                {
                    if (itemArray.Count > 0)
                    {
                        foreach (JObject item in itemArray)
                        {
                            UserData.Instance.user.Item.SetItem(item);
                        }
                    }
                }
            }

            if (CHARACTER != null)
            {
                if (CHARACTER.Count > 0)
                {
                    foreach (JObject character in CHARACTER)
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
                                    return;
                                }

                                characterData.SetDataCharacter(dataCharacter);
                                characterData.SetUpgradeCnt(character.Value<int>("ENCHANTED_COUNT"));
                                characterData.SetPositionIdx(character.Value<int>("POSITION"));
                                //characterData.SetHeroEquipment()

                                UserData.Instance.user.Character.SetAddCharacterData(characterData);
                            }
                        }
                    }
                    if (Hero.Instance != null)
                    {
                        Hero.Instance.InitHeroList();
                        Hero.Instance.SetHeroList();
                    }
                }                
            }
            
        }        
    }

    public class PacketHero
    {
        public JArray SELECTED_LIST;
        public JObject TOTAL;
        public JArray CHARACTER;

        public void SetData(Action<CharacterData> successCb = null)
        {
            if (SELECTED_LIST != null)
            {
                List<BaseItemData> rewardItemList = new List<BaseItemData>();

                if (SELECTED_LIST.Count > 0)
                {
                    foreach (JObject rewardData in SELECTED_LIST)
                    {
                        if (rewardData.Value<int>("COUNT") > 0)
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

                    if (rewardItemList.Count > 0)
                    {
                        ShowRewardItemsPopup showRewardItemsPopup = UIManager.Instance.GetPopup(BasePopup.EPopupType.ShowRewardItems) as ShowRewardItemsPopup;
                        showRewardItemsPopup.SetTitle("획득 아이템");
                        showRewardItemsPopup.SetDesc("여기 누르면 창 꺼짐!");
                        showRewardItemsPopup.SetRewardItemDataList(rewardItemList);
                        showRewardItemsPopup.Active();
                    }
                }
            }

            if (TOTAL != null)
            {
                UserData.Instance.user.SetGoods(NetworkManager.Instance.CopyData<UserData.UserGoods>(TOTAL.Value<JObject>("MONEY"), UserData.Instance.user.Goods));

                JArray itemArray = TOTAL.Value<JArray>("ITEM");

                if (itemArray != null)
                {
                    if (itemArray.Count > 0)
                    {
                        foreach (JObject item in itemArray)
                        {
                            UserData.Instance.user.Item.SetItem(item);
                        }
                    }
                }
            }

            if (CHARACTER != null)
            {
                if (CHARACTER.Count > 0)
                {
                    foreach (JObject character in CHARACTER)
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
                                    return;
                                }

                                characterData.SetDataCharacter(dataCharacter);
                                characterData.SetUpgradeCnt(character.Value<int>("ENCHANTED_COUNT"));
                                characterData.SetPositionIdx(character.Value<int>("POSITION"));

                                UserData.Instance.user.Character.SetAddCharacterData(characterData);

                                if (successCb != null)
                                {
                                    successCb.Invoke(characterData);
                                }
                            }
                        }
                    }

                    if (Hero.Instance != null)
                    {
                        Hero.Instance.InitHeroList();
                        Hero.Instance.SetHeroList();
                    }
                }
            }
        }
    }
}
