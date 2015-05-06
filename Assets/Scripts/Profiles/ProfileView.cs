using System;
using System.Collections.Generic;
using UnityEngine;

public class ProfileView : MonoBehaviour
{
    #region Unity Editor
    public CUIHandle btClose;
	public ProfileTabbarController controller;
	public ProfileTabMessageView panelMessage;
	public ProfileTabSettingView panelSettings;
    #endregion


    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickBack;

        CUIHandle.AddClick(btClose, OnClickBack);   
	
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(btClose, OnClickBack);
        //Transform obj = HeaderMenu.Instance.transform.FindChild("Camera");
        //obj.camera.depth = 51;
    }

    void OnClickBack(GameObject go)
    {
        GameObject.Destroy(GameObject.Find("__ProfilePrefab"));
    }

    //public static ProfileView Create()
    //{
        
    //    return profile.GetComponent<ProfileView>();
    //}
    public static ProfileView _instances;
    public static ProfileView Instance
    {
        get
        {
            if (_instances == null)
            {
                GameObject profile = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/ProfilePrefab"));
                profile.name = "__ProfilePrefab";
                profile.transform.position = new Vector3(-1401, 240, -128f);
                _instances = profile.GetComponent<ProfileView>();
            }
            return _instances;
        }
    }
    public void CheckWhenStart()
    {
        StartCoroutine(_CheckWhenStart());
    }
	System.Collections.IEnumerator _CheckWhenStart()
	{
		if(StoreGame.Contains(StoreGame.EType.SEND_FRIEND_MESSAGE))
		{
            yield return new WaitForEndOfFrame();
            controller.OnSelectTabbar(1);
		}
        else if (StoreGame.Contains(StoreGame.EType.CHANGE_INFORMATION))
        {
            controller.OnSelectTabbar(5);
			panelSettings.ShowEditGeneral();
            StoreGame.Remove(StoreGame.EType.CHANGE_INFORMATION);
        }
	}

    
}
