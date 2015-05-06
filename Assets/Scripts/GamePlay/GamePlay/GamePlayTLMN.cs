using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Electrotank.Electroserver5.Api;

public class GamePlayTLMN : MonoBehaviour {

    #region Unity Editor
    public PlaymatTLMN mPlaymat;
    public Transform transformParentButton;
    public CUIHandle btReady, btStart,
        //btDraw, btDiscard, btSorted,
        btStealCard, btGuiBai, btHaBai,
        btChatHang, btFulllaying, btBoChon;

    
    public UIDisableButton btDraw, btDiscard, btSorted;
    public GameObject deck, imageTurnLayMeld;
    public EasyFontTextMesh lbDiscard;
    #endregion
    /// <summary>
    /// Có phải lượt đánh bài mới hay không
    /// (Trả về true khi lượt đánh bài là mới).
    /// </summary>
    [HideInInspector]
    public bool newTurn;
    public bool? allowChatHang = null;
    public bool isHideOneCard;
    /// <summary>
    /// Danh sách card card có hiệu ứng khi đánh ra.
    /// </summary>
    [HideInInspector]
    List<TLMNCard> listEffectCardPair = new List<TLMNCard>();
    /// <summary>
    /// Danh sách những card sẽ bị remove khỏi tay để hiện hiệu ứng.
    /// </summary>
    [HideInInspector]
    public List<ECard> cardEffectFinishGame = new List<ECard>();

    [HideInInspector]
    public CardControllerTLMN cardController = new CardControllerTLMN();
    /// <summary>
    /// Lưu cái type của finish game để check khi tạo effect end game
    /// </summary>
    [HideInInspector]
    public EFinishTypeTLMN gameFinishTypeTLMN;
    public enum EFinishTypeTLMN
    {
        NORMAL_DEMLA,   //normal - Đếm lá
        NORMAL_DEMLA_DUC_BA_BICH,
        NORMAL_XEPHANG,	//normal - Xếp hạng
        THANG_TRANG		//thang trang
    }
    [HideInInspector]
    public EFinishByThangTrangType gameFinishThangTrangType;
    public enum EFinishByThangTrangType
    {
        // Van dau tien
        TU_QUY_BANG_CAI,
        BA_DOI_THONG_CO_CAI,
        BON_DOI_THONG_CO_CAI,
        // Van thu 2 tro di
        HAI_TU_QUY,
        TU_QUY_2,
        NAM_DOI_THONG,
        SAU_DOI_THONG,
        SANH_RONG,
        SANH_3_DEN_A,
        NONE
    }

    /// <summary>
    /// Xử lý button gameplay
    /// </summary>
    [HideInInspector]
    public YourButtonControllerTLMN button = new YourButtonControllerTLMN();

    /// <summary>
    /// Listener lắng nghe các Event trong gameplay
    /// </summary>
    [HideInInspector]
    public GameplayListener Listener = new GameplayListener();

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
    /// Người chiến thắng
    /// </summary>
    [HideInInspector]
    public PlayerControllerTLMN winner;

    #region Time Count Down
    float _timeCountDownTurm = 0;
    public float TimeCountDown
    {
        get { return _timeCountDownTurm; }
        set
        {
            _timeCountDownTurm = value;
            GameModelTLMN.GetPlayer(GameModelTLMN.IndexInTurn).cuiPlayer.StartTime(_timeCountDownTurm);
        }
    }

    public void StartTimeRemaining(float totalTime, float remainingTime)
    {
        _timeCountDownTurm = totalTime;
        GameModelTLMN.GetPlayer(GameModelTLMN.IndexInTurn).cuiPlayer.StartRemainingTime(remainingTime);
    }
    List<ECard> lastFaceDown = new List<ECard>();
    public void UpdateTrashFaceDown()
    {
        lastFaceDown.Clear();
        while (GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp].Count > 0)
        {
            for (int i = 0; i < GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp][0].Count; i++)
            {
                GameObject obj = GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp][0][i].gameObject;
                obj.GetComponent<ECardTexture>().texture.depth = -1;
                obj.name = "Face Down " + GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp][0][i].ToString();

                if (obj.GetComponent<iTween>() != null && obj.GetComponent<iTween>().isRunning)
                    iTween.Stop(obj);

                //obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0f);

                System.Collections.Hashtable hash = new System.Collections.Hashtable();
                hash.Add("islocal", true);
                hash.Add("time", 0.5f);
                hash.Add("position", new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0f));
                iTween.MoveTo(obj, hash);

                GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp][0][i].CardId = -1;
            }
            lastFaceDown.AddRange(GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp][0]);
            GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceDown].Add(GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp][0]);
            GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp].RemoveAt(0);
        }
        GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp].Clear();
    }

    public void CreateEffectCardPair(List<ECard> list)
    {
        Debug.Log("CreateEffectCardPair !!!!");
        return;
        float time = 0.5f;
        foreach (TLMNCard c in list)
        {
            TLMNCard card = new TLMNCard(GameModelTLMN.YourController.slotServer, c.CardId);
            card.Instantiate();
            listEffectCardPair.Add(card);
        }
        for (int i = 0; i < listEffectCardPair.Count; i++)
        {
            ECard card = listEffectCardPair[i];
            iTween.MoveTo(card.gameObject, iTween.Hash("islocal", true, "time", time, "position", mPlaymat.GetLocationEffect(i, listEffectCardPair)));
            iTween.ScaleTo(card.gameObject, Vector3.one * 1.5f, time);
            iTween.RotateTo(card.gameObject, Vector3.zero, time);
            GameManager.Instance.FunctionDelay(delegate() { card.SetHightlight(); }, time);
        }
    }
    public void UpdateTrash(float time)
    {
        #region IN TRASH
        List<List<ECard>> listDiscardFaceUp = GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp];

        int indexInTotal = 0;
        for (int i = 0; i < listDiscardFaceUp.Count; i++)
        {
            for (int j = 0; j < listDiscardFaceUp[i].Count; j++)
            {
                if (lastFaceDown.Contains(listDiscardFaceUp[i][j])) continue;
                indexInTotal++;
                GameObject obj = listDiscardFaceUp[i][j].gameObject;
                //obj.GetComponent<ECardTexture>().texture.depth = i * 2;
                obj.GetComponent<ECardTexture>().texture.depth = indexInTotal;
                iTween.MoveTo(obj, iTween.Hash("islocal", true, "time", time, "position", mPlaymat.GetLocationTrashFaceUp(i, j, indexInTotal)));
                iTween.RotateTo(obj, Vector3.zero, time);
                iTween.ScaleTo(obj, Vector3.one, time);
            }
        }
        #endregion
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
                btStart.gameObject.SetActive(GameModelTLMN.YourController.isMaster && GameModelTLMN.CurrentState == GameModelTLMN.EGameState.waitingForReady);
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
    List<ECard> clickCardList = new List<ECard>();

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
        GameModelTLMN.model = new GameModelTLMN(this);
        HeaderMenu.Instance.OnClickButtonBackCallBack = delegate()
        {
            if (GameObject.Find("__Prefab Quit Gameplay") == null)
                GPQuitTLMN.Create();
        };

        UIDisableButton.AddClick(btDiscard, OnClickDiscard);
        UIDisableButton.AddClick(btSorted, OnClickSort);
        UIDisableButton.AddClick(btDraw, OnClickDraw);

        CUIHandle.AddClick(btReady, OnClickReady);
        CUIHandle.AddClick(btStart, OnClickStart);
        CUIHandle.AddClick(deck.transform.Find("btDrawInDeck").GetComponent<CUIHandle>(), OnClickDraw);
        CUIHandle.AddClick(btStealCard, OnClickSteadCard);
        CUIHandle.AddClick(btHaBai, OnClickMeldDown);
        CUIHandle.AddClick(btFulllaying, OnClickFulllaying);
        CUIHandle.AddClick(btGuiBai, OnClickAddMeld);
        CUIHandle.AddClick(btChatHang, OnClickChatHang);
        CUIHandle.AddClick(btBoChon, OnClickUnPick);

        GameManager.Server.EventPluginMessageOnProcess += ProcessOnGameplay;
        GameManager.Server.EventLeaveRoom += OnLeaveRoom;
    }
    void OnClickUnPick(GameObject go)
    {
        UpdateHand(GameModelTLMN.YourController.slotServer, 0f);
        GameManager.Instance.FunctionDelay(delegate() { HideCardAll(); }, 0.2f);
    }
    void OnClickDiscard(GameObject go)
    {
        //btDiscard.SetStatus(true, false);
        if (ListClickCard.Count > 0 && GameModelTLMN.CanDiscard(ListClickCard))
            OnDiscard(ListClickCard);
    }
    void OnClickDraw(GameObject go)
    {
        btDraw.SetStatus(true, false);

        if (GameManager.GAME == EGame.Phom)
            GameManager.Server.DoRequestGameAction("drawCard");
        else
        {
            GameManager.Server.DoRequestGameAction("outTurn");
            HideCardAll();
        }
    }
    void OnClickChatHang(GameObject go)
    {
        btChatHang.gameObject.SetActive(false);
        GameModelTLMN.game.button.UpdateButton();

        List<ECard> listDiscard = GameModelTLMN.game.cardController.GetFourPairs();
        if (listDiscard.Count > 0)
            OnDiscard(listDiscard);
    }
    public void OnDiscard(List<ECard> cardsList)
    {
        if (cardsList.Count == 0) return;

        int[] cardsId = new int[cardsList.Count];
        for (int i = 0; i < cardsList.Count; i++)
            cardsId[i] = cardsList[i].CardId;

        GameModelTLMN.Discard(GameModelTLMN.YourController.slotServer, cardsId);

        GameManager.Server.DoRequestPluginGame(Utility.SetEsObject(Fields.GAMEPLAY.PLAY, new object[] { 
            Fields.ACTION, "discard", 
            Fields.CARD.CARD_ID, cardsId 
        }));

        HideCardAll();
    }
    void OnDestroy()
    {
        GameModelTLMN.game = null;

        UIDisableButton.RemoveClick(btDiscard, OnClickDiscard);
        UIDisableButton.RemoveClick(btSorted, OnClickSort);
        UIDisableButton.RemoveClick(btDraw, OnClickDraw);

        CUIHandle.RemoveClick(btReady, OnClickReady);
        CUIHandle.RemoveClick(btStart, OnClickStart);
        if (deck != null)
            CUIHandle.RemoveClick(deck.transform.Find("btDrawInDeck").GetComponent<CUIHandle>(), OnClickDraw);
        CUIHandle.RemoveClick(btStealCard, OnClickSteadCard);
        CUIHandle.RemoveClick(btHaBai, OnClickMeldDown);
        CUIHandle.RemoveClick(btFulllaying, OnClickFulllaying);
        CUIHandle.RemoveClick(btGuiBai, OnClickAddMeld);
        CUIHandle.RemoveClick(btChatHang, OnClickChatHang);
        CUIHandle.RemoveClick(btBoChon, OnClickUnPick);

        if (!GameManager.IsExist) return;

        GameManager.Server.EventPluginMessageOnProcess -= ProcessOnGameplay;
        GameManager.Server.EventLeaveRoom -= OnLeaveRoom;
    }

    #region SEND REQUEST SERVER
    public void OnQuitGame(bool quitEndGame)
    {
        if (!quitEndGame)
        {
            //Khi Ấn Quit
            GameManager.Server.DoRequestGameAction("quitGame");
            DoJoinRoom();
        }
    }

    void DoJoinRoom()
    {
        if (GameManager.OldScene == ESceneName.LoginScreen || GameManager.Instance.mInfo.chip < ((ChannelTLMN)GameManager.Instance.selectedChannel).minimumMoney)
            GameManager.Server.DoJoinRoom(GameManager.Instance.channelRoom.zoneId, GameManager.Instance.channelRoom.roomId);
        else
            GameManager.Server.DoJoinRoom(GameManager.Instance.selectedChannel.zoneId, GameManager.Instance.selectedChannel.roomId);
    }

    void OnLeaveRoom(LeaveRoomEvent e)
    {
        if (e.RoomId == GameManager.Instance.selectedLobby.roomId && e.ZoneId == GameManager.Instance.selectedLobby.zoneId)
        {
            if (GameManager.OldScene == ESceneName.LoginScreen || GameManager.Instance.mInfo.chip < ((ChannelTLMN)GameManager.Instance.selectedChannel).minimumMoney)
                GameManager.LoadScene(ESceneName.ChannelTLMN);
            else
                GameManager.LoadScene(ESceneName.LobbyTLMN);
        }
    }

    void OnClickFulllaying(GameObject go)
    {
        go.SetActive(false);

        GameManager.Server.DoRequestGameAction("fullLaying");
    }

    /// <summary>
    /// Call from PlayerController.cs
    /// btAddMeldPrefab
    /// </summary>
    void OnClickAddMeldCard(GameObject go)
    {
        GamePlayTLMN.GiveCard giveCard = (GamePlayTLMN.GiveCard)go.GetComponent<UIContainerAnonymous>().intermediary;
        DoRequestAddMeldCard(giveCard);
    }

    void DoRequestAddMeldCard(GiveCard giveCard)
    {
        GameManager.Server.DoRequestPluginGame(Utility.SetEsObject(Fields.GAMEPLAY.PLAY, new object[] {
            Fields.ACTION, "addMeld",
            "cardsToAdd", new EsObject[] { 
                Utility.SetEsObject(null, new object[] {
                    Fields.CARD.CARD_ID, giveCard.cardId,
                    Fields.GAMEPLAY.PLAYER, GameModelTLMN.GetPlayer(giveCard.slotIndex).username,
                    "slotId", giveCard.slotIndex,
                    "cardInMeld", giveCard.meld.meld[0].CardId })}
         }));
    }

    void OnClickAddMeld(GameObject go)
    {
        go.SetActive(false);

        if (((LobbyTLMN)GameManager.Instance.selectedLobby).config.isAdvanceGame)
            DoRequestAddMeldCard(listGiveCard.Find(gc => gc.cardId == ListClickCard[0].CardId));
        else
        {
            List<EsObject> list = new List<EsObject>();
            GameModelTLMN.YourController.mCardHand.ForEach(c => { EsObject obj = new EsObject(); obj.setInteger(Fields.CARD.CARD_ID, c.CardId); list.Add(obj); });
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
            Fields.GAMEPLAY.MELDS, ListClickCard.Count > 2 ? ListClickCard.Select(c => c.CardId).ToArray() : GameModelTLMN.YourController.mCardHand.Select(c => c.CardId).ToArray()
        }));
    }

    public void OnClickSort(GameObject go)
    {
        btSorted.StopImpact(0.7f);

        if (canRequestSort)
        {
            //GameManager.Server.DoRequestGameAction("sortCard");
            sortList = cardController.SortCard();
            GameModelTLMN.SortHand();
            canRequestSort = false;
        }
        else
            GameModelTLMN.SortHand();
        GameModelTLMN.model.GetCardCollection();

        GameManager.Instance.FunctionDelay(delegate() { HideCardAll(); }, 0.6f);
    }

    /// <summary>
    /// Call from objStealCard PlayerController.cs
    /// btStealCardPrefab
    /// </summary>
    void OnProcessStealCard()
    {
        //canRequestSort = true;
        GameManager.Server.DoRequestGameAction("stealCard");
    }
    void OnClickSteadCard(GameObject go)
    {
        go.SetActive(false);

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

    void DoRequestCheckGiveCardAndMeld()
    {
        if (GameModelTLMN.IndexInTurn == GameModelTLMN.YourController.slotServer)
        {
            //GameManager.Server.DoRequestGameAction("checkGiveCard");
            listGiveCard = cardController.ListGiveCard();
            //GameManager.Server.DoRequestGameAction("checkMeld");
            meldList = cardController.ListMeld();
            GameModelTLMN.game.button.UpdateButton();
        }
    }
    #endregion

    /// <summary>
    /// Cập nhật lại UI của các Players
    /// </summary>
    public void UpdateUI()
    {
        if (GameManager.CurrentScene == ESceneName.GameplayTLMN)
            GameModelTLMN.ListPlayerInGame.ForEach(p => p.cuiPlayer.UpdateInfo());
    }

    void Update()
    {
        if ((int)GameModelTLMN.CurrentState >= (int)GameModelTLMN.EGameState.deal)
            if (deck.transform.FindChild("Label").GetComponent<UILabel>().text != GameModelTLMN.DeckCount.ToString())
                deck.transform.FindChild("Label").GetComponent<UILabel>().text = GameModelTLMN.DeckCount.ToString();
    }

    #region UpdateHand
    public void UpdateHand()
    {
        foreach (PlayerControllerTLMN p in GameModelTLMN.ListPlayerInGame)
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
        if (slotIndex == GameModelTLMN.YourController.slotServer)
            HideClickCard();

        PlayerControllerTLMN player = GameModelTLMN.GetPlayer(slotIndex);

        #region IN HAND
        int numberStolen = 0;
        for (int i = 0; i < player.mCardHand.Count; i++)
        {
            if (player.mCardHand[i].gameObject != null)
            {
                GameObject obj = player.mCardHand[i].gameObject;
                //iTween.Stop(obj);
                System.Collections.Hashtable hash = new System.Collections.Hashtable();
                hash.Add("islocal", true);
                hash.Add("time", time);

                if (player.mCardHand[i].originSide != player.mCardHand[i].currentSide && (player.mCardHand[i].currentSide == ESide.Enemy_1 || player.mCardHand[i].currentSide == ESide.Enemy_3)
                    ) //Những cây bài đã ăn được
                {
                    numberStolen++;
                    Vector3 position = mPlaymat.GetLocationHand(player, (numberStolen * 3) - 1);

                    position.z += (player.mSide == ESide.Enemy_2) ? -0.5f : 0.5f;

                    if (player.mSide == ESide.Enemy_1 || player.mSide == ESide.Enemy_3)
                        position.y += TLMNCardTexture.CARD_HEIGHT / 3;

                    hash.Add("position", position);
                    iTween.RotateTo(obj, Vector3.zero, time);
                }
                else
                {
                    hash.Add("position", mPlaymat.GetLocationHand(player, i - numberStolen));

                    if (GameModelTLMN.CurrentState == GameModelTLMN.EGameState.finalizing)
                        iTween.RotateTo(obj, Vector3.zero, time);
                    else
                        iTween.RotateTo(obj, mPlaymat.GetRotateHand(player), time);
                }

                player.mCardHand[i].cardTexture.texture.depth = i;

                iTween.MoveTo(obj, hash);
                if (player.mSide == ESide.You)
                    iTween.ScaleTo(obj, Vector3.one * 2, time);
                else
                    iTween.ScaleTo(obj, Vector3.one, time);
            }
        }
        #endregion

        #region IN MELDS

        for (int indexMeld = 0; indexMeld < player.mCardMelds.Count; indexMeld++)
        {
            player.mCardMelds[indexMeld].meld.Sort((c1, c2) => c1.CompareTo(c2));

            for (int i = 0; i < player.mCardMelds[indexMeld].meld.Count; i++)
            {
                if (player.mCardMelds[indexMeld].meld[i].gameObject != null)
                {
                    GameObject obj = player.mCardMelds[indexMeld].meld[i].gameObject;
                    obj.GetComponent<ECardTexture>().texture.depth = i;
                    //iTween.Stop(obj);
                    System.Collections.Hashtable hash = new System.Collections.Hashtable();
                    hash.Add("islocal", true);
                    hash.Add("time", time);
                    hash.Add("position", mPlaymat.GetLocationMeld(player, indexMeld, i));
                    iTween.MoveTo(obj, hash);
                    iTween.RotateTo(obj, Vector3.zero, time);
                    iTween.ScaleTo(obj, Vector3.one, time);
                }
            }
        }
        #endregion
    }
    #endregion

    #region Click CARD
    public void HideClickCard()
    {
        HideCard();
        HideCardAll();
    }

    const float INCREASE_VECTOR_Y = 15f;

    #region SINGLE CLICK CARD
    Vector3 oldVectorPosition = Vector3.one;
    void HideCard()
    {
        if (clickCard != null)
        {
            clickCard.transform.localPosition = oldVectorPosition;
            clickCard = null;
            isHideOneCard = true;
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
        isHideOneCard = false;
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
        if (GameModelTLMN.IsYourTurn)
        {
            GameModelTLMN.game.button.UpdateButtonDiscard();
        }
    }
    #endregion

    #region MULTI CLICK CARD
    List<Vector3> listVectorPosition = new List<Vector3>();
    public void CardClick(ECard card)
    {
        if (card != null)
        {
            if (clickCardList.Contains(card))
                HideCard(card);
            else
                ShowCard(card);

        }
        //else
        //    HideCardAll();

        GameModelTLMN.game.button.UpdateButtonDiscard();
        GameModelTLMN.game.button.UpdateButtonLayMeld();
    }

    void ShowCard(ECard card)
    {
        clickCardList.Add(card);
        listVectorPosition.Add(card.gameObject.transform.localPosition);

        card.gameObject.transform.localPosition = new Vector3(card.gameObject.transform.localPosition.x, card.gameObject.transform.localPosition.y + INCREASE_VECTOR_Y, card.gameObject.transform.localPosition.z);

        AudioManager.Instance.Play(AudioManager.SoundEffect.SelectCard);
        isHideOneCard = false;
    }

    void HideCard(ECard card)
    {
        if (clickCardList.Contains(card) == false) return;

        card.gameObject.transform.localPosition = listVectorPosition[clickCardList.IndexOf(card)];
        listVectorPosition.RemoveAt(clickCardList.IndexOf(card));
        clickCardList.Remove(card);
        isHideOneCard = true;
    }

    void HideCardAll()
    {
        while (clickCardList.Count > 0)
            HideCard(clickCardList[0]);

        GameModelTLMN.game.button.UpdateButtonDiscard();
    }
    #endregion

    /// <summary>
    /// Các card đang pick
    /// </summary>
    public List<ECard> ListClickCard
    {
        get
        {
            if (clickCard == null)
                return clickCardList;
            else
                return new List<ECard>(new ECard[1] { clickCard.GetComponent<TLMNCardTexture>().card });
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
        NORMAL,         //normal
        U_THUONG,       //playerFullLaying
        U_TRON,         //playerFullLaying10Cards 
        U_DEN_THUONG,   //playerFullLayingBySteal3Cards
        U_DEN_TRON,     //playerFullLaying10CardsBySteal3Cards
        U_XUONG          //playerFullLayingWithNoMeld
    }
    public void SetTypeFinishGame(string str)
    {
        if (str == "playerFullLaying")
            gameFinishType = EFinishType.U_THUONG;
        else if (str == "playerFullLaying10Cards")
            gameFinishType = EFinishType.U_TRON;
        else if (str == "playerFullLayingBySteal3Cards")
            gameFinishType = EFinishType.U_DEN_THUONG;
        else if (str == "playerFullLaying10CardsBySteal3Cards")
            gameFinishType = EFinishType.U_DEN_TRON;
        else if (str == "playerFullLayingWithNoMeld")
            gameFinishType = EFinishType.U_XUONG;
        else
            gameFinishType = EFinishType.NORMAL;
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
            GameModelTLMN.GetPlayer(index).SetDataPlayer(player);
        }
        else
        {
            PlayerControllerTLMN p = GameModelTLMN.GetPlayer(player.getString(Fields.PLAYER.USERNAME));
            p.SetDataPlayer(player);
            index = p.slotServer;
        }
    }

    /// <summary>
    /// Xử lý hạ phỏm từ server
    /// </summary>
    private void ProcessLayMelds(EsObject esoPlayer, EsObject esoMeld, ref int index, params string[] variable)
    {
        SetPlayerEsObject(esoPlayer, ref index, variable);

        if (esoMeld.variableExists(Fields.GAMEPLAY.MELDS))
        {
            EsObject[] lst = esoMeld.getEsObjectArray(Fields.GAMEPLAY.MELDS);
            foreach (EsObject obj in lst)
            {
                Meld meld = new Meld(obj.getIntegerArray("meld"), GameModelTLMN.GetPlayer(index));
                GameModelTLMN.GetPlayer(index).mCardMelds.Add(meld);
            }
            GameModelTLMN.LayMeld(index);
        }
    }

    /// <summary>
    /// Xử lý finishgame từ server
    /// </summary>
    private IEnumerator ProcessFinishGame(EsObject eso)
    {
        while (GameModelTLMN.ListPlayerInGame.Contains(GameModelTLMN.YourController) && GameModelTLMN.DealCardDone == false)
            yield return new WaitForFixedUpdate();

        #region Không cho phép người chơi click card hay Drag&Drop card khi xẩy ra hiệu ứng lúc finish Game
        UpdateHand();
        foreach (ECard c in GameModelTLMN.YourController.mCardHand)
            c.cardTexture.SetCollider(false);
        #endregion

        #region Thang trang Effect
        cardEffectFinishGame.Clear();

        if (eso.variableExists("winner"))
            winner = GameModelTLMN.GetPlayer(eso.getString("winner"));
        if (eso.variableExists("cards"))
        {
            foreach (int cardId in eso.getIntegerArray("cards"))
            {
                TLMNCard card = GameModelTLMN.GetCard_FromHandPlayer(winner.slotServer, cardId);
                cardEffectFinishGame.Add(card);
            }
        }
        #endregion

        if (eso.variableExists("lastPlayer"))
            SetPlayerEsObject(eso, ref GameModelTLMN.IndexLastInTurn, new string[] { "lastPlayer" });

        if (eso.variableExists("thangTrangType"))
            SetTypeFinishTTGameTLMN(eso.getString("finishType"), eso.getString("thangTrangType"));
        else
            SetTypeFinishNormalGameTLMN(eso.getString("finishType"), eso.getString("gameType"));

        if (eso.variableExists(Fields.CARD.CARD_ID))
        {
            newTurn = false;
            GameModelTLMN.Discard(GameModelTLMN.IndexLastInTurn, eso.getIntegerArray(Fields.CARD.CARD_ID));
            yield return new WaitForSeconds(0.5f);
        }

        EsObject result = eso.getEsObject("result");
        EsObject[] lstSummary = result.getEsObjectArray("summary");
        foreach (EsObject obj in lstSummary)
            GameModelTLMN.GetPlayer(obj.getString(Fields.PLAYER.USERNAME)).SetDataSummary(obj);

        summaryGame.Clear();
        EsObject[] lstAction = result.getEsObjectArray("actions");

        //GPChatView.MessageFromSystem("[FF0000]***** THỐNG KÊ. KẾT QUẢ TRẬN ĐẤU *****[-]\n");
        Listener.RegisterEventLogGame("[FFCC00]***** THỐNG KÊ. KẾT QUẢ TRẬN ĐẤU *****[-]\n");
        foreach (EsObject obj in lstAction)
        {
            Summary sum = new Summary();
            sum.sourcePlayer = obj.getString("sourcePlayer");
            sum.targetPlayer = obj.getString("targetPlayer");
            sum.description = obj.getString("description");
            long.TryParse(obj.getString("moneyExchange"), out sum.moneyExchange);
            sum.actionTLMN = (Summary.EActionTLMN)obj.getInteger(Fields.ACTION);
            Debug.Log("sum.sourcePlayer: " + sum.sourcePlayer + " sum.actionTLMN: " + sum.actionTLMN.ToString());

            summaryGame.Add(sum);
            //GPChatView.MessageFromSystem("[FF0000]" + sum.targetPlayer + "[-] đã nhận được [FF0000]" + sum.moneyExchange + "[-] từ người chơi [FF0000]" + sum.sourcePlayer + "[-] vì [FF0000]" + sum.description + "[-]\n");
            Listener.RegisterEventLogGame("[FFCC00]" + sum.targetPlayer + "[-] đã nhận được [FFCC00]" + sum.moneyExchange + "[-] từ người chơi [FFCC00]" + sum.sourcePlayer + "[-] vì [FFCC00]" + sum.description + "[-]\n");
        }

        _timeCountDownTurm = eso.getInteger("time") / 1000f; //Lưu time lại nhưng ko chạy đồng hồ
        GameModelTLMN.ActiveState(GameModelTLMN.EGameState.finalizing);
    }

    EsObject updateMoneyAfterFinishGame = null;
    /// <summary>
    /// Cập nhật lại tiền của người chơi sau khi hết thúc trận đấu
    /// </summary>
    public void UpdateUserInfo()
    {
        if (updateMoneyAfterFinishGame != null)
        {
            string field = updateMoneyAfterFinishGame.getString("field");
            if (field == "money")
                GameModelTLMN.GetPlayer(updateMoneyAfterFinishGame.getString(Fields.PLAYER.USERNAME)).UpdateMoney(updateMoneyAfterFinishGame.getString("value"));
            updateMoneyAfterFinishGame = null;
        }
    }

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
        if (!IsProcesResonseDone || listResponse.Count > 0)
        {
            Debug.LogWarning("Xếp hàng chờ: " + command + " - " + Utility.Convert.TimeToStringFull(DateTime.Now));
            listResponse.Add(new CommandEsObject(command, action, eso));
        }
        else
        {
            ///Chờ cho đến khi đã load được dữ liệu của trận đấu thì mới xử lý luồng gameplay
            StartCoroutine(_ProcessOnGameplay(command, action, eso));
        }
    }

    IEnumerator _ProcessOnGameplay(string command, string action, EsObject eso)
    {
        IsProcesResonseDone = false;

        while (isStartDone == false)
            yield return new WaitForEndOfFrame();

        if (command == "playerListChanged")
        {
            #region LIST PLAYER CHANGE
            if (action == "playerAdded")
            {
                EsObject player = eso.getEsObject(Fields.GAMEPLAY.PLAYER);
                PlayerControllerTLMN p = new PlayerControllerTLMN(player);

                if ((int)GameModelTLMN.YourController.PlayerState >= (int)PlayerControllerTLMN.EPlayerState.ready && GameModelTLMN.CurrentState >= GameModelTLMN.EGameState.deal)
                {
                    //Nếu là bạn đang chơi => Thì người mới sẽ được thêm vào danh sách khác.
                    GameModelTLMN.ListJoinGameWhenPlaying.Add(p);
                    GameModelTLMN.UpdatePlayerSide();
                    GameModelTLMN.game.Listener.RegisterEventPlayerListChanged(p, false);
                }
                else
                {
                    //Nếu là bạn không tham gia trận đấu thì sẽ làm như bình thường
                    GameModelTLMN.SetPlayer(p.slotServer, p);
                    GameModelTLMN.UpdatePlayerSide();
                    GameModelTLMN.game.Listener.RegisterEventPlayerListChanged(GameModelTLMN.GetPlayer(p.slotServer), false);
                }
            }
            else if (action == "playerRemoved")
            {
                EsObject esPlayer = eso.getEsObject(Fields.GAMEPLAY.PLAYER);

                PlayerControllerTLMN joinWhenPlaying = GameModelTLMN.ListJoinGameWhenPlaying.Find(pp => pp.username == esPlayer.getString(Fields.PLAYER.USERNAME));
                if (joinWhenPlaying != null)
                {
                    ///Thoát ra khi mà mới join vào trận đấu, nhưng chưa tham gia trận đấu
                    GameObject.Destroy(joinWhenPlaying.cuiPlayer.gameObject);
                    GameModelTLMN.ListJoinGameWhenPlaying.Remove(joinWhenPlaying);
                    GameModelTLMN.DrawInfoPlayerNoSlot();
                }
                else
                {
                    PlayerControllerTLMN p = GameModelTLMN.GetPlayer(esPlayer.getString(Fields.PLAYER.USERNAME));
                    ///Nếu người thoát ra đang chơi thì đánh dấu là đã quit nhưng vẫn lưu lại các thông tin trên bàn chơi.
                    if ((int)p.PlayerState >= (int)PlayerControllerTLMN.EPlayerState.ready && GameModelTLMN.CurrentState >= GameModelTLMN.EGameState.deal)
                    {
                        StartCoroutine(GameModelTLMN.model.PlayerLeftOut(p));
                        p.IsHasQuit = true;
                    }
                    else ///Ngược còn không tức là trường hợp bình thường, là người chơi out ra khi trận đấu chưa diễn ra.
                        GameModelTLMN.SetPlayer(p.slotServer, null);
                }
            }
            #endregion
        }
        else if (command == "updateUserInfo")
        {
            #region THÔNG TIN NGƯỜI CHƠI THAY ĐỔI (UPDATE SAU KHI KẾT THÚC GAME)
            updateMoneyAfterFinishGame = eso;
            #endregion
        }
        else if (command == "playerReady")
        {
            #region NGƯỜI CHƠI READRY
            int slotIndex = 0;
            SetPlayerEsObject(eso, ref slotIndex);

            if (eso.variableExists("okToDealCard"))
            {
                if (ClickStartGame == false && eso.variableExists("timeAutoDealCard"))
                    StartTimeAutoDeal(eso.getInteger("timeAutoDealCard") / 1000f);

                if (eso.variableExists("okToDealCard"))
                    ClickStartGame = eso.getBoolean("okToDealCard");
            }

            GameModelTLMN.game.UpdateUI();
            #endregion
        }
        else if (command == "updateHand")
        {
            #region BẮT ĐẦU GAME
            ///Tự ẩn nút bắt đầu trận đấu khi thời gian đếm ngược kết thúc
            WhenDealCard();
            GameModelTLMN.DealCardDone = false;

            EsObject[] players = eso.getEsObjectArray("players");

            if (eso.variableExists("gameState"))
            {
                string gameState = eso.getString("gameState");
                if (gameState == "playing")
                    GameModelTLMN.ActiveState(GameModelTLMN.ConvertGameState(gameState));
            }

            foreach (EsObject obj in players)
            {
                int slotIndex = obj.getInteger("slotIndex");
                if (slotIndex == GameModelTLMN.YourController.slotServer)
                    continue;

                GameModelTLMN.GetPlayer(slotIndex).SetDataPlayer(obj);
                int handSize = GameModelTLMN.GetPlayer(slotIndex).handSize;

                GameModelTLMN.GetPlayer(slotIndex).Reset();
                GameModelTLMN.GetPlayer(slotIndex).mCardHand.Clear();

                if (obj.variableExists("hand") && Common.CanViewHand)
                {
                    Array.ForEach<int>(obj.getIntegerArray("hand"), id =>
                    {
                        GameModelTLMN.GetPlayer(slotIndex).mCardHand.Add(new TLMNCard(slotIndex, id));
                    });

                    GameModelTLMN.GetPlayer(slotIndex).mCardHand.Sort((c1, c2) => c1.CompareTo(c2));
                }
                else
                {
                    while (handSize >= 1)
                    {
                        GameModelTLMN.GetPlayer(slotIndex).mCardHand.Add(new TLMNCard(slotIndex));
                        handSize--;
                    }
                }
                GameModelTLMN.GetPlayer(slotIndex).SetDataPlayer(obj);
            }

            GameModelTLMN.IndexInTurn = GameModelTLMN.YourController.slotServer;
            TimeCountDown = eso.getInteger("timeForAnimation") / 1000f;

            if (GameModelTLMN.IndexInTurn == GameModelTLMN.YourController.slotServer)
                if (eso.variableExists("fullLaying"))
                    fullLaying = eso.getBoolean("fullLaying");

            GameModelTLMN.YourController.mCardHand.Clear();
            int[] listCard = eso.getIntegerArray("hand");
            Array.ForEach<int>(listCard, cardValue => GameModelTLMN.YourController.mCardHand.Add(new TLMNCard(GameModelTLMN.YourController.slotServer, cardValue)));

            GameModelTLMN.ActiveState(GameModelTLMN.EGameState.deal);
            GameModelTLMN.game.button.UpdateButton(); //Check lại button (fixbug)
            GameModelTLMN.game.UpdateUI();
            #endregion
        }
        else if (command == "updateGameState")
        {
            #region UPDATE GAME STATE
            string gameState = eso.getString("gameState");
            string lastGameState = eso.getString("lastGameState");

            Debug.LogWarning(GameModelTLMN.ConvertGameState(lastGameState).ToString());

            ///Phòng trường hợp trước đó bước finishGame chưa desroy đc sẽ destroy lại
            if (GameModelTLMN.ConvertGameState(lastGameState) == GameModelTLMN.EGameState.finalizing)
            {
                //Fix lỗi khi click card lúc hết trận kiểm tra phần tử trong list
                clickCardList.Clear();
                GameModelTLMN.DestroyObject();
            }

            GameModelTLMN.ActiveState(GameModelTLMN.ConvertGameState(gameState));
            #endregion
        }
        else if (command == "turn")
        {
            #region ĐÁNH BÀI

            if (eso.variableExists(Fields.GAMEPLAY.PLAYER))
            {
                SetPlayerEsObject(eso, ref GameModelTLMN.IndexInTurn);
            }
            else //Có người chơi đánh hai (nếu có hàng hay dây sẽ được chặt)
            {
                //Đưa xuống dòng dưới - if (eso.variableExists("lastPlayer"))
            }

            if (eso.variableExists("lastPlayer"))
            {
                if (eso.variableExists("newTurn"))
                    newTurn = eso.getBoolean("newTurn");
                else
                    newTurn = false;
            }
            else
                newTurn = true;

            if (eso.variableExists("lastPlayer"))
            {
                SetPlayerEsObject(eso, ref GameModelTLMN.IndexLastInTurn, new string[] { "lastPlayer" });

                if (!eso.variableExists(Fields.GAMEPLAY.PLAYER))
                    if (GameModelTLMN.IndexLastInTurn != GameModelTLMN.YourController.slotServer)
                        GameModelTLMN.IndexInTurn = GameModelTLMN.YourController.slotServer;

                //Trường hợp người đánh và người lượt mới cùng là một người thì cập nhật lại thông tin
                if (GameModelTLMN.IndexLastInTurn == GameModelTLMN.IndexInTurn && eso.variableExists(Fields.GAMEPLAY.PLAYER))
                    SetPlayerEsObject(eso, ref GameModelTLMN.IndexInTurn);

                if (GameModelTLMN.IndexInTurn == GameModelTLMN.YourController.slotServer)
                    if (eso.variableExists("stolen"))
                        stolen = eso.getBoolean("stolen");

                AudioManager.Instance.Play(AudioManager.SoundEffect.Discard);

                //if (GameManager.GAME == EGame.TLMN)
                    GameModelTLMN.Discard(GameModelTLMN.IndexLastInTurn, eso.getIntegerArray(Fields.CARD.CARD_ID));
                //else
                //    GameModelTLMN.Discard(GameModelTLMN.IndexLastInTurn, new int[] { eso.getInteger(Fields.CARD.CARD_ID) });
            }
            else
            {
                //Sau khi chia bai
                GameModelTLMN.ActiveState(GameModelTLMN.EGameState.playing);

                //Trường hợp người chơi trước thoát ra khỏi bàn
                if (GameModelTLMN.GetPlayer(GameModelTLMN.IndexInTurn).mCardHand.Count == 9)
                    GameModelTLMN.MiniState = GameModelTLMN.EGameStateMini.stealCard_or_draw;
            }

            TimeCountDown = eso.getInteger("time") / 1000f;
            #endregion
        }
        else if (command == "playerDrawCard")
        {
            #region BỐC BÀI
            SetPlayerEsObject(eso, ref GameModelTLMN.IndexInTurn);
            int cardId = eso.getInteger(Fields.CARD.CARD_ID);

            if (GameModelTLMN.IndexInTurn == GameModelTLMN.YourController.slotServer)
                fullLaying = eso.getBoolean("fullLaying");

            GameModelTLMN.DrawCard(GameModelTLMN.IndexInTurn, cardId);
            TimeCountDown = eso.getInteger("time") / 1000f;

            if (GameModelTLMN.IndexInTurn == GameModelTLMN.YourController.slotServer
                && GameModelTLMN.ListPlayerInGameEnemy.TrueForAll(p => p.mCardTrash.Count >= 4))
            {
                //Trường hợp tái vòng không ăn cây
                DoRequestCheckGiveCardAndMeld();
            }
            #endregion
        }
        else if (command == "playerStealCard")
        {
            #region ĂN CÂY
            SetPlayerEsObject(eso, ref GameModelTLMN.IndexInTurn);
            SetPlayerEsObject(eso, ref GameModelTLMN.IndexLastInTurn, new string[] { "lastPlayer" });

            AudioManager.Instance.Play(AudioManager.SoundEffect.StealCard);
            GameModelTLMN.StealCard(GameModelTLMN.IndexInTurn, GameModelTLMN.IndexLastInTurn);

            if (eso.variableExists("cardToTransfer"))
            {
                EsObject cardToTransfer = eso.getEsObject("cardToTransfer");

                int indexCardInTrash = GameModelTLMN.GetPlayer(cardToTransfer.getString("sourcePlayer")).mCardTrash.FindIndex(o => o.CardId == cardToTransfer.getInteger(Fields.CARD.CARD_ID));
                ECard cardTransfer = GameModelTLMN.GetPlayer(cardToTransfer.getString("sourcePlayer")).mCardTrash[indexCardInTrash];

                GameModelTLMN.GetPlayer(cardToTransfer.getString("sourcePlayer")).mCardTrash.Remove(cardTransfer);
                GameModelTLMN.GetPlayer(cardToTransfer.getString("targetPlayer")).mCardTrash.Add(cardTransfer);
                UpdateHand();
            }

            if (GameModelTLMN.IndexInTurn == GameModelTLMN.YourController.slotServer)
            {
                fullLaying = eso.getBoolean("fullLaying");
                if (fullLaying)
                    GameModelTLMN.game.button.UpdateButton();
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
            GameModelTLMN.SortHand();
            canRequestSort = false;
            #endregion
        }
        else if (command == "lay")
        {
            #region EVENT HẠ BÀI
            SetPlayerEsObject(eso, ref GameModelTLMN.IndexInTurn);

            GameModelTLMN.StartLayMeld(GameModelTLMN.IndexInTurn);

            TimeCountDown = eso.getInteger("time") / 1000f;

            //Khi thông báo hạ phỏm sẽ gửi yêu cầu kiểm tra cây gửi. (Không đưa vào playerLayMeld vì có khi tái nhưng ko có phỏm)
            DoRequestCheckGiveCardAndMeld();
            #endregion
        }
        else if (command == "playerLayMeld")
        {
            #region EVENT CÓ NGƯỜI CHƠI HẠ BÀI

            AudioManager.Instance.Play(AudioManager.SoundEffect.LayingMeld);

            ProcessLayMelds(eso, eso, ref GameModelTLMN.IndexInTurn);

            DoRequestCheckGiveCardAndMeld();
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
                card.meld = GameModelTLMN.GetPlayer(card.slotIndex).GetMeld(card.meldResponse);
                listGiveCard.Add(card);
            }
            GameModelTLMN.game.button.UpdateButton();
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
            GameModelTLMN.game.button.UpdateButton();
            #endregion
        }
        else if (command == "playerAddMeld")
        {
            #region GỬI BÀI VÀO PHỎM
            SetPlayerEsObject(eso, ref GameModelTLMN.IndexInTurn);

            EsObject[] cardAddingToMelds = eso.getEsObjectArray("cardAddingToMelds");

            foreach (EsObject obj in cardAddingToMelds)
            {
                int slotId = obj.getInteger("slotId");
                int[] meld = obj.getIntegerArray("meld");
                int cardId = obj.getInteger(Fields.CARD.CARD_ID);
                GameModelTLMN.AddMeld(GameModelTLMN.IndexInTurn, slotId, cardId, meld);
            }

            DoRequestCheckGiveCardAndMeld();
            #endregion
        }
        else if (command == "updateRoomMaster")
        {
            #region Update Room Master
            EsObject player = eso.getEsObject(Fields.GAMEPLAY.PLAYER);
            PlayerControllerTLMN p = new PlayerControllerTLMN(player);
            GameModelTLMN.ListPlayer.ForEach(pC => RemoveRoomMasterIcon(pC));
            if (GameModelTLMN.GetPlayer(p.slotServer) != null)
            {
                GameModelTLMN.GetPlayer(p.slotServer).SetDataPlayer(player);
                GameModelTLMN.GetPlayer(p.slotServer).cuiPlayer.UpdateInfo();
            }

            ((LobbyTLMN)GameManager.Instance.selectedLobby).roomMasterUsername = p.username;
            ((LobbyTLMN)GameManager.Instance.selectedLobby).config.password = eso.getString("password");

            if (GameModelTLMN.CurrentState == GameModelTLMN.EGameState.waitingForPlayer || GameModelTLMN.CurrentState == GameModelTLMN.EGameState.waitingForReady)
                GameModelTLMN.game.ClickStartGame = GameModelTLMN.ListPlayerInGameEnemy.Count > 0;

            Listener.RegisterEventRoomMasterChanged(GameModelTLMN.GetPlayer(p.slotServer));
            #endregion
        }
        else if (command == "finishGame")
        {
            #region FINISH GAME
            StartCoroutine(ProcessFinishGame(eso));
            #endregion
        }
        else if (command == "updatePlayersSlot")
        {
            #region Switch Player
            EsObject[] players = eso.getEsObjectArray("players");

            GameModelTLMN.ListPlayer.ForEach(p => GameObject.Destroy(p.cuiPlayer.gameObject));

            PlayerControllerTLMN[] lst = new PlayerControllerTLMN[5];
            foreach (EsObject player in players)
                lst[player.getInteger("slotIndex")] = new PlayerControllerTLMN(player);
            GameModelTLMN.SetPlayer(lst);
            #endregion
        }
        else if (command == "changeConfigGame")
        {
            #region Change Config Game
            GameManager.Instance.selectedLobby.SetDataJoinLobby(eso);
            if (GameModelTLMN.YourController.isMaster)
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

            GameModelTLMN.game.button.UpdateButton();
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
                if (eso.variableExists("code") && DefinedError.IsDiscardErrorTLMN(eso.getInteger("code")))
                {
                    Debug.LogWarning("Error Code: " + eso.getInteger("code"));
                    lbDiscard.Text = "";
                    int inTurn = GameModelTLMN.IndexInTurn;
                    int lastInTurn = GameModelTLMN.IndexLastInTurn;

                    //Trước đó có đổi turn rồi nên phải đổi lại
                    GameModelTLMN.IndexInTurn = GameModelTLMN.IndexLastInTurn;
                    PlayerControllerTLMN p = GameModelTLMN.GetPlayer(GameModelTLMN.IndexInTurn);

                    List<ECard> lastDiscard = GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp][GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp].Count - 1];
                    foreach (ECard card in lastDiscard)
                    {
                        p.mCardTrash.Remove(card);
                        p.mCardHand.Add(card);
                        card.UpdateParent(GameModelTLMN.IndexInTurn);
                    }
                    GameModelTLMN.game.allowChatHang = null;
                    GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp].Remove(lastDiscard);
                    GameModelTLMN.MiniState = GameModelTLMN.EGameStateMini.discard;

                    if (eso.getInteger("code") == DefinedError.GetCodeError(DefinedError.GameplayError.DISCARD_ACTION_DENIED))
                    {
                        GameModelTLMN.IndexInTurn = inTurn;
                        GameModelTLMN.IndexLastInTurn = lastInTurn;
                    }
                    UpdateHand(GameModelTLMN.YourController.slotServer, 0f);
                    UpdateTrash(0.5f);
                }
            }

            GameModelTLMN.game.button.UpdateButton();
            #endregion
        }
        else if (command == "userKicked")
        {
            #region Kick Player
            if (eso.variableExists("id"))
            {
                int id = eso.getInteger("id");
                Debug.LogWarning(eso.getString("reason"));
                if (id == 0)
                    NotificationView.ShowMessage("Bạn đã bị chủ phòng đuổi khỏi bàn chơi.");
                else if (id == 1)
                    Common.MessageRecharge("Bạn đã không còn đủ tiền để tiếp tục chơi.");
                DoJoinRoom();
            }
            #endregion
        }
        else if (command == "updateGame")
        {
            #region UPDATE GAME
            GameManager.Instance.selectedLobby.SetDataJoinLobby(eso);

            EsObject[] players = eso.getEsObjectArray("players");

            int isAddPlayerSlot = -1;
            foreach (EsObject obj in players)
            {
                PlayerControllerTLMN p = new PlayerControllerTLMN(obj);
                if (GameModelTLMN.GetPlayer(p.slotServer) == null)
                {
                    GameModelTLMN.SetPlayer(p.slotServer, p);
                    isAddPlayerSlot = p.slotServer;
                }
                else
                    GameModelTLMN.GetPlayer(p.slotServer).SetDataPlayer(obj);
            }

            GameModelTLMN.CreateNewGame();

            GameModelTLMN.ActiveState(GameModelTLMN.ConvertGameState(eso.getString("gameState")));
            Debug.Log("Load data start-game: " + GameModelTLMN.CurrentState.ToString());

            #region Người chơi tham gia khi trận đấu đang diễn ra.
            if (GameModelTLMN.CurrentState == GameModelTLMN.EGameState.playing)
            {
                deck.SetActive(true);
                foreach (EsObject obj in players)
                {
                    int slotIndex = obj.getInteger("slotIndex");

                    int handSize = GameModelTLMN.GetPlayer(slotIndex).handSize;
                    GameModelTLMN.GetPlayer(slotIndex).mCardHand.Clear();
                    if (obj.variableExists("hand") == false)
                    {
                        while (handSize >= 1)
                        {
                            GameModelTLMN.DeckCount--;
                            TLMNCard card = new TLMNCard(slotIndex);
                            card.Instantiate();
                            GameModelTLMN.GetPlayer(slotIndex).mCardHand.Add(card);
                            handSize--;
                        }
                        GameModelTLMN.DeckCount = 0;
                    }
                    else
                    {
                        if (obj.variableExists("hand"))
                        {
                            Array.ForEach<int>(obj.getIntegerArray("hand"), cardId =>
                            {
                                GameModelTLMN.DeckCount--;
                                TLMNCard card = new TLMNCard(slotIndex, cardId);
                                card.Instantiate();
                                GameModelTLMN.GetPlayer(slotIndex).mCardHand.Add(card);
                                card.UpdateParent(slotIndex);
                            });
                        }

                        if (obj.variableExists("stolen"))
                        {
                            Array.ForEach<int>(obj.getIntegerArray("stolen"), cardId =>
                            {
                                GameModelTLMN.GetPlayer(slotIndex).mCardHand.Find(c => c.CardId == cardId).originSide = ESide.Enemy_3;
                            });
                        }
                    }

                    if (obj.variableExists("trash"))
                    {
                        int[] trash = obj.getIntegerArray("trash");
                        foreach (int cardValue in trash)
                        {
                            GameModelTLMN.DeckCount--;
                            TLMNCard card = new TLMNCard(slotIndex, cardValue);
                            card.Instantiate();
                            GameModelTLMN.GetPlayer(slotIndex).mCardTrash.Add(card);
                            card.UpdateParent(slotIndex);
                        }
                    }
                }
                UpdateHand();

                PlayerControllerTLMN playerInTurn = GameModelTLMN.ListPlayerInGame.Find(p => p.PlayerState == PlayerControllerTLMN.EPlayerState.inTurn || p.PlayerState == PlayerControllerTLMN.EPlayerState.laying);
                if (playerInTurn == null)
                {
                    //Đang ở bước chia bài
                }
                else
                {
                    GameModelTLMN.IndexInTurn = playerInTurn.slotServer;

                    if (eso.variableExists("lastCards"))
                    {
                        newTurn = false;
                        PlayerControllerTLMN lastPlayer = GameModelTLMN.GetLastPlayer(GameModelTLMN.IndexInTurn);
                        foreach (int cardId in eso.getIntegerArray("lastCards"))
                        {
                            TLMNCard card = new TLMNCard(lastPlayer.slotServer, cardId);
                            card.Instantiate();
                            lastPlayer.mCardHand.Add(card);
                            card.UpdateParent(lastPlayer.slotServer);
                        }
                        GameModelTLMN.Discard(lastPlayer.slotServer, eso.getIntegerArray("lastCards"));
                    }

                    if (GameModelTLMN.YourController.slotServer == GameModelTLMN.IndexInTurn)
                    {
                        if (GameModelTLMN.YourController.PlayerState == PlayerControllerTLMN.EPlayerState.laying)
                            GameModelTLMN.MiniState = GameModelTLMN.EGameStateMini.lay_meld;
                        else
                        {
                            if (GameManager.GAME == EGame.TLMN)
                            {
                                GameModelTLMN.MiniState = GameModelTLMN.EGameStateMini.discard;
                            }
                            else
                            {
                                if (GameModelTLMN.YourController.mCardHand.Count == 10)
                                    GameModelTLMN.MiniState = GameModelTLMN.EGameStateMini.discard;
                                else
                                    GameModelTLMN.MiniState = GameModelTLMN.EGameStateMini.stealCard_or_draw;
                            }
                        }
                    }
                }
                GameModelTLMN.DealCardDone = true;
            }
            #endregion

            if (eso.variableExists("totalTime") && eso.variableExists("remainingTime"))
                StartTimeRemaining(eso.getInteger("totalTime") / 1000f, eso.getInteger("remainingTime") / 1000f);
            else if (eso.variableExists("timeAutoDealCard"))
                StartTimeAutoDeal(eso.getInteger("timeAutoDealCard") / 1000f);

            if (isAddPlayerSlot > -1)
                Listener.RegisterEventPlayerListChanged(GameModelTLMN.GetPlayer(isAddPlayerSlot), false);

            if (GameModelTLMN.ListPlayer.Count > 1)
            {
                if (GameManager.OldScene == ESceneName.LobbyTLMN)
                    GameManager.Server.DoLeaveRoom(GameManager.Instance.selectedChannel.zoneId, GameManager.Instance.selectedChannel.roomId);
            }
            #endregion
        }

        if (command == "turn")
        {
            yield return new WaitForSeconds(0.5f);
            IsProcesResonseDone = true;
        }
        else if (command != "finishGame" && command != "updateHand")
        {
            yield return new WaitForSeconds(0.1f);
            IsProcesResonseDone = true;
        }
    }

    public void RemoveRoomMasterIcon(PlayerControllerTLMN p)
    {
        p.isMaster = false;
        p.cuiPlayer.UpdateInfo();
    }
    public void ClearEffectCardPair()
    {
        #region CLEAR EFECT IN "BA, BỐN ĐÔI THÔNG", "TỨ QUÝ"
        foreach (ECard c in listEffectCardPair)
            GameObject.Destroy(c.gameObject);
        listEffectCardPair.Clear();
        #endregion
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
            AN_CAY_THU_NHAT = 0,    //Ăn cây thứ nhất
            AN_CAY_THU_HAI = 1,     //Ăn cây thứ hai
            AN_CAY_THU_BA = 2,      //Ăn cây thứ ba không phải chốt
            AN_CAY_CHOT = 3,        //Ăn cây chốt
            AN_CAY_CHOT_DEN = 4,    //Ăn cây chốt đền

            TOP_2 = 5,              //Về nhì
            TOP_3 = 6,              //Về ba
            TOP_4 = 7,              //Về tư

            MOM = 8,                //Móm
            XAO_KHAN = 9,           //Xào khan

            U_XUONG = 10,            //Ù Xuông
            U_THUONG = 11,          //Ù thường
            U_TRON = 12,            //Ù tròn 10 cây
            DEN_LANG = 13,          //Đền làng
        }
        public EActionTLMN actionTLMN;
        public enum EActionTLMN
        {
            THANG_TRANG = 0, 		//Thắng trắng
            TREO = 1, 				//Treo bài
            HAI_BICH = 2, 			//Bị bắt hoặc thối mất hai bích
            HAI_TEP = 3, 			//Bị bắt hoặc thối mất hai tép
            HAI_RO = 4, 			//Bị bắt hoặc thối mất hai rô
            HAI_CO = 5, 			//Bị bắt hoặc thối mất hai cơ
            BA_DOI_THONG = 6, 		//Bị bắt hoặc thối mất ba đôi thông
            TU_QUY = 7, 			//Bị bắt hoặc thối mất tứ quý
            BON_DOI_THONG = 8, 		//Bị bắt hoặc thối mất bốn đôi thông
            BA_BICH = 9, 			//Thối hoặc đút ba bích
            TOP_2 = 10, 			//Về nhì
            TOP_3 = 11, 			//Về ba
            TOP_4 = 12, 			//Về tư
            THUA_DEM_LA = 13, 		//Thua đếm lá
            DEN_LANG = 14           //Đền làng
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

    public class CommandEsObject
    {
        public string command;
        public string action;
        public EsObject eso;
        public PluginMessageEvent e;

        public CommandEsObject(string command, string action, EsObject eso)
        {
            this.command = command;
            this.action = action;
            this.eso = eso;
        }

        public CommandEsObject(PluginMessageEvent e, string command, string action, EsObject eso)
        {
            this.command = command;
            this.action = action;
            this.eso = eso;
            this.e = e;
        }
    }
    public void SetTypeFinishNormalGameTLMN(string finishType, string gameType)
    {
        Debug.Log("Finish Normal Game: " + finishType + " - " + gameType);

        if (gameType == "xepHang")
            gameFinishTypeTLMN = EFinishTypeTLMN.NORMAL_XEPHANG;
        else
        {
            if (finishType == "ducBaBich")
                gameFinishTypeTLMN = EFinishTypeTLMN.NORMAL_DEMLA_DUC_BA_BICH;
            else
                gameFinishTypeTLMN = EFinishTypeTLMN.NORMAL_DEMLA;
        }
    }
    public void SetTypeFinishTTGameTLMN(string gameType, string tType)
    {
        Debug.Log("Finish game with: " + gameType + " type: " + tType);
        gameFinishTypeTLMN = EFinishTypeTLMN.THANG_TRANG;
        // Van dau 
        if (tType.Trim() == "quân cái tạo thành tứ quý - ván đầu")
            gameFinishThangTrangType = EFinishByThangTrangType.TU_QUY_BANG_CAI;
        else if (tType.Trim() == "ba đôi thông có cái - ván đầu")
            gameFinishThangTrangType = EFinishByThangTrangType.BA_DOI_THONG_CO_CAI;
        else if (tType.Trim() == "bốn đôi thông có cái - ván đầu")							// not testing
            gameFinishThangTrangType = EFinishByThangTrangType.BON_DOI_THONG_CO_CAI;

        // Van thu 2 tro di
        else if (tType.Trim() == "2 tứ quý")
            gameFinishThangTrangType = EFinishByThangTrangType.HAI_TU_QUY;
        else if (tType.Trim() == "tứ quý 2")
            gameFinishThangTrangType = EFinishByThangTrangType.TU_QUY_2;
        else if (tType.Trim() == "sảnh rồng")
            gameFinishThangTrangType = EFinishByThangTrangType.SANH_RONG;
        else if (tType.Trim() == "6 đôi")
            gameFinishThangTrangType = EFinishByThangTrangType.SAU_DOI_THONG;
        else if (tType.Trim() == "sảnh từ 3 đến A")
            gameFinishThangTrangType = EFinishByThangTrangType.SANH_3_DEN_A;
        else if (tType.Trim() == "5 đôi thông")
            gameFinishThangTrangType = EFinishByThangTrangType.NAM_DOI_THONG;
    }
}
