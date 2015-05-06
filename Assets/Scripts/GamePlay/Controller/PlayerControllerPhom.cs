using UnityEngine;
using System.Collections;
using Electrotank.Electroserver5.Api;
using System.Collections.Generic;

public class PlayerControllerPhom : EPlayerController
{
    public CUIPlayerPhom cuiPlayer;

    bool _isHasQuit;
    public override bool IsHasQuit
    {
        get { return _isHasQuit; }
        set
        {
            _isHasQuit = value;

            if (value)
                cuiPlayer.IsHasQuit();
        }
    }
    EPlayerState _playerState;
    /// <summary>
    /// Trạng thái của người chơi
    /// </summary>
    public EPlayerState PlayerState
    {
        get { return _playerState; }
        set
        {
            _playerState = value;
            DoChangeState();
        }
    }
    public bool? isVirtualPlayer = null;
    public override void SetDataPlayer(EsObject es)
    {
        base.SetDataUser(es);
        SetDataOther(es);
    }
    /// <summary>
    /// Thông tin khi kết thúc trận đấu của người chơi.
    /// </summary>
    public FinishGame summary;

    public PlayerControllerPhom() { }
    public PlayerControllerPhom(EsObject es)
        :base(es)
    {
        SetDataOther(es);
        summary.inHand = new List<int>();
    }

    public void Reset()
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

        PlayerState = EPlayerState.waiting;
    }

    public override void SetDataSummary(EsObject obj)
    {
        if (obj.variableExists("disconnected"))
            summary.disconnected = obj.getBoolean("disconnected");
        if (obj.variableExists("moneyExchange"))
            long.TryParse(obj.getString("moneyExchange"), out summary.moneyExchange);
        if (obj.variableExists("sumPoint"))
            summary.sumPoint = obj.getInteger("sumPoint");
        if (obj.variableExists("sumRank"))
            summary.sumRank = obj.getInteger("sumRank");

        if (obj.variableExists("hand"))
            summary.inHand = new List<int>(obj.getIntegerArray("hand"));
    }

    public override void UpdateMoney(string money)
    {
        long.TryParse(money, out chip);
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

    public override void UpdateMoney(string money, string type)
    {
        long.TryParse(money, out chip);
    }

    public override void DoChangeState(int state)
    {
        if (GameModelPhom.game == null) return;
        if (GameModelPhom.YourController == null) return;

        if (slotServer == GameModelPhom.YourController.slotServer)
            GameModelPhom.game.button.UpdateButton();
    }
    void DoChangeState()
    {
        if (GameModelPhom.game == null) return;
        if (GameModelPhom.YourController == null) return;

        if (slotServer == GameModelPhom.YourController.slotServer)
            GameModelPhom.game.button.UpdateButton();
    }
    void SetDataOther(EsObject obj)
    {
        if (obj.variableExists("isMaster"))
            isMaster = obj.getBoolean("isMaster");
        if (obj.variableExists("slotIndex"))
            slotServer = obj.getInteger("slotIndex");
        if (obj.variableExists("playerState"))
            PlayerState = ConvertPlayerState(obj.getString("playerState"));
        if (obj.variableExists("handSize"))
            handSize = obj.getInteger("handSize");
        if (obj.variableExists("isVirtualPlayer"))
            isVirtualPlayer = obj.getBoolean("isVirtualPlayer");
    }
    public EPlayerState ConvertPlayerState(string state)
    {
        System.Reflection.FieldInfo[] info = PlayerState.GetType().GetFields();
        for (int i = 0; i < info.Length; i++)
        {
            if (i == 0) continue;
            if (info[i].Name == state) return (EPlayerState)i - 1;
        }
        return EPlayerState.none;
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
}
