using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Class view các thông báo trong game
/// </summary>
public class NotificationView : MonoBehaviour
{
    public delegate void OnClickDialogYes(string inputDialog);
    public delegate void HideMessageBox();

    public enum EType
    {
        Server,
        Message,
        Dialog,
        Confirm
    }

    #region Unity Editor
    public Transform panelMessage;
    public Transform panelDialog;
    public Transform panelConfirm;
    #endregion

    OnClickDialogYes callbackFuntionYesDialog;
    CallBackFunction callbackFuntionNoConfirm;
    CallBackFunction callbackFuntionYesConfirm;
    HideMessageBox callbackFuntionHideMessage;

    CUIHandle buttonClose, buttonYes, buttonNo;
    UIInput txtContent;
    UILabel lbContent;
	UILabel lbTitle;
    public static bool isHide = true;
    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickClose;
    }

    void OnDestroy()
    {
        if(buttonClose !=null)
            CUIHandle.RemoveClick(buttonClose, OnClickClose);
    }

    public void OnBackgroundClick()
    {
        OnClickClose(gameObject);
    }

    void OnClickClose(GameObject go)
    {
        buttonClose.StopImpact(0.5f);
        NextNotification();
        isHide = true;
        if (callbackFuntionHideMessage != null)
            callbackFuntionHideMessage();

        buttonClose.gameObject.SetActive(true);
    }
    public void hideNotification()
    {
        buttonClose.StopImpact(0.5f);
        NextNotification();
    }
    public static NotificationView Instance
    {
        get
        {
            GameObject go = GameObject.Find("__Notification");
            if (go == null)
            {
                go = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/NotificationPrefab"));            
                go.name = "__Notification";
                go.transform.localPosition = new Vector3(210f, 453f, 127f);
                GameObject.DontDestroyOnLoad(go);
            }
            return go.GetComponent<NotificationView>();
        }
    }
    public void HideMessageView() 
    {
        if (Instance.gameObject!=null)
            GameObject.Destroy(Instance.gameObject);
    }
    void Init(EType _type)
    {
        Transform trans = panelMessage;
        if (_type == EType.Message)
        {
            trans = panelMessage;

            panelMessage.gameObject.SetActive(true);
            panelDialog.gameObject.SetActive(false);
            panelConfirm.gameObject.SetActive(false);

            lbContent = trans.Find("1.3 LableContent").GetComponent<UILabel>();
			lbContent.supportEncoding = true;
        }
        else if (_type == EType.Confirm)
        {
            trans = panelConfirm;

            panelMessage.gameObject.SetActive(false);
            panelDialog.gameObject.SetActive(false);
            panelConfirm.gameObject.SetActive(true);

            lbContent = trans.Find("1.3 LableContent").GetComponent<UILabel>();

            buttonYes = trans.Find("1.4 Button Yes").GetComponent<CUIHandle>();
            buttonNo = trans.Find("1.5 Button No").GetComponent<CUIHandle>();

            CUIHandle.AddClick(buttonYes, delegate(GameObject go) {
                if(callbackFuntionYesConfirm != null)
                    callbackFuntionYesConfirm();
                OnClickClose(go);
            });
            CUIHandle.AddClick(buttonNo, delegate(GameObject go)
            {
                if(callbackFuntionNoConfirm != null)
                    callbackFuntionNoConfirm();
                OnClickClose(go);
            });
        }
        else
        {
            trans = panelDialog;

            panelMessage.gameObject.SetActive(false);
            panelDialog.gameObject.SetActive(true);
            panelConfirm.gameObject.SetActive(false);

            txtContent = trans.FindChild("1.3 Input Text").GetComponent<UIInput>();
            buttonYes = trans.FindChild("1.4 Button OK").GetComponent<CUIHandle>();

            CUIHandle.AddClick(buttonYes,  delegate(GameObject go) { 
                callbackFuntionYesDialog(txtContent.value);
                OnClickClose(go);
            });
        }

		buttonClose = trans.FindChild("Background").GetComponentInChildren<CUIHandle>();
        lbTitle = trans.FindChild("1.2 LableTitle").GetComponent<UILabel>();

        CUIHandle.AddClick(buttonClose, OnClickClose);
        trans.GetComponent<UIPanel>().alpha = 1f;
    }

    void NextNotification()
    {
        listNotification.RemoveAt(0);
        CUIHandle.RemoveClick(buttonClose, OnClickClose);
        if (listNotification.Count > 0)
        {
            Notification next = listNotification[0];
            if (next.type == EType.Message)
                _StartMessage(next.message, next.timeAutoClose);
            else if (next.type == EType.Dialog)
                _StartDialog(next.title, next.callbackFuntionYesDialog);
            else if (next.type == EType.Confirm)
                _StartConfirm(next.title, next.message, next.callbackFuntionYesConfirm, next.callbackFuntionNoConfirm, next.textButtonYes, next.textButtonNo);
            else NextNotification();
        }
        else
            GameObject.Destroy(gameObject);
    }
    #region Show Message
    void _StartMessage(string message, float time)
    {
        Init(EType.Message);
        lbTitle.text = "Thông báo";
		lbContent.text = message;
        
        if(time > 0f)
            StartCoroutine(FadeTo(panelMessage.GetComponent<UIPanel>(), time));
    }
    
    public static void ShowMessage(string message, float time)
    {
        if (Instance.listNotification.Count == 0)
            Instance._StartMessage(message, time);

        Instance.listNotification.Add(new Notification(message, time));
    }

    public static void ShowMessage(string message)
    {
        ShowMessage(message, 0f);
        isHide = false;
    }
    public static void ShowMessage(string message,HideMessageBox onClose)
    {
        Instance.ShowMessgeWithEvent(message, onClose);
    }
    public void ShowMessgeWithEvent(string message, HideMessageBox onClose) 
    {
        ShowMessage(message, 0f);
        isHide = false;
        callbackFuntionHideMessage = onClose;
    }
    #endregion

    #region Show Dialog
    void _StartDialog(string title, OnClickDialogYes onYes)
    {
        Init(EType.Dialog);
		lbTitle.text = title;
        callbackFuntionYesDialog = onYes;
    }

    public static void ShowDialog(string title, OnClickDialogYes onYes)
    {
        if (Instance.listNotification.Count == 0)
            Instance._StartDialog(title, onYes);

        Instance.listNotification.Add(new Notification(onYes, title));
    }
    #endregion

    #region Show Confirm Dialog
    void _StartConfirm(string title, string message, CallBackFunction onYes, CallBackFunction onNo, string textButtonYes, string textButtonNo)
    {
        Init(EType.Confirm);

		lbTitle.text = title;
        lbContent.text = message;

        callbackFuntionNoConfirm = onNo;
        callbackFuntionYesConfirm = onYes;

        if (string.IsNullOrEmpty(textButtonYes) == false)
            buttonYes.GetComponentInChildren<UILabel>().text = textButtonYes;
        else
            buttonYes.GetComponentInChildren<UILabel>().text = "ĐỒNG Ý";

        if (string.IsNullOrEmpty(textButtonNo) == false)
            buttonNo.GetComponentInChildren<UILabel>().text = textButtonNo;
        else
            buttonNo.GetComponentInChildren<UILabel>().text = "BỎ QUA";
    }

    public static void ShowConfirm(string title, string message, CallBackFunction onYes, CallBackFunction onNo)
    {
        ShowConfirm(title, message, onYes, onNo, null, null);
    }

    public static void ShowConfirm(string title, string message, CallBackFunction onYes, CallBackFunction onNo, string textButtonYes, string textButtonNo)
    {
        if (Instance.listNotification.Count == 0)
            Instance._StartConfirm(title, message, onYes, onNo, textButtonYes, textButtonNo);
        Instance.listNotification.Add(new Notification(title, message, onYes, onNo, textButtonYes, textButtonNo));
    }

    //----mustUpdate
    void _StartConfirmMustUpdate(string title, string message, CallBackFunction onYes, CallBackFunction onNo, string textButtonYes, string textButtonNo)
    {
        Init(EType.Confirm);

		lbTitle.text = title;
        lbContent.text = message;

        callbackFuntionNoConfirm = onNo;
        callbackFuntionYesConfirm = onYes;

        if (string.IsNullOrEmpty(textButtonYes) == false)
            buttonYes.GetComponentInChildren<UILabel>().text = textButtonYes;
        else
            buttonYes.GetComponentInChildren<UILabel>().text = Application.platform == RuntimePlatform.Android ? "Google Play" : Application.platform == RuntimePlatform.IPhonePlayer ? "App Store" : "App Store";

        if (string.IsNullOrEmpty(textButtonNo) == false)
            buttonNo.GetComponentInChildren<UILabel>().text = textButtonNo;
        else
            buttonNo.GetComponentInChildren<UILabel>().text = "Chieuvuong.com";
        buttonClose.gameObject.SetActive(false);
    }

    public static void ShowConfirmMustUpdate(string title, string message, CallBackFunction onYes, CallBackFunction onNo)
    {
        ShowConfirmMustUpdate(title, message, onYes, onNo, null, null);
    }

    public static void ShowConfirmMustUpdate(string title, string message, CallBackFunction onYes, CallBackFunction onNo, string textButtonYes, string textButtonNo)
    {
        if (Instance.listNotification.Count == 0)
            Instance._StartConfirmMustUpdate(title, message, onYes, onNo, textButtonYes, textButtonNo);

        Instance.listNotification.Add(new Notification(title, message, onYes, onNo, textButtonYes, textButtonNo));
    }
    #endregion

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

    /// <summary>
    /// Danh sách các thông báo đang chờ
    /// </summary>
    public List<Notification> listNotification = new List<Notification>();

    public class Notification
    {
        public string title, message;
        public EType type;
        public float timeAutoClose;

        /// <summary>
        /// Dialog Box
        /// </summary>
        public OnClickDialogYes callbackFuntionYesDialog;

        /// <summary>
        /// Confirm Box
        /// </summary>
        public CallBackFunction callbackFuntionYesConfirm;
        public CallBackFunction callbackFuntionNoConfirm;
        public string textButtonYes, textButtonNo;

        public Notification(string message)
        {
            this.type = EType.Server;
            this.message = message;
        }

        public Notification(string message, float timeAutoClose)
        {
            this.type = EType.Message;

            this.message = message;
            this.timeAutoClose = timeAutoClose;
        }

        public Notification(OnClickDialogYes dialogOk, string title)
        {
            this.type = EType.Dialog;

            this.title = title;
            this.callbackFuntionYesDialog = dialogOk;
        }

        public Notification(string title, string message, CallBackFunction onYes, CallBackFunction onNo, string textButtonYes, string textButtonNo)
        {
            this.type = EType.Confirm;

            this.title = title;
            this.message = message;
            this.callbackFuntionYesConfirm = onYes;
            this.callbackFuntionNoConfirm = onNo;
            this.textButtonYes = textButtonYes;
            this.textButtonNo = textButtonNo;
        }
        void Update()
        {
            
        }
    }
}
