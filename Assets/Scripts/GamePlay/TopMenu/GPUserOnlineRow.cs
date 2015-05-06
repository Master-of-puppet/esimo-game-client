using System;
using System.Collections.Generic;
using UnityEngine;

public class GPUserOnlineRow : UIToggle
{
    
    public UILabel lbUserName, lbChip, lbLevel;

    [HideInInspector]
    public User mUser;

    public static GPUserOnlineRow Create(int index, Transform parent, User user, GPInviteUserView code)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/TopMenu/GameplayInvitePlayer"));
        obj.name = string.Format("UserOnline_{0:0000000000}", index);
        obj.transform.parent = parent;
        obj.transform.localPosition = new Vector3(0f, 0f, -1f);
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<GPUserOnlineRow>().SetData(user, code);
        return obj.GetComponent<GPUserOnlineRow>();
    }

    void SetData(User user, GPInviteUserView code)
    {
        lbUserName.text = user.username;

        if (GameManager.PlayGoldOrChip == "chip")
            lbChip.text = Utility.Convert.Chip(user.chip);
        if (GameManager.PlayGoldOrChip == "gold")
            lbChip.text = Utility.Convert.Chip(user.gold);

        lbLevel.text = "Level " + user.level;
        this.code = code;
        mUser = user;
    }

    GPInviteUserView code;
    void OnActivate(bool isCheck)
    {
        GPUserOnlineRow row = code.listUserOnline.Find(u => u.mUser.username == mUser.username);
        if (row != null)
            row.value = isCheck;   
    }
}
