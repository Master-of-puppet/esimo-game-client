using System;
using System.Collections;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;
using UnityEngine;

public class PrefabMessageTabbarControllerView : UITabbarController
{
    public GameObject panelTitle;
	public UILabel title;
    public CUIHandle buttonSendOther;
    public UIInput txtFindOtherFriend;
    public CUIHandle btFindOtherFriend;
    public NumberWarning numberPeddingBuddies;
    public NumberWarning numberMessageSystem;
    
    public UILabel Title
    {
        get
        {
            return title;
        }
    }

    void Awake()
    {
        CUIHandle.AddClick(buttonSendOther, ClickPanelOther);
        CUIHandle.AddClick(btFindOtherFriend, OnClickFindOtherFriend);
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(buttonSendOther, ClickPanelOther);
        CUIHandle.RemoveClick(btFindOtherFriend, OnClickFindOtherFriend);
    }

    void ClickPanelOther(GameObject go)
    {
        OnSelectTabbar(0);
    }

    public override void OnSelectTabbarAtIndex(int index)
    {
        numberPeddingBuddies.gameObject.SetActive(index != 2);
        numberMessageSystem.gameObject.SetActive(index != 1);

        if (index == 1)
        {
            //List<int> list = new List<int>();
            List<string> list = new List<string>();
            GameManager.Instance.ListMessageSystem.FindAll(m => m.read == false && m.sender == 0).ForEach(mess =>
            {
                list.Add(mess.id);
                mess.read = true;
            });

            if (list.Count > 0)
            {
                GameManager.Server.DoRequestPlugin(Utility.SetEsObject("readMessage", new object[] {
                    "listId", list.ToArray()
                }));
                MessageSystemCache.SaveCache();
            }
        }
        
        panelTitle.gameObject.SetActive(index != 0);
        if (panelTitle.gameObject.activeSelf)
        {
            if (index == 1)
                Title.text = "Tin nhắn hệ thống";
            else if (index == 2)
                Title.text = "Lời mời kết bạn";
            else
                Title.text = Utility.Convert.ToTitleCase(((PrefabMessageFriendView)tabbarButtons[index]).username.text);
        }
    }

    void OnClickFindOtherFriend(GameObject go)
    {
        if (txtFindOtherFriend.value.Equals(GameManager.Instance.mInfo.username)) return;

        GameManager.Server.DoRequestPlugin(Utility.SetEsObject("checkUser",
            new object[] { Fields.PLAYER.USERNAME, txtFindOtherFriend.value }));
    }

}
