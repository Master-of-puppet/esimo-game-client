using System;
using System.Collections.Generic;
using System.Collections;
using Electrotank.Electroserver5.Api;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Model của các tin nhắn
/// </summary>
public class Messages
{
    //public int id;
    public string id;
    public int sender;
    public string sender_name;
    public int receiver;
    public string receiver_name;
    public string content;
    public DateTime time_sent;
    public bool read;
    public int status;
    public int sender_avatar;
    public int receiver_avatar;
    public int type;

    public Messages() { }

    public Messages(EsObject eso)
    {
        if (eso.variableExists("id"))
            //id = eso.getInteger("id");
            id = eso.getString("id");
        if (eso.variableExists("sender"))
            sender = eso.getInteger("sender");
        if (eso.variableExists("sender_name"))
            sender_name = eso.getString("sender_name");
        if (eso.variableExists("receiver"))
            receiver = eso.getInteger("receiver");
        if (eso.variableExists("receiver_name"))
            receiver_name = eso.getString("receiver_name");
        if (eso.variableExists("content"))
            content = eso.getString("content");
        
        if (eso.variableExists("time_sent"))
            time_sent = DateTime.Parse(eso.getString("time_sent"));

        if(eso.variableExists("read"))
            read = eso.getBoolean("read");
        if (eso.variableExists("status"))
            status = eso.getInteger("status");
        if (eso.variableExists("sender_avatar"))
            sender_avatar = eso.getInteger("sender_avatar");
        if (eso.variableExists("receiver_avatar"))
            receiver_avatar = eso.getInteger("receiver_avatar");
        if (eso.variableExists("type"))
            type = eso.getInteger("type");
    }

    public void SetDataOutGoing(EsObject eso)
    {
        content = eso.getString("content");
        sender = GameManager.Instance.mInfo.id;
        sender_name = GameManager.Instance.mInfo.username;
        receiver_name = eso.getString("receiver_name");

        if (eso.variableExists("time_sent"))
            time_sent = System.DateTime.Parse(eso.getString("time_sent"));
        else
            time_sent = System.DateTime.Now;
        read = true;
    }

    #region FOR JSON
    public Messages(Hashtable json)
    {
        //id = int.Parse(json["id"].ToString());
        id = json["id"].ToString();
        sender = int.Parse(json["sender"].ToString());
        sender_name = json["sender_name"].ToString();
        content = json["content"].ToString();
        time_sent = DateTime.Parse(json["time_sent"].ToString());
        read = int.Parse(json["read"].ToString()) == 1;
        
        receiver = GameManager.Instance.mInfo.id;
        receiver_name = GameManager.Instance.mInfo.username;
    }

    public Hashtable ParseToHashtable
    {
        get
        {
            Hashtable hash = new Hashtable();

            hash.Add("id", id);
            hash.Add("sender", sender);
            hash.Add("sender_name", sender_name);
            hash.Add("content", content);
            hash.Add("time_sent", time_sent.ToString());
            hash.Add("read", read ? 1 : 0);
            hash.Add("receiver", GameManager.Instance.mInfo.id);
            hash.Add("receiver_name", GameManager.Instance.mInfo.username);
            return hash;
        }
    }
    #endregion

}
