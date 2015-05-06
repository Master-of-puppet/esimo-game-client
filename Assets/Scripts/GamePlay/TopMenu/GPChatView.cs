using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GPChatView : MonoBehaviour
{
    public CUIHandle btSendMessage,btnClose;
    public CUITextList listChat;
    public ChatInput chatinput;
    //Others
    public static List<string> listMessage = new List<string>();
    
    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickButtonClose;
        CUIHandle.AddClick(btSendMessage, sendMessageChat);
        CUIHandle.AddClick(btnClose, OnClickButtonClose);
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(btSendMessage, sendMessageChat);
        CUIHandle.RemoveClick(btnClose, OnClickButtonClose);
    }

    void OnClickButtonClose(GameObject go)
    {
        GameObject.Destroy(gameObject);
    }

    void Start()
    {
        listMessage.ForEach(str => {
            listChat.Add(str);
            Utility.AutoScrollChat(listChat);
        });
        FocusTextChat();
    }

    public static void MessageFromSystem(string textChat)
    {
		Debug.Log ("==========> " + textChat);
        listMessage.Add(textChat);
        GameObject obj = GameObject.Find("__Prefab Chat Gameplay");
        if (obj != null)
            obj.GetComponent<GPChatView>().NewChatWhenOpen(textChat);
    }

    public void NewChatWhenOpen(string textChat)
    {
        listChat.Add(textChat);
        Utility.AutoScrollChat(listChat);
    }

    public void FocusTextChat()
    {
        if (Common.IsMobile) return;
        UICamera.selectedObject = chatinput.gameObject;
    }

    public static GPChatView Create()
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/TopMenu/GameplayChatView"));
		obj.name = "__Prefab Chat Gameplay";
        return obj.GetComponent<GPChatView>();
    }

    void sendMessageChat(GameObject go)
    {
        chatinput.SendMessage("Submit");
    }
}
