using System;
using UnityEngine;
using System.Collections.Generic;

public class ServerMessagesView : MonoBehaviour
{
    #region Unity Editor
    public Transform panelServer;
    #endregion

    [HideInInspector]
    public List<NotificationView.Notification> listMessages = new List<NotificationView.Notification>();

    CallBackFunction callbackFuntionTryAgainDialog;
    public CUIHandle buttonClose, buttonYes, buttonNo;
    UIInput txtContent;
    UILabel lbContent;
    UILabel lbTitle;


    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickClose;
        CUIHandle.AddClick(buttonClose, OnClickClose);
        CUIHandle.AddClick(buttonYes, OnClickClose);
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(buttonClose, OnClickClose);
        CUIHandle.RemoveClick(buttonYes, OnClickClose);
    }

    void OnClickClose(GameObject go)
    {
        buttonClose.StopImpact(0.5f);

        NextMessage();
    }

    public static ServerMessagesView Instance
    {
        get
        {
            GameObject go = GameObject.Find("__Messages From Server");
            if (go == null)
            {
                go = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/ServerMessagePrefab"));
                GameObject.DontDestroyOnLoad(go);
                go.name = "__Messages From Server";
                go.transform.localPosition = new Vector3(-85f, 501f, 111f);
            }
            return go.GetComponent<ServerMessagesView>();
        }
    }

    void Init(NotificationView.EType _type)
    {
        Transform trans = panelServer;
        
        lbContent = trans.Find("1.3 LableContent").GetComponent<UILabel>();

        //buttonClose = trans.FindChild("PopupBackground").GetComponentInChildren<CUIHandle>();
        lbTitle = trans.FindChild("1.2 LableTitle").GetComponent<UILabel>();

        //buttonYes = trans.Find("1.1 ButtonClose").GetComponent<CUIHandle>();
        CUIHandle.AddClick(buttonClose, OnClickClose);
        CUIHandle.AddClick(buttonYes, delegate(GameObject go)
         {
                if(callbackFuntionTryAgainDialog != null)
                    callbackFuntionTryAgainDialog();
                OnClickClose(go);
         });
    }

    public void hideNotification()
    {
        buttonClose.StopImpact(0.5f);
        NextMessage();
    }
    void NextMessage()
    {
		if(Instance.listMessages.Count > 0)
			Instance.listMessages.RemoveAt(0);
       
        CUIHandle.RemoveClick(buttonClose, OnClickClose);
        if (Instance.listMessages.Count > 0)
        {

            NotificationView.Notification next = Instance.listMessages[0];
            if (next.type == NotificationView.EType.Server)
                _StartMessageServer(next.message);
            else NextMessage();
        }
        else
            GameObject.Destroy(gameObject);
    }

	void _StartMessageServer(string message)
	{
		Init(NotificationView.EType.Server);
		lbTitle.text = "Thông báo từ máy chủ";
		lbContent.text = message.Replace("\n", "\\n");
	}
    void _StartMessageServer(string title, string message, float time)
	{
		Init(NotificationView.EType.Server);
		lbTitle.text = title;
		lbContent.text = message.Replace("\n", "\\n");
		if (time > 0f)
			StartCoroutine (FadeTo (panelServer.GetComponent<UIPanel> (), time));
	}

    void _StartMessageServer(string title, string message, CallBackFunction delegateTryAgain = null, string buttonTitle = "ĐÓNG" , float time = 0)
    {
        Init(NotificationView.EType.Server);
		lbTitle.text = title;
        lbContent.text = message.Replace("\n", "\\n");
        buttonYes.gameObject.GetComponentInChildren<UILabel>().text = buttonTitle;
        buttonClose.transform.parent.GetComponent<UISliceBackgroundPopup>().IsShowBtnClose = false;
		buttonClose.transform.parent.GetComponent<UISliceBackgroundPopup>().Resize();
        callbackFuntionTryAgainDialog = delegateTryAgain;
        if (time > 0f)
            StartCoroutine(FadeTo(panelServer.GetComponent<UIPanel>(), time));
        Instance.listMessages.Add(new NotificationView.Notification(message));
    }
    public static void MessageServer(string title, string message, float time)
    {
        if (Instance.listMessages.Count == 0)
            Instance._StartMessageServer(title, message, time);

        Instance.listMessages.Add(new NotificationView.Notification(message));
    }
    public static void MessageServer(string title, string message, CallBackFunction delegateTryAgain = null, string buttonTitle = "ĐÓNG", float time = 0)
    {
        if (Instance.listMessages.Count == 0)
            Instance._StartMessageServer(title, message,delegateTryAgain,buttonTitle, time);

        Instance.listMessages.Add(new NotificationView.Notification(message));
    }
	public static void MessageServer(string message)
	{
		if (Instance.listMessages.Count == 0)
			Instance._StartMessageServer(message);
		
		Instance.listMessages.Add(new NotificationView.Notification(message));
        WaitingView.Instance.Close();
	}
	/// <summary>
	/// Ẩn dần thông báo
	/// </summary>
	/// <param name="panel"></param>
	/// <param name="time"></param>
	/// <returns></returns>
	System.Collections.IEnumerator FadeTo(UIPanel panel, float time)
	{
		yield return new WaitForSeconds(time * 3 / 4f);
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / (time / 4))
		{
			panel.alpha = Mathf.Lerp(1f, 0f, t);
			yield return null;
		}
		OnClickClose(gameObject);
	}
}
