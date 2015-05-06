using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Electrotank.Electroserver5.Api;

public delegate void DelegateOnBettingValueChange(EsObject eso);

public class GameplayChan : MonoBehaviour
{
    #region Unity Editor
    public Playmat mPlaymat;
    public Transform transformParentButton;

    public CUIHandle btReady, btStart;
    public CUIHandle btStealCard, btSorted, btDraw, btView, btFulllaying, btnBetting;
    public GameObject timeChiu;

    public UIDisableButton btDiscard, btChiu, btDuoi;
    public GameObject deck;
    public UILabel lbDiscard;
    public GameObject chicken;
    public ChickenNotification notification;
    public GameObject infoBanChoi;
    public LevelInformationView levelInforView;
    public UISprite iconBettingUp;
    const float timeToCheck = 5;
    float timeCheck = timeToCheck;

    public Transform[] playerSlot;
    #endregion

    [HideInInspector]
    public bool isViewCompleted = false;
    [HideInInspector]
    public CardControllerChan cardController = new CardControllerChan();

    [HideInInspector]
    public GameplayDealCardEffect dealCardEffect = new GameplayDealCardEffect();

    /// <summary>
    /// Xử lý button gameplay
    /// </summary>
    [HideInInspector]
    public YourButtonControllerChan button = new YourButtonControllerChan();

    /// <summary>
    /// Listener lắng nghe các Event trong gameplay
    /// </summary>
    [HideInInspector]
    public GameplayListener Listener = new GameplayListener();

    /// <summary>
    /// Class xử lý show thông tin khi có người chơi Ù
    /// </summary>
    [HideInInspector]
    public GameplayFullLaying fullLayingEffect = new GameplayFullLaying();

    /// <summary>
    /// Tổng kết game khi kết thúc trận đấu
    /// </summary>
    [HideInInspector]
    public List<Summary> summaryGame = new List<Summary>();

    /// <summary>
    /// Thống kê những card có thể gửi được.
    /// </summary>
    [HideInInspector]
    public List<GiveCard> listGiveCard = new List<GiveCard>();

    /// <summary>
    /// Danh sách các quân còn trong Nọc
    /// </summary>
    [HideInInspector]
    public List<int> listCardInNoc = new List<int>();

    /// <summary>
    /// Danh sách card đã báo Ù
    /// </summary>
    [HideInInspector]
    public EsObject[][] listFullLaying = new EsObject[3][];

    /// <summary>
    /// Người chiến thắng
    /// </summary>
    [HideInInspector]
    public PlayerControllerChan winner;
    /// <summary>
    /// Biến cờ , để kiểm tra xem lúc người chơi thoát ra lúc đang "pickPlayer" , "firstPlayer" , để bỏ các sự kiện slotChoice
    /// </summary>
    private bool isDealing = false;
    int cardU = 0;
    public event DelegateOnBettingValueChange OnBettingValueCallBack;
    #region Time Count Down
    float _timeCountDownTurm = 0;
    public float TimeCountDown
    {
        get { return _timeCountDownTurm; }
        set
        {
            _timeCountDownTurm = value;
            GameModelChan.GetPlayer(GameModelChan.IndexInTurn).cuiPlayer.StartTime(_timeCountDownTurm);
        }
    }
    public void StartTimeRemaining(float totalTime, float remainingTime)
    {
        _timeCountDownTurm = totalTime;
        GameModelChan.GetPlayer(GameModelChan.IndexInTurn).cuiPlayer.StartRemainingTime(remainingTime);
    }

    private NumberCountDown NumberCountDownObj;
    public void StartTimeAutoDeal(float time)
    {
        if (NumberCountDownObj == null || NumberCountDownObj.gameObject == null)
            NumberCountDownObj = NumberCountDown.Create(time, transformParentButton);
        else
            NumberCountDownObj.StartCountDown(time);
    }
    void WhenDealCard()
    {
        if (NumberCountDownObj != null)
        {
            GameObject.Destroy(NumberCountDownObj.gameObject);
            NumberCountDownObj = null;
        }
        if (btStart.gameObject.activeSelf)
            btStart.gameObject.SetActive(false);
    }
    #endregion

    /// <summary>
    /// Có thể bắt đầu trận đấu
    /// </summary>
    public bool ClickStartGame
    {
        set
        {
            _canStartGame = value;

            if (value == false)
                WhenDealCard();
            else
            {
                btStart.gameObject.SetActive(GameModelChan.YourController != null && GameModelChan.YourController.isMaster && GameModelChan.CurrentState == GameModelChan.EGameState.waitingForReady);
            }
        }

        get { return _canStartGame; }
    }
    private bool _canStartGame = false;

    /// <summary>
    /// Click Card object
    /// </summary>
    GameObject clickCard = null;
    /// <summary>
    /// Click card object khi người chơi hạ phỏm
    /// </summary>
    List<ChanCard> clickCardList = new List<ChanCard>();

    /// <summary>
    /// Có thể báo ăn cây người trước đánh ra 
    /// (Server sẽ trả về true khi có thể)
    /// </summary>
    [HideInInspector]
    public bool stolen = false;

    /// <summary>
    /// Có thể báo ù 
    /// (Server sẽ trả về true khi có thể)
    /// </summary>
    [HideInInspector]
    public bool fullLaying = false;

    /// <summary>
    /// Có gì thay đổi không thì gửi lại list Sort Card 
    /// (Trả về true khi có thay đổi số lượng bài chơi)
    /// </summary>
    [HideInInspector]
    public bool canRequestSort = true;

    /// <summary>
    /// Danh sách list có thể sắp xếp
    /// (Sẽ nhận được giá trị sau khi Request đến Server)
    /// </summary>
    [HideInInspector]
    public List<List<int>> sortList = new List<List<int>>();

    /// <summary>
    /// Các phỏm có thể hạ
    /// (Sẽ nhận được giá trị sau khi Request đến Server)
    /// </summary>
    [HideInInspector]
    public List<List<int>> meldList = new List<List<int>>();

    [HideInInspector]
    public Dictionary<string, bool> dicUserBetting = new Dictionary<string, bool>();
    bool isStartDone = false;
    void Start()
    {
        Debug.LogWarning("Starting Gameplay");
        GameManager.Server.ProcessStackWaiting();
        if (btStart.gameObject.activeSelf)
            btStart.gameObject.SetActive(false);
        deck.SetActive(false);
        isStartDone = true;
        Debug.LogWarning("Started Gameplay");
    }

    void Awake()
    {
        GameModelChan.model = new GameModelChan(this);

        HeaderMenu.Instance.OnClickButtonBackCallBack = delegate()
        {
            if (GameObject.Find("__Prefab Quit Gameplay") == null)
                GPQuitChan.Create();
        };

        UIDisableButton.AddClick(btDiscard, OnClickDiscard);
        UIDisableButton.AddClick(btChiu, OnClickChiu);
        UIDisableButton.AddClick(btDuoi, OnClickSkip);

        CUIHandle.AddClick(btView, OnClickView);
        CUIHandle.AddClick(btSorted, OnClickSort);
        CUIHandle.AddClick(btDraw, OnClickDraw);
        CUIHandle.AddClick(btnBetting, OnClickBetting);

        CUIHandle.AddClick(btReady, OnClickReady);
        CUIHandle.AddClick(btStart, OnClickStart);

        CUIHandle.AddClick(btStealCard, OnClickSteadCard);
        CUIHandle.AddClick(btFulllaying, OnClickFulllaying);

        GameManager.Server.EventPluginMessageOnProcess += ProcessOnGameplay;
        GameManager.Server.EventLeaveRoom += OnLeaveRoom;
        GameplayDealCardEffect.EventResponseMessage += ShowMessageBocCai;
        // GameManager.Server.EventConfigClientChanged += ActiveButtonRecharge;

    }

    private void OnJoinRoomSuccess(JoinRoomEvent e)
    {

        if (e.RoomId == GameManager.Instance.selectedChannel.roomId && e.ZoneId == GameManager.Instance.selectedChannel.zoneId)
        {
            GameManager.LoadScene(ESceneName.LobbyChan);
            GameManager.Server.doJoiningRoom = null;
        }
        else
            GameManager.LoadScene(ESceneName.ChannelChan);


    }
    private void OnClickRecharge(GameObject targetObject)
    {
        RechargeView.Create();
    }
    void OnDestroy()
    {
        GameModelChan.game = null;
        UIDisableButton.RemoveClick(btDiscard, OnClickDiscard);
        UIDisableButton.RemoveClick(btChiu, OnClickChiu);
        UIDisableButton.RemoveClick(btDuoi, OnClickSkip);

        CUIHandle.RemoveClick(btView, OnClickView);
        CUIHandle.RemoveClick(btSorted, OnClickSort);
        CUIHandle.RemoveClick(btDraw, OnClickDraw);
        CUIHandle.RemoveClick(btReady, OnClickReady);
        CUIHandle.RemoveClick(btStart, OnClickStart);
        CUIHandle.RemoveClick(btnBetting, OnClickBetting);
        CUIHandle.RemoveClick(btStealCard, OnClickSteadCard);
        CUIHandle.RemoveClick(btFulllaying, OnClickFulllaying);
        if (!GameManager.IsExist) return;

        GameManager.Server.EventPluginMessageOnProcess -= ProcessOnGameplay;
        GameManager.Server.EventLeaveRoom -= OnLeaveRoom;
        GameplayDealCardEffect.EventResponseMessage -= ShowMessageBocCai;
        GPInformationView.listMessage.Clear();
        GameManager.Server.EventJoinRoom -= OnJoinRoomSuccess;
    }

    private void OnClickBetting(GameObject targetObject)
    {
        GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("checkValidBettingGaNgoai", new object[] { "getAllBetting", true }));
        if (iconBettingUp.active)
        {
            iconBettingUp.gameObject.SetActive(false);
        }

    }
    public void HideAllOtherObjectWhenFullaying()
    {
        infoBanChoi.gameObject.SetActive(false);
        btnBetting.transform.parent.gameObject.SetActive(false);
        HeaderMenu.Instance.gameObject.SetActive(false);
        levelInforView.gameObject.SetActive(false);
    }
    public void ShowAllOtherObjectWhenFullLaying()
    {
        infoBanChoi.gameObject.SetActive(true);
        btnBetting.transform.parent.gameObject.SetActive(true);
        HeaderMenu.Instance.gameObject.SetActive(true);
        levelInforView.gameObject.SetActive(true);
    }

    #region SEND REQUEST SERVER
    public void OnQuitGame(bool quitEndGame)
    {
        if (!quitEndGame)
        {
            //Khi Ấn Quit
            WaitingView.Show("Đang thoát phòng");
            GameManager.Server.DoRequestGameAction("quitGame");
            GameManager.Server.DoLeaveCurrentRoom();
            //DoJoinRoom();
        }
    }

    void DoJoinRoom()
    {
        GameManager.Server.EventJoinRoom += OnJoinRoomSuccess;
        if (GameManager.OldScene == ESceneName.LoginScreen || GameManager.Instance.mInfo.chip < ((ChannelChan)GameManager.Instance.selectedChannel).minimumMoney)
            GameManager.Server.DoJoinRoom(GameManager.Instance.channelRoom.zoneId, GameManager.Instance.channelRoom.roomId);
        else
            GameManager.Server.DoJoinRoom(GameManager.Instance.selectedChannel.zoneId, GameManager.Instance.selectedChannel.roomId);
        if (GameManager.PlayGoldOrChip == "gold")
        {
            if (GameManager.OldScene == ESceneName.LoginScreen || GameManager.Instance.mInfo.gold < ((ChannelChan)GameManager.Instance.selectedChannel).minimumMoney)
                GameManager.Server.DoJoinRoom(GameManager.Instance.channelRoom.zoneId, GameManager.Instance.channelRoom.roomId);
            else
                GameManager.Server.DoJoinRoom(GameManager.Instance.selectedChannel.zoneId, GameManager.Instance.selectedChannel.roomId);
        }
    }

    void OnLeaveRoom(LeaveRoomEvent e)
    {
        //Debug.Log("attemping to leave room: " + e.RoomId + "@" + e.ZoneId + ", ");
        if (e.RoomId == GameManager.Instance.selectedLobby.roomId && e.ZoneId == GameManager.Instance.selectedLobby.zoneId)
        {
            DoJoinRoom();
        }
    }

    void OnClickFulllaying(GameObject go)
    {
        GameManager.Server.DoRequestGameAction("fullLaying");
    }
    /// <summary>
    /// Call from PlayerController.cs
    /// btAddMeldPrefab
    /// </summary>
    void OnClickAddMeldCard(GameObject go)
    {
        GameplayChan.GiveCard giveCard = (GameplayChan.GiveCard)go.GetComponent<UIContainerAnonymous>().intermediary;
        DoRequestAddMeldCard(giveCard);
    }

    void DoRequestAddMeldCard(GiveCard giveCard)
    {
        GameManager.Server.DoRequestPluginGame(Utility.SetEsObject(Fields.GAMEPLAY.PLAY, new object[] {
            Fields.ACTION, "addMeld",
            "cardsToAdd", new EsObject[] { 
                Utility.SetEsObject(null, new object[] {
                    Fields.CARD.CARD_ID, giveCard.cardId,
                    Fields.GAMEPLAY.PLAYER, GameModelChan.GetPlayer(giveCard.slotIndex).username,
                    "slotId", giveCard.slotIndex,
                    "cardInMeld", giveCard.meld.meld[0].CardId })}
         }));
    }

    void OnClickAddMeld(GameObject go)
    {
        go.SetActive(false);

        if (((LobbyChan)GameManager.Instance.selectedLobby).config.RULE_FULL_PLAYING == 1)
            DoRequestAddMeldCard(listGiveCard.Find(gc => gc.cardId == ListClickCard[0].CardId));
        else
        {
            List<EsObject> list = new List<EsObject>();
            GameModelChan.YourController.mCardHand.ForEach(c => { EsObject obj = new EsObject(); obj.setInteger(Fields.CARD.CARD_ID, c.CardId); list.Add(obj); });
            GameManager.Server.DoRequestPluginGame(Utility.SetEsObject(Fields.GAMEPLAY.PLAY, new object[] { 
                Fields.ACTION, "addMeld", 
                "cardsToAdd", list.ToArray() 
            }));
        }
    }

    void OnClickMeldDown(GameObject go)
    {
        go.SetActive(false);

        GameManager.Server.DoRequestPluginGame(Utility.SetEsObject(Fields.GAMEPLAY.PLAY, new object[] {
            Fields.ACTION, Fields.REQUEST.LAY_MELDS,
            Fields.GAMEPLAY.MELDS, ListClickCard.Count > 2 ? ListClickCard.Select(c => c.CardId).ToArray() : GameModelChan.YourController.mCardHand.Select(c => c.CardId).ToArray()
        }));
    }

    public void OnClickSort(GameObject go)
    {
        btSorted.StopImpact(0.7f);

        if (ListClickCard.Count > 0)
        {
            UpdateButtonWithCard();
        }

        if (go.GetComponent<UIContainerAnonymous>() == null)
            AudioManager.Instance.Play(AudioManager.SoundEffect.OrderCard);
        else
            GameObject.Destroy(go.GetComponent<UIContainerAnonymous>());

        //if (canRequestSort)
        //{
        //    //GameManager.Server.DoRequestGameAction("sortCard");
        //    //sortList = cardController.SortCard();
        //    //GameModel.SortHand();
        //    cardController.SortChan();
        //    UpdateHand(GameModel.YourController.slotServer, 0.5f);
        //    canRequestSort = false;
        //}
        //else
        {
            cardController.SortChan();
            UpdateHand(GameModelChan.YourController.slotServer, 0.5f);
            canRequestSort = false;
            GameModelChan.game.button.UpdateButtonDiscard();
        }
    }

    void OnClickDraw(GameObject go)
    {
        go.SetActive(false);
        GameManager.Server.DoRequestGameAction("drawCard");
    }

    void OnClickView(GameObject go)
    {
        if (listCardInNoc.Count > 0)
        {
            if (GameModelChan.ListGameObjectNoc.Count == 0)
            {
                foreach (int cardId in listCardInNoc)
                {
                    ChanCard c = new ChanCard(0, cardId);
                    c.Instantiate(mPlaymat.locationViewNoc);
                    c.cardTexture.texture.depth = 3;
                    GameModelChan.ListGameObjectNoc.Add(c.gameObject);
                }
                UpdateViewNoc(0.5f);
                deck.transform.FindChild("2. Card").gameObject.SetActive(false);
            }
            else
            {
                if (isViewCompleted)
                {
                    GameModelChan.DestroyViewNoc();
                    deck.transform.FindChild("2. Card").gameObject.SetActive(true);
                }
            }
        }
    }

    #region ON CLICK DISCARD
    void OnClickDiscard(GameObject go)
    {
        //btDiscard.SetStatus(true, false);
        GameModelChan.game.button.UpdateButton();

        if (ListClickCard.Count > 0)
            OnDiscard(ListClickCard[0].CardId);
    }
    public void OnDiscard(int cardId)
    {
        int i = 0;

        //btDiscard.SetStatus(true, false);
        GameModelChan.game.button.UpdateButton();
        i++;
        GameManager.Server.DoRequestPluginGame(Utility.SetEsObject(Fields.GAMEPLAY.PLAY, new object[] { 
            Fields.ACTION, "discard", 
            Fields.CARD.CARD_ID, cardId 
        }));
    }
    #endregion

    void OnClickChiu(GameObject go)
    {
        GameManager.Server.DoRequestGameAction("chiuCard");
        UpdateButtonWithCard();
        StartTimerChiu(0);
    }

    void OnClickSkip(GameObject go)
    {
        GameManager.Server.DoRequestGameAction("ignore");
        UpdateButtonWithCard();
    }

    /// <summary>
    /// Call from objStealCard PlayerController.cs
    /// btStealCardPrefab
    /// </summary>
    void OnProcessStealCard()
    {
        if (ListClickCard.Count > 0)
        {
            GameManager.Server.DoRequestPluginGame(Utility.SetEsObject(Fields.GAMEPLAY.PLAY, new object[] { 
                Fields.ACTION, "stealCard", 
                Fields.CARD.CARD_ID, ListClickCard[0].CardId 
            }));
        }
    }
    void OnClickSteadCard(GameObject go)
    {
        OnProcessStealCard();
    }

    public void OnClickReady(GameObject go)
    {
        btReady.gameObject.SetActive(false);

        GameManager.Server.DoRequestGameAction("ready");
    }
    void OnClickStart(GameObject go)
    {
        if (go.activeSelf)
            go.SetActive(false);

        GameManager.Server.DoRequestGameAction("dealCard");
    }
    #endregion

    /// <summary>
    /// Cập nhật lại UI của các Players
    /// </summary>
    public void UpdateUI()
    {
        if (GameManager.CurrentScene == ESceneName.GameplayChan)
            GameModelChan.ListPlayerInGame.ForEach(p => p.cuiPlayer.UpdateInfo());
    }

    void Update()
    {
        timeCheck -= Time.deltaTime;

        if (timeCheck <= 0)
        {
            if (listResponse.Count > 5)
            {
                WaitingView.Show("Có lỗi xảy ra. Vui lòng đợi!");
                GameManager.Server.DoRequestGameCommand("refreshGame");
            }
            timeCheck = timeToCheck;
        }
        else
            timeCheck = timeToCheck;
    }

    #region UpdateHand
    public void UpdateHand()
    {
        foreach (PlayerControllerChan p in GameModelChan.ListPlayerInGame)
            UpdateHand(p.slotServer, 0.5f);
    }

    public void UpdateHand(int slotIndex, float delay, float time)
    {
        StartCoroutine(_UpdateHand(slotIndex, delay, time));
    }
    IEnumerator _UpdateHand(int slotIndex, float delay, float time)
    {
        yield return new WaitForSeconds(delay);
        UpdateHand(slotIndex, time);
    }

    public void UpdateHand(int slotIndex, float time)
    {
        if (GameModelChan.YourController != null && slotIndex == GameModelChan.YourController.slotServer)
            HideCard();

        PlayerControllerChan player = GameModelChan.GetPlayer(slotIndex);
        #region IN HAND
        if (player.mSide == ESide.Slot_0)
        {
            for (int i = 0; i < player.mCardHand.Count; i++)
            {
                if (player.mCardHand[i].gameObject != null)
                {
                    bool isTouch = false;
                    GameObject obj = player.mCardHand[i].gameObject;
                    player.mCardHand[i].cardTexture.texture.depth = i;
                    System.Collections.Hashtable hash = new System.Collections.Hashtable();
                    hash.Add("islocal", true);
                    hash.Add("time", time);
                    hash.Add("position", mPlaymat.GetLocationHand(player, i, false, isTouch));

                    iTween.MoveTo(obj, hash);
                    iTween.RotateTo(obj, mPlaymat.GetLocationHand(player, i, true, isTouch), time);
                    iTween.ScaleTo(obj, isTouch ? Vector3.one * 1.3f : Vector3.one, time);
                }
            }
        }
        #endregion

        #region IN TRASH
        for (int i = 0; i < player.mCardTrash.Count; i++)
        {
            if (player.mCardTrash[i].gameObject != null)
            {
                GameObject obj = player.mCardTrash[i].gameObject;
                obj.GetComponent<ECardTexture>().texture.depth = i;
                System.Collections.Hashtable hash = new System.Collections.Hashtable();
                hash.Add("islocal", true);
                hash.Add("time", time);
                hash.Add("position", mPlaymat.GetLocationTrash(player, i));

                iTween.MoveTo(obj, hash);
                iTween.RotateTo(obj, Vector3.zero, time);
                iTween.ScaleTo(obj, Vector3.one * 0.60f, time);
            }
        }
        #endregion

        #region STEAL
        for (int i = 0; i < player.mCardSteal.Count; i++)
        {
            for (int j = 0; j < player.mCardSteal[i].steals.Count; j++)
            {
                if (player.mCardSteal[i].steals[j].gameObject != null)
                {
                    GameObject obj = player.mCardSteal[i].steals[j].gameObject;
                    System.Collections.Hashtable hash = new System.Collections.Hashtable();
                    hash.Add("islocal", true);
                    hash.Add("time", time);
                    hash.Add("position", mPlaymat.GetLocationSteals(player, i, j));

                    iTween.MoveTo(obj, hash);
                    iTween.RotateTo(obj, Vector3.zero, time);
                    iTween.ScaleTo(obj, Vector3.one * 0.60f, time);
                }
            }
        }
        #endregion
    }

    public void UpdateHand(GameObject objTouch)
    {
        if (GameModelChan.YourController == null)
            return;
        float time = 0.5f;

        for (int i = 0; i < GameModelChan.YourController.mCardHand.Count; i++)
        {
            if (GameModelChan.YourController.mCardHand[i].gameObject != null)
            {
                bool isTouch = false;

                GameObject obj = GameModelChan.YourController.mCardHand[i].gameObject;

                if (objTouch != null && objTouch == obj) isTouch = true;

                System.Collections.Hashtable hash = new System.Collections.Hashtable();
                hash.Add("islocal", true);
                hash.Add("time", time);
                hash.Add("position", mPlaymat.GetLocationHand(GameModelChan.YourController, i, false, isTouch));

                iTween.MoveTo(obj, hash);
                iTween.RotateTo(obj, mPlaymat.GetLocationHand(GameModelChan.YourController, i, true, isTouch), time);
                iTween.ScaleTo(obj, isTouch ? Vector3.one * 1.4f : Vector3.one, time);
            }
        }
    }

    /// <summary>
    /// Update View Danh Sách Quân Nọc
    /// </summary>
    public void UpdateViewNoc(float time)
    {
        for (int i = 0; i < GameModelChan.ListGameObjectNoc.Count; i++)
        {
            GameObject obj = GameModelChan.ListGameObjectNoc[i];
            iTween.MoveTo(obj, iTween.Hash("islocal", true, "time", time, "position", mPlaymat.GetLocationViewNoc(i)));
            iTween.RotateTo(obj, Vector3.zero, time);
            iTween.ScaleTo(obj, new Vector3(0.66f, 0.66f, 1f), time);
        }
        GameManager.Instance.FunctionDelay(delegate()
        {
            if (GameModelChan.ListGameObjectNoc.Count > 0)
            {
                GameObject lastCard = GameModelChan.ListGameObjectNoc[GameModelChan.ListGameObjectNoc.Count - 1];
                if (lastCard != null)
                {
                    Transform image = lastCard.transform.Find("Image");
                    GameObject objText = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/LableMoneyExchangePrefab"));
                    objText.transform.parent = lastCard.transform.parent;
                    objText.transform.localScale = Vector3.one;
                    iTween.RotateTo(objText, new Vector3(0f, 0f, 270f), 0.1f);
                    //handle image null
                    if (image)
                        objText.transform.localPosition = new Vector3(lastCard.transform.localPosition.x + (image.GetComponent<UITexture>().width / 2), lastCard.transform.localPosition.y, lastCard.transform.localPosition.z);
                    else
                        Debug.LogWarning("Image is null");
                    UILabel txt = objText.GetComponent<UILabel>();
                    txt.fontSize = 14;
                    txt.text = "Đốc nọc";
                    GameModelChan.ListGameObjectNoc.Add(objText);
                }
                isViewCompleted = true;
            }
        }, time);
    }
    #endregion

    #region Click CARD
    const float INCREASE_VECTOR_Y = 15f;

    #region SINGLE CLICK CARD
    Vector3 oldVectorPosition = Vector3.one;
    void HideCard()
    {
        if (clickCard != null)
        {
            clickCard.transform.localPosition = oldVectorPosition;
            clickCard = null;
        }
    }
    void ShowCard(GameObject go)
    {
        HideCard();

        if (go != null)
        {
            clickCard = go;

            oldVectorPosition = go.transform.localPosition;
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y + INCREASE_VECTOR_Y, go.transform.localPosition.z);

            AudioManager.Instance.Play(AudioManager.SoundEffect.SelectCard);
        }
    }
    public void CardClick(GameObject go)
    {
        if (clickCard != null)
        {
            if (clickCard == go)
                HideCard();
            else
                ShowCard(go);
        }
        else
            ShowCard(go);

        UpdateButtonWithCard();
    }

    void UpdateButtonWithCard()
    {
        if (GameModelChan.IsYourTurn)
        {
            GameModelChan.game.button.UpdateButtonDiscard();
        }
    }
    #endregion

    #region MULTI CLICK CARD
    public void CardClick(ECard card)
    {
        if (card != null)
        {
            if (clickCard == card.gameObject)
                clickCard = null;
            else
                clickCard = card.gameObject;

            if (clickCard == null)
                UpdateHand(GameModelChan.YourController.slotServer, 0.5f);
            else
                UpdateHand(clickCard);
        }

        GameModelChan.game.button.UpdateButtonDiscard();
    }
    #endregion

    /// <summary>
    /// Các card đang pick
    /// </summary>
    public List<ChanCard> ListClickCard
    {
        get
        {
            if (clickCard == null)
                return clickCardList;
            else
                return new List<ChanCard>(new ChanCard[1] { (ChanCard)clickCard.GetComponent<CardTextureChan>().card });
        }
    }

    #endregion

    #region gameFinishType
    /// <summary>
    /// Lưu cái type của finish game để check khi tạo effect end game
    /// </summary>
    [HideInInspector]
    public EFinishType gameFinishType;
    public enum EFinishType
    {
        FULL_LAYING,
        GAME_DRAW,
        //NORMAL,         //normal
        //U_THUONG,       //playerFullLaying
        //U_TRON,         //playerFullLaying10Cards 
        //U_DEN_THUONG,   //playerFullLayingBySteal3Cards
        //U_DEN_TRON,     //playerFullLaying10CardsBySteal3Cards
        //U_XUONG          //playerFullLayingWithNoMeld
    }
    public void SetTypeFinishGame(string str)
    {
        if (str == "gameDraw")
            gameFinishType = EFinishType.GAME_DRAW;
        else //fullLaying
            gameFinishType = EFinishType.FULL_LAYING;
    }
    #endregion

    /// <summary>
    /// Cập nhật thông tin người chơi khi có thông tin mới
    /// </summary>
    /// <param name="eso">Thông tin server trả về</param>
    /// <param name="index">Vị trí của người chơi trên server</param>
    /// <param name="variable">Tham số lấy về của player</param>
    private void SetPlayerEsObject(EsObject eso, ref int index, params string[] variable)
    {
        if (variable == null || variable.Length == 0)
            variable = new string[] { Fields.GAMEPLAY.PLAYER };

        EsObject player = eso.getEsObject(variable[0]);

        if (player.variableExists("slotIndex"))
        {
            index = player.getInteger("slotIndex");
            if (GameModelChan.GetPlayer(index) != null)
                GameModelChan.GetPlayer(index).SetDataPlayer(player);
        }
        else
        {
            PlayerControllerChan p = GameModelChan.GetPlayer(player.getString(Fields.PLAYER.USERNAME));
            p.SetDataPlayer(player);
            index = p.slotServer;
        }
    }

    /// <summary>
    /// Xử lý finishgame từ server
    /// </summary>
    private IEnumerator ProcessFinishGame(EsObject eso)
    {
        dealCardEffect.OnDestroy();

        while (GameModelChan.YourController != null && GameModelChan.ListPlayerInGame.Contains(GameModelChan.YourController) && GameModelChan.DealCardDone == false)
            yield return new WaitForFixedUpdate();

        #region Không cho phép người chơi click card hay Drag&Drop card khi xẩy ra hiệu ứng lúc finish Game
        if (GameModelChan.YourController != null)
        {
            UpdateHand();
            foreach (ECard c in GameModelChan.YourController.mCardHand)
                c.cardTexture.SetCollider(false);
        }
        #endregion

        if (eso.variableExists("lastPlayer"))
            SetPlayerEsObject(eso, ref GameModelChan.IndexLastInTurn, new string[] { "lastPlayer" });

        SetTypeFinishGame(eso.getString("finishType"));

        EsObject result = eso.getEsObject("result");
        EsObject[] lstSummary = result.getEsObjectArray("summary");
        foreach (EsObject obj in lstSummary)
            GameModelChan.GetPlayer(obj.getString(Fields.PLAYER.USERNAME)).SetDataSummary(obj);

        summaryGame.Clear();
        EsObject[] lstAction = result.getEsObjectArray("actions");

        Listener.RegisterEventLogGame("[FF9900]***** THỐNG KÊ. KẾT QUẢ TRẬN ĐẤU *****[-]\n");
        string winner, cuoc, cuocIncorrect;
        //EsObject resultPMeso = eso.getEsObject("PMeso");
        if (eso.variableExists("winner") && eso.variableExists("cuoc"))
        {
            winner = eso.getString("winner");
            cuoc = eso.getString("cuoc");
            Listener.RegisterEventLogGame("[FF9900]Người chơi [-]" + winner + " [FF9900]xướng cước [-]" + cuoc + "\n");
        }

        if (eso.variableExists("cuocIncorrect"))
        {
            cuocIncorrect = eso.getString("cuocIncorrect");
            if (cuocIncorrect.Length > 0)
                Listener.RegisterEventLogGame("[FF9900](" + cuocIncorrect + ")[-]\n");

        }
        foreach (EsObject obj in lstAction)
        {
            Summary sum = new Summary();
            sum.sourcePlayer = obj.getString("sourcePlayer");
            sum.targetPlayer = obj.getString("targetPlayer");
            sum.description = obj.getString("description");
            sum.action = (Summary.EAction)obj.getInteger(Fields.ACTION);
            sum.moneyExchange = long.Parse(obj.getString("moneyExchange"));
            summaryGame.Add(sum);
            Listener.RegisterEventLogGame("[FF9900]" + sum.targetPlayer + "[-] đã nhận được [FF9900]" + sum.moneyExchange + "[-] từ người chơi [FF9900]" + sum.sourcePlayer + "[-] vì [FF9900]" + sum.description + "[-]\n");
        }
        _timeCountDownTurm = eso.getInteger("time") / 1000f; //Lưu time lại nhưng ko chạy đồng hồ

        if (gameFinishType == EFinishType.FULL_LAYING)
        {
            GameModelChan.DestroyObject();
            ListResultXuongView.Create(eso.getString("cuoc"), eso.getString("cuocIncorrect"), eso.getInteger("point"), eso.variableExists("textGa") ? eso.getString("textGa") : "", lstSummary, eso.getString("infoFullLaying"));
            HideAllOtherObjectWhenFullaying();
            AudioManager.Instance.PlaySounXuong(eso.getIntegerArray("cuocSound"), cardU);
        }
        else
        {
            SoundGameplay.Instances.ShowAudioOther(3, 0);
            lbDiscard.text = eso.getString("textNotification");
        }

        GameModelChan.ActiveState(GameModelChan.EGameState.finalizing);
    }

    //EsObject updateMoneyAfterFinishGame = null;
    /// <summary>
    /// Cập nhật lại tiền của người chơi sau khi hết thúc trận đấu
    /// </summary>
    public void UpdateUserInfo(EsObject updateMoneyAfterFinishGame)
    {
        if (updateMoneyAfterFinishGame != null)
        {
            string field = updateMoneyAfterFinishGame.getString("field");
            if (field == "money")
            {
                string type = "";
                if (updateMoneyAfterFinishGame.variableExists("moneyType"))
                    type = updateMoneyAfterFinishGame.getString("moneyType");
                if (GameModelChan.GetPlayer(updateMoneyAfterFinishGame.getString(Fields.PLAYER.USERNAME)) != null && type != "")
                    GameModelChan.GetPlayer(updateMoneyAfterFinishGame.getString(Fields.PLAYER.USERNAME)).UpdateMoney(updateMoneyAfterFinishGame.getString("value"), type);
            }
        }
    }
    //public void UpdateUserInfo()
    //{
    //    if (updateMoneyAfterFinishGame != null)
    //    {
    //        string field = updateMoneyAfterFinishGame.getString("field");
    //        if (field == "money")
    //            GameModel.GetPlayer(updateMoneyAfterFinishGame.getString(Fields.PLAYER.USERNAME)).UpdateMoney(updateMoneyAfterFinishGame.getString("value"));
    //        //updateMoneyAfterFinishGame = null;
    //    }
    //}

    List<CommandEsObject> listResponse = new List<CommandEsObject>();
    bool _isProcessDone = true;
    public bool IsProcesResonseDone
    {
        get { return _isProcessDone; }
        set
        {
            _isProcessDone = value;

            if (_isProcessDone && listResponse.Count > 0)
            {
                CommandEsObject response = listResponse[0];
                Debug.LogWarning("Đang xử lý command: " + response.command + " - " + Utility.Convert.TimeToStringFull(DateTime.Now));

                StartCoroutine(_ProcessOnGameplay(response.command, response.action, response.eso));
                listResponse.RemoveAt(0);
            }
        }
    }

    /// <summary>
    /// Xử lý thông tin lấy về từ server
    /// </summary>
    void ProcessOnGameplay(string command, string action, EsObject eso)
    {
        if (command == "refreshGame")
        {
            DoRefreshGame(eso);
            return;
        }

        if (!IsProcesResonseDone || listResponse.Count > 0)
        {
            Debug.LogWarning("Xếp hàng chờ: " + command + " - " + Utility.Convert.TimeToStringFull(DateTime.Now));
            listResponse.Add(new CommandEsObject(command, action, eso));
        }
        else
        {
            ///Chờ cho đến khi đã load được dữ liệu của trận đấu thì mới xử lý luồng 
            StartCoroutine(_ProcessOnGameplay(command, action, eso));
        }
    }

    void DoRefreshGame(EsObject eso)
    {
        //stop queue in list
        GameModelChan.CreateNewGame();
        GameModelChan.DestroyObject();

        DoUpdateGame(eso);
        IsProcesResonseDone = true;
        WaitingView.Instance.Close();
    }

    public string ShowMessageFullaying(string msg)
    {
        string[] str = msg.Split('.');
        string textStr = "";
        Array.ForEach(str, arrstr => textStr += "\n" + arrstr);
        return textStr;

    }
    void ProcessOnChiu(EsObject eso)
    {
        bool ChuiCuaNhaMinh = false;
        GameModelChan.IsCanChiu = eso.variableExists("timeWaittingChiu");

        if (GameModelChan.IsCanChiu)
        {
            Debug.LogWarning("CÓ THỂ CHÍU.........");

            int cardId = eso.getInteger("cardId");
            if (GameModelChan.YourController != null && GameModelChan.YourController.mCardHand.FindAll(c => c.CardId == cardId).Count == 3)
            {
                if (GameModelChan.IndexInTurn != GameModelChan.YourController.slotServer)
                {
                    StartTimerChiu(eso.getInteger("timeWaittingChiu") / 1000f);
                    timeChiu.gameObject.SetActive(true);
                    ChuiCuaNhaMinh = false;
                }
                if (GameModelChan.IndexInTurn == GameModelChan.YourController.slotServer)
                {
                    ChuiCuaNhaMinh = true;
                }
            }
            if (GameModelChan.YourController != null && GameModelChan.IndexInTurn == GameModelChan.YourController.slotServer)
            {
                if (!ChuiCuaNhaMinh)
                {
                    TimeCountDown = 0;
                }
            }
        }
    }
    float valueTimer;
    float startTimer;
    void StartTimerChiu(float time)
    {
        startTimer = time;
        valueTimer = time;
        UpdateTime();
    }

    void UpdateTime()
    {
        valueTimer -= Time.deltaTime;
        timeChiu.transform.FindChild("Foreground").GetComponent<UISprite>().fillAmount = valueTimer / startTimer;

        if (valueTimer > 0)
        {
            Invoke("UpdateTime", Time.deltaTime);
        }
        else
        {
            CancelInvoke("UpdateTime");
            timeChiu.gameObject.SetActive(false);
        }
    }
    public void ShowMessageBocCai()
    {

        if (GameModelChan.YourController != null && GameModelChan.IndexInTurn == GameModelChan.YourController.slotServer)
        {
            lbDiscard.text = "Mời bạn bốc cái";
        }
    }
    IEnumerator _ProcessOnGameplay(string command, string action, EsObject eso)
    {
        string cm = command;

        IsProcesResonseDone = false;
        while (isStartDone == false)
        {
            yield return new WaitForEndOfFrame();
        }

        if (command == "getCuoc")
        {
            #region TRẢ VỀ DANH SÁCH CÁC CƯỚC CÓ THỂ Ù ĐƯỢC
            GameManager.Instance.ListCuocU.Clear();
            foreach (EsObject es in eso.getEsObjectArray("cuoc"))
                GameManager.Instance.ListCuocU.Add(new CuocUXuong(es.getInteger("id"), es.getString("name"), es.getInteger("diem"), es.getInteger("dich"), es.getInteger("max"), es.getIntegerArray("incompatibles")));
            #endregion
        }
        else if (command == "playerFullLaying")
        {

            #region CÓ NGƯỜI CHƠI BÁO Ù
            string notification = eso.getString("textNotification");
            bool isUChi = true;
            if (eso.variableExists("cuaChi"))
            {
                isUChi = eso.getBoolean("cuaChi");
            }
            if (!string.IsNullOrEmpty(eso.getString("userName")))
            {

                PlayerControllerChan p = GameModelChan.GetPlayer(eso.getString("userName"));
                ChanCard card = null;
                if (eso.variableExists("cardFullLaying"))
                {
                    EsObject cardFullLaying = eso.getEsObject("cardFullLaying");
                    int cardId = cardFullLaying.getInteger("cardId");
                    cardU = cardId;
                    string username = cardFullLaying.getString("userName");
                    if (cardFullLaying.getBoolean("onTrash"))
                    {
                        card = (ChanCard)GameModelChan.GetPlayer(username).mCardTrash.FindLast(c => c.CardId == cardId);
                        if (card == null)
                        {
                            StealCard cardSteals = GameModelChan.GetPlayer(username).mCardSteal.Find(steal => steal.steals.Find(c => ((ChanCard)c).CardId == cardId && ((ChanCard)c).isDrawFromDeck && ((ChanCard)c).timeExpire > 0) != null);
                            if (cardSteals != null && cardSteals.steals.Count > 0)
                                card = (ChanCard)cardSteals.steals[0];
                        }
                    }
                    else
                    {
                        StealCard cardSteals = GameModelChan.GetPlayer(username).mCardSteal.Find(steal => steal.steals.Find(c => ((ChanCard)c).CardId == cardId && ((ChanCard)c).isDrawFromDeck && ((ChanCard)c).timeExpire > 0) != null);
                        if (cardSteals != null && cardSteals.steals.Count > 0)
                            card = (ChanCard)cardSteals.steals[0];
                        else
                            card = (ChanCard)GameModelChan.GetPlayer(username).mCardTrash.FindLast(c => c.CardId == cardId);
                    }
                    card.SetHightlight();
                    if (!isUChi)
                    {
                        AudioManager.Instance.playSounUXaAndUGan(cardId, true);
                    }
                    else if (isUChi)
                    {
                        SoundGameplay.Instances.nhoGanAndDayRoi(cardId);
                    }
                }
                else
                {
                    if (isUChi)
                        SoundGameplay.Instances.ShowAudioOther(6, 0);
                }
                lbDiscard.text = ShowMessageFullaying(notification);
            }
            else
                lbDiscard.text = "Đang chờ xem có ai Ù hay không ?";

            GameModelChan.Start_WaitFullLaying();

            #endregion
        }
        else if (command == "waittingPlayerXuong")
        {
            if (GameObject.Find("__BettingView") != null)
                GameObject.Destroy(GameObject.Find("__BettingView").gameObject);

            GameModelChan.ListPlayer.ForEach(p => p.cuiPlayer.StartTime(0));
            #region CHỜ NGƯỜI CHƠI Ù XƯỚNG
            PlayerControllerChan playerXuong = GameModelChan.GetPlayer(eso.getString("userName"));

            listFullLaying = new EsObject[3][];
            if (eso.variableExists("chan"))
                listFullLaying[0] = eso.getEsObjectArray("chan");
            if (eso.variableExists("ca"))
                listFullLaying[1] = eso.getEsObjectArray("ca");
            if (eso.variableExists("cardFullLaying"))
                listFullLaying[2] = eso.getEsObjectArray("cardFullLaying");

            if (GameModelChan.YourController != null && GameModelChan.YourController == playerXuong)
            {
                GameModelChan.DestroyObject();
                HideAllOtherObjectWhenFullaying();
                ListCuocUView.Create(eso.getInteger("time") / 1000f, eso.getInteger("remainingTime") / 1000f);
            }
            else
            {
                if (playerXuong != null)
                {
                    GameModelChan.IndexInTurn = playerXuong.slotServer;
                    TimeCountDown = eso.getInteger("time") / 1000f;
                    lbDiscard.text = eso.getString("textNotification");
                }
                else
                {
                    lbDiscard.text = eso.getString("textNotification");
                }

                listCardInNoc.Clear();
                foreach (int cardId in eso.getIntegerArray("cardInNoc"))
                {
                    listCardInNoc.Add(cardId);
                }
            }

            GameModelChan.MiniState = GameModelChan.EGameStateMini.wait_player_xuong;
            #endregion
        }
        else if (command == "playError")
        {
            #region NGƯỜI CHƠI XẨY RA LỖI
            PlayerControllerChan p = GameModelChan.GetPlayer(eso.getString("userName"));
            if (p != null)
            {
                p.warningMessage = eso.getString("description");
                p.cuiPlayer.CheckIcon();
                GameplayWarningView.Warning(p.warningMessage, 6f);
                SoundGameplay.Instances.ShowAudioOther(4, 0);
                p.isBatBao = true;
            }
            #endregion
        }
        else if (command == "playerListChanged")
        {

            #region LIST PLAYER CHANGE
            if (action == "playerAdded")
            {
                if (GameModelChan.CurrentState == GameModelChan.EGameState.dealing && GameModelChan.YourController != null)
                {
                    lbDiscard.text = "Bạn vừa thoát ra khi chia bài sever sẽ tự động bắt cái!";
                }
                EsObject player = eso.getEsObject(Fields.GAMEPLAY.PLAYER);
                PlayerControllerChan p = new PlayerControllerChan(player);

                if (GameModelChan.YourController != null && (int)GameModelChan.YourController.PlayerState >= (int)PlayerControllerChan.EPlayerState.ready && GameModelChan.CurrentState >= GameModelChan.EGameState.dealClient)
                {
                    //Nếu là bạn đang chơi => Thì người mới sẽ được thêm vào danh sách khác.
                    GameModelChan.ListJoinGameWhenPlaying.Add(p);
                    if (!ListResultXuongView.IsShowing && !ListCuocUView.IsShowing)
                        GameModelChan.UpdatePlayerSide();
                    GameModelChan.game.Listener.RegisterEventPlayerListChanged(p, false);
                }
                else
                {

                    //Nếu là bạn không tham gia trận đấu thì sẽ làm như bình thường
                    GameModelChan.SetPlayer(p.slotServer, p);
                    GameModelChan.UpdatePlayerSide();
                    GameModelChan.game.Listener.RegisterEventPlayerListChanged(GameModelChan.GetPlayer(p.slotServer), false);
                }
            }
            else if (action == "playerRemoved")
            {

                EsObject esPlayer = eso.getEsObject(Fields.GAMEPLAY.PLAYER);
                PlayerControllerChan joinWhenPlaying = GameModelChan.ListJoinGameWhenPlaying.Find(pp => pp.username == esPlayer.getString(Fields.PLAYER.USERNAME));
                if (joinWhenPlaying != null)
                {

                    ///Thoát ra khi mà mới join vào trận đấu, nhưng chưa tham gia trận đấu
                    GameObject.Destroy(joinWhenPlaying.cuiPlayer.gameObject);
                    GameModelChan.ListJoinGameWhenPlaying.Remove(joinWhenPlaying);
                    GameModelChan.DrawInfoPlayerNoSlot();
                }
                else
                {

                    PlayerControllerChan p = GameModelChan.GetPlayer(esPlayer.getString(Fields.PLAYER.USERNAME));
                    ///Nếu người thoát ra đang chơi thì đánh dấu là đã quit nhưng vẫn lưu lại các thông tin trên bàn chơi.
                    if ((int)p.PlayerState >= (int)PlayerControllerChan.EPlayerState.ready && GameModelChan.CurrentState >= GameModelChan.EGameState.dealClient)
                    {
                        StartCoroutine(GameModelChan.model.PlayerLeftOut(p));
                        p.IsHasQuit = true;
                    }
                    else
                    { ///Ngược còn không tức là trường hợp bình thường, là người chơi out ra khi trận đấu chưa diễn ra.
                        GameModelChan.SetPlayer(p.slotServer, null);
                    }
                }
            }
            else if (action == "playerQuitGameChan")
            {
                PlayerControllerChan p = GameModelChan.GetPlayer(eso.getString(Fields.PLAYER.USERNAME));
                p.QuitWithoutRemove();

            }
            else if (action == "playerComeBack")
            {
                string userName = eso.getString(Fields.PLAYER.USERNAME);
                //if (userName != GameManager.Instance.mInfo.username)
                //{
                PlayerControllerChan p = GameModelChan.GetPlayer(userName);
                if (p != null)
                    p.ProcessWhenPlayerComeback();
                //}
            }
            else if (action == "waitingPlayerAdded")
            {
                EsObject esPlayer = eso.getEsObject(Fields.GAMEPLAY.PLAYER);
                PlayerControllerChan waitingPlayer = new PlayerControllerChan(esPlayer);
                Listener.RegisterEventPlayerListChanged(waitingPlayer, false);
                GameModelChan.ListWaitingPlayer.Add(waitingPlayer);
                HeaderMenu.Instance.AddWaitingPlayer(waitingPlayer);
            }
            else if (action == "waitingPlayerRemoved")
            {
                //PlayerController plc = new PlayerController(eso);
                EsObject esPlayer = eso.getEsObject(Fields.GAMEPLAY.PLAYER);
                PlayerControllerChan player = new PlayerControllerChan(esPlayer);
                PlayerControllerChan plc = GameModelChan.ListWaitingPlayer.Find(pc => pc.username == player.username);
                if (plc != null)
                {
                    GameModelChan.ListWaitingPlayer.Remove(plc);
                    HeaderMenu.Instance.RemoveWaitingPlayer(plc);
                    Listener.RegisterEventPlayerListChanged(plc, true);
                }

            }
            #endregion
        }
        else if (command == "updateUserInfo")
        {
            #region THÔNG TIN NGƯỜI CHƠI THAY ĐỔI (UPDATE SAU KHI KẾT THÚC GAME)
            UpdateUserInfo(eso);
            GameModelChan.game.UpdateUI();
            //updateMoneyAfterFinishGame = eso;
            #endregion
        }
        else if (command == "playerReady")
        {
            #region NGƯỜI CHƠI READRY
            int slotIndex = 0;
            SetPlayerEsObject(eso, ref slotIndex);

            if (eso.variableExists("okToDealCard"))
            {
                if ((ClickStartGame == false || NumberCountDownObj == null || NumberCountDownObj.gameObject == null || !NumberCountDownObj.IsRunning) && eso.variableExists("timeAutoDealCard"))
                    StartTimeAutoDeal(eso.getInteger("timeAutoDealCard") / 1000f);

                if (eso.variableExists("okToDealCard"))
                    ClickStartGame = eso.getBoolean("okToDealCard");
            }

            GameModelChan.game.UpdateUI();
            #endregion
        }
        else if (command == "pickPlayer")
        {

            #region XÉT CÁI CHO NGƯỜI CHƠI
            GameModelChan.DealCardDone = false;
            GameModelChan.ActiveState(GameModelChan.EGameState.dealClient);
            ///Tự ẩn nút bắt đầu trận đấu khi thời gian đếm ngược kết thúc
            WhenDealCard();

            dealCardEffect = new GameplayDealCardEffect();
            dealCardEffect.firstPlayer = "";
            dealCardEffect.firstCardServerResponse = -1;

            GameModelChan.IndexInTurn = GameModelChan.GetPlayer(eso.getString("userName")).slotServer;

            if (GameModelChan.YourController == null || GameModelChan.IndexInTurn != GameModelChan.YourController.slotServer)
                lbDiscard.text = "Vui lòng chờ người chơi " + eso.getString("userName") + " bắt cái";

            dealCardEffect.OnInit();
            #endregion
        }
        else if (command == "firstPick")
        {
            Debug.Log("current Game state " + GameModelChan.CurrentState.ToString());
            #region NGƯỜI CHƠI PICK LƯỢT 1
            if (GameModelChan.CurrentState != GameModelChan.EGameState.dealing)
            {
                if (eso.variableExists("slotChoice"))
                {
                    EsObject objChoise = eso.getEsObject("slotChoice");
                    dealCardEffect.Pick(objChoise.getInteger("firstPick"), -1);
                }
            }
            else
            {
                isDealing = true;
            }

            #endregion
        }
        else if (command == "updateHand")
        {
            #region BẮT ĐẦU GAME
            /////Tự ẩn nút bắt đầu trận đấu khi thời gian đếm ngược kết thúc
            //WhenDealCard();
            lbDiscard.text = "";

            EsObject[] players = eso.getEsObjectArray("players");

            if (eso.variableExists("gameState"))
            {
                string gameState = eso.getString("gameState");
                if (gameState == "playing")
                    GameModelChan.ActiveState(GameModelChan.ConvertGameState(gameState));
            }

            foreach (EsObject obj in players)
            {
                int slotIndex = obj.getInteger("slotIndex");
                if (GameModelChan.YourController != null && slotIndex == GameModelChan.YourController.slotServer)
                    continue;
                if (GameModelChan.GetPlayer(slotIndex) == null)
                    continue;
                GameModelChan.GetPlayer(slotIndex).SetDataPlayer(obj);
                int handSize = GameModelChan.GetPlayer(slotIndex).handSize;

                GameModelChan.GetPlayer(slotIndex).Reset();
                GameModelChan.GetPlayer(slotIndex).mCardHand.Clear();

                GameModelChan.GetPlayer(slotIndex).SetDataPlayer(obj);
            }
            if (!isDealing)
            {
                if (eso.variableExists("slotChoice"))
                {
                    EsObject objChoise = eso.getEsObject("slotChoice");
                    dealCardEffect.Pick(-1, objChoise.getInteger("donePick"));
                    dealCardEffect.firstCardServerResponse = objChoise.getInteger("firstCard");
                    dealCardEffect.firstPlayer = objChoise.getString("firstPlayer");
                    //int soundID = objChoise.getInteger("sound");
                    // play sound boc cai
                    if (objChoise.variableExists("sound"))
                    {
                        SoundGameplay.Instances.PlaySoundBocCai(dealCardEffect.firstCardServerResponse, GameModelChan.ListPlayerInGame.Count, objChoise.getInteger("sound"));
                    }
                    while (dealCardEffect.currentStep != GameplayDealCardEffect.StepDealCard.DONE)
                        yield return new WaitForSeconds(0.1f);
                }
            }
            else
                isDealing = false;
            if (GameModelChan.YourController != null) 
            {
                GameModelChan.IndexInTurn = GameModelChan.YourController.slotServer;
                TimeCountDown = eso.getInteger("timeForAnimation") / 1000f;
                if (GameModelChan.IndexInTurn == GameModelChan.YourController.slotServer)
                    if (eso.variableExists("fullLaying"))
                        fullLaying = eso.getBoolean("fullLaying");
                GameModelChan.YourController.mCardHand.Clear();
                int[] listCard = eso.getIntegerArray("hand");
                Array.ForEach<int>(listCard, cardValue => GameModelChan.YourController.mCardHand.Add(new ChanCard(GameModelChan.YourController.slotServer, cardValue)));
            }
            GameModelChan.ActiveState(GameModelChan.EGameState.deal);
            GameModelChan.game.button.UpdateButton();
            GameModelChan.game.UpdateUI();
            #endregion
        }
        else if (command == "updateGameState")
        {
            #region UPDATE GAME STATE
            string gameState = eso.getString("gameState");
            string lastGameState = eso.getString("lastGameState");
            Debug.LogWarning("UpdateGameState: Serverr Last Game State => " + GameModelChan.ConvertGameState(lastGameState).ToString());

            ///Phòng trường hợp trước đó bước finishGame chưa desroy đc sẽ destroy lại
            if (GameModelChan.ConvertGameState(lastGameState) == GameModelChan.EGameState.finalizing)
            {
                //Fix lỗi khi click card lúc hết trận kiểm tra phần tử trong list
                clickCardList.Clear();
                GameModelChan.DestroyObject();
            }

            GameModelChan.ActiveState(GameModelChan.ConvertGameState(gameState));
            #endregion
        }
        else if (command == "expireChiu")
        {
            #region HẾT THỜI GIAN CHÍU
            Debug.LogWarning("HẾT THỜI GIAN CHÍU.........");

            GameModelChan.IsCanChiu = false;
            TimeCountDown = eso.getInteger("time") / 1000f;
            if (eso.variableExists("slotId"))
                GameModelChan.IndexInTurn = eso.getInteger("slotId");
            GameModelChan.MiniState = GameModelChan.MiniState;
            #endregion
        }
        else if (command == "turn")
        {
            #region ĐÁNH BÀI
            int soundId = -1;
            SetPlayerEsObject(eso, ref GameModelChan.IndexInTurn);

            if (eso.variableExists("lastPlayer"))
            {
                SetPlayerEsObject(eso, ref GameModelChan.IndexLastInTurn, new string[] { "lastPlayer" });
                //Trường hợp sau khi chíu xong đến lượt luôn
                if (GameModelChan.IndexLastInTurn == GameModelChan.IndexInTurn)
                    SetPlayerEsObject(eso, ref GameModelChan.IndexInTurn);

                if (GameModelChan.YourController != null)
                    if (GameModelChan.IndexInTurn == GameModelChan.YourController.slotServer)
                        if (eso.variableExists("stolen"))
                            stolen = eso.getBoolean("stolen");

                AudioManager.Instance.Play(AudioManager.SoundEffect.Discard);
                if (eso.variableExists("sound"))
                {
                    soundId = eso.getInteger("sound");
                }
                if (eso.variableExists(Fields.CARD.CARD_ID))
                {
                    GameModelChan.Discard(soundId, GameModelChan.IndexLastInTurn, eso.getInteger(Fields.CARD.CARD_ID), eso.variableExists("cardAddedToPlayer") ? eso.getString("cardAddedToPlayer") : null);
                }
                else
                {
                    GameModelChan.SkipCard(soundId, GameModelChan.IndexLastInTurn);
                    GameModelChan.MiniState = GameModelChan.EGameStateMini.stealCard_or_draw;
                }
            }
            else
            {
                GameModelChan.ActiveState(GameModelChan.EGameState.playing);
                //Trường hợp người chơi trước thoát ra khỏi bàn
                if (GameModelChan.GetPlayer(GameModelChan.IndexInTurn).PlayerState == EPlayerController.EPlayerState.inTurnStealOrDrawCard)
                    GameModelChan.MiniState = GameModelChan.EGameStateMini.stealCard_or_draw;
            }
            TimeCountDown = eso.getInteger("time") / 1000f;
            ProcessOnChiu(eso);
            #endregion
        }
        else if (command == "playerDrawCard")
        {
            #region BỐC BÀI
            SetPlayerEsObject(eso, ref GameModelChan.IndexInTurn);
            int cardId = eso.getInteger(Fields.CARD.CARD_ID);

            if (eso.variableExists("sound") && eso.getIntegerArray("sound").Length > 0)
            {
                int[] soundId = eso.getIntegerArray("sound");
                SoundGameplay.Instances.PlaySoundDrawCard(soundId);
            }
            else
            {
                SoundGameplay.Instances.PlayerShowCard(cardId, true);
            }


            if (GameModelChan.YourController != null)
                if (GameModelChan.IndexInTurn == GameModelChan.YourController.slotServer)
                    fullLaying = eso.variableExists("fullLaying") ? eso.getBoolean("fullLaying") : false;

            GameModelChan.DrawCard(GameModelChan.IndexInTurn, cardId, eso.getInteger("timeExpire") / 1000);
            TimeCountDown = eso.getInteger("time") / 1000f;

            ProcessOnChiu(eso);
            lbDiscard.text = "";
            #endregion
        }
        else if (command == "playerStealCard")
        {
            #region ĂN CÂY
            SetPlayerEsObject(eso, ref GameModelChan.IndexInTurn);
            SetPlayerEsObject(eso, ref GameModelChan.IndexLastInTurn, new string[] { "lastPlayer" });

            int soundId = -1;
            if (eso.variableExists("sound"))
            {
                soundId = eso.getInteger("sound");
            }
            GameModelChan.StealCard(soundId, GameModelChan.IndexInTurn, GameModelChan.IndexLastInTurn, eso.getIntegerArray("cardId"));


            //if (eso.variableExists("moneyExchange"))
            //    MoneyExchangeView.Create(GameModel.IndexLastInTurn, GameModel.IndexInTurn, eso.getInteger("moneyExchange"));

            //if (eso.variableExists("cardToTransfer"))
            //{
            //    EsObject cardToTransfer = eso.getEsObject("cardToTransfer");
            //    int indexCardInTrash = GameModel.GetPlayer(cardToTransfer.getString("sourcePlayer")).mCardTrash.FindIndex(o => o.CardId == cardToTransfer.getInteger(Fields.CARD.CARD_ID));
            //    Card cardTransfer = GameModel.GetPlayer(cardToTransfer.getString("sourcePlayer")).mCardTrash[indexCardInTrash];
            //    GameModel.GetPlayer(cardToTransfer.getString("sourcePlayer")).mCardTrash.Remove(cardTransfer);
            //    GameModel.GetPlayer(cardToTransfer.getString("targetPlayer")).mCardTrash.Add(cardTransfer);
            //    UpdateHand();
            //}

            if (GameModelChan.YourController != null && GameModelChan.IndexInTurn == GameModelChan.YourController.slotServer)
            {
                fullLaying = eso.variableExists("fullLaying") ? eso.getBoolean("fullLaying") : false;
                if (fullLaying)
                    GameModelChan.game.button.UpdateButton();
            }

            TimeCountDown = eso.getInteger("time") / 1000f;
            #endregion
        }
        else if (command == "sortCard")
        {
            #region XẾP BÀI
            EsObject[] results = eso.getEsObjectArray("results");
            sortList.Clear();
            foreach (EsObject es in results)
            {
                List<int> lst = new List<int>(es.getIntegerArray("result"));
                if (lst != null && lst.Count > 0)
                    sortList.Add(lst);
            }
            GameModelChan.SortHand();
            canRequestSort = false;
            #endregion
        }
        else if (command == "checkGiveCard")
        {
            #region CHECK CÓ THỂ GỬI BÀI
            EsObject[] lst = eso.getEsObjectArray("result");

            listGiveCard.Clear();
            foreach (EsObject obj in lst)
            {
                GiveCard card = new GiveCard();
                card.cardId = obj.getInteger(Fields.CARD.CARD_ID);
                card.slotIndex = obj.getInteger("slotId");
                card.meldResponse = obj.getIntegerArray(Fields.GAMEPLAY.MELDS);
                card.meld = GameModelChan.GetPlayer(card.slotIndex).GetMeld(card.meldResponse);
                listGiveCard.Add(card);
            }
            GameModelChan.game.button.UpdateButton();
            #endregion
        }
        else if (command == "checkMeld")
        {
            #region CHECK MELD
            EsObject[] lst = eso.getEsObjectArray(Fields.GAMEPLAY.MELDS);

            meldList.Clear();

            if (lst.Length > 0)
            {
                Array.ForEach<EsObject>(lst, obj =>
                    meldList.Add(new List<int>(obj.getIntegerArray("meld")))
                );
            }
            GameModelChan.game.button.UpdateButton();
            #endregion
        }
        else if (command == "updateRoomMaster")
        {
            #region Update Room Master

            EsObject player = eso.getEsObject(Fields.GAMEPLAY.PLAYER);

            PlayerControllerChan p = new PlayerControllerChan(player);
            GameModelChan.ListPlayer.ForEach(pC => RemoveRoomMasterIcon(pC));
            if (GameModelChan.GetPlayer(p.slotServer) != null)
            {
                GameModelChan.GetPlayer(p.slotServer).SetDataPlayer(player);
                GameModelChan.GetPlayer(p.slotServer).cuiPlayer.UpdateInfo();
            }

            ((LobbyChan)GameManager.Instance.selectedLobby).roomMasterUsername = p.username;
            ((LobbyChan)GameManager.Instance.selectedLobby).config.password = eso.getString("password");


            if (GameModelChan.CurrentState == GameModelChan.EGameState.waitingForPlayer || GameModelChan.CurrentState == GameModelChan.EGameState.waitingForReady)
                GameModelChan.game.ClickStartGame = GameModelChan.ListPlayerInGameEnemy.Count > 0;

            Listener.RegisterEventRoomMasterChanged(GameModelChan.GetPlayer(p.slotServer));

            GameModelChan.UpdatePlayerSide();
            #endregion
        }
        else if (command == "finishGame")
        {
            #region FINISH GAME
            ListCuocUView.Close();
            StartCoroutine(ProcessFinishGame(eso));
            #endregion
        }
        else if (command == "updatePlayersSlot")
        {
            #region Switch Player
            EsObject[] players = eso.getEsObjectArray("players");

            GameModelChan.ListPlayer.ForEach(p => GameObject.Destroy(p.cuiPlayer.gameObject));

            PlayerControllerChan[] lst = new PlayerControllerChan[5];
            foreach (EsObject player in players)
                lst[player.getInteger("slotIndex")] = new PlayerControllerChan(player);
            GameModelChan.SetPlayer(lst);
            #endregion
        }
        else if (command == "changeConfigGame")
        {
            #region Change Config Game
            ((LobbyChan)GameManager.Instance.selectedLobby).SetDataJoinLobby(eso);
            this.ResetRoomInfo(eso);
            //Bao gio co ga thi bo comment ra 
            if (((LobbyChan)GameManager.Instance.selectedLobby).config.NUOI_GA_RULE != LobbyChan.EGaRule.none)
            {
                switch (((LobbyChan)GameManager.Instance.selectedLobby).config.NUOI_GA_RULE)
                {
                    case LobbyChan.EGaRule.NuoiGa:
                        chicken.transform.Find("1. Chicken").GetComponent<UISprite>().spriteName = "ga_trong";
                        break;
                    case LobbyChan.EGaRule.GaNhai:
                        chicken.transform.Find("1. Chicken").GetComponent<UISprite>().spriteName = "ga_mai";
                        break;
                }
            }

            if (GameModelChan.YourController.isMaster)
            {
                NotificationView.ShowMessage("Thay đổi thành công", 3f);
            }

            #endregion
        }
        else if (command == "error")
        {
            #region ERROR FROM SERVER
            Debug.LogWarning(
                "Error Process From Server: "
                + (eso.variableExists("code") ? " code= " + eso.getInteger("code") + " - " : "")
                + eso.getString("detail"));
            if (eso.variableExists("code"))
                switch (eso.getInteger("code"))
                {
                    case 11:
                        NotificationView.ShowMessage("Bạn không thể đổi chỗ người chơi khác trong khi ván bài đang diễn ra.");
                        break;
                }
            GameModelChan.game.button.UpdateButton();
            #endregion
        }
        else if (command == "warning")
        {
            #region Warning FROM SERVER
            Debug.LogWarning(
                "Warning Process From Server: "
                + (eso.variableExists("code") ? " code= " + eso.getInteger("code") + " - " : "")
                + eso.getString("detail"));

            if (eso.variableExists("detail"))
            {
                GameplayWarningView.Warning(eso.getString("detail"), 3f);

                //Client check đánh bài lỗi.
                if (eso.variableExists("code") && DefinedError.IsDiscardError(eso.getInteger("code")))
                {
                    Debug.LogWarning("Error Code: " + eso.getInteger("code"));

                    int inTurn = GameModelChan.IndexInTurn;
                    int lastInTurn = GameModelChan.IndexLastInTurn;

                    //Trước đó có đổi turn rồi nên phải đổi lại
                    GameModelChan.IndexInTurn = GameModelChan.IndexLastInTurn;
                    PlayerControllerChan p = GameModelChan.GetPlayer(GameModelChan.IndexInTurn);
                    ECard cardInTrash = p.mCardTrash[p.mCardTrash.Count - 1];
                    p.mCardTrash.Remove(cardInTrash);
                    p.mCardHand.Add(cardInTrash);
                    cardInTrash.UpdateParent(GameModelChan.IndexInTurn);

                    GameModelChan.MiniState = GameModelChan.EGameStateMini.discard;

                    if (eso.getInteger("code") == DefinedError.GetCodeError(DefinedError.GameplayError.DISCARD_ACTION_DENIED))
                    {
                        if (GameModelChan.YourController != null)
                        {
                            UpdateHand(GameModelChan.YourController.slotServer, 0f);
                        }
                        GameModelChan.IndexInTurn = inTurn;
                        GameModelChan.IndexLastInTurn = lastInTurn;
                    }
                    else
                        GameModelChan.SortHand();
                }
            }

            GameModelChan.game.button.UpdateButton();
            #endregion
        }
        else if (command == "userKicked")
        {
            #region Kick Player
            if (eso.variableExists("textNotification"))
            {
                int id = eso.getInteger("id");
                string notification = eso.getString("textNotification");
                Debug.LogWarning(eso.getString("reason"));
                if (id == 0)
                    NotificationView.ShowMessage(notification);
                else if (id == 1)
                    Common.MessageRecharge("Bạn đã không còn đủ tiền để tiếp tục chơi.");
                OnQuitGame(false);
            }
            #endregion
        }
        else if (command == "updateGame")
        {
            #region UPDATE GAME
            if (GameManager.Instance.ListCuocU.Count == 0)
            {
                GameManager.Server.DoRequestGameCommand("getCuoc");
            }
            ((LobbyChan)GameManager.Instance.selectedLobby).SetDataJoinLobby(eso);
            // bao gio co ga thi bo comment di 
            if (((LobbyChan)GameManager.Instance.selectedLobby).config.NUOI_GA_RULE != LobbyChan.EGaRule.none)
            {

                chicken.SetActive(true);
                switch (((LobbyChan)GameManager.Instance.selectedLobby).config.NUOI_GA_RULE)
                {
                    case LobbyChan.EGaRule.NuoiGa:
                        chicken.transform.Find("1. Chicken").GetComponent<UISprite>().spriteName = "ga_trong";
                        break;
                    case LobbyChan.EGaRule.GaNhai:
                        chicken.transform.Find("1. Chicken").GetComponent<UISprite>().spriteName = "ga_mai";
                        break;
                }
                if (eso.variableExists("moneyGa"))
                    chicken.transform.Find("2. lbMoney").GetComponent<UILabel>().text = eso.getLong("moneyGa") > 0 ? Utility.Convert.Chip(eso.getLong("moneyGa")) + "" : "0";
                else
                    chicken.transform.Find("2. lbMoney").GetComponent<UILabel>().text = "";
            }
            EsObject[] players = eso.getEsObjectArray("players");

            int isAddPlayerSlot = -1;
            foreach (EsObject obj in players)
            {
                PlayerControllerChan p = new PlayerControllerChan(obj);
                if (GameModelChan.GetPlayer(p.slotServer) == null)
                {
                    GameModelChan.SetPlayer(p.slotServer, p);
                    isAddPlayerSlot = p.slotServer;
                }
                else
                {
                    GameModelChan.GetPlayer(p.slotServer).SetDataPlayer(obj);
                }
            }
            if (eso.variableExists("waitingPlayers"))
            {
                EsObject[] waitingPlayer = eso.getEsObjectArray("waitingPlayers");
                foreach (EsObject obj in waitingPlayer)
                {
                    GameModelChan.ListWaitingPlayer.Add(new PlayerControllerChan(obj));
                    Listener.RegisterEventPlayerListChanged(new PlayerControllerChan(obj), false);
                }
                HeaderMenu.Instance.LoadListWaitingView();
            }
            GameModelChan.CreateNewGame();

            GameModelChan.ActiveState(GameModelChan.ConvertGameState(eso.getString("gameState")));

            Debug.Log("Load data start-game: " + GameModelChan.CurrentState.ToString());

            #region Người chơi tham gia khi trận đấu đang diễn ra.
            if (GameModelChan.CurrentState == GameModelChan.EGameState.playing)
            {
                deck.SetActive(true);

                foreach (EsObject obj in players)
                {
                    int slotIndex = obj.getInteger("slotIndex");
                    int handSize = GameModelChan.GetPlayer(slotIndex).handSize;
                    GameModelChan.GetPlayer(slotIndex).mCardHand.Clear();
                    if (obj.variableExists("hand") == false)
                    {
                        //while (handSize >= 1)
                        //{
                        //    GameModel.DeckCount--;
                        //    Card card = new Card(slotIndex);
                        //    card.Instantiate();
                        //    GameModel.GetPlayer(slotIndex).mCardHand.Add(card);
                        //    handSize--;
                        //}
                    }
                    else
                    {
                        if (obj.variableExists("hand"))
                        {
                            Array.ForEach<int>(obj.getIntegerArray("hand"), cardId =>
                            {
                                ChanCard card = new ChanCard(slotIndex, cardId);
                                card.Instantiate();
                                GameModelChan.GetPlayer(slotIndex).mCardHand.Add(card);
                                card.UpdateParent(slotIndex);
                            });
                        }
                    }
                    if (obj.variableExists("stolen"))
                    {
                        EsObject[] arrStolenObj = obj.getEsObjectArray("stolen");
                        foreach (EsObject stolensEsObject in arrStolenObj)
                        {
                            int index = 0;
                            StealCard stealCard = new StealCard();
                            stealCard.player = GameModelChan.GetPlayer(slotIndex);
                            EsObject[] arrMelds = stolensEsObject.getEsObjectArray("meld");
                            foreach (EsObject meldEsObject in arrMelds)
                            {
                                ChanCard card = new ChanCard(slotIndex, meldEsObject.getInteger("id"));
                                card.Instantiate();
                                if (index == 0)
                                {
                                    bool isDraw = meldEsObject.variableExists("cardDraw") ? true : false;
                                    if (isDraw)
                                        card.isDrawFromDeck = isDraw;
                                    card.originSide = GameModelChan.GetLastPlayer(slotIndex).mSide;
                                }
                                stealCard.steals.Add(card);
                                index++;
                            }
                            GameModelChan.GetPlayer(slotIndex).mCardSteal.Add(stealCard);
                            stealCard.steals.ForEach(c => { c.UpdateParent(slotIndex); });
                        }
                    }
                    if (obj.variableExists("trash"))
                    {
                        EsObject[] trash = obj.getEsObjectArray("trash");
                        foreach (EsObject cardValue in trash)
                        {

                            GameModelChan.DeckCount--;
                            ChanCard card = new ChanCard(slotIndex, cardValue.getInteger("id"));
                            card.Instantiate();
                            bool isDraw = cardValue.variableExists("cardDraw") ? true : false;
                            if (isDraw)
                                card.isDrawFromDeck = isDraw;
                            //Card card = new Card(slotIndex, cardValue);
                            GameModelChan.GetPlayer(slotIndex).mCardTrash.Add(card);
                            card.UpdateParent(slotIndex);
                        }
                    }
                    if (obj.variableExists("melds"))
                    {
                        EsObject[] arrMeldsObj = obj.getEsObjectArray("melds");
                        foreach (EsObject meldEsObject in arrMeldsObj)
                        {
                            int[] meld = meldEsObject.getIntegerArray("meld");
                            foreach (int cardValue in meld)
                            {
                                ChanCard card = new ChanCard(slotIndex, cardValue);
                                card.Instantiate();
                                GameModelChan.GetPlayer(slotIndex).mCardHand.Add(card);
                                card.UpdateParent(slotIndex);
                            }
                            GameModelChan.GetPlayer(slotIndex).mCardMelds.Add(new Meld(meld, GameModelChan.GetPlayer(slotIndex)));
                        }
                    }
                }
                UpdateHand();

                PlayerControllerChan playerInTurn = GameModelChan.ListPlayerInGame.Find(p => p.PlayerState > EPlayerController.EPlayerState.waitingForTurn && p.PlayerState <= EPlayerController.EPlayerState.fullLaying);
                if (playerInTurn == null)
                {
                    //Đang ở bước chia bài
                }
                else
                {
                    GameModelChan.IndexInTurn = playerInTurn.slotServer;

                    if (GameModelChan.YourController != null && GameModelChan.YourController.slotServer == GameModelChan.IndexInTurn)
                    {

                        if (GameModelChan.YourController.PlayerState == EPlayerController.EPlayerState.fullLaying)
                            GameModelChan.MiniState = GameModelChan.EGameStateMini.wait_player_xuong;
                        else
                        {
                            if (GameManager.GAME == EGame.TLMN)
                            {
                                GameModelChan.MiniState = GameModelChan.EGameStateMini.discard;
                            }
                            else
                            {
                                if (GameModelChan.YourController.PlayerState == EPlayerController.EPlayerState.inTurnDisCard)
                                    GameModelChan.MiniState = GameModelChan.EGameStateMini.discard;
                                else
                                    GameModelChan.MiniState = GameModelChan.EGameStateMini.stealCard_or_draw;
                            }
                        }
                    }
                }
                GameModelChan.DealCardDone = true;
            }
            #endregion

            if (eso.variableExists("totalTime") && eso.variableExists("remainingTime"))
                StartTimeRemaining(eso.getInteger("totalTime") / 1000f, eso.getInteger("remainingTime") / 1000f);
            else if (eso.variableExists("timeAutoDealCard"))
                StartTimeAutoDeal(eso.getInteger("timeAutoDealCard") / 1000f);

            if (isAddPlayerSlot > -1)
                Listener.RegisterEventPlayerListChanged(GameModelChan.GetPlayer(isAddPlayerSlot), false);

            if (GameModelChan.ListPlayer.Count > 1)
            {
                if (GameManager.OldScene == ESceneName.LobbyChan)
                    GameManager.Server.DoLeaveRoom(GameManager.Instance.selectedChannel.zoneId, GameManager.Instance.selectedChannel.roomId);
            }
            #endregion


            if (eso.variableExists("gameDetails"))
            {
                EsObject gameDetails = eso.getEsObject("gameDetails");
                if (gameDetails.variableExists("moneyType"))
                {
                    GameManager.PlayGoldOrChip = gameDetails.getString("moneyType");

                    infoBanChoi.transform.Find("Chip").gameObject.SetActive(false);
                    infoBanChoi.transform.Find("Gold").gameObject.SetActive(false);
                    levelInforView.transform.Find("Chip").gameObject.SetActive(false);
                    levelInforView.transform.Find("Gold").gameObject.SetActive(false);

                    if (GameManager.PlayGoldOrChip == "chip")
                    {
                        infoBanChoi.transform.Find("Chip").gameObject.SetActive(true);
                        levelInforView.transform.Find("Chip").gameObject.SetActive(true);
                    }

                    if (GameManager.PlayGoldOrChip == "gold")
                    {
                        infoBanChoi.transform.Find("Gold").gameObject.SetActive(true);
                        levelInforView.transform.Find("Gold").gameObject.SetActive(true);
                    }

                    for (int i = 0; i < playerSlot.Length; i++)
                    {
                        if (playerSlot[i].Find("Player " + i + 1) != null)
                        {
                            playerSlot[i].Find("Player " + i + 1).GetComponent<CUIPlayerChan>().UpdateInfo();
                        }
                    }
                }
            }

        }
        else if (command == "updateGameToWaitingPlayer")
        {
            #region UPDATE LẠI GAME KHI CÓ MỘT NGƯỜI Ở DANH SÁCH WAITING VÀO BÀN CHƠI
            GameModelChan.RemoveAllPlayerWhenAddWaitingPlayer();
            yield return new WaitForEndOfFrame();
            EsObject[] players = eso.getEsObjectArray("players");

            int isAddPlayerSlot = -1;
            foreach (EsObject obj in players)
            {
                PlayerControllerChan p = new PlayerControllerChan(obj);
                if (GameModelChan.GetPlayer(p.slotServer) == null)
                {
                    GameModelChan.SetPlayer(p.slotServer, p);
                    isAddPlayerSlot = p.slotServer;
                }
                else
                {
                    GameModelChan.GetPlayer(p.slotServer).SetDataPlayer(obj);
                }
            }
            if (eso.variableExists("waitingPlayers"))
            {
                EsObject[] waitingPlayer = eso.getEsObjectArray("waitingPlayers");
                foreach (EsObject obj in waitingPlayer)
                {
                    GameModelChan.ListWaitingPlayer.Add(new PlayerControllerChan(obj));
                    HeaderMenu.Instance.LoadListWaitingView();
                }
            }
            GameModelChan.CreateNewGame();

            GameModelChan.ActiveState(GameModelChan.ConvertGameState(eso.getString("gameState")));

            if (eso.variableExists("totalTime") && eso.variableExists("remainingTime"))
                StartTimeRemaining(eso.getInteger("totalTime") / 1000f, eso.getInteger("remainingTime") / 1000f);
            else if (eso.variableExists("timeAutoDealCard"))
                StartTimeAutoDeal(eso.getInteger("timeAutoDealCard") / 1000f);
            //if (isAddPlayerSlot > -1)
            //    Listener.RegisterEventPlayerListChanged(GameModel.GetPlayer(isAddPlayerSlot), false);
            #endregion
        }
        else if (command == "notifyGa")
        {
            #region UPDATE KHI CO NGUOI AN GA OR NAP TIEN VAO GA
            if (eso.variableExists("textNotification"))
            {
                string textNotification = eso.getString("textNotification");
                if (!string.IsNullOrEmpty(textNotification))
                {
                    if (!chicken.gameObject.activeInHierarchy)
                        chicken.gameObject.SetActive(true);
                    notification.ShowNotification(textNotification, 4f);
                }
            }
            GameManager.Instance.FunctionDelay(delegate()
            {
                string targetPlayer = null;
                if (eso.variableExists("targetPlayer"))
                    targetPlayer = eso.getString("targetPlayer");
                if (!string.IsNullOrEmpty(targetPlayer))
                {
                    if (eso.variableExists("players"))
                    {
                        EsObject[] players = eso.getEsObjectArray("players");
                        Array.ForEach<EsObject>(players, p => MoneyExchangeView.Create(p.getString(Fields.PLAYER.USERNAME), targetPlayer));
                    }
                }
            }, 0.25f);
            GameManager.Instance.FunctionDelay(delegate()
            {
                if (eso.variableExists("moneyGa"))
                {
                    chicken.transform.Find("2. lbMoney").GetComponent<UILabel>().text = eso.getLong("moneyGa") > 0 ? Utility.Convert.Chip(eso.getLong("moneyGa")) + "" : "0";
                }
            }, MoneyExchangeView.TIME_EFFECT + 0.25f);
            #endregion
        }
        else if (command == "notify")
        {
            #region SHOW NOTIFY
            if (eso.variableExists("textNotification"))
            {
                string notifycation = eso.getString("textNotification");
                lbDiscard.text = notifycation;
                SoundGameplay.Instances.PlaySoundInGame(152, null);
                yield return new WaitForSeconds(3f);
                StartCoroutine(FadeTextNotifyCenterScreen(3f));
            }
            #endregion
        }
        else if (command == "betting")
        {
            if (action == "checkValidBettingGaNgoai")
            {
                #region SHOW BETTING GA NGOAI
                BettingView bettingView = BettingView.Instance;
                bettingView.ShowPanelJoin();
                bettingView.OnBettingValueHandler(eso);
                #endregion
            }
            else if (action == "getIndividualCards")
            {
                if (GameObject.Find("__BettingView") != null)
                    BettingView.Instance.OnShowIndividualCard(eso);
            }
            else if (action == "getLog")
            {
                if (GameObject.Find("__BettingView") != null)
                    BettingView.Instance.OnShowPreViewBetting(eso);
            }
            else if (action == "bettingGaNgoai")
            {
                if (GameObject.Find("__BettingView") != null)
                {
                    BettingView.Instance.InitBetting(eso);
 
                }
                else
                {
                    if (dicUserBetting.ContainsKey(eso.getString("userName")))
                        dicUserBetting[eso.getString("userName")] = true;
                    else
                        dicUserBetting.Add(eso.getString("userName"), false);
                    if (!iconBettingUp.active)
                        NGUITools.SetActive(iconBettingUp.gameObject, true);
                }
            }

        }
        else if (command == "updatePriorityWaitingPlayers")
        {
            if (eso.variableExists("userName"))
            {
                foreach (PlayerControllerChan player in GameModelChan.ListWaitingPlayer)
                {
                    if (player.username == eso.getString("userName"))
                        player.isPriority = true;
                    else
                        player.isPriority = false;
                }
                HeaderMenu.Instance.LoadListWaitingView();
                Listener.RegisterEventWaitingChangePriority();
            }
        }
        if (command != "finishGame" && command != "updateHand")
        {
            yield return new WaitForSeconds(0.1f);
            IsProcesResonseDone = true;
        }

    }

    void DoUpdateGame(EsObject eso)
    {
        #region UPDATE GAME
        if (GameManager.Instance.ListCuocU.Count == 0)
        {
            GameManager.Server.DoRequestGameCommand("getCuoc");
        }
        ((LobbyChan)GameManager.Instance.selectedLobby).SetDataJoinLobby(eso);
        // bao gio co ga thi bo comment di 
        if (((LobbyChan)GameManager.Instance.selectedLobby).config.NUOI_GA_RULE != LobbyChan.EGaRule.none)
        {

            chicken.SetActive(true);
            switch (((LobbyChan)GameManager.Instance.selectedLobby).config.NUOI_GA_RULE)
            {
                case LobbyChan.EGaRule.NuoiGa:
                    chicken.transform.Find("1. Chicken").GetComponent<UISprite>().spriteName = "ga_trong";
                    break;
                case LobbyChan.EGaRule.GaNhai:
                    chicken.transform.Find("1. Chicken").GetComponent<UISprite>().spriteName = "ga_mai";
                    break;
            }
            if (eso.variableExists("moneyGa"))
                chicken.transform.Find("2. lbMoney").GetComponent<UILabel>().text = eso.getLong("moneyGa") > 0 ? Utility.Convert.Chip(eso.getLong("moneyGa")) + "" : "0";
            else
                chicken.transform.Find("2. lbMoney").GetComponent<UILabel>().text = "";
        }
        EsObject[] players = eso.getEsObjectArray("players");

        int isAddPlayerSlot = -1;
        foreach (EsObject obj in players)
        {
            PlayerControllerChan p = new PlayerControllerChan(obj);
            if (GameModelChan.GetPlayer(p.slotServer) == null)
            {
                GameModelChan.SetPlayer(p.slotServer, p);
                isAddPlayerSlot = p.slotServer;
            }
            else
            {
                GameModelChan.GetPlayer(p.slotServer).SetDataPlayer(obj);
            }
        }
        if (eso.variableExists("waitingPlayers"))
        {
            EsObject[] waitingPlayer = eso.getEsObjectArray("waitingPlayers");
            foreach (EsObject obj in waitingPlayer)
            {
                GameModelChan.ListWaitingPlayer.Add(new PlayerControllerChan(obj));
                Listener.RegisterEventPlayerListChanged(new PlayerControllerChan(obj), false);
            }
            HeaderMenu.Instance.LoadListWaitingView();
        }
        GameModelChan.CreateNewGame();

        GameModelChan.ActiveState(GameModelChan.ConvertGameState(eso.getString("gameState")));

        Debug.Log("Load data start-game: " + GameModelChan.CurrentState.ToString());

        #region Người chơi tham gia khi trận đấu đang diễn ra.
        if (GameModelChan.CurrentState == GameModelChan.EGameState.playing)
        {
            deck.SetActive(true);

            foreach (EsObject obj in players)
            {
                int slotIndex = obj.getInteger("slotIndex");
                int handSize = GameModelChan.GetPlayer(slotIndex).handSize;
                GameModelChan.GetPlayer(slotIndex).mCardHand.Clear();
                if (obj.variableExists("hand") == false)
                {
                    //while (handSize >= 1)
                    //{
                    //    GameModel.DeckCount--;
                    //    Card card = new Card(slotIndex);
                    //    card.Instantiate();
                    //    GameModel.GetPlayer(slotIndex).mCardHand.Add(card);
                    //    handSize--;
                    //}
                }
                else
                {
                    if (obj.variableExists("hand"))
                    {
                        Array.ForEach<int>(obj.getIntegerArray("hand"), cardId =>
                        {
                            ChanCard card = new ChanCard(slotIndex, cardId);
                            card.Instantiate();
                            GameModelChan.GetPlayer(slotIndex).mCardHand.Add(card);
                            card.UpdateParent(slotIndex);
                        });
                    }
                }
                if (obj.variableExists("stolen"))
                {
                    EsObject[] arrStolenObj = obj.getEsObjectArray("stolen");
                    foreach (EsObject stolensEsObject in arrStolenObj)
                    {
                        int index = 0;
                        StealCard stealCard = new StealCard();
                        stealCard.player = GameModelChan.GetPlayer(slotIndex);
                        EsObject[] arrMelds = stolensEsObject.getEsObjectArray("meld");
                        foreach (EsObject meldEsObject in arrMelds)
                        {
                            ChanCard card = new ChanCard(slotIndex, meldEsObject.getInteger("id"));
                            card.Instantiate();
                            if (index == 0)
                            {
                                bool isDraw = meldEsObject.variableExists("cardDraw") ? true : false;
                                if (isDraw)
                                    card.isDrawFromDeck = isDraw;
                                card.originSide = GameModelChan.GetLastPlayer(slotIndex).mSide;
                            }
                            stealCard.steals.Add(card);
                            index++;
                        }
                        GameModelChan.GetPlayer(slotIndex).mCardSteal.Add(stealCard);
                        stealCard.steals.ForEach(c => { c.UpdateParent(slotIndex); });
                    }
                }
                if (obj.variableExists("trash"))
                {
                    EsObject[] trash = obj.getEsObjectArray("trash");
                    foreach (EsObject cardValue in trash)
                    {

                        GameModelChan.DeckCount--;
                        ChanCard card = new ChanCard(slotIndex, cardValue.getInteger("id"));
                        card.Instantiate();
                        bool isDraw = cardValue.variableExists("cardDraw") ? true : false;
                        if (isDraw)
                            card.isDrawFromDeck = isDraw;
                        //Card card = new Card(slotIndex, cardValue);
                        GameModelChan.GetPlayer(slotIndex).mCardTrash.Add(card);
                        card.UpdateParent(slotIndex);
                    }
                }
                if (obj.variableExists("melds"))
                {
                    EsObject[] arrMeldsObj = obj.getEsObjectArray("melds");
                    foreach (EsObject meldEsObject in arrMeldsObj)
                    {
                        int[] meld = meldEsObject.getIntegerArray("meld");
                        foreach (int cardValue in meld)
                        {
                            ChanCard card = new ChanCard(slotIndex, cardValue);
                            card.Instantiate();
                            GameModelChan.GetPlayer(slotIndex).mCardHand.Add(card);
                            card.UpdateParent(slotIndex);
                        }
                        GameModelChan.GetPlayer(slotIndex).mCardMelds.Add(new Meld(meld, GameModelChan.GetPlayer(slotIndex)));
                    }
                }
            }
            UpdateHand();

            PlayerControllerChan playerInTurn = GameModelChan.ListPlayerInGame.Find(p => p.PlayerState > EPlayerController.EPlayerState.waitingForTurn && p.PlayerState <= EPlayerController.EPlayerState.fullLaying);
            if (playerInTurn == null)
            {
                //Đang ở bước chia bài
            }
            else
            {
                GameModelChan.IndexInTurn = playerInTurn.slotServer;

                if (GameModelChan.YourController != null && GameModelChan.YourController.slotServer == GameModelChan.IndexInTurn)
                {

                    if (GameModelChan.YourController.PlayerState == EPlayerController.EPlayerState.fullLaying)
                        GameModelChan.MiniState = GameModelChan.EGameStateMini.wait_player_xuong;
                    else
                    {
                        if (GameManager.GAME == EGame.TLMN)
                        {
                            GameModelChan.MiniState = GameModelChan.EGameStateMini.discard;
                        }
                        else
                        {
                            if (GameModelChan.YourController.PlayerState == EPlayerController.EPlayerState.inTurnDisCard)
                                GameModelChan.MiniState = GameModelChan.EGameStateMini.discard;
                            else
                                GameModelChan.MiniState = GameModelChan.EGameStateMini.stealCard_or_draw;
                        }
                    }
                }
            }
            GameModelChan.DealCardDone = true;
        }
        #endregion

        if (eso.variableExists("totalTime") && eso.variableExists("remainingTime"))
            StartTimeRemaining(eso.getInteger("totalTime") / 1000f, eso.getInteger("remainingTime") / 1000f);
        else if (eso.variableExists("timeAutoDealCard"))
            StartTimeAutoDeal(eso.getInteger("timeAutoDealCard") / 1000f);

        if (isAddPlayerSlot > -1)
            Listener.RegisterEventPlayerListChanged(GameModelChan.GetPlayer(isAddPlayerSlot), false);

        if (GameModelChan.ListPlayer.Count > 1)
        {
            if (GameManager.OldScene == ESceneName.LobbyChan)
                GameManager.Server.DoLeaveRoom(GameManager.Instance.selectedChannel.zoneId, GameManager.Instance.selectedChannel.roomId);
        }
        #endregion
    }

    /// <summary>
    /// khi thay đổi thông tin trong game thì cập nhật lại dữ liệu game hiện tại
    /// </summary>
    void ResetRoomInfo(EsObject esobj)
    {
        if (esobj.variableExists("gameDetails"))
        {
            EsObject gameDetails = esobj.getEsObject("gameDetails");

            if (gameDetails.variableExists("config"))
            {
                EsObject esConfig = gameDetails.getEsObject("config");
                if (esConfig.variableExists("description"))
                {
                    GameInfo gameinfo = GameObject.FindObjectOfType<GameInfo>();
                    if (gameinfo)
                    {
                        gameinfo.lbName.text = ((LobbyChan)GameManager.Instance.selectedLobby).gameIndex + " - " + esConfig.getString("description");
                    }
                }
            }
        }
    }
    System.Collections.IEnumerator FadeTextNotifyCenterScreen(float time)
    {
        yield return new WaitForSeconds(time);
        lbDiscard.text = "";
    }

    public void RemoveRoomMasterIcon(PlayerControllerChan p)
    {
        p.isMaster = false;
        p.cuiPlayer.UpdateInfo();
    }
    /// <summary>
    /// Thống kê lịch sử trận đấu
    /// </summary>
    public class Summary
    {
        public string sourcePlayer, targetPlayer;
        public string description;
        public long moneyExchange;

        /// <summary>
        /// Lưu lịch sử trao đổi tiền
        /// </summary>
        public EAction action;
        public enum EAction
        {
            LOSS_FINISH_TYPE = 0,
            DEN_LANG_FINISH_TYPE = 1,
            GA__NHAI = 2,
            NUOI_GA = 3,
            AN_GA_NUOI = 4,
            GA_NGOAI = 5,
        }

    }

    /// <summary>
    /// Thông tin phỏm có thể gửi
    /// </summary>
    public class GiveCard
    {
        public int cardId;
        public int slotIndex;
        public int[] meldResponse;
        public Meld meld;
    }
}