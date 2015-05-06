using UnityEngine;
public sealed class TLMNCard : ECard {
    int RankValue;
    public TLMNCard()
    {

    }
    public TLMNCard(int slotIndex)
        :base(slotIndex)
    {
        originSide = currentSide = GameModelTLMN.GetPlayer(slotIndex).mSide;
    }
    public TLMNCard(int slotIndex, int cardId)
        :base(slotIndex,cardId)
    {
        originSide = currentSide = GameModelTLMN.game == null ? ESide.You : GameModelTLMN.GetPlayer(slotIndex).mSide;
    }
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

            if (value == -1)
                parentCard = null;
            else
                parentCard = GameModelTLMN.game.cardController.Deck.PeekCard(value);

            if (_cardId < 8)
                RankValue = _cardId + 52;
            else
                RankValue = _cardId;
        }
    }
    public bool IsNextRank(TLMNCard c)
    {
        return (RankValue / 4) + 1 == c.RankValue / 4;
    }
    public override void ChangeSide(int slotIndex)
    {
        currentSide = GameModelTLMN.GetPlayer(slotIndex).mSide;
        currentSlot = slotIndex;
    }

    public override void Instantiate()
    {
        if (gameObject != null) return;

        gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Cards/TLMN/card"));

        cardTexture = gameObject.GetComponent<TLMNCardTexture>();
        cardTexture.card = this;

        cardTexture.SetValue();

        UpdateParent(currentSlot);

        gameObject.name = (int)originSide + " " + CardId;
        gameObject.transform.parent = GameModelTLMN.game.mPlaymat.locationHand[(int)currentSide].parent;
        gameObject.transform.localPosition = new Vector3(0f, 0f, -1f);
        gameObject.transform.localScale = Vector3.one;
    }
    
    public override void Instantiate(Transform parent)
    {
        throw new System.NotImplementedException();
    }

    public override int CompareTo(ECard c)
    {
        return RankValue - ((TLMNCard)c).RankValue;
    }

    public override void SetColor()
    {
        if (GameManager.GAME == EGame.TLMN && GameModelTLMN.GetPlayer(currentSlot).mCardTrash.Contains(this))
            return;

        gameObject.GetComponentInChildren<UITexture>().color = new Color(112 / 255f, 88 / 255f, 80 / 255f, 255 / 255f);
    }

    public override void SetColor(Color c)
    {
        gameObject.GetComponentInChildren<UITexture>().color = c;
    }

    public override void SetHightlight()
    {
        if (gameObject)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Cards/BigCard/hightlightCardPrefab"));
            obj.transform.parent = gameObject.transform;
            obj.transform.localPosition = new Vector3(0f, 0f, 10f);
            obj.transform.localScale = new Vector3(49f, 68f, 1f);
        }
    }
    GameObject imageStar;
    /// <summary>
    /// Cập nhật lại Parent của GameObject và tên.
    /// </summary>
    /// <param name="slotIndex">Index của parent mới</param>
    public override void UpdateParent(int slotIndex)
    {
        if (GameModelTLMN.GetPlayer(slotIndex).mCardHand.Contains(this))
        {
            if (currentSide != originSide && imageStar == null)
            {
                imageStar = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Cards/BigCard/CardEffect"));
                imageStar.transform.parent = gameObject.transform;
                imageStar.transform.localPosition = new Vector3(0f, 0f, -0.1f);
                imageStar.transform.localScale = new Vector3(48f, 66f, 0f);
                imageStar.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);

                //Chờ card xử lý iTween xong chạy lại effect
                GameManager.Instance.FunctionDelay(delegate() { imageStar.transform.localPosition = new Vector3(0f, 0f, -0.1f); imageStar.transform.localScale = new Vector3(48f, 66f, 0f); imageStar.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); }, 0.75f);
            }
        }

        if (GameModelTLMN.GetPlayer(slotIndex) != null)
            cardTexture.SetCollider(GameModelTLMN.GetPlayer(slotIndex).mCardHand.Contains(this)
                && GameModelTLMN.YourController.mSide == currentSide);

        if (GameModelTLMN.GetPlayer(slotIndex).mCardHand.Contains(this))
            gameObject.transform.parent = GameModelTLMN.game.mPlaymat.locationHand[(int)currentSide].parent;
        else if (GameModelTLMN.GetPlayer(slotIndex).mCardTrash.Contains(this))
            gameObject.transform.parent = GameModelTLMN.game.mPlaymat.locationTrash[currentSlot].parent;
        else
        {
            foreach (Meld meld in GameModelTLMN.GetPlayer(slotIndex).mCardMelds)
            {
                if (meld.meld.Contains(this))
                {
                    gameObject.transform.parent = GameModelTLMN.game.mPlaymat.locationMelds[currentSlot].parent;
                    break;
                }
            }
        }
        gameObject.name = (int)originSide + " " + CardId;
    }
    public override string ToString()
    {
        return parentCard.ToString();
    }
}
