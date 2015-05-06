using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EUserOnlineRow : MonoBehaviour {

    #region UnityEditor
    public UITexture imageAvatar;
    public UILabel lbUsername, lbMoney;
    //public UISprite imageIcon;
    #endregion
    public User user;
    static List<EUserOnlineRow> _list = new List<EUserOnlineRow>();
    public static List<EUserOnlineRow> List
    {
        get { return _list; }
    }
    public static List<CUIHandle> listCUI = new List<CUIHandle>();

    public static void Remove(EUserOnlineRow row)
    {
        if (List.Contains(row))
        {
            if (row.gameObject != null)
            {
                CUIHandle.RemoveClick(row.gameObject.GetComponent<CUIHandle>(), row.gameObject.GetComponent<EUserOnlineRow>().OnClickUserOnline);
                GameObject.Destroy(row.gameObject);
            }
            List.Remove(row);
        }
    }
    void Awake()
    {
        CUIHandle.AddClick(gameObject.GetComponent<CUIHandle>(), OnClickUserOnline);
    }
	
    
    public void OnClickUserOnline(GameObject go)
    {
        ViewProfile.Create(this.user);
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(gameObject.GetComponent<CUIHandle>(), OnClickUserOnline);
    }
}
