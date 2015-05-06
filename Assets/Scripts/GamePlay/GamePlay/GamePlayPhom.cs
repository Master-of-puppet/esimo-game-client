using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Electrotank.Electroserver5.Api;

public class GamePlayPhom : MonoBehaviour
{
    #region Unity Editor
    public PlaymatPhom mPlaymat;
    public Transform transformParentButton;
    public CUIHandle btReady, btStart,
        //btDraw, btDiscard, btSorted,
        btStealCard, btGuiBai, btHaBai,
        btFulllaying;

    public UIDisableButton btDraw, btDiscard, btSorted;
    public GameObject deck, imageTurnLayMeld;
    #endregion

    [HideInInspector]
    public CardControllerPhom cardController = new CardControllerPhom();

    /// <summary>
    /// Xử lý button gameplay
    /// </summary>
    [HideInInspector]
    public YourButtonControllerPhom button = new YourButtonControllerPhom();

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
    public PlayerControllerPhom winner;

    #region Time Count Down
    float _timeCountDownTurm = 0;
    public float TimeCountDown
    {
        get { return _timeCountDownTurm; }
        set
        {
            _timeCountDownTurm = value;
            GameModelPhom.GetPlayer(GameModelPhom.IndexInTurn).cuiPlayer.StartTime(_timeCountDownTurm);
        }
    }

    public void StartTimeRemaining(float totalTime, float remainingTime)
    {
        _timeCountDownTurm = totalTime;
        GameModelPhom.GetPlayer(GameModelPhom.IndexInTurn).cuiPlayer.StartRemainingTime(remainingTime);
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
                btStart.gameObject.SetActive(GameModelPhom.YourController.isMaster && GameModelPhom.CurrentState == GameModelPhom.EGameState.waitingForReady);
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
        GameModelPhom.model = new GameModelPhom(this);

        HeaderMenu.Instance.OnClickButtonBackCallBack = delegate()
        {
            if (GameObject.Find("__Prefab Quit Gameplay") == null)
                GPQuitPhom.Create();
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

        GameManager.Server.EventPluginMessageOnProcess += ProcessOnGameplay;
        GameManager.Server.EventLeaveRoom += OnLeaveRoom;
    }

    void OnDestroy()
    {
        GameModelPhom.game = null;

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
        if (GameManager.OldScene == ESceneName.LoginScreen || GameManager.Instance.mInfo.chip < ((ChannelPhom)GameManager.Instance.selectedChannel).minimumMoney)
            GameManager.Server.DoJoinRoom(GameManager.Instance.channelRoom.zoneId, GameManager.Instance.channelRoom.roomId);
        else
            GameManager.Server.DoJoinRoom(GameManager.Instance.selectedChannel.zoneId, GameManager.Instance.selectedChannel.roomId);
    }

    void OnLeaveRoom(LeaveRoomEvent e)
    {
        if (e.RoomId == GameManager.Instance.selectedLobby.roomId && e.ZoneId == GameManager.Instance.selectedLobby.zoneId)
        {
            if (GameManager.OldScene == ESceneName.LoginScreen || GameManager.Instance.mInfo.chip < ((ChannelPhom)GameManager.Instance.selectedChannel).minimumMoney)
                GameManager.LoadScene(ESceneName.ChannelPhom);
            else
                GameManager.LoadScene(ESceneName.LobbyPhom);
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
        GamePlayPhom.GiveCard giveCard = (GamePlayPhom.GiveCard)go.GetComponent<UIContainerAnonymous>().intermediary;
        DoRequestAddMeldCard(giveCard);
    }

    void DoRequestAddMeldCard(GiveCard giveCard)
    {
        GameManager.Server.DoRequestPluginGame(Utility.SetEsObject(Fields.GAMEPLAY.PLAY, new object[] {
            Fields.ACTION, "addMeld",
            "cardsToAdd", new EsObject[] { 
                Utility.SetEsObject(null, new object[] {
                    Fields.CARD.CARD_ID, giveCard.cardId,
                    Fields.GAMEPLAY.PLAYER, GameModelPhom.GetPlayer(giveCard.slotIndex).username,
                    "slotId", giveCard.slotIndex,
                    "cardInMeld", giveCard.meld.meld[0].CardId })}
         }));
    }

    void OnClickAddMeld(GameObject go)
    {
        go.SetActive(false);

        if (((LobbyPhom)GameManager.Instance.selectedLobby).config.isAdvanceGame)
            DoRequestAddMeldCard(listGiveCard.Find(gc => gc.cardId == ListClickCard[0].CardId));
        else
        {
            List<EsObject> list = new List<EsObject>();
            GameModelPhom.YourController.mCardHand.ForEach(c => { EsObject obj = new EsObject(); obj.setInteger(Fields.CARD.CARD_ID, c.CardId); list.Add(obj); });
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
            Fields.GAMEPLAY.MELDS, ListClickCard.Count > 2 ? ListClickCard.Select(c => c.CardId).ToArray() : GameModelPhom.YourController.mCardHand.Select(c => c.CardId).ToArray()
        }));
    }

    public void OnClickSort(GameObject go)
    {
        btSorted.StopImpact(0.7f);

        if (ListClickCard.Count > 0)
        {
            HideClickCard();
            UpdateButtonWithCard();
        }

        if (go.GetComponent<UIContainerAnonymous>() == null)
            AudioManager.Instance.Play(AudioManager.SoundEffect.OrderCard);
        else
            GameObject.Destroy(go.GetComponent<UIContainerAnonymous>());

        if (canRequestSort)
        {
            sortList = cardController.SortCard();
            GameModelPhom.SortHand();
            canRequestSort = false;
        }
        else
            GameModelPhom.SortHand();
    }

    void OnClickDraw(GameObject go)
    {
        btDraw.SetStatus(true, false);

        GameManager.Server.DoRequestGameAction("drawCard");
    }

    #region ON CLICK DISCARD
    void OnClickDiscard(GameObject go)
    {
        ////btDiscard.SetStatus(true, false);
        //GameModelPhom.game.button.UpdateButton();
        if (ListClickCard.Count == 1 && GameModelPhom.CanDiscard(ListClickCard[0]))
            OnDiscard(ListClickCard[0].CardId);
    }
    public void OnDiscard(int cardId)
    {
        ////btDiscard.SetStatus(true, false);
        //GameModelPhom.game.button.UpdateButton();

        //Thực hiện đánh bài ngay, mà không cần chờ phản hồi phía server
        GameModelPhom.Discard(GameModelPhom.YourController.slotServer, cardId);

        GameManager.Server.DoRequestPluginGame(Utility.SetEsObject(Fields.GAMEPLAY.PLAY, new object[] { 
            Fields.ACTION, "discard", 
            Fields.CARD.CARD_ID, cardId 
        }));
    }
    #endregion

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
        if (GameModelPhom.IndexInTurn == GameModelPhom.YourController.slotServer)
        {
            //GameManager.Server.DoRequestGameAction("checkGiveCard");
            listGiveCard = cardController.ListGiveCard();
            //GameManager.Server.DoRequestGameAction("checkMeld");
            meldList = cardController.ListMeld();
            GameModelPhom.game.button.UpdateButton();
        }
    }
    #endregion

    /// <summary>
    /// Cập nhật lại UI của các Players
    /// </summary>
    public void UpdateUI()
    {
        if (GameManager.CurrentScene == ESceneName.GameplayPhom)
            GameModelPhom.ListPlayerInGame.ForEach(p => p.cuiPlayer.UpdateInfo());
    }

    void Update()
    {
        if ((int)GameModelPhom.CurrentState >= (int)GameModelPhom.EGameState.deal)
            if (deck.transform.FindChild("Label").GetComponent<UILabel>().text != GameModelPhom.DeckCount.ToString())
                deck.transform.FindChild("Label").GetComponent<UILabel>().text = GameModelPhom.DeckCount.ToString();
    }

    #region UpdateHand
    public void UpdateHand()
    {
        foreach (PlayerControllerPhom p in GameModelPhom.ListPlayerInGame)
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
        if (slotIndex == GameModelPhom.YourController.slotServer)
            HideClickCard();

        PlayerControllerPhom player = GameModelPhom.GetPlayer(slotIndex);

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

                    player.mCardHand[i].cardTexture.texture.depth = i;
                    if (player.mCardHand[i].gameObject.GetComponentInChildren<UISpriteAnimationCard>() != null)
                        player.mCardHand[i].gameObject.GetComponentInChildren<UISpriteAnimationCard>().GetComponent<UISprite>().depth = player.mCardHand[i].cardTexture.texture.depth;

                    if (player.mSide == ESide.Enemy_1 || player.mSide == ESide.Enemy_3)
                        position.y += ECardTexture.CARD_HEIGHT / 3;

                    hash.Add("position", position);
                    iTween.RotateTo(obj, Vector3.zero, time);
                }
                else
                {
                    player.mCardHand[i].cardTexture.texture.depth = i + 1;
                    if (player.mCardHand[i].gameObject.GetComponentInChildren<UISpriteAnimationCard>() != null)
                        player.mCardHand[i].gameObject.GetComponentInChildren<UISpriteAnimationCard>().GetComponent<UISprite>().depth = player.mCardHand[i].cardTexture.texture.depth;

                    hash.Add("position", mPlaymat.GetLocationHand(player, i - numberStolen));

                    if (GameModelPhom.CurrentState == GameModelPhom.EGameState.finalizing)
                        iTween.RotateTo(obj, Vector3.zero, time);
                    else
                        iTween.RotateTo(obj, mPlaymat.GetRotateHand(player), time);
                }

                //player.mCardHand[i].cardTexture.texture.depth = i;
                //if (player.mCardHand[i].gameObject.GetComponentInChildren<UISpriteAnimationCard>() != null)
                //    player.mCardHand[i].gameObject.GetComponentInChildren<UISpriteAnimationCard>().GetComponent<UISprite>().depth = player.mCardHand[i].cardTexture.texture.depth;

                iTween.MoveTo(obj, hash);
                if (player.mSide == ESide.You)
                    iTween.ScaleTo(obj, Vector3.one * 2, time);
                else
                    iTween.ScaleTo(obj, Vector3.one, time);
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
                if (obj.GetComponentInChildren<UISpriteAnimationCard>() != null)
                    obj.GetComponentInChildren<UISpriteAnimationCard>().GetComponent<UISprite>().depth = obj.GetComponent<ECardTexture>().texture.depth;

                //iTween.Stop(obj);
                System.Collections.Hashtable hash = new System.Collections.Hashtable();
                hash.Add("islocal", true);
                hash.Add("time", time);
                hash.Add("position", mPlaymat.GetLocationTrash(player, i));
                iTween.MoveTo(obj, hash);
                iTween.RotateTo(obj, Vector3.zero, time);
                iTween.ScaleTo(obj, Vector3.one, time);
            }
        }
        #endregion

        #region IN MELDS

        for (int indexMeld = 0; indexMeld < player.mCardMelds.Count; indexMeld++)
        {
            //player.mCardMelds[indexMeld].meld.Sort(delegate(Card c1, Card c2) { return c1.CardId == c2.CardId ? 0 : c1.CardId < c2.CardId ? -1 : 1; });
            player.mCardMelds[indexMeld].meld.Sort((c1, c2) => c1.CompareTo(c2));

            for (int i = 0; i < player.mCardMelds[indexMeld].meld.Count; i++)
            {
                if (player.mCardMelds[indexMeld].meld[i].gameObject != null)
                {
                    GameObject obj = player.mCardMelds[indexMeld].meld[i].gameObject;
                    obj.GetComponent<ECardTexture>().texture.depth = i - 3;
                    if (obj.GetComponentInChildren<UISpriteAnimationCard>() != null)
                        obj.GetComponentInChildren<UISpriteAnimationCard>().GetComponent<UISprite>().depth = obj.GetComponent<ECardTexture>().texture.depth;

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
        if (GameModelPhom.IsYourTurn)
        {
            GameModelPhom.game.button.UpdateButtonDiscard();
            if (GameModelPhom.MiniState == GameModelPhom.EGameStateMini.lay_meld && ((LobbyPhom)GameManager.Instance.selectedLobby).config.isAdvanceGame)
                GameModelPhom.game.button.UpdateButtonLayMeld();
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

        GameModelPhom.game.button.UpdateButtonDiscard();
        GameModelPhom.game.button.UpdateButtonLayMeld();
    }

    void ShowCard(ECard card)
    {
        //List<int> meldSelected = null;
        //if (clickCardList.Count == 0)
        //    meldSelected = meldList.Find(meld => meld.Contains(card.CardId));
        //else
        //    meldSelected = meldList.Find(meld => meld.Contains(card.CardId) && meld.Contains(clickCardList[0].CardId));

        //if (meldSelected == null) 
        //    HideCardAll();
        //else if (clickCardList.Count > 0 && !meldSelected.Contains(clickCardList[0].CardId)) 
        //    HideCardAll();

        clickCardList.Add(card);
        listVectorPosition.Add(card.gameObject.transform.localPosition);

        card.gameObject.transform.localPosition = new Vector3(card.gameObject.transform.localPosition.x, card.gameObject.transform.localPosition.y + INCREASE_VECTOR_Y, card.gameObject.transform.localPosition.z);

        AudioManager.Instance.Play(AudioManager.SoundEffect.SelectCard);
    }

    void HideCard(ECard card)
    {
        if (clickCardList.Contains(card) == false) return;

        card.gameObject.transform.localPosition = listVectorPosition[clickCardList.IndexOf(card)];
        listVectorPosition.RemoveAt(clickCardList.IndexOf(card));
        clickCardList.Remove(card);
    }

    void HideCardAll()
    {
        while (clickCardList.Count > 0)
            HideCard(clickCardList[0]);
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
                return new List<ECard>(new ECard[1] { clickCard.GetComponent<PhomCardTexture>().card });
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
            GameModelPhom.GetPlayer(index).SetDataPlayer(player);
        }
        else
        {
            PlayerControllerPhom p = GameModelPhom.GetPlayer(player.getString(Fields.PLAYER.USERNAME));
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
                Meld meld = new Meld(obj.getIntegerArray("meld"), GameModelPhom.GetPlayer(index));
                GameModelPhom.GetPlayer(index).mCardMelds.Add(meld);
            }
            GameModelPhom.LayMeld(index);
        }
    }

    /// <summary>
    /// Xử lý finishgame từ server
    /// </summary>
    private IEnumerator ProcessFinishGame(EsObject eso)
    {
        while (GameModelPhom.ListPlayerInGame.Contains(GameModelPhom.YourController) && GameModelPhom.DealCardDone == false)
            yield return new WaitForFixedUpdate();

        #region Không cho phép người chơi click card hay Drag&Drop card khi xẩy ra hiệu ứng lúc finish Game
        UpdateHand();
        foreach (ECard c in GameModelPhom.YourController.mCardHand)
            c.cardTexture.SetCollider(false);
        #endregion

        if (eso.variableExists("lastPlayer"))
            SetPlayerEsObject(eso, ref GameModelPhom.IndexLastInTurn, new string[] { "lastPlayer" });

        SetTypeFinishGame(eso.getString("type"));

        if (gameFinishType != EFinishType.NORMAL && GameModelPhom.GetPlayer(GameModelPhom.IndexLastInTurn).mCardMelds.Count == 0)
        {
            ProcessLayMelds(eso, eso.getEsObject("lastPlayer"), ref GameModelPhom.IndexLastInTurn, new string[] { "lastPlayer" });
            yield return new WaitForSeconds(0.5f);
        }

        if (eso.variableExists(Fields.CARD.CARD_ID))
            GameModelPhom.Discard(GameModelPhom.IndexLastInTurn, eso.getInteger(Fields.CARD.CARD_ID));

        EsObject result = eso.getEsObject("result");
        EsObject[] lstSummary = result.getEsObjectArray("summary");
        foreach (EsObject obj in lstSummary)
            GameModelPhom.GetPlayer(obj.getString(Fields.PLAYER.USERNAME)).SetDataSummary(obj);

        summaryGame.Clear();
        EsObject[] lstAction = result.getEsObjectArray("actions");

        //GPChatView.MessageFromSystem("[FF9900]***** THỐNG KÊ. KẾT QUẢ TRẬN ĐẤU *****[-]\n");
        Listener.RegisterEventLogGame("[FF9900]***** THỐNG KÊ. KẾT QUẢ TRẬN ĐẤU *****[-]\n");
        foreach (EsObject obj in lstAction)
        {
            Summary sum = new Summary();
            sum.sourcePlayer = obj.getString("sourcePlayer");
            sum.targetPlayer = obj.getString("targetPlayer");
            sum.description = obj.getString("description");
            long.TryParse(obj.getString("moneyExchange"), out sum.moneyExchange);
            sum.action = (Summary.EAction)obj.getInteger(Fields.ACTION);

            summaryGame.Add(sum);
            //GPChatView.MessageFromSystem("[FF9900]" + sum.targetPlayer + "[-] đã nhận được [FF9900]" + sum.moneyExchange + "[-] từ người chơi [FF9900]" + sum.sourcePlayer + "[-] vì [FF9900]" + sum.description + "[-]\n");
            Listener.RegisterEventLogGame("[FF9900]" + sum.targetPlayer + "[-] đã nhận được [FF9900]" + sum.moneyExchange + "[-] từ người chơi [FF9900]" + sum.sourcePlayer + "[-] vì [FF9900]" + sum.description + "[-]\n");
        }

        _timeCountDownTurm = eso.getInteger("time") / 1000f; //Lưu time lại nhưng ko chạy đồng hồ
        GameModelPhom.ActiveState(GameModelPhom.EGameState.finalizing);
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
                GameModelPhom.GetPlayer(updateMoneyAfterFinishGame.getString(Fields.PLAYER.USERNAME)).UpdateMoney(updateMoneyAfterFinishGame.getString("value"));
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
                PlayerControllerPhom p = new PlayerControllerPhom(player);

                if ((int)GameModelPhom.YourController.PlayerState >= (int)PlayerControllerPhom.EPlayerState.ready && GameModelPhom.CurrentState >= GameModelPhom.EGameState.deal)
                {
                    //Nếu là bạn đang chơi => Thì người mới sẽ được thêm vào danh sách khác.
                    GameModelPhom.ListJoinGameWhenPlaying.Add(p);
                    GameModelPhom.UpdatePlayerSide();
                    GameModelPhom.game.Listener.RegisterEventPlayerListChanged(p, false);
                }
                else
                {
                    //Nếu là bạn không tham gia trận đấu thì sẽ làm như bình thường
                    GameModelPhom.SetPlayer(p.slotServer, p);
                    GameModelPhom.UpdatePlayerSide();
                    GameModelPhom.game.Listener.RegisterEventPlayerListChanged(GameModelPhom.GetPlayer(p.slotServer), false);
                }
            }
            else if (action == "playerRemoved")
            {
                EsObject esPlayer = eso.getEsObject(Fields.GAMEPLAY.PLAYER);

                PlayerControllerPhom joinWhenPlaying = GameModelPhom.ListJoinGameWhenPlaying.Find(pp => pp.username == esPlayer.getString(Fields.PLAYER.USERNAME));
                if (joinWhenPlaying != null)
                {
                    ///Thoát ra khi mà mới join vào trận đấu, nhưng chưa tham gia trận đấu
                    GameObject.Destroy(joinWhenPlaying.cuiPlayer.gameObject);
                    GameModelPhom.ListJoinGameWhenPlaying.Remove(joinWhenPlaying);
                    GameModelPhom.DrawInfoPlayerNoSlot();
                }
                else
                {
                    PlayerControllerPhom p = GameModelPhom.GetPlayer(esPlayer.getString(Fields.PLAYER.USERNAME));
                    ///Nếu người thoát ra đang chơi thì đánh dấu là đã quit nhưng vẫn lưu lại các thông tin trên bàn chơi.
                    if ((int)p.PlayerState >= (int)PlayerControllerPhom.EPlayerState.ready && GameModelPhom.CurrentState >= GameModelPhom.EGameState.deal)
                    {
                        StartCoroutine(GameModelPhom.model.PlayerLeftOut(p));
                        p.IsHasQuit = true;
                    }
                    else ///Ngược còn không tức là trường hợp bình thường, là người chơi out ra khi trận đấu chưa diễn ra.
                        GameModelPhom.SetPlayer(p.slotServer, null);
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

            GameModelPhom.game.UpdateUI();
            #endregion
        }
        else if (command == "updateHand")
        {
            #region BẮT ĐẦU GAME
            ///Tự ẩn nút bắt đầu trận đấu khi thời gian đếm ngược kết thúc
            WhenDealCard();
            GameModelPhom.DealCardDone = false;

            EsObject[] players = eso.getEsObjectArray("players");

            if (eso.variableExists("gameState"))
            {
                string gameState = eso.getString("gameState");
                if (gameState == "playing")
                    GameModelPhom.ActiveState(GameModelPhom.ConvertGameState(gameState));
            }

            foreach (EsObject obj in players)
            {
                int slotIndex = obj.getInteger("slotIndex");
                if (slotIndex == GameModelPhom.YourController.slotServer)
                    continue;

                GameModelPhom.GetPlayer(slotIndex).SetDataPlayer(obj);
                int handSize = GameModelPhom.GetPlayer(slotIndex).handSize;

                GameModelPhom.GetPlayer(slotIndex).Reset();
                GameModelPhom.GetPlayer(slotIndex).mCardHand.Clear();

                if (obj.variableExists("hand") && Common.CanViewHand)
                {
                    Array.ForEach<int>(obj.getIntegerArray("hand"), id =>
                    {
                        GameModelPhom.GetPlayer(slotIndex).mCardHand.Add(new PhomCard(slotIndex, id));
                    });

                    GameModelPhom.GetPlayer(slotIndex).mCardHand.Sort((c1, c2) => c1.CompareTo(c2));
                }
                else
                {
                    while (handSize >= 1)
                    {
                        GameModelPhom.GetPlayer(slotIndex).mCardHand.Add(new PhomCard(slotIndex));
                        handSize--;
                    }
                }
                GameModelPhom.GetPlayer(slotIndex).SetDataPlayer(obj);
            }

            GameModelPhom.IndexInTurn = GameModelPhom.YourController.slotServer;
            TimeCountDown = eso.getInteger("timeForAnimation") / 1000f;

            if (GameModelPhom.IndexInTurn == GameModelPhom.YourController.slotServer)
                if (eso.variableExists("fullLaying"))
                    fullLaying = eso.getBoolean("fullLaying");

            GameModelPhom.YourController.mCardHand.Clear();
            int[] listCard = eso.getIntegerArray("hand");
            Array.ForEach<int>(listCard, cardValue => GameModelPhom.YourController.mCardHand.Add(new PhomCard(GameModelPhom.YourController.slotServer, cardValue)));

            GameModelPhom.ActiveState(GameModelPhom.EGameState.deal);
            GameModelPhom.game.button.UpdateButton(); //Check lại button (fixbug)
            GameModelPhom.game.UpdateUI();
            #endregion
        }
        else if (command == "updateGameState")
        {
            #region UPDATE GAME STATE
            string gameState = eso.getString("gameState");
            string lastGameState = eso.getString("lastGameState");

            Debug.LogWarning(GameModelPhom.ConvertGameState(lastGameState).ToString());

            ///Phòng trường hợp trước đó bước finishGame chưa desroy đc sẽ destroy lại
            if (GameModelPhom.ConvertGameState(lastGameState) == GameModelPhom.EGameState.finalizing)
            {
                //Fix lỗi khi click card lúc hết trận kiểm tra phần tử trong list
                clickCardList.Clear();
                GameModelPhom.DestroyObject();
            }

            GameModelPhom.ActiveState(GameModelPhom.ConvertGameState(gameState));
            #endregion
        }
        else if (command == "turn")
        {
            #region ĐÁNH BÀI
            SetPlayerEsObject(eso, ref GameModelPhom.IndexInTurn);

            if (eso.variableExists("lastPlayer"))
            {
                SetPlayerEsObject(eso, ref GameModelPhom.IndexLastInTurn, new string[] { "lastPlayer" });

                if (GameModelPhom.IndexInTurn == GameModelPhom.YourController.slotServer)
                    if (eso.variableExists("stolen"))
                        stolen = eso.getBoolean("stolen");

                AudioManager.Instance.Play(AudioManager.SoundEffect.Discard);
                GameModelPhom.Discard(GameModelPhom.IndexLastInTurn, eso.getInteger(Fields.CARD.CARD_ID));
            }
            else
            {
                //Sau khi chia bai
                GameModelPhom.ActiveState(GameModelPhom.EGameState.playing);

                //Trường hợp người chơi trước thoát ra khỏi bàn
                if (GameModelPhom.GetPlayer(GameModelPhom.IndexInTurn).mCardHand.Count == 9)
                    GameModelPhom.MiniState = GameModelPhom.EGameStateMini.stealCard_or_draw;
            }

            TimeCountDown = eso.getInteger("time") / 1000f;
            #endregion
        }
        else if (command == "playerDrawCard")
        {
            #region BỐC BÀI
            SetPlayerEsObject(eso, ref GameModelPhom.IndexInTurn);
            int cardId = eso.getInteger(Fields.CARD.CARD_ID);

            if (GameModelPhom.IndexInTurn == GameModelPhom.YourController.slotServer)
                fullLaying = eso.getBoolean("fullLaying");

            GameModelPhom.DrawCard(GameModelPhom.IndexInTurn, cardId);
            TimeCountDown = eso.getInteger("time") / 1000f;

            if (GameModelPhom.IndexInTurn == GameModelPhom.YourController.slotServer
                && GameModelPhom.ListPlayerInGameEnemy.TrueForAll(p => p.mCardTrash.Count >= 4))
            {
                //Trường hợp tái vòng không ăn cây
                DoRequestCheckGiveCardAndMeld();
            }
            #endregion
        }
        else if (command == "playerStealCard")
        {
            #region ĂN CÂY
            SetPlayerEsObject(eso, ref GameModelPhom.IndexInTurn);
            SetPlayerEsObject(eso, ref GameModelPhom.IndexLastInTurn, new string[] { "lastPlayer" });

            //AudioManager.Instance.Play(AudioManager.SoundEffect.StealCard);
            GameModelPhom.StealCard(GameModelPhom.IndexInTurn, GameModelPhom.IndexLastInTurn);

            if (eso.variableExists("moneyExchange"))
                MoneyExchangeView.Create(GameModelPhom.IndexLastInTurn, GameModelPhom.IndexInTurn, eso.getInteger("moneyExchange"));

            if (eso.variableExists("cardToTransfer"))
            {
                EsObject cardToTransfer = eso.getEsObject("cardToTransfer");

                int indexCardInTrash = GameModelPhom.GetPlayer(cardToTransfer.getString("sourcePlayer")).mCardTrash.FindIndex(o => o.CardId == cardToTransfer.getInteger(Fields.CARD.CARD_ID));
                ECard cardTransfer = GameModelPhom.GetPlayer(cardToTransfer.getString("sourcePlayer")).mCardTrash[indexCardInTrash];

                GameModelPhom.GetPlayer(cardToTransfer.getString("sourcePlayer")).mCardTrash.Remove(cardTransfer);
                GameModelPhom.GetPlayer(cardToTransfer.getString("targetPlayer")).mCardTrash.Add(cardTransfer);
                UpdateHand();
            }

            if (GameModelPhom.IndexInTurn == GameModelPhom.YourController.slotServer)
            {
                fullLaying = eso.getBoolean("fullLaying");
                if (fullLaying)
                    GameModelPhom.game.button.UpdateButton();
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
            GameModelPhom.SortHand();
            canRequestSort = false;
            #endregion
        }
        else if (command == "lay")
        {
            #region EVENT HẠ BÀI
            SetPlayerEsObject(eso, ref GameModelPhom.IndexInTurn);

            GameModelPhom.StartLayMeld(GameModelPhom.IndexInTurn);

            TimeCountDown = eso.getInteger("time") / 1000f;

            //Khi thông báo hạ phỏm sẽ gửi yêu cầu kiểm tra cây gửi. (Không đưa vào playerLayMeld vì có khi tái nhưng ko có phỏm)
            DoRequestCheckGiveCardAndMeld();
            #endregion
        }
        else if (command == "playerLayMeld")
        {
            #region EVENT CÓ NGƯỜI CHƠI HẠ BÀI

            AudioManager.Instance.Play(AudioManager.SoundEffect.LayingMeld);

            ProcessLayMelds(eso, eso, ref GameModelPhom.IndexInTurn);

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
                card.meld = GameModelPhom.GetPlayer(card.slotIndex).GetMeld(card.meldResponse);
                listGiveCard.Add(card);
            }
            GameModelPhom.game.button.UpdateButton();
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
            GameModelPhom.game.button.UpdateButton();
            #endregion
        }
        else if (command == "playerAddMeld")
        {
            #region GỬI BÀI VÀO PHỎM
            SetPlayerEsObject(eso, ref GameModelPhom.IndexInTurn);

            EsObject[] cardAddingToMelds = eso.getEsObjectArray("cardAddingToMelds");

            foreach (EsObject obj in cardAddingToMelds)
            {
                int slotId = obj.getInteger("slotId");
                int[] meld = obj.getIntegerArray("meld");
                int cardId = obj.getInteger(Fields.CARD.CARD_ID);
                GameModelPhom.AddMeld(GameModelPhom.IndexInTurn, slotId, cardId, meld);
            }

            DoRequestCheckGiveCardAndMeld();
            #endregion
        }
        else if (command == "updateRoomMaster")
        {
            #region Update Room Master
            EsObject player = eso.getEsObject(Fields.GAMEPLAY.PLAYER);
            PlayerControllerPhom p = new PlayerControllerPhom(player);
            GameModelPhom.ListPlayer.ForEach(pC => RemoveRoomMasterIcon(pC));
            if (GameModelPhom.GetPlayer(p.slotServer) != null)
            {
                GameModelPhom.GetPlayer(p.slotServer).SetDataPlayer(player);
                GameModelPhom.GetPlayer(p.slotServer).cuiPlayer.UpdateInfo();
            }

            ((LobbyPhom)GameManager.Instance.selectedLobby).roomMasterUsername = p.username;
            ((LobbyPhom)GameManager.Instance.selectedLobby).config.password = eso.getString("password");

            if (GameModelPhom.CurrentState == GameModelPhom.EGameState.waitingForPlayer || GameModelPhom.CurrentState == GameModelPhom.EGameState.waitingForReady)
                GameModelPhom.game.ClickStartGame = GameModelPhom.ListPlayerInGameEnemy.Count > 0;

            Listener.RegisterEventRoomMasterChanged(GameModelPhom.GetPlayer(p.slotServer));
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

            GameModelPhom.ListPlayer.ForEach(p => GameObject.Destroy(p.cuiPlayer.gameObject));

            PlayerControllerPhom[] lst = new PlayerControllerPhom[5];
            foreach (EsObject player in players)
                lst[player.getInteger("slotIndex")] = new PlayerControllerPhom(player);
            GameModelPhom.SetPlayer(lst);
            #endregion
        }
        else if (command == "changeConfigGame")
        {
            #region Change Config Game
            GameManager.Instance.selectedLobby.SetDataJoinLobby(eso);

            if (GameModelPhom.YourController.isMaster)
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

            GameModelPhom.game.button.UpdateButton();
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

                    int inTurn = GameModelPhom.IndexInTurn;
                    int lastInTurn = GameModelPhom.IndexLastInTurn;

                    //Trước đó có đổi turn rồi nên phải đổi lại
                    GameModelPhom.IndexInTurn = GameModelPhom.IndexLastInTurn;
                    PlayerControllerPhom p = GameModelPhom.GetPlayer(GameModelPhom.IndexInTurn);
                    ECard cardInTrash = p.mCardTrash[p.mCardTrash.Count - 1];
                    p.mCardTrash.Remove(cardInTrash);
                    p.mCardHand.Add(cardInTrash);
                    cardInTrash.UpdateParent(GameModelPhom.IndexInTurn);

                    GameModelPhom.MiniState = GameModelPhom.EGameStateMini.discard;

                    if (eso.getInteger("code") == DefinedError.GetCodeError(DefinedError.GameplayError.DISCARD_ACTION_DENIED))
                    {
                        UpdateHand(GameModelPhom.YourController.slotServer, 0f);
                        GameModelPhom.IndexInTurn = inTurn;
                        GameModelPhom.IndexLastInTurn = lastInTurn;
                    }
                    else
                        GameModelPhom.SortHand();
                }
            }

            GameModelPhom.game.button.UpdateButton();
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
                PlayerControllerPhom p = new PlayerControllerPhom(obj);
                if (GameModelPhom.GetPlayer(p.slotServer) == null)
                {
                    GameModelPhom.SetPlayer(p.slotServer, p);
                    isAddPlayerSlot = p.slotServer;
                }
                else
                    GameModelPhom.GetPlayer(p.slotServer).SetDataPlayer(obj);
            }
            GameModelPhom.CreateNewGame();
            GameModelPhom.ActiveState(GameModelPhom.ConvertGameState(eso.getString("gameState")));
            Debug.Log("Load data start-game: " + GameModelPhom.CurrentState.ToString());

            #region Người chơi tham gia khi trận đấu đang diễn ra.
            if (GameModelPhom.CurrentState == GameModelPhom.EGameState.playing)
            {
                deck.SetActive(true);
                foreach (EsObject obj in players)
                {
                    int slotIndex = obj.getInteger("slotIndex");

                    int handSize = GameModelPhom.GetPlayer(slotIndex).handSize;
                    GameModelPhom.GetPlayer(slotIndex).mCardHand.Clear();
                    if (obj.variableExists("hand") == false)
                    {
                        while (handSize >= 1)
                        {
                            GameModelPhom.DeckCount--;
                            PhomCard card = new PhomCard(slotIndex);
                            card.Instantiate();
                            GameModelPhom.GetPlayer(slotIndex).mCardHand.Add(card);
                            handSize--;
                        }
                    }
                    else
                    {
                        if (obj.variableExists("hand"))
                        {
                            Array.ForEach<int>(obj.getIntegerArray("hand"), cardId =>
                            {
                                GameModelPhom.DeckCount--;
                                PhomCard card = new PhomCard(slotIndex, cardId);
                                card.Instantiate();
                                GameModelPhom.GetPlayer(slotIndex).mCardHand.Add(card);
                                card.UpdateParent(slotIndex);
                            });
                        }

                        if (obj.variableExists("stolen"))
                        {
                            Array.ForEach<int>(obj.getIntegerArray("stolen"), cardId =>
                            {
                                GameModelPhom.GetPlayer(slotIndex).mCardHand.Find(c => c.CardId == cardId).originSide = ESide.Enemy_3;
                            });
                        }
                    }

                    if (obj.variableExists("trash"))
                    {
                        int[] trash = obj.getIntegerArray("trash");
                        foreach (int cardValue in trash)
                        {
                            GameModelPhom.DeckCount--;
                            PhomCard card = new PhomCard(slotIndex, cardValue);
                            card.Instantiate();
                            GameModelPhom.GetPlayer(slotIndex).mCardTrash.Add(card);
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
                                GameModelPhom.DeckCount--;
                                PhomCard card = new PhomCard(slotIndex, cardValue);
                                card.Instantiate();
                                GameModelPhom.GetPlayer(slotIndex).mCardHand.Add(card);
                                card.UpdateParent(slotIndex);
                            }
                            GameModelPhom.GetPlayer(slotIndex).mCardMelds.Add(new Meld(meld, GameModelPhom.GetPlayer(slotIndex)));
                        }
                    }
                }
                UpdateHand();

                PlayerControllerPhom playerInTurn = GameModelPhom.ListPlayerInGame.Find(p => p.PlayerState == PlayerControllerPhom.EPlayerState.inTurn || p.PlayerState == PlayerControllerPhom.EPlayerState.laying);
                if (playerInTurn == null)
                {
                    //Đang ở bước chia bài
                }
                else
                {
                    GameModelPhom.IndexInTurn = playerInTurn.slotServer;
                    if (GameModelPhom.YourController.slotServer == GameModelPhom.IndexInTurn)
                    {
                        if (GameModelPhom.YourController.PlayerState == PlayerControllerPhom.EPlayerState.laying)
                            GameModelPhom.MiniState = GameModelPhom.EGameStateMini.lay_meld;
                        else
                        {
                            if (GameManager.GAME == EGame.TLMN)
                            {
                                GameModelPhom.MiniState = GameModelPhom.EGameStateMini.discard;
                            }
                            else
                            {
                                if (GameModelPhom.YourController.mCardHand.Count == 10)
                                    GameModelPhom.MiniState = GameModelPhom.EGameStateMini.discard;
                                else
                                    GameModelPhom.MiniState = GameModelPhom.EGameStateMini.stealCard_or_draw;
                            }
                        }
                    }
                }
                GameModelPhom.DealCardDone = true;
            }
            #endregion

            if (eso.variableExists("totalTime") && eso.variableExists("remainingTime"))
                StartTimeRemaining(eso.getInteger("totalTime") / 1000f, eso.getInteger("remainingTime") / 1000f);
            else if (eso.variableExists("timeAutoDealCard"))
                StartTimeAutoDeal(eso.getInteger("timeAutoDealCard") / 1000f);

            if (isAddPlayerSlot > -1)
                Listener.RegisterEventPlayerListChanged(GameModelPhom.GetPlayer(isAddPlayerSlot), false);

            if (GameModelPhom.ListPlayer.Count > 1)
            {
                if (GameManager.OldScene == ESceneName.LobbyPhom)
                {
                    Debug.LogWarning("Send DoLeaveRoom");
                    GameManager.Server.DoLeaveRoom(GameManager.Instance.selectedChannel.zoneId, GameManager.Instance.selectedChannel.roomId);
                }
            }
            #endregion
        }

        if (command != "finishGame" && command != "updateHand")
        {
            yield return new WaitForSeconds(0.1f);
            IsProcesResonseDone = true;
        }
    }

    public void RemoveRoomMasterIcon(PlayerControllerPhom p)
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
}