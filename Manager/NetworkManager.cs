using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using Cysharp.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using socket.io;
using static NetworkManager;
using System.Threading;
using System.Linq;
using System.Xml.Linq;

public class NetworkManager : MonoSingleton<NetworkManager>
{

    #region API
    private const string SERVER_URL = "http://3.37.117.15:3000";
    private const int TIMEOUT = 15;
    private const int RETRY = 3;

    private int retryCnt = 0;

    public bool IsAsyncNetwork = false;                 //클릭 방지용으로 쓸꺼
    TimeoutController timeoutController = new TimeoutController();

    private bool isRefreshSession = false;

    private Coroutine _sessionChecker = null;
    private DateTime _refreshSessionTime = DateTime.MinValue;
    private DateTime _currentSessionTime = DateTime.MinValue;

    public class Response<T>
    {
        public int status;
        public T data;
        public string message;

        public Response()
        {
        }
    }
    #endregion

    private void Awake()
    {
        _sessionChecker = StartCoroutine(IESessionChecker());
    }

    public int GetTimeOut()
    {
        return TIMEOUT;
    }

    public bool IsAuthorization()
    {
        return !string.IsNullOrEmpty(UserData.Instance.user.UserID);
    }

    public void SetSessionTime(DateTime time)
    {
        if(time == DateTime.MinValue)
        {
            time = DateTime.UtcNow;
        }

        _currentSessionTime = time;
        _refreshSessionTime = _currentSessionTime.AddMinutes(30);
    }

    private IEnumerator IESessionChecker()
    {
        yield return new WaitUntil(() => _refreshSessionTime != DateTime.MinValue && _currentSessionTime != DateTime.MinValue);

        while ((_refreshSessionTime - _currentSessionTime).TotalSeconds > 0)
        {
            yield return new WaitForSeconds(1f);

            _currentSessionTime = DateTime.UtcNow;
        }

        if (IsAuthorization() == true && isRefreshSession == false)
        {
            LoadingManager.Instance.ActiveLoading();

            isRefreshSession = true;

            RequestSessionRefresh().Forget();
        }

        yield return null;
    }

    private async UniTaskVoid RequestSessionRefresh()
    {
        await SendLogin<JObject>(NetworkPacketUser.Session,
           udid: UserData.Instance.user.UDID,
           uid: UserData.Instance.user.UserID,
           _successCb: (_result) =>
           {
               _currentSessionTime = DateTime.UtcNow;
               _refreshSessionTime = _currentSessionTime.AddMinutes(30);

               StopCoroutine(_sessionChecker);

               _sessionChecker = null;

               _sessionChecker = StartCoroutine(IESessionChecker());

               isRefreshSession = false;

               LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
           });
    }

    public async UniTask<T> SendLogin<T>(string url, string udid, string uid, 
        T originObject = default(T), Action<T> _successCb = null)
    {
        LoadingManager.Instance.ActiveLoading();

        Uri _uri = new Uri(SERVER_URL + url);
        using (UnityWebRequest _request = UnityWebRequest.Post(_uri, string.Empty))
        {
            _request.SetRequestHeader("Content-Type", "application/json");
            _request.SetRequestHeader("Accept", "application/json");

            if(!string.IsNullOrEmpty(udid)) 
            {
                _request.SetRequestHeader("udid", UserData.Instance.user.UDID);
            }
            if (!string.IsNullOrEmpty(uid))
            {
                _request.SetRequestHeader("uid", UserData.Instance.user.UserID);
            }

            if (isRefreshSession == true)
            {
                _request.SetRequestHeader("session", UserData.Instance.user.SessionToken);
            }

            try
            {
                await _request.SendWebRequest().WithCancellation(timeoutController.Timeout(TimeSpan.FromSeconds(TIMEOUT)));
                timeoutController.Reset();

                if (_request.result == UnityWebRequest.Result.Success)
                {
                    byte[] _dataBytes = _request.downloadHandler.data;

                    string _data = Encoding.UTF8.GetString(_dataBytes);

                    Debug.Log(string.Format("### Recv data : uri({0}) len({1}) data({2})", _uri, _data.Length, _data));

                    Response<T> _tmpData = DeserializeReciveData<Response<T>>(_data);

                    retryCnt = 0;

                    if (_tmpData.status == (int)ErrorCode.Succeeded)
                    {
                        if (_tmpData.data != null)
                        {
                            if (Equals(originObject, default(T)) == false)
                            {
                                if (originObject != null)
                                {
                                    _tmpData = CopyReciveData(_tmpData, originObject);
                                }
                            }

                            if (_successCb != null)
                            {
                                _successCb(_tmpData.data);
                            }

                            return _tmpData.data;
                        }
                        else
                        {
                            return default;
                        }
                    }

                    else
                    {
                        Debug.LogError(string.Format("Code : {0} Message : {1}", _tmpData.status, _tmpData.message));

                        if(isLoginError(_tmpData.status) == true)
                        {
                            Debug.LogError($"로그인 에러 : {_tmpData.status}");
                        }

                        // SiginUp => 이미 있는 ID -> UID 넣어주고 로그인 실행
                        if (_tmpData.status == (int)ErrorCode.AlreadyCreated)
                        {
                            if (_tmpData.data != null)
                            {
                                _tmpData = CopyReciveData(_tmpData, originObject);

                                UserData.Instance.SetUser(_tmpData.data as UserData.User);

                                NetworkPacketUser.Instance.TaskLogin().Forget();

                                return default;
                            }
                        }

                        IsAsyncNetwork = false;

                        LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
                    }
                }
                else
                {
                    Debug.LogError(JsonConvert.DeserializeObject(_request.downloadHandler.text));

                    LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
                }
            }

            catch (OperationCanceledException ex)
            {
                if (timeoutController.IsTimeout())
                {
                    Debug.LogError(url + " :: " + "TimeOut");
                }
                else
                {
                    Debug.LogError(url + " :: " + ex.Message);
                }

                LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
            }

            catch (Exception e)
            {
                Debug.LogError(url + " :: " + e.Message);
                
                LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
            }

            if (RETRY > retryCnt)
            {
                LoadingManager.Instance.Reset(LoadingManager.EState.Loading);

                ShowRetryMessageBox(UniTask.Action(async () =>
                {
                    await SendLogin(url, udid, uid, originObject, _successCb);
                }));
            }
            else
            {
                retryCnt = 0;

                ShowGoTitleMessageBox();
            }


            IsAsyncNetwork = false;

            LoadingManager.Instance.Reset(LoadingManager.EState.Loading);

            return default;
        }
    }

    private bool isLoginError(int errorCode)
    {
        switch (errorCode)
        {
            case (int)ErrorCode.Unauthorized:
                return true;

            case (int)ErrorCode.InvalidToken:// 잘못된 토큰
                return true;
            case (int)ErrorCode.InvalidUid:// 잘못된 uid
                return true;
            case (int)ErrorCode.SuspendedUser:// 정지된 유저
                return true;
            case (int)ErrorCode.WithdrawnUser:// 탈퇴한 유저
                return true;
            case (int)ErrorCode.InvalidParameter:// 잘못된 파라미터
                return true;
            case (int)ErrorCode.InvalidSocialUser:// 잘못된 소셜 유저
                return true;
            case (int)ErrorCode.NotFoundUser:// 유저를 못찾았다
                return true;
            case (int)ErrorCode.InvalidUserDeviceId:// 잘못된 유저 디바이스 아이디이다
                return true;
        }

        return false;
    }

    /// <summary>
    /// 서버 Post 양식
    /// </summary>
    /// <typeparam name="T">클래스, Jobject</typeparam>
    /// <param name="url">주소</param>
    /// <param name="form">파라미터</param>
    /// <param name="originObject">기존 데이터 업데이트할때 필요</param>
    /// <param name="_successCb">성공 콜백 필요하면 넣으세요(선언한 타입 따라감)</param>
    /// <param name="_failCb">실패 콜백 필요하면 넣으세요</param>
    /// <returns></returns>
    /// 
    public async UniTask<T> SendRequest<T>(string url, string form = "",
        T originObject = default(T), Action<T> _successCb = null, Action<int> _failCb = null, bool retry = true)
    {
        LoadingManager.Instance.ActiveLoading();

        Uri _uri = new Uri(SERVER_URL + url);

        using (UnityWebRequest _request = UnityWebRequest.Post(_uri, form))
        {
            Debug.Log(string.Format("### Send data : uri({0}) parameter ({1}))", url, form));

            if (!string.IsNullOrEmpty(form))
            {
                _request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(form));
                _request.uploadHandler.contentType = "application/json";
            }

            _request.SetRequestHeader("Content-Type", "application/json");
            _request.SetRequestHeader("Accept", "application/json");
            _request.SetRequestHeader("udid", UserData.Instance.user.UDID);
            _request.SetRequestHeader("uid", UserData.Instance.user.UserID);
            _request.SetRequestHeader("session", UserData.Instance.user.SessionToken);

            try
            {
                await _request.SendWebRequest().WithCancellation(timeoutController.Timeout(TimeSpan.FromSeconds(TIMEOUT)));
                timeoutController.Reset();

                if (_request.result == UnityWebRequest.Result.Success)
                {
                    byte[] _dataBytes = _request.downloadHandler.data;

                    string _data = Encoding.UTF8.GetString(_dataBytes);

                    Debug.Log(string.Format("### Recv data : uri({0}) len({1}) data({2})", _uri, _data.Length, _data));

                    Response<T> _tmpData = DeserializeReciveData<Response<T>>(_data);

                    retryCnt = 0;

                    if (_tmpData.status == (int)ErrorCode.Succeeded)
                    {                        
                        if (_tmpData.data != null)
                        {
                            if (Equals(originObject, default(T)) == false)
                            {
                                if (originObject != null)
                                {
                                    _tmpData = CopyReciveData(_tmpData, originObject);
                                }
                            }

                            if (_successCb != null)
                            {
                                _successCb(_tmpData.data);
                            }

                            return _tmpData.data;
                        }
                        else
                        {
                            if (_successCb != null)
                            {
                                _successCb(default);
                            }

                            return default;
                        }
                    }
                    
                    else
                    {
                        Debug.LogError(string.Format("Code : {0} Message : {1}", _tmpData.status, _tmpData.message));

                        AlramPopup alramPopup = UIManager.Instance.GetPopup(BasePopup.EPopupType.Alram) as AlramPopup;
                        alramPopup.SetButtonType(BasePopup.EButtonType.One);
                        alramPopup.SetTitle("오류");
                        alramPopup.SetDesc(((ErrorCode)_tmpData.status).ToString());
                        alramPopup.SetConfirmBtLabel("확인");
                        alramPopup.SetConfirmCallBack(() => { alramPopup.DeActive(); });
                        alramPopup.Active();

                        if (_failCb != null)
                        {
                            _failCb.Invoke(_tmpData.status);
                        }

                        IsAsyncNetwork = false;

                        LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
                        return default;
                    }
                }
                else
                {
                    Debug.LogError(JsonConvert.DeserializeObject(_request.downloadHandler.text));

                    LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
                }
            }

            catch (OperationCanceledException ex)
            {
                if (timeoutController.IsTimeout())
                {
                    Debug.LogError(url + " :: " + "TimeOut");
                }
                else
                {
                    Debug.LogError(url + " :: " + ex.Message);
                }

                LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
            }

            catch (Exception e)
            {
                Debug.LogError(url + " :: " + e.Message);

                LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
            }

            if (retry == true)
            {
                if (RETRY > retryCnt)
                {
                    LoadingManager.Instance.Reset(LoadingManager.EState.Loading);

                    ShowRetryMessageBox(UniTask.Action(async () =>
                    {
                        await SendRequest(url, form, originObject);
                    }));
                }
                else
                {
                    retryCnt = 0;

                    ShowGoTitleMessageBox();
                }
            }

            IsAsyncNetwork = false;

            return default;
        }
    }

    private T DeserializeReciveData<T>(string reciveData)
    {
        if (!string.IsNullOrEmpty(reciveData))
        {
            return JsonConvert.DeserializeObject<T>(reciveData,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        }

        return default(T);
    }
    private Response<T> CopyReciveData<T>(Response<T> _data, T originObject)
    {
        JsonSerializer serializer = new JsonSerializer();

        StringReader _reciveData = new StringReader(
            JsonConvert.SerializeObject(_data.data,
                Formatting.Indented,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

        if (_reciveData != null)
        {
            serializer.Populate(_reciveData, originObject);
            _data.data = originObject;
        }

        return _data;
    }

    public T CopyData<T>(JObject _data, T originObject)
    {
        JObject originJobject = JObject.Parse(SerializeObject(originObject));

        originJobject.Merge(_data, new JsonMergeSettings 
        { 
            MergeArrayHandling = MergeArrayHandling.Union,
            MergeNullValueHandling = MergeNullValueHandling.Ignore
        });

        originObject = DeserializeReciveData<T>(originJobject.ToString());

        return originObject;
    }

    public string SerializeObject(object _data)
    {
        if (_data == null)
        {
            return string.Empty;
        }

        if (isCorrectDataType(_data))
        {
            if (_data != null)
            {
                return JsonConvert.SerializeObject(_data,
                    Formatting.Indented,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
        }

        return string.Empty;
    }

    private bool isCorrectDataType<T>(T _data) where T : class
    {
        if (_data.GetType() == typeof(string))
        {
            return false;
        }

        else
        {
            if (_data.GetType() == typeof(JObject) ||
                _data.GetType().IsGenericType &&
                _data.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>) ||
                _data.GetType().IsClass)
            {
                return true;
            }
        }

        return false;
    }

    public void ShowRetryMessageBox(Action _retryAction = null)
    {
        if (_retryAction != null)
        {
            AlramPopup alramPopup = UIManager.Instance.GetPopup(BasePopup.EPopupType.Alram) as AlramPopup;
            alramPopup.SetButtonType(BasePopup.EButtonType.Two);
            alramPopup.SetTitle("오류");
            alramPopup.SetDesc("통신 리트 ?");
            alramPopup.SetConfirmBtLabel("확인");
            alramPopup.SetCancelBtLabel("취소");
            alramPopup.SetConfirmCallBack(() => 
            {
                _retryAction.Invoke();
                alramPopup.DeActive();
            });
            alramPopup.SetCancelCallback(() => { Application.Quit(); });
            alramPopup.Active();

        }

        else
        {
            ShowGoTitleMessageBox();
        }
    }

    public void ShowGoTitleMessageBox()
    {

        //MessageBoxHelper.ShowMessageBoxKey_OneButton("ALERT", "MSG_SERVER_FAULT", "GO_TITLE",
        //                    (int _idx, string _data) =>
        //                    {
        //                        _AuthorizationKey = "";
        //                        SceneManager.Instance.ReturnToLogo();
        //                    });
    }

    public void ShowAppQuitMessageBox()
    {
        AlramPopup alramPopup = UIManager.Instance.GetPopup(BasePopup.EPopupType.Alram) as AlramPopup;
        alramPopup.SetButtonType(BasePopup.EButtonType.Two);
        alramPopup.SetTitle("앱 종료");
        alramPopup.SetDesc("앱 종료?");
        alramPopup.SetConfirmBtLabel("종료");
        alramPopup.SetConfirmBtLabel("취소");
        alramPopup.SetConfirmCallBack(() => { Application.Quit(); });
        alramPopup.SetCancelCallback(() => { alramPopup.DeActive(); });
        alramPopup.Active();

    }



    #region 웹소켓
    public enum ESOCKET_PROTOCOL
    {
        PING = 10001,
        CONNECT = 10002,
        RECONNECT = 10003,

        STAGE_CLEAR = 11002,
        STAGE_BOSS = 11003,

        REWARD = 12001,
        STAT_UP = 12002,
        AD = 12003,

        ITEM_ENCHANT = 13001,
        ITEM_SELL = 13002,
        ITEM_EQUIP = 13003,

        CONNECT_REWARD_INFO = 14001,
        GET_CONNECT_REWARD = 14002,

        HERO_POSITION = 15001,
        HERO_CHANGE = 15002,
        HERO_ENCHANT = 15003,

        SHOP_BUY = 16004,
        SHOP_HERO = 16005,
    }

    private const string WEBSOCKET_URL = "http://3.37.117.15:3010";
    private const string MESSAGE = "message";

    private const int RECONNECT_COUNT = 5;
    private Socket _socket = null;
    private bool isConnectSocket = false;                   // 소켓 연결 체크
    private bool isConnect = false;                         // 유저 정보 연결 체크

    public Socket socket => _socket;
    public bool IsConnectSocket => isConnectSocket;
    public bool IsConnect => isConnect;

    public class SocketResponse<T>
    {
        private int pid;
        private int status;
        public T data;

        public ESOCKET_PROTOCOL Protocol => (ESOCKET_PROTOCOL)pid;
        public ErrorCode Status => (ErrorCode)status;

        public SocketResponse()
        {
        }
    }

    public class SocketParameter<T>
    {
        private int _pid;
        private T _data;

        public int pid => _pid;
        public T data => _data;

        public SocketParameter()
        {
        }

        public void SetPid(ESOCKET_PROTOCOL pid)
        {
            _pid = (int)pid;
        }

        public void SetData(T data)
        {
            _data = data;
        }
    }

    public void InitWebSocket()
    {
        _socket = Socket.Connect($"{WEBSOCKET_URL}");

        _socket.On(SystemEvents.connect, () => 
        {
            Debug.LogError("소켓 접속 완");
            isConnectSocket = true;

            _socket.On(MESSAGE, (string data) =>
            {
                ReciveProtocol(data);                
            });
        });

        socket.On(SystemEvents.connectError, (Exception e) =>
        {
            Debug.LogError(e.ToString());
            ResetSocket();
        });
    }

    public void ResetSocket()
    {
        isConnectSocket = false;
        isConnect = false;
        _socket.Off(SystemEvents.connect);
        _socket.Off(SystemEvents.connectError);

        _socket = null;
    }

    public async UniTaskVoid SendPing()
    {
        CancellationTokenSource cancellToken = new CancellationTokenSource();

        while(_socket != null)
        {
            SendProtocol(ESOCKET_PROTOCOL.PING);
            await UniTask.Delay(7000, cancellationToken: cancellToken.Token);
        }
    }
   
    private void ReciveProtocol(string data)
    {

        ESOCKET_PROTOCOL protocol = (ESOCKET_PROTOCOL)JObject.Parse(data).Value<int>("pid");
        ErrorCode statusCode = (ErrorCode)JObject.Parse(data).Value<int>("status");

        Debug.LogError(protocol + "::" + statusCode + "::" + JObject.Parse(data).Value<JObject>("data"));

        if (statusCode == ErrorCode.Succeeded)
        {
            switch (protocol)
            {
                case ESOCKET_PROTOCOL.PING:
                    Debug.LogError(DeserializeReciveData<SocketResponse<NetworkSocketReciver.Ping>>(data).data.message);
                    break;

                case ESOCKET_PROTOCOL.CONNECT:
                case ESOCKET_PROTOCOL.RECONNECT:

                    SocketResponse<NetworkSocketReciver.Connect> connectData = 
                        DeserializeReciveData<SocketResponse<NetworkSocketReciver.Connect>>(data);
                    connectData.data.SetData();
                    isConnect = true;  
                    break;

                case ESOCKET_PROTOCOL.STAGE_CLEAR:
                    {
                        SocketResponse<NetworkSocketReciver.Stage> stageData =
                            DeserializeReciveData<SocketResponse<NetworkSocketReciver.Stage>>(data);
                        stageData.data.SetData();

                        GameManager.Instance.SetNextStage();
                    }

                    break;

                case ESOCKET_PROTOCOL.STAGE_BOSS:
                    {
                        SocketResponse<NetworkSocketReciver.Stage> stageData =
                          DeserializeReciveData<SocketResponse<NetworkSocketReciver.Stage>>(data);
                        stageData.data.SetData();
                    }
                    break;

                case ESOCKET_PROTOCOL.REWARD:

                    SocketResponse<NetworkSocketReciver.EnemyDropCoin> dropCoinData =
                        DeserializeReciveData<SocketResponse<NetworkSocketReciver.EnemyDropCoin>>(data);
                    dropCoinData.data.SetData();

                    LobbyManager.Instance.RefreshGoodsLabel();

                    break;

                case ESOCKET_PROTOCOL.STAT_UP:
                    SocketResponse<NetworkSocketReciver.StatUp> statUpData =
                        DeserializeReciveData<SocketResponse<NetworkSocketReciver.StatUp>>(data);

                    statUpData.data.SetData();

                    LobbyManager.Instance.RefreshGoodsLabel();

                    Messenger<BaseCharacter>.Broadcast(ConstHelper.MessengerString.MSG_STATUP_RESET,
                        GameManager.Instance.HeroCharacter,
                        MessengerMode.DONT_REQUIRE_LISTENER);
                    break;

                case ESOCKET_PROTOCOL.AD:

                    SocketResponse<NetworkSocketReciver.ADGacha> adGachaData =
                        DeserializeReciveData<SocketResponse<NetworkSocketReciver.ADGacha>>(data);

                    adGachaData.data.SetData();
                    break;

                case ESOCKET_PROTOCOL.ITEM_ENCHANT:
                    break;
                case ESOCKET_PROTOCOL.ITEM_SELL:
                    break;
                case ESOCKET_PROTOCOL.ITEM_EQUIP:
                    break;

                case ESOCKET_PROTOCOL.CONNECT_REWARD_INFO:
                case ESOCKET_PROTOCOL.GET_CONNECT_REWARD:
                    SocketResponse<NetworkSocketReciver.ConnectReward> connectRewardData =
                        DeserializeReciveData<SocketResponse<NetworkSocketReciver.ConnectReward>>(data);

                    connectRewardData.data.SetData();
                    break;
                case ESOCKET_PROTOCOL.HERO_POSITION:
                    {
                        SocketResponse<NetworkSocketReciver.PacketHero> heroData =
                                DeserializeReciveData<SocketResponse<NetworkSocketReciver.PacketHero>>(data);
                        heroData.data.SetData();

                        LobbyManager.Instance.RefreshHeroMenu(BaseCharacter.CHARACTER_TYPE.HERO);
                        GameManager.Instance.SetFollowCharacter();
                        GameManager.Instance.RefreshCharacters();
                        GameManager.Instance.SetSkillIcon();
                        LoadingManager.Instance.ActiveOneLineAlram("각 영웅들 배치가 완료됨");
                    }
                    break;
                case ESOCKET_PROTOCOL.HERO_CHANGE:
                    {
                        SocketResponse<NetworkSocketReciver.PacketHero> heroData =
                            DeserializeReciveData<SocketResponse<NetworkSocketReciver.PacketHero>>(data);

                        heroData.data.SetData();

                        LobbyManager.Instance.RefreshHeroMenu(BaseCharacter.CHARACTER_TYPE.HERO);
                        GameManager.Instance.RefreshCharacters();
                    }
                    break;
                case ESOCKET_PROTOCOL.HERO_ENCHANT:
                    {
                        SocketResponse<NetworkSocketReciver.PacketHero> heroData =
                          DeserializeReciveData<SocketResponse<NetworkSocketReciver.PacketHero>>(data);

                        HeroUpgradePopup popup = UIManager.Instance.GetPopup(BasePopup.EPopupType.HeroUpgrade) as HeroUpgradePopup;

                        if (popup != null)
                        {
                            heroData.data.SetData(popup.SetUpgradeHeroInfo);
                        }
                        else
                        {
                            heroData.data.SetData();
                        }

                        LobbyManager.Instance.RefreshGoodsLabel();
                        LobbyManager.Instance.RefreshHeroMenu(BaseCharacter.CHARACTER_TYPE.HERO);
                    }
                    break;
                    
                case ESOCKET_PROTOCOL.SHOP_BUY:
                    {
                        SocketResponse<NetworkSocketReciver.Shop> shopData =
                            DeserializeReciveData<SocketResponse<NetworkSocketReciver.Shop>>(data);

                        shopData.data.SetData();
                        LobbyManager.Instance.RefreshGoodsLabel();
                    }
                    break;

                case ESOCKET_PROTOCOL.SHOP_HERO:
                    {
                        SocketResponse<NetworkSocketReciver.Shop> shopData =
                            DeserializeReciveData<SocketResponse<NetworkSocketReciver.Shop>>(data);

                        shopData.data.SetData();
                        LobbyManager.Instance.RefreshGoodsLabel();
                        Hero.Instance.InitHeroList();
                        Hero.Instance.SetHeroList();
                        LobbyManager.Instance.RefreshHeroMenu(BaseCharacter.CHARACTER_TYPE.HERO);
                    }
                    break;
            }
        }
        else
        {
            Debug.LogError(statusCode.ToString());
        }

        if (protocol != ESOCKET_PROTOCOL.REWARD)
        {
            LoadingManager.Instance.Reset(LoadingManager.EState.NetworkSync);
        }
    }

    public void SendProtocol(ESOCKET_PROTOCOL protocol, params object[] values)
    {
        if (protocol != ESOCKET_PROTOCOL.REWARD)
        {
            LoadingManager.Instance.ActiveNetworkSync();
        }

        switch (protocol)
        {
            case ESOCKET_PROTOCOL.PING:

                NetworkSocketParameter.Ping ping = new NetworkSocketParameter.Ping();
                SocketParameter<NetworkSocketParameter.Ping> pingParam = new SocketParameter<NetworkSocketParameter.Ping>();

                pingParam.SetPid(protocol);
                pingParam.SetData(ping);

                socket.EmitJson(MESSAGE, SerializeObject(pingParam));                
                break;

            case ESOCKET_PROTOCOL.CONNECT:

                NetworkSocketParameter.Connect connect = new NetworkSocketParameter.Connect();
                connect.SetUID(UserData.Instance.user.UserID);
                connect.SetSession(UserData.Instance.user.SessionToken);

                SocketParameter<NetworkSocketParameter.Connect> connectParam = new SocketParameter<NetworkSocketParameter.Connect>();
                connectParam.SetPid(protocol);
                connectParam.SetData(connect);

                socket.EmitJson(MESSAGE, SerializeObject(connectParam));
                break;

            case ESOCKET_PROTOCOL.RECONNECT:
                NetworkSocketParameter.Connect reConnect = new NetworkSocketParameter.Connect();
                reConnect.SetUID(UserData.Instance.user.UserID);
                reConnect.SetSession(UserData.Instance.user.ReSessionToken);

                SocketParameter<NetworkSocketParameter.Connect> reConnectParam = new SocketParameter<NetworkSocketParameter.Connect>();
                reConnectParam.SetPid(protocol);
                reConnectParam.SetData(reConnect);

                socket.EmitJson(MESSAGE, SerializeObject(reConnectParam));
                break;

            case ESOCKET_PROTOCOL.STAGE_CLEAR:
                NetworkSocketParameter.Stage clearStage = new NetworkSocketParameter.Stage();
                clearStage.SetUID(values[0].ToString());

                SocketParameter<NetworkSocketParameter.Stage> clearStageParam = new SocketParameter<NetworkSocketParameter.Stage>();
                clearStageParam.SetPid(protocol);
                clearStageParam.SetData(clearStage);
                socket.EmitJson(MESSAGE, SerializeObject(clearStageParam));
                break;

            case ESOCKET_PROTOCOL.STAGE_BOSS:
                NetworkSocketParameter.Stage enterBossStage = new NetworkSocketParameter.Stage();
                enterBossStage.SetUID(values[0].ToString());

                SocketParameter<NetworkSocketParameter.Stage> enterBossStageParam = new SocketParameter<NetworkSocketParameter.Stage>();
                enterBossStageParam.SetPid(protocol);
                enterBossStageParam.SetData(enterBossStage);
                socket.EmitJson(MESSAGE, SerializeObject(enterBossStageParam));
                break;

            case ESOCKET_PROTOCOL.REWARD:
                NetworkSocketParameter.EnemyDropCoin dropCoin = new NetworkSocketParameter.EnemyDropCoin();
                dropCoin.SetUID(values[0].ToString());

                SocketParameter<NetworkSocketParameter.EnemyDropCoin> dropCoinParam = new SocketParameter<NetworkSocketParameter.EnemyDropCoin>();
                dropCoinParam.SetPid(protocol);
                dropCoinParam.SetData(dropCoin);
                socket.EmitJson(MESSAGE, SerializeObject(dropCoinParam));

                break;

            case ESOCKET_PROTOCOL.STAT_UP:
                NetworkSocketParameter.StatUp statUp = new NetworkSocketParameter.StatUp();
                statUp.SetType(values[0].ToString());

                SocketParameter<NetworkSocketParameter.StatUp> statUpParam = new SocketParameter<NetworkSocketParameter.StatUp>();
                statUpParam.SetPid(protocol);
                statUpParam.SetData(statUp);
                socket.EmitJson(MESSAGE, SerializeObject(statUpParam));
                break;

            case ESOCKET_PROTOCOL.AD:
                NetworkSocketParameter.ADGacha adGacha = new NetworkSocketParameter.ADGacha();
                adGacha.SetUID(values[0].ToString());

                SocketParameter<NetworkSocketParameter.ADGacha> adGachaParam = new SocketParameter<NetworkSocketParameter.ADGacha>();
                adGachaParam.SetPid(protocol);
                adGachaParam.SetData(adGacha);
                socket.EmitJson(MESSAGE, SerializeObject(adGachaParam));
                break;

            case ESOCKET_PROTOCOL.ITEM_ENCHANT:
                break;
            case ESOCKET_PROTOCOL.ITEM_SELL:
                break;
            case ESOCKET_PROTOCOL.ITEM_EQUIP:
                break;

            case ESOCKET_PROTOCOL.CONNECT_REWARD_INFO:
                {
                    SocketParameter<NetworkSocketParameter.ConnectReward> connectRewardParam = new SocketParameter<NetworkSocketParameter.ConnectReward>();
                    connectRewardParam.SetPid(protocol);
                    socket.EmitJson(MESSAGE, SerializeObject(connectRewardParam));
                }
                break;

            case ESOCKET_PROTOCOL.GET_CONNECT_REWARD:
                {
                    NetworkSocketParameter.ConnectReward connectReward = new NetworkSocketParameter.ConnectReward();
                    //나중에 광고 연결
                    connectReward.SetAdBonus(false);
                    SocketParameter<NetworkSocketParameter.ConnectReward> connectRewardParam = new SocketParameter<NetworkSocketParameter.ConnectReward>();
                    connectRewardParam.SetPid(protocol);
                    connectRewardParam.SetData(connectReward);
                    socket.EmitJson(MESSAGE, SerializeObject(connectRewardParam));
                }
                break;

            case ESOCKET_PROTOCOL.HERO_POSITION:
                {
                    var result = ((IEnumerable)values[0]).Cast<CharacterData>().ToList();

                    if (result.Count > 0)
                    {
                        Dictionary<string, List<NetworkSocketParameter.PacketHero>> tmpParam = new Dictionary<string, List<NetworkSocketParameter.PacketHero>>();

                        List<NetworkSocketParameter.PacketHero> tmpHeros = new List<NetworkSocketParameter.PacketHero>();

                        foreach (CharacterData item in result)
                        {
                            if (item != null)
                            {
                                NetworkSocketParameter.PacketHero hero = new NetworkSocketParameter.PacketHero();
                                hero.SetUID(item.DataCharacter.UID.ToString());
                                hero.SetPosition(item.TmpPositionIdx.ToString());

                                tmpHeros.Add(hero);
                            }
                        }

                        if (tmpHeros.Count > 0)
                        {
                            tmpParam.Add("LIST", tmpHeros);

                            SocketParameter<Dictionary<string, List<NetworkSocketParameter.PacketHero>>> packetHeroParam = new SocketParameter<Dictionary<string, List<NetworkSocketParameter.PacketHero>>>();
                            packetHeroParam.SetPid(protocol);
                            packetHeroParam.SetData(tmpParam);
                            socket.EmitJson(MESSAGE, SerializeObject(packetHeroParam));
                        }
                    }
                }
                break;
                
            case ESOCKET_PROTOCOL.HERO_CHANGE:
            case ESOCKET_PROTOCOL.HERO_ENCHANT:
                {
                    NetworkSocketParameter.PacketHero hero = new NetworkSocketParameter.PacketHero();
                    hero.SetUID(values[0].ToString());

                    SocketParameter<NetworkSocketParameter.PacketHero> packetHeroParam = new SocketParameter<NetworkSocketParameter.PacketHero>();
                    packetHeroParam.SetPid(protocol);
                    packetHeroParam.SetData(hero);
                    socket.EmitJson(MESSAGE, SerializeObject(packetHeroParam));
                }
                break;
                
            case ESOCKET_PROTOCOL.SHOP_BUY:
            case ESOCKET_PROTOCOL.SHOP_HERO:

                NetworkSocketParameter.Shop shop = new NetworkSocketParameter.Shop();
                shop.SetUID(values[0].ToString());

                SocketParameter<NetworkSocketParameter.Shop> shopParam = new SocketParameter<NetworkSocketParameter.Shop>();
                shopParam.SetPid(protocol);
                shopParam.SetData(shop);
                socket.EmitJson(MESSAGE, SerializeObject(shopParam));
                break;
        }
    }

    #endregion

}
