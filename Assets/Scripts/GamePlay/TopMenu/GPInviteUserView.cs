using Electrotank.Electroserver5.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class GPInviteUserView : MonoBehaviour
{
    #region Unity Editor
    public UITable tableInvite;
    public CUIHandle btnInvite,btnAddRobot;
    public UILabel lbTitle;
    bool hadRequest = false;
    #endregion

    void OnEnable()
    {
        GameManager.Server.EventPluginMessageOnProcess += OnPluginMessageOnProcess;
        GameManager.Server.EventGameplayUpdateUserOnline += OnPluginUpdateUserOnline;
        if (hadRequest){
            GameManager.Server.DoRequestPlugin(Utility.SetEsObject(Fields.REQUEST.GET_USER_ONLINE, new object[] {
			    "zoneId", GameManager.Instance.selectedChannel.zoneId,
			    "roomId", GameManager.Instance.selectedChannel.roomId,
		    }));
        }

    }
    void OnDisable() 
    {
        GameManager.Server.EventPluginMessageOnProcess -= OnPluginMessageOnProcess;
        GameManager.Server.EventGameplayUpdateUserOnline -= OnPluginUpdateUserOnline;
    }
    void Start()
    {
        GameManager.Server.DoRequestPlugin(Utility.SetEsObject(Fields.REQUEST.GET_USER_ONLINE, new object[] {
            "zoneId", GameManager.Instance.selectedChannel.zoneId,
            "roomId", GameManager.Instance.selectedChannel.roomId,
        }));
        hadRequest = true;
        CUIHandle.AddClick(btnAddRobot, OnClickButtonAddRobot);
        CUIHandle.AddClick(btnInvite, OnClickButtonInvite);
        if (GameManager.PlayGoldOrChip == "chip")
            lbTitle.text = "Chip hiện tại";
        else if (GameManager.PlayGoldOrChip == "gold")
            lbTitle.text = "Gold hiện tại";
    }

    void Update()
    {
        if (GameManager.CurrentScene == ESceneName.GameplayChan)
            btnAddRobot.gameObject.SetActive(GameModelChan.NumberBot < ((LobbyChan)GameManager.Instance.selectedLobby).numberRobotAllowed);
        else if (GameManager.CurrentScene == ESceneName.GameplayPhom)
            btnAddRobot.gameObject.SetActive(GameModelPhom.NumberBot < ((LobbyPhom)GameManager.Instance.selectedLobby).numberRobotAllowed);
        else if (GameManager.CurrentScene == ESceneName.GameplayTLMN)
            btnAddRobot.gameObject.SetActive(GameModelTLMN.NumberBot < ((LobbyTLMN)GameManager.Instance.selectedLobby).numberRobotAllowed);
    }
    private void OnClickButtonAddRobot(GameObject targetObject)
    {
        //Debug.Log("number Bot " + GameModelChan.NumberBot + " number allow " + ((LobbyChan)GameManager.Instance.selectedLobby).numberRobotAllowed); 
        GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("addRobot"));   
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnInvite, OnClickButtonInvite);
        CUIHandle.RemoveClick(btnAddRobot, OnClickButtonAddRobot);
    }
    public void OnClickButtonInvite(GameObject targetObject)
    {
        List<string> lstStr = new List<string>();
        listUserOnline.ForEach(u =>
        {
            if (u.value) lstStr.Add(u.mUser.username);
        });

        if (lstStr.Count > 0)
        {
            GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("invitePlayGame", new object[] {
                "invitedUsers", lstStr.ToArray(),
                "zoneId", GameManager.Instance.selectedChannel.zoneId,
                "roomId", GameManager.Instance.selectedChannel.roomId
            }));
			for(int i = 0; i< tableInvite.transform.childCount;i++)
			{
				tableInvite.transform.GetChild(i).GetComponent<UIToggle>().value = false;
			}
            NotificationView.ShowMessage("Đã gửi lời mời thành công", 3f);
        }
        else
        {
            NotificationView.ShowMessage("Bạn chưa chọn một người chơi nào để mời ",3f);
        }
    }

    #region PROCESS USERS ONLINE
    /// <summary>
    /// Danh sách obj các người chơi online
    /// </summary>
    [HideInInspector]
    public List<GPUserOnlineRow> listUserOnline = new List<GPUserOnlineRow>();

    /// <summary>
    /// Danh sách khi request lên lấy về các user
    /// </summary>
    List<User> lstUser = new List<User>();

    /// <summary>
    /// Xử lý khi request lên server để lấy về danh sách những người đang online trong Lobby ngoài
    /// </summary>
    public void OnPluginMessageOnProcess(string command, string action, Electrotank.Electroserver5.Api.EsObject eso)
    {
        if (command == Fields.REQUEST.GET_USER_ONLINE)
        {
            #region Lấy danh sách những người chơi đang online khi vào room
            Electrotank.Electroserver5.Api.EsObject[] children = eso.getEsObjectArray(Fields.PLAYER.USERS);

            lstUser.Clear();
            System.Array.ForEach<Electrotank.Electroserver5.Api.EsObject>(children, u => lstUser.Add(new User(u)));
            createListUserOnline(lstUser);

            #endregion
        }
    }

    /// <summary>
    /// Xủ lý khi có sự thay đổi về danh sách người online, thêm người mới vào lobby, có người thoát ra khỏi lobby
    /// </summary>
    public void OnPluginUpdateUserOnline(string command, string action, EsObject eso)
    {
        if (command == Fields.RESPONSE.USER_ONLINE_UPDATE)
        {
            #region Khi có người mới tham gia hoặc thoát ra khởi room
            EsObject es = eso.getEsObject(Fields.PLAYER.USERNAME);
            User user = new User(es);
            if (action == "addUserOnline")
            {
                if (es.getString(Fields.PLAYER.USERNAME) != GameManager.Instance.mInfo.username)
                {
                    lstUser.Add(user);
                    DrawListUserOnline(user, ++i);
					tableInvite.Reposition();
                }
            }
            else if (action == "removeUserOnline")
            {
                User us = lstUser.Find(u => u.username == user.username);
                if (lstUser.Contains(us))
                    lstUser.Remove(us);

                GPUserOnlineRow rowUser = listUserOnline.Find(o => o.mUser.username == es.getString(Fields.PLAYER.USERNAME));

                if (rowUser != null)
                {
                    listUserOnline.Remove(rowUser);
                    GameObject.Destroy(rowUser.gameObject);
                    tableInvite.repositionNow = true;
                }

            }
            #endregion
        }
    }

    /// <summary>
    /// Khi nhập nội dung tìm kiếm bạn bè
    /// </summary>
    void OnFindFriendTextChanged(string content)
    {
        createListUserOnline(string.IsNullOrEmpty(content) ? lstUser : lstUser.FindAll(u => u.username.IndexOf(content) >= 0));
    }

    /// <summary>
    /// Khi click tìm bạn bè
    /// </summary>
    void OnClickClearSearchFriend(GameObject go)
    {
        OnFindFriendTextChanged("");
    }

    void createListUserOnline(List<User> listUser)
    {
        StartCoroutine(_createListUserOnline(listUser));
    }
    int i = 0;
    IEnumerator _createListUserOnline(List<User> listUser)
    {
        while (listUserOnline.Count > 0)
        {
            GameObject.Destroy(listUserOnline[0].gameObject);
            listUserOnline.RemoveAt(0);
        }

        tableInvite.Reposition();
        yield return new WaitForEndOfFrame();

        foreach (User u in listUser)
        {
            if (u.username == GameManager.Instance.mInfo.username) continue;
            DrawListUserOnline(u, ++i);
        }
        tableInvite.Reposition();
    }

    void DrawListUserOnline(User u, int index)
    {
        GPUserOnlineRow userOnline = GPUserOnlineRow.Create(index, tableInvite.transform, u, this);
        listUserOnline.Add(userOnline);
    }
    #endregion

}

