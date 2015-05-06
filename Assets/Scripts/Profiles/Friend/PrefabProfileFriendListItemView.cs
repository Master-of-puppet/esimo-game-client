using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PrefabProfileFriendListItemView : MonoBehaviour
{
    static PrefabProfileFriendListItemView isSelected;
    
    #region UnityEditor
    public UILabel lbUserName, lbChip, lbExp, lbGold;
    public UITexture avatar;
    public CUIHandle btnRemoveFriend, btnGift, btnSendMsg,btnInfo;
	private string nameBackgroundUnCheck = "BackgroundFriendUnChecked" , nameBackgroundChecked = "BackgroundFriendChecked";
    #endregion

    [HideInInspector]
    public User mUser;
	[HideInInspector]
    public ProfileTabFriend tabFriend;
    void Awake()
    {
        CUIHandle.AddClick(gameObject.GetComponent<CUIHandle>(), OnClickMe);
        CUIHandle.AddClick(btnRemoveFriend, OnClickRemove);
        CUIHandle.AddClick(btnGift, OnClickGift);
        CUIHandle.AddClick(btnSendMsg, OnClickSendMsg);
		CUIHandle.AddClick (btnInfo, OnClickBtnInfo);
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(gameObject.GetComponent<CUIHandle>(), OnClickMe);
        CUIHandle.RemoveClick(btnRemoveFriend, OnClickRemove);
        CUIHandle.RemoveClick(btnGift, OnClickGift);
        CUIHandle.RemoveClick(btnSendMsg, OnClickSendMsg);
		CUIHandle.AddClick (btnInfo, OnClickBtnInfo);
    }

    public void SetData(User friend)
    {
        lbUserName.text = friend.username;
        lbChip.text = Utility.Convert.Chip(friend.chip);
        lbGold.text = Utility.Convert.Chip(friend.gold);
        lbExp.text = System.Convert.ToString(friend.level);
        //lbGuild.text = friend.
        friend.AvatarTexture(delegate(Texture _texture) { if (avatar != null) avatar.mainTexture = _texture; });
        hideAllButton();
        this.mUser = friend;
    }

    public void hideAllButton()
    {
        btnRemoveFriend.gameObject.SetActive(false);
        btnGift.gameObject.SetActive(false);
        btnSendMsg.gameObject.SetActive(false);
		btnInfo.gameObject.SetActive (false);
    }
    public void showAllButton()
    {
        btnRemoveFriend.gameObject.SetActive(true);
        btnGift.gameObject.SetActive(true);
        btnSendMsg.gameObject.SetActive(true);
		btnInfo.gameObject.SetActive (true);
    }

    #region Listener

	void OnClickBtnInfo (GameObject targetObject)
	{
		ViewProfile.Create (mUser);
	}

    private void OnClickSendMsg(GameObject targetObject)
    {
        tabFriend.FindFriend(mUser);
    }

    private void OnClickGift(GameObject targetObject)
    {
		 Common.VersionNotSupport("Tặng quà");
    }

    private void OnClickRemove(GameObject targetObject)
    {
		NotificationView.ShowConfirm("Xác Nhận","Bạn có muốn xóa "+mUser.username+" người này ra khỏi danh sách bạn bè ?", Accept, null);
    }
	private void Accept()
	{
		Electrotank.Electroserver5.Api.RemoveBuddiesRequest request = new Electrotank.Electroserver5.Api.RemoveBuddiesRequest();
        request.BuddyNames = new List<string>(new string[] { mUser.username });
        GameManager.Server.DoRequest(request);	
	}
	
    private void OnClickMe(GameObject targetObject)
    {

        if (this == isSelected) return;
        if (isSelected != null && isSelected.gameObject != null)
        {
			GameObject BackgroundUnSelected = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Profiles/BackgroundFriendUnChecked", typeof(GameObject)));
			BackgroundUnSelected.name = nameBackgroundUnCheck;
            BackgroundUnSelected.transform.parent = isSelected.transform;
			BackgroundUnSelected.transform.localScale = Vector3.one;
			BackgroundUnSelected.transform.localPosition = Vector3.zero;
			if(isSelected.transform.FindChild(nameBackgroundChecked) !=null)
				GameObject.Destroy(isSelected.transform.FindChild(nameBackgroundChecked).gameObject);	
			
            isSelected.gameObject.GetComponent<BoxCollider>().size = new Vector3(950f, 63f, 3f);
			for(int i = 0 ; i< isSelected.gameObject.transform.childCount; i++)
			{
			UILabel label = isSelected.gameObject.transform.GetChild(i).GetComponent<UILabel>();
				if(label  !=null)
				{
					label.transform.Translate(0f, -0.055f, 0f);
				}
			}
            Array.ForEach<UITexture>(isSelected.gameObject.GetComponentsInChildren<UITexture>(), lable => { lable.transform.Translate(0f, -0.055f, 0f); });
            isSelected.hideAllButton();
        }
		
		GameObject BackgroundSelected = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Profiles/BackgroundFriendChecked", typeof(GameObject)));
		BackgroundSelected.name = nameBackgroundChecked;
        BackgroundSelected.transform.parent = gameObject.transform;
		BackgroundSelected.transform.localScale = Vector3.one;
		BackgroundSelected.transform.localPosition = Vector3.zero;
		if(gameObject.transform.FindChild(nameBackgroundUnCheck) !=null)
			GameObject.Destroy(gameObject.transform.FindChild(nameBackgroundUnCheck).gameObject);
		
        GetComponent<BoxCollider>().size = new Vector3(950f, 86f, 10f);
		
		for(int i = 0 ; i< gameObject.transform.childCount; i++)
		{
			UILabel label = gameObject.transform.GetChild(i).GetComponent<UILabel>();
			if(label  !=null)
			{
				label.transform.Translate(0f, 0.055f, 0f);
			}
		}
        Array.ForEach<UITexture>(GetComponentsInChildren<UITexture>(), lable => { lable.transform.Translate(0f, 0.055f, 0f); });
        gameObject.transform.parent.GetComponent<UITable>().repositionNow = true;
        showAllButton();
        isSelected = this;
    }

    #endregion


}
