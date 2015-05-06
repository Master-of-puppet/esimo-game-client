using UnityEngine;
using System.Collections;
using Electrotank.Electroserver5.Api;
using System.Collections.Generic;

public class PlayerControllerTLMN : EPlayerController
{
    public FinishGame summary;
    public FinishGameTLMN summaryTLMN;
    public CUIPlayerTLMN cuiPlayer;
    public override void SetDataPlayer(EsObject es)
    {
        base.SetDataUser(es);
        SetDataOther(es);
    }
    public PlayerControllerTLMN() { }
    public PlayerControllerTLMN(EsObject obj)
        :base(obj)
    {
        SetDataOther(obj);
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
    public override void SetDataSummary(EsObject obj)
    {
        Debug.LogWarning(username + " : " + obj);

        if (obj.variableExists("disconnected"))
        {
            summary.disconnected = obj.getBoolean("disconnected");
            summaryTLMN.disconnectedTLMN = obj.getBoolean("disconnected");
        }
        if (obj.variableExists("moneyExchange"))
            long.TryParse(obj.getString("moneyExchange"), out summaryTLMN.moneyExchangeTLMN);
        if (obj.variableExists("sumPoint"))
        {
            summary.sumPoint = obj.getInteger("sumPoint");
            summaryTLMN.sumPointTLMN = obj.getInteger("sumPoint");
        }
        if (obj.variableExists("pointLoss"))
        {
            summaryTLMN.pointLoss = obj.getInteger("pointLoss");
        }
        if (obj.variableExists("sumRank"))
        {
            summary.sumRank = obj.getInteger("sumRank");
            summaryTLMN.sumRankTLMN = obj.getInteger("sumRank");
        }
        if (obj.variableExists("hand"))
        {
            summary.inHand = new List<int>(obj.getIntegerArray("hand"));
            summaryTLMN.inHandTLMN = new List<int>(obj.getIntegerArray("hand"));
        }
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
        throw new System.NotImplementedException();
    }

    [HideInInspector]
    public GameObject imageSkip;
    public override void DoChangeState(int state)
    {
        if (GameModelTLMN.game == null) return;
        if (GameModelTLMN.YourController == null) return;

        if (slotServer == GameModelTLMN.YourController.slotServer)
            GameModelTLMN.game.button.UpdateButton();

        if (PlayerState == PlayerControllerTLMN.EPlayerState.outTurn)
        {
            if (imageSkip == null)
            {
                imageSkip = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/PlayerSkipPrefab"));
                imageSkip.transform.parent = cuiPlayer.avatar;
                imageSkip.transform.localPosition = (mSide == ESide.You) ? new Vector3(376f, 0f, 0f) : Vector3.zero;
                imageSkip.transform.localScale = Vector3.one;
            }
        }
        else if (imageSkip != null)
            GameObject.Destroy(imageSkip);
    }
    public struct FinishGameTLMN
    {
        /// <summary>
        /// Tên của Sprite so với hiệu ứng kết quả
        /// </summary>
        public enum ResultSpriteTLMN
        {
            None,

            THANG,				//Thắng đếm lá thường
            THUA_LA,			//Thua bao nhiêu đếm lá thường
            THOI_LA,            //Thối bao nhiêu lá

            VE_NHAT,    		//Về thứ nhất xếp hạng
            VE_NHI,     		//Về thứ nhì xếp hạng
            VE_BA,      		//Về thứ ba xếp hạng
            VE_TU,      		//Về thứ tư xếp hạng

            THANG_TRANG,		//Thắng trắng	

            DUC_3_BICH,         //Đúc 3 bích
            CAY_CAI_TAO_HANG,   //Cây cái tạo hàng

            SANH_RONG, 	        //Thắng trắng bằng sảnh rồng
        }

        public bool disconnectedTLMN;
        public int sumRankTLMN, sumPointTLMN;
        public long moneyExchangeTLMN;
        public int pointLoss;

        public List<int> inHandTLMN;

        /// <summary>
        /// Kết quả của người chơi sau trận đấu
        /// </summary>
        public ResultSpriteTLMN resultTLMN;
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
