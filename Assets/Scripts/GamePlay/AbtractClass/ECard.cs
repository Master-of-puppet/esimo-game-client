using UnityEngine;
public enum ESide
{
    None = -1,
    Slot_0 = 0,
    Slot_1 = 1,
    Slot_2 = 2,
    Slot_3 = 3,
    Slot_4 = 4,
    Slot_5 = 5,
    Slot_6 = 6,
    Slot_7 = 7,
    Slot_8 = 8,
    Slot_9 = 9,
    Slot_10 = 10,
    You = 0,
    Enemy_1 = 1,
    Enemy_2 = 2,
    Enemy_3 = 3
}
public abstract class ECard {
    #region Enum Card
    public CardLib.Model.Card parentCard;
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
	public virtual int CardId { get; set; }
    public ESide originSide = ESide.None, currentSide;
    public int originSlot, currentSlot;
    public GameObject gameObject;
    public virtual ECardTexture cardTexture { get; set; }
    public Transform parent = null;
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
    public ECard()
    {

    }

    public ECard(int slotIndex)
    {
        CardId = -1;
        originSlot = currentSlot = slotIndex;
    }

    public ECard(int slotIndex, int cardId)
    {
        CardId = cardId;
        originSlot = currentSlot = slotIndex;
    }   
    public abstract void ChangeSide(int slotIndex);
    public abstract void Instantiate();
    public abstract void Instantiate(Transform parent);
    public abstract int CompareTo(ECard c);
    public abstract void SetColor();
    public abstract void SetColor(Color c);
    public abstract void SetHightlight();
    public abstract void UpdateParent(int slotIndex);
}
