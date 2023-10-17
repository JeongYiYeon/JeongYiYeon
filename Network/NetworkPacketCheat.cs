using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPacketCheat : Singleton<NetworkPacketCheat>
{
    private const string requestPath = "/test";

    private const string GetMaterial = requestPath + "/getAllEtcItem";
    private const string GetGoods = requestPath + "/getAllMoneyItem";

    public async UniTask TaskGetMaterial()
    {
        await NetworkManager.Instance.SendRequest<JObject>(GetMaterial,
            _successCb: (jsonData) =>
            {
                JArray itemArray = jsonData.Value<JArray>("ITEM");

                if (itemArray != null && itemArray.Count > 0)
                {
                    foreach (JObject item in itemArray)
                    {
                        UserData.Instance.user.Item.SetItem(item);
                    }
                }

                LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
            });
    }

    public async UniTask TaskGetGoods()
    {
        await NetworkManager.Instance.SendRequest<JObject>(GetGoods,
            _successCb: (jsonData) =>
            {
                UserData.Instance.user.SetGoods(JsonConvert.DeserializeObject<UserData.UserGoods>(jsonData.Value<JObject>("MONEY").ToString()));

                if(LobbyManager.Instance != null)
                {
                    LobbyManager.Instance.RefreshGoodsLabel();
                }

                LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
            });
    }
}
