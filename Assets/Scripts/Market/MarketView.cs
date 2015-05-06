using UnityEngine;
using System.Collections;

public class MarketView : MonoBehaviour
{

    #region Unity Editor
    public CUIHandle btnClose;
    public UniWebView webView;
    public UILabel lblTitle;

    //Money
    public UILabel chip, gold;
    #endregion

#if UNITY_ANDROID || UNITY_IPHONE || UNITY_EDITOR 
    void Start () {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickButtonClose;
        CUIHandle.AddClick(btnClose, OnClickButtonClose);

        webView.OnReceivedMessage += OnReceivedMessage;
        webView.OnLoadComplete += OnLoadComplete;
        webView.OnWebViewShouldClose += OnWebViewShouldClose;
        webView.OnEvalJavaScriptFinished += OnEvalJavaScriptFinished;
        GameManager.Server.EventUpdateUserInfo += GetDataUserInfo;

        SetWebView();
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void GetDataUserInfo()
    {      
        chip.text = Utility.Convert.Chip(GameManager.Instance.mInfo.chip);
        gold.text = Utility.Convert.Chip(GameManager.Instance.mInfo.gold);
        Debug.LogWarning("gold = " + GameManager.Instance.mInfo.gold);

    }
    public static MarketView Create(){
        GameObject gobj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Market/MarketPrefab"));
        gobj.name = "__MarketPrefab";
        gobj.transform.localPosition = new Vector3(-1602, -1991, -2014);
        gobj.GetComponent<MarketView>().Load();
        return gobj.GetComponent<MarketView>();
    }

    public void Load(string url = "")
    {
        string accessToken = !string.IsNullOrEmpty(GameManager.Instance.mInfo.accessToken) ? GameManager.Instance.mInfo.accessToken : GameManager.Instance.accessToken;
        if (string.IsNullOrEmpty(url))
            webView.url = ServerWeb.URL_REQUEST_MARKET + "?accessToken=" + accessToken;
        else
            webView.url = url;
        webView.Load();
        
        GetDataUserInfo();
    }
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            webView.Hide();
            OnClickButtonClose(null);
        }
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnClose, OnClickButtonClose);
        webView.OnReceivedMessage -= OnReceivedMessage;
        webView.OnLoadComplete -= OnLoadComplete;
        webView.OnWebViewShouldClose -= OnWebViewShouldClose;
        webView.OnEvalJavaScriptFinished -= OnEvalJavaScriptFinished;
        GameManager.Server.EventUpdateUserInfo -= GetDataUserInfo;
    }
    private void SetWebView()
    {
        int uiFactor = UniWebViewHelper.RunningOnRetinaIOS() ? 2 : 1;
        UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);
        float ratio = ((float)mRoot.activeHeight / Screen.height) * uiFactor;
        int width = Mathf.FloorToInt(Screen.width * ratio / uiFactor);
        int height = Mathf.FloorToInt(Screen.height * ratio / uiFactor);

        UISliceBackgroundPopup backgroundPopup = gameObject.GetComponentInChildren<UISliceBackgroundPopup>();
        int webMarginWidth = Mathf.FloorToInt(width - (backgroundPopup.PopupWidth - 25f));
        int webMarginHeight = Mathf.FloorToInt(height - (backgroundPopup.PopupHeight - 24f));

        int leftRight = Mathf.FloorToInt(webMarginWidth / (2 * ratio));

        int top = Mathf.RoundToInt(((webMarginHeight / 2) + backgroundPopup.buttonClose.GetComponentInChildren<UISprite>().height) / ratio);
        int bottom = Mathf.RoundToInt(webMarginHeight / (2 * ratio));
        webView.insets = new UniWebViewEdgeInsets(top+1, leftRight, bottom, leftRight);

    }

    void OnClickButtonClose(GameObject targetObject)
    {
        GameObject.Destroy(gameObject);
    }

    void OnReceivedMessage(UniWebView webView, UniWebViewMessage message)
    {
        throw new System.NotImplementedException();
    }

    void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
    {
        if (success)
        {
            webView.Show();
        }
        else
        {
            Debug.Log("Something wrong in webview loading: " + errorMessage);
            //_errorMessage = errorMessage;
        }
    }

    bool OnWebViewShouldClose(UniWebView webView)
    {
        if (this.webView == webView)
        {
            this.webView = null;
            OnClickButtonClose(null);
            return true;
        }
        return false;
    }

    void OnEvalJavaScriptFinished (UniWebView webView, string result)
	{
		throw new System.NotImplementedException ();
    }
#endif
}
