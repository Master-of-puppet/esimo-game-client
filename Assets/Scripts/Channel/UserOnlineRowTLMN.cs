using UnityEngine;
using System.Collections.Generic;
using System;

public class UserOnlineRowTLMN : EUserOnlineRow {
    public static UserOnlineRowTLMN Create(Transform parent, User user)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Channel/UserOnlineRowTLMN"));
        obj.name = string.Format("UserOnline_{0:0000000000}", DateTime.Now.ToBinary());
        obj.transform.parent = parent;
        obj.transform.localPosition = new Vector3(0f, 0f, -1f);
        obj.transform.localScale = Vector3.one;
        UserOnlineRowTLMN row = obj.GetComponent<UserOnlineRowTLMN>();
        //listCUI.Add(obj.gameObject.AddComponent<CUIHandle>());
        row.SetData(user);
        List.Add(row);
        return obj.GetComponent<UserOnlineRowTLMN>();
    }

    void SetData(User user)
    {
        this.user = user;
        user.AvatarTexture(delegate(Texture _texture) { if (imageAvatar != null) imageAvatar.mainTexture = _texture; });
        lbUsername.text = user.username.ToString();
        lbMoney.text = Utility.Convert.Chip(user.chip);
    }
}
