using UnityEngine;
using System.Collections;

public class EGameplayChatManager : MonoBehaviour
{
    public CUIHandle buttonChat;
    public GameObject panelNumberMessage;
    GPChatView chatView;

    void Awake()
    {
        CUIHandle.AddClick(buttonChat, OnClickButtonChat);
        GameManager.Server.EventPublicMessage += OnPublicMessage;
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(buttonChat, OnClickButtonChat);
        if (!GameManager.IsExist) return;
        GameManager.Server.EventPublicMessage -= OnPublicMessage;
    }

    void Start()
    {
        GPChatView.listMessage.Clear();
        panelNumberMessage.gameObject.SetActive(false);
    }

    private int numberNewMessage = 0;
    void OnClickButtonChat(GameObject go)
    {
        chatView = GPChatView.Create();
        panelNumberMessage.gameObject.SetActive(false);
        numberNewMessage = 0;
    }
    [HideInInspector]
    public string str = "";
    public virtual void OnPublicMessage(Electrotank.Electroserver5.Api.PublicMessageEvent e)
    {
       
        GPChatView.listMessage.Add(str);

        if (e.UserName == GameManager.Instance.mInfo.username)
        {
            chatView.FocusTextChat();
            return;
        }

        if (chatView != null && chatView.gameObject != null)
            chatView.NewChatWhenOpen(str);
        else
        {
            numberNewMessage++;
            if (numberNewMessage > 0)
            {
                if (numberNewMessage > 99)
                    numberNewMessage = 99;
                panelNumberMessage.SetActive(true);
                panelNumberMessage.GetComponentInChildren<UILabel>().text = numberNewMessage.ToString();
            }
        }
    }
}