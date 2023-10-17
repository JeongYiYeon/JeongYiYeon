using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using static NetworkManager;

public class NetworkPacketUser : Singleton<NetworkPacketUser>
{
    private const string requestPath = "/user";

    private const string Login = requestPath + "/login";
    private const string SiginUp = requestPath + "/signup";
    public const string Session = requestPath + "/session";
    private const string ResetUser = requestPath + "/reset";

    public async UniTask TaskLogin()
    {
        await NetworkManager.Instance.SendLogin<JObject>(Login,
            udid: UserData.Instance.user.UDID,
            uid: UserData.Instance.user.UserID,
            _successCb: async (jsonData) =>
            {
                UserData.Instance.user.SetSessionToken(jsonData.Value<string>("SESSION_TOKEN"));
                UserData.Instance.user.SetReSessionToken(jsonData.Value<string>("RE_SESSION_TOKEN"));

                Dictionary<string, double> tmpConfigDic = new Dictionary<string, double>();

                foreach(DataConfig data in DataManager.Instance.DataHelper.Config)
                {
                    if (tmpConfigDic.ContainsKey(data.ENUM_ID) == false)
                    {
                        tmpConfigDic.Add(data.ENUM_ID, data.INT_VALUE);
                    }
                }

                UserData.Instance.user.SetConfig(JsonConvert.DeserializeObject<ConfigManager>(JsonConvert.SerializeObject(tmpConfigDic)));

                IEnumerable<KeyValuePair<string,double>> tmpStatusGoldDic = tmpConfigDic.Where(x => x.Key.Contains("_GOLD_"));
                Dictionary<string, double> statusGoldDic = new Dictionary<string, double>();

                if (tmpStatusGoldDic != null)
                {
                    foreach (KeyValuePair<string, double> item in tmpStatusGoldDic)
                    {
                        statusGoldDic.Add(item.Key, item.Value);
                    }

                    UserData.Instance.user.Config.SetStatusGoldDic(statusGoldDic);
                }

                IEnumerable<KeyValuePair<string, double>> tmpStatusValueDic = tmpConfigDic.Where(x => x.Key.Contains("_VALUE_"));
                Dictionary<string, double> statusValueDic = new Dictionary<string, double>();

                if (tmpStatusValueDic != null)
                {
                    foreach (KeyValuePair<string, double> item in tmpStatusValueDic)
                    {
                        statusValueDic.Add(item.Key, item.Value);
                    }

                    UserData.Instance.user.Config.SetStatusValueDic(statusValueDic);
                }

                await NetworkPacketEvent.Instance.TaskAttendance();

                NetworkManager.Instance.InitWebSocket();

                await UniTask.WaitUntil(() => NetworkManager.Instance.IsConnectSocket);

                NetworkManager.Instance.SendProtocol(ESOCKET_PROTOCOL.CONNECT);

                await UniTask.WaitUntil(() => NetworkManager.Instance.IsConnect);

                NetworkManager.Instance.SendPing().Forget();

                Debug.LogError("Success");

                LoadingManager.Instance.LoadScene(LoadingManager.EScene.LOBBY);
            });
    }

    public async UniTask TaskSiginUp()
    {
        UserData.User user = await NetworkManager.Instance.SendLogin<UserData.User>(SiginUp,
            udid: UserData.Instance.user.UDID,
            uid: string.Empty,
            originObject: UserData.Instance.user,
            _successCb: async (userData) =>
            {
                //최초 가입 성공 시 로그인
                await TaskLogin();
            });

        if (user != null)
        {
            UserData.Instance.SetUser(user);
        }
    }

    public async UniTask TaskResetUser()
    {
        await NetworkManager.Instance.SendRequest<JObject>(ResetUser,
            _successCb: (data) =>
            {
                UserData.Instance.SetUser(null);
                NetworkManager.Instance.ResetSocket();
                LoadingManager.Instance.LoadScene(LoadingManager.EScene.INTRO);
            });
    }
}
