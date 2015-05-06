using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabInviteFriendView : MonoBehaviour
{
    static PrefabInviteFriendView oldSelected;

    public UILabel lbUsername, lbChip, lbLevel, lbGold;
    public UITexture avatar;
    public CUIHandle buttonAccept, buttonCancel;
	private string nameBackgroundUnSelect = "BackgroundUnSelect", nameBackgroundSelected = "BackgroundSelected";
    
    [HideInInspector]
    public User mUser;

    void Awake()
    {
        CUIHandle.AddClick(gameObject.GetComponent<CUIHandle>(), OnClickMe);
        CUIHandle.AddClick(buttonAccept, ClickAccept);
        CUIHandle.AddClick(buttonCancel, ClickCancel);
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(gameObject.GetComponent<CUIHandle>(), OnClickMe);
        CUIHandle.RemoveClick(buttonAccept, ClickAccept);
        CUIHandle.RemoveClick(buttonCancel, ClickCancel);
    }

    void OnClickMe(GameObject go)
    {
        if (this == oldSelected) return;

        if (oldSelected != null && oldSelected.gameObject != null)
        {
			GameObject unSelected = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Profiles/BackgroundInvitedFriendUnCheck", typeof(GameObject)));
			unSelected.name = nameBackgroundUnSelect;
            unSelected.transform.parent = oldSelected.transform;
			unSelected.transform.localScale = Vector3.one;
			unSelected.transform.localPosition = new Vector3(157f,44f,0f);
			if(oldSelected.transform.FindChild(nameBackgroundSelected) !=null)
				GameObject.Destroy(oldSelected.transform.FindChild(nameBackgroundSelected).gameObject);
			
			
            oldSelected.gameObject.GetComponent<BoxCollider>().size = new Vector3(700f, 76f, -1f);
			for(int i = 0 ; i< oldSelected.gameObject.transform.childCount; i++)
			{
			UILabel label = oldSelected.gameObject.transform.GetChild(i).GetComponent<UILabel>();
				if(label  !=null)
				{
					label.transform.Translate(0f, -0.075f, 0f);
				}
			}
           // Array.ForEach<UILabel>(oldSelected.gameObject.GetComponentsInChildren<UILabel>(), lable => { lable.transform.Translate(0f, -0.075f, 0f); });
            Array.ForEach<UITexture>(oldSelected.gameObject.GetComponentsInChildren<UITexture>(), lable => { lable.transform.Translate(0f, -0.081f, 0f); });

            oldSelected.buttonAccept.gameObject.SetActive(false);
            oldSelected.buttonCancel.gameObject.SetActive(false);
        }
		
		GameObject selectedInstance = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Profiles/BackgroundInvitedFriendChecked", typeof(GameObject)));
		selectedInstance.name = nameBackgroundSelected;
        selectedInstance.transform.parent = gameObject.transform;
		selectedInstance.transform.localPosition =  new Vector3(157f,44f,0f);
		selectedInstance.transform.localScale = Vector3.one;
	
		if(	gameObject.transform.FindChild(nameBackgroundUnSelect) != null)
			GameObject.Destroy(gameObject.transform.FindChild(nameBackgroundUnSelect).gameObject);
		
        GetComponent<BoxCollider>().size = new Vector3(700f, 126f, -1f);
		
		for(int i = 0 ; i< gameObject.transform.childCount; i++)
			{
			UILabel label = gameObject.transform.GetChild(i).GetComponent<UILabel>();
				if(label  !=null)
				{
					label.transform.Translate(0f, 0.075f, 0f);
				}
			}
      //  Array.ForEach<UILabel>(GetComponentsInChildren<UILabel>(), lable => { lable.transform.Translate(0f, 0.075f, 0f); });
        Array.ForEach<UITexture>(GetComponentsInChildren<UITexture>(), lable => { lable.transform.Translate(0f, 0.081f, 0f); });

        buttonAccept.gameObject.SetActive(true);
        buttonCancel.gameObject.SetActive(true);

        gameObject.transform.parent.GetComponent<UITable>().repositionNow = true;
        oldSelected = this;
    }

    public void SetData(User user)
    {
        lbUsername.text = user.username;
        lbLevel.text = "Level " + user.level;
        lbChip.text = Utility.Convert.Chip(user.chip);
        lbGold.text = Utility.Convert.Chip(user.gold);
        user.AvatarTexture(delegate(Texture _texture) { if (avatar != null) avatar.mainTexture = _texture; });

        buttonAccept.gameObject.SetActive(false);
        buttonCancel.gameObject.SetActive(false);

        mUser = user;
    }

    void ClickCancel(GameObject go)
    {
        Electrotank.Electroserver5.Api.RemoveBuddiesRequest request = new Electrotank.Electroserver5.Api.RemoveBuddiesRequest();
        request.BuddyNames = new List<string>(new string[] { mUser.username });
        GameManager.Server.DoRequest(request);
    }

    void ClickAccept(GameObject go)
    {
        Electrotank.Electroserver5.Api.AddBuddiesRequest request = new Electrotank.Electroserver5.Api.AddBuddiesRequest();
        request.BuddyNames = new List<string>(new string[] { mUser.username });
        GameManager.Server.DoRequest(request);
    }
}
