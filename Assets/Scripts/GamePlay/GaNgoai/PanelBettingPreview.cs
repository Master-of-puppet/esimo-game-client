using UnityEngine;
using System.Collections;
using Electrotank.Electroserver5.Api;
using System;
using System.Collections.Generic;

public class PanelBettingPreview : MonoBehaviour
{
    #region Unity Editor
    public CUIHandle btnBack, btnPrevious, btnNext;
    public UIGrid tableUser;
    public BettingView parent;
    #endregion
    int next = -1;
    int previous = -1;
    string winner = "";
   

    [HideInInspector]
    public List<PlayerBettingView> listBettingPlayer = new List<PlayerBettingView>();
    void Awake()
    {
        CUIHandle.AddClick(btnBack, OnClickListenerBack);
        CUIHandle.AddClick(btnPrevious, OnClickListenerPrevious);
        CUIHandle.AddClick(btnNext, OnClickListenerNext);
    }


    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnBack, OnClickListenerBack);
        CUIHandle.RemoveClick(btnPrevious, OnClickListenerPrevious);
        CUIHandle.RemoveClick(btnNext, OnClickListenerNext);
    }

    private void OnClickListenerBack(GameObject targetObject)
    {
        DestroyUser();
        parent.ShowPanelJoin();
        //SetCenterUITable(tableUser);
        
    }

    private void OnClickListenerPrevious(GameObject targetObject)
    {
        DestroyUser();
        //SetCenterUITable(tableUser);
        GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("getLogGaNgoai", new object[] { "id", previous }));

    }

    private void OnClickListenerNext(GameObject targetObject)
    {
        DestroyUser();
        //SetCenterUITable(tableUser);
        GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("getLogGaNgoai", new object[] { "id", next }));

    }

    internal void ShowPreview(EsObject eso)
    {
        if (!eso.variableExists("next"))
            next = -1;
        else
            next = eso.getInteger("next");

        if (!eso.variableExists("prev"))
            previous = -1;
        else
            previous = eso.getInteger("prev");

        if (eso.variableExists("winner"))
        {
            winner = eso.getString("winner");
        }
        if (eso.variableExists("bettings"))
            InitUser(eso.getEsObjectArray("bettings"));
        ShowOrHideButtonNext();
        ShowOrHideButtonPrev();
    }
    public void ShowOrHideButtonNext()
    {
        btnNext.gameObject.collider.enabled = next != -1;
        if (next == -1)
            Array.ForEach<UISprite>(btnNext.GetComponentsInChildren<UISprite>(), s => s.color = new Color(1f, 1f, 1f, 90f / 255f));
        else
            Array.ForEach<UISprite>(btnNext.GetComponentsInChildren<UISprite>(), s => s.color = new Color(1f, 1f, 1f, 255f / 255f));
    }
    public void ShowOrHideButtonPrev()
    {
        btnPrevious.gameObject.collider.enabled = previous != -1;
        if (previous == -1)
            Array.ForEach<UISprite>(btnPrevious.GetComponentsInChildren<UISprite>(), s => s.color = new Color(1f, 1f, 1f, 90f / 255f));
        else
            Array.ForEach<UISprite>(btnPrevious.GetComponentsInChildren<UISprite>(), s => s.color = new Color(1f, 1f, 1f, 255f / 255f));
    }
    public void InitUser(EsObject[] esoArr)
    {
        DestroyUser();
        for (int i = 0; i < esoArr.Length; i++)
        {
            EsObject eso = esoArr[i];
            PlayerBettingModel model = new PlayerBettingModel();;
            model.Player = GameModelChan.GetPlayer(eso.getString("userName"));
            if (model == null)
            {
                model = new PlayerBettingModel();
                model.Player.username = eso.getString("userName");
            }
            model.CardId = eso.getInteger("cardId");
            if (winner == eso.getString("userName"))
                model.IsWinner = true;
            else
                model.IsWinner = false;
            model.ETypeLaying = (ETypeLayingBetting)eso.getInteger("gaNgoaiType");
            model.ChipBetting = eso.getLong("value");
            PlayerBettingView bettingView = PlayerBettingView.Create(model, tableUser.transform);
            listBettingPlayer.Add(bettingView);
            if (eso.getString("userName") == GameManager.Instance.mInfo.username)
            {
                bettingView.gameObject.name = "__0";
            }
        }
        tableUser.repositionNow = true;
       // tableUser.Reposition();
        SetCenterUITable(tableUser);
        
        GameManager.Instance.FunctionDelay(delegate() {
            foreach (PlayerBettingView pv in listBettingPlayer)
            {
                if (pv.model.IsWinner != null)
                {
                    if (pv.model.IsWinner == true)
                    {
                        pv.iconChicken.gameObject.SetActive(true);
                    }
                    else
                    {
                        pv.lbMoney.gameObject.GetComponent<UILabel>().color = new Color(1f, 155f / 255f, 0f);
                        ECardTexture texture1 = pv.gameObject.GetComponentInChildren<ECardTexture>();
                        texture1.card.SetColor(new Color(1f, 1f, 1f, 90f / 255f));
                    }
                }
            }

        }, 0.1f);
       
    }
    public void SetCenterUITable(UIGrid table)
    {
        
        float widthTable = tableUser.transform.childCount * tableUser.cellWidth;
        table.transform.localPosition = new Vector3(-(widthTable / 2 - (table.transform.childCount * 22f)), table.transform.localPosition.y, table.transform.localPosition.z);
        //Debug.LogWarning("widthTable: " + widthTable);
        //Debug.LogWarning("tableUser.transform.childCount: "+ tableUser.transform.childCount);
        //Debug.LogWarning("tableUser.cellWidth: " + tableUser.cellWidth);
    }
    public void DestroyUser()
    {
        
        while (listBettingPlayer.Count > 0)
        {
            GameObject.Destroy(listBettingPlayer[0].gameObject);
            listBettingPlayer.RemoveAt(0);
        }

        tableUser.repositionNow = true;
        //tableUser.Reposition();
        tableUser.transform.parent = tableUser.transform;
        tableUser.transform.localPosition = Vector3.zero;
        tableUser.transform.localScale = Vector3.one;
    }
}

