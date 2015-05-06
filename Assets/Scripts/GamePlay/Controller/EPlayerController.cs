using UnityEngine;
using System.Collections;
using Electrotank.Electroserver5.Api;
using System.Collections.Generic;

public abstract class EPlayerController : User
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
        fullLaying = 7,
        inTurn = 8,
        outTurn = 9,
        laying = 10,
        layingDone = 11,
        finish = 12
    }
    /// <summary>
    /// Vị trí người chơi trên server.
    /// </summary>
    public int slotServer;
    /// <summary>
    /// Vị trí người chơi client.
    /// </summary>
    public ESide mSide;
    /// <summary>
    /// Danh sách card trên tay của người chơi.
    /// </summary>
    public List<ECard> mCardHand = new List<ECard>();
     /// <summary>
    /// Danh sách card đã đánh và bốc mà không ăn của người chơi.(chỉ dc sử dụng cho check âm thanh)
    /// </summary>
    public List<ECard> mCardDiscardedAndDraw = new List<ECard>();
    /// <summary>
    /// Danh sách card rác dưới chiếu
    /// </summary>
    public List<ECard> mCardTrash = new List<ECard>();
    /// <summary>
    /// Danh sách phỏm của người chơi.
    /// </summary>
    public List<Meld> mCardMelds = new List<Meld>();
    /// <summary>
    /// Người chơi là chủ phòng
    /// </summary>
    public bool isMaster;
    /// <summary>
    /// Số card trên tay (Chỉ dùng khi khởi tạo Object)
    /// </summary>
    public int handSize;
    bool _isHasQuit;
    public virtual bool IsHasQuit
    {
        get { return _isHasQuit; }
        set { _isHasQuit = value; }
    }

    EPlayerState _playerState;
    /// <summary>
    /// Trạng thái của người chơi
    /// </summary>
    /// 
    public virtual EPlayerState PlayerState
    {
        get { return _playerState; }
        set
        {
            _playerState = value;
        }
    }

    public EPlayerController()
    {

    }

    public EPlayerController(EsObject es)
        :base(es)
    {
        
    }

    public abstract void SetDataPlayer(EsObject es);
    public abstract void SetDataSummary(EsObject obj);
    public abstract void UpdateMoney(string money);
    public abstract Meld GetMeld(int[] melds);
    public abstract void UpdateMoney(string money,string type);
    /// <summary>
    /// change state of player
    /// </summary>
    /// <param name="state"></param>
    public abstract void DoChangeState(int state);
}
