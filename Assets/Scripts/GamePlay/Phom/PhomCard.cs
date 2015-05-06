using UnityEngine;
public sealed class PhomCard : ECard {
    int _cardId = -1;
    public override int CardId
    {
        get { return _cardId; }
        set
        {
            _cardId = value;
            if (cardTexture != null)
                cardTexture.SetValue();

            if (value == -1)
                parentCard = null;
            else
                parentCard = GameModelPhom.game.cardController.Deck.PeekCard(value);
        }
    }
    public PhomCard(int slotIndex)
        :base(slotIndex)
    {
        originSide = currentSide = GameModelPhom.GetPlayer(slotIndex).mSide;
    }
    public PhomCard(int slotIndex, int cardId)
        :base(slotIndex,cardId)
    {
        originSide = currentSide = GameModelPhom.game == null ? ESide.You : GameModelPhom.GetPlayer(slotIndex).mSide;
    }
    
    public override void ChangeSide(int slotIndex)
    {
        currentSide = GameModelPhom.GetPlayer(slotIndex).mSide;
        currentSlot = slotIndex;
    }

    public override void Instantiate()
    {
        if (gameObject != null) return;

        gameObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Cards/Phom/card"));

        cardTexture = gameObject.GetComponent<PhomCardTexture>();
        cardTexture.card = this;

        cardTexture.SetValue();

        UpdateParent(currentSlot);

        gameObject.name = (int)originSide + " " + CardId;
        gameObject.transform.parent = GameModelPhom.game.mPlaymat.locationHand[(int)currentSide].parent;
        gameObject.transform.localPosition = new Vector3(0f, 0f, -1f);
        gameObject.transform.localScale = Vector3.one;
    }

    public override void Instantiate(Transform parent)
    {
        Debug.LogError("Not implement");
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
        gameObject.GetComponentInChildren<UITexture>().color = c;
    }

    public override void SetHightlight()
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Cards/BigCard/hightlightCardPrefab"));
        obj.transform.parent = gameObject.transform;
        obj.transform.localPosition = new Vector3(0f, 0f, 10f);
        obj.transform.localScale = new Vector3(49f, 68f, 1f);
    }
    GameObject imageStar;
    /// <summary>
    /// Cập nhật lại Parent của GameObject và tên.
    /// </summary>
    /// <param name="slotIndex">Index của parent mới</param>
    public override void UpdateParent(int slotIndex)
    {
        if (GameModelPhom.GetPlayer(slotIndex).mCardHand.Contains(this))
        {
            if (currentSide != originSide && imageStar == null)
            {
                imageStar = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Cards/BigCard/CardEffect"));
                imageStar.transform.parent = gameObject.transform;
                imageStar.GetComponent<UISprite>().depth = gameObject.GetComponent<ECardTexture>().texture.depth;
                imageStar.transform.localPosition = new Vector3(0f, 0f, -0.1f);
                imageStar.transform.localScale = new Vector3(0.48f, 0.48f, 1f);
                imageStar.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);

                //Chờ card xử lý iTween xong chạy lại effect
                GameManager.Instance.FunctionDelay(delegate() { imageStar.transform.localPosition = new Vector3(0f, 0f, -0.1f); imageStar.transform.localScale = new Vector3(0.48f, 0.48f, 1f); imageStar.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); }, 0.75f);
            }
        }

        if (GameModelPhom.GetPlayer(slotIndex) != null)
            cardTexture.SetCollider(GameModelPhom.GetPlayer(slotIndex).mCardHand.Contains(this)
                && GameModelPhom.YourController.mSide == currentSide);

        if (GameModelPhom.GetPlayer(slotIndex).mCardHand.Contains(this))
            gameObject.transform.parent = GameModelPhom.game.mPlaymat.locationHand[(int)currentSide].parent;
        else if (GameModelPhom.GetPlayer(slotIndex).mCardTrash.Contains(this))
            gameObject.transform.parent = GameModelPhom.game.mPlaymat.locationTrash[currentSlot].parent;
        else
        {
            foreach (Meld meld in GameModelPhom.GetPlayer(slotIndex).mCardMelds)
            {
                if (meld.meld.Contains(this))
                {
                    gameObject.transform.parent = GameModelPhom.game.mPlaymat.locationMelds[currentSlot].parent;
                    break;
                }
            }
        }

        gameObject.name = (int)originSide + " " + CardId;
    }
}
