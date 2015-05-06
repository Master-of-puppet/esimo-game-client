using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;
using System;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Class xử lý view cho Header Menu
/// </summary>
public class HeaderMenu : MonoBehaviour
{
    static HeaderMenu _instance;

    #region UnityEditor
    public CUIHandle[] btMenuHide, btnListWaitingHide;
    public CUIHandle btMenuShow, btLogout, btProfile, btSetting, btSupport, btnRecharge, btnQuest, btnNew, btnBack, btnMessage, btnGameInfo, btnListWaitingShow, btnMarket;

    public UILabel lbUsername, lbLevel, lbChip, lbGold;
    public UITexture textureAvatar;
    public NumberWarning nTotalCount, nTotalProfile;
    public GameObject background;
    public GameObject AvatarView,wifiPrefab;
    public Transform formWaiting,profile;
    public Transform PanelFullInfo;
    public UIGrid tableListWaiting;

    #endregion



    public CallBackFunction OnClickButtonBackCallBack;
    [HideInInspector]
    public bool isClickedBtnBack = false;
    [HideInInspector]
    public List<PlayerWaitingView> waitingViews = new List<PlayerWaitingView>();
    #region BASIC
    /// <summary>
    /// Lấy về Instance của HeaderMenu
    /// </summary>
    public static HeaderMenu Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = GameObject.Find("__HeaderMenu");
                if (obj == null)
                {
                    obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu"));
                    obj.transform.position = new Vector3(-1000f, 1000f, 50f);
                    obj.name = "__HeaderMenu";
                }
                _instance = obj.gameObject.GetComponent<HeaderMenu>();
                _instance.Init();
                _instance.isClickedBtnBack = false;
            }
            return _instance;
        }
    }
    void Update()
    {
        if (!IsHidden && nTotalCount.gameObject.activeSelf)
        {
            nTotalCount.gameObject.SetActive(false);
        }
    }
    private bool _isHidden = true;
    public bool IsHidden
    {
        get { return _isHidden; }
        set
        {
            _isHidden = value;

            if (value)
                HideMenu();
            else
                ShowMenu();
        }
    }

    public static bool IsExist
    {
        get
        {
            if (_instance == null)
                return false;
            return UnityEngine.GameObject.Find("__HeaderMenu") != null;
        }
    }

    bool isWasInit = false;
    /// <summary>
    /// Hàm chạy ngay khi GameManager được khởi tạo
    /// </summary>
    void Init()
    {
        if (isWasInit) return;
#if UNITY_ANDROID
        btMenuShow.transform.parent.localPosition = new Vector3(0f, -157f, 0f);
#endif
        HideMenu();
        ReDraw();
        ActiveButtonRecharge();
        isWasInit = true;
        if (GameManager.CurrentScene != ESceneName.GameplayChan)
            formWaiting.gameObject.SetActive(false);
        else
        {
            formWaiting.transform.localPosition = Vector3.zero;
            formWaiting.gameObject.SetActive(true);

        }
    }
    public void LoadListWaitingView()
    {
        StartCoroutine(_LoadListWaitingView());
    }
    IEnumerator _LoadListWaitingView()
    {

        if (GameModelChan.game == null || GameModelChan.ListWaitingPlayer.Count == 0)
            yield break;
        while (waitingViews.Count > 0)
        {
            GameObject.Destroy(waitingViews[0].gameObject);
            waitingViews.RemoveAt(0);
        }
        waitingViews.Clear();
        yield return new WaitForEndOfFrame();

        foreach (PlayerControllerChan playerController in GameModelChan.ListWaitingPlayer)
        {
            PlayerWaitingView view = PlayerWaitingView.Create(playerController, tableListWaiting);
            string nameRow = "";
            if (playerController.isPriority)
                nameRow = "00";
            else
                nameRow = waitingViews.Count + playerController.username;
            view.gameObject.name = nameRow;
            waitingViews.Add(view);
        }
        tableListWaiting.Reposition();
    }
    public void RemoveWaitingPlayer(EPlayerController player)
    {
        PlayerWaitingView waitingView = waitingViews.Find(pl => pl.player.username == player.username);
        if (waitingView != null)
        {
            GameObject.Destroy(waitingView.gameObject);
            waitingViews.Remove(waitingView);
            GameManager.Instance.FunctionDelay(delegate() { tableListWaiting.Reposition(); tableListWaiting.transform.parent.GetComponent<UIScrollView>().ResetPosition(); }, 0.001f);

        }
    }
    public void AddWaitingPlayer(PlayerControllerChan player)
    {
        PlayerWaitingView view = PlayerWaitingView.Create(player, tableListWaiting);
        view.gameObject.name = waitingViews.Count + " " + player.username;
        waitingViews.Add(view);
        tableListWaiting.Reposition();
        tableListWaiting.transform.parent.GetComponent<UIScrollView>().ResetPosition();
    }
    public void ActiveButtonRecharge()
    {
        if (!GameManager.Setting.Platform.EnableRecharge)
            btnRecharge.gameObject.SetActive(false);
        else
            btnRecharge.gameObject.SetActive(true);
    }

    void Awake()
    {
        Init();
        CUIHandle.AddClick(btLogout, OnClickButtonLogout);
        CUIHandle.AddClick(btMenuShow, OnClickButtonMenu);
        CUIHandle.AddClick(btMenuHide, OnClickButtonMenuHide);
        CUIHandle.AddClick(btnMessage, OnClickButtonMessage);
        CUIHandle.AddClick(btnGameInfo, OnClickButtonGameInfo);
        CUIHandle.AddClick(btProfile, OnClickProfile);
        CUIHandle.AddClick(btSetting, OnClickSetting);
        CUIHandle.AddClick(btSupport, OnClickSupport);
        CUIHandle.AddClick(btnRecharge, OnClickRecharge);
        CUIHandle.AddClick(btnBack, OnClickButtonBack);
        CUIHandle.AddClick(btnListWaitingHide, OnClickButtonWaitingHide);
        CUIHandle.AddClick(btnListWaitingShow, OnClickButtonShowListWaiting);
        // New Function not Implement
        CUIHandle.AddClick(btnQuest, OnClickNewFuncationAlert);
        CUIHandle.AddClick(btnNew, OnClickEvent);
        CUIHandle.AddClick(btnMarket, OnClickMarket);
        GameManager.Server.EventUpdateUserInfo += OnUpdateUserInfo;
        GameManager.Server.EventConfigClientChanged += ConfigClientChangedHandler;

    }
    void Start() {
        
    }   

    private void OnClickButtonShowListWaiting(GameObject targetObject)
    {
        IsHiddenListWaiting = false;
    }

    private void OnClickButtonWaitingHide(GameObject targetObject)
    {
        IsHiddenListWaiting = true;
    }


    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnGameInfo, OnClickButtonGameInfo);
        CUIHandle.RemoveClick(btLogout, OnClickButtonLogout);
        CUIHandle.RemoveClick(btMenuShow, OnClickButtonMenu);
        CUIHandle.RemoveClick(btProfile, OnClickProfile);
        CUIHandle.RemoveClick(btSetting, OnClickSetting);
        CUIHandle.RemoveClick(btSupport, OnClickSupport);
        CUIHandle.RemoveClick(btnRecharge, OnClickRecharge);
        CUIHandle.RemoveClick(btnBack, OnClickButtonBack);
        CUIHandle.RemoveClick(btnMessage, OnClickButtonMessage);
        CUIHandle.RemoveClick(btnListWaitingHide, OnClickButtonWaitingHide);
        CUIHandle.AddClick(btnListWaitingShow, OnClickButtonShowListWaiting);
        // New Function not Implement
        CUIHandle.RemoveClick(btnQuest, OnClickNewFuncationAlert);
        CUIHandle.RemoveClick(btnNew, OnClickEvent);
        CUIHandle.RemoveClick(btnMarket, OnClickMarket);
        if (!GameManager.IsExist) return;

        GameManager.Server.EventUpdateUserInfo -= OnUpdateUserInfo;
        GameManager.Server.EventConfigClientChanged -= ConfigClientChangedHandler;
    }




    #endregion

    private void ConfigClientChangedHandler(IDictionary value)
    {
        ActiveButtonRecharge();
    }

    #region  OnClick
    private void OnClickButtonGameInfo(GameObject targetObject)
    {
        if (GameManager.CurrentScene == ESceneName.GameplayChan)
            GPGameManagerChan.Create();
        else if (GameManager.CurrentScene == ESceneName.GameplayTLMN)
            GPGameManagerTLMN.Create();
        else if (GameManager.CurrentScene == ESceneName.GameplayPhom)
            GPGameManagerPhom.Create();
    }
    private void OnClickNewFuncationAlert(GameObject targetObject)
    {
        NotificationView.ShowMessage("Tính năng đang được phát triển mời bạn quay lại sau ", 3f);
    }

    void OnClickEvent(GameObject targetObject)
    {
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_EDITOR
        EventView.Create();
#else
        NotificationView.ShowMessage("Bạn có thể xem thêm nhiều tin tức sự kiện tại trang chủ https://chieuvuong.com");
#endif
    }
    private void OnClickButtonMessage(GameObject targetObject)
    {
        StoreGame.SaveString(StoreGame.EType.SEND_FRIEND_MESSAGE, "");
        ProfileView.Instance.CheckWhenStart();
    }
    public void OnClickButtonBack(GameObject targetObject)
    {
        if (!isClickedBtnBack)
        {
            if (OnClickButtonBackCallBack != null)
                OnClickButtonBackCallBack();
            if (GameManager.CurrentScene == ESceneName.GameplayChan || GameManager.CurrentScene == ESceneName.HallSceen || GameManager.CurrentScene == ESceneName.GameplayPhom || GameManager.CurrentScene == ESceneName.GameplayTLMN)
                isClickedBtnBack = false;
            else
                isClickedBtnBack = true;
        }
    }
    void OnClickMarket(GameObject targetObject)
    {
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_EDITOR
        MarketView.Create();
#else
        NotificationView.ShowMessage("Phiên bản window không hỗ trợ tính năng này");
#endif
    }
    public void OnClickButtonLogout(GameObject go)
    {
        NotificationView.ShowConfirm("Lưu ý", "Bạn có chắc chắn muốn đăng xuất ? ", delegate() { GameManager.Server.DoLogOut(); }, null, "Đồng ý", "Hủy");
    }

    void OnUpdateUserInfo()
    {
        lbChip.text = Utility.Convert.Chip(GameManager.Instance.mInfo.chip);
    }

    void OnClickButtonMenu(GameObject go)
    {
        ShowMenu();
    }

    void OnClickButtonMenuHide(GameObject go)
    {
        HideMenu();
    }

    void OnClickProfile(GameObject go)
    {
        //GameManager.Server.DoRequestPlugin(Utility.SetEsObject("getLevel", new object[]{
        //        "appId", GameManager.Instance.hallRoom.gameId
        //    }));
        StoreGame.Remove(StoreGame.EType.SEND_FRIEND_MESSAGE);
        ProfileView.Instance.CheckWhenStart();
        HideMenu();
      
    }

    void OnClickSetting(GameObject go)
    {
        SettingView.Create();
        HideMenu();
    }

    void OnClickSupport(GameObject go)
    {
        SupportView.Create();
        HideMenu();
    }

    void OnClickRecharge(GameObject go)
    {
        RechargeView.Create();
    }

    //     void OnClickCommunity(GameObject go)
    //     {
    //         Common.VersionNotSupport("Cộng Đồng");
    //     }
    #endregion

    #region Show Hide Header Menu
    void ShowMenu()
    {
        StartCoroutine(_ShowMenu());

    }
    System.Collections.IEnumerator _ShowMenu()
    {
        _isHidden = false;
        Vector3 vectorsShow = new Vector3(54f, background.transform.localPosition.y, -1f);


        TweenPosition.Begin(background, 0.1f, vectorsShow);


        if (nTotalCount.gameObject.activeSelf)
        {
            nTotalCount.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.2f);
        Array.ForEach<CUIHandle>(btMenuHide, i =>
        {
            if (!i.gameObject.activeSelf)
            {
                i.gameObject.SetActive(true);
            }
        });
        btMenuShow.gameObject.SetActive(false);
    }
    /// <summary>
    /// Hien thi list menu waiting
    /// </summary>
    bool _isHiddenListWaiting = true;
    public bool IsHiddenListWaiting
    {
        get { return _isHiddenListWaiting; }
        set
        {
            _isHiddenListWaiting = value;
            if (value)
                HideListWaiting();
            else
                ShowListWaiting();
        }
    }
    void HideListWaiting()
    {
        StartCoroutine(_HideListWaiting());
    }
    IEnumerator _HideListWaiting()
    {
        Vector3 vectorsHide = new Vector3(0f, 0f, -1f);
        TweenPosition.Begin(formWaiting.gameObject, 0.1f, vectorsHide);
        yield return new WaitForSeconds(0.1f);
        Array.ForEach<CUIHandle>(btnListWaitingHide, i => { if (i.gameObject.activeSelf) { i.gameObject.SetActive(false); } });
        btnListWaitingShow.gameObject.SetActive(true);
    }
    void ShowListWaiting()
    {
        StartCoroutine(_ShowListWaiting());
    }
    IEnumerator _ShowListWaiting()
    {
        _isHiddenListWaiting = false;
        Vector3 vectorsShow = new Vector3(-200f, 0f, -1f);
        TweenPosition.Begin(formWaiting.gameObject, 0.1f, vectorsShow);
        yield return new WaitForSeconds(0.1f);
        Array.ForEach<CUIHandle>(btnListWaitingHide, i =>
        {
            if (!i.gameObject.activeSelf)
            {
                i.gameObject.SetActive(true);
            }
        });
        btnListWaitingShow.gameObject.SetActive(false);
    }
    void HideMenu()
    {
        StartCoroutine(_HideMenu());
    }
    System.Collections.IEnumerator _HideMenu()
    {
        Vector3 vectorsHide = new Vector3(-55f, background.transform.localPosition.y, -1f);
        TweenPosition.Begin(background, 0.1f, vectorsHide);
        yield return new WaitForSeconds(0.1f);
        Array.ForEach<CUIHandle>(btMenuHide, i => { if (i.gameObject.activeSelf) { i.gameObject.SetActive(false); } });
        btMenuShow.gameObject.SetActive(true);
        ReDraw();
        _isHidden = true;
        nTotalCount.gameObject.SetActive(true);
    }

    public void ReDraw()
    {
        if (GameManager.CurrentScene == ESceneName.ChannelLeague && GameManager.Instance.displayTournamentMenu || GameManager.CurrentScene == ESceneName.Tournament && GameManager.Instance.displayTournamentMenu)
        {
            NGUITools.SetActive(AvatarView, false);
            NGUITools.SetActive(btnNew.gameObject, false);
            NGUITools.SetActive(btnQuest.gameObject, false);
            if (GameManager.CurrentScene == ESceneName.ChannelLeague)
                NGUITools.SetActive(btnBack.gameObject, false);
            NGUITools.SetActive(btnMarket.gameObject, false);
            NGUITools.SetActive(btnRecharge.gameObject, false);
            NGUITools.SetActive(wifiPrefab, false);
        }
        else
        {
            if (GameManager.CurrentScene == ESceneName.ChannelLeague)
            {
                NGUITools.SetActive(AvatarView, true);
                NGUITools.SetActive(btnNew.gameObject, true);
                NGUITools.SetActive(btnQuest.gameObject, true);
                NGUITools.SetActive(btnBack.gameObject, true);
                NGUITools.SetActive(btnMarket.gameObject, true);
                ActiveButtonRecharge();
                NGUITools.SetActive(wifiPrefab, true);
            }
            else
            {
                if (GameManager.CurrentScene == ESceneName.HallSceen)
                {
                    profile.gameObject.SetActive(false);
                    btnQuest.gameObject.SetActive(false);
                }

                if (GameManager.CurrentScene == ESceneName.LoginScreen)
                    btnBack.gameObject.SetActive(false);
                else if (GameManager.CurrentScene == ESceneName.GameplayChan || GameManager.CurrentScene == ESceneName.CreateRoomChan || GameManager.CurrentScene == ESceneName.GameplayPhom || GameManager.CurrentScene == ESceneName.CreateRoomPhom || GameManager.CurrentScene == ESceneName.GameplayTLMN || GameManager.CurrentScene == ESceneName.CreateRoomTLMN || GameManager.CurrentScene == ESceneName.HallSceen)
                {
                    btnBack.gameObject.SetActive(true);
                    AvatarView.SetActive(false);
                    if (GameManager.CurrentScene == ESceneName.GameplayChan)
                    {
                        btnGameInfo.transform.parent.gameObject.SetActive(true);
                        btnGameInfo.gameObject.SetActive(true);
                        btnNew.gameObject.SetActive(false);
                        btnNew.transform.parent.GetComponent<UIGrid>().Reposition();
                    }

                    if (GameManager.CurrentScene == ESceneName.GameplayPhom || GameManager.CurrentScene == ESceneName.GameplayTLMN)
                    {
                        btnGameInfo.transform.parent.gameObject.SetActive(true);
                        btnGameInfo.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (!AvatarView.activeSelf)
                        AvatarView.SetActive(true);
                    if (!btnBack.gameObject.activeSelf)
                        btnBack.gameObject.SetActive(true);
                }

                if (GameManager.CurrentScene == ESceneName.ChannelChan || GameManager.CurrentScene == ESceneName.HallSceen || GameManager.CurrentScene == ESceneName.ChannelPhom || GameManager.CurrentScene == ESceneName.ChannelTLMN)
                {
                    AvatarView.SetActive(false);
                    DrawFullPlayerInfo(PanelFullInfo);
                }

            }
        }
        UpdateUserInfo();
    }

    public void UpdateUserInfo()
    {
        lbUsername.text = Utility.Convert.ToTitleCase(GameManager.Instance.mInfo.username.Length > 20 ? GameManager.Instance.mInfo.username.Substring(0, 20) + "..." : GameManager.Instance.mInfo.username);
        lbChip.text = Utility.Convert.Chip(GameManager.Instance.mInfo.chip);
        lbGold.text = Utility.Convert.Chip(GameManager.Instance.mInfo.gold);
        lbLevel.text = "LV " + GameManager.Instance.mInfo.level;
        GameManager.Instance.mInfo.AvatarTexture(delegate(Texture _texture) { if (textureAvatar != null) textureAvatar.mainTexture = _texture; });
        if (GameManager.PlayGoldOrChip == "gold")
        {
            lbGold.gameObject.SetActive(true);
            lbChip.gameObject.SetActive(false);
        }
        else
        {
            lbGold.gameObject.SetActive(false);
            lbChip.gameObject.SetActive(true);
        }
    }
    #endregion

    public void DrawFullPlayerInfo(Transform parent)
    {
        GameObject obj = GameObject.Find("__GroupInfoGoldAndChip");
        if (obj == null)
        {
            obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/GroupInfoGoldAndChip"));
            obj.name = "__GroupInfoGoldAndChip";
            obj.transform.parent = parent;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
        }

    }
}

