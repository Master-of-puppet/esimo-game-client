using UnityEngine;
using System.Collections;

public class BettingView : MonoBehaviour
{
    #region Unity Editor
    public PanelBettingJoin panelJoin;
    public PanelBettingPreview panelPreview;
    public PanelBetting panelBetting;
    public CUIHandle btnClose;
    #endregion
    [HideInInspector]
    public PlayerBettingModel model = null;
    public long maxChipAllow = 0;
    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickListenerClose;
        CUIHandle.AddClick(btnClose, OnClickListenerClose);
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnClose, OnClickListenerClose);
    }
    public static BettingView Instance
    {
        get
        {
            GameObject go = GameObject.Find("__BettingView");
            if (go == null)
            {
                go = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/TopMenu/GameplayBetting"));
                go.name = "__BettingView";
                go.transform.localPosition = new Vector3(100, 240, -128f);
            }
            return go.GetComponent<BettingView>();
        }
    }
    public void OnBettingValueHandler(Electrotank.Electroserver5.Api.EsObject eso)
    {
        panelJoin.OnBettingValueChange(eso);
    }
    internal void OnShowIndividualCard(Electrotank.Electroserver5.Api.EsObject eso)
    {
		ShowPanelBetting();
        panelBetting.ShowIndividualCard(eso);
    }
    internal void OnShowPreViewBetting(Electrotank.Electroserver5.Api.EsObject eso)
    {
        if (!eso.variableExists("bettings"))
        {
            NotificationView.ShowMessage("Chưa có lịch sử chơi gà ngoài", 3f);
            return;
        }
        ShowPanelPreview();
        panelPreview.ShowPreview(eso);
    }


    public void ShowPanelPreview()
    {
        panelJoin.gameObject.SetActive(false);
        panelPreview.gameObject.SetActive(true);
        panelBetting.gameObject.SetActive(false);
    }
    public void ShowPanelJoin()
    {
        panelJoin.gameObject.SetActive(true);
        panelPreview.gameObject.SetActive(false);
        panelBetting.gameObject.SetActive(false);
    }
    public void ShowPanelBetting()
    {
        panelJoin.gameObject.SetActive(false);
        panelPreview.gameObject.SetActive(false);
        panelBetting.gameObject.SetActive(true);
    }
    void OnClickListenerClose(GameObject targetObject)
    {
        GameObject.Destroy(gameObject);
    }

    internal void InitBetting(Electrotank.Electroserver5.Api.EsObject eso)
    {
        if(panelBetting.gameObject.activeSelf)
            panelBetting.HideAllButton();
        ShowPanelJoin();
		foreach (PlayerBettingView pv in panelJoin.listBettingPlayer)
            if (pv.iconChange.gameObject.activeSelf)
				pv.iconChange.gameObject.SetActive(false);
        PlayerBettingView view = panelJoin.listBettingPlayer.Find(lbv => lbv.model.Player.username == eso.getString("userName"));
        ECardTexture texture = view.GetComponentInChildren<ECardTexture>();
        view.model.CardId = eso.getInteger("cardId");
        view.model.ETypeLaying = (ETypeLayingBetting)eso.getInteger("gaNgoaiType");
        view.model.ChipBetting = eso.getLong("value");
        view.SetData(view.model);
        if (GameManager.Instance.mInfo.username == eso.getString("userName"))
            panelJoin.ShowButonJoin(model);
        if (eso.getString("userName") != GameManager.Instance.mInfo.username)
            view.iconChange.gameObject.SetActive(true);
    }
}

