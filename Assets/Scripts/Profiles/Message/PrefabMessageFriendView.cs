using System;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;
using UnityEngine;


public class PrefabMessageFriendView : UITabbarButton
{
    public PrefabMessageTabbarControllerView tabbarController;
    public UILabel username, time, count;
    public UITexture avatar;
    int numberNoneRead = 0;

    [HideInInspector]
    public List<Messages> listMessage;

    public void SetData(PrefabMessageTabbarControllerView controller, List<Messages> _listMessage)
    {
        tabbarController = controller;
        listMessage = _listMessage;
        if (listMessage[0].sender_name == GameManager.Instance.mInfo.username)
        {
            username.text = listMessage[0].receiver_name;
            username.text = CutString(username.text);
        }
        else
        {
            username.text = listMessage[0].sender_name;
            username.text = CutString(username.text);
        }
        listMessage.Sort((x, y) => x.time_sent.CompareTo(y.time_sent));
        //GameManager.Instance.mInfo.AvatarTexture(delegate(Texture _texture) { avatar.mainTexture = _texture; });
        //Debug.Log
        ServerWeb.GetAvatarFromId(GameManager.Instance.mInfo.id == listMessage[0].receiver ? listMessage[0].sender_avatar : listMessage[0].receiver_avatar,
           delegate(Texture _avatar) { 
                if(avatar !=null) avatar.mainTexture = _avatar; 
           });

        numberNoneRead = listMessage.FindAll(m => m.receiver_name == GameManager.Instance.mInfo.username && m.read == false).Count;
        SetCountNumber(numberNoneRead);


    }

    private string CutString(string username) {
        string value = username;
        if (username.Length > 16)
            value = username.Substring(0, 16) + "...";
        return value;
    }

    protected override void OnSelectedChanged(bool selected)
    {
        if (username != null)
            username.color = isSelected ? new Color(24/255f , 51/255f , 47/255f) : new Color(255 / 255f, 204 / 255f, 0 / 255f);
        time.color = isSelected ? new Color(24 / 255f, 51 / 255f, 47 / 255f) : new Color(255 / 255f, 204 / 255f, 0 / 255f);
        
        if (selected && listMessage != null && numberNoneRead > 0)
        {
            //List<int> list = new List<int>();
            List<string> list = new List<string>();
            listMessage.FindAll(m => m.read == false && m.sender_name != GameManager.Instance.mInfo.username).ForEach(mess =>
            {
                //list.Add(mess.id);
                list.Add(mess.id.ToString());
                mess.read = true;
            });
            GameManager.Server.DoRequestPlugin(Utility.SetEsObject("readMessage", new object[] {
                "listId", list.ToArray()
            }));

            SetCountNumber(0);
            numberNoneRead = 0;
            GameManager.Instance.ReadedMessageProfile();
        }
    }

    public void Update()
    {
        if (index == 2)
            time.text = GameManager.Instance.mInfo.pendingBuddies.Count == 0 ? "Không có" : Utility.Convert.MessageTime(GameManager.Instance.mInfo.pendingBuddies[GameManager.Instance.mInfo.pendingBuddies.Count - 1].timeRequest);
        else if (index == 0 || index == 1)
            time.text = "Không có";
        else
            time.text = Utility.Convert.MessageTime(listMessage[listMessage.Count - 1].time_sent);
    }

    public void SetCountNumber(int _value)
    {
        Debug.LogWarning("set number value:" + _value);
        if (count == null) return;
        count.text = _value.ToString();
        count.transform.parent.gameObject.SetActive(_value > 0);
    }
}
