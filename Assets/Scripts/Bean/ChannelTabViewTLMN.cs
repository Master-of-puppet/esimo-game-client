using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;
public class ChannelTabViewTLMN : MonoBehaviour
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
        GameManager.LoadScene(ESceneName.GameplayTLMN);
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
        parentListUseOnline.repositionNow = true;
    }

    void OnLoadAnnounceDone()
    {
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
            GameManager.LoadScene(ESceneName.ChannelTLMN);
    }

    void OnProcessPluginMessage(string command, string action, EsObject paremeters)
    {
        if (command == Fields.RESPONSE.FULL_UPDATE)
        {
            #region Lấy danh sách các lobby sau khi vào room
            LobbyRowTLMN.List.Clear();
            EsObject[] children = paremeters.getEsObjectArray("children");

            UIScrollView panel = parentListLobby.transform.parent.gameObject.GetComponent<UIScrollView>();

            foreach (EsObject obj in children)
            {
                LobbyTLMN lobby = new LobbyTLMN(obj);
                LobbyRowTLMN.Create(panelLobbyRow,parentListLobby.transform, lobby);
            }
            if (children.Length > 0)
                parentListLobby.repositionNow = true;
            #endregion
        }
        else if (command == Fields.RESPONSE.LOBBY_ADD)
        {
            #region Có một lobby mới được tạo.
            EsObject es = paremeters.getEsObject("child");
            UIScrollView panel = parentListLobby.transform.parent.gameObject.GetComponent<UIScrollView>();
            LobbyRowTLMN.Create(panelLobbyRow,parentListLobby.transform, new LobbyTLMN(es));
            parentListLobby.repositionNow = true;
            #endregion
        }
        else if (command == Fields.RESPONSE.LOBBY_UPDATE)
        {
            #region Có một lobby nào đó có thay đổi.
            EsObject es = paremeters.getEsObject("child");
            LobbyRowTLMN row = LobbyRowTLMN.List.Find(o => o.lobby.gameId == es.getInteger("gameId"));
            if (row != null)
                row.UpdateData(es);
            #endregion
        }
        else if (command == Fields.RESPONSE.LOBBY_REMOVE)
        {
            #region Có một lobby nào đó thoát
            EsObject es = paremeters.getEsObject("child");
            LobbyRowTLMN row = LobbyRowTLMN.List.Find(o => o.lobby.gameId == es.getInteger("gameId"));
            LobbyRowTLMN.Remove(row);
            parentListLobby.repositionNow = true;
            parentListUseOnline.repositionNow = true;
            #endregion
        }
        else if (command == Fields.REQUEST.GET_USER_ONLINE)
        {
            #region Lấy danh sách những người chơi đang online khi vào room
            UserOnlineRowTLMN.List.Clear();

            EsObject[] children = paremeters.getEsObjectArray("users");
            foreach (EsObject obj in children)
            {
                if (obj.getString(Fields.PLAYER.USERNAME) == GameManager.Instance.mInfo.username) continue;
                UserOnlineRowTLMN.Create(parentListUseOnline.transform, new User(obj));
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
                    UserOnlineRowTLMN.Create(parentListUseOnline.transform, new User(es));
                    parentListLobby.repositionNow = true;
                    parentListUseOnline.repositionNow = true;
                }
            }
            else if (action == "removeUserOnline")
            {
                EsObject es = paremeters.getEsObject(Fields.PLAYER.USERNAME);
                EUserOnlineRow row = UserOnlineRowTLMN.List.Find(o => o.user.username == es.getString(Fields.PLAYER.USERNAME));
                if (row != null)
                {
                    UserOnlineRowTLMN.Remove(row);
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
                LobbyTLMN lobby = LobbyRowTLMN.List.Find(lb => lb.lobby.gameId == gameId).lobby;
                GameManager.Instance.selectedLobby = new LobbyTLMN(lobby.zoneId, lobby.roomId, lobby.gameId);
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
                        GameManager.Instance.selectedLobby = new LobbyTLMN(gameId);
                        GameManager.Server.DoJoinGame(password);
                    }, null);
            }
        }
        else if (command == "tryCreateGame")
        {
            bool allowCreateRoom = paremeters.getBoolean("allowCreateGame");
            if (allowCreateRoom)
            {
                if (CommonTLMN.ValidateChipToBetting(((ChannelTLMN)GameManager.Instance.selectedChannel).bettingValues[0]))
                {
                    GameManager.LoadScene(ESceneName.CreateRoomTLMN);
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
                        GameManager.Instance.selectedLobby = new LobbyTLMN(gameId);
                        GameManager.Server.DoJoinGame(password);
                    }, null);
            }
        }
    }

    public void showNameChannel()
    {
        ChannelTLMN.ChannelType type = ((ChannelTLMN)GameManager.Instance.selectedChannel).type;
        transform.FindChild("channel").GetComponent<UISprite>().spriteName = type == ChannelTLMN.ChannelType.Amateur ? "nendat"
            : type == ChannelTLMN.ChannelType.Professional ? "chieucoi"
            : type == ChannelTLMN.ChannelType.Experts ? "phango"
            : type == ChannelTLMN.ChannelType.Giants ? "sapgu" : "chieuvuong";
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
            case "tilm_nghiepdu":
                spriteChannelName.spriteName = "amateur";
                break;
            case "tlmn_chuyennghiep":
                spriteChannelName.spriteName = "pro";
                break;
            case "tlmn_caothu":
                spriteChannelName.spriteName = "master";
                break;
            case "tlmn_daigia":
                spriteChannelName.spriteName = "boss";
                break;
        }
    }
}