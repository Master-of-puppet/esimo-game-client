using UnityEngine;
using System.Collections;
using System;

public class UserOnlineRowPhom : EUserOnlineRow
{
    public static UserOnlineRowPhom Create(Transform parent, User user)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Channel/UserOnlineRowPhom"));
        obj.name = string.Format("UserOnline_{0:0000000000}", DateTime.Now.ToBinary());
        obj.transform.parent = parent;
        obj.transform.localPosition = new Vector3(0f, 0f, -1f);
        obj.transform.localScale = Vector3.one;
        UserOnlineRowPhom row = obj.GetComponent<UserOnlineRowPhom>();
        row.SetData(user);
        List.Add(row);
        return obj.GetComponent<UserOnlineRowPhom>();
    }
    void SetData(User user)
    {
        this.user = user;
        user.AvatarTexture(delegate(Texture _texture) { if (imageAvatar != null) imageAvatar.mainTexture = _texture; });
        lbUsername.text = user.username.ToString();
        lbMoney.text = Utility.Convert.Chip(user.chip);
    }
}
