using UnityEngine;
using System.Collections;
using Electrotank.Electroserver5.Api;
using Electrotank.Electroserver5.Core;
using log4net;
using System;
using System.Collections.Generic;

public delegate void DelegatePluginMessage(PluginMessageEvent e);
public delegate void DelegatePluginMessageOnProcess(string command, string action, EsObject PluginMessageParameters);
public delegate void DelegateJoinRoom(JoinRoomEvent e);
public delegate void DelegateLeaveRoom(LeaveRoomEvent e);
public delegate void DelegatePublicMessage(PublicMessageEvent e);
public delegate void DelegateLoginResponse(LoginResponse e);
public delegate void DelegateCreateOrJoinGame(CreateOrJoinGameResponse e);
public delegate void DelegateAddBuddies(AddBuddiesResponse e);
public delegate void DelegateRemoveBuddies(RemoveBuddiesResponse e);
public delegate void DelegateFriendChanged(User friend, bool isRemove);
public delegate void DelegateMessageChanged(Messages mess, bool isOutGoing);
public delegate void DelegateConfigClientChanged(IDictionary value);
public delegate void DelegateGenericErrorResponse(Electrotank.Electroserver5.Api.GenericErrorResponse message);

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Xử lý kết nối với ElectroServer
/// </summary>
public class CServer
{
    //ELECTRO SERVER : Khai báo các biến sẽ dùng trong Unity server
    protected ElectroServer es;
    public static string HOST_NAME = "";
    public const string PORT = "9899";
    public const string SERVER_ID = "default_server";
    public const AvailableConnection.TransportType TYPE_CONNECT = AvailableConnection.TransportType.BinaryTCP;
    IGamePlay currentGamePlay = null;

    public event DelegatePluginMessageOnProcess EventPluginMessageOnProcess;
    public event DelegateJoinRoom EventJoinRoom;
    public event DelegateLeaveRoom EventLeaveRoom;
    public event DelegatePublicMessage EventPublicMessage;
    public event DelegateLoginResponse EventLoginResponse;
    public event DelegateCreateOrJoinGame EventCreateOrJoinGame;
    public event DelegateAddBuddies EventAddBuddies;
    public event DelegateRemoveBuddies EventRemoveBuddies;
    public event CallBackFunction EventUpdateUserInfo;
    public event DelegateConfigClientChanged EventHelpChanged;
    public event DelegateConfigClientChanged EventConfigClientChanged;
    public event DelegateConfigClientChanged EventAdsChanged;
    public event DelegateGenericErrorResponse EventGenericErrorResponse;
    public event CallBackFunction EventGetMessageCallBack;
    public event CallBackFunction EventGetBuddiesCallBack;
    //use to load game select
    public event CallBackFunction EventLoadSence;
    private bool isLogged = false;
    //template total messeage get after login
    public int totalMesseageCount;

    #region OTHER EVENT
    /// <summary>
    /// Event xảy ra khi có người muốn kết bạn hoặc từ chối lời mời kết bạn
    /// </summary>
    public event DelegateFriendChanged EventFriendPendingChanged;
    /// <summary>
    /// Event xảy ra khi có thêm bạn mới, hủy kết bạn
    /// </summary>
    public event DelegateFriendChanged EventFriendChanged;

    /// <summary>
    ///Sự kiện xảy ra khi có tin nhắn đến và đi
    /// </summary>
    public event DelegateMessageChanged EventMessageChanged;

    /// <summary>
    /// Giống như EventPluginMessageOnProcess nhưng nó được gọi chỉ khi 
    /// xử lý LOBBY_UPDATE hoặc RESPONSE.USER_ONLINE_UPDATE (và bị skip)
    /// </summary>
    public event DelegatePluginMessageOnProcess EventGameplayUpdateUserOnline;

    #endregion

    #region Start & Destroy
    public CServer()
    {
        es = new ElectroServer();
        Debug.Log("Electro Server is created !!!");
        es.Engine.ConnectionClosedEvent += OnConnectClosed;
        es.Engine.ConnectionResponse += OnConnectionResponse;
        es.Engine.LoginResponse += OnLoginResponse;
        es.Engine.ServerKickUserEvent += OnServerKickUser;
        es.Engine.GatewayKickUserRequest += OnGatewayKickUser;
        es.Engine.GenericErrorResponse += OnGenericError;
        es.Engine.PluginMessageEvent += OnPluginMessage;
        es.Engine.JoinRoomEvent += OnJoinRoom;
        es.Engine.LeaveRoomEvent += OnLeaveRoom;
        es.Engine.PublicMessageEvent += OnPublicMessage;
        es.Engine.CreateOrJoinGameResponse += OnCreateOrJoinGame;
        es.Engine.AddBuddiesResponse += OnAddBuddies;
        es.Engine.RemoveBuddiesResponse += OnRemoveBuddies;

        mustThreadConnecting = false;
    }

    public void OnDestroy()
    {
        es.Engine.ConnectionClosedEvent -= OnConnectClosed;
        es.Engine.ConnectionResponse -= OnConnectionResponse;
        es.Engine.LoginResponse -= OnLoginResponse;
        es.Engine.ServerKickUserEvent -= OnServerKickUser;
        es.Engine.GatewayKickUserRequest -= OnGatewayKickUser;
        es.Engine.GenericErrorResponse -= OnGenericError;
        es.Engine.PluginMessageEvent -= OnPluginMessage;
        es.Engine.JoinRoomEvent -= OnJoinRoom;
        es.Engine.LeaveRoomEvent -= OnLeaveRoom;
        es.Engine.PublicMessageEvent -= OnPublicMessage;
        es.Engine.CreateOrJoinGameResponse -= OnCreateOrJoinGame;
        es.Engine.AddBuddiesResponse -= OnAddBuddies;
        es.Engine.RemoveBuddiesResponse -= OnRemoveBuddies;
        if (es.Engine.Connected)
            es.Engine.Close();
    }
    #endregion

    #region Connection
    public EsEngine Engine { get { return es.Engine; } }

    System.Threading.Thread threadConnect = null;
    IEnumerator _DoConnection(CallBackFunction callback)
    {
        //Đặt biến không cho gửi kết nối nữa.
        mustThreadConnecting = true;

        es.Engine.Queueing = EsEngine.QueueDispatchType.External;
        Server server = new Server(CServer.SERVER_ID);
        //if (es.Engine.Servers.Contains(server) == false)
        server.AddAvailableConnection(new AvailableConnection(CServer.HOST_NAME, System.Convert.ToInt32(CServer.PORT), TYPE_CONNECT));
        es.Engine.AddServer(server);

        log4net.Config.BasicConfigurator.Configure();

#if UNITY_WEBPLAYER
		if (!Application.isEditor && Application.isWebPlayer)
            if (!Security.PrefetchSocketPolicy(CServer.HOST_NAME, System.Convert.ToInt32(CServer.PORT), 999))
                Debug.LogError("Security Exception. Policy file load failed!");
#endif

        //Set là chưa có thông tin phản hồi về kết nối
        bool? connectionResponseSuccessful = null;

        WaitingView.Show("Kết nối đến máy chủ");

        threadConnect = new System.Threading.Thread(delegate()
        {
            es.Engine.Connect();
            connectionResponseSuccessful = es.Engine.Connected;
        });
        threadConnect.Start();

        while (connectionResponseSuccessful == null)
            yield return new WaitForEndOfFrame();

        if (isLogged)
            WaitingView.Instance.Close();

        if (!es.Engine.Connected)
        {
            WaitingView.Hide();
            NotificationView.ShowConfirm(
                "Lỗi kết nối TCP/IP.",
                "Không thể kết nối đến máy chủ.\n\n Bạn có muốn thực hiện lại hành động vừa rồi ?",
                 delegate()
                 {
                     DoConnection(callback);
                 }, null);
        }
        else
        {
            callback();
            
        }
        mustThreadConnecting = false;
    }

    bool mustThreadConnecting = false;
    void DoConnection(CallBackFunction callback)
    {
        if (mustThreadConnecting == false)
            GameManager.Instance.StartCoroutine(_DoConnection(callback));
        else
        {
           
        }
    }

    /// <summary>
    /// Khi kết nối đến server đã trả về
    /// Null    : Chưa có yêu cầu kết nối.
    /// Yes/No  : Kết nối trả về thành công hay thất bại
    /// </summary>
    protected virtual void OnConnectionResponse(ConnectionResponse e) 
    {
    }

    #endregion

    #region Do Request (Gửi những Request lên server)
    /// <summary>
    /// Gửi request lên server với kiểu request là kiểu đã được ElectroServer định nghĩa
    /// </summary>
    /// <param name="request">Gói request được gửi đi</param>
    public void DoRequest(EsMessage request)
    {
        if (request is PluginRequest)
            Debug.LogWarning("PluginRequest Server time : " + System.DateTime.Now.ToString() + " - " + ((PluginRequest)request).PluginName + " - param :" + ((PluginRequest)request).Parameters);
        else
            Debug.LogWarning(request.GetType().Name + " time : " + System.DateTime.Now.ToString());

        if (!es.Engine.Connected)
            DoConnection(delegate() {
                es.Engine.Send(request); 
            });
        else
            es.Engine.Send(request);
    }

    #region Do Request Plugin
    /// <summary>
    /// Gửi request lên server với kiểu request là PluginRequest
    /// </summary>
    /// <param name="param">EsObject được gửi lên server</param>
    /// <param name="isNode">Là yêu cầu ngoài game hay trong game</param>
    void _DoRequestPlugin(EsObject param, bool isNode)
    {
        PluginRequest request = new PluginRequest();
        if (isNode)
            request.PluginName = Fields.REQUEST.NODE_PLUGIN;
        else
            request.PluginName = Fields.REQUEST.GAME_PLUGIN;

        request.RoomId = GameManager.Instance.currentRoom.roomId;
        request.ZoneId = GameManager.Instance.currentRoom.zoneId;
        request.Parameters = param;
        DoRequest(request);
    }

    /// <summary>
    /// login 2 phase
    /// </summary>
    /// <param name="param"></param>
    public void DoRequestPluginLogin(EsObject param)
    {
        _DoPluginLogin(param);
    }

    void _DoPluginLogin(EsObject param)
    {
        PluginRequest request = new PluginRequest();
        request.PluginName = Fields.REQUEST.LOGIN_PLUGIN;
        request.Parameters = param;
        DoRequest(request);
    }

    public void DoRequestPlugin(EsObject param)
    {
        _DoRequestPlugin(param, true);
    }

    public void DoRequestPluginGame(EsObject param)
    {
        _DoRequestPlugin(param, false);
    }

    public void DoRequestGameAction(string action)
    {
        DoRequestPluginGame(Utility.SetEsObject(Fields.GAMEPLAY.PLAY, new object[] { Fields.ACTION, action }));
    }

    public void DoRequestGameCommand(string command)
    {
        DoRequestPluginGame(Utility.SetEsObject(command));
    }
    #endregion

    /// <summary>
    /// Gửi request lên server với kiểu request là PublicMessage
    /// </summary>
    /// <param name="message">Message của PublicMessage gửi lên</param>
    public void DoRequestPublicMessage(string message)
    {
        PublicMessageRequest request = new PublicMessageRequest();
        request.ZoneId = GameManager.Instance.currentRoom.zoneId;
        request.RoomId = GameManager.Instance.currentRoom.roomId;
        request.Message = message;
        DoRequest(request);
    }

    /// <summary>
    /// Rời khỏi phòng hiện tại
    /// </summary>
    public void DoLeaveCurrentRoom()
    {
        Debug.Log("attemping to leave room: Zone ="  + GameManager.Instance.currentRoom.zoneId + " -  Room = " + GameManager.Instance.currentRoom.roomId + ", ");
        DoLeaveRoom(GameManager.Instance.currentRoom.zoneId, GameManager.Instance.currentRoom.roomId);
    }

    /// <summary>
    /// Rời khỏi phòng nào đó (Chỉ sử dụng khi JoinGame) còn lại đã tự động hết rồi.
    /// </summary>
    public void DoLeaveRoom(int zoneId, int roomId)
    {
        LeaveRoomRequest leave = new LeaveRoomRequest();
        leave.ZoneId = zoneId;
        leave.RoomId = roomId;
        DoRequest(leave);
    }


    public RoomInfo doJoiningRoom = null;
    /// <summary>
    /// Hàm sẽ gửi lệnh Leave sau đó Send Messeage đến cho Server và đẩy lên server JoinRoomRequest
    /// Ta sẽ viết hàm hứng OnAfterJoinRoom để xử lý khi join room thành công
    /// </summary>
    public void DoJoinRoom(int _zoneID, int _roomID)
    {
        Debug.Log("_zoneID " + _zoneID + " _roomID " + _roomID);
        doJoiningRoom = new RoomInfo(_zoneID, _roomID);
        JoinRoomRequest reqJoin = new JoinRoomRequest();
        reqJoin.ZoneId = _zoneID;
        reqJoin.RoomId = _roomID;
        DoRequest(reqJoin);
    }

    public void DoJoinGameTournament(int gameId)
    {
        DoRequestPlugin(Utility.SetEsObject("joinGame", new object[] {
            "gameId", gameId
        }));
    }

    /// <summary>
    /// Hàm sẽ gửi lên server Join vào trận đấu.
    /// (Tương tự như JoinRoom nhưng mà sử dụng sẵn hàm của ES5.)
    /// </summary>
    /// <param name="password"></param>
    public void DoJoinGame(string password)
    {
        DoRequestPlugin(Utility.SetEsObject("joinGame", new object[] {
            "gameId", GameManager.Instance.selectedLobby.gameId,
            "password", password
        }));
    }

    /// <summary>
    /// Gửi như DoRequestPlugin nhưng không có object, chỉ gửi command
    /// </summary>
    public void DoRequestCommand(string command)
    {
        DoRequestPlugin(Utility.SetEsObject(command));
    }

    public void DoLogin()
    {
        WaitingView.Show("Đang đăng nhập");
        LoginRequest loginRequest = new LoginRequest();
        if (string.IsNullOrEmpty(GameManager.Instance.accessToken))
            loginRequest.Password = GameManager.Instance.passwordLogin;

        loginRequest.UserName = GameManager.Instance.userNameLogin;

        if (!string.IsNullOrEmpty(GameManager.Instance.accessToken))
        {
            loginRequest.EsObject = Utility.SetEsObject(null, new object[] { 
            "isLogin2Phase", true,
            "accessToken",GameManager.Instance.accessToken
        });
        }
        else
        {
            loginRequest.EsObject = Utility.SetEsObject(null, new object[] { 
            "isLogin2Phase", true,
        });
        }

        DoRequest(loginRequest);
    }

    public void DoLogin(string username, string password, string deviceToken)
    {
        LoginRequest loginRequest = new LoginRequest();
        loginRequest.UserName = username;
        loginRequest.Password = password;
        loginRequest.EsObject = Utility.SetEsObject(null, new object[] { 
            Fields.REQUEST.APP_ID, (int)GameManager.GAME,
            "environment", Common.GetDevice,
            "version", GameSettings.CurrentVersion,
			"platform", PlatformSetting.GetSamplePlatform.ToString(),
			"deviceToken", deviceToken
        });

        DoRequest(loginRequest);
    }
	public void DoLogin(string accessToken,string deviceToken)
	{
		LoginRequest request = new LoginRequest ();
		request.EsObject = Utility.SetEsObject(null, new object[] { 
			Fields.REQUEST.ACCESS_TOKEN , accessToken,
			Fields.REQUEST.APP_ID, (int)GameManager.GAME,
			"environment", Common.GetDevice,
			"version", GameSettings.CurrentVersion,
			"platform", PlatformSetting.GetSamplePlatform.ToString(),
			"deviceToken", deviceToken
		});
		DoRequest(request);
	}

    /// <summary>
    /// Sự kiện xảy ra khi bạn logout ứng dụng
    /// </summary>
    public void DoLogOut()
    {
        StoreGame.Remove(StoreGame.EType.DEBUG_LOG);//REMOVE DEBUG_LOG
        StoreGame.SaveString(StoreGame.EType.BOOL_SEND_LOG_TO_SERVER,"false");//REMOVE DEBUG_LOG
        WaitingView.Hide();
        if (Application.loadedLevelName == ESceneName.LoginScreen.ToString()) { Disconnect(); return; }
        //DoLeaveCurrentRoom();
        LogOutRequest logOutRequest = new LogOutRequest();
        DoRequest(logOutRequest);
        #region BUILD WEB ESIMO COMMENT VAO KHI BUILD CHO FACEBOOK
#if UNITY_WEBPLAYER
        switch (GameSettings.Instance.TypeBuildFor)
        {
            case GameSettings.EBuildType.esimo:
            case GameSettings.EBuildType.web_esimo:
                    Application.ExternalEval("window.location = \"/site/logout\"");
                break;
        } 
#endif
        #endregion
        StoreGame.Remove(StoreGame.EType.SAVE_USERNAME);
        StoreGame.Remove(StoreGame.EType.SAVE_PASSWORD);
        StoreGame.Remove(StoreGame.EType.SAVE_ACCESSTOKEN);
        if (es.Engine.Connected)
            es.Engine.Close();
        if ((FB.IsLoggedIn || !string.IsNullOrEmpty(FB.AccessToken)) && !Application.isWebPlayer)
            FB.Logout();
        Disconnect();
        GameManager.LoadScene(ESceneName.LoginScreen);
    }

    /// <summary>
    /// Sự kiện xảy ra khi bị ngắt kết nối.
    /// </summary>
    public void WhenDisconnect()
    {
        Disconnect();

        if (Application.loadedLevelName == ESceneName.LoginScreen.ToString()) return;

        GameManager.LoadScene(ESceneName.LoginScreen);
    }

    void Disconnect()
    {
        GameManager.Instance.selectedLobby = new RoomInfo();
        GameManager.Instance.selectedChannel = new RoomInfo();
        GameManager.Instance.channelRoom = new RoomInfo();
        GameManager.Instance.currentRoom = new RoomInfo();

        GameManager.Instance.ListAnnouncement.RemoveAll(a => a.show == Announcement.Scene.announce && a.type == Announcement.Type.Gift);
        GameManager.Instance.ServerIsNull();
        WaitingView.Instance.Close();

        isLogged = false;
		PlaySameDevice.Clear ();

    }

    /// <summary>
    /// remove all waiting view
    /// </summary>
    void RemoveAllWaitingView()
    {
        WaitingView[] waiting = GameObject.FindObjectsOfType<WaitingView>();
        foreach (var wait in waiting)
        {
            waiting.Clone();
        }
    }

    #endregion

    #region ĐĂNG KÝ EVENT VỚI SERVER
    /// <summary>
    /// When the server idle timeout disconnect a user or when the server evicts the user due to another login with the same user name and password (evictGhostUser), the client receives a GatewayKickUserRequest message as seen in the Unity editor/player log.
    /// </summary>
    protected virtual void OnGatewayKickUser(GatewayKickUserRequest e)
    {
        //Debug.LogWarning("ServerTimeout or Disconnect or Another login with the same user");
        //if (e.ClientId == 0)
        //{
        //    DoLogOut();
        //    NotificationView.ShowConfirm("Thông báo", "Bạn đã bị kick do có người chơi khác đăng nhập vào tài khoản của bạn.\n\nHoặc", null, null);
        //}
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void OnServerKickUser(ServerKickUserEvent e)
    {
        DoLogOut();
        if (e.Error == ErrorType.IdleTimeReached)
            NotificationView.ShowConfirm("Thông báo", "Bạn đã bị ngắt kết nối đến máy chủ\n\ndo quá lâu không tương tác",
                delegate()
                {
                    GameManager.Server.DoLogin(GameManager.Instance.mInfo.username, GameManager.Instance.mInfo.password, GameManager.Instance.deviceToken);
                }, null, "Kết nối lại", "Đóng");
        else if (e.Error == ErrorType.UserKickedFromServer){
            if (e.EsObject.variableExists("reason"))
            {
                //NotificationView.ShowMessage("Bạn đã bị thoát ra vì có người chơi khác\n\nđăng nhập vào tài khoản của bạn.");
                NotificationView.ShowMessage(e.EsObject.getString("reason"));
            }
        }
    }

    protected virtual void OnConnectClosed(ConnectionClosedEvent e)
    {
       
        //Đã có thông báo nào đó trước rồi. Và đã disconnect rồi
        if (GameManager.Instance.currentRoom == null) return;

        Debug.LogWarning("ConnectClosed: Server Disconnect.....!");

        WhenDisconnect();

        if(GameManager.CurrentScene!=ESceneName.LoginScreen)
            NotificationView.ShowMessage("Bạn đã bị mất kết nối đến với máy chủ !");
    }

    protected virtual void OnGenericError(GenericErrorResponse e)
    {
        Debug.LogError("GenericErrorResponse: " + e.ErrorType);
        switch (e.ErrorType)
        {
            case ErrorType.ActionRequiresLogin:
                DoLogOut();
                NotificationView.ShowMessage("Phiên đăng nhập hết hạn, mời bạn đăng nhập lại.", 5f);
                break;
            case ErrorType.UserBanned:
                NotificationView.ShowMessage("Tài khoản của bạn đã bị ban, không thể vào phòng.", 5f);
                break;
            case ErrorType.FailedToJoinGameRoom:
                NotificationView.ShowMessage("Không thể vào phòng game.", 5f);
                break;
            case ErrorType.UserNameExists:
                NotificationView.ShowMessage("Gặp lỗi bất thường khi thực hiện đăng nhập, mã lỗi: -1", 5f);
                break;
            case ErrorType.UserNotJoinedToRoom:
                // NotificationView.ShowMessage("", 5f);
                break;
            default:
                break;

        }
    }

    protected virtual void OnLoginResponse(LoginResponse e)
    {
        isLogged = true;
        if (e.Successful)
        {
            EsObject param = new EsObject();

            if (!string.IsNullOrEmpty(GameManager.Instance.userNameLogin) && !string.IsNullOrEmpty(GameManager.Instance.passwordLogin))
            {
                param.setString("userName", GameManager.Instance.userNameLogin);
                param.setString("password", GameManager.Instance.passwordLogin);
                param.setInteger(Fields.REQUEST.APP_ID, (int)GameManager.GAME);
                param.setString("environment", Common.GetDevice);
                param.setString("version", GameSettings.CurrentVersion);
                param.setString("platform", PlatformSetting.GetSamplePlatform.ToString());
                param.setString("deviceToken", GameManager.Instance.deviceToken);
            }
            else if (!string.IsNullOrEmpty(GameManager.Instance.accessToken))
            {
                param.setString("accessToken", GameManager.Instance.accessToken);
                param.setInteger(Fields.REQUEST.APP_ID, (int)GameManager.GAME);
                param.setString("environment", Common.GetDevice);
                param.setString("version", GameSettings.CurrentVersion);
                param.setString("platform", PlatformSetting.GetSamplePlatform.ToString());
                param.setString("deviceToken", GameManager.Instance.deviceToken);
            }

            GameManager.Server.DoRequestPluginLogin(Utility.SetEsObject(Fields.REQUEST.COMMAND_LOGIN, new object[] { "loginInfo", param }));
        }
        else
        {
            WaitingView.Instance.Close();
            StoreGame.Remove(StoreGame.EType.SAVE_USERNAME);
            StoreGame.Remove(StoreGame.EType.SAVE_PASSWORD);
            StoreGame.Remove(StoreGame.EType.SAVE_ACCESSTOKEN);
            string message = "Thông tin đăng nhập không hợp lệ. Yêu cầu nhập lại thông tin truy cập.";
            if (e.EsObject.variableExists("reason"))
            {
                message = e.EsObject.getString("reason");
            }
            NotificationView.ShowMessage(message);
        }
    }

    protected virtual void OnCreateOrJoinGame(CreateOrJoinGameResponse e)
    {
        WaitingView.Instance.Close();
        if (EventCreateOrJoinGame != null)
        {
            //Debug.Log(LogEvent(EventCreateOrJoinGame));
            EventCreateOrJoinGame(e);
        }
    }

    protected virtual void OnAddBuddies(AddBuddiesResponse e)
    {
        if (EventAddBuddies != null)
        {
            //Debug.Log(LogEvent(EventAddBuddies));
            EventAddBuddies(e);
        }
    }
    protected virtual void OnRemoveBuddies(RemoveBuddiesResponse e)
    {
        if (EventRemoveBuddies != null)
        {
            //Debug.Log(LogEvent(EventRemoveBuddies));
            EventRemoveBuddies(e);
        }
    }

    protected virtual void OnJoinRoom(JoinRoomEvent e)
    {    
        Debug.Log("Current Room Saved!!! - " + this.GetType().ToString() + ": " + "Join to room:" + e.RoomName + " - ZoneID, RooID: " + e.ZoneId + "," + e.RoomId);
        //Rời room cũ trước khi Join vào room mới (Chỉ có lần đầu Join là không cần rời)
        if (GameManager.CurrentScene != ESceneName.LoginScreen && doJoiningRoom != null && GameManager.CurrentScene != ESceneName.GameplayChan)
        {
			doJoiningRoom = null;
            DoLeaveCurrentRoom();
        }

        if (GameManager.CurrentScene != ESceneName.ChannelLeague)
            GameManager.Instance.currentRoomGiaiDau = new RoomInfo(e.ZoneId, e.RoomId);

        GameManager.Instance.currentRoom = new RoomInfo(e.ZoneId, e.RoomId);
        if (EventJoinRoom != null)
        {
            //Debug.Log(LogEvent(EventJoinRoom));
            EventJoinRoom(e);
        }
        WaitingView.Instance.Close();
    }

    protected virtual void OnLeaveRoom(LeaveRoomEvent e)
    {
        Debug.Log(this.GetType().ToString() + ": " + "LeaveRoomEvent !!! Zone:" + e.ZoneId + " Room:" + e.RoomId);
        if (EventLeaveRoom != null)
        {
            //Debug.Log(LogEvent(EventLeaveRoom));
            EventLeaveRoom(e);
        }
        WaitingView waiting = GameObject.FindObjectOfType<WaitingView>();
        if (waiting)
        {
            waiting.Close();
        }
    }

    protected virtual void OnPublicMessage(PublicMessageEvent e)
    {
        Debug.Log(this.GetType().ToString() + ": " + "OnPublicMessage !!! " + e.UserName + ": " + e.Message + " - " + e.EsObject);

        if (EventPublicMessage != null)
        {
            //Debug.Log(LogEvent(EventPublicMessage));
            EventPublicMessage(e);
        }
    }

    protected virtual void OnPluginMessage(PluginMessageEvent e)
    {
        EsObject eso = e.Parameters;
        string command = eso.getString(Fields.COMMAND);
        string action = string.Empty;
        if (eso.variableExists(Fields.ACTION))
            action = eso.getString(Fields.ACTION);

        Debug.Log(this.GetType().ToString() + ": " + "time :" + System.DateTime.Now.ToString() + "eso :" + eso);

        if (command == "joinGame" || listWaitingGameplay.Count > 0)
        {
            
            Debug.LogWarning("CServer -> Gameplay chờ command : " + command);
            if (command == "joinGame")
            {
                ProcessGeneral(command, action, eso);
                listWaitingGameplay.Add(null);
            }
            else
                listWaitingGameplay.Add(new CommandEsObject(e, command, action, eso));
        }
        else
            ProcessPluginMessage(e, command, action, eso);
    }

    /// <summary>
    /// Process plugin message from server response
    /// </summary>
    void ProcessPluginMessage(PluginMessageEvent e, string command, string action, EsObject eso)
    {
        ProcessGeneral(command, action, eso);
        if (EventPluginMessageOnProcess != null)
        {
            //Trường hợp các phòng mà không phải đang đứng ở đó nhưng server vẫn trả về thông tin
            if ((e.OriginRoomId != GameManager.Instance.currentRoom.roomId || e.OriginZoneId != GameManager.Instance.currentRoom.zoneId)
                && (command == Fields.RESPONSE.LOBBY_UPDATE || command == Fields.RESPONSE.USER_ONLINE_UPDATE))
            {
                if (command == Fields.RESPONSE.USER_ONLINE_UPDATE && EventGameplayUpdateUserOnline != null)
                    EventGameplayUpdateUserOnline(command, action, eso);
                else
                    Debug.Log("SKIP----> " + e.OriginRoomId + " - " + e.OriginZoneId + "- command=" + command);
                return;
            }

            EventPluginMessageOnProcess(command, action, eso);
        }
    }
    #endregion

    #region Register Event Listener
    public void RegisterEventMessageChanged(Messages mess, bool isOutGoing)
    {
        if (EventMessageChanged != null)
            EventMessageChanged(mess, isOutGoing);
    }
    #endregion

    #region Processs General
    void ProcessGeneral(string command, string action, EsObject eso)
    {
        if (command == Fields.RESPONSE.CREATE_GAME)
        {
            #region CREATE_GAME
            if (eso.getBoolean("successful"))
                GameManager.Instance.selectedLobby.SetDataJoinLobby(eso);
            else
               NotificationView.ShowMessage("Lỗi! Tạo bàn chơi không thành công.",3f);
            #endregion
        } 
        else if (command == "joinGame")
        {
            #region JOIN GAME
            if (EventLoadSence != null)
                EventLoadSence();
            GameManager.Instance.selectedLobby.SetDataJoinLobby(eso);
            WaitingView.Hide();
            PlaySameDevice.SaveDeviceWhenJoinGame();
            #endregion
        }
        else if (command == "removeFriend")
        {
            #region REMOVE FRIEND
            EsObject obj = eso.getEsObject("user");

            User pendingUser = GameManager.Instance.mInfo.pendingBuddies.Find(user => user.username == (obj.variableExists("username") ? obj.getString("username") : obj.getString(Fields.PLAYER.USERNAME)));
            if (pendingUser != null)
            {
                GameManager.Instance.mInfo.pendingBuddies.Remove(pendingUser);

                if (EventFriendPendingChanged != null)
                    EventFriendPendingChanged(pendingUser, true);
            }
            else
            {
                pendingUser = GameManager.Instance.mInfo.buddies.Find(user => user.username == (obj.variableExists("username") ? obj.getString("username") : obj.getString(Fields.PLAYER.USERNAME)));
                GameManager.Instance.mInfo.buddies.Remove(pendingUser);

                if (EventFriendChanged != null)
                    EventFriendChanged(pendingUser, true);
            }
            #endregion
        }
        else if (command == "acceptFriendRequest")
        {
            #region ACCEPT FRIEND REQUEST
            EsObject obj = eso.getEsObject("user");
            User user = new User(obj);

            if (GameManager.Instance.mInfo.requestBuddies.Find(u => u.username == user.username) != null)
                GameManager.Instance.mInfo.requestBuddies.Remove(GameManager.Instance.mInfo.requestBuddies.Find(u => u.username == user.username));

            if (GameManager.Instance.mInfo.buddies.Find(u => u.username == user.username) == null)
            {
                User pendingUser = GameManager.Instance.mInfo.pendingBuddies.Find(u => u.username == user.username);
                if (pendingUser != null)
                    GameManager.Instance.mInfo.pendingBuddies.Remove(pendingUser);

                GameManager.Instance.mInfo.buddies.Add(user);
                if (EventFriendChanged != null)
                    EventFriendChanged(user, false);
            }
            #endregion
        }
        else if (command == "friendRequest")
        {
            #region FRIEND REQUEST
            EsObject obj = eso.getEsObject("user");
            User user = new User(obj);
            GameManager.Instance.mInfo.pendingBuddies.Add(user);

            if (EventFriendPendingChanged != null)
                EventFriendPendingChanged(user, false);
            #endregion
        }
        else if (command == "inComingMessage")
        {
            #region INCOME MESSAGE
            Messages message = new Messages(eso.getEsObject("message"));
            GameManager.Instance.mInfo.messages.Add(message);

            RegisterEventMessageChanged(message, false);
            #endregion
        }
        else if (command == "outGoingMessage")
        {
            #region OUT GOING MESSAGE
            Messages mess = new Messages();
            mess.SetDataOutGoing(eso);
            GameManager.Instance.mInfo.messages.Add(mess);
            RegisterEventMessageChanged(mess, true);
            #endregion
        }
        else if (command == "systemMessage")
        {
            #region SYSTEM MESSAGE
            Messages message = new Messages(eso.getEsObject("message"));

            message.sender = 0;
            message.read = false;
            message.receiver = GameManager.Instance.mInfo.id;
            message.receiver_name = GameManager.Instance.mInfo.username;

            MessageSystemCache.SaveCache(new Hashtable[] { message.ParseToHashtable });

            if (message.type != 0)
                ServerMessagesView.MessageServer(message.content);
            else
                RegisterEventMessageChanged(message, false);
            #endregion
        }
        else if (command == "invitePlayGame")
        {
            #region INVITE PLAY GAME
            string actionInvited = "joinGame";
            if (eso.variableExists("action"))
                actionInvited = eso.getString("action");

            if (actionInvited == "createGame")
            {
                NotificationView.ShowConfirm("Xác nhận", "Bạn có muốn tạo phòng mới hay không?",
                delegate()
                {
                    GameManager.Server.DoRequestPlugin(Utility.SetEsObject(Fields.RESPONSE.CREATE_GAME,
                        new object[] { "config", createRoom() }));
                }, delegate()
                {
                    GameManager.Server.DoRequestCommand("abortInvitation");
                });
            }
            else
            {
                #region INVITE PLAYER
                string userName = eso.getString(Fields.PLAYER.USERNAME);
                int gameId = eso.getInteger("gameId");
                string password = eso.variableExists("password") ? eso.getString("password") : "";
                if (password.Length == 0 && eso.variableExists("config"))
                {
                    EsObject esoConfig = eso.getEsObject("config");
                    if (esoConfig.variableExists("password"))
                    {
                        password = esoConfig.getString("password");
                    }
                }
                int roomId = eso.getInteger("roomId");
                int zoneId = eso.getInteger("zoneId");
                int gameIndex = eso.getInteger("gameIndex");

                if (GameManager.CurrentScene == ESceneName.LobbyChan || GameManager.CurrentScene==ESceneName.LobbyPhom || GameManager.CurrentScene == ESceneName.LobbyTLMN)
                {
                    string contentMsg = "Bạn nhận được một lời mời chơi!";
                    if (userName.Length > 0)
                    {
                        contentMsg = "\"" + Utility.Convert.ToTitleCase(userName) +
                            "\" gửi bạn một lời mời tham gia vào bàn chơi số " + gameIndex + ".\n\n";
                    }
                    NotificationView.ShowConfirm("Mời chơi",
                        contentMsg,
                        delegate()
                        {
                            //GameManager.Instance.selectedLobby = GameManager.CurrentScene == ESceneName.LobbyChan ? new LobbyChan(zoneId, roomId, gameId) : GameManager.CurrentScene == ESceneName.LobbyPhom ? new LobbyPhom(zoneId, roomId, gameId) : new LobbyTLMN(zoneId, roomId, gameId);
                            if (GameManager.CurrentScene == ESceneName.LobbyPhom)
                                GameManager.Instance.selectedLobby = new LobbyPhom(zoneId, roomId, gameId);
                            else if(GameManager.CurrentScene==ESceneName.LobbyChan)
                                GameManager.Instance.selectedLobby = new LobbyChan(zoneId, roomId, gameId);
                            else
                                GameManager.Instance.selectedLobby = new LobbyTLMN(zoneId, roomId, gameId);
                            GameManager.Server.DoJoinGame(password);
                        }, delegate()
                    {
                        GameManager.Server.DoRequestCommand("abortInvitation");
                    });
                }
                #endregion
            }
            #endregion
        }
        else if (command == "updateUserInfo")
        {
            #region CẬP NHẬT THÔNG TIN USERS
            if (eso.variableExists("userName") && eso.getString("userName") == GameManager.Instance.mInfo.username)
            {
                if (eso.variableExists("field"))
                {
                    string field = eso.getString("field");
                    
                    if (field == "money"){
                        string moneyType = "";
                        if (eso.variableExists("moneyType")) { moneyType = eso.getString("moneyType"); }
                        if(moneyType!="" && moneyType=="chip")
                            long.TryParse(eso.getString("value"), out GameManager.Instance.mInfo.chip);
                        else if (moneyType != "" && moneyType == "gold")
                            long.TryParse(eso.getString("value"), out GameManager.Instance.mInfo.gold);
                    }
                    else if (field == "experience")
                        GameManager.Instance.mInfo.SetDataUser(eso.getEsObject("userInfo"));

                    if (EventUpdateUserInfo != null)
                        EventUpdateUserInfo();
                }
            }
            else if (eso.variableExists("userName") && eso.getString("userName") != GameManager.Instance.mInfo.username)
            {
                if (GameManager.CurrentScene == ESceneName.GameplayChan && eso.variableExists("field"))
                {
                    string field = eso.getString("field");
                    if (field == "experience")
                    {
                        if (GameModelChan.GetPlayer(eso.getString("userName")) != null)
                            GameModelChan.GetPlayer(eso.getString("userName")).SetDataUser(eso.getEsObject("userInfo"));
                    }
                }
            }
            #endregion
        }
        else if (command == "updateConfigClient")
        {
            #region CẬP NHẬT THÔNG TIN CONFIG 
            if (eso.variableExists("config"))
            {
                IDictionary obj = (IDictionary)JSON.JsonDecode(eso.getString("config"));
                string type = obj[Fields.CONFIGCLIENT.KEY_TYPE_REAL_TIME].ToString();
                switch (type)
                {
                    case Fields.CONFIGCLIENT.VALUE_ADS:
                        Announcement announce = new Announcement(
                            Convert.ToInt32(obj["index"]),
                            obj["description"].ToString(),
                            obj["scenes"].ToString() == "lobby"
                                ? Announcement.Scene.lobby
                                : obj["scenes"].ToString() == "login"
                                ? Announcement.Scene.login
                                : Announcement.Scene.announce,
                            obj["url"].ToString(),
                            obj["image"].ToString(),
                            obj["type"].ToString() == "Ads" ? Announcement.Type.Advertisement : Announcement.Type.Event
                        );
                        GameManager.Instance.ListAnnouncement.Remove(GameManager.Instance.ListAnnouncement.Find(ads => ads.show == announce.show && ads.type == Announcement.Type.Advertisement));
                        GameManager.Instance.ListAnnouncement.Add(announce);
                        if (EventAdsChanged != null) { EventAdsChanged(obj); }
                        break;
                    case Fields.CONFIGCLIENT.VALUE_HELP:
                        StoreGame.Remove(StoreGame.EType.CACHE_HELP);
                        GameManager.Instance.ListHelp.Clear();
                        StoreGame.SaveString(StoreGame.EType.CACHE_HELP, JSON.JsonEncode(obj[Fields.RESPONSE.PHP_RESPONSE_ITEMS]));
                        ArrayList list = (ArrayList)obj[Fields.RESPONSE.PHP_RESPONSE_ITEMS];
                        foreach (Hashtable item in list)
                            GameManager.Instance.ListHelp.Add(item);
                        if (EventHelpChanged != null) EventHelpChanged(obj);
                        break;
                    case Fields.CONFIGCLIENT.VALUE_CONFIG_CLIENT:
                        ArrayList items = (ArrayList)obj[Fields.RESPONSE.PHP_RESPONSE_ITEMS];
                        foreach (Hashtable item in items)
                        {
                            GameManager.Setting.Platform.AddOrUpdatePlatformConfig(item);
                        }
                        if (GameManager.Setting.Platform.GetConfigByType(PlatformType.url_ping) != null)
                        {
                            ServerWeb.URL_PING = GameManager.Setting.Platform.GetConfigByType(PlatformType.url_ping).Value;
                        }
                        if (EventConfigClientChanged != null) EventConfigClientChanged(obj);
                        break;
                }
            }
            #endregion
        }
        else if (command == Fields.RESPONSE.DAYLY_GIFT)
        {
            #region Thông tin trả về khi nhận quà tặng
            //AnnouncementView.Instance.Close();
            if (eso.variableExists("textNotification"))
            {
                
                if (GameObject.Find("__Announcement") != null)
                {
                    AnnouncementView.Instance.SetCenterOnNextCurrentDay();
                }
                GameManager.Instance.FunctionDelay(delegate()
                {
                    string text = eso.getString("textNotification");
                    NotificationView.ShowMessage(text);
                }, 1f);
                GameManager.Instance.ListAnnouncement.RemoveAll(a => a.show == Announcement.Scene.announce && a.type == Announcement.Type.Gift);
            }
            #endregion
        }
        else if (command == Fields.RESPONSE.GET_MESSAGE)
        {
            #region LẤY THÔNG TIN TIN NHẮN TỪ SERVER 
            WaitingView.Hide();
            //disable total count khi chưa load messeage
            this.totalMesseageCount = -1;
            if (eso.variableExists("messages"))
            {
                EsObject[] array = eso.getEsObjectArray("messages");
                System.Array.ForEach<EsObject>(array, o => { GameManager.Instance.mInfo.messages.Add(new Messages(o)); });

                List<Messages> systemMessage = GameManager.Instance.mInfo.messages.FindAll(m => m.sender == 0);
                GameManager.Instance.ListMessageSystem.Clear();
                while (systemMessage.Count > 0)
                {
                    GameManager.Instance.ListMessageSystem.Add(systemMessage[0]);
                    GameManager.Instance.mInfo.messages.Remove(systemMessage[0]);
                    systemMessage.RemoveAt(0);
                }
                if (EventGetMessageCallBack != null) 
                {
                    EventGetMessageCallBack();
                }
            }
            #endregion
        }
        else if (command == Fields.RESPONSE.GET_BUDDIES)
        {
            #region LẤY THÔNG TIN DANH SÁCH BẠN BÈ
            WaitingView.Hide();
            GameManager.Instance.mInfo.SetDataUser(eso);
            if (EventGetBuddiesCallBack != null)
            {
                EventGetBuddiesCallBack();
            }
            #endregion
        }
        else  if (command == "getLevel")
        {
            if (eso.variableExists("level"))
                GameManager.Instance.mInfo.level = eso.getInteger("level");
            if (eso.variableExists("experience"))
                GameManager.Instance.mInfo.experience = eso.getInteger("experience");
            if (eso.variableExists("expMinCurrentLevel"))
                GameManager.Instance.mInfo.expMinCurrentLevel = eso.getInteger("expMinCurrentLevel");
            if (eso.variableExists("expMinNextLevel"))
                GameManager.Instance.mInfo.expMinNextLevel = eso.getInteger("expMinNextLevel");
        }
    }
    public EsObject createRoom()
    {
        EsObject gameConfig = new EsObject();
        string roomName = "Bàn chơi của " + GameManager.Instance.mInfo.username;
        gameConfig.setString(LobbyChan.DEFINE_LOBBY_NAME, roomName);
        gameConfig.setString(LobbyChan.DEFINE_LOBBY_PASWORD, "");
        gameConfig.setInteger(LobbyChan.DEFINE_BETTING, 10);
        gameConfig.setInteger(LobbyChan.DEFINE_USING_NUOI_GA, (int)LobbyChan.EGaRule.none);//true nuoi ga false ga nhai
        gameConfig.setBoolean(LobbyChan.DEFINE_USING_AUTO_BAT_BAO, true);//true 
        gameConfig.setBoolean(LobbyChan.DEFINE_USING_AUTO_U, true);//true 
        gameConfig.setInteger(LobbyChan.DEFINE_RULE_FULL_PLAYING, 1);// mặc đinh là ù xuông;
        gameConfig.setStringArray(LobbyChan.DEFINE_INVITED_USERS, new string[0]);
        gameConfig.setInteger(LobbyChan.DEFINE_PLAY_ACTION_TIME, 20);// defalt 20s
        return gameConfig;
    }
    #endregion
    #region DANH SÁCH CHỜ ĐỂ JOIN GAME THÀNH CÔNG
    public void ProcessStackWaiting()
    {
        GameManager.Instance.StartCoroutine(_ProcessStackWaiting());
    }
    IEnumerator _ProcessStackWaiting()
    {
        while (listWaitingGameplay.Count > 0)
        {
            CommandEsObject item = listWaitingGameplay[0];
            listWaitingGameplay.RemoveAt(0);

            if (item == null) continue;

            Debug.LogWarning("CServer Đang xử lý command : " + item.command);

            ProcessPluginMessage(item.e, item.command, item.action, item.eso);
            yield return new WaitForSeconds(0.01f);
        }
    }
    public List<CommandEsObject> listWaitingGameplay = new List<CommandEsObject>();
    #endregion

    //////// - Nguyễn Việt Dũng : Anh em chú ý nhé. (BỎ COMMENT RA KHI MUỐN BIẾT SỰ KIỆN PHÁT SINH TỪ ĐÂU) - ////////////
    string LogEvent(System.Delegate _event)
    {
        if (_event == null) return "";
        string strDebug = "Event: " + _event.ToString() + " ->\n";
        foreach (System.Delegate _delegate in _event.GetInvocationList())
            strDebug += "" + _delegate.Method.ReflectedType.Name + " : " + _delegate.Method.Name + "\n";
        return strDebug;
    }
}