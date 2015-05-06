using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;
using System;

public class PanelBettingJoin : MonoBehaviour
{
    #region Unity Editor
    public UIGrid tableUser;
    public CUIHandle btnChangeBet, btnJoinBet, btnPreview;
    public BettingView parent;
    #endregion
    [HideInInspector]
    public List<PlayerBettingView> listBettingPlayer = new List<PlayerBettingView>();
    void Awake()
    {
        CUIHandle.AddClick(btnChangeBet, OnClickListenerJoinBet);
        CUIHandle.AddClick(btnJoinBet, OnClickListenerJoinBet);
        CUIHandle.AddClick(btnPreview, OnClickListenerPreview);
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnChangeBet, OnClickListenerJoinBet);
        CUIHandle.RemoveClick(btnJoinBet, OnClickListenerJoinBet);
        CUIHandle.RemoveClick(btnPreview, OnClickListenerPreview);
    }
    public void OnBettingValueChange(EsObject eso)
    {
        if (eso.variableExists("valid"))
        {
            btnJoinBet.gameObject.collider.enabled = eso.getBoolean("valid");
            btnChangeBet.gameObject.collider.enabled = eso.getBoolean("valid");
            if (!eso.getBoolean("valid"))
            {
                btnJoinBet.gameObject.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 90f / 255f);
                btnChangeBet.gameObject.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 90f / 255f);
            }
            else
            {
                btnJoinBet.gameObject.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
                btnChangeBet.gameObject.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
            }
        }
        if (eso.variableExists("bettings"))
            InitUser(eso.getEsObjectArray("bettings"));
        if (eso.variableExists("maxChipAllow"))
            parent.maxChipAllow = eso.getLong("maxChipAllow");
    }

    void OnClickListenerJoinBet(GameObject targetObject)
    {
        if (GameModelChan.CurrentState > GameModelChan.EGameState.dealClient)
            GameManager.Server.DoRequestGameAction("getIndividualCards");
        else
            NotificationView.ShowMessage("Ván bài chưa bắt đầu",3f);
    }
    void OnClickListenerPreview(GameObject targetObject)
    {
        GameManager.Server.DoRequestGameCommand("getLogGaNgoai");
    }
    internal void ShowButonJoin(PlayerBettingModel model)
    {
        if (GameModelChan.CurrentState >= GameModelChan.EGameState.playing)
        {
            if (GameModelChan.CurrentState == GameModelChan.EGameState.playerFullLaying)
                HideBothButton();       
            else
                if (model.ETypeLaying != ETypeLayingBetting.None)
                    HideJoin();
                else
                    HideChange();
        }
        else
            HideBothButton();
    }

    private void HideChange()
    {
        btnJoinBet.gameObject.SetActive(true);
        btnChangeBet.gameObject.SetActive(false);
        if (!string.IsNullOrEmpty(GameModelChan.YourController.warningMessage))
        {
            btnJoinBet.collider.enabled = false;
            btnJoinBet.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 200 / 255f);
        }
    }

    private void HideJoin()
    {
        btnJoinBet.gameObject.SetActive(false);
        btnChangeBet.gameObject.SetActive(true);
        if (!string.IsNullOrEmpty(GameModelChan.YourController.warningMessage))
        {
            btnChangeBet.collider.enabled = false;
            btnChangeBet.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 200 / 255f);
        }
    }

    private void HideBothButton()
    {
        btnJoinBet.gameObject.SetActive(false);
        btnChangeBet.gameObject.SetActive(false);
    }
    public void InitUser(EsObject[] esoArr)
    {
        while (listBettingPlayer.Count > 0)
        {
            GameObject.Destroy(listBettingPlayer[0].gameObject);
            listBettingPlayer.RemoveAt(0);
        }
        for (int i = 0; i < esoArr.Length; i++)
        {
            EsObject eso = esoArr[i];
            PlayerBettingModel model = new PlayerBettingModel();

            model.Player = GameModelChan.GetPlayer(eso.getString("userName"));
            if (model.Player == null)
            {
                model.Player = new PlayerControllerChan();
                model.Player.username = eso.getString("userName");
            }
            if (!GameModelChan.game.dicUserBetting.ContainsKey(eso.getString("userName")))
                GameModelChan.game.dicUserBetting.Add(eso.getString("userName"), false);
            model.CardId = eso.getInteger("cardId");
            model.ETypeLaying = (ETypeLayingBetting)eso.getInteger("gaNgoaiType");
            model.ChipBetting = eso.getLong("value");
            PlayerBettingView bettingView = PlayerBettingView.Create(model, tableUser.transform);
            listBettingPlayer.Add(bettingView);
            if (eso.getString("userName") == GameManager.Instance.mInfo.username)
            {
                parent.model = model;
                ShowButonJoin(model);
                bettingView.gameObject.name = "__0";
            }
        }
        tableUser.repositionNow = true;
        SetCenterUITable(tableUser);
        if (Array.Find<EsObject>(esoArr, eso => eso.getString("userName") == GameManager.Instance.mInfo.username) == null)
        {
            HideBothButton();
        }
        GameManager.Instance.FunctionDelay(delegate()
        {
            foreach (PlayerBettingView view in listBettingPlayer)
            {
                if (GameModelChan.game.dicUserBetting.ContainsKey(view.model.Player.username) && GameModelChan.game.dicUserBetting[view.model.Player.username])
                {
                    view.iconChange.gameObject.SetActive(true);
                    GameModelChan.game.dicUserBetting[view.model.Player.username] = false;
                }
            }

        }, 0.01f);
    }
    public void SetCenterUITable(UIGrid table)
    {
        float widthTable = table.transform.childCount * table.cellWidth;
        table.transform.localPosition = new Vector3(- (widthTable / 2 - (table.transform.childCount * 20f) ), table.transform.localPosition.y, table.transform.localPosition.z);
    }
    public void DestroyUser()
    {
        for (int i = 0; i < tableUser.transform.childCount; i++)
        {
            GameObject.Destroy(tableUser.transform.GetChild(i).gameObject);
        }
        tableUser.repositionNow = true;
    }
}

