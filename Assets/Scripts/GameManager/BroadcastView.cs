using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Cái này là phần View của cái Broadcast
/// </summary>
public class BroadcastView : MonoBehaviour
{
    #region Unity Editor
    public UILabel lable;
    public TweenPosition tween;
    public Transform broadCastObj;
    #endregion

    public int i = 0;

    private Vector3 pointStart = new Vector3(300f, -1f, -1f);
    private Vector3 pointEnd = new Vector3(-300f, -1f, -1f);
    public void Show(string text)
    {
        lable.text = text;
        int length = lable.text.Length;
        pointEnd.x = (pointEnd.x - lable.width) - 100f;
        tween.to = pointEnd;
        tween.duration = lable.width * 0.02f;
    }
	
    public void ShowInLobby(string text) 
    {
        if (GameManager.CurrentScene != ESceneName.LobbyChan) return;
        broadCastObj.localPosition = new Vector3(50f, 95f, 0f);
        UISprite background = broadCastObj.GetChild(0).GetComponent<UISprite>();
        UIPanel panel = broadCastObj.GetChild(1).GetComponent<UIPanel>();
        background.width = 340;
        background.height = 40;
        panel.baseClipRegion = new Vector4(0f, 0f, 330f, 45f);
        pointStart = new Vector3(125f, -1f, -1f);
        pointEnd = new Vector3(-125f, -1f, -1f);
        lable.transform.localPosition = new Vector3(125f, -1f, -1f);
		Show (text);
    }
    public void ShowInChannel(string text)
    {
        if (GameManager.CurrentScene != ESceneName.ChannelChan) return;
        broadCastObj.localPosition = new Vector3(95f, -474f, 0f);
        UISprite background = broadCastObj.GetChild(0).GetComponent<UISprite>();
        UIPanel panel = broadCastObj.GetChild(1).GetComponent<UIPanel>();
        background.width = 572;
        background.height = 35;
        //panel.baseClipRegion = new Vector4(0f, 0f, 640f, 45f);
        pointStart = new Vector3(280f, -1f, -1f);
        pointEnd = new Vector3(-280f, -1f, -1f);
        lable.transform.localPosition = new Vector3(280f, -1f, -1f);
		Show (text);
    }
	static BroadcastView _instance;
    public static BroadcastView Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/BroadcastPrefab"));
                obj.name = "__BroadcastPrefab";
                //GameObject.DontDestroyOnLoad(obj);
                _instance = obj.GetComponent<BroadcastView>();
            }
            return _instance;
        }
    }

    public static void Destroy()
    {
        if (GameObject.Find("__BroadcastPrefab") != null)
            GameObject.Destroy(GameObject.Find("__BroadcastPrefab"));
    }
    
}

