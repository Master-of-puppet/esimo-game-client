using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameModelTLMN 
{
    #region ENUM
    public enum EDiscard
    {
        FaceUp = 0,
        FaceDown = 1
    }
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

    public List<List<ECard>>[] listDiscard = new List<List<ECard>>[2];
    List<List<int>> cardCollections;
    public List<List<int>> CardCollections
    {
        get
        {
            if (cardCollections == null)
                cardCollections = new List<List<int>>();
            return cardCollections;
        }
        set
        {
            cardCollections = value;
        }
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

    private static GameModelTLMN _model = null;
    public static GameModelTLMN model
    {
        get
        {
            //if (_model == null) 
            //    _model = new GameModel();
            return _model;
        }
        set { _model = value; }
    }

    public GameModelTLMN(GamePlayTLMN gameplay)
    {
        _model = this;
        _game = gameplay;
        for (int i = 0; i < listDiscard.Length; i++)
        {
            if (listDiscard[i] == null)
            {
                listDiscard[i] = new List<List<ECard>>();
            }
        }
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
            if (value && GameModelTLMN.YourController != null)
                GameModelTLMN.game.button.UpdateButton();
        }
    }
    bool _dealCardDone = false;

    private PlayerControllerTLMN[] _listPlayer = new PlayerControllerTLMN[5];

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

            if (GameModelTLMN.YourController != null)
                GameModelTLMN.game.button.UpdateButton();
        }
    }
    private EGameStateMini _stateMini = EGameStateMini.discard;

    public static GamePlayTLMN game
    {
        get { return model._game; }
        set { model._game = value; }
    }
    private GamePlayTLMN _game = null;

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
    public static List<PlayerControllerTLMN> ListJoinGameWhenPlaying
    {
        get { return model._listJoinGameWhenPlay; }
        set { model._listJoinGameWhenPlay = value; }
    }
    List<PlayerControllerTLMN> _listJoinGameWhenPlay = new List<PlayerControllerTLMN>();


    /// <summary>
    /// Bắt đầu game. Reset lại các biến.
    /// </summary>
    public static void CreateNewGame()
    {
        GameModelTLMN.ListPlayer.ForEach(p => { if (p.IsHasQuit) GameModelTLMN.SetPlayer(p.slotServer, null); });

        ListJoinGameWhenPlaying.ForEach(p => GameModelTLMN.SetPlayer(p.slotServer, p));
        ListJoinGameWhenPlaying.Clear();

        GameModelTLMN.game.Listener.RegisterEventNewGame();

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

        GameModelTLMN.UpdatePlayerSide();

        if (IsQuitWhenEndGame)
            game.OnQuitGame(false);

        model.OnCreateNewGame();
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
                if (GameModelTLMN.YourController != null && GameModelTLMN.YourController.PlayerState == PlayerControllerTLMN.EPlayerState.waiting)
                    game.OnClickReady(null);
                GameModelTLMN.game.ClickStartGame = false;
                break;
            case EGameState.playing:
                MiniState = EGameStateMini.discard; //For Update Button
                break;
        }
    }

    IEnumerator _FinishGame()
    {
        game.ClearEffectCardPair();
        game.lbDiscard.Text = "";

        Debug.Log("Finish type: " + game.gameFinishTypeTLMN);
        GamePlayTLMN.Summary sum;

        //Mặc định tất cả đều thua nếu là kiểu đếm lá, sau đó set icon thêm sau
        if (((LobbyTLMN)GameManager.Instance.selectedLobby).config.GAME_TYPE_TLMN == LobbyTLMN.GameConfig.GameTypeLTMN.DEM_LA)
            foreach (PlayerControllerTLMN p in GameModelTLMN.ListPlayerInGame)
                p.summaryTLMN.resultTLMN = PlayerControllerTLMN.FinishGameTLMN.ResultSpriteTLMN.THOI_LA;

        if (game.gameFinishTypeTLMN == GamePlayTLMN.EFinishTypeTLMN.NORMAL_DEMLA || game.gameFinishTypeTLMN == GamePlayTLMN.EFinishTypeTLMN.NORMAL_DEMLA_DUC_BA_BICH)
        {
            SoundGamePlayTLMN.Instance.CheckFinishNormal(model);
            #region ĐẾM LÁ
            sum = game.summaryGame.Find(o => o.actionTLMN == GamePlayTLMN.Summary.EActionTLMN.THUA_DEM_LA);
            if (sum != null)
            {
                GameModelTLMN.GetPlayer(sum.targetPlayer).summaryTLMN.resultTLMN = (game.gameFinishTypeTLMN == GamePlayTLMN.EFinishTypeTLMN.NORMAL_DEMLA_DUC_BA_BICH
                    ? PlayerControllerTLMN.FinishGameTLMN.ResultSpriteTLMN.DUC_3_BICH
                    : PlayerControllerTLMN.FinishGameTLMN.ResultSpriteTLMN.THANG);
            }
            #endregion
            // play sound nhan 3
        }
        else if (game.gameFinishTypeTLMN == GamePlayTLMN.EFinishTypeTLMN.NORMAL_XEPHANG)
        {
            #region XẾP HẠNG
            sum = game.summaryGame.Find(o => o.actionTLMN == GamePlayTLMN.Summary.EActionTLMN.TOP_2);
            if (sum != null)
            {
                GameModelTLMN.GetPlayer(sum.targetPlayer).summaryTLMN.resultTLMN = PlayerControllerTLMN.FinishGameTLMN.ResultSpriteTLMN.VE_NHAT;
                GameModelTLMN.GetPlayer(sum.sourcePlayer).summaryTLMN.resultTLMN = PlayerControllerTLMN.FinishGameTLMN.ResultSpriteTLMN.VE_NHI;
            }

            sum = game.summaryGame.Find(o => o.actionTLMN == GamePlayTLMN.Summary.EActionTLMN.TOP_3);
            if (sum != null)
                GameModelTLMN.GetPlayer(sum.sourcePlayer).summaryTLMN.resultTLMN = PlayerControllerTLMN.FinishGameTLMN.ResultSpriteTLMN.VE_BA;

            sum = game.summaryGame.Find(o => o.actionTLMN == GamePlayTLMN.Summary.EActionTLMN.TOP_4);
            if (sum != null)
                GameModelTLMN.GetPlayer(sum.sourcePlayer).summaryTLMN.resultTLMN = PlayerControllerTLMN.FinishGameTLMN.ResultSpriteTLMN.VE_TU;
            #endregion
        }
        else //if (game.gameFinishTypeTLMN == Gameplay.EFinishTypeTLMN.THANG_TRANG)
        {
            //play sound thang trang
            SoundGamePlayTLMN.Instance.PlaySoundFinishGame(game.gameFinishThangTrangType);
            #region THẮNG TRẮNG
            if (game.gameFinishThangTrangType == GamePlayTLMN.EFinishByThangTrangType.TU_QUY_BANG_CAI)
            {
                sum = game.summaryGame.Find(o => o.actionTLMN == GamePlayTLMN.Summary.EActionTLMN.THANG_TRANG);
                if (sum != null)
                {
                    GameModelTLMN.GetPlayer(sum.targetPlayer).summaryTLMN.resultTLMN = PlayerControllerTLMN.FinishGameTLMN.ResultSpriteTLMN.CAY_CAI_TAO_HANG;
                    game.lbDiscard.Text = "Tứ Quý Cây Cái";
                }
            }
            else if (game.gameFinishThangTrangType == GamePlayTLMN.EFinishByThangTrangType.BA_DOI_THONG_CO_CAI)
            {
                sum = game.summaryGame.Find(o => o.actionTLMN == GamePlayTLMN.Summary.EActionTLMN.THANG_TRANG);
                if (sum != null)
                {
                    GameModelTLMN.GetPlayer(sum.targetPlayer).summaryTLMN.resultTLMN = PlayerControllerTLMN.FinishGameTLMN.ResultSpriteTLMN.CAY_CAI_TAO_HANG;
                    game.lbDiscard.Text = "Ba Đôi Thông Có Cái";
                }
            }
            else if (game.gameFinishThangTrangType == GamePlayTLMN.EFinishByThangTrangType.BON_DOI_THONG_CO_CAI)
            {
                sum = game.summaryGame.Find(o => o.actionTLMN == GamePlayTLMN.Summary.EActionTLMN.THANG_TRANG);
                if (sum != null)
                {
                    GameModelTLMN.GetPlayer(sum.targetPlayer).summaryTLMN.resultTLMN = PlayerControllerTLMN.FinishGameTLMN.ResultSpriteTLMN.CAY_CAI_TAO_HANG;
                    game.lbDiscard.Text = "Bốn Đôi Thông Có Cái";
                }
            }
            else if (game.gameFinishThangTrangType == GamePlayTLMN.EFinishByThangTrangType.TU_QUY_2)
            {
                sum = game.summaryGame.Find(o => o.actionTLMN == GamePlayTLMN.Summary.EActionTLMN.THANG_TRANG);
                if (sum != null)
                {
                    GameModelTLMN.GetPlayer(sum.targetPlayer).summaryTLMN.resultTLMN = PlayerControllerTLMN.FinishGameTLMN.ResultSpriteTLMN.THANG_TRANG;
                    game.lbDiscard.Text = "Tứ Quý Hai";
                }
            }
            else if (game.gameFinishThangTrangType == GamePlayTLMN.EFinishByThangTrangType.HAI_TU_QUY)
            {
                sum = game.summaryGame.Find(o => o.actionTLMN == GamePlayTLMN.Summary.EActionTLMN.THANG_TRANG);
                if (sum != null)
                {
                    GameModelTLMN.GetPlayer(sum.targetPlayer).summaryTLMN.resultTLMN = PlayerControllerTLMN.FinishGameTLMN.ResultSpriteTLMN.THANG_TRANG;
                    game.lbDiscard.Text = "Hai Tứ Quý";
                }
            }
            else if (game.gameFinishThangTrangType == GamePlayTLMN.EFinishByThangTrangType.NAM_DOI_THONG)
            {
                sum = game.summaryGame.Find(o => o.actionTLMN == GamePlayTLMN.Summary.EActionTLMN.THANG_TRANG);
                if (sum != null)
                {
                    GameModelTLMN.GetPlayer(sum.targetPlayer).summaryTLMN.resultTLMN = PlayerControllerTLMN.FinishGameTLMN.ResultSpriteTLMN.THANG_TRANG;
                    game.lbDiscard.Text = "Năm Đôi Thông";
                }
            }
            else if (game.gameFinishThangTrangType == GamePlayTLMN.EFinishByThangTrangType.SAU_DOI_THONG)
            {
                sum = game.summaryGame.Find(o => o.actionTLMN == GamePlayTLMN.Summary.EActionTLMN.THANG_TRANG);
                if (sum != null)
                {
                    GameModelTLMN.GetPlayer(sum.targetPlayer).summaryTLMN.resultTLMN = PlayerControllerTLMN.FinishGameTLMN.ResultSpriteTLMN.THANG_TRANG;
                    game.lbDiscard.Text = "Sáu Đôi";
                }
            }
            else if (game.gameFinishThangTrangType == GamePlayTLMN.EFinishByThangTrangType.SANH_3_DEN_A)
            {
                sum = game.summaryGame.Find(o => o.actionTLMN == GamePlayTLMN.Summary.EActionTLMN.THANG_TRANG);
                if (sum != null)
                {
                    GameModelTLMN.GetPlayer(sum.targetPlayer).summaryTLMN.resultTLMN = PlayerControllerTLMN.FinishGameTLMN.ResultSpriteTLMN.THANG_TRANG;
                    game.lbDiscard.Text = "Sảnh Từ 3 Đến A";
                }
            }
            else //if (game.gameFinishThangTrangType == Gameplay.EFinishByThangTrangType.SANH_RONG)
            {
                sum = game.summaryGame.Find(o => o.actionTLMN == GamePlayTLMN.Summary.EActionTLMN.THANG_TRANG);
                if (sum != null)
                    GameModelTLMN.GetPlayer(sum.targetPlayer).summaryTLMN.resultTLMN = PlayerControllerTLMN.FinishGameTLMN.ResultSpriteTLMN.SANH_RONG;
            }

            #endregion
        }
        //play sound
        #region SHOW OTHER HAND
        foreach (PlayerControllerTLMN p in GameModelTLMN.ListPlayerInGame)
        {
            if (p.slotServer == YourController.slotServer) continue;

            //Đặt những card ăn vào danh sách card của bản thân (Fix trường hợp có người Ù giữa trận card ăn bị kẹt chèn đè lên)
            p.mCardHand.ForEach(c => { c.originSide = c.currentSide; c.originSlot = c.currentSlot; });
            foreach (int cardId in p.summaryTLMN.inHandTLMN)
            {
                if (p.mCardHand.Find(c => c.CardId == cardId) == null)
                    p.mCardHand.Find(c => c.CardId == -1).CardId = cardId;
            }
            p.mCardHand.Sort((c1, c2) => c1.CompareTo(c2));
        }
        #endregion

        #region CHUYỂN CÁC QUÂN BÀI THẮNG TRẮNG RA GIỮA MÀN HÌNH
        if (game.gameFinishTypeTLMN == GamePlayTLMN.EFinishTypeTLMN.THANG_TRANG)
        {
            int i = 0;
            game.cardEffectFinishGame.ForEach(card =>
            {
                game.winner.mCardHand.Remove(card);
                card.cardTexture.texture.depth = i;
                iTween.MoveTo(card.gameObject, iTween.Hash("islocal", true, "time", 0.75f, "position", game.mPlaymat.GetLocationEffect(i, game.cardEffectFinishGame)));
                iTween.ScaleTo(card.gameObject, Vector3.one * 1.5f, 0.75f);
                iTween.RotateTo(card.gameObject, Vector3.zero, 0.75f);
                GameManager.Instance.FunctionDelay(delegate() { card.SetHightlight(); }, 1f);
                i++;
            });
        }
        #endregion
        game.UpdateHand();

        //0.5s cho Start Game mới. O.5s cho Gameplay Process Command Update Hand
        float time = game.TimeCountDown;

        float timeShowPoint = (time - 1) / 3f;
        float timeShowResult = timeShowPoint, timeShowMoney = timeShowPoint;

        MiniState = EGameStateMini.summary_point;
        GameModelTLMN.game.UpdateUI();
        yield return new WaitForSeconds(timeShowPoint);

        #region PLAY SOUNDS
        //        if (YourController.PlayerState >= PlayerController.EPlayerState.ready)
        //        {
        //            
        //        }
        #endregion

        MiniState = EGameStateMini.summary_result;
        GameModelTLMN.game.UpdateUI();
        yield return new WaitForSeconds(timeShowResult);

        MiniState = EGameStateMini.summary_exchange;

        GameModelTLMN.game.UpdateUserInfo();

        GameModelTLMN.game.UpdateUI();
        yield return new WaitForSeconds(timeShowMoney);

        MiniState = EGameStateMini.summary_end;
        GameModelTLMN.game.UpdateUI();
        yield return new WaitForSeconds(0.5f);

        game.IsProcesResonseDone = true;
        game.lbDiscard.Text = "";
    }

    void OnCreateNewGame()
    {
        model.listDiscard[(int)EDiscard.FaceUp].Clear();
        model.listDiscard[(int)EDiscard.FaceDown].Clear();
        game.allowChatHang = null;
    }

    IEnumerator _CreateObjects()
    {
        float time = GameModelTLMN.game.TimeCountDown / GameModelTLMN.ListPlayerInGame.Count;
        time /= 13;

        AudioManager.Instance.Play(AudioManager.SoundEffect.ChiaBai);
        for (int i = 0; i <= 13; i++)
        {
            foreach (PlayerControllerTLMN p in GameModelTLMN.ListPlayerInGame)
            {
                if (p.mCardHand.Count > i)
                {
                    int cardId = p.mCardHand[i].CardId;
                    p.mCardHand[i].CardId = -1;

                    p.mCardHand[i].Instantiate();

                    DeckCount--;

                    GameModelTLMN.game.UpdateHand(p.slotServer, time);

                    yield return new WaitForSeconds(time);
                    p.mCardHand[i].CardId = cardId;
                }
            }
        }
        DeckCount = 0;
        GameModelTLMN.game.UpdateHand();
        AudioManager.Instance.Stop(AudioManager.SoundEffect.ChiaBai);
        DealCardDone = true;
        model.GetCardCollection();
        game.IsProcesResonseDone = true;
    }

    public void GetCardCollection()
    {
        if (model != null)
        {
            string hand = "";
            YourController.mCardHand.ForEach(cardId => hand += " " + cardId.CardId.ToString());
            Debug.LogWarning("--------GetCardCollection------- : " + hand);
            CardCollections.Clear();
            List<CardLib.Model.Card> list = (from c in YourController.mCardHand select c.parentCard).ToList();
            List<List<CardLib.Model.Card>> listCardCollection = TLMNLogicCenter.GetCardCollection(list);
            foreach (List<CardLib.Model.Card> cards in listCardCollection)
            {
                List<int> listId = new List<int>();
                string str = "";
                foreach (CardLib.Model.Card card in cards)
                {
                    str += card.ToString() + " ";
                    listId.Add(card.Id);
                }
                Debug.Log("======> " + str);

                CardCollections.Add(listId);
            }
        }
    }
    
    /// <summary>
    /// Hủy các Object đã được tạo ra trong trận đấu
    /// </summary>
    public static void DestroyObject()
    {
        foreach (PlayerControllerTLMN p in GameModelTLMN.ListPlayerInGame)
        {
            p.Reset();
            p.summary.result = PlayerControllerTLMN.FinishGame.ResultSprite.None;
        }
        TLMNCardTexture[] listObj = (TLMNCardTexture[])GameObject.FindObjectsOfType(typeof(TLMNCardTexture));
        Array.ForEach<TLMNCardTexture>(listObj, obj => { GameObject.Destroy(obj.card.gameObject); });
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

        TLMNCard card = new TLMNCard(index, cardValue);
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
        game.UpdateHand(GameModelTLMN.IndexLastInTurn, 0.5f);

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

        GetPlayer(indexLast).mCardTrash.RemoveAt(indexTrash);
        GetPlayer(index).mCardHand.Add(card);

        if (YourController.slotServer == index)
            game.canRequestSort = true;
        game.stolen = false;

        game.HideClickCard();
        MiniState = EGameStateMini.discard;

        if(YourController.slotServer == index)
            game.OnClickSort(game.btSorted.gameObject);
        else
            game.UpdateHand(index, 0.5f);

        game.UpdateHand(GameModelTLMN.IndexLastInTurn, 0.5f);

        card.UpdateParent(index);
    }

    /// <summary>
    /// Khi có người chơi đánh bài
    /// </summary>
    /// <param name="index">Người đánh bài</param>
    /// <param name="cardValue">Card đã đánh</param>
    public static void Discard(int index, int [] cardValue)
    {
        model.OnDiscard(index, cardValue);
    }
    void OnDiscard(int index, int[] cardsValue)
    {
        bool canDiscard = true;
        foreach (int cardValue in cardsValue)
        {
            if (GetCard_FromHandPlayer(index, cardValue) == null)
            {
                canDiscard = false;
                break;
            }
        }
        if (canDiscard)
            _OnDiscard(index, cardsValue);
    }

    void _OnDiscard(int index, int[] cardsValue)
    {
        game.allowChatHang = null;
        game.lbDiscard.Text = "";
        game.ClearEffectCardPair();

        if (game.newTurn)
        {
            //Chờ 0.75s mới thực hiện úp card để thực hiện xong hành động UpdateTrash()
            game.UpdateTrashFaceDown();
            GameModelTLMN.ListPlayerInGame.ForEach(p => { if (p.imageSkip != null) { GameObject.Destroy(p.imageSkip); } });
        }

        if (cardsValue.Length == 0)
        {
            //Khi xác định lượt mới
            GameModelTLMN.game.button.UpdateButton();
            game.UpdateTrash(0f);
            return;
        }

        List<ECard> lastDiscard = new List<ECard>();
        foreach (int cardValue in cardsValue)
        {
            TLMNCard card = GetCard_FromHandPlayer(index, cardValue);
            GetPlayer(index).mCardHand.Remove(card);
            GetPlayer(index).mCardTrash.Add(card);
            lastDiscard.Add(card);
            card.UpdateParent(index);
        }
        model.listDiscard[(int)EDiscard.FaceUp].Add(lastDiscard);

        #region DISCARD TEXT && EFFECT PAIR
        CardControllerTLMN.EMultiType typeDiscard = game.cardController.TypeLastDiscard;

        if (lastDiscard.Count > 2)
            lastDiscard.Sort((c1, c2) => c1.CompareTo(c2));

        if (typeDiscard == CardControllerTLMN.EMultiType.Horizontal)
        {
            if (lastDiscard.Count == 2)
                game.lbDiscard.Text = "Đôi " + lastDiscard[lastDiscard.Count - 1].ToString();
            else if (lastDiscard.Count == 3)
                game.lbDiscard.Text = "Sám " + lastDiscard[lastDiscard.Count - 1].parentCard.Rank.iconToString();
            else if (lastDiscard.Count == 4)
                //game.CreateEffectCardPair(lastDiscard);
                game.lbDiscard.Text = "Tứ quý " + lastDiscard[lastDiscard.Count - 1].ToString();
        }
        else if (typeDiscard == CardControllerTLMN.EMultiType.Vertical)
            game.lbDiscard.Text = "Sảnh " + lastDiscard.Count + " lá đến " + lastDiscard[lastDiscard.Count - 1].ToString();
        else if (typeDiscard == CardControllerTLMN.EMultiType.MoreThreePairs || typeDiscard == CardControllerTLMN.EMultiType.FourPairs)
            //game.CreateEffectCardPair(lastDiscard);
            game.lbDiscard.Text = lastDiscard.Count / 2 + " đôi thông";
        #endregion

        #region PLAY SOUND
        if (GetPlayer(index).mCardHand.Count != 0)
            SoundGamePlayTLMN.Instance.PlaySound(model, game, lastDiscard, GetPlayer(index));
        #endregion

        if (YourController.slotServer == index)
            game.canRequestSort = true;

        if (YourController.slotServer == index)
        {
            GameModelTLMN.IndexInTurn = GetNextPlayer(index).slotServer;
            GameModelTLMN.IndexLastInTurn = index;
        }

        #region CƯỚP LƯỢT DÀNH CHO 4 ĐÔI THÔNG
        //Kiểm tra nếu cây cuối đánh là hai hoặc hàng thì cho phép chặt hàng
        if (
            //Dây vừa đánh ra là "Hai gì đó hoặc là đôi hai"
            (cardsValue.Length <= 2 && cardsValue[0] >= 4 && cardsValue[0] <= 7)
            || //Dây vừa đánh ra là "Tứ quý"
            (cardsValue.Length == 4 && game.cardController.TypeLastDiscard == CardControllerTLMN.EMultiType.Horizontal)
            ||
            //Dây vừa đánh ra là "Ba hoặc 4 đôi thông"
            (cardsValue.Length >= 6 && game.cardController.TypeLastDiscard == CardControllerTLMN.EMultiType.MoreThreePairs)
        )
        {
            game.allowChatHang = false;
            if (IndexInTurn == GameModelTLMN.YourController.slotServer)
            {
                List<ECard> fourPairs = game.cardController.GetFourPairs();
                if (fourPairs.Count > 0)
                {
                    game.allowChatHang = true;
                    GameManager.Instance.FunctionDelay(delegate()
                    {
                        foreach (ECard c in fourPairs)
                            game.CardClick(c);
                    }, 0f);
                }
            }
        }
        #endregion

        MiniState = EGameStateMini.discard;

        game.UpdateHand(index, 0.5f);
        game.UpdateTrash(0.5f);

        model.GetCardCollection();
    }
	
	public static bool CanDiscard(List<ECard> check)
    {
        return GameModelTLMN.MiniState == GameModelTLMN.EGameStateMini.discard
            && GameModelTLMN.IsYourTurn
            && GameModelTLMN.game.allowChatHang == null
            && GameModelTLMN.game.cardController.IsValidInTLMN(check);
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
        foreach (Meld m in GameModelTLMN.GetPlayer(indexTo).mCardMelds)
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
            List<GamePlayTLMN.GiveCard> canGive = new List<GamePlayTLMN.GiveCard>();
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
        get { return GameModelTLMN.IndexInTurn == GameModelTLMN.YourController.slotServer; }
    }
    public static PlayerControllerTLMN YourController
    {
        get { return GameModelTLMN.ListPlayer.Find(p => p.username == GameManager.Instance.mInfo.username); }
    }

    public static void SetPlayer(PlayerControllerTLMN[] _newListPlayer)
    {
        model._listPlayer = _newListPlayer;

        UpdatePlayerSide();

        game.Listener.RegisterEventSwapSlot(model._listPlayer);
    }
    public static void SetPlayer(int index, PlayerControllerTLMN p)
    {
        if (p == null)
        {
            if (model._listPlayer[index] != null)
            {
                GameModelTLMN.game.Listener.RegisterEventPlayerListChanged(model._listPlayer[index], true);

                model._listPlayer[index].Reset();

                if (model._listPlayer[index].cuiPlayer != null && model._listPlayer[index].cuiPlayer.gameObject)
                    GameObject.Destroy(model._listPlayer[index].cuiPlayer.gameObject);
            }
        }

        model._listPlayer[index] = p;

        if (p == null)
            DrawInfoPlayerNoSlot();
    }
    public static PlayerControllerTLMN GetPlayer(int index)
    {
        return model._listPlayer[index];
    }
    public static PlayerControllerTLMN GetPlayer(string username)
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

    public static PlayerControllerTLMN GetNextPlayer(int slotCheck)
    {
        int checking = slotCheck + 1; if (checking > 3) checking = 0;

        if (GetPlayer(checking) != null && !GetPlayer(checking).IsHasQuit && GetPlayer(checking).PlayerState > PlayerControllerTLMN.EPlayerState.ready)
            return GetPlayer(checking);
        else
            return GetNextPlayer(checking);
    }
    public static PlayerControllerTLMN GetLastPlayer(int slotCheck)
    {
        int checking = slotCheck - 1; if (checking < 0) checking = 3;

        if (GetPlayer(checking) != null && !GetPlayer(checking).IsHasQuit && GetPlayer(checking).PlayerState > PlayerControllerTLMN.EPlayerState.ready)
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
                if (GetPlayer(i) == null && GameModelTLMN.ListJoinGameWhenPlaying.Find(p => p.slotServer == i) == null)
                    lst.Add(i);
            }
            return lst;
        }
    }

    /// <summary>
    /// Danh sách người chơi trong phòng
    /// </summary>
    public static List<PlayerControllerTLMN> ListPlayer
    {
        get { return new List<PlayerControllerTLMN>(Array.FindAll<PlayerControllerTLMN>(model._listPlayer, p => p != null)); }
    }

    /// <summary>
    /// Danh sách người chơi trong phòng có tham gia trận đấu
    /// </summary>
    public static List<PlayerControllerTLMN> ListPlayerInGame
    {
        get { return ListPlayer.FindAll(p => (int)p.PlayerState >= (int)PlayerControllerTLMN.EPlayerState.ready); }
    }

    /// <summary>
    /// Danh sách các đối thủ của người đang chơi game.
    /// </summary>
    public static List<PlayerControllerTLMN> ListPlayerInGameEnemy
    {
        get { return GameModelTLMN.ListPlayerInGame.FindAll(p => p.slotServer != YourController.slotServer); }
    }
    #endregion

    #region CARD
    /// <summary>
    /// Lấy thông tin card của người chơi
    /// </summary>
    /// <param name="indexPlayer"></param>
    /// <param name="cardId"></param>
    /// <returns></returns>
    public static TLMNCard GetCard_FromHandPlayer(int indexPlayer, int cardId)
    {
        TLMNCard card = (TLMNCard)GetPlayer(indexPlayer).mCardHand.Find(c => c.CardId == cardId);
        if (card == null)
        {
            card = (TLMNCard)GetPlayer(indexPlayer).mCardHand.Find(c => c.CardId == -1);
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
        System.Reflection.FieldInfo[] info = GameModelTLMN.CurrentState.GetType().GetFields();
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

        foreach (PlayerControllerTLMN p in ListPlayer)
            p.mSide = SlotToSide(p.slotServer);

        foreach (PlayerControllerTLMN p in ListJoinGameWhenPlaying)
            p.mSide = SlotToSide(p.slotServer);

        //Vẽ thông tin người đang tham gia trận đấu. Khi vẽ thông tin người dùng sẽ tự động destroy các slot trống.
        ListPlayer.ForEach(p => { if (p.cuiPlayer == null && p.slotServer <= 3) CUIPlayerTLMN.Create(p, game.mPlaymat.locationPlayer[(int)p.mSide]); });
        //Vẽ thông tin người đang chờ trận đấu mới.
        ListJoinGameWhenPlaying.ForEach(p => { if (p.cuiPlayer == null) CUIPlayerTLMN.Create(p, game.mPlaymat.locationPlayer[(int)p.mSide]); });

        DrawInfoPlayerNoSlot();
    }

    public static void DrawInfoPlayerNoSlot()
    {
        //Vẽ thông tin các slot trống
        ListSlotEmpty.ForEach(slot => CUIPlayerTLMN.CreateNoSlot(slot, game.mPlaymat.locationPlayer[(int)SlotToSide(slot)]));
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

        AudioManager.Instance.Play(AudioManager.SoundEffect.OrderCard);

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

            lock (GameModelTLMN.YourController.mCardHand)
            {
                foreach (int value in sort)
                    listCard.Add(GameModelTLMN.YourController.mCardHand.
                        Find(c => c.CardId == value));
                GameModelTLMN.YourController.mCardHand = listCard;
                GameModelTLMN.game.UpdateHand(YourController.slotServer, 0.5f);
            }
        }
    }
    #endregion

    /// <summary>
    /// Khi người chơi thoát ra khỏi bàn
    /// </summary>
    public IEnumerator PlayerLeftOut(PlayerControllerTLMN p)
    {
        while (GameModelTLMN.DealCardDone == false)
            yield return new WaitForEndOfFrame();

        foreach (ECard c in p.mCardHand)
            c.SetColor();
        foreach (ECard c in p.mCardTrash)
            c.SetColor();
        foreach (Meld m in p.mCardMelds)
            foreach (ECard c in m.meld)
                c.SetColor();
    }
}
