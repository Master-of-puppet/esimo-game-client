using UnityEngine;
using System.Collections;
using Electrotank.Electroserver5.Api;
using System.Collections.Generic;

public class PlayerControllerChan : EPlayerController
{
    public enum EPlayerState
    {
        none = 0,
        waiting = 1,
        ready = 2,
        waitingForTurn = 3,
        inTurnStealOrDrawCard = 4,
        inTurnStealOrIgnore = 5,
        inTurnDisCard = 6,
        fullLaying = 7
    }

    /// <summary>
    /// Biến này để biết xem người đang xem bàn chơi có được ưu tiên vào bàn hay không.
    /// </summary>
    public bool isPriority = false;
    /// <summary>
    /// Thông tin khi kết thúc trận đấu của người chơi.
    /// </summary>
    public FinishGame summary;
    /// <summary>
    /// Danh sách card đã đánh(chỉ dc sử dụng cho check âm thanh)
    /// </summary>
    public List<ECard> mCardDiscarded = new List<ECard>();

    /// <summary>
    /// Danh sách những cây ăn của người chơi.
    /// </summary>
    public List<StealCard> mCardSteal = new List<StealCard>();
    /// <summary>
    /// Thông tin cảnh báo, lỗi từ server
    /// </summary>
    public string warningMessage;
    public bool playSoundLenXeLaPhong = false;
    public bool playSoundThuMotConXemSao = false;
    public bool playSoundBoTomOmLeo = false;
    public bool playSounChayKoNhaTrenDe = false;
    public bool isBatBao = false;
    public CUIPlayerChan cuiPlayer;
    public override bool IsHasQuit
    {
        get
        {
            return base.IsHasQuit;
        }
        set
        {
            base.IsHasQuit = value;
            if (value)
                cuiPlayer.IsHasQuit();
        }
    }

    public override EPlayerController.EPlayerState PlayerState
    {
        get
        {
            return base.PlayerState;
        }
        set
        {
            base.PlayerState = value;
            DoChangeState();
        }
    }

    void DoChangeState()
    {
        if (GameModelChan.game == null) return;
        if (GameModelChan.YourController == null)
        {
            GameModelChan.game.button.UpdateDeck();
            return;
        }
        if (slotServer == GameModelChan.YourController.slotServer)
            GameModelChan.game.button.UpdateButton();

    }

    public PlayerControllerChan()
    {
        
    }

    public PlayerControllerChan(EsObject es)
        :base(es)
    {
        SetDataOther(es);

        summary.inHand = new List<int>();
    }

    public override void SetDataPlayer(EsObject obj)
    {
        base.SetDataUser(obj);
        SetDataOther(obj);
    }

    public override void SetDataSummary(EsObject obj)
    {
        
    }

    public override void DoChangeState(int state)
    {
        if (GameModelChan.game == null) return;
        if (GameModelChan.YourController == null)
        {
            GameModelChan.game.button.UpdateDeck();
            return;
        }
        if (slotServer == GameModelChan.YourController.slotServer)
            GameModelChan.game.button.UpdateButton();
    }

    void SetDataOther(Electrotank.Electroserver5.Api.EsObject obj)
    {
        if (obj.variableExists("isMaster"))
            isMaster = obj.getBoolean("isMaster");
        if (obj.variableExists("slotIndex"))
            slotServer = obj.getInteger("slotIndex");
        if (obj.variableExists("playerState"))
            PlayerState = ConvertPlayerState(obj.getString("playerState"));
        if (obj.variableExists("handSize"))
            handSize = obj.getInteger("handSize");
        if (obj.variableExists("isRobot"))
            isRobot = obj.getBoolean("isRobot");
        if (obj.variableExists("priority"))
            isPriority = obj.getBoolean("priority");
    }

    public EPlayerController.EPlayerState ConvertPlayerState(string state)
    {
        System.Reflection.FieldInfo[] info = PlayerState.GetType().GetFields();
        for (int i = 0; i < info.Length; i++)
        {
            if (i == 0) continue;
            if (info[i].Name == state) return (EPlayerController.EPlayerState)i - 1;
        }
        return EPlayerController.EPlayerState.none;
    }

    public void QuitWithoutRemove()
    {
        cuiPlayer.IsHasQuitWithoutRemove();
    }
    public void ProcessWhenPlayerComeback()
    {
        cuiPlayer.IsComeback();
    }

    public void DestroyObject()
    {
        handSize = 0;
        foreach (ECard c in mCardHand)
            GameObject.Destroy(c.gameObject);
        mCardHand.Clear();

        foreach (ECard c in mCardTrash)
            GameObject.Destroy(c.gameObject);
        mCardTrash.Clear();

        foreach (Meld m in mCardMelds)
            foreach (ECard c in m.meld)
                GameObject.Destroy(c.gameObject);
        mCardMelds.Clear();

        foreach (StealCard s in mCardSteal)
            foreach (ECard c in s.steals)
                GameObject.Destroy(c.gameObject);
        mCardSteal.Clear();
        mCardDiscardedAndDraw.Clear();
        mCardDiscarded.Clear();
        playSoundLenXeLaPhong = false;
        playSoundThuMotConXemSao = false;
        playSoundBoTomOmLeo = false;
        playSounChayKoNhaTrenDe = false;
        isBatBao = false;
    }

    public void Reset()
    {
        DestroyObject();

        PlayerState = EPlayerController.EPlayerState.waiting;
    }

    public struct FinishGame
    {
        /// <summary>
        /// Tên của Sprite so với hiệu ứng kết quả
        /// </summary>
        public enum ResultSprite
        {
            None,

            VE_NHAT,    //Về thứ nhất
            VE_NHI,     //Về thứ nhì
            VE_BA,      //Về thứ ba
            VE_TU,      //Về thứ tư

            MOM,        //Móm
            XAO_KHAN,   //Xào khan

            U,          //Ù
            U_TRON,     //Ù tròn
            U_XUONG,    //Ù xuông
        }

        public bool disconnected;
        public int sumRank, sumPoint;
        public long moneyExchange;

        public List<int> inHand;

        /// <summary>
        /// Kết quả của người chơi sau trận đấu
        /// </summary>
        public ResultSprite result;
    }

    public override void UpdateMoney(string money)
    {
        
    }

    public override void UpdateMoney(string money, string type)
    {
        if (type == "chip")
            long.TryParse(money, out chip);
        else if (type == "gold")
            long.TryParse(money, out gold);
    }

    public override Meld GetMeld(int[] melds)
    {
        Debug.Log("GetMeld Player= " + username + " - index=" + slotServer);
        string s = "";
        foreach (Meld mMeld in mCardMelds)
        {
            foreach (int _value in melds)
            {
                s += _value + ",";
                if (mMeld.meld.Exists(c => c.CardId == _value))
                    return mMeld;
            }
        }

        Debug.LogError("Melds: " + s);
        Debug.LogError("LỖI: Không thể tìm thấy phỏm.");
        return null;
    }
}
