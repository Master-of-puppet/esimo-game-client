using System;
using System.Collections;
using UnityEngine;

public class SupportView : MonoBehaviour
{
    #region Unity Editor
    public CUIHandle btClose, btSendFeedback;
    public UIGrid tableParent;
	public UILabel lbSkype, lbYahoo , lbPhone;
    public UIInput txtTitle, txtContent;
    public UIToggle CBSendLog;
    #endregion

    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickBack;
        CUIHandle.AddClick(btClose, OnClickBack);
        CUIHandle.AddClick(btSendFeedback, OnClickSendFeedBack);
        GameManager.Server.EventConfigClientChanged += ConfigClientHandler;
        GameManager.Server.EventHelpChanged += EventHelpHandler;
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(btClose, OnClickBack);
        CUIHandle.RemoveClick(btSendFeedback, OnClickSendFeedBack);
        GameManager.Server.EventConfigClientChanged -= ConfigClientHandler;
        GameManager.Server.EventHelpChanged -= EventHelpHandler;
    }


    void OnClickBack(GameObject go)
    {
        GameObject.Destroy(gameObject);
    }
    private void ConfigClientHandler(IDictionary value)
    {
        InitSupport();
    }

    public static SupportView Create()
    {
        GameObject setting = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/SupportPrefab"));
        setting.name = "__SupportPrefab";
        setting.transform.position = new Vector3(-1301, 240, -128f);
        //Transform obj = HeaderMenu.Instance.transform.FindChild("Camera");
        //obj.camera.depth = 5;

        return setting.GetComponent<SupportView>();
    }
    private void EventHelpHandler(IDictionary value)
    {
        InitListTutorial();
    }
    public void InitListTutorial()
    {
        while (LineTutorialView.ListTutorialView.Count > 0)
        {
            if(LineTutorialView.ListTutorialView[0] != null )
            {
                GameObject.Destroy(LineTutorialView.ListTutorialView[0].gameObject);
            }
            LineTutorialView.ListTutorialView.RemoveAt(0);
        }
        foreach (Hashtable hash in GameManager.Instance.ListHelp)
            LineTutorialView.Create(hash, tableParent.transform);

        tableParent.repositionNow = true;
    }
    public void InitSupport()
    {
        foreach (PlatformConfig config in GameManager.Setting.Platform.Configs)
        {
            switch (config.Type)
            {
                case PlatformType.support_phone:
					lbPhone.text = config.Value;
                    break;
                case PlatformType.support_skype:
                    lbSkype.text = config.Value;
                    break;
                case PlatformType.support_yahoo:
                    lbYahoo.text = config.Value;
                    break;
            }
        }
    }
    public void InitData()
    {
        InitListTutorial();
        InitSupport();
    }
    void Start()
    {
        CBSendLog.value = false;
        InitData();
    }

	bool isFeedBackClicked = false;
    void OnClickSendFeedBack(GameObject go)
    {
        if (isFeedBackClicked)
            return;

        if (!Utility.Input.IsStringValid(txtContent.value, 10, 999999))
        {
            NotificationView.ShowMessage("Nội dung bạn gửi quá ngắn.\n\nHãy nhập một lời góp ý trên 10 ký tự", 3f);
            return;
        }

        ServerWeb.StartThread(ServerWeb.URL_SEND_FEEDBACK,
            new object[] { "game_id", (int)GameManager.GAME, "user_id", GameManager.Instance.mInfo.id, "title", txtTitle.value, "content", txtContent.value },
            CallBackResponse);
        if (CBSendLog.value == true)
        {
            //LogViewer.SaveLogToFile();//for test
            LogViewer.SendLogToServer();
            CBSendLog.value = false;
            //Debug.LogWarning("Đã gửi debug_log");
        }
        isFeedBackClicked = true;

    }

    void CallBackResponse(bool isDone, WWW response, IDictionary json)
    {
        if (isDone && json["code"].ToString() == "1")
        {
			isFeedBackClicked = false;
            NotificationView.ShowMessage("Chúng tôi đã nhận được phản hồi của bạn.", 3f);
            txtTitle.value = "";
            txtContent.value = "";
        }
    }
}
