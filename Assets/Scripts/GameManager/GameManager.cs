using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;
using System;

public delegate void CallBackFunction();
public enum ESceneName
{
    LoginScreen = 0,
    HallSceen = 1,
    ChannelChan = 2,
    LobbyChan = 3,
    CreateRoomChan = 4,
    GameplayChan = 5,
    ChannelTLMN = 6,
    LobbyTLMN = 7,
    CreateRoomTLMN = 8,
    GameplayTLMN = 9,
    ChannelPhom = 10,
    LobbyPhom = 11,
    CreateRoomPhom = 12,
    GameplayPhom = 13,
    Profiles = 14,
    ChannelLeague = 15,
    Tournament = 16
}

public enum EGame
{
    Phom = 1,
    TLMN = 2,
    Chan = 3,
    Esimo = 4,
}

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Khi chạy app thì tự động được sinh ra
/// và luôn luôn chạy cùng app.
/// </summary>
public class GameManager : MonoBehaviour
{
    //đặt thêm biến để test. do test trên cùng 1 thiết bị lưu kiểu store sẽ mất dữ liệu ko test đc
    [HideInInspector]
    public string userNameLogin,passwordLogin,accessToken;
    [HideInInspector]
    public bool displayTournamentMenu = false;

    static GameManager _instance;
    CServer _mServer;

    //current Tournament selected
    [HideInInspector]
    public TournamentInfo currentTournamentInfo;

    public static string PlayGoldOrChip;

    /// <summary>
    /// Game đang xử lý
    /// </summary>
    public static EGame GAME = EGame.Esimo;

    IGameModel currentModel;
    /// <summary>
    /// Lưu thông tin người chơi sau khi đăng nhập
    /// </summary>
    [HideInInspector]
    public User mInfo;

    /// <summary>
    /// Lưu thông tin room channel để sau này Back Join lại
    /// channelRoom này là Room Phỏm.
    /// </summary>
    [HideInInspector]
    public RoomInfo channelRoom,hallRoom;

    /// <summary>
    /// Lưu thông tin Room hiện tại
    /// </summary>
    [HideInInspector]
    public RoomInfo currentRoom,currentRoomGiaiDau;

    /// <summary>
    /// Phòng các lobby mà bạn vào
    /// </summary>
    [HideInInspector]
    public RoomInfo selectedChannel;

    /// <summary>
    /// Game mà bạn đã chọn
    /// </summary>
    [HideInInspector]
    public RoomInfo selectedLobby;

    /// <summary>
    /// Lưu các Popup
    /// </summary>
    [HideInInspector]
    public List<CUIPopup> ListPopup = new List<CUIPopup>();

    /// <summary>
    /// Lưu danh sách các quảng cáo, event
    /// </summary>
    [HideInInspector]
    public List<Announcement> ListAnnouncement = new List<Announcement>();

    /// <summary>
    /// Danh sách nội dung phần Help được response từ server.
    /// "title": lấy ra tiêu đề
    /// "content": lấy ra nội dung
    /// </summary>
    [HideInInspector]
    public List<Hashtable> ListHelp = new List<Hashtable>();


    /// <summary>
    /// Danh sách nội dung phần Policy được response từ server.
    /// "Policy": danh sách policy
    /// "content": lấy ra nội dung
    /// </summary>
    [HideInInspector]
    public PolicyModel PolicyModel = new PolicyModel();

    /// <summary>
    /// Sự kiện khi người chơi tạo lại thông tin kết nối server mới
    /// </summary>
    [HideInInspector]
    public event CallBackFunction EventServerCreateInstance;

    [HideInInspector]
    public ApplicationStart applicationStart;

    /// <summary>
    /// Danh sách các tin nhắn hệ thống
    /// </summary>
    [HideInInspector]
    public List<Messages> ListMessageSystem = new List<Messages>();

    /// <summary>
    /// Danh sách cước ù
    /// </summary>
    [HideInInspector]
    public List<CuocUXuong> ListCuocU = new List<CuocUXuong>();

    /// <summary>
    /// Danh sách các game hiện tại esimo đang có
    /// </summary>
    [HideInInspector]
    public List<GameLogoModel> ListGameLogo = new List<GameLogoModel>(); 
    /// <summary>
    /// Cấu hình game ở client
    /// </summary>
    [HideInInspector]
    public static SettingGame Setting
    {
        get { return Instance._setting; }
        set { Instance._setting = value; }
    }

    SettingGame _setting = new SettingGame();

    /// <summary>
    /// Cấu hình List recharge trả về từ server
    /// </summary>
    [HideInInspector]
    public event CallBackFunction ReadedMessageCallBack;
    public List<RechargeModel> ListRechargeModel = new List<RechargeModel>();

    #region BASIC
    /// <summary>
    /// Lấy về Instance của GameManager
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = GameObject.Find("____GameManager");
                if (obj == null)
                {
                    //obj = new GameObject("____GameManager", typeof(GameManager));
                    obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/GameManager"));
                    obj.name = "____GameManager";
                    DontDestroyOnLoad(obj);
                }
                _instance = obj.gameObject.GetComponent<GameManager>();
                _instance.Init();
            }
            return _instance;
        }
    }
  
    public static bool IsExist { get { return _instance != null; } }

    /// <summary>
    /// Hàm chạy ngay khi GameManager được khởi tạo
    /// </summary>
    void Init()
    {
#if UNITY_WEBPLAYER
        if (!Security.PrefetchSocketPolicy(CServer.HOST_NAME, System.Convert.ToInt32(CServer.PORT), 999))
            Debug.LogError("Security Exception. Policy file load failed!");
#elif UNITY_ANDROID
        GCM.Register(GCM.SENDER_IDS);
#elif UNITY_IPHONE
		EtceteraBinding.registerForRemoteNotifcations( P31RemoteNotificationType.Alert | P31RemoteNotificationType.Badge | P31RemoteNotificationType.Sound );
#endif
        Debug.LogWarning("GameManager: Init()");
        mInfo = new User();
        channelRoom = new RoomInfo();
        currentRoom = new RoomInfo();
        selectedChannel = new RoomInfo();
        selectedLobby = new RoomInfo();
        
        Setting.Init();
        applicationStart = new ApplicationStart();
		#if UNITY_ANDROID || UNITY_IPHONE
		if(!FB.IsLoggedIn)
			FB.Init(onInitComplete, onHideUnity);
		#endif
    }
	private void onHideUnity(bool isUnityShown)
	{
		
	}

	[HideInInspector]
	public bool isInit = false;

	private void onInitComplete()
	{
		isInit = true;
		Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
		//        SocialCommon.Instance.LoginFaceBook(cbSavePass.isChecked);
	}
    void OnDestroy()
    {
		#if UNITY_IPHONE
			EtceteraManager.remoteNotificationReceivedAtLaunchEvent -= remoteNotificationAtLauch;
		    EtceteraManager.remoteNotificationReceivedEvent -= remoteNotification;
			EtceteraManager.remoteRegistrationSucceededEvent -= remoteRegistrationSucceeded;
			EtceteraManager.remoteRegistrationFailedEvent -= remoteRegistrationFailed;
		#elif UNITY_ANDROID
			GCMReceiver._onRegistered -= remoteRegistrationSucceeded;
			GCMReceiver._onUnregistered -= remoteRegistrationFailed;
			GCMReceiver._onMessage -= gcmReceiverMessage;
			if (GameObject.Find("GCMReceiver") != null)
			{
			    GameObject.Destroy(GameObject.Find("GCMReceiver"));
			}
		#endif
		
        Debug.LogWarning(this.GetType().Name + ": OnDestroy");
        if(_mServer != null)
            _mServer.OnDestroy();
        //_mServer.DoLogOut();
        _instance = null;
        StoreGame.Remove(StoreGame.EType.SAVE_ACCESSTOKEN);
    }

    public void ServerIsNull()
    {
        _mServer = null;
    }
    public static CServer Server
    {
        get
        {
            if (Instance._mServer == null)
            {
                Instance._mServer = new CServer();
                if (_instance.EventServerCreateInstance != null)
                    _instance.EventServerCreateInstance();
            }

            return Instance._mServer;
        }
    }

    void FixedUpdate()
    {
        if (_instance != null)
            Server.Engine.Dispatch();
    }

    /// <summary>
    /// Khi thoát ứng dụng (tắt ứng dụng).
    /// </summary>
    void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
            Setting.IsFullScreen = true;

        Application.runInBackground = true;

        //cu 3s kiem tra connect internet mot lan
        //StartCoroutine(checkConnection());

		#if UNITY_IPHONE
			EtceteraManager.remoteNotificationReceivedAtLaunchEvent += remoteNotificationAtLauch;
			EtceteraManager.remoteNotificationReceivedEvent += remoteNotification;
			EtceteraManager.remoteRegistrationSucceededEvent += remoteRegistrationSucceeded;
			EtceteraManager.remoteRegistrationFailedEvent += remoteRegistrationFailed;
			if(EtceteraBinding.getBadgeCount() != 0)
				EtceteraBinding.setBadgeCount (0);
        #elif UNITY_ANDROID
            GCMReceiver._onRegistered += remoteRegistrationSucceeded;
            GCMReceiver._onUnregistered += remoteRegistrationFailed;
            GCMReceiver._onMessage += gcmReceiverMessage;
		#endif



    }
    #endregion

	void remoteRegistrationFailed( string error )
	{
		Debug.Log( "remoteRegistrationFailed : " + error );
	}

	[HideInInspector]
	public string deviceToken = "";
	void remoteRegistrationSucceeded( string deviceToken )
    {
        
        if (CurrentScene != ESceneName.LoginScreen)
        {
            EsObject eso = Utility.SetEsObject("updateDeviceToken", new object[] { "deviceToken", deviceToken, "platform", PlatformSetting.GetSamplePlatform.ToString() });
            Server.DoRequestPlugin(eso);
        }
        this.deviceToken = deviceToken;
		ServerWeb.StartThread(ServerWeb.URL_REQUEST_SAVE_ACCESSTOKEN, new object[] { ServerWeb.PARAM_DEVICE_TOKEN, deviceToken, ServerWeb.PARAM_PLATFORM, PlatformSetting.GetSamplePlatform.ToString() }, delegate(bool isDone, WWW response, IDictionary json)
        {
			if(isDone)
			{

			}
		});
    }
  
	#if UNITY_IPHONE
	/// <summary>
	/// Remotes the notification. 
	/// Dictionary co 3 key : badge la so hien thi o ngoai icon  , sound , alert la message duoc gui tu server 
	/// </summary>
	/// 
	/// <param name="obj">Object.</param>
	void remoteNotification (IDictionary obj)
	{

		//IDictionary aps = obj["aps"];
		   
		if (obj.Contains ("message")) {
			string title = "Thông báo từ hệ thống";  
			string message = obj ["message"].ToString ();
			if (obj.Contains ("title"))
					title = obj ["title"].ToString ();
			if (obj.Contains ("action")) {
					string action = obj ["action"].ToString ();
					string url = obj ["url"].ToString ();
					NotificationView.ShowConfirm (title, message, delegate() {
							Application.OpenURL (url);
							Debug.Log ("open url");
					}, null);
			} else {
					ServerMessagesView.MessageServer (title, message, 5f);
			}
		}

	}
	public void remoteNotificationAtLauch(IDictionary obj )
	{
		
		if (obj.Contains ("message")) {
			string title = "Thông báo từ hệ thống";  
			string message = obj ["message"].ToString ();
			if (obj.Contains ("title"))
				title = obj ["title"].ToString ();
			if (obj.Contains ("action")) {
				string action = obj ["action"].ToString ();
				string url = obj ["url"].ToString ();
				NotificationView.ShowConfirm (title, message, delegate() {
					Application.OpenURL (url);
					Debug.Log ("open url");
				}, null);
			} else {
				ServerMessagesView.MessageServer (title, message, 5f);
			}
		}
		EtceteraBinding.setBadgeCount (0);

	}
#elif UNITY_ANDROID
    private void gcmReceiverMessage(Dictionary<string, object> obj)
    {
        if (obj.ContainsKey ("message")) {
			string title = "Thông báo từ hệ thống";  
			string message = obj ["message"].ToString ();
            if (obj.ContainsKey("title"))
				title = obj ["title"].ToString ();
            if (obj.ContainsKey("action"))
            {
				string action = obj ["action"].ToString ();
				string url = obj ["url"].ToString ();
				NotificationView.ShowConfirm (title, message, delegate() {
					Application.OpenURL (url);
					Debug.Log ("open url");
				}, null);
			} else {
				ServerMessagesView.MessageServer (title, message, 5f);
			}
		}
    }
#endif
    #region CHANGE SCENE, GET SCENES...
    ESceneName _oldScene = ESceneName.LoginScreen;
    public static ESceneName OldScene
    {
        get { return Instance._oldScene; }
    }

    ESceneName _currentScene = ESceneName.LoginScreen;
    public static ESceneName CurrentScene
    {
        get { return Instance._currentScene; }
    }

    public static void LoadScene(ESceneName scene)
    {
        Instance.StartCoroutine(Instance._LoadScene(scene));
    }

    IEnumerator _LoadScene(ESceneName scene)
    {
        if (GameManager.Setting.AllowLockScreen)
        {
            //dont allow sleep all sences
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
           
            //if (scene == ESceneName.Gameplay)
            //    Screen.sleepTimeout = SleepTimeout.NeverSleep;
            //else if (Screen.sleepTimeout != SleepTimeout.SystemSetting)
            //    Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
        
        _oldScene = _currentScene;
        _currentScene = scene;
        Debug.LogWarning("Loading Scene: " + scene.ToString());

        //AsyncOperation async = Application.LoadLevelAsync(scene.ToString());

        Application.LoadLevel(scene.ToString());

        yield return new WaitForFixedUpdate();

        if (scene == ESceneName.LoginScreen)
            GameObject.Destroy(HeaderMenu.Instance.gameObject);
        if (scene == ESceneName.GameplayChan || scene == ESceneName.LoginScreen)
        {
            GPChatView.listMessage.Clear();
            BroadcastView.Destroy();
        }
        HeaderMenu.Instance.isClickedBtnBack = false;
        System.GC.Collect();
        PlaySameDevice.ClearCache();
        TournamentInfo item = GameManager.Instance.currentTournamentInfo;
    }

    /// <summary>
    /// Gửi lệnh Ping lên server để xác định là còn đang chơi (Chỉ dành cho iOS)
    /// </summary>
    void Ping()
    {
        if (Setting.IsPingRequire == false) return;

        Server.DoRequestPlugin(Utility.SetEsObject("ping"));
    }
    #endregion

    #region Camera
    static GameObject nGUICamera;
    public static bool IsExistGUICamera
    {
        get
        {
            if (nGUICamera == null)
                nGUICamera = GameObject.FindGameObjectWithTag("GUICamera");
            if (nGUICamera != null)
                return false;
            return true;
        }
    }

    public static Camera GetGUICamera
    {
        get
        {
            if (nGUICamera == null)
                nGUICamera = GameObject.FindGameObjectWithTag("GUICamera");
            return nGUICamera.GetComponent<Camera>();
        }
    }
    #endregion
	#region When Application Pause run on ios and android 
	#if UNITY_IPHONE || UNITY_ANDROID
	void OnApplicationPause(bool pauseStatus) {
		string status = "active";
		if(pauseStatus){
			status = "pause";
        }
		if (CurrentScene != ESceneName.LoginScreen) {
			EsObject eso = Utility.SetEsObject ("changeApplicationStatus", new object[]{"status",status});
			Server.DoRequestPlugin (eso);
		#if UNITY_IPHONE
		if(EtceteraBinding.getBadgeCount() != 0)
			EtceteraBinding.setBadgeCount (0);
		#endif
         
		}
	}
	#endif
	#endregion
    #region PROCESS BUTTON BACK IN ANDROID
    bool dialogIsShowing = false;
    void Update()
    {
        if (Common.IsExistsEscape == false) return;

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (ListPopup.Count > 0) { ListPopup[ListPopup.Count - 1].buttonClose(null); return; }
            if (HeaderMenu.IsExist && HeaderMenu.Instance.IsHidden == false) { HeaderMenu.Instance.IsHidden = true; }
            else if (CurrentScene == ESceneName.ChannelChan) { HeaderMenu.Instance.OnClickButtonLogout(null); return; }
            else if (CurrentScene == ESceneName.LoginScreen)
            {
                GameObject obj = GameObject.Find("LoginScreen Code");
                LoginScreenView loginView = obj.GetComponent<LoginScreenView>();
                if (!loginView.IsFormRegisterHide) { loginView.IsFormRegisterHide = true; return; }
                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WindowsPlayer)
                    Application.Quit();

                return;
            }
            else { HeaderMenu.Instance.OnClickButtonBack(null); return; }
        }
	}
    #endregion
	void OnApplicationQuit (){
		PlaySameDevice.Clear ();
	}
    //IEnumerator checkConnection()
    //{
    //    WWW www = new WWW(ServerWeb.URL_PING);
    //    yield return www;
    //    if (!www.isDone || !string.IsNullOrEmpty(www.error))
    //    {
    //        if (!dialogIsShowing)
    //        {
    //            ServerMessagesView.MessageServer("Thông báo", "Bạn bị mất kết nối internet!", delegate()
    //            {
    //                dialogIsShowing = false;
    //            }, "Thử lại");
    //            dialogIsShowing = true;
    //            Server.WhenDisconnect();
    //        }
    //        float timePing = 10f;
    //        if (GameManager.Setting.Platform.GetConfigByType(PlatformType.ping_interval) != null)
    //            timePing = float.Parse(GameManager.Setting.Platform.GetConfigByType(PlatformType.ping_interval).Value);
    //        yield return new WaitForSeconds(timePing);// trying again after 2 sec
    //        StartCoroutine(checkConnection());
    //    }
    //    else
    //    {
    //        if (dialogIsShowing)
    //        {
    //            ServerMessagesView.Instance.hideNotification();
    //            dialogIsShowing = !dialogIsShowing;
    //        }
    //        float timePing = 10f;
    //        if (GameManager.Setting.Platform.GetConfigByType(PlatformType.ping_interval) != null)
    //            timePing = float.Parse(GameManager.Setting.Platform.GetConfigByType(PlatformType.ping_interval).Value);
    //        yield return new WaitForSeconds(timePing);// recheck if the internet still exists after 5 sec
    //        StartCoroutine(checkConnection());
    //    }
    //}
    #region CALLBACK METHOD
    public void FunctionDelay(CallBackFunction callback, float delay)
    {
        StartCoroutine(_FunctionDelay(callback, delay));
    }
    IEnumerator _FunctionDelay(CallBackFunction callback, float delay)
    {
        yield return new WaitForSeconds(delay);
        callback();
    }
    #endregion

    /// <summary>
    /// Thông tin Setting trên mỗi client
    /// </summary>
    public class SettingGame
    {
        /// <summary>
        /// Các cấu hình theo platform
        /// </summary>
        public PlatformSetting Platform;

        /// <summary>
        /// Không cho phép sử dụng touch trên các thiết bị Mobile (sử dụng chuột)
        /// </summary>
        public bool IsDisableTouch = false;

        /// <summary>
        /// Thông tin thông báo
        /// </summary>
        public string BroadcastMessage = "";

        /// <summary>
        /// Là lần đầu tiên đăng nhập vào game sau khi đăng ký tài khoản
        /// </summary>
        public bool IsFirstLogin = false;

        /// <summary>
        /// Là phải cập nhật, để được phép chơi game
        /// </summary>
        public string IsMustUpdate = "";

        #region Full Screen
        /// <summary>
        /// Là cho phép chế độ Full Screen
        /// </summary>

        public string Home()
        {
            return "http://www.chieuvuong.com";
        }
        public bool IsFullScreen
        {
            get { return _isFullScreen; }
            set
            {
                if (_isFullScreen == value) return;

                if (value)
                {
                    rootResolution = Screen.currentResolution;
#if UNITY_WEBPLAYER
                    Screen.SetResolution(Screen.width, Screen.height, true);
#else
                    Screen.SetResolution(960, 640, false);
#endif
                }
                else
                    Screen.SetResolution(rootResolution.width, rootResolution.height, false, rootResolution.refreshRate);

                _isFullScreen = value;

                StoreGame.SaveInt(StoreGame.EType.FULL_SCREEN, value ? (int)StoreGame.EToggle.ON : (int)StoreGame.EToggle.OFF);
            }
        }
        bool _isFullScreen = false;
        Resolution rootResolution;
        #endregion

        bool _allowLockScreen = true;
        public bool AllowLockScreen
        {
            get { return _allowLockScreen; }
            set
            {
                _allowLockScreen = value;

                if (value)
                {
                    if (CurrentScene == ESceneName.GameplayChan)
                        Screen.sleepTimeout = SleepTimeout.NeverSleep;
                }
                else
                {
                    if (Screen.sleepTimeout != SleepTimeout.SystemSetting)
                        Screen.sleepTimeout = SleepTimeout.SystemSetting;
                }
            }
        }

        #region Require Ping
        /// <summary>
        /// Yêu cầu ping từ server
        /// </summary>
        public bool IsPingRequire
        {
            get
            {
                //if (GameManager.CurrentScene != ESceneName.Gameplay)
                return false;
                //return _isPingRequire;
            }
            set
            {
                _isPingRequire = value;

                if (value)
                    GameManager.Instance.InvokeRepeating("Ping", 0f, 15f);
                else if (GameManager.Instance.IsInvoking("Ping"))
                    GameManager.Instance.CancelInvoke("Ping");
            }
        }
        bool _isPingRequire = false;
        #endregion

        #region Initialization
        /// <summary>
        /// Khởi tạo thông tin cài đặt
        /// </summary>
        public void Init()
        {
            Platform = new PlatformSetting();

            if (StoreGame.Contains(StoreGame.EType.FULL_SCREEN))
                IsFullScreen = StoreGame.LoadInt(StoreGame.EType.FULL_SCREEN) == (int)StoreGame.EToggle.ON;
            if (StoreGame.Contains(StoreGame.EType.LOCK_SCREEN))
                AllowLockScreen = StoreGame.LoadInt(StoreGame.EType.LOCK_SCREEN) == (int)StoreGame.EToggle.ON;
        }
        #endregion 
    }
    private string cookie;

    public string Cookie
    {
        get { return cookie; }
    }

    public void UpdateCookieString()
    {
        Application.ExternalEval("if (typeof window.getCookie != \"function\") { window.getCookie = function () {u.getUnity().SendMessage(\"" + gameObject.name + "\", \"OnGetCookie\", document.cookie);}}");
        Application.ExternalCall("getCookie");
    }

    void OnGetCookie(string cookie)
    {
        this.cookie = cookie;
    }

    public void ReadedMessageProfile()
    {
        if (ReadedMessageCallBack != null)
            ReadedMessageCallBack();
    }


}

