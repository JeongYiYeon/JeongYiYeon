using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPacketEvent : Singleton<NetworkPacketEvent>
{
    private const string requestPath = "/event";

    private const string Attendance = requestPath + "/attendance";

    private const string ReciveGamePass = requestPath + "/gamePass";

    Dictionary<string, EventRequestParameter> requestForm = new Dictionary<string, EventRequestParameter>();

    private class EventRequestParameter
    {
        private int uid;
        private List<int> eventClearList = null;

        public int UID => uid;
        public List<int> EventClearList => eventClearList;

        public void SetUID(int uid)
        {
            this.uid = uid;
        }

        public void SetEventClearUID(int clearUID)
        {
            if(eventClearList == null)
            {
                eventClearList = new List<int>();
            }

            if (eventClearList.Contains(clearUID) == false)
            {
                eventClearList.Add(clearUID);
            }
        }
    }

    public async UniTask TaskAttendance()
    {
        await NetworkManager.Instance.SendRequest<JObject>(Attendance,
            _successCb: (jsonData) =>
            {
                if(jsonData != null)
                {
                    JObject dayAttendance = jsonData.Value<JObject>("Day");

                    if(dayAttendance != null)
                    {
                        UserData.Instance.user.SetDayAttendanceCount(dayAttendance.Value<int>("ATTENDANCE_DAY"));
                    }

                    JObject weeklyAttendance = jsonData.Value<JObject>("Weekly");

                    if (weeklyAttendance != null)
                    {
                        UserData.Instance.user.SetWeeklyAttendanceCount(weeklyAttendance.Value<int>("ATTENDANCE_DAY"));
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
                }
            });
    }
   
    public async UniTask TaskReciveGamePass(int reciveGamePassUID)
    {
        requestForm.Clear();

        EventRequestParameter requestParameter = new EventRequestParameter();
        requestParameter.SetUID(reciveGamePassUID);

        requestForm.Add("UID", requestParameter);

        await NetworkManager.Instance.SendRequest<JObject>(ReciveGamePass,
           NetworkManager.Instance.SerializeObject(requestForm),
           _successCb: (jsonData) => 
           {
               JObject gamePassData = jsonData.Value<JObject>("GAME_PASS");

               if(gamePassData != null) 
               {
                   GamePassPopup gamePassPopup = UIManager.Instance.GetPopup(BasePopup.EPopupType.GamePass) as GamePassPopup;

                   gamePassPopup.SetGamePassSeason(gamePassData.Value<int>("SEASON"));
                   gamePassPopup.SetGamePassCurrentExp(gamePassData.Value<int>("POINT"));
                   gamePassPopup.SetGamePassLevel(gamePassData.Value<int>("Free"));
               }


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

               if (rewardItemList.Count > 0)
               {
                   ShowRewardItemsPopup showRewardItemsPopup = UIManager.Instance.GetPopup(BasePopup.EPopupType.ShowRewardItems) as ShowRewardItemsPopup;
                   showRewardItemsPopup.SetTitle("획득 아이템");
                   showRewardItemsPopup.SetDesc("여기 누르면 창 꺼짐!");
                   showRewardItemsPopup.SetRewardItemDataList(rewardItemList);
                   showRewardItemsPopup.Active();
               }
           });
    }


}
