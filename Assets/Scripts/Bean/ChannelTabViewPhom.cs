using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;

public class ChannelTabViewPhom : MonoBehaviour
{
    #region UnityEditor
    public UIGrid parentListLobby;
    public UIGrid parentListUseOnline;

    public CUIHandle btCreateRoom, btQuickplay;
    public UITexture imageAds;
    public CUITextList textChat;
    public UISprite spriteChannelName;
    public UIPanel panelLobbyRow;
    #endregion

    void Awake()
    {
        CUIHandle.AddClick(imageAds.GetComponent<CUIHandle>(), OnClickAds);
        CUIHandle.AddClick(btCreateRoom, CLickButtonCreate);
        CUIHandle.AddClick(btQuickplay, ClickButtonQuick);

        GameManager.Server.EventPluginMessageOnProcess += OnProcessPluginMessage;
        GameManager.Server.EventJoinRoom += OnAfterJoinRoom;
        GameManager.Server.EventLoadSence += OnJoinGame;
        GameManager.Server.EventPublicMessage += OnPublicMessage;
        GameManager.Instance.applicationStart.EventLoadAnnounce += OnLoadAnnounceDone;
        GameManager.Server.EventAdsChanged += CallBackAdsChangeHandler;
        HeaderMenu.Instance.ReDraw();

        HeaderMenu.Instance.OnClickButtonBackCallBack = delegate()
        {
            WaitingView.Show("Đang thoát");
            Debug.Log("Channel current selected:" + GameManager.Instance.channelRoom.roomId + "," + GameManager.Instance.channelRoom.zoneId);
            GameManager.Server.DoJoinRoom(GameManager.Instance.channelRoom.zoneId, GameManager.Instance.channelRoom.roomId);
        };
       
    }

    private void OnJoinGame()
    {
        GameManager.LoadScene(ESceneName.GameplayPhom);
    }

    void OnDestroy()
    {
        if (imageAds != null)
            CUIHandle.RemoveClick(imageAds.GetComponent<CUIHandle>(), OnClickAds);
        CUIHandle.RemoveClick(btCreateRoom, CLickButtonCreate);

        if (!GameManager.IsExist) return;

        GameManager.Server.EventPluginMessageOnProcess -= OnProcessPluginMessage;
        GameManager.Server.EventJoinRoom -= OnAfterJoinRoom;
        GameManager.Server.EventLoadSence -= OnJoinGame;
        GameManager.Server.EventPublicMessage -= OnPublicMessage;
        GameManager.Instance.applicationStart.EventLoadAnnounce -= OnLoadAnnounceDone;
        GameManager.Server.EventAdsChanged -= CallBackAdsChangeHandler;
    }

    void Start()
    {
        GameManager.Server.DoRequestCommand(Fields.REQUEST.REQUEST_FULL);
        GameManager.Server.DoRequestCommand(Fields.REQUEST.GET_USER_ONLINE);

        //HeaderMenu.Instance.methodCallWhenClickBack = delegate()
        //{
        //    GameManager.Server.DoJoinRoom(GameManager.Instance.channelRoom.zoneId, GameManager.Instance.channelRoom.roomId);
        //};
        //HeaderMenu.Instance.OnClickButtonBackCallBack = delegate()
        //{
        //    WaitingView.Show("Đang thoát");
        //    GameManager.Server.DoJoinRoom(GameManager.Instance.channelRoom.zoneId, GameManager.Instance.channelRoom.roomId);
        //};
        OnLoadAnnounceDone();
        ShowChannelName();
    }

    void OnLoadAnnounceDone()
    {
        LoadImageAds();
    }

    private void CallBackAdsChangeHandler(IDictionary value)
    {
        Announcement ads = GameManager.Instance.ListAnnouncement.Find(a => a.show == Announcement.Scene.lobby && a.type == Announcement.Type.Advertisement);
        LoadImageAds();
    }

    void LoadImageAds()
    {
        Announcement ads = GameManager.Instance.ListAnnouncement.Find(a => a.show == Announcement.Scene.lobby && a.type == Announcement.Type.Advertisement);
        if (ads != null)
        {
            ads.LoadTexture(delegate(Texture texture) { if (imageAds != null) imageAds.mainTexture = texture; });
            imageAds.collider.enabled = true;
        }
        else
            imageAds.collider.enabled = false;
    }

    void OnClickAds(GameObject go)
    {
        Announcement ads = GameManager.Instance.ListAnnouncement.Find(a => a.show == Announcement.Scene.lobby && a.type == Announcement.Type.Advertisement);
        if (ads != null && GameManager.Setting.Platform.EnableRecharge)
            RechargeView.Create();
    }

    void CLickButtonCreate(GameObject go)
    {

        GameManager.Server.DoRequestCommand("tryCreateGame");
    }

    void ClickButtonQuick(GameObject go)
    {
        GameManager.Server.DoRequestCommand("quickJoinGame");
    }

    void OnAfterJoinRoom(JoinRoomEvent e)
    {
        if (e.ZoneId == GameManager.Instance.channelRoom.zoneId && e.RoomId == GameManager.Instance.channelRoom.roomId)
        {
            GameManager.LoadScene(ESceneName.ChannelPhom);
        }
        //else if (e.ZoneId == GameManager.Instance.selectedLobby.zoneId && e.RoomId == GameManager.Instance.selectedLobby.roomId)
        //    GameManager.LoadScene(ESceneName.Gameplay);

    }

    void OnProcessPluginMessage(string command, string action, EsObject paremeters)
    {
        if (command == Fields.RESPONSE.FULL_UPDATE)
        {
            #region Lấy danh sách các lobby sau khi vào room
            LobbyRowPhom.List.Clear();
            EsObject[] children = paremeters.getEsObjectArray("children");

            //UIDragScrollView panel = parentListLobby.transform.parent.gameObject.GetComponent<UIDragScrollView>();

            foreach (EsObject obj in children)
            {
                LobbyPhom lobby = new LobbyPhom(obj);
                LobbyRowPhom.Create(panelLobbyRow, parentListLobby.transform, lobby);
            }
            if (children.Length > 0)
                parentListLobby.repositionNow = true;
            #endregion
        }
        else if (command == Fields.RESPONSE.LOBBY_ADD)
        {
            #region Có một lobby mới được tạo.
            EsObject es = paremeters.getEsObject("child");
            UIDragScrollView panel = parentListLobby.transform.parent.gameObject.GetComponent<UIDragScrollView>();
            LobbyRowPhom.Create(panelLobbyRow,parentListLobby.transform, new LobbyPhom(es));
            parentListLobby.repositionNow = true;
            #endregion
        }
        else if (command == Fields.RESPONSE.LOBBY_UPDATE)
        {
            #region Có một lobby nào đó có thay đổi.
            EsObject es = paremeters.getEsObject("child");
            LobbyRowPhom row = LobbyRowPhom.List.Find(o => o.lobby.gameId == es.getInteger("gameId"));
            if (row != null)
                row.UpdateData(es);
            #endregion
        }
        else if (command == Fields.RESPONSE.LOBBY_REMOVE)
        {
            #region Có một lobby nào đó thoát
            EsObject es = paremeters.getEsObject("child");
            LobbyRowPhom row = LobbyRowPhom.List.Find(o => o.lobby.gameId == es.getInteger("gameId"));
            LobbyRowPhom.Remove(row);
            parentListLobby.repositionNow = true;
            parentListUseOnline.repositionNow = true;
            //parentListLobby.transform.parent.GetComponent<UIDragScrollView>().RestrictWithinBounds(false);
            #endregion
        }
        else if (command == Fields.REQUEST.GET_USER_ONLINE)
        {
            #region Lấy danh sách những người chơi đang online khi vào room
            UserOnlineRowPhom.List.Clear();

            UIDragScrollView panel = parentListUseOnline.transform.parent.gameObject.GetComponent<UIDragScrollView>();
            EsObject[] children = paremeters.getEsObjectArray("users");
            foreach (EsObject obj in children)
            {
                if (obj.getString(Fields.PLAYER.USERNAME) == GameManager.Instance.mInfo.username) continue;
                UserOnlineRowPhom.Create(parentListUseOnline.transform, new User(obj));
            }
            #endregion
        }
        else if (command == Fields.RESPONSE.USER_ONLINE_UPDATE)
        {
            #region Khi có người mới tham gia hoặc thoát ra khởi room
            if (action == "addUserOnline")
            {
                EsObject es = paremeters.getEsObject(Fields.PLAYER.USERNAME);
                if (es.getString(Fields.PLAYER.USERNAME) != GameManager.Instance.mInfo.username)
                {
                    //UIDragScrollView panel = parentListUseOnline.transform.parent.gameObject.GetComponent<UIDragScrollView>();
                    UserOnlineRowPhom.Create(parentListUseOnline.transform, new User(es));
                    parentListLobby.repositionNow = true;
                    parentListUseOnline.repositionNow = true;
                }
            }
            else if (action == "removeUserOnline")
            {
                EsObject es = paremeters.getEsObject(Fields.PLAYER.USERNAME);
                EUserOnlineRow row = UserOnlineRowPhom.List.Find(o => o.user.username == es.getString(Fields.PLAYER.USERNAME));
                if (row != null)
                {
                    UserOnlineRowPhom.Remove(row);
                    parentListLobby.repositionNow = true;
                    parentListUseOnline.repositionNow = true;
                }
            }
            #endregion
        }
        else if (command == "quickJoinGame")
        {
            #region Chơi nhanh
            int gameId = paremeters.getInteger("gameId");
            if (gameId == -1)
                NotificationView.ShowMessage("Hiện không có bàn chơi nào sẵn sàng.", 3f);
            else
            {
                LobbyPhom lobby = LobbyRowPhom.List.Find(lb => lb.lobby.gameId == gameId).lobby;
                GameManager.Instance.selectedLobby = new LobbyPhom(lobby.zoneId, lobby.roomId, lobby.gameId);
                if (PlaySameDevice.IsCanJoinGameplay)
                    GameManager.Server.DoJoinGame("");
            }
            #endregion
        }
        else if (command == "error")
        {
            int id = paremeters.getInteger("error");
            if (id == 0)
                Common.MessageRecharge("Bạn không đủ tiền để tham gia bàn chơi.");
            else if (id == 1)
                NotificationView.ShowMessage("Bàn chơi đã đủ người hoặc đã được thêm máy.");
            else if (id == 2)
                NotificationView.ShowMessage("Bạn đã bị đuổi khỏi bài chơi trước đó.");
            else if (id == 4)
                NotificationView.ShowMessage("Mật khẩu không chính xác.\n\nĐề nghị nhập lại.");
            else if (id == 5)
            {
                string contentMsg = paremeters.getString("textNotification");
                int gameId = paremeters.getInteger("gameId");
                string password = paremeters.variableExists("password") ? paremeters.getString("password") : "";
                NotificationView.ShowConfirm("Xác nhận",
                    contentMsg,
                    delegate()
                    {
                        GameManager.Instance.selectedLobby = new LobbyPhom(gameId);
                        GameManager.Server.DoJoinGame(password);
                    }, null);
            }
        }
        else if (command == "tryCreateGame")
        {
            bool allowCreateRoom = paremeters.getBoolean("allowCreateGame");
            if (allowCreateRoom)
            {
                if (CommonPhom.ValidateChipToBetting(((ChannelPhom)GameManager.Instance.selectedChannel).bettingValues[0]))
                {
                    GameManager.LoadScene(ESceneName.CreateRoomPhom);
                }
                else
                {
                    Common.MessageRecharge("Bạn không đủ tiền để tạo bàn chơi.");
                }
            }
            else
            {
                string contentMsg = paremeters.getString("textNotification");
                int gameId = paremeters.getInteger("gameId");
                string password = paremeters.variableExists("password") ? paremeters.getString("password") : "";
                NotificationView.ShowConfirm("Xác nhận",
                    contentMsg,
                    delegate()
                    {
                        GameManager.Instance.selectedLobby = new LobbyPhom(gameId);
                        GameManager.Server.DoJoinGame(password);
                    }, null);
            }
        }
    }

    /// <summary>
    /// Khi có người chát đến (mình chát gửi đi được viết trong ChatInput
    /// </summary>
    void OnPublicMessage(PublicMessageEvent e)
    {
        if (e.UserName == GameManager.Instance.mInfo.username) return;

        textChat.Add("[#FFCC00]" + e.UserName.ToUpper() + ":[-] " + e.Message + "\n");
        Utility.AutoScrollChat(textChat);
    }

    void ShowChannelName()
    {
        switch (GameManager.Instance.selectedChannel.roomName)
        {
            case "phom_nghiepdu":
                spriteChannelName.spriteName = "amateur";
                break;
            case "phom_chuyennghiep":
                spriteChannelName.spriteName = "pro";
                break;
            case "phom_caothu":
                spriteChannelName.spriteName = "master";
                break;
            case "phom_daigia":
                spriteChannelName.spriteName = "boss";
                break;
        }
    }
}