using UnityEngine;
using System.Collections;
using System;
using Electrotank.Electroserver5.Api;
using System.Collections.Generic;

public class PanelBetting : MonoBehaviour
{
    #region Unity Editor
    public UIToggle cbLarge, cbSmall;
    public CUIHandle btnMinus, btnPlus, btnSubmit;
    public UILabel lbChipBet, lbCardBet;
    public UIGrid tableCardFA;
    public Transform cardParent;
    public BettingView parent;
    #endregion
    private int CARD_WIDTH = 60;
    private int CARD_HIGHT = 270;
    private int cardId = -1;
    private EsObject esObj;
    void Awake()
    {
        CUIHandle.AddClick(btnMinus, OnClickListenerMinus);
        CUIHandle.AddClick(btnPlus, OnClickListenerPlus);
        CUIHandle.AddClick(btnSubmit, OnClickListenerSubmit);
    }
    void Update()
    {
        if (!gameObject.active)
            if (esObj != null)
                esObj = null;
		if (!cbLarge.value && !cbSmall.value) {
				btnSubmit.gameObject.GetComponentInChildren<UISprite> ().color = new Color (1f, 1f, 1f, 90f / 255f);
				btnSubmit.gameObject.transform.collider.enabled = false;
		} else {
			if(!btnSubmit.gameObject.transform.collider.enabled)
			{
				btnSubmit.gameObject.GetComponentInChildren<UISprite> ().color = new Color (1f, 1f, 1f, 255f / 255f);
				btnSubmit.gameObject.transform.collider.enabled = true;
			}
		}
						
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnMinus, OnClickListenerMinus);
        CUIHandle.RemoveClick(btnPlus, OnClickListenerPlus);
        CUIHandle.RemoveClick(btnSubmit, OnClickListenerSubmit);
    }
    internal void ChoosenCard(int cardID)
    {
        if (parent.model != null && parent.model.CardId != cardID && btnSubmit.gameObject.transform.collider.enabled == false)
        {
            btnSubmit.gameObject.GetComponentInChildren<UISprite>().color = Color.white;
            btnSubmit.gameObject.transform.collider.enabled = true;
        }
        else if (parent.model != null && parent.model.CardId == cardID && btnSubmit.gameObject.transform.collider.enabled == true)
        {
            btnSubmit.gameObject.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 90f / 255f);
            btnSubmit.gameObject.transform.collider.enabled = false;
        }
        for (int i = 0; i < tableCardFA.transform.childCount; i++)
        {
            if (tableCardFA.transform.GetChild(i).GetComponent<FACardBettingView>().cardId == cardID)
                tableCardFA.transform.GetChild(i).GetComponent<FACardBettingView>().gameObject.SetActive(false);
            else
            {
                if (!tableCardFA.transform.GetChild(i).GetComponent<FACardBettingView>().gameObject.activeSelf)
                    tableCardFA.transform.GetChild(i).GetComponent<FACardBettingView>().gameObject.SetActive(true);
            }
        }
        tableCardFA.Reposition() ;
        cardId = cardID;
        for (int i = 0; i < cardParent.childCount; i++)
        {
            GameObject.Destroy(cardParent.transform.GetChild(i).gameObject);
        }
        ChanCard card = new ChanCard();
        card.CardId = cardID;
        card.parent = cardParent;
        card.Instantiate();
        card.cardTexture.texture.width = CARD_WIDTH;
        card.cardTexture.texture.height = CARD_HIGHT;
        GameObject.Destroy(card.gameObject.GetComponent<BoxCollider>());
        lbCardBet.text = card.ToString();
    }
    private void OnClickListenerPlus(GameObject targetObject)
    {
        lbChipBet.text = (Convert.ToInt32(lbChipBet.text) + ((LobbyChan)GameManager.Instance.selectedLobby).betting).ToString();
        if (Convert.ToInt64(lbChipBet.text) >= parent.maxChipAllow)
        {
            lbChipBet.text = "" + parent.maxChipAllow;
            btnPlus.gameObject.collider.enabled = false;
            Array.ForEach<UISprite>(btnPlus.GetComponentsInChildren<UISprite>(), s => s.color = new Color(1f, 1f, 1f, 90f / 255f));
        }
        if (btnMinus.gameObject.collider.enabled == false)
        {
            Array.ForEach<UISprite>(btnMinus.gameObject.GetComponentsInChildren<UISprite>(), s => s.color = Color.white);
            btnMinus.gameObject.collider.enabled = true;
        }
        if (btnSubmit.gameObject.transform.collider.enabled == false)
        {
            btnSubmit.gameObject.GetComponentInChildren<UISprite>().color = Color.white;
            btnSubmit.gameObject.transform.collider.enabled = true;
        }
    }

    private void OnClickListenerMinus(GameObject targetObject)
    {
        int moneyMinus = Convert.ToInt32(lbChipBet.text) - ((LobbyChan)GameManager.Instance.selectedLobby).betting;
        if (moneyMinus >= parent.model.ChipBetting)
        {
            lbChipBet.text = moneyMinus.ToString();
            if ((parent.model.ChipBetting == 0 && Convert.ToInt64(lbChipBet.text) == ((LobbyChan)GameManager.Instance.selectedLobby).betting) || Convert.ToInt64(lbChipBet.text) == parent.model.ChipBetting)
            {
                btnMinus.gameObject.collider.enabled = false;
                Array.ForEach<UISprite>(btnMinus.GetComponentsInChildren<UISprite>(), s => s.color = new Color(1f, 1f, 1f, 90f / 255f));
            }
        }
        else
        {
            btnMinus.gameObject.collider.enabled = false;
            Array.ForEach<UISprite>(btnMinus.GetComponentsInChildren<UISprite>(), s => s.color = new Color(1f, 1f, 1f, 90f / 255f));
            btnSubmit.gameObject.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 90f / 255f);
            btnSubmit.gameObject.transform.collider.enabled = false;
        }
        if (!btnPlus.gameObject.collider.enabled)
        {
            btnPlus.gameObject.collider.enabled = true;
            Array.ForEach<UISprite>(btnPlus.GetComponentsInChildren<UISprite>(), s => s.color = Color.white);
        }
    }
    private void OnClickListenerSubmit(GameObject targetObject)
    {
        ETypeLayingBetting types = ETypeLayingBetting.None;
        if (cbLarge.value && cbSmall.value)
        {
            types = ETypeLayingBetting.RongHep;
        }
        else if (cbLarge.value && !cbSmall.value)
        {
            types = ETypeLayingBetting.Rong;
        }
        else if (!cbLarge.value && cbSmall.value)
        {
            types = ETypeLayingBetting.Hep;
        }
        if (cardId == -1 || (cardId == parent.model.CardId && parent.model.ChipBetting == Convert.ToInt64(lbChipBet.text) && types == ETypeLayingBetting.None))
        {
            NotificationView.ShowMessage("Bạn chưa chọn luật hoặc quân ù hoặc chưa đổi mức tiền", 3f);
            return;
        }
        PlayerBettingModel model = new PlayerBettingModel();
        model.CardId = cardId;
        model.ETypeLaying = types;
        model.ChipBetting = Convert.ToInt64(lbChipBet.text);
        if (esObj != null && esObj.variableExists("textNotification"))
            NotificationView.ShowConfirm("Chú ý", esObj.getString("textNotification"), delegate()
            {
                GameManager.Server.DoRequestPluginGame(Utility.SetEsObject(Fields.GAMEPLAY.PLAY, new object[] { Fields.ACTION, Fields.REQUEST.GA_NGOAI, "gaNgoaiType", (int)model.ETypeLaying, "value", model.ChipBetting, "cardId", model.CardId }));
            }, delegate()
            {

            }, "Tiếp tục", "Hủy bỏ");
        else
        {
            GameManager.Server.DoRequestPluginGame(Utility.SetEsObject(Fields.GAMEPLAY.PLAY, new object[] { Fields.ACTION, Fields.REQUEST.GA_NGOAI, "gaNgoaiType", (int)model.ETypeLaying, "value", model.ChipBetting, "cardId", model.CardId }));
        }
    }

    public void HideAllButton()
    {
        switch (parent.model.ETypeLaying)
        {
            case ETypeLayingBetting.Hep:
                Array.ForEach<UISprite>(cbSmall.GetComponentsInChildren<UISprite>(), s => s.color = new Color(1f, 1f, 1f, 90f / 255f));
                cbLarge.value = false;
                cbSmall.gameObject.collider.enabled = false;
                break;
            case ETypeLayingBetting.Rong:
                Array.ForEach<UISprite>(cbLarge.GetComponentsInChildren<UISprite>(), s => s.color = new Color(1f, 1f, 1f, 90f / 255f));
                cbSmall.value = false;
                cbLarge.gameObject.collider.enabled = false;
                break;
            case ETypeLayingBetting.RongHep:
                cbLarge.gameObject.collider.enabled = false;
                cbSmall.gameObject.collider.enabled = false;
                Array.ForEach<UISprite>(cbLarge.GetComponentsInChildren<UISprite>(), s => s.color = new Color(1f, 1f, 1f, 90f / 255f));
                Array.ForEach<UISprite>(cbSmall.GetComponentsInChildren<UISprite>(), s => s.color = new Color(1f, 1f, 1f, 90f / 255f));
                break;
        }
        btnMinus.gameObject.collider.enabled = false;
        Array.ForEach<UISprite>(btnMinus.GetComponentsInChildren<UISprite>(), s => s.color = new Color(1f, 1f, 1f, 90f / 255f));
        if (Convert.ToInt64(string.IsNullOrEmpty(lbChipBet.text) ? "0" : lbChipBet.text) >= parent.maxChipAllow)
        {
            btnPlus.gameObject.collider.enabled = false;
            Array.ForEach<UISprite>(btnPlus.GetComponentsInChildren<UISprite>(), s => s.color = new Color(1f, 1f, 1f, 90f / 255f));
        }
        btnSubmit.gameObject.transform.collider.enabled = false;
        if (btnSubmit.gameObject.GetComponentInChildren<UISprite>() != null)
        {
            btnSubmit.gameObject.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 90f / 255f);
        }
    }
    internal void ShowIndividualCard(Electrotank.Electroserver5.Api.EsObject eso)
    {
        esObj = eso;
        DestroyCard();
        if (eso.variableExists("cards"))
        {
            int[] cardIds = eso.getIntegerArray("cards");
            for (int i = 0; i < cardIds.Length; i++)
            {
                if (parent.model != null)
                    if (parent.model.CardId == cardIds[i])
                    {
                        FACardBettingView.Create(cardIds[i], tableCardFA.transform, gameObject.GetComponent<PanelBetting>()).gameObject.SetActive(false);
                        continue;
                    }
                FACardBettingView cars = FACardBettingView.Create(cardIds[i], tableCardFA.transform, gameObject.GetComponent<PanelBetting>());
            }
            tableCardFA.Reposition();
            GameManager.Instance.FunctionDelay(delegate() { tableCardFA.transform.parent.localPosition = Vector3.zero; }, 0.01f);
        }

        GameManager.Instance.FunctionDelay(delegate()
        {
            if (parent.model != null)
            {
                if (parent.model.CardId != -1)
                {
                    ChanCard card = new ChanCard();
                    card.CardId = parent.model.CardId;
                    card.parent = cardParent;
                    card.Instantiate();
                    card.cardTexture.texture.width= CARD_WIDTH;
                    card.cardTexture.texture.height = CARD_HIGHT;
                    GameObject.Destroy(card.gameObject.GetComponent<BoxCollider>());
                    lbCardBet.text = card.ToString();
                    cardId = parent.model.CardId;
                }
                if (parent.model.ChipBetting == 0)
                    lbChipBet.text = ((LobbyChan)GameManager.Instance.selectedLobby).betting.ToString();
                else
                    lbChipBet.text = parent.model.ChipBetting.ToString();
                HideAllButton();
            }
        }, 0.15f);




    }
    public void DestroyCard()
    {
        for (int i = 0; i < tableCardFA.transform.childCount; i++)
        {
            GameObject.Destroy(tableCardFA.transform.GetChild(i).gameObject);
        }
        tableCardFA.Reposition();
    }
    void OnActivateRong()
    {
        if (parent.model.ETypeLaying != ETypeLayingBetting.None)
        {
            if (!cbLarge.value)
            {
                btnSubmit.gameObject.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 90f / 255f);
                btnSubmit.gameObject.transform.collider.enabled = false;
            }
            else
            {
                btnSubmit.gameObject.GetComponentInChildren<UISprite>().color = Color.white;
                btnSubmit.gameObject.transform.collider.enabled = true;
            }
        }
    }
    void OnActivateHep()
    {
        if (parent.model.ETypeLaying != ETypeLayingBetting.None)
        {
            if (!cbSmall.value)
            {
                btnSubmit.gameObject.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 90f / 255f);
                btnSubmit.gameObject.transform.collider.enabled = false;
            }
            else
            {
                btnSubmit.gameObject.GetComponentInChildren<UISprite>().color = Color.white;
                btnSubmit.gameObject.transform.collider.enabled = true;
            }
        }
    }
}

