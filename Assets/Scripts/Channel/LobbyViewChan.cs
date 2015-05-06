using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;

public class LobbyViewChan : MonoBehaviour
{
    #region UnityEditor
    public UIGrid parentListLobby;
    public UIGrid parentListUseOnline;
    public CUIHandle btCreateRoom, btQuickplay, btFilter;
    public CUITextList textChat;
    public UIDragScrollView lobbyDrag, userOnlineDrag;
    public UITabbarController controller;
    public GameObject containerFilter;
    public GameObject btnUserOnline;
    public UIPanel panelLobbyRow;
    #endregion
    ChannelChan channelSelected { get { return (ChannelChan)GameManager.Instance.selectedChannel; } }


    private List<LobbyChan> lobbies = new List<LobbyChan>();
    bool isFiltered = false;
    void Awake()
    {

        CUIHandle.AddClick(btCreateRoom, CLickButtonCreate);
        CUIHandle.AddClick(btQuickplay, ClickButtonQuick);
        CUIHandle.AddClick(btFilter, OnClickButtonFilter);
        GameManager.Server.EventPluginMessageOnProcess += OnProcessPluginMessage;
        GameManager.Server.EventJoinRoom += OnAfterJoinRoom;
        GameManager.Server.EventPublicMessage += OnPublicMessage;
        GameManager.Server.EventLoadSence += OnJoinGame;
        //GameManager.Instance.applicationStart.EventLoadAnnounce += OnLoadAnnounceDone;
        GameManager.Server.EventAdsChanged += CallBackAdsChangeHandler;
        //BroadcastView.Instance.ShowInLobby(GameManager.Setting.BroadcastMessage);

        Debug.Log("Channel selected:" + GameManager.Instance.selectedChannel.roomId + "," + GameManager.Instance.selectedChannel.zoneId); ;
        Debug.Log("Channel chan:" + GameManager.Instance.channelRoom.roomId + "," + GameManager.Instance.channelRoom.zoneId);

        HeaderMenu.Instance.OnClickButtonBackCallBack = delegate()
        {
            WaitingView.Show("Đang thoát");
            Debug.Log("Channel current selected:" + GameManager.Instance.channelRoom.roomId + "," + GameManager.Instance.channelRoom.zoneId);
            GameManager.Server.DoJoinRoom(GameManager.Instance.channelRoom.zoneId, GameManager.Instance.channelRoom.roomId);
        };
        // get name channel
        showNameChannel();
        controller.OnTabbarSelectEvent += OnTabbarSelectHandler;
        containerFilter.GetComponent<LobbyFilter>().FilterCallBack += FilterHandler;
        containerFilter.GetComponent<LobbyFilter>().CancelFilterCallBack += CancelFilterHandler;
    }

    private void OnJoinGame()
    {
        GameManager.LoadScene(ESceneName.GameplayChan);
    }
    private void FilterHandler()
    {
        List<Criteria> criterias = containerFilter.GetComponent<LobbyFilter>().criterias;
        if (criterias.Count == 0 && isFiltered)
        {
            isFiltered = false;
            OnReDrawLobby(lobbies);
        }
        if (criterias.Count != 0)
        {
            if (!isFiltered)
                isFiltered = true;
            List<Dictionary<string, object>> dictLobby = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> dictCriteria = new List<Dictionary<string, object>>();
            foreach (LobbyChan lobby in lobbies)
            {
                dictLobby.Add(lobby.ToDictionary());
            }
            foreach (Criteria cri in criterias)
            {
                dictCriteria.Add(cri.iDict);
            }
            List<Dictionary<string, object>> dictFilter = Utility.ComparableCriteria.filterByCriterias(dictLobby, dictCriteria);
            List<LobbyChan> lobbiesFilter = new List<LobbyChan>();
            foreach (Dictionary<string, object> lobby in dictFilter)
            {
                lobbiesFilter.Add((LobbyChan)lobby[Fields.LobbyFilter.OBJECT_REFRENCE]);
            }
            OnReDrawLobby(lobbiesFilter);
        }
    }

    private void CancelFilterHandler()
    {
        if (isFiltered)
        {
            isFiltered = false;
            containerFilter.GetComponent<LobbyFilter>().isButtonCancelClick = true;
            OnReDrawLobby(lobbies);
        }

    }
    private void FilterWhenLobbyAdded(LobbyChan lobby)
    {
        List<Criteria> criterias = containerFilter.GetComponent<LobbyFilter>().criterias;
        List<Dictionary<string, object>> dictLobby = new List<Dictionary<string, object>>();
        List<Dictionary<string, object>> dictCriteria = new List<Dictionary<string, object>>();
        dictLobby.Add(lobby.ToDictionary());
        foreach (Criteria cri in criterias)
        {
            dictCriteria.Add(cri.iDict);
        }
        List<Dictionary<string, object>> dictFilter = Utility.ComparableCriteria.filterByCriterias(dictLobby, dictCriteria);
        List<LobbyChan> lobbiesFilter = new List<LobbyChan>();
        foreach (Dictionary<string, object> lb in dictFilter)
        {
            LobbyRow.Create(panelLobbyRow,parentListLobby.transform, (LobbyChan)lb[Fields.LobbyFilter.OBJECT_REFRENCE]);
        }
        parentListLobby.Reposition();
    }
    private void InitComponent()
    {
        //RedrawUIGridInScene();
        lobbyDrag.collider.enabled = false;
        userOnlineDrag.collider.enabled = false;
    }

    private void RedrawUIGrid(UIGrid grid)
    {
        UIPanel panel = grid.transform.parent.GetComponent<UIPanel>();
        grid.maxPerLine = Mathf.FloorToInt(panel.baseClipRegion.z / grid.cellWidth);
        grid.cellWidth = grid.cellWidth + (panel.baseClipRegion.z - (grid.cellWidth * grid.maxPerLine)) / grid.maxPerLine;
        grid.Reposition();
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(btCreateRoom, CLickButtonCreate);
        CUIHandle.RemoveClick(btQuickplay, ClickButtonQuick);
        CUIHandle.RemoveClick(btFilter, OnClickButtonFilter);
        containerFilter.GetComponent<LobbyFilter>().FilterCallBack -= FilterHandler;
        containerFilter.GetComponent<LobbyFilter>().CancelFilterCallBack -= CancelFilterHandler;
        if (!GameManager.IsExist) return;

        GameManager.Server.EventPluginMessageOnProcess -= OnProcessPluginMessage;
        GameManager.Server.EventLoadSence -= OnJoinGame;
        GameManager.Server.EventJoinRoom -= OnAfterJoinRoom;
        GameManager.Server.EventPublicMessage -= OnPublicMessage;
        //GameManager.Instance.applicationStart.EventLoadAnnounce -= OnLoadAnnounceDone;
        GameManager.Server.EventAdsChanged -= CallBackAdsChangeHandler;
        controller.OnTabbarSelectEvent += OnTabbarSelectHandler;
    }
    bool isReDrawUserOnline = false;
    private void OnTabbarSelectHandler(int index)
    {
        switch (index)
        {
            case 0:
                btFilter.gameObject.SetActive(true);
                break;
            case 1:
                btFilter.gameObject.SetActive(false);
                if (isReDrawUserOnline) return;
                isReDrawUserOnline = true;
                RedrawUIGrid(parentListUseOnline);
                break;
        }

    }

    private void OnClickButtonFilter(GameObject targetObject)
    {
        //NotificationView.ShowMessage("Tính năng đang được phát triển mời bạn quay lại sau ", 3f);
        if (!containerFilter.activeSelf)
            containerFilter.SetActive(true);
        NGUITools.SetActive(btnUserOnline, false);
    }

    void Start()
    {
        //remove list waiting view
        WaitingView[] waiting = GameObject.FindObjectsOfType<WaitingView>();
        foreach (var wait in waiting)
        {
            wait.Close();
        }

        GameManager.Server.DoRequestCommand(Fields.REQUEST.REQUEST_FULL);
        GameManager.Server.DoRequestCommand(Fields.REQUEST.GET_USER_ONLINE);
        InitComponent();
        //addData();
    }

    private void CallBackAdsChangeHandler(IDictionary value)
    {
        Announcement ads = GameManager.Instance.ListAnnouncement.Find(a => a.show == Announcement.Scene.lobby && a.type == Announcement.Type.Advertisement);
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
            GameManager.LoadScene(ESceneName.ChannelChan);
        }
        //else if (e.ZoneId == GameManager.Instance.selectedLobby.zoneId && e.RoomId == GameManager.Instance.selectedLobby.roomId)
        //    GameManager.LoadScene(ESceneName.Gameplay);
    }
    public void addData()
    {

        LobbyChan[] lbby = new LobbyChan[10];
        lbby[0] = new LobbyChan(50, false, "Chắn vương ở đâu thì bơi vào đây", 1, 4, 20, true, false, 1);
        LobbyChan.GameConfig config = new LobbyChan.GameConfig(1, 1, true, 20, true);
        lbby[0].config = config;

        lbby[1] = new LobbyChan(200, false, "Chắn vương ở đâu thì bơi vào đây", 2, 4, 20, true, true, 2);
        LobbyChan.GameConfig config1 = new LobbyChan.GameConfig(2, 1, true, 20, true);
        lbby[1].config = config1;

        lbby[2] = new LobbyChan(50, false, "Chắn vương ở đâu thì bơi vào đây", 3, 4, 20, true, false, 0);
        LobbyChan.GameConfig config2 = new LobbyChan.GameConfig(1, 1, true, 20, true);
        lbby[2].config = config2;

        lbby[3] = new LobbyChan(50, false, "Chắn vương ở đâu thì bơi vào đây", 3, 4, 20, true, false, 1);
        LobbyChan.GameConfig config3 = new LobbyChan.GameConfig(3, 1, true, 20, true);
        lbby[3].config = config3;

        lbby[4] = new LobbyChan(50, false, "Chắn vương ở đâu thì bơi vào đây", 2, 4, 20, false, true, 1);
        LobbyChan.GameConfig config4 = new LobbyChan.GameConfig(2, 1, true, 20, true);
        lbby[4].config = config4;

        lbby[5] = new LobbyChan(50, false, "Chắn vương ở đâu thì bơi vào đây", 3, 4, 20, true, true, 2);
        LobbyChan.GameConfig config5 = new LobbyChan.GameConfig(1, 1, true, 20, true);
        lbby[5].config = config5;

        lbby[6] = new LobbyChan(200, false, "Chắn vương ở đâu thì bơi vào đây", 2, 4, 20, true, true, 0);
        LobbyChan.GameConfig config6 = new LobbyChan.GameConfig(3, 1, true, 20, true);
        lbby[6].config = config6;

        lbby[7] = new LobbyChan(50, false, "Chắn vương ở đâu thì bơi vào đây", 3, 4, 20, true, true, 1);
        LobbyChan.GameConfig config7 = new LobbyChan.GameConfig(1, 1, true, 20, true);
        lbby[7].config = config7;

        lbby[8] = new LobbyChan(500, false, "Chắn vương ở đâu thì bơi vào đây", 1, 4, 20, true, true, 0);
        LobbyChan.GameConfig config8 = new LobbyChan.GameConfig(2, 1, true, 20, true);
        lbby[8].config = config8;

        lbby[9] = new LobbyChan(100, false, "Chắn vương ở đâu thì bơi vào đây", 3, 4, 20, true, true, 2);
        LobbyChan.GameConfig config9 = new LobbyChan.GameConfig(3, 1, true, 20, true);
        lbby[9].config = config9;

        for (int i = 0; i < lbby.Length; i++)
        {
            test(lbby[i]);
            lobbies.Add(lbby[i]);
        }
        parentListLobby.Reposition();
    }

    public void test(LobbyChan lb)
    {

        LobbyRow.Create(panelLobbyRow,parentListLobby.transform, lb);

    }

    void OnReDrawLobby(List<LobbyChan> lobbi)
    {
        StartCoroutine(_OnReDrawLobby(lobbi));
    }

    IEnumerator _OnReDrawLobby(List<LobbyChan> lobbi)
    {
        while (LobbyRow.List.Count > 0)
        {
            LobbyRow.Remove(LobbyRow.List[0]);
        }
        parentListLobby.Reposition();
        yield return new WaitForEndOfFrame();
        foreach (LobbyChan lobby in lobbi)
        {
            LobbyRow.Create(panelLobbyRow,parentListLobby.transform, lobby);
        }
        lobbyDrag.collider.enabled = false;
        lobbyDrag.collider.enabled = true;
        parentListLobby.Reposition();
        if (containerFilter.GetComponent<LobbyFilter>().isButtonCancelClick)
            containerFilter.GetComponent<LobbyFilter>().isButtonCancelClick = !containerFilter.GetComponent<LobbyFilter>().isButtonCancelClick;
    }

    void OnProcessPluginMessage(string command, string action, EsObject paremeters)
    {
        if (command == Fields.RESPONSE.FULL_UPDATE)
        {
            #region Lấy danh sách các lobby sau khi vào room
            LobbyRow.List.Clear();
            lobbies.Clear();
            EsObject[] children = paremeters.getEsObjectArray("children");
            foreach (EsObject obj in children)
            {
                LobbyChan lobby = new LobbyChan(obj);
                lobbies.Add(lobby);
                LobbyRow.Create(panelLobbyRow,parentListLobby.transform, lobby);
            }
            if (children.Length > 0)
            {
                parentListLobby.Reposition();
            }
            parentListLobby.transform.parent.GetComponent<UIScrollView>().ResetPosition();
            RedrawUIGrid(parentListLobby);
            lobbyDrag.collider.enabled = true;
            //GameManager.Instance.FunctionDelay(delegate() { RedrawUIGrid(parentListLobby); lobbyDrag.collider.enabled = true; }, 0.01f);
            #endregion

        }
        else if (command == Fields.RESPONSE.LOBBY_ADD)
        {
            #region Có một lobby mới được tạo.
            EsObject es = paremeters.getEsObject("child");
            LobbyChan lobby = new LobbyChan(es);
            lobbies.Add(lobby);
            if (!isFiltered)
            {
                LobbyRow.Create(panelLobbyRow,parentListLobby.transform, lobby);
                parentListLobby.Reposition();
            }
            else
            {
                FilterWhenLobbyAdded(lobby);
            }
            lobbyDrag.collider.enabled = false;
            lobbyDrag.collider.enabled = true;
            #endregion
        }
        else if (command == Fields.RESPONSE.LOBBY_UPDATE)
        {
            #region Có một lobby nào đó có thay đổi.
            EsObject es = paremeters.getEsObject("child");
            LobbyRow row = LobbyRow.List.Find(o => o.lobby.gameId == es.getInteger("gameId"));
            if (row != null)
                row.UpdateData(new LobbyChan(es));
            #endregion
        }
        else if (command == Fields.RESPONSE.LOBBY_REMOVE)
        {
            #region Có một lobby nào đó thoát
            EsObject es = paremeters.getEsObject("child");
            LobbyRow row = LobbyRow.List.Find(o => o.lobby.gameId == es.getInteger("gameId"));
            int index = lobbies.FindIndex(o => o.gameId == es.getInteger("gameId"));
            if (index != -1)
                lobbies.RemoveAt(index);
            LobbyRow.Remove(row);
            parentListLobby.Reposition();
            #endregion
        }
        else if (command == Fields.REQUEST.GET_USER_ONLINE)
        {
            #region Lấy danh sách những người chơi đang online khi vào room
            UserOnlineRowChan.List.Clear();

            EsObject[] children = paremeters.getEsObjectArray("users");
            foreach (EsObject obj in children)
            {
                if (obj.getString(Fields.PLAYER.USERNAME) == GameManager.Instance.mInfo.username) continue;
                UserOnlineRowChan.Create(parentListUseOnline.transform, new User(obj));
            }
            if (children.Length > 0)
            {
                parentListUseOnline.repositionNow = true;
            }

            parentListUseOnline.transform.parent.GetComponent<UIScrollView>().ResetPosition();
            RedrawUIGrid(parentListUseOnline);
            userOnlineDrag.collider.enabled = true;
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
                    UserOnlineRowChan.Create(parentListUseOnline.transform, new User(es));
                    parentListLobby.repositionNow = true;
                    parentListUseOnline.repositionNow = true;
                }
                userOnlineDrag.collider.enabled = false;
                userOnlineDrag.collider.enabled = true;
            }
            else if (action == "removeUserOnline")
            {
                EsObject es = paremeters.getEsObject(Fields.PLAYER.USERNAME);
                EUserOnlineRow row = UserOnlineRowChan.List.Find(o => o.user.username == es.getString(Fields.PLAYER.USERNAME));
                if (row != null)
                {
                    UserOnlineRowChan.Remove(row);
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

                LobbyChan lobby = lobbies.Find(lb => lb.gameId == gameId);
                GameManager.Instance.selectedLobby = new LobbyChan(lobby.zoneId, lobby.roomId, lobby.gameId);
                if (PlaySameDevice.IsCanJoinGameplay)
                    GameManager.Server.DoJoinGame("");
            }
            #endregion
        }
        else if (command == "error")
        {
            WaitingView.Instance.Close();
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
                        GameManager.Instance.selectedLobby = new LobbyChan(gameId);
                        GameManager.Server.DoJoinGame(password);
                    }, null);
            }
        }
        else if (command == "tryCreateGame")
        {
            bool allowCreateRoom = paremeters.getBoolean("allowCreateGame");
            if (allowCreateRoom)
            {
                if (Common.ValidateChipToBetting(((ChannelChan)GameManager.Instance.selectedChannel).bettingValues[0], GameManager.PlayGoldOrChip))
                {
                    GameManager.LoadScene(ESceneName.CreateRoomChan);
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
                        GameManager.Instance.selectedLobby = new LobbyChan(gameId);
                        GameManager.Server.DoJoinGame(password);
                    }, null);
            }
        }
    }

    public void showNameChannel()
    {
        ChannelChan.ChannelType type = channelSelected.type;
        GameObject.Find("Selected Channel").GetComponent<UISprite>().spriteName = type == ChannelChan.ChannelType.Amateur ? "nendat"
            : type == ChannelChan.ChannelType.Professional ? "chieucoi"
            : type == ChannelChan.ChannelType.Experts ? "phango"
            : type == ChannelChan.ChannelType.Giants ? "sapgu" : "chieuvuong";
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
}
