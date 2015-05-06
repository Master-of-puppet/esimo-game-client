using UnityEngine;
using System.Collections;

public class EventView : MonoBehaviour
{

    #region Unity Editor
    public CUIHandle btnClose;
    public UniWebView webView;
    public UITable table;
    public UILabel lblTitle;
    #endregion

	#if UNITY_ANDROID || UNITY_IPHONE || UNITY_EDITOR 
    void Start()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickButtonClose;
        CUIHandle.AddClick(btnClose, OnClickButtonClose);

		webView.OnReceivedMessage += OnReceivedMessage;
		webView.OnLoadComplete += OnLoadComplete;
		webView.OnWebViewShouldClose += OnWebViewShouldClose;
		webView.OnEvalJavaScriptFinished += OnEvalJavaScriptFinished;

        SetWebView();
        //webView.url = ServerWeb.URL_REQUEST_EVENT;
        //webView.Load ();
        //ServerWeb.StartThread(ServerWeb.URL_REQUEST_EVENT, ResponseHttpHandler);
    }

    private void ResponseHttpHandler(bool isDone, WWW response, IDictionary json)
    {
		EventItemView.List.Clear ();
		if (isDone && json["code"].ToString() == "0")
        {
            ArrayList array = (ArrayList)json["items"];
            foreach (Hashtable obj in array)
            {
				EventModel model = new EventModel(obj);
				EventItemView.Create(model,table.transform,this);
            }
			OnChooseEvent(EventItemView.List[0].model);
			//table.Reposition();
        }
    }
    public static EventView Create()
    {
        GameObject gobj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Event/EventView"));
        gobj.name = "__EventView";
        gobj.transform.localPosition = new Vector3(-1602, -1991, -2014);
        gobj.GetComponent<EventView>().Load();
        return gobj.GetComponent<EventView>();
    }

    public static EventView Create(string title,string url)
    {
        EventView view = Create();
        view.lblTitle.text = title;
        view.Load(url);
        return view;
    }

    public void Load(string url ="")
    {
        if (string.IsNullOrEmpty(url))
            webView.url = ServerWeb.URL_REQUEST_EVENT;
        else
            webView.url = url;
        webView.Load();
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
    }
    private void SetWebView() 
    {
		int uiFactor = UniWebViewHelper.RunningOnRetinaIOS() ? 2:1;
		UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);
		float ratio = ((float) mRoot.activeHeight / Screen.height) * uiFactor;
		int width = Mathf.FloorToInt(Screen.width * ratio / uiFactor);
		int height = Mathf.FloorToInt(Screen.height * ratio / uiFactor);
	
        UISliceBackgroundPopup backgroundPopup = gameObject.GetComponentInChildren<UISliceBackgroundPopup>();
		int webMarginWidth = Mathf.FloorToInt( width - (backgroundPopup.PopupWidth - 30f) );
		int webMarginHeight = Mathf.FloorToInt( height - (backgroundPopup.PopupHeight - 30f));

		int leftRight = Mathf.FloorToInt(webMarginWidth/(2 * ratio));
			
		int top = Mathf.RoundToInt (((webMarginHeight/2) + backgroundPopup.buttonClose.GetComponentInChildren<UISprite>().height) /  ratio);
		int bottom = Mathf.RoundToInt (webMarginHeight / (2*ratio));
		webView.insets = new UniWebViewEdgeInsets(top,leftRight, bottom, leftRight);

    }

    void OnClickButtonClose(GameObject targetObject)
    {
        GameObject.Destroy(gameObject);
    }

	void OnReceivedMessage (UniWebView webView, UniWebViewMessage message)
	{
		throw new System.NotImplementedException ();
	}

	void OnLoadComplete (UniWebView webView, bool success, string errorMessage)
	{
		if (success) {
			webView.Show();
		} else {
			Debug.Log("Something wrong in webview loading: " + errorMessage);
            //_errorMessage = errorMessage;
        }
	}

	bool OnWebViewShouldClose (UniWebView webView)
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

	public void OnChooseEvent (EventModel model)
	{
		foreach (EventItemView item in EventItemView.List) {
			if(item.model == model)
				item.gameObject.SetActive(false);
			else
				if(!item.gameObject.activeSelf)
					item.gameObject.SetActive(true);
		}
		table.Reposition ();
		webView.url = model.UrlWeb;
		webView.Load ();
	}
	#endif
}
