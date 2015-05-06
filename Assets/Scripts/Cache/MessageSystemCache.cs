using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Cache cho tin nhắn hệ thống
/// Nó cũng giống Avatar Cache thôi nhé. Nên cũng ko nói gì cả.
/// </summary>
public class MessageSystemCache
{
    public delegate void DelegateResponseMessageSystem();
    public static event DelegateResponseMessageSystem EventResponseMessageSystem;
    /// <summary>
    /// Load tin nhắn hệ thống từ cache của game và kiểm tra tin nhắn mới từ database
    /// </summary>
    public static void DownloadOrLoadCache()
    {
        GameManager.Instance.ListMessageSystem.Clear();
        if (StoreGame.Contains(StoreGame.EType.CACHE_MESSAGE_SYSTEM))
        {
            ArrayList cacheMessageSystem = (ArrayList)JSON.JsonDecode(StoreGame.LoadString(StoreGame.EType.CACHE_MESSAGE_SYSTEM));

            foreach (Hashtable hash in cacheMessageSystem)
                GameManager.Instance.ListMessageSystem.Add(new Messages(hash));

            GameManager.Instance.ListMessageSystem.Sort((x, y) => x.time_sent.CompareTo(y.time_sent));
        }
        
        #region DOWNLOAD
        WWWForm formMessageSystem = new WWWForm();
        formMessageSystem.AddField("sender", 0);
        formMessageSystem.AddField("receiver", GameManager.Instance.mInfo.id);
        formMessageSystem.AddField(ServerWeb.PARAM_PARTNER_ID, GameSettings.Instance.ParnerCodeIdentifier);
        if (GameManager.Instance.ListMessageSystem.Count > 0)
        {
            DateTime time_send = GameManager.Instance.ListMessageSystem[GameManager.Instance.ListMessageSystem.Count - 1].time_sent;
            formMessageSystem.AddField("time", string.Format("{0:yyyy-MM-dd HH:mm:ss}", time_send));
            //Debug.Log(ServerWeb.URL_GETINFO_MESSAGE + "?sender=0&receiver=" + GameManager.Instance.mInfo.id + "&time=" + string.Format("{0:yyyy-MM-dd HH:mm:ss}", time_send));
        }

        ServerWeb.StartThread(ServerWeb.URL_GETINFO_MESSAGE, formMessageSystem, delegate(bool isDone, WWW response, IDictionary json)
        {
            var x = json;
            if (isDone && json["code"].ToString() == "1")
            {
                ArrayList downloadMessageSystem = (ArrayList)json["item"];
                List<Hashtable> lst = new List<Hashtable>();
                foreach (Hashtable hash in downloadMessageSystem)
                    lst.Add(hash);
                SaveCache(lst.ToArray());
                if (EventResponseMessageSystem != null)
                    EventResponseMessageSystem();
            }
        });
        #endregion
    }

    /// <summary>
    /// Lưu tin nhắn hệ thống vào cache của game
    /// </summary>
    /// <param name="_params">Danh sách các tin mới</param>
    public static void SaveCache(params Hashtable[] _params)
    {
        if (_params != null && _params.Length > 0)
        {
            foreach (Hashtable hash in _params)
            {
                Messages mess = new Messages(hash);
                if (GameManager.Instance.ListMessageSystem.Contains(mess) == false)
                    GameManager.Instance.ListMessageSystem.Add(new Messages(hash));
            }
        }

        GameManager.Instance.ListMessageSystem.Sort((x, y) => x.time_sent.CompareTo(y.time_sent));
        List<Messages> messageSystem = GameManager.Instance.ListMessageSystem;

        ArrayList cache = new ArrayList();
        foreach (Messages mess in messageSystem)
            cache.Add(mess.ParseToHashtable);

        StoreGame.SaveString(StoreGame.EType.CACHE_MESSAGE_SYSTEM, JSON.JsonEncode(cache));
    }
}
