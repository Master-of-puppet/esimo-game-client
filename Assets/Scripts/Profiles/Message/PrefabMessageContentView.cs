using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabMessageContentView : MonoBehaviour
{
    public UILabel lbUsername, lbContent;
    public UITexture avatar;

    public void SetData(Messages message)
    {
        lbUsername.text = message.sender_name;// GameManager.Instance.mInfo.username == message.receiver_name ? message.sender_name : message.receiver_name;
        lbContent.text = message.content;// +" Mỗi lần xuất hiện, hai bé gái song sinh nhà của ngôi sao ‘Sex and the City’, Sarah Jessica Parker luôn gây chú ý về vẻ dễ thương cùng gu ăn mặc sành điệu.";

        ServerWeb.GetAvatarFromId(message.sender_avatar, // GameManager.Instance.mInfo.username == message.receiver_name ? message.sender_avatar : message.receiver_avatar,
            delegate(Texture _avatar) { if(avatar!=null) avatar.mainTexture = _avatar; });
    }
}
