using System;
using System.Collections.Generic;
using UnityEngine;

class ViewProfile : MonoBehaviour
{
    #region UnityEditor
    public UILabel lbUserName = null;
    public UILabel lbFullName = null, lbAge = null, lbGender = null, lbAddress = null, lbLevel = null, lbMoney = null, lbNumFriend = null, lbExp = null, lbStatus = null;
    //public Transform IconGold, IconChip;
    public UITexture avatar = null;
    public CUIHandle btnAddFriend = null, btnRemoveFriend = null, btnSendMoney = null, btnGift = null, btnSendMsg = null, btnClose = null,btnRemoveInvited = null;
    public UISlider processbar = null;
	public User information;
    public Transform Chip, Gold;
    #endregion

    User mUser;
    public static ViewProfile Create(User user)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Profiles/ViewProfilePrefab"), new Vector3(100f, 100f, 10f), Quaternion.identity);
        obj.name = "__Prefab View Profile";
        obj.GetComponent<ViewProfile>().mUser = user;
        obj.GetComponent<ViewProfile>().initInformation(user);
        return obj.GetComponent<ViewProfile>();
    }
    public void initInformation(User user)
    {
		this.information = user;
        lbUserName.text = Utility.Convert.ToTitleCase(user.username);
        lbFullName.text = user.FullName;
        lbAge.text = Utility.Convert.TimeToAge(user.brithday) > 0 ? Convert.ToString(Utility.Convert.TimeToAge(user.brithday)) : "";
        lbAddress.text = user.address;
        lbLevel.text = Convert.ToString(user.level);
        lbExp.text = user.experience + "/" + user.expMinNextLevel;
        processbar.value = (user.experience - user.expMinCurrentLevel) / (float)user.expMinNextLevel;
        if (user.isRobot)
        {
			lbGender.text =null;
			btnAddFriend.gameObject.SetActive(false);
            btnRemoveFriend.gameObject.SetActive(false);
            btnSendMoney.gameObject.SetActive(false);   
            btnGift.gameObject.SetActive(false);
            btnSendMsg.gameObject.SetActive(false);
		}else{
            if (user.gender != null)
            {
                string sex = user.gender.ToLower().Equals("male") ? "Nam" : "Nữ";
                lbGender.text = sex;
            }
		}
        //lbChip.text = Utility.Convert.Chip(user.chip);
        lbNumFriend.text = Convert.ToString(user.NumberBuddies);
        lbStatus.text = "";
        user.AvatarTexture(delegate(Texture _texture) { if (avatar != null) avatar.mainTexture = _texture; });

        bool isFriend = GameManager.Instance.mInfo.buddies.Exists(u => u.username == user.username);
        if (isFriend)
        {
            btnAddFriend.gameObject.SetActive(false);
            btnRemoveFriend.gameObject.SetActive(true);
            btnRemoveInvited.gameObject.SetActive(false);
        }
        else
        {
            if (GameManager.Instance.mInfo.requestBuddies.Find(us => us.username == information.username) != null)
            {
                btnAddFriend.gameObject.SetActive(false);
                btnRemoveInvited.gameObject.SetActive(true);
                btnRemoveFriend.gameObject.SetActive(false);
            }
        }
        if (GameManager.PlayGoldOrChip == "chip")
        {
            NGUITools.SetActive(Chip.gameObject, true);
            NGUITools.SetActive(Gold.gameObject, false);
            lbMoney.text = Utility.Convert.Chip(user.chip);
        }
        else if (GameManager.PlayGoldOrChip == "gold")
        {
            NGUITools.SetActive(Gold.gameObject, true);
            NGUITools.SetActive(Chip.gameObject, false);
            lbMoney.text = Utility.Convert.Chip(user.gold);
        }
    }

    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickClose;

        CUIHandle.AddClick(btnAddFriend, OnClickAddFriend);
        CUIHandle.AddClick(btnClose, OnClickClose);
        CUIHandle.AddClick(btnRemoveFriend, OnClickRemoveFriend);
        CUIHandle.AddClick(btnSendMoney, OnClickSendMoney);
        CUIHandle.AddClick(btnGift, OnClickGift);
        CUIHandle.AddClick(btnSendMsg, OnClickSendMsg);
		CUIHandle.AddClick(btnRemoveInvited,OnClickRemoveInvited);

        GameManager.Server.EventFriendChanged += OnFriendChanged;
        GameManager.Server.EventFriendPendingChanged += OnFriendPending;
        GameManager.Server.EventPluginMessageOnProcess += PluginMessageOnProcess;
    }

    void PluginMessageOnProcess(string command, string action, Electrotank.Electroserver5.Api.EsObject PluginMessageParameters)
    {
        if (command == "acceptFriendRequest")
        {
            this.initInformation(mUser);
        }
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnAddFriend, OnClickAddFriend);
        CUIHandle.RemoveClick(btnClose, OnClickClose);
        CUIHandle.RemoveClick(btnRemoveFriend, OnClickRemoveFriend);
        CUIHandle.RemoveClick(btnSendMoney, OnClickSendMoney);
        CUIHandle.RemoveClick(btnGift, OnClickGift);
        CUIHandle.RemoveClick(btnSendMsg, OnClickSendMsg);
		CUIHandle.RemoveClick(btnRemoveInvited,OnClickRemoveInvited);

        if (!GameManager.IsExist) return;

        GameManager.Server.EventFriendChanged -= OnFriendChanged;
        GameManager.Server.EventFriendPendingChanged -= OnFriendPending;
        GameManager.Server.EventPluginMessageOnProcess -= PluginMessageOnProcess;
    }

    #region Listener
    private void OnFriendPending(User user, bool isRemove) {
		Debug.Log("OnFriendPending");
	}
    void OnFriendChanged(User friend, bool isRemove)
    {
        if (isRemove)
        {
            btnAddFriend.gameObject.SetActive(true);
            btnRemoveFriend.gameObject.SetActive(false);
			btnRemoveInvited.gameObject.SetActive(false);
			GameManager.Instance.mInfo.requestBuddies.Clear();
        }
	}

    private void OnClickSendMsg(GameObject targetObject)
    {
        StoreGame.SaveString(StoreGame.EType.SEND_FRIEND_MESSAGE, mUser.username);
        ProfileView.Instance.CheckWhenStart();
        //ProfileView profile = ProfileView.Instance.C;
        GameObject.Destroy(gameObject);
    }
    private void OnClickGift(GameObject targetObject)
    {
        Common.VersionNotSupport("Tặng quà");
    }
    private void OnClickSendMoney(GameObject targetObject)
    {
        Common.VersionNotSupport("Chuyển tiền");
    }

    private void OnClickAddFriend(GameObject targetObject)
    {
        if (GameManager.Instance.mInfo.requestBuddies.Contains(mUser))
        {
            NotificationView.ShowMessage("Lời mời của bạn đã được gửi đi.\n\n Hãy chờ người chơi \"" + mUser.username + "\" chấp nhận.");
            return;
        }

        Electrotank.Electroserver5.Api.AddBuddiesRequest request = new Electrotank.Electroserver5.Api.AddBuddiesRequest();
        request.BuddyNames = new List<string>(new string[] { mUser.username });
        btnRemoveInvited.gameObject.SetActive(true);
        btnAddFriend.gameObject.SetActive(false);
			
        GameManager.Server.DoRequest(request);

        if (GameManager.Instance.mInfo.requestBuddies.Contains(mUser) == false){	
           GameManager.Instance.mInfo.requestBuddies.Add(mUser);
		}
    }
    private void OnClickClose(GameObject targetObject)
    {
        GameObject.Destroy(gameObject);
    }

    private void OnClickRemoveFriend(GameObject targetObject)
    {
        Electrotank.Electroserver5.Api.RemoveBuddiesRequest request = new Electrotank.Electroserver5.Api.RemoveBuddiesRequest();
        request.BuddyNames = new List<string>(new string[] { mUser.username });
        GameManager.Server.DoRequest(request);
    }
	private void OnClickRemoveInvited(GameObject targetObject)
	{
		NotificationView.ShowConfirm("Xác Nhận","Bạn đã gửi lời mời đến "+mUser.username+" hãy chờ "+mUser.username+" xác nhận.\n\nBạn có muốn hủy lời mời kết bạn này không?", Accept ,null);
	}
	private void Accept(){
		Electrotank.Electroserver5.Api.RemoveBuddiesRequest request = new Electrotank.Electroserver5.Api.RemoveBuddiesRequest();
        request.BuddyNames = new List<string>(new string[] { mUser.username });
        GameManager.Server.DoRequest(request);
		GameManager.Instance.mInfo.requestBuddies.Clear();
	}
    #endregion
}
