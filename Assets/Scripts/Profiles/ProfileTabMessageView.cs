using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;
using System;

public class ProfileTabMessageView : MonoBehaviour
{
    // Use this for initialization
    #region UnityEditor
    public UIGrid gridFriend;
    public UITable gridInvited;
    public UITable gridMessageSystem;
    public UIAnchor[] leftRight;
    public Transform parentPanel;
    public UIInput txtSearchFriend;
	public UITabbarController controller;

    #endregion

	
	bool watingInitMessage = false;
    void Start()
    {
        //createListMessageSystem();
        //if (GameManager.Instance.mInfo.messages.Count > 0)
        //    OnResponseMessageHandler();
        if (GameManager.Instance.mInfo.pendingBuddies.Count > 0)
            OnResponseBuddiesHandler();
    }
    //void OnEnable()
    //{
    //    GameManager.Server.EventGetMessageCallBack += OnResponseMessageHandler;
    //    GameManager.Server.EventGetBuddiesCallBack += OnResponseBuddiesHandler;
    //}

   
    //void OnDisable()
    //{
    //    if (!GameManager.IsExist) return;
    //    GameManager.Server.EventGetMessageCallBack -= OnResponseMessageHandler;
    //    GameManager.Server.EventGetBuddiesCallBack -= OnResponseBuddiesHandler;
    //    GameManager.Server.EventMessageChanged -= OnMessageChanged;
    //}

    void Awake()
    {
        Debug.Log("=========>Call register event");
        GameManager.Server.EventGetMessageCallBack += OnResponseMessageHandler;
        GameManager.Server.EventGetBuddiesCallBack += OnResponseBuddiesHandler;

        GameManager.Server.EventMessageChanged += OnMessageChanged;
        GameManager.Server.EventFriendPendingChanged += OnPendingFriend;
        GameManager.Server.EventFriendChanged += OnFriendChanged;
        GameManager.Server.EventPluginMessageOnProcess += OnProcessPulginResponse;
        UICamera.onScreenResize += ScreenSizeChanged;
    }

    private void ScreenSizeChanged()
    {
        if (gridMessageSystem.gameObject.activeSelf)
            gridMessageSystem.transform.parent.GetComponent<UIScrollView>().ResetPosition();
        if (gridFriend.gameObject.activeSelf)
            gridFriend.Reposition();
        if (gridInvited.gameObject.activeSelf)
            gridInvited.Reposition();
    }

    private void OnResponseBuddiesHandler()
    {
        createListInvited();
    }
    public void OnResponseMessageHandler()
    {
        string username = StoreGame.LoadString(StoreGame.EType.SEND_FRIEND_MESSAGE);
        if (StoreGame.Contains(StoreGame.EType.SEND_FRIEND_MESSAGE) && !string.IsNullOrEmpty(username) && GameManager.Instance.mInfo.messages.Find(m => m.receiver_name == username ) == null )
		{
            Messages mess = new Messages();
            mess.sender_name = GameManager.Instance.mInfo.username;
            mess.receiver_avatar = 0;
            mess.time_sent = System.DateTime.Now;
            mess.receiver_name = username;
            mess.content = "";
            mess.read = true;
            GameManager.Instance.mInfo.messages.Add(mess);
            createListMessageFriend(GameManager.Instance.mInfo.messages, controller.selectedIndex);
        }
        else
        {
            createListMessageFriend(GameManager.Instance.mInfo.messages, controller.selectedIndex);
        }
        if(!string.IsNullOrEmpty(username))
            StartCoroutine(FindFriendWhenSelect(username));

        if (GameManager.Instance.ListMessageSystem.Count > 0)
            createListMessageSystem();
    }

    private IEnumerator FindFriendWhenSelect(string userName) 
    {
        while (watingInitMessage)
            yield return new WaitForEndOfFrame();
        if (!string.IsNullOrEmpty(userName))
            FindFriend(userName,0);
    }
	public void reLoadListMessageFriendWhenSelectTabMessage()
	{
        if(controller.selectedIndex > 1 )
		    createListMessageFriend(GameManager.Instance.mInfo.messages, controller.selectedIndex);
	}
    void OnDestroy()
    {
        if (!GameManager.IsExist) return;
        Debug.Log("============>Call destroy event");
        GameManager.Server.EventGetMessageCallBack -= OnResponseMessageHandler;
        GameManager.Server.EventFriendPendingChanged -= OnPendingFriend;
        GameManager.Server.EventFriendChanged -= OnFriendChanged;
        GameManager.Server.EventPluginMessageOnProcess -= OnProcessPulginResponse;
        UICamera.onScreenResize -= ScreenSizeChanged;
        GameManager.Server.EventMessageChanged -= OnMessageChanged;
    }

    List<GameObject> listInvite = new List<GameObject>();
    List<GameObject> listMessageButton = new List<GameObject>();
    List<GameObject> listMessagePanel = new List<GameObject>();

    public void createListMessageFriend(List<Messages> lstMessage, int selectIndex)
    {
        if (watingInitMessage) return;
        //if (!gameObject.activeSelf) return;
        GameManager.Instance.StartCoroutine(_createListMessageFriend(lstMessage, selectIndex));
    }
    int indexDraw = 0;
    IEnumerator _createListMessageFriend(List<Messages> lstMessage, int selectIndex)
    {
        watingInitMessage = true;
        while (listMessageButton.Count > 0)
        {
            GameObject.Destroy(listMessageButton[0].gameObject);
            listMessageButton.RemoveAt(0);
            GameObject.Destroy(listMessagePanel[0].gameObject);
            listMessagePanel.RemoveAt(0);
        }

        gridFriend.Reposition();
        yield return new WaitForEndOfFrame();


        Dictionary<string, List<Messages>> dictMessages = new Dictionary<string, List<Messages>>();
        List<Messages> list = lstMessage.FindAll(m => m.sender_name == GameManager.Instance.mInfo.username);
        list.ForEach(message =>
        {
            if (dictMessages.ContainsKey(message.receiver_name) == false)
            {
                List<Messages> lst = new List<Messages>(); lst.Add(message);
                dictMessages.Add(message.receiver_name, lst);
            }
            else
                dictMessages[message.receiver_name].Add(message);
        });


        list = lstMessage.FindAll(m => m.receiver_name == GameManager.Instance.mInfo.username);
        list.ForEach(message =>
        {
            if (dictMessages.ContainsKey(message.sender_name) == false)
            {
                List<Messages> lst = new List<Messages>(); 
                lst.Add(message);
                dictMessages.Add(message.sender_name, lst);
            }
            else
                dictMessages[message.sender_name].Add(message);
        });


        List<UITabbarButton> listButton = new List<UITabbarButton>(new UITabbarButton[3]{ 
            gameObject.GetComponentInChildren<UITabbarController>().tabbarButtons[0],
            gameObject.GetComponentInChildren<UITabbarController>().tabbarButtons[1],
            gameObject.GetComponentInChildren<UITabbarController>().tabbarButtons[2]
        });
        List<UITabbarPanel> listPanel = new List<UITabbarPanel>(new UITabbarPanel[3] {
            gameObject.GetComponentInChildren<UITabbarController>().tabbarPanel[0],
            gameObject.GetComponentInChildren<UITabbarController>().tabbarPanel[1],
            gameObject.GetComponentInChildren<UITabbarController>().tabbarPanel[2]
        });


        indexDraw = 0;

        foreach (string name in dictMessages.Keys)
        {
            if (string.IsNullOrEmpty(name)) continue;
            UITabbarButton button;
            PrefabMessagePanelView panel;

            CreateClone(dictMessages[name], out button, out panel);

            listButton.Add(button);
            listPanel.Add(panel);
        }
        gameObject.GetComponentInChildren<UITabbarController>().tabbarButtons = listButton.ToArray();
        gameObject.GetComponentInChildren<UITabbarController>().tabbarPanel = listPanel.ToArray();

        if (dictMessages.Count != 0)
            gameObject.GetComponentInChildren<UITabbarController>().selectedIndex = selectIndex;
        else
            gameObject.GetComponentInChildren<UITabbarController>().selectedIndex = 1;

        gameObject.GetComponentInChildren<UITabbarController>().Start();

        ReSortFriendMessage(listButton);

        gridFriend.Reposition();
        gridFriend.transform.parent.GetComponent<UIScrollView>().UpdateScrollbars(true);
        watingInitMessage = false;
    }



    void ReSortFriendMessage(List<UITabbarButton> lst)
    {
        lst.RemoveAt(0);
        lst.RemoveAt(0);
        lst.RemoveAt(0);
        lst.Sort((x, y) => x.GetComponent<PrefabMessageFriendView>().listMessage[x.GetComponent<PrefabMessageFriendView>().listMessage.Count - 1].time_sent.CompareTo(y.GetComponent<PrefabMessageFriendView>().listMessage[y.GetComponent<PrefabMessageFriendView>().listMessage.Count - 1].time_sent));
        lst.Reverse();

        int i = 0;
        foreach (UITabbarButton bt in lst)
        {
            i++;
            bt.gameObject.name = string.Format("{0:00000} tabbar button", i);
        }
    }

    void CreateClone(List<Messages> lstMess, out UITabbarButton button, out PrefabMessagePanelView panel)
    {
        indexDraw++;
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/Message/ProfileMessageFriendPrefab"));
        obj.name = string.Format("{0:00000} tabbar button", indexDraw) + " " + name;
        obj.transform.parent = gridFriend.transform;
        obj.transform.localPosition = new Vector3(0f, 0f, -1f);
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<PrefabMessageFriendView>().SetData(GetComponentInChildren<PrefabMessageTabbarControllerView>(), lstMess);

        button = obj.GetComponent<UITabbarButton>();
        listMessageButton.Add(obj);


        obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/Message/ProfileMessagePanelPrefab"));
        obj.name = string.Format("{0:00000} tabbar panel", indexDraw) + " " + name;
        obj.transform.parent = parentPanel;
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        obj.GetComponent<PrefabMessagePanelView>().SetAnchorScrollView(parentPanel.gameObject);
        obj.GetComponent<PrefabMessagePanelView>().SetData(lstMess);

        panel = obj.GetComponent<PrefabMessagePanelView>();
        listMessagePanel.Add(obj);
    }

    public void createListInvited()
    {
        GameManager.Instance.StartCoroutine(_CreateListInvited());
    }

    IEnumerator _CreateListInvited()
    {
        while (listInvite.Count > 0)
        {
            GameObject.Destroy(listInvite[0]);
            listInvite.RemoveAt(0);
        }
        //gridInvited.repositionNow = true;
        yield return new WaitForEndOfFrame();

        foreach (User buddies in GameManager.Instance.mInfo.pendingBuddies)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/Message/ProfileMessageInviteFriendPrefabs"));
            obj.transform.parent = gridInvited.transform;
            obj.transform.localScale = Vector3.one;
			obj.transform.localPosition = Vector3.zero;
            obj.GetComponent<PrefabInviteFriendView>().SetData(buddies);
            listInvite.Add(obj);
        }
		gridInvited.Reposition();
		gridInvited.transform.parent.GetComponent<UIAnchor> ().ScreenSizeChanged ();
        gridInvited.transform.parent.parent.GetComponent<UIScrollView>().UpdateScrollbars(true);
    }

    void OnFriendChanged(User user, bool isRemove)
    {
        if (!isRemove)
        {
            GameObject obj = listInvite.Find(o => o.GetComponent<PrefabInviteFriendView>() != null && o.GetComponent<PrefabInviteFriendView>().mUser.username == user.username);
            if (obj != null)
                GameObject.Destroy(obj);
            listInvite.Remove(obj);
            gridInvited.Reposition();
        }
    }

    void OnPendingFriend(User user, bool isRemove)
    {
        if (isRemove)
        {
            GameObject obj = listInvite.Find(o => o.GetComponent<PrefabInviteFriendView>() != null && o.GetComponent<PrefabInviteFriendView>().mUser.username == user.username);
            if (obj != null)
                GameObject.Destroy(obj);
            listInvite.Remove(obj);
            gridInvited.Reposition();
        }
        else
            createListInvited();   
    }

    string searchFriend = "";
    void OnMessageChanged(Messages message, bool isOutGoing)
    {
        if (message.sender == 0)//Là tin nhắn hệ thống
        {
            DrawMessageSystem(message);
            gridMessageSystem.repositionNow = true;
        }
        else
        {
            if (string.IsNullOrEmpty(searchFriend))
                createListMessageFriend(GameManager.Instance.mInfo.messages, gameObject.GetComponentInChildren<UITabbarController>() != null ? gameObject.GetComponentInChildren<UITabbarController>().selectedIndex : 1);
            else
                OnTextChangedFindFriendMessage(searchFriend);
        }
    }

    /// <summary>
    /// Unity Editor SendMessage
    /// </summary>
    public void OnClickSearchFriend()
    {
        OnTextChangedFindFriendMessage(txtSearchFriend.value.ToLower());
    }

    /// <summary>
    /// UnityEditor SendMessage
    /// </summary>
    void OnTextChangedFindFriendMessage(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            createListMessageFriend(GameManager.Instance.mInfo.messages, gameObject.GetComponentInChildren<UITabbarController>().selectedIndex);
            return;
        }
        searchFriend = str;
        createListMessageFriend(GameManager.Instance.mInfo.messages.FindAll
        (m =>
            (m.receiver_name == GameManager.Instance.mInfo.username && m.sender_name.IndexOf(str) >= 0)
            ||
            (m.sender_name == GameManager.Instance.mInfo.username && m.receiver_name.IndexOf(str) >= 0)
        ), 3);
    }

    void OnProcessPulginResponse(string command, string action, EsObject eso)
    {
        if (command == "checkUser")
        {
            if (eso.getBoolean("exist") == false)
            {
                NotificationView.ShowMessage("Người chơi không tồn tại.", 5f);
                return;
            }
            FindFriend(transform.GetComponentInChildren<PrefabMessageTabbarControllerView>().txtFindOtherFriend.value, eso.getInteger("avatar"));
        }
    }
	
	public void FindFriend(string username, int avatar)
	{
        GameManager.Instance.StartCoroutine(_FindFriend(username, avatar));
	}

    IEnumerator _FindFriend(string username, int avatar)
    {
        while (watingInitMessage)
            yield return new WaitForFixedUpdate();

      	int index = listMessageButton.FindIndex(o => o.GetComponent<PrefabMessageFriendView>().username.text.ToUpper() == username.ToUpper());
        if (index >= 0)
            gameObject.GetComponentInChildren<UITabbarController>().OnSelectTabbar(index + 3);
        else
        {
            List<UITabbarButton> listButton = new List<UITabbarButton>(GetComponentInChildren<UITabbarController>().tabbarButtons);
            List<UITabbarPanel> listPanel = new List<UITabbarPanel>(GetComponentInChildren<UITabbarController>().tabbarPanel);

            Messages mess = new Messages();
            mess.sender_name = GameManager.Instance.mInfo.username;
            mess.receiver_avatar = avatar;
            mess.time_sent = System.DateTime.Now;
            mess.receiver_name = username;
            mess.content = "";
            mess.read = true;

            UITabbarButton button;
            PrefabMessagePanelView panel;
            CreateClone(new List<Messages>(new Messages[] { mess }), out button, out panel);
            listButton.Add(button);
            listPanel.Add(panel);
            
            GetComponentInChildren<UITabbarController>().tabbarButtons = listButton.ToArray();
            GetComponentInChildren<UITabbarController>().tabbarPanel = listPanel.ToArray();
            gameObject.GetComponentInChildren<UITabbarController>().Start();
            gameObject.GetComponentInChildren<UITabbarController>().OnSelectTabbar(GetComponentInChildren<UITabbarController>().tabbarButtons.Length - 1);

            gridFriend.Reposition();
        }
    }


    #region System Message
    List<GameObject> lstObjMessageSystem = new List<GameObject>();

    public void createListMessageSystem()
    {
        GameManager.Instance.StartCoroutine(_CreateListMessageSystem());
        //_CreateListMessageSystem();
    }

    IEnumerator _CreateListMessageSystem()
    {
        while (lstObjMessageSystem.Count > 0)
        {
            GameObject.Destroy(lstObjMessageSystem[0]);
            lstObjMessageSystem.RemoveAt(0);
        }
		gridMessageSystem.Reposition ();
        yield return new WaitForEndOfFrame();

        foreach (Messages mess in GameManager.Instance.ListMessageSystem)
            DrawMessageSystem(mess);
		
        gridMessageSystem.Reposition();
        gridMessageSystem.transform.parent.GetComponent<UIScrollView>().ResetPosition();
		gridMessageSystem.transform.parent.GetComponent<UIScrollView>().UpdateScrollbars(true);
    }
    void DrawMessageSystem(Messages mess)
    {
        lstObjMessageSystem.Add(PrefabMessageSystem.Create(mess, gridMessageSystem, leftRight).gameObject);
     
    }
    #endregion
}
