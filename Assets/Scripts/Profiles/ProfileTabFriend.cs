using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProfileTabFriend : MonoBehaviour
{
    #region UnityEditor
    public UITabbarController controller;
    public UITable tableFriend;
    public UIInput txtSearch;
    public CUIHandle btnOk;
    #endregion

    List<GameObject> listFriend = new List<GameObject>();
    void OnEnable()
    {
        GameManager.Server.EventGetBuddiesCallBack += OnResponseBuddiesHandler;
    }


    void OnDisable()
    {
        GameManager.Server.EventGetBuddiesCallBack -= OnResponseBuddiesHandler;
    }
    void Awake()
    {
        CUIHandle.AddClick(btnOk, OnClickFindFriend);

        GameManager.Server.EventFriendChanged += onFriendChanged;

    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnOk, OnClickFindFriend);

        if (!GameManager.IsExist) return;

        GameManager.Server.EventFriendChanged -= onFriendChanged;
    }

    void onFriendChanged(User user, bool isRemove)
    {
        if (!isRemove)
            LoadListFriend(txtSearch.value);
        else
        {
            GameObject gameObj = listFriend.Find(o => o.GetComponent<PrefabProfileFriendListItemView>() != null && o.GetComponent<PrefabProfileFriendListItemView>().mUser.username == user.username);
            GameObject.Destroy(gameObj);
            tableFriend.repositionNow = true;
        }
    }
    private void OnResponseBuddiesHandler()
    {
        LoadListFriend("");
    }
    // Use this for initialization
    void Start()
    {
        if (GameManager.Instance.mInfo.buddies.Count > 0)
            OnResponseBuddiesHandler();
    }

    public void FindFriend(User friend)
    {
        controller.OnSelectTabbar(1);
        ProfileTabMessageView panelMess = System.Array.Find<UITabbarPanel>(controller.tabbarPanel, p => p.GetComponent<ProfileTabMessageView>() != null).GetComponent<ProfileTabMessageView>();
        panelMess.FindFriend(friend.username, 0);
    }

    void OnClickFindFriend(GameObject go)
    {
        LoadListFriend(txtSearch.value);
    }

    public void LoadListFriend(string friendName)
    {
        GameManager.Instance.StartCoroutine(_CreateListFriend(friendName.Trim()));
    }

    IEnumerator _CreateListFriend(string friendName)
    {
        while (listFriend.Count > 0)
        {
            GameObject.Destroy(listFriend[0]);
            listFriend.RemoveAt(0);
        }
        tableFriend.repositionNow = true;
        yield return new WaitForEndOfFrame();

        int indexDraw = 0;

        List<User> lstFriend = string.IsNullOrEmpty(friendName) ? GameManager.Instance.mInfo.buddies :
            GameManager.Instance.mInfo.buddies.FindAll(u => u.username.IndexOf(friendName) >= 0);

        foreach (User user in lstFriend)
        {
            indexDraw++;
            GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/Message/ProfileFriendPrefab"));
            obj.name = string.Format("{0:00000} list friend", indexDraw) + " " + user.username;
            obj.transform.parent = tableFriend.transform;
            obj.transform.localPosition = new Vector3(0f, 0f, -1f);
            obj.transform.localScale = Vector3.one;
            obj.GetComponent<PrefabProfileFriendListItemView>().SetData(user);
            obj.GetComponent<PrefabProfileFriendListItemView>().tabFriend = this;
            Utility.AddCollider(obj);
            listFriend.Add(obj);
        }
        tableFriend.Reposition();
		tableFriend.transform.parent.GetComponent<UIAnchor> ().ScreenSizeChanged ();
		tableFriend.transform.parent.parent.GetComponent<UIScrollView> ().UpdateScrollbars (true);
    }
}
