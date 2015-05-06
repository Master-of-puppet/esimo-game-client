using UnityEngine;
using System.Collections;

public class LoginGoogleView : MonoBehaviour {
#region UnityEditor
    public UIPopupList popupUser;
    public CUIHandle btLogin, btOtherAccount,btClose;

#endregion
    [HideInInspector]
    private string account;

    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnCloseClick;
        CUIHandle.AddClick(btLogin,OnLoginOnClick);
        CUIHandle.AddClick(btOtherAccount, OnOtherAccountClick);
        CUIHandle.AddClick(btClose, OnCloseClick);
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(btLogin, OnLoginOnClick);
        CUIHandle.RemoveClick(btOtherAccount, OnOtherAccountClick);
        CUIHandle.RemoveClick(btClose, OnCloseClick);

    }
	void Start () {

	}
	void Update () {
	
	}


    private void OnCloseClick(GameObject targetObject)
    {
        GameObject.Destroy(gameObject);
    }

    private void OnOtherAccountClick(GameObject targetObject)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                GoogleAndroid.addOtherUser();
                break;
        }
    }

    private void OnLoginOnClick(GameObject targetObject)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                GoogleAndroid.login(account);
                break;
        }
        
    }
    public static LoginGoogleView Create(string[] accounts)
    {
        if (GameObject.Find("__Google ID Root") != null)
            return null;
        GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/GoogleID")) as GameObject;
        obj.name = string.Format("__Google ID Root");
        obj.transform.localPosition = new Vector3(1602f,0f,0f);
        obj.GetComponent<LoginGoogleView>().addAccounts(accounts);
        return obj.GetComponent<LoginGoogleView>();
        
    }
    private void addAccounts(string[] accounts)
    {
        popupUser.items.Clear();
        for (int i = 0; i < accounts.Length;i++ )
        {
            popupUser.items.Add(accounts[i]);
        }
    }
    void OnSelectionChange()
    {
        account = popupUser.value;
    }
}
