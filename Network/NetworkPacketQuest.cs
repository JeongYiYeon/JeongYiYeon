using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPacketQuest : Singleton<NetworkPacketQuest>
{
    private const string requestPath = "/quest";
    private const string QuestList = requestPath + "/list";
    private const string QuestClear = requestPath + "/clear";

    Dictionary<string, int[]> requestForm = new Dictionary<string, int[]>();

    private class QuestRequestParameter
    {
        private int uid;
        private int position;

        public int UID => uid;

        public void SetUID(int uid)
        {
            this.uid = uid;
        }
    }

    public async UniTask TaskQuestList(Action<List<QuestData>> successCb = null)
    {
        await NetworkManager.Instance.SendRequest<JObject>(QuestList,
            _successCb: (jsonData) =>
            {
                if(jsonData != null)
                {
                    JArray questArrayData = jsonData.Value<JArray>("QUEST");

                    if (questArrayData != null && questArrayData.Count > 0)
                    {
                        if (successCb != null)
                        {
                            successCb.Invoke(GetQuestList(questArrayData));
                        }
                    }
                }
            });
    }
    public async UniTask TaskQuestClear(int questUID, Action<List<QuestData>> successCb = null)
    {
        requestForm.Clear();

        //한번에 클리어 기능 생기면 바꾸면됨
        int[] requestParameter = new int[1];
        requestParameter[0] = questUID;

        requestForm.Add("LIST", requestParameter);

        await NetworkManager.Instance.SendRequest<JObject>(QuestClear,
            NetworkManager.Instance.SerializeObject(requestForm),
            _successCb: (jsonData) => 
            {
                if(jsonData != null)
                {
                    JArray questArrayData = jsonData.Value<JArray>("QUEST");

                    JArray getRewardList = jsonData.Value<JArray>("SELECTED_LIST");
                    List<BaseItemData> rewardItemList = new List<BaseItemData>();

                    if (getRewardList != null && getRewardList.Count > 0)
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
                        successCb.Invoke(GetQuestList(questArrayData));
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
            });
            

    }

    private List<QuestData> GetQuestList(JArray questArrayData)
    {
        List<QuestData> questDataList = new List<QuestData>();

        if (questArrayData != null && questArrayData.Count > 0)
        {
            for (int i = 0; i < questArrayData.Count; i++)
            {
                QuestData questData = JsonConvert.DeserializeObject<QuestData>(questArrayData[i].ToString());

                DataQuest dataQuest = DataManager.Instance.DataHelper.Quest.Find(x => x.UID == questData.QuestUID);

                if (dataQuest != null)
                {
                    if (dataQuest.EQuest == EnumQuest.EQuest.Day)
                    {
                        questData.SetQuestType(QuestData.EQuestType.Day);
                    }
                    else if (dataQuest.EQuest == EnumQuest.EQuest.Guide)
                    {
                        questData.SetQuestType(QuestData.EQuestType.Guide);
                    }

                    QuestData.EQuestCategoryType questCategoryType = QuestData.EQuestCategoryType.NONE;

                    if (Enum.TryParse<QuestData.EQuestCategoryType>(dataQuest.QUEST_TYPE, out questCategoryType) == true)
                    {
                        questData.SetQuestCategoryType(questCategoryType);
                    }

                    questData.SetClearCount(dataQuest.QUEST_COUNT);

                    if (dataQuest.QUEST_VALUE > 0)
                    {
                        questData.SetClearStageUID(dataQuest.QUEST_VALUE);
                    }

                    questData.SetTitle(DataManager.Instance.GetLocalization(dataQuest.QUEST_NAME, questData.CurrentCount, questData.ClearCount));
                    questData.SetDesc(DataManager.Instance.GetLocalization(dataQuest.QUEST_TEXT));


                    if (dataQuest.REWARD_ITEM > 0)
                    {
                        BaseItemData item = new BaseItemData();

                        DataItem dataItem = DataManager.Instance.DataHelper.Item.Find(x => x.UID == dataQuest.REWARD_ITEM);

                        if (dataItem == null)
                        {
                            continue;
                        }

                        item.SetDataItem(dataItem);
                        item.SetItemUID(dataItem.UID);
                        item.SetTitle(DataManager.Instance.GetLocalization(dataItem.ITEM_NAME));
                        item.SetDesc(DataManager.Instance.GetLocalization(dataItem.ITEM_DESC));
                        item.SetItemCategory(Enum.Parse<BaseItem.EItemCategory>(dataItem.ITEM_CATEGORY));

                        item.SetItemGrade(dataItem.ITEM_GRADE);
                        item.SetAtlasPath(dataItem.ITEM_ICON_ATLAS);
                        item.SetImgItemPath(dataItem.ITEM_ICON);
                        item.SetItemCount(dataQuest.REWARD_ITEM_COUNT);

                        questData.SetRewardItemData(item);
                    }

                    questDataList.Add(questData);
                }
            }
        }

        return questDataList;
    }
}
