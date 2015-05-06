using UnityEngine;
using System.Collections;

public sealed class ChanCard : ECard
{
    #region Enum Card
    public enum ECardType
    {
        [System.ComponentModel.Description("Không rõ")]
        Unknown = -1,
        [System.ComponentModel.Description("Vạn")]
        TenThousand = 0,
        [System.ComponentModel.Description("Văn")]
        Letter = 1,
        [System.ComponentModel.Description("Sách")]
        Book = 2,
        [System.ComponentModel.Description("Chi")]
        Chi = 3
    }
    public enum ECardValue
    {
        [System.ComponentModel.Description("Không rõ")]
        Unknown = -1,
        [System.ComponentModel.Description("Nhị")]
        Two = 0,
        [System.ComponentModel.Description("Tam")]
        Three = 1,
        [System.ComponentModel.Description("Tứ")]
        Four = 2,
        [System.ComponentModel.Description("Ngũ")]
        Five = 3,
        [System.ComponentModel.Description("Lục")]
        Six = 4,
        [System.ComponentModel.Description("Thất")]
        Seven = 5,
        [System.ComponentModel.Description("Bát")]
        Eight = 6,
        [System.ComponentModel.Description("Cửu")]
        Nine = 7,
        [System.ComponentModel.Description("Chi")]
        Ten = 8,
    }

    public ECardType CardType
    {
        get
        {
            return CardId == -1 ? ECardType.Unknown : (ECardType)(CardId % 3);
        }
    }
    public ECardValue CardValue
    {
        get
        {
            return CardId == -1 ? ECardValue.Unknown : (ECardValue)Mathf.FloorToInt(CardId / 3);
        }
    }
    #endregion

    bool _isDrawFromDeck = false;
    public bool isDrawFromDeck
    {
        get { return _isDrawFromDeck; }
        set
        {
            _isDrawFromDeck = value;
            if (value)
                SetColor(new Color(153f / 255f, 153f / 255f, 153f / 255f));
        }
    }

    public override ECardTexture cardTexture
    {
        get
        {
            if (gameObject)
                return gameObject.GetComponent<CardTextureChan>();
            else
                return null;
        }
        set
        {
            base.cardTexture = value;
        }
    }

    public float timeExpire;
    public CardLib.Model.Card parentCard;
    int RankValue;
    int _cardId = -1;
    public override int CardId
    {
        get
        {
            return _cardId;
        }
        set
        {
            _cardId = value;
            if (cardTexture != null)
                cardTexture.SetValue();
        }
    }
    public ChanCard() { }
    public ChanCard(int slotIndex)
        :base(slotIndex)
    {
        originSide = currentSide = GameModelChan.GetPlayer(slotIndex).mSide;
    }

    public ChanCard(int slotIndex,int cardId)
        :base(slotIndex,cardId)
    {
        originSide = currentSide = GameModelChan.game == null ? ESide.Slot_0 : GameModelChan.GetPlayer(slotIndex) == null ? ESide.Slot_0 : GameModelChan.GetPlayer(slotIndex).mSide;
    }

    public override void ChangeSide(int slotIndex)
    {
        currentSide = GameModelChan.GetPlayer(slotIndex).mSide;
        currentSlot = slotIndex;
    }

    public void setTimeExpire(int timeExpire)
    {
        this.timeExpire = timeExpire;
        ((CardTextureChan)cardTexture).setTimeCountDown();
    }

    public override void Instantiate()
    {
        if (gameObject != null) return;

        gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Cards/Chan/card"));
        Debug.Log("Card texture :" + gameObject.GetComponent<CardTextureChan>() == null);
        cardTexture = gameObject.GetComponent<CardTextureChan>();
        cardTexture.card = this;

        cardTexture.SetValue();

        //UpdateParent(currentSlot);

        gameObject.name = (int)originSide + " " + CardId;
        if (parent != null)
            gameObject.transform.parent = parent;
        else
            gameObject.transform.parent = GameModelChan.game.mPlaymat.locationHand.parent;
        gameObject.transform.localPosition = new Vector3(0f, 0f, -1f);
        gameObject.transform.localScale = Vector3.one;
    }

    public override void Instantiate(Transform parent)
    {
        if (gameObject != null) return;
        gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Cards/Chan/card"));

        cardTexture = gameObject.GetComponent<CardTextureChan>();
        cardTexture.card = this;

        cardTexture.SetValue();

        //UpdateParent(currentSlot);

        gameObject.name = (int)originSide + " " + CardId;
        if (parent != null)
            gameObject.transform.parent = parent;
        else
            gameObject.transform.parent = GameModelChan.game.mPlaymat.locationHand.parent;
        gameObject.transform.localPosition = new Vector3(0f, 0f, -1f);
        gameObject.transform.localScale = Vector3.one;
    }

    public string ConvertToString
    {
        get
        {
            if (CardId == 24)
                return "8c";
            return ((int)CardValue).ToString() + (CardType == ECardType.TenThousand ? "j" : CardType == ECardType.Letter ? "w" : CardType == ECardType.Book ? "s" : "");
        }
    }

    public override int CompareTo(ECard c)
    {
        return CardId - c.CardId;
    }

    public override void SetColor()
    {
        gameObject.GetComponentInChildren<UITexture>().color = new Color(112 / 255f, 88 / 255f, 80 / 255f, 255 / 255f);
    }

    public override void SetColor(Color c)
    {
        if (gameObject != null)
            if (gameObject.GetComponentInChildren<UITexture>() != null)
                gameObject.GetComponentInChildren<UITexture>().color = c;
    }

    public override void SetHightlight()
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Cards/Chan/CardEffect"));
        obj.transform.parent = gameObject.transform;
        obj.transform.localPosition = new Vector3(0f, 0f, 10f);
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<UISprite>().width = 63;
        obj.GetComponent<UISprite>().height = 200;
    }
    public override string ToString()
    {
        if (CardId == 24)
            return "Chi Chi";
        return Utility.EnumUtility.GetDescription(CardValue) + " " + Utility.EnumUtility.GetDescription(CardType);
    }
    public override void UpdateParent(int slotIndex)
    {
        if (GameModelChan.GetPlayer(slotIndex) != null)
        {
            bool enableColloder = GameModelChan.GetPlayer(slotIndex).mCardHand.Contains(this) && GameModelChan.YourController != null && GameModelChan.YourController.mSide == currentSide;
            cardTexture.SetCollider(enableColloder);
        }

        if (GameModelChan.GetPlayer(slotIndex).mCardHand.Contains(this))
            gameObject.transform.parent = GameModelChan.game.mPlaymat.locationHand.parent;
        else if (GameModelChan.GetPlayer(slotIndex).mCardTrash.Contains(this))
            gameObject.transform.parent = GameModelChan.game.mPlaymat.locationTrash[(int)currentSide];
        else
        {
            foreach (StealCard stealCard in GameModelChan.GetPlayer(slotIndex).mCardSteal)
            {
                if (stealCard.steals.Contains(this))
                {
                    gameObject.transform.parent = GameModelChan.game.mPlaymat.locationStealCards[(int)currentSide];
                    break;
                }
            }
        }
        gameObject.name = (int)originSide + " " + CardId;
    }
}
