using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModelPhom 
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
        deal = 3,               //Chia bài. (Server)
        playing = 4,            //Trận đấu đang diễn ra. (Server)
        finalizing = 5          //Kết thúc trận đấu. (Server)
    }

    /// <summary>
    /// State phụ của game
    /// </summary>
    public enum EGameStateMini
    {
        stealCard_or_draw,  //Ăn hoặc bốc
        discard,            //đánh bài
        lay_meld,           //Hạ phỏm

        summary_point,      //Tính điểm
        summary_result,     //Kết quả (1-2-3-Hiệu ứng)
        summary_exchange,
        summary_end         //Hiện tiền, chờ ván mới
    }
    #endregion

    private static GameModelPhom _model = null;
    public static GameModelPhom model
    {
        get
        {
            //if (_model == null) 
            //    _model = new GameModelPhom();
            return _model;
        }
        set { _model = value; }
    }

    public GameModelPhom(GamePlayPhom gameplay)
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
            if (value && GameModelPhom.YourController != null)
                GameModelPhom.game.button.UpdateButton();
        }
    }
    bool _dealCardDone = false;

    private PlayerControllerPhom[] _listPlayer = new PlayerControllerPhom[5];

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

            //Show ảnh "Vòng hạ phỏm"
            game.imageTurnLayMeld.SetActive(model._stateMini == EGameStateMini.lay_meld && IsYourTurn && YourController.mCardMelds.Count == 0);

            Debug.Log("Mini State = " + model._stateMini.ToString());

            if (GameModelPhom.YourController != null)
                GameModelPhom.game.button.UpdateButton();
        }
    }
    private EGameStateMini _stateMini = EGameStateMini.discard;

    private GamePlayPhom _game = null;
    public static GamePlayPhom game
    {
        get { return model._game; }
        set { model._game = value; }
    }

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
    /// Danh sách các người chơi tham gia trận đấu khi trận đấu đang diễn ra.
    /// (Phải thêm mảng này vì để tách ra lưu lại được cả thông tin slot của người chơi cũ thoát ra và người chơi mới tham gia trận đấu mà không gây lỗi hệ thống)
    /// </summary>
    public static List<PlayerControllerPhom> ListJoinGameWhenPlaying
    {
        get { return model._listJoinGameWhenPlay; }
        set { model._listJoinGameWhenPlay = value; }
    }
    List<PlayerControllerPhom> _listJoinGameWhenPlay = new List<PlayerControllerPhom>();


    /// <summary>
    /// Bắt đầu game. Reset lại các biến.
    /// </summary>
    public static void CreateNewGame()
    {
        GameModelPhom.ListPlayer.ForEach(p => { if (p.IsHasQuit) GameModelPhom.SetPlayer(p.slotServer, null); });

        ListJoinGameWhenPlaying.ForEach(p => GameModelPhom.SetPlayer(p.slotServer, p));
        ListJoinGameWhenPlaying.Clear();

        GameModelPhom.game.Listener.RegisterEventNewGame();

        DealCardDone = false;

        DeckCount = 52;
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

        GameModelPhom.UpdatePlayerSide();

        if (IsQuitWhenEndGame)
            game.OnQuitGame(false);
    }

    /// <summary>
    /// Set state game
    /// </summary>
    /// <param name="state">State sẽ active</param>
    public static void ActiveState(EGameState state)
    {
        model._currentState = state;
        Debug.Log("Game State = " + state.ToString());
        if (state.ToString() == "deal")
            HeaderMenu.Instance.isClickedBtnBack = true; // đang chia bài không cho Back ra
        else
            HeaderMenu.Instance.isClickedBtnBack = false;
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
                if (GameModelPhom.YourController != null && GameModelPhom.YourController.PlayerState == PlayerControllerPhom.EPlayerState.waiting)
                    game.OnClickReady(null);
                GameModelPhom.game.ClickStartGame = false;
                break;
            case EGameState.playing:
                MiniState = EGameStateMini.discard; //For Update Button
                break;
        }
    }

    /// <summary>
    /// Chia bài
    /// </summary>
    private IEnumerator _CreateObjects()
    {
        float time = GameModelPhom.game.TimeCountDown / GameModelPhom.ListPlayerInGame.Count;
        time /= 10;

        AudioManager.Instance.Play(AudioManager.SoundEffect.ChiaBai);
        for (int i = 0; i <= 9; i++)
        {
            foreach (PlayerControllerPhom p in GameModelPhom.ListPlayerInGame)
            {
                if (p.mCardHand.Count > i)
                {
                    int cardId = p.mCardHand[i].CardId;
                    p.mCardHand[i].CardId = -1;

                    p.mCardHand[i].Instantiate();

                    DeckCount--;

                    GameModelPhom.game.UpdateHand(p.slotServer, time);

                    yield return new WaitForSeconds(time);
                    p.mCardHand[i].CardId = cardId;
                }
            }
        }
        GameModelPhom.game.UpdateHand();
        AudioManager.Instance.Stop(AudioManager.SoundEffect.ChiaBai);
        DealCardDone = true;
        game.IsProcesResonseDone = true;
    }

    /// <summary>
    /// Hiện thông tin, kết quả khi hết trận.
    /// </summary>
    private IEnumerator _FinishGame()
    {
        #region SET RESULT
        if (game.gameFinishType == GamePlayPhom.EFinishType.NORMAL)
        {
            GamePlayPhom.Summary sum = game.summaryGame.Find(o => o.action == GamePlayPhom.Summary.EAction.TOP_2);
            if (sum != null)
            {
                GameModelPhom.GetPlayer(sum.targetPlayer).summary.result = PlayerControllerPhom.FinishGame.ResultSprite.VE_NHAT;
                GameModelPhom.GetPlayer(sum.sourcePlayer).summary.result = PlayerControllerPhom.FinishGame.ResultSprite.VE_NHI;
            }
            else
            {
                if (game.summaryGame.Find(o => o.action == GamePlayPhom.Summary.EAction.MOM) != null)
                    GameModelPhom.GetPlayer(game.summaryGame.Find(o => o.action == GamePlayPhom.Summary.EAction.MOM).targetPlayer).summary.result = PlayerControllerPhom.FinishGame.ResultSprite.VE_NHAT;
                else if (game.summaryGame.Find(o => o.action == GamePlayPhom.Summary.EAction.XAO_KHAN) != null)
                {
                    GamePlayPhom.Summary xaoKhan = game.summaryGame.Find(o => o.action == GamePlayPhom.Summary.EAction.XAO_KHAN);
                    GameModelPhom.GetPlayer(xaoKhan.targetPlayer).summary.result = PlayerControllerPhom.FinishGame.ResultSprite.XAO_KHAN;
                    ListPlayerInGame.ForEach(p =>
                    {
                        if (p.username != xaoKhan.targetPlayer)
                            p.summary.result = PlayerControllerPhom.FinishGame.ResultSprite.MOM;
                    });
                }
            }

            sum = game.summaryGame.Find(o => o.action == GamePlayPhom.Summary.EAction.TOP_3);
            if (sum != null)
                GameModelPhom.GetPlayer(sum.sourcePlayer).summary.result = PlayerControllerPhom.FinishGame.ResultSprite.VE_BA;

            sum = game.summaryGame.Find(o => o.action == GamePlayPhom.Summary.EAction.TOP_4);
            if (sum != null)
                GameModelPhom.GetPlayer(sum.sourcePlayer).summary.result = PlayerControllerPhom.FinishGame.ResultSprite.VE_TU;

            List<GamePlayPhom.Summary> lstSum = game.summaryGame.FindAll(o => o.action == GamePlayPhom.Summary.EAction.MOM);
            if (lstSum.Count > 0)
                foreach (GamePlayPhom.Summary s in lstSum)
                    GameModelPhom.GetPlayer(s.sourcePlayer).summary.result = PlayerControllerPhom.FinishGame.ResultSprite.MOM;
        }
        else
        {
            GamePlayPhom.Summary sum;
            if (game.gameFinishType == GamePlayPhom.EFinishType.U_THUONG || game.gameFinishType == GamePlayPhom.EFinishType.U_DEN_THUONG)
            {
                sum = game.summaryGame.Find(o => o.action == GamePlayPhom.Summary.EAction.U_THUONG || o.action == GamePlayPhom.Summary.EAction.U_TRON || o.action == GamePlayPhom.Summary.EAction.DEN_LANG);
                if (sum != null)
                    GameModelPhom.GetPlayer(sum.targetPlayer).summary.result = PlayerControllerPhom.FinishGame.ResultSprite.U;
            }
            else if (game.gameFinishType == GamePlayPhom.EFinishType.U_TRON || game.gameFinishType == GamePlayPhom.EFinishType.U_DEN_TRON)
            {
                sum = game.summaryGame.Find(o => o.action == GamePlayPhom.Summary.EAction.U_TRON || o.action == GamePlayPhom.Summary.EAction.DEN_LANG);
                if (sum != null)
                    GameModelPhom.GetPlayer(sum.targetPlayer).summary.result = PlayerControllerPhom.FinishGame.ResultSprite.U_TRON;
            }
            else if (game.gameFinishType == GamePlayPhom.EFinishType.U_XUONG)
            {
                sum = game.summaryGame.Find(o => o.action == GamePlayPhom.Summary.EAction.U_XUONG);
                if (sum != null)
                    GameModelPhom.GetPlayer(sum.targetPlayer).summary.result = PlayerControllerPhom.FinishGame.ResultSprite.U_XUONG;
            }
        }
        #endregion

        #region SHOW OTHER HAND
        foreach (PlayerControllerPhom p in GameModelPhom.ListPlayerInGame)
        {
            if (p.slotServer == YourController.slotServer) continue;

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

        #region CHUYỂN CÁC QUÂN BÀI THẮNG TRẮNG RA GIỮA MÀN HÌNH
        if (game.gameFinishType != GamePlayPhom.EFinishType.NORMAL && YourController.mCardHand.Count == 0)
        {
            int i = 0;
            YourController.mCardMelds.ForEach(meld =>
            {
                for (int j = 0; j < meld.meld.Count; j++)
                {
                    ECard card = meld.meld[j];
                    card.SetHightlight();
                    Hashtable hash = new Hashtable();
                    hash.Add("islocal", true);
                    hash.Add("time", 0.75f);
                    hash.Add("position", game.mPlaymat.GetLocationFulllaying(YourController, i, j));
                    iTween.MoveTo(card.gameObject, hash);
                    iTween.ScaleTo(card.gameObject, Vector3.one * 1.5f, 0.75f);
                }
                i++;
            });
        }
        #endregion

        //0.5s cho Start Game mới. O.5s cho GamePlayPhom Process Command Update Hand
        float time = game.TimeCountDown;

        float timeShowPoint = (time - 1) / 3f;
        float timeShowResult = timeShowPoint, timeShowMoney = timeShowPoint;

        MiniState = EGameStateMini.summary_point;
        GameModelPhom.game.UpdateUI();
        yield return new WaitForSeconds(timeShowPoint);

        #region PLAY SOUNDS

        SoundGamePhom.Instances.PlaySound(game);


        #endregion

        MiniState = EGameStateMini.summary_result;
        GameModelPhom.game.UpdateUI();
        yield return new WaitForSeconds(timeShowResult);

        MiniState = EGameStateMini.summary_exchange;

        GameModelPhom.game.UpdateUserInfo();

        GameModelPhom.game.UpdateUI();
        yield return new WaitForSeconds(timeShowMoney);

        MiniState = EGameStateMini.summary_end;
        GameModelPhom.game.UpdateUI();
        yield return new WaitForSeconds(0.5f);
        //DestroyObject();

        game.IsProcesResonseDone = true;
    }

   
    /// <summary>
    /// Hủy các Object đã được tạo ra trong trận đấu
    /// </summary>
    public static void DestroyObject()
    {
        foreach (PlayerControllerPhom p in GameModelPhom.ListPlayerInGame)
        {
            p.Reset();
            p.summary.result = PlayerControllerPhom.FinishGame.ResultSprite.None;
        }
        PhomCardTexture[] listObj = (PhomCardTexture[])GameObject.FindObjectsOfType(typeof(PhomCardTexture));
        Array.ForEach<PhomCardTexture>(listObj, obj => { GameObject.Destroy(obj.card.gameObject); });
        CreateNewGame();
    }

    #region các method diễn ra trong trận đấu
    /// <summary>
    /// Khi có người chơi bốc bài
    /// </summary>
    /// <param name="index">Người bốc bài</param>
    /// <param name="cardValue">Bài bốc được</param>
    public static void DrawCard(int index, int cardValue)
    {
        if (index != YourController.slotServer)
            if (!Common.CanViewHand)
                cardValue = -1;

        PhomCard card = new PhomCard(index, cardValue);
        card.Instantiate();
        GetPlayer(index).mCardHand.Add(card);

        DeckCount--;
        if (YourController.slotServer == index)
            game.canRequestSort = true;
        game.stolen = false;

        MiniState = EGameStateMini.discard;

        if (Common.CanViewHand && YourController.slotServer != index)
            GetPlayer(index).mCardHand.Sort((c1, c2) => c1.CompareTo(c2));

        game.UpdateHand(index, 0.5f);
        game.UpdateHand(GameModelPhom.IndexLastInTurn, 0.5f);

        card.UpdateParent(index);
    }

    /// <summary>
    /// Khi có người chơi ăn bài
    /// </summary>
    /// <param name="index">Index Người chơi ăn</param>
    /// <param name="indexLast">Index người chơi đánh bài</param>
    public static void StealCard(int index, int indexLast)
    {
        int indexTrash = GetPlayer(indexLast).mCardTrash.Count - 1;
        ECard card = GetPlayer(indexLast).mCardTrash[indexTrash];

        card.ChangeSide(index);
        bool isStealCard = SoundGamePhom.Instances.PlaySoundStealCard(GetPlayer(index), GetPlayer(indexLast));
     

        GetPlayer(indexLast).mCardTrash.RemoveAt(indexTrash);
        GetPlayer(index).mCardHand.Add(card);

        if (YourController.slotServer == index)
            game.canRequestSort = true;
        game.stolen = false;

        game.HideClickCard();
        MiniState = EGameStateMini.discard;

        if (YourController.slotServer == index)
        {
            if (isStealCard)
                game.btSorted.gameObject.AddComponent<UIContainerAnonymous>();
            game.OnClickSort(game.btSorted.gameObject);

        }
        else
            game.UpdateHand(index, 0.5f);

        game.UpdateHand(GameModelPhom.IndexLastInTurn, 0.5f);

        card.UpdateParent(index);
    }

    /// <summary>
    /// Khi có người chơi đánh bài
    /// </summary>
    /// <param name="index">Người đánh bài</param>
    /// <param name="cardValue">Card đã đánh</param>
    public static void Discard(int index, int cardValue)
    {
        ECard card = GetCard_FromHandPlayer(index, cardValue);
        //Kiểm tra có quân bài đó trên tay không thì mới đánh.
        //Nếu không có trên tay thì có thể là do quân bài đã được đánh ra rồi.
        if (card != null)
            model._Discard(index, cardValue);
    }

    void _Discard(int index, int cardValue)
    {
        ECard card = GetCard_FromHandPlayer(index, cardValue);

        GetPlayer(index).mCardHand.Remove(card);
        GetPlayer(index).mCardTrash.Add(card);

        if (YourController.slotServer == index)
            game.canRequestSort = true;
        game.fullLaying = false;

        if (game.listGiveCard.Count > 0)
            game.listGiveCard.Clear();

        if (YourController.slotServer == index)
        {
            GameModelPhom.IndexInTurn = GetNextPlayer(index).slotServer;
            GameModelPhom.IndexLastInTurn = index;
        }

        MiniState = EGameStateMini.stealCard_or_draw;

        card.UpdateParent(index);

        game.UpdateHand(index, 0.5f);

        SoundGamePhom.Instances.PlaySoundDisCard(GetPlayer(index), GetNextPlayer(index));
    }
	public static bool CanDiscard(ECard check)
    {
        return GameModelPhom.IsYourTurn
            && GameModelPhom.game.cardController.CanDiscard(check)
            && 
            (
                GameModelPhom.MiniState == GameModelPhom.EGameStateMini.discard && GameModelPhom.game.cardController.CanDiscard(check)
                ||
                (GameModelPhom.MiniState == GameModelPhom.EGameStateMini.lay_meld && GameModelPhom.YourController.mCardHand.TrueForAll(c => c.originSide == c.currentSide) ? true : false)
            );
    }

    /// <summary>
    /// Khi có người chơi gửi phỏm
    /// </summary>
    /// <param name="index">Người gửi phỏm</param>
    /// <param name="indexTo">Gửi phỏm đến người chơi</param>
    /// <param name="cardValue">Card sẽ gửi</param>
    /// <param name="melds">Phỏm ké gửi vào</param>
    public static void AddMeld(int index, int indexTo, int cardValue, int[] melds)
    {
        Meld meld = GetPlayer(indexTo).GetMeld(melds);

        if (meld != null)
        {
            ECard card = GetCard_FromHandPlayer(index, cardValue);
            GetPlayer(index).mCardHand.Remove(card);
            meld.meld.Add(card);

            game.UpdateHand(index, 0.5f);
            game.UpdateHand(indexTo, 0.5f);

            CheckRemoveGiveCard(index);

            card.UpdateParent(indexTo);
            return;
        }

        if (YourController.slotServer == index)
            game.canRequestSort = true;

        Debug.Log(index);
        Debug.Log(indexTo);
        foreach (Meld m in GameModelPhom.GetPlayer(indexTo).mCardMelds)
            Debug.Log(m.meld);
        Debug.Log(cardValue);
        Debug.Log(melds);

        Debug.LogError("LỖI: Không thể tìm thấy phỏm để gửi");
    }

    /// <summary>
    /// Thông báo bắt đầu lượt hạ phỏm
    /// </summary>
    /// <param name="index">Vòng hạ phỏm của người chơi</param>
    public static void StartLayMeld(int index)
    {
        //if (game.playerHasNoMeld)
        //    MiniState = EGameStateMini.discard;
        //else
        MiniState = EGameStateMini.lay_meld;
    }

    /// <summary>
    /// Xử lý hạ phỏm của người chơi nào đó
    /// </summary>
    /// <param name="index">Người hạ phỏm</param>
    public static void LayMeld(int index)
    {
        foreach (Meld meld in GetPlayer(index).mCardMelds)
            foreach (ECard card in meld.meld)
                card.UpdateParent(index);

        CheckRemoveGiveCard(index);

        if (YourController.slotServer == index)
            game.canRequestSort = true;
        game.fullLaying = false;

        //MiniState = EGameStateMini.discard;
        MiniState = EGameStateMini.lay_meld;

        if (index == YourController.slotServer)
            YourController.cuiPlayer.UpdateLocationTimerCountDown();

        game.UpdateHand(index, 0.5f);
    }

    /// <summary>
    /// Kiểm tra xem có thể gửi những card nào
    /// </summary>
    /// <param name="index">index người chơi</param>
    public static void CheckRemoveGiveCard(int index)
    {
        //Khi hạ phỏm check lại những card có thể gửi.
        if (index == YourController.slotServer && game.listGiveCard.Count > 0)
        {
            List<GamePlayPhom.GiveCard> canGive = new List<GamePlayPhom.GiveCard>();
            while (game.listGiveCard.Count > 0)
            {
                if (GetPlayer(index).mCardHand.Find(c => c.CardId == game.listGiveCard[0].cardId) != null)
                    canGive.Add(game.listGiveCard[0]);
                game.listGiveCard.RemoveAt(0);
            }
            game.listGiveCard = canGive;
        }
    }
    #endregion

    #region SET-GET PLAYER...
    /// <summary>
    /// Kiểm tra là or không lượt chơi của người đang chơi game.
    /// </summary>
    public static bool IsYourTurn
    {
        get { return GameModelPhom.IndexInTurn == GameModelPhom.YourController.slotServer; }
    }
    public static PlayerControllerPhom YourController
    {
        get { return GameModelPhom.ListPlayer.Find(p => p.username == GameManager.Instance.mInfo.username); }
    }

    public static void SetPlayer(PlayerControllerPhom[] _newListPlayer)
    {
        model._listPlayer = _newListPlayer;

        UpdatePlayerSide();

        game.Listener.RegisterEventSwapSlot(model._listPlayer);
    }
    public static void SetPlayer(int index, PlayerControllerPhom p)
    {
        if (p == null)
        {
            if (model._listPlayer[index] != null)
            {
                GameModelPhom.game.Listener.RegisterEventPlayerListChanged(model._listPlayer[index], true);

                model._listPlayer[index].Reset();

                if (model._listPlayer[index].cuiPlayer != null && model._listPlayer[index].cuiPlayer.gameObject)
                    GameObject.Destroy(model._listPlayer[index].cuiPlayer.gameObject);
            }
        }
        Debug.Log("Set model:" + index + ":p" + p);
        model._listPlayer[index] = p;

        if (p == null)
            DrawInfoPlayerNoSlot();
    }
    public static PlayerControllerPhom GetPlayer(int index)
    {
        return model._listPlayer[index];
    }
    public static PlayerControllerPhom GetPlayer(string username)
    {
        return ListPlayer.Find(p => p.username == username);
    }

    public static int NumberBot
    {
        get
        {
            return ListPlayer.FindAll(p => p.username.IndexOf("esimo_") == 0).Count;
        }
    }

    public static PlayerControllerPhom GetNextPlayer(int slotCheck)
    {
        int checking = slotCheck + 1; if (checking > 3) checking = 0;

        if (GetPlayer(checking) != null && !GetPlayer(checking).IsHasQuit && GetPlayer(checking).PlayerState > PlayerControllerPhom.EPlayerState.ready)
            return GetPlayer(checking);
        else
            return GetNextPlayer(checking);
    }
    public static PlayerControllerPhom GetLastPlayer(int slotCheck)
    {
        int checking = slotCheck - 1; if (checking < 0) checking = 3;

        if (GetPlayer(checking) != null && !GetPlayer(checking).IsHasQuit && GetPlayer(checking).PlayerState > PlayerControllerPhom.EPlayerState.ready)
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
                if (GetPlayer(i) == null && GameModelPhom.ListJoinGameWhenPlaying.Find(p => p.slotServer == i) == null)
                    lst.Add(i);
            }
            return lst;
        }
    }

    /// <summary>
    /// Danh sách người chơi trong phòng
    /// </summary>
    public static List<PlayerControllerPhom> ListPlayer
    {
        get { return new List<PlayerControllerPhom>(Array.FindAll<PlayerControllerPhom>(model._listPlayer, p => p != null)); }
    }

    /// <summary>
    /// Danh sách người chơi trong phòng có tham gia trận đấu
    /// </summary>
    public static List<PlayerControllerPhom> ListPlayerInGame
    {
        get { return ListPlayer.FindAll(p => (int)p.PlayerState >= (int)PlayerControllerPhom.EPlayerState.ready); }
    }

    /// <summary>
    /// Danh sách các đối thủ của người đang chơi game.
    /// </summary>
    public static List<PlayerControllerPhom> ListPlayerInGameEnemy
    {
        get { return GameModelPhom.ListPlayerInGame.FindAll(p => p.slotServer != YourController.slotServer); }
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
        System.Reflection.FieldInfo[] info = GameModelPhom.CurrentState.GetType().GetFields();
        for (int i = 0; i < info.Length; i++)
        {
            if (i == 0) continue;
            if (info[i].Name == state) return (EGameState)i - 1;
        }
        return EGameState.none;
    }

    public static ESide SlotToSide(int slotServer)
    {
        ESide Side = ESide.You;

        //if (YourController.slotServer != slotServer) 
        //{
        //    if (slotServer > YourController.slotServer)
        //        Side = (ESide)(slotServer - YourController.slotServer);
        //    else
        //        Side = (ESide)(3 - slotServer);
        //}
        //Debug.Log("Username: " + p.username + ", Side: " + Side.ToString() + ", Slot: " + slotServer);

        if (YourController.slotServer == 0 || YourController.slotServer >= 4)
            Side = (ESide)slotServer;
        else if (YourController.slotServer == 1)
        {
            if (slotServer == 0)
                Side = ESide.Enemy_3;
            else if (slotServer == 2)
                Side = ESide.Enemy_1;
            else if (slotServer == 3)
                Side = ESide.Enemy_2;
        }
        else if (YourController.slotServer == 2)
        {
            if (slotServer == 0)
                Side = ESide.Enemy_2;
            else if (slotServer == 1)
                Side = ESide.Enemy_3;
            else if (slotServer == 3)
                Side = ESide.Enemy_1;
        }
        else if (YourController.slotServer == 3)
        {
            if (slotServer == 0)
                Side = ESide.Enemy_1;
            else if (slotServer == 1)
                Side = ESide.Enemy_2;
            else if (slotServer == 2)
                Side = ESide.Enemy_3;
        }
        return Side;
    }

    /// <summary>
    /// Cập nhật lại vị trí và giao diện
    /// </summary>
    public static void UpdatePlayerSide()
    {
        YourController.mSide = ESide.You;

        foreach (PlayerControllerPhom p in ListPlayer)
            p.mSide = SlotToSide(p.slotServer);

        foreach (PlayerControllerPhom p in ListJoinGameWhenPlaying)
            p.mSide = SlotToSide(p.slotServer);

        //Vẽ thông tin người đang tham gia trận đấu. Khi vẽ thông tin người dùng sẽ tự động destroy các slot trống.
        ListPlayer.ForEach(p => {
            Debug.Log("Vi tri nguoi choi:" + (int)p.mSide);
            if (p.cuiPlayer == null && p.slotServer <= 3) CUIPlayerPhom.Create(p, game.mPlaymat.locationPlayer[(int)p.mSide]); 
        });
        //Vẽ thông tin người đang chờ trận đấu mới.
        ListJoinGameWhenPlaying.ForEach(p => { if (p.cuiPlayer == null) CUIPlayerPhom.Create(p, game.mPlaymat.locationPlayer[(int)p.mSide]); });

        DrawInfoPlayerNoSlot();
    }

    public static void DrawInfoPlayerNoSlot()
    {
        //Vẽ thông tin các slot trống
        ListSlotEmpty.ForEach(slot => CUIPlayerPhom.CreateNoSlot(slot, game.mPlaymat.locationPlayer[(int)SlotToSide(slot)]));
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

            lock (GameModelPhom.YourController.mCardHand)
            {
                foreach (int value in sort)
                    listCard.Add(GameModelPhom.YourController.mCardHand.
                        Find(c => c.CardId == value));
                GameModelPhom.YourController.mCardHand = listCard;
                GameModelPhom.game.UpdateHand(YourController.slotServer, 0.5f);
            }
        }
    }
    #endregion

    /// <summary>
    /// Khi người chơi thoát ra khỏi bàn
    /// </summary>
    public IEnumerator PlayerLeftOut(PlayerControllerPhom p)
    {
        while (GameModelPhom.DealCardDone == false)
            yield return new WaitForEndOfFrame();

        foreach (ECard c in p.mCardHand)
            c.SetColor();
    }
}
