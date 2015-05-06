using System;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;
using UnityEngine;

public class PrefabMessagePanelView : UITabbarPanel
{
    public UITable tableContent;
    public CUIHandle buttonSendMessage;
	public UIInput input;
    [HideInInspector]
    public List<Messages> listMessage;

    void Awake()
    {
        CUIHandle.AddClick(buttonSendMessage, OnClickSendMessage);
        
    }
    public void SetAnchorScrollView(GameObject gobj)
    {
        tableContent.transform.parent.GetComponent<UIPanel>().SetAnchor(gobj,0, 0, 0, 0);
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(buttonSendMessage, OnClickSendMessage);
    }

    public void SetData(List<Messages> messs)
    {
        listMessage = messs;
        listMessage.Sort((x, y) => x.time_sent.CompareTo(y.time_sent));

        foreach (Messages m in listMessage)
            Draw(m);
        tableContent.Reposition();
        tableContent.onReposition = SetScroll;
    }

    void SetScroll()
    {
		if(tableContent.transform.childCount > 1)
		{
		    float StartPosition = Math.Abs(tableContent.transform.GetChild(tableContent.transform.childCount - 1).transform.localPosition.y);
            StartPosition += tableContent.transform.GetChild(tableContent.transform.childCount - 1).GetComponent<BoxCollider>().size.y;
            tableContent.transform.parent.GetComponent<UIScrollView>().MoveRelative(new Vector3(tableContent.transform.parent.localPosition.x, StartPosition - tableContent.transform.parent.GetComponent<UIPanel>().finalClipRegion.w - tableContent.transform.parent.GetComponent<UIPanel>().finalClipRegion.y, tableContent.transform.parent.localPosition.z));
            tableContent.onReposition = null;
		}
    }

    int countDraw = 0;
    Messages oldDraw;
    public void Draw(Messages message)
    {
        if (string.IsNullOrEmpty(message.content)) return;

        GameObject obj;
        if (oldDraw == null || oldDraw.time_sent.ToShortDateString() != message.time_sent.ToShortDateString())
        {
            countDraw++;
            obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/Message/ProfileMessageLineDatePrefab"));
			obj.transform.parent = tableContent.transform;
			obj.transform.localPosition = new Vector3(0, 0, -10f);
			obj.transform.localScale = Vector3.one;
            obj.name = string.Format("{0:00000}", countDraw);
            obj.transform.GetComponentInChildren<UIAnchor>().GetComponentInChildren<UILabel>().text = Utility.Convert.MessageTime(message.time_sent);
            UISprite line = obj.transform.GetComponentInChildren<UISprite>();
            line.leftAnchor.target = tableContent.transform.parent;
            line.rightAnchor.target = tableContent.transform.parent;
            line.leftAnchor.absolute = 0;
            line.rightAnchor.absolute = 0;
            line.ResetAnchors();
            line.UpdateAnchors();
        }

        countDraw++;
        obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/Message/ProfileMessageContentPrefab"));
        obj.name = string.Format("{0:00000}", countDraw);
        obj.transform.parent = tableContent.transform;
        obj.transform.localPosition = new Vector3(0, 0, -10f);
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<PrefabMessageContentView>().SetData(message);
        Utility.AddCollider(obj);
        obj.gameObject.collider.enabled = false;
        oldDraw = message;
    }

    /// <summary>
    /// SendMessage "Unity Edtior"
    /// </summary>
    /// <param name="content"></param>
    void OnSubmitSendMessage(string content)
    {
        DoRequestSendMessage(content);
    }

    void OnClickSendMessage(GameObject go)
    {
		OnSubmitSendMessage(input.value);
    }
	public void OnSubmitMessage(){
		OnSubmitSendMessage(input.value);
	}
    void DoRequestSendMessage(string contentChat)
    {
        if (string.IsNullOrEmpty(contentChat)) return;

        string receiver_name = listMessage[0].receiver_name == GameManager.Instance.mInfo.username ? listMessage[0].sender_name : listMessage[0].receiver_name;

        if (GameManager.CurrentScene == ESceneName.GameplayChan)
        {
            if (GameModelChan.ListPlayer.Find(p => p.username == receiver_name) != null)
            {
                buttonSendMessage.transform.parent.GetComponentInChildren<UIInput>().value = "";
                NotificationView.ShowMessage("Bạn không thể chát riêng với người đang cùng trong bàn chơi.", 3f);
                return;
            }
        }

        GameManager.Server.DoRequestPlugin(Utility.SetEsObject("sendMessage", new object[] { 
            "receiver", receiver_name, 
            "content", contentChat 
        }));
    }
}
