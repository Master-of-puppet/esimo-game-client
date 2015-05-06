using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UserOnlineRowChan : EUserOnlineRow
{
    public static UserOnlineRowChan Create(Transform parent, User user)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Channel/UserOnlineRow"));
        obj.name = string.Format("UserOnline_{0:0000000000}", DateTime.Now.ToBinary());
        obj.transform.parent = parent;
        obj.transform.localPosition = new Vector3(0f, 0f, -1f);
        obj.transform.localScale = Vector3.one;
        UserOnlineRowChan row = obj.GetComponent<UserOnlineRowChan>();
        row.SetData(user);
        List.Add(row);
        return obj.GetComponent<UserOnlineRowChan>();
    }

    void SetData(User user)
    {
        this.user = user;
        user.AvatarTexture(delegate(Texture _texture) { if (imageAvatar != null) imageAvatar.mainTexture = _texture; });
        lbUsername.text = user.username.ToString();
        if (GameManager.PlayGoldOrChip == "chip")
            lbMoney.text = Utility.Convert.Chip(user.chip);
        else if (GameManager.PlayGoldOrChip == "gold")
            lbMoney.text = Utility.Convert.Chip(user.gold);
    }
}
