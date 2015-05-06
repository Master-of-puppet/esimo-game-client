using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FACardBettingView : MonoBehaviour
{
    private static int CARD_WIDTH = 40;
    private static int CARD_HIGHT = 180;
    [HideInInspector]
    public int cardId;
    [HideInInspector]
    private PanelBetting grandParentView;
    void Awake()
    {
        CUIHandle.AddClick(gameObject.GetComponent<CUIHandle>(), OnClickChooseCard);
    }
    void OnDestroy()
    {
        CUIHandle.AddClick(gameObject.GetComponent<CUIHandle>(), OnClickChooseCard);
    }

    private void OnClickChooseCard(GameObject targetObject)
    {
        grandParentView.ChoosenCard(cardId);
    }
    public static FACardBettingView Create(int cardId, Transform parent,PanelBetting grandParent)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/GaNgoai/CardFA"));
        obj.name = "card " + cardId;
        obj.transform.parent = parent;
        obj.transform.localPosition = new Vector3(0f, 0f, -1f);
        obj.transform.localScale = Vector3.one;
        FACardBettingView card = obj.GetComponent<FACardBettingView>();
        card.SetData(cardId,grandParent);
        return card;
    }
    public void SetData(int cardId,PanelBetting grandParent)
    {
        this.cardId = cardId;
        this.grandParentView = grandParent;
        ChanCard card = new ChanCard();
        card.CardId = cardId;
        card.parent = gameObject.transform;
        card.Instantiate();
        card.cardTexture.texture.width = CARD_WIDTH;
        card.cardTexture.texture.height = CARD_HIGHT;
        GameObject.Destroy(card.gameObject.GetComponent<BoxCollider>());
    }
}

