using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPacketItem : Singleton<NetworkPacketItem>
{
    private const string requestPath = "/item";

    private const string EquipItem = requestPath + "/equip";
    private const string UpgradeItem = requestPath + "/enchant";
    private const string ChargeEnergy = requestPath + "/charge";

    private class ItemRequestParameter
    {
        private int uid;
        private List<string> itemUIDList = null;

        public int UID => uid;
        public List<string> LIST => itemUIDList;

        public void SetUID(int uid)
        {
            this.uid = uid;
        }

        public void SetItemUIDList(string uid)
        {
            if (itemUIDList == null)
            {
                itemUIDList = new List<string>();
            }

            if (itemUIDList.Contains(uid) == false)
            {
                itemUIDList.Add(uid);
            }
        }
    }

    public async UniTask TaskEquipItem(int characterUID, string itemUID, Action successCb = null)
    {
        ItemRequestParameter parameter = new ItemRequestParameter();
        parameter.SetUID(characterUID);
        parameter.SetItemUIDList(itemUID);        

        await NetworkManager.Instance.SendRequest<JObject>(EquipItem,
            NetworkManager.Instance.SerializeObject(parameter), 
            _successCb: (jsonData) => 
            {
                if(jsonData != null)
                {
                    JArray itemArray = jsonData.Value<JArray>("ITEM");

                    if (itemArray != null && itemArray.Count > 0)
                    {
                        foreach (JObject item in itemArray)
                        {
                            UserData.Instance.user.Item.SetItem(item);
                        }
                    }

                    if(successCb != null)
                    {
                        successCb.Invoke();
                    }
                }
            });
    }
    public async UniTask TaskUpgradeItem(string itemUID, Action successCb = null)
    {
        Dictionary<string, string> parameter = new Dictionary<string, string>();
        parameter.Add("UID", itemUID);

        await NetworkManager.Instance.SendRequest<JObject>(UpgradeItem,
           NetworkManager.Instance.SerializeObject(parameter),
           _successCb: (jsonData) =>
           {
               if (jsonData != null)
               {
                   JObject getMoney = jsonData.Value<JObject>("MONEY");

                   if (getMoney != null)
                   {
                       UserData.Instance.user.SetGoods(
                           NetworkManager.Instance.CopyData<UserData.UserGoods>(getMoney, UserData.Instance.user.Goods));
                   }

                   JArray itemArray = jsonData.Value<JArray>("ITEM");

                   if (itemArray != null && itemArray.Count > 0)
                   {
                       foreach (JObject item in itemArray)
                       {
                           UserData.Instance.user.Item.SetItem(item);
                       }
                   }

                   if (successCb != null)
                   {
                       successCb.Invoke();
                   }
               }
           });
    }

    public async UniTask TaskChargeEnergy(Action successCb = null)
    {
        await NetworkManager.Instance.SendRequest<JObject>(ChargeEnergy,
            _successCb: (jsonData) =>
            {
                if (jsonData != null)
                {
                    JObject goodsInfo = jsonData.Value<JObject>("TOTAL");

                    if (goodsInfo != null)
                    {
                        JObject getMoney = goodsInfo.Value<JObject>("MONEY");

                        if (getMoney != null)
                        {
                            UserData.Instance.user.SetGoods(
                                NetworkManager.Instance.CopyData<UserData.UserGoods>(getMoney, UserData.Instance.user.Goods));
                        }

                        JObject getTime = goodsInfo.Value<JObject>("ENERGY_CHARGY_TIME");
                        if (getTime != null) 
                        {
                            UserData.Instance.user.SetGoods(
                                NetworkManager.Instance.CopyData<UserData.UserGoods>(getTime, UserData.Instance.user.Goods));
                        }


                        if (successCb != null)
                        {
                            successCb.Invoke();
                        }
                    }
                }
            });
    }
}
