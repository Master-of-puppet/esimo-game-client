using System;
using System.Collections.Generic;
using UnityEngine;

public class ProfileTabInfo : MonoBehaviour
{
    #region Unity Editor
    public UILabel lbUsername,lbFullname, lbAge, lbSex, lbAddress;
    public UIInput inputStatus;
    public UILabel lbChip, lbMoney, lbNumberFriend;
    public CUIHandle btSaveStatus;
    public UITexture avatar;
    public UILabel lbTextLevel;
    public UILabel lbLevel;
    public UISlider processbar;
    #endregion

//    void Start()
//    {
//		initUserInfo();
//    }
	public void initUserInfo(){
		User user = GameManager.Instance.mInfo;
		lbUsername.text = Utility.Convert.ToTitleCase(user.username);
		
		string fullName = user.lastName + " "+ user.middleName + " " + user.firstName;
		lbFullname.text = fullName;
		string sex = user.gender.ToLower().Equals("male")?"Nam":"Nữ";
		lbSex.text = sex;
		string age = Utility.Convert.TimeToAge(user.brithday) > 0 ? Convert.ToString(Utility.Convert.TimeToAge(user.brithday)) : "";
		lbAge.text = age;
		lbAddress.text = user.address;
		lbChip.text = Utility.Convert.Chip(user.chip);
        lbMoney.text = Utility.Convert.Chip(user.gold);
        user.AvatarTexture(delegate(Texture _texture) { if (avatar != null) avatar.mainTexture = _texture; });
        //lbNumberFriend.text = Convert.ToString(user.buddies.Count);
        lbNumberFriend.text = user.NumberBuddies.ToString();
        lbLevel.text = (lbTextLevel == null ? "LV " : "") + GameManager.Instance.mInfo.level;
        processbar.value = (GameManager.Instance.mInfo.experience - GameManager.Instance.mInfo.expMinCurrentLevel) / (float)GameManager.Instance.mInfo.expMinNextLevel;
        if (lbTextLevel != null)
            lbTextLevel.text = GameManager.Instance.mInfo.experience + "/" + GameManager.Instance.mInfo.expMinNextLevel;
    }
    
}
