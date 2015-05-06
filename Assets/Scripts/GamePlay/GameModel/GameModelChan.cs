using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModelChan
{

    #region ENUM
    /// <summary>
    /// State chính của game
    /// </summary>
    public enum EGameState
    {
        none = 0,               //Chưa có thông tin. (Client)
        waitingForPlayer = 1,   //Đang đợi người chơi nào đó tham gia. (Server)
        waitingForReady = 2,    //Đang đợi để bắt đầu trận đấu. (Server)
        dealClient = 3,         //Hiệu ứng chia bài ở client (Client)
        dealing = 4,            // Đang chia bài
        deal = 5,               //Chia bài. (Server)
        playing = 6,            //Trận đấu đang diễn ra. (Server)
        playerFullLaying = 7,   //Có người chơi báo Ù. (Server)
        finalizing = 8         //Kết thúc trận đấu. (Server)
    }

    /// <summary>
    /// State phụ của game
    /// </summary>
    public enum EGameStateMini
    {
        discard,            //đánh bài
        stealCard_or_draw,  //Ăn hoặc bốc
        stealCard_or_skip,  //Ăn hoặc dưới
        player_full_laying, //Có người chơi báo Ù
        wait_player_xuong,  //Chờ người chơi xướng

        summary_point,      //Tính điểm
        summary_result,     //Kết quả (1-2-3-Hiệu ứng)
        summary_exchange,
        summary_end         //Hiện tiền, chờ ván mới
    }
    #endregion

    private static GameModelChan _model = null;
    public static GameModelChan model
    {
        get
        {
            //if (_model == null) 
            //    _model = new GameModel();
            return _model;
        }
        set { _model = value; }
    }

    public GameModelChan(GameplayChan gameplay)
    {
        _model = this;
        _game = gameplay;
    }

    /// <summary>
    /// Index player của người chơi đang cầm lượt
    /// </summary>
    public static int IndexInTurn;
    /// <summary>
    /// Index player của người chơi lượt trước đó
    /// </summary>
    public static int IndexLastInTurn;

    /// <summary>
    /// Đã chia bài hoàn tất hay chưa ?
    /// </summary>
    public static bool DealCardDone
    {
        get { return model._dealCardDone; }
        set
        {
            model._dealCardDone = value;
            if (value)
                GameModelChan.game.button.UpdateButton();
        }
    }
    bool _dealCardDone = false;

    private PlayerControllerChan[] _listPlayer = new PlayerControllerChan[5];

    public static EGameState CurrentState
    {
        get { return model._currentState; }
    }
    private EGameState _currentState = EGameState.none;

    public static EGameStateMini MiniState
    {
        get { return model._stateMini; }
        set
        {
            model._stateMini = value;

            ////Show ảnh "Vòng hạ phỏm"
            //game.imageTurnLayMeld.SetActive(model._stateMini == EGameStateMini.lay_meld && IsYourTurn && YourController.mCardMelds.Count == 0);

            Debug.Log("Mini State = " + model._stateMini.ToString() + "\nGameplay State = " + CurrentState.ToString());
            GameModelChan.game.button.UpdateButton();
        }
    }
    private EGameStateMini _stateMini = EGameStateMini.discard;

    public static GameplayChan game
    {
        get { return model._game; }
        set { model._game = value; }
    }
    private GameplayChan _game = null;

    /// <summary>
    /// Số lượng card đang còn trên deck
    /// </summary>
    public static int DeckCount
    {
        get { return model._deckCount; }
        set { model._deckCount = value; }
    }
    private int _deckCount;

    /// <summary>
    /// Thoát game khi hết trận
    /// </summary>
    static public bool IsQuitWhenEndGame
    {
        get { return model._isQuitWhenEndGame; }
        set { model._isQuitWhenEndGame = value; }
    }
    bool _isQuitWhenEndGame = false;

    /// <summary>
    /// Có thể chíu hay không.
    /// </summary>
    static public bool IsCanChiu
    {
        get { return model._isCanChiu; }
        set { model._isCanChiu = value; }
    }
    private bool _isCanChiu;

    /// <summary>
    /// Danh sách các Object quân Nọc khi view
    /// </summary>
    public static List<GameObject> ListGameObjectNoc
    {
        get { return model._listGameObjectNoc; }
        set { model._listGameObjectNoc = value; }
    }
    List<GameObject> _listGameObjectNoc = new List<GameObject>();

    /// <summary>
    /// Danh sách các người chơi tham gia trận đấu khi trận đấu đang diễn ra.
    /// (Phải thêm mảng này vì để tách ra lưu lại được cả thông tin slot của người chơi cũ thoát ra và người chơi mới tham gia trận đấu mà không gây lỗi hệ thống)
    /// </summary>
    public static List<PlayerControllerChan> ListJoinGameWhenPlaying
    {
        get { return model._listJoinGameWhenPlay; }
        set { model._listJoinGameWhenPlay = value; }
    }
    List<PlayerControllerChan> _listJoinGameWhenPlay = new List<PlayerControllerChan>();

    /// <summary>
    /// Danh sách người chơi chầu rìa.
    /// </summary>
    public static List<PlayerControllerChan> ListWaitingPlayer
    {
        get { return model._listWaitingPlayer; }
        set { model._listWaitingPlayer = value; }
    }

    List<PlayerControllerChan> _listWaitingPlayer = new List<PlayerControllerChan>();
    /// <summary>
    /// Bắt đầu game. Reset lại các biến.
    /// </summary>
    public static void CreateNewGame()
    {
        if (GameModelChan.game.isViewCompleted)
            GameModelChan.game.isViewCompleted = !GameModelChan.game.isViewCompleted;

        GameModelChan.ListPlayer.ForEach(p => { if (p.IsHasQuit) GameModelChan.SetPlayer(p.slotServer, null); });
        ListJoinGameWhenPlaying.ForEach(p => GameModelChan.SetPlayer(p.slotServer, p));
        model.ChangePositionStealAndTrashYourController();
        ListJoinGameWhenPlaying.Clear();
        //if(GameModel.game.iconBettingUp.active)
        if (GameModelChan.game.iconBettingUp.gameObject.activeSelf)
            GameModelChan.game.iconBettingUp.gameObject.SetActive(false);
        GameModelChan.game.Listener.RegisterEventNewGame();
        GameModelChan.game.dicUserBetting.Clear();

        DeckCount = 23;
        IndexInTurn = 0;
        IndexLastInTurn = 0;

        game.stolen = false;
        game.fullLaying = false;
        game.canRequestSort = true;

        game.listGiveCard.Clear();
        game.sortList.Clear();
        game.summaryGame.Clear();
        game.meldList.Clear();

        MiniState = EGameStateMini.discard;

        GameModelChan.UpdatePlayerSide();

        if (IsQuitWhenEndGame)
            game.OnQuitGame(false);
        // clear am thanh
        SoundGameplay.Instances.ResetSoundWhenNewGame();
    }
    /// <summary>
    /// Đoạn này được viết để thêm khi người xem đc tham gia ván chơi thì vị trí trash và steal sẽ thay đổi
    /// </summary>
    public void ChangePositionStealAndTrashYourController()
    {

        if (YourController == null)
        {
            game.mPlaymat.locationStealCards[0].transform.localPosition = new Vector3(415f, 135f, -1f);
            game.mPlaymat.locationTrash[0].transform.localPosition = new Vector3(80f, -185f, -1f);
        }
        else
        {
            game.mPlaymat.locationStealCards[0].transform.localPosition = new Vector3(70f, 148f, -1f);
            game.mPlaymat.locationTrash[0].transform.localPosition = new Vector3(220f, -150f, -1f);
        }
    }
    public static void RemoveAllPlayerWhenAddWaitingPlayer()
    {
        ListWaitingPlayer.Clear();
        ListPlayer.ForEach(p => { GameModelChan.SetPlayer(p.slotServer, null); });
        ListPlayer.Clear();
    }
    /// <summary>
    /// Set state game
    /// </summary>
    /// <param name="state">State sẽ active</param>
    public static void ActiveState(EGameState state)
    {
        model._currentState = state;
        Debug.Log("Game State = " + state.ToString());
        switch (state)
        {
            case EGameState.deal:
                game.StartCoroutine(model._CreateObjects());
                break;
            case EGameState.finalizing:
                game.StartCoroutine(model._FinishGame());
                break;
            case EGameState.waitingForReady:
            case EGameState.waitingForPlayer:
                if (GameModelChan.YourController != null && GameModelChan.YourController.PlayerState == EPlayerController.EPlayerState.waiting)
                    game.OnClickReady(null);
                GameModelChan.game.ClickStartGame = false;
                break;
            case EGameState.playing:
                MiniState = EGameStateMini.discard; //For Update Button
                break;
            case EGameState.playerFullLaying:
                MiniState = EGameStateMini.player_full_laying;
                break;
        }
    }

    /// <summary>
    /// Chia bài
    /// </summary>
    private IEnumerator _CreateObjects()
    {
        if (YourController != null)
        {
            float time = GameModelChan.game.TimeCountDown;// / GameModel.ListPlayerInGame.Count;
            time /= 20;

            AudioManager.Instance.Play(AudioManager.SoundEffect.ChiaBai);
            foreach (ECard c in YourController.mCardHand)
            {
                Debug.Log("CardID  " + c.CardId);
                int cardId = c.CardId;
                c.CardId = -1;
                c.Instantiate();
                GameModelChan.game.UpdateHand(YourController.slotServer, time);
                yield return new WaitForSeconds(time);
                c.CardId = cardId;
            }
            GameModelChan.game.UpdateHand();
            AudioManager.Instance.Stop(AudioManager.SoundEffect.ChiaBai);
        }
        DealCardDone = true;
        game.IsProcesResonseDone = true;
    }

    /// <summary>
    /// Hiện thông tin, kết quả khi hết trận.
    /// </summary>
    private IEnumerator _FinishGame()
    {
        #region SHOW OTHER HAND
        foreach (PlayerControllerChan p in GameModelChan.ListPlayerInGame)
        {
            if (GameModelChan.YourController != null && p.slotServer == YourController.slotServer) continue;

            //Đặt những card ăn vào danh sách card của bản thân (Fix trường hợp có người Ù giữa trận card ăn bị kẹt chèn đè lên)
            p.mCardHand.ForEach(c => { c.originSide = c.currentSide; c.originSlot = c.currentSlot; });

            List<int> lst = new List<int>();
            while (p.summary.inHand.Count > 0)
            {
                if (p.mCardHand.Find(c => c.CardId == p.summary.inHand[0]) == null)
                    lst.Add(p.summary.inHand[0]);
                p.summary.inHand.RemoveAt(0);
            }

            foreach (int _value in lst)
            {
                ECard card = p.mCardHand.Find(c => c.CardId == -1);
                if (card != null)
                    card.CardId = _value;
            }

            p.mCardHand.Sort((c1, c2) => c1.CardId.CompareTo(c2.CardId));
        }
        game.UpdateHand();
        #endregion


        //0.5s cho Start Game mới. O.5s cho Gameplay Process Command Update Hand
        float time = game.TimeCountDown;

        float timeShowPoint = (time - 1) / 3f;
        float timeShowResult = timeShowPoint, timeShowMoney = timeShowPoint;

        MiniState = EGameStateMini.summary_point;
        GameModelChan.game.UpdateUI();
        yield return new WaitForSeconds(timeShowPoint);

        #region PLAY SOUNDS


        #endregion

        MiniState = EGameStateMini.summary_result;
        GameModelChan.game.UpdateUI();
        yield return new WaitForSeconds(timeShowResult);

        MiniState = EGameStateMini.summary_exchange;

        //GameModel.game.UpdateUserInfo();

        //GameModel.game.UpdateUI();
        //yield return new WaitForSeconds(timeShowMoney);

        MiniState = EGameStateMini.summary_end;
        GameModelChan.game.UpdateUI();
        yield return new WaitForSeconds(0.5f);
        //DestroyObject();

        game.lbDiscard.text = "";

        ListResultXuongView.Close();
        game.fullLayingEffect.Destroy();

        GameModelChan.CreateNewGame();

        ListPlayerInGame.ForEach(p =>
        {
            p.warningMessage = "";
            p.Reset();
            p.summary.result = PlayerControllerChan.FinishGame.ResultSprite.None;
        });

        game.IsProcesResonseDone = true;
    }


    /// <summary>
    /// Hủy các Object đã được tạo ra trong trận đấu
    /// </summary>
    public static void DestroyObject()
    {
        DestroyViewNoc();
        game.fullLayingEffect.Destroy();
        foreach (PlayerControllerChan p in GameModelChan.ListPlayerInGame)
            p.DestroyObject();

        CardTextureChan[] listObj = (CardTextureChan[])GameObject.FindObjectsOfType(typeof(CardTextureChan));
        Array.ForEach<CardTextureChan>(listObj, obj => { GameObject.Destroy(obj.card.gameObject); });
    }

    /// <summary>
    /// Hủy các object trong nọc mà đang xem
    /// </summary>
    public static void DestroyViewNoc()
    {
        while (ListGameObjectNoc.Count > 0)
        {
            GameObject.Destroy(ListGameObjectNoc[0]);
            ListGameObjectNoc.RemoveAt(0);
        }

        if (GameModelChan.CurrentState != EGameState.playerFullLaying || GameModelChan.MiniState != EGameStateMini.wait_player_xuong)
            game.btView.gameObject.SetActive(false);
    }

    #region các method diễn ra trong trận đấu
    /// <summary>
    /// Khi có người chơi bốc bài
    /// </summary>
    /// <param name="index">Người bốc bài</param>
    /// <param name="cardValue">Bài bốc được</param>
    public static void DrawCard(int index, int cardValue, int timeExpire)
    {
        ChanCard card = new ChanCard(index, cardValue);

        card.Instantiate(GameModelChan.game.deck.transform);
        card.isDrawFromDeck = true;
        GetPlayer(index).mCardTrash.Add(card);

        card.setTimeExpire(timeExpire);
        DeckCount--;
        game.stolen = false;

        MiniState = EGameStateMini.stealCard_or_skip;

        if (Common.CanViewHand && (YourController == null || YourController.slotServer != index))
            GetPlayer(index).mCardHand.Sort((c1, c2) => c1.CompareTo(c2));

        card.UpdateParent(index);

        game.UpdateHand(index, 0.5f);
        //game.UpdateHand(GameModel.IndexLastInTurn, 0.5f);
    }
    #endregion
    /// <summary>
    /// Khi có người chơi ăn bài
    /// </summary>
    /// <param name="index">Index Người chơi ăn</param>
    /// <param name="indexLast">Index người chơi đánh bài</param>
    public static void StealCard(int soundId, int index, int indexLast, int[] cardId)
    {
        StealCard stealCard = new StealCard();
        stealCard.player = GetPlayer(index);
        stealCard.steals.Clear();
        #region FIND CARD STEAL
        ECard cardSteal = GetPlayer(indexLast).mCardTrash[GetPlayer(indexLast).mCardTrash.Count - 1];
        GetPlayer(indexLast).mCardTrash.Remove(cardSteal);
        cardSteal.ChangeSide(index);
        stealCard.steals.Add(cardSteal);
        #endregion

        #region FIND CARD STEAL ON HAND
        for (int i = 1; i < cardId.Length; i++)
        {
            ECard cardOnHand = GetPlayer(index).mCardHand.Find(c => c.CardId == cardId[1]);
            if (cardOnHand != null)
            {
                GetPlayer(index).mCardHand.Remove(cardOnHand);
                if (GameModelChan.YourController != null && GameModelChan.YourController.slotServer != index)
                    Debug.LogError("Tìm thấy card trên tay người chơi: " + GetPlayer(index).username);
            }
            else if (cardOnHand == null && (GameModelChan.YourController == null || GameModelChan.YourController.slotServer != index))
            {
                cardOnHand = new ChanCard(index, cardId[1]);
                cardOnHand.Instantiate();
            }
            else
                Debug.LogError("Không tìm thấy card phù hợp!!!");

            stealCard.steals.Add(cardOnHand);
        }
        #endregion

        GetPlayer(index).mCardSteal.Add(stealCard);

        for (int i = 0; i < stealCard.steals.Count; i++)
            stealCard.steals[i].cardTexture.texture.depth = i;

        stealCard.steals.ForEach(c => c.UpdateParent(index));

        MiniState = EGameStateMini.discard;

        if (GameModelChan.YourController != null && YourController.slotServer == index)
            game.OnClickSort(game.btSorted.gameObject);
        else
            game.UpdateHand(index, 0.5f);

        game.UpdateHand(GameModelChan.IndexLastInTurn, 0.5f);
        if (soundId == -1)
            SoundGameplay.Instances.PlaySoundStealCard(cardId, GetPlayer(index), GetLastPlayer(index));
        else
            SoundGameplay.Instances.PlaySoundInGame(soundId, GetPlayer(index));
    }

    public static void SkipCard(int soundId, int index)
    {
        PlayerControllerChan currentPlayer = GetPlayer(index);
        if (currentPlayer.mCardTrash.Count > 0)
            currentPlayer.mCardDiscardedAndDraw.Add(currentPlayer.mCardTrash[currentPlayer.mCardTrash.Count - 1]);
        SoundGameplay.Instances.PlaySoundInGame(soundId, GetPlayer(index));
    }

    /// <summary>
    /// Khi có người chơi đánh bài
    /// </summary>
    /// <param name="index">Người đánh bài</param>
    /// <param name="cardValue">Card đã đánh</param>
    public static void Discard(int soundId, int index, int cardValue, params string[] discardToPlayer)
    {
        ECard card = GetCard_FromHandPlayer(index, cardValue);

        if (GameModelChan.YourController == null || GameModelChan.YourController.slotServer != index)
        {
            card = new ChanCard(index, cardValue);
            card.Instantiate(game.mPlaymat.locationPlayer[(int)GetPlayer(index).mSide]);
            GetPlayer(index).mCardHand.Add(card);
            card.UpdateParent(index);
        }
        GetPlayer(index).mCardDiscardedAndDraw.Add(card);
        GetPlayer(index).mCardDiscarded.Add(card);
        GetPlayer(index).mCardHand.Remove(card);

        if (discardToPlayer != null && discardToPlayer.Length > 0 && discardToPlayer[0] != null)
        {
            PlayerControllerChan p = GetPlayer(discardToPlayer[0]);
            GetPlayer(discardToPlayer[0]).mCardTrash.Add(card);
            card.ChangeSide(p.slotServer);
            card.UpdateParent(p.slotServer);
        }
        else
        {
            GetPlayer(index).mCardTrash.Add(card);
        }



        game.fullLaying = false;

        card.UpdateParent(index);
        if (YourController != null && YourController.slotServer == index)
            game.OnClickSort(game.btSorted.gameObject);
        else
            game.UpdateHand(index, 0.5f);

        if (discardToPlayer != null && discardToPlayer.Length > 0 && discardToPlayer[0] != null)
            game.UpdateHand(GetPlayer(discardToPlayer[0]).slotServer, 0.5f);
        if (discardToPlayer != null && discardToPlayer.Length > 0 && discardToPlayer[0] != null)// co' chiu' tra? cua
        {
            Debug.Log(GameModelChan.GetPlayer(discardToPlayer[0]).username + " PlayerState " + GameModelChan.GetPlayer(index).PlayerState.ToString());

            if (GameModelChan.GetPlayer(discardToPlayer[0]).PlayerState == EPlayerController.EPlayerState.inTurnStealOrIgnore)
            {
                // Chíu cây bốc
                MiniState = EGameStateMini.stealCard_or_skip;
                //SoundGameplay.Instances.playAudioDisCard(cardValue, GetPlayer(index), GetPlayer(discardToPlayer[0]), null);
            }
            else
            {
                // Chíu cây bốc đánh
                MiniState = EGameStateMini.stealCard_or_draw;
                //SoundGameplay.Instances.playAudioDisCard(cardValue, GetPlayer(index), GetNextPlayer(discardToPlayer[0]), null);
            }

        }
        else
        {
            MiniState = EGameStateMini.stealCard_or_draw;
            //SoundGameplay.Instances.playAudioDisCard(cardValue, GetPlayer(index), GetNextPlayer(index), GetLastPlayer(index));
        }
        SoundGameplay.Instances.PlaySoundInGame(soundId, GetPlayer(index));

    }
    public static bool CanDiscard(ChanCard check)
    {
        return GameModelChan.IsYourTurn
            && GameModelChan.game.cardController.CanDiscard(check)
            && GameModelChan.MiniState == GameModelChan.EGameStateMini.discard && GameModelChan.game.cardController.CanDiscard(check);
    }
    /// <summary>
    /// Xử lý hạ phỏm của người chơi nào đó
    /// </summary>
    public static void Start_WaitFullLaying()
    {
        GameModelChan.ActiveState(EGameState.playerFullLaying);
    }

    #region SET-GET PLAYER...
    /// <summary>
    /// Kiểm tra là or không lượt chơi của người đang chơi game.
    /// </summary>
    public static bool IsYourTurn
    {
        get { return GameModelChan.YourController != null && GameModelChan.IndexInTurn == GameModelChan.YourController.slotServer; }
    }
    public static PlayerControllerChan YourController
    {
        get { return GameModelChan.ListPlayer.Find(p => p.username == GameManager.Instance.mInfo.username); }
    }

    public static void SetPlayer(PlayerControllerChan[] _newListPlayer)
    {
        model._listPlayer = _newListPlayer;

        UpdatePlayerSide();

        game.Listener.RegisterEventSwapSlot(model._listPlayer);
    }
    public static void SetPlayer(int index, PlayerControllerChan p)
    {
        if (p == null)
        {
            if (model._listPlayer[index] != null)
            {
                GameModelChan.game.Listener.RegisterEventPlayerListChanged(model._listPlayer[index], true);

                model._listPlayer[index].Reset();

                if (model._listPlayer[index].cuiPlayer != null && model._listPlayer[index].cuiPlayer.gameObject)
                    GameObject.Destroy(model._listPlayer[index].cuiPlayer.gameObject);
            }
        }

        model._listPlayer[index] = p;

        if (p == null)
            DrawInfoPlayerNoSlot();
    }
    public static PlayerControllerChan GetPlayer(int index)
    {
        return model._listPlayer[index];
    }
    public static PlayerControllerChan GetPlayer(string username)
    {
        return ListPlayer.Find(p => p.username == username);
    }

    public static int NumberBot
    {
        get
        {
            return ListPlayer.FindAll(p => p.isRobot == true).Count;
        }
    }

    public static PlayerControllerChan GetNextPlayer(int slotCheck)
    {
        int checking = slotCheck + 1; if (checking > 3) checking = 0;

        if (GetPlayer(checking) != null && !GetPlayer(checking).IsHasQuit && GetPlayer(checking).PlayerState >= EPlayerController.EPlayerState.ready)
            return GetPlayer(checking);
        else
            return GetNextPlayer(checking);
    }
    public static PlayerControllerChan GetNextPlayer(string username)
    {
        return GetNextPlayer(GetPlayer(username).slotServer);
    }
    public static PlayerControllerChan GetLastPlayer(int slotCheck)
    {
        int checking = slotCheck - 1; if (checking < 0) checking = 3;

        if (GetPlayer(checking) != null && !GetPlayer(checking).IsHasQuit && GetPlayer(checking).PlayerState >= EPlayerController.EPlayerState.ready)
            return GetPlayer(checking);
        else
            return GetLastPlayer(checking);
    }

    /// <summary>
    /// Trả về các vị trí mà không lưu thông tin người chơi
    /// (Hiện không có người chơi, không có người chơi thoát ra trước đó, không có ai đang chờ trận đấu)
    /// </summary>
    public static List<int> ListSlotEmpty
    {
        get
        {
            List<int> lst = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                if (GetPlayer(i) == null && GameModelChan.ListJoinGameWhenPlaying.Find(p => p.slotServer == i) == null)
                    lst.Add(i);
            }
            return lst;
        }
    }

    /// <summary>
    /// Danh sách người chơi trong phòng
    /// </summary>
    public static List<PlayerControllerChan> ListPlayer
    {
        get { return new List<PlayerControllerChan>(Array.FindAll<PlayerControllerChan>(model._listPlayer, p => p != null)); }
    }

    /// <summary>
    /// Danh sách người chơi trong phòng có tham gia trận đấu
    /// </summary>
    public static List<PlayerControllerChan> ListPlayerInGame
    {
        get { return ListPlayer.FindAll(p => (int)p.PlayerState >= (int)PlayerControllerChan.EPlayerState.ready); }
    }

    /// <summary>
    /// Danh sách các đối thủ của người đang chơi game.
    /// </summary>
    public static List<PlayerControllerChan> ListPlayerInGameEnemy
    {

        get
        {
            if (YourController != null) return GameModelChan.ListPlayerInGame.FindAll(p => p.slotServer != YourController.slotServer);
            else
                return GameModelChan.ListPlayerInGame;
        }
    }
    #endregion

    #region CARD
    /// <summary>
    /// Lấy thông tin card của người chơi
    /// </summary>
    /// <param name="indexPlayer"></param>
    /// <param name="cardId"></param>
    /// <returns></returns>
    public static ECard GetCard_FromHandPlayer(int indexPlayer, int cardId)
    {
        ECard card = GetPlayer(indexPlayer).mCardHand.Find(c => c.CardId == cardId);
        if (card == null)
        {
            card = GetPlayer(indexPlayer).mCardHand.Find(c => c.CardId == -1);
            if (card != null)
                card.CardId = cardId;
        }
        return card;
    }
    #endregion

    /// <summary>
    /// Chuyển đổi GameState từ chuỗi sang Enum
    /// </summary>
    public static EGameState ConvertGameState(string state)
    {
        System.Reflection.FieldInfo[] info = GameModelChan.CurrentState.GetType().GetFields();
        for (int i = 0; i < info.Length; i++)
        {
            if (i == 0) continue;
            if (info[i].Name == state) return (EGameState)i - 1;
        }
        return EGameState.none;
    }

    public static ESide SlotToSide(int slotServer)
    {
        if (YourController == null)
            return (ESide)slotServer;
        ESide Side = ESide.Slot_0;
        if (YourController.slotServer == 0 || YourController.slotServer >= 4)
            Side = (ESide)slotServer;
        else if (YourController.slotServer == 1)
        {
            if (slotServer == 0)
                Side = ESide.Slot_3;
            else if (slotServer == 2)
                Side = ESide.Slot_1;
            else if (slotServer == 3)
                Side = ESide.Slot_2;
        }
        else if (YourController.slotServer == 2)
        {
            if (slotServer == 0)
                Side = ESide.Slot_2;
            else if (slotServer == 1)
                Side = ESide.Slot_3;
            else if (slotServer == 3)
                Side = ESide.Slot_1;
        }
        else if (YourController.slotServer == 3)
        {
            if (slotServer == 0)
                Side = ESide.Slot_1;
            else if (slotServer == 1)
                Side = ESide.Slot_2;
            else if (slotServer == 2)
                Side = ESide.Slot_3;
        }
        return Side;
    }

    /// <summary>
    /// Cập nhật lại vị trí và giao diện
    /// </summary>
    public static void UpdatePlayerSide()
    {
        if (YourController != null)
            YourController.mSide = ESide.Slot_0;

        foreach (PlayerControllerChan p in ListPlayer)
            p.mSide = SlotToSide(p.slotServer);

        foreach (PlayerControllerChan p in ListJoinGameWhenPlaying)
            p.mSide = SlotToSide(p.slotServer);

        //Vẽ thông tin người đang tham gia trận đấu. Khi vẽ thông tin người dùng sẽ tự động destroy các slot trống.
        ListPlayer.ForEach(p => { if (p.cuiPlayer == null && p.slotServer <= 3) CUIPlayerChan.Create(p, game.mPlaymat.locationPlayer[(int)p.mSide]); });
        //Vẽ thông tin người đang chờ trận đấu mới.
        ListJoinGameWhenPlaying.ForEach(p => { if (p.cuiPlayer == null) CUIPlayerChan.Create(p, game.mPlaymat.locationPlayer[(int)p.mSide]); });

        DrawInfoPlayerNoSlot();
    }

    public static void DrawInfoPlayerNoSlot()
    {

        //Vẽ thông tin các slot trống
        ListSlotEmpty.ForEach(slot => CUIPlayerChan.CreateNoSlot(slot, game.mPlaymat.locationPlayer[(int)SlotToSide(slot)]));
    }

    #region SORT CARD IN HAND
    bool isSortHandRandom = false;
    List<int> wasRandomSort = new List<int>();
    int lastRandomSort = 0;
    /// <summary>
    /// Xếp bài trên tay
    /// </summary>
    public static void SortHand()
    {
        if (game.sortList.Count == 0) return;

        int rand = model.lastRandomSort;
        if (model.isSortHandRandom)
        {
            if (model.wasRandomSort.Count >= game.sortList.Count)
                model.wasRandomSort.Clear();

            if (game.sortList.Count > 1)
            {
                System.Random r = new System.Random();
                while (true)
                {
                    rand = r.Next(0, game.sortList.Count);
                    if (rand != model.lastRandomSort && !model.wasRandomSort.Contains(rand))
                    {
                        model.wasRandomSort.Add(rand);
                        model.lastRandomSort = rand;
                        break;
                    }
                }
            }
        }
        else
        {
            rand++;
            if (rand >= game.sortList.Count)
                rand = 0;

            model.lastRandomSort = rand;
        }

        lock (game.sortList)
        {
            List<int> sort = game.sortList[rand];
            List<ECard> listCard = new List<ECard>();
            if (GameModelChan.YourController == null) return;
            lock (GameModelChan.YourController.mCardHand)
            {
                foreach (int value in sort)
                    listCard.Add(GameModelChan.YourController.mCardHand.Find(c => c.CardId == value));
                GameModelChan.YourController.mCardHand = listCard;
                GameModelChan.game.UpdateHand(YourController.slotServer, 0.5f);
            }
        }
    }
    #endregion

    /// <summary>
    /// Khi người chơi thoát ra khỏi bàn
    /// </summary>
    public IEnumerator PlayerLeftOut(PlayerControllerChan p)
    {
        while (GameModelChan.DealCardDone == false)
            yield return new WaitForEndOfFrame();

        foreach (ChanCard c in p.mCardHand)
            c.SetColor();
        foreach (ChanCard c in p.mCardTrash)
            c.SetColor();
        foreach (Meld m in p.mCardMelds)
            foreach (ChanCard c in m.meld)
                c.SetColor();
    }

    /// <summary>
    /// Ẩn avatar người chơi
    /// </summary>
    public static void HideAvatarPlayer(bool isResult)
    {
        Array.ForEach<Transform>(GameModelChan.game.mPlaymat.locationPlayer, player => player.gameObject.SetActive(false));
        GameModelChan.game.fullLayingEffect.ShowCardFullLaying(isResult);
    }

    /// <summary>
    /// Hiện Avatar người chơi
    /// </summary>
    public static void ShowAvatarPlayer()
    {
        Array.ForEach<Transform>(GameModelChan.game.mPlaymat.locationPlayer, player => player.gameObject.SetActive(true));
    }
}
