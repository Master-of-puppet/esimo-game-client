using System;
using System.Collections.Generic;
using UnityEngine;

public class ProfileTabbarController : UITabbarController
{
    public NumberWarning numberTabMessage;
    public bool isReloadData = true;
    bool hadRequestBuddies = false;
    bool hadRequestMessage = false;
    public override void OnSelectTabbarAtIndex(int index)
    {
		if (tabbarPanel [index].gameObject.GetComponent<ProfileTabInfo> () != null) {
				tabbarPanel [index].gameObject.GetComponent<ProfileTabInfo> ().initUserInfo ();
		} else if (tabbarPanel [index].gameObject.GetComponent<ProfileTabSettingView> () != null) {
				tabbarPanel [index].gameObject.GetComponent<ProfileTabSettingView> ().initInformation ();
		} else if (tabbarPanel [index].gameObject.GetComponent<ProfileTabMessageView> () != null) {
            tabbarPanel[index].gameObject.GetComponent<ProfileTabMessageView>().gridMessageSystem.Reposition();
            tabbarPanel[index].gameObject.GetComponent<ProfileTabMessageView>().gridInvited.Reposition();
            tabbarPanel[index].gameObject.GetComponent<ProfileTabMessageView>().gridFriend.Reposition();

            if (!hadRequestMessage && GameManager.Instance.mInfo.messages.Count == 0){        
                WaitingView.Show("Đang tải dữ liệu");
                if (!hadRequestMessage)
                {
                    GameManager.Server.DoRequestCommand(Fields.REQUEST.GET_MESSAGE);
                    hadRequestMessage = true;
                }
                
                if (!hadRequestBuddies)
                {
                    hadRequestBuddies = true;
                    GameManager.Server.DoRequestCommand(Fields.REQUEST.GET_BUDDIES);
                }
            }
            else
            {
                if (StoreGame.Contains(StoreGame.EType.SEND_FRIEND_MESSAGE))
                    tabbarPanel[index].gameObject.GetComponent<ProfileTabMessageView>().OnResponseMessageHandler();
                else
                    tabbarPanel[index].gameObject.GetComponent<ProfileTabMessageView>().reLoadListMessageFriendWhenSelectTabMessage();
            }
                
			numberTabMessage.gameObject.SetActive (index != 1);
		} else if (tabbarPanel [index].gameObject.GetComponent<ProfileTabFriend> () != null) {
            tabbarPanel[index].gameObject.GetComponent<ProfileTabFriend>().tableFriend.Reposition();
            if (!hadRequestBuddies &&  GameManager.Instance.mInfo.buddies.Count == 0)
            {
                WaitingView.Show("Đang tải dữ liệu");
                hadRequestBuddies = true;
                GameManager.Server.DoRequestCommand(Fields.REQUEST.GET_BUDDIES);
            }
		}
	}

    void RepositionGridAndTable()
    {
        foreach (UITabbarPanel tab in this.tabbarPanel)
        {
            
        }
    }

    public override void OnStart()
    {
        numberTabMessage.isDisable = delegate() { return selectedIndex == 1; };
    }
}
