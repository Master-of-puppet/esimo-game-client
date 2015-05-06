using System;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;
using UnityEngine;
using System.Collections;
public class ChannelListViewChan : MonoBehaviour
{
    #region UnityEditor
    /// <summary>
    /// Vị trí các Channel và thứ tự các Channel
    /// </summary>
    public List<CUIHandle> listButton;
    //public List<UILabel> listChip;

    //Info user
    public UILabel username, chip, gold, level;
    public UISlider processbar;
    public UITexture textureAvatar;

    //Button
    public CUIHandle btnChip, btnGold;

    public List<UISprite> icon;
    public List<UISprite> background;
    public UITabbarController tabIcon;


    #endregion

    string GOLD = "gold";
    string CHIP = "chip";
    /// <summary>
    /// Danh Sách Các Phòng đấu (Chuyên nghiệp, nghiệp dư ...)
    /// </summary>
    List<ChannelChan> listChannel = new List<ChannelChan>();

    void Awake()
    {
        GameManager.Server.EventUpdateUserInfo += OnUpdateUserInfo;
        CUIHandle.AddClick(listButton.ToArray(), OnClickButtonChannel);
        CUIHandle.AddClick(btnChip, OnClickButtonChip);
        CUIHandle.AddClick(btnGold, OnClickButtonGold);

        GameManager.Server.EventPluginMessageOnProcess += OnProcessPluginMessage;
        GameManager.Server.EventJoinRoom += OnAfterJoinRoom;
        HeaderMenu.Instance.ReDraw();

        BroadcastView.Instance.ShowInChannel(GameManager.Setting.BroadcastMessage);
        new AfterJoinChannel();
    }

    void OnDestroy()
    {
        GameManager.Server.EventUpdateUserInfo -= OnUpdateUserInfo;
        CUIHandle.RemoveClick(listButton.ToArray(), OnClickButtonChannel);
        CUIHandle.RemoveClick(btnChip, OnClickButtonChip);
        CUIHandle.RemoveClick(btnGold, OnClickButtonGold);
        if (!GameManager.IsExist) return;

        GameManager.Server.EventPluginMessageOnProcess -= OnProcessPluginMessage;
        GameManager.Server.EventJoinRoom -= OnAfterJoinRoom;
    }

    void OnClickButtonChannel(GameObject go)
    {
        if (listChannel.Count > 0)
        {
            //GameManager.Instance.selectedChannel = listChannel[go.GetComponent<UIContainerAnonymous>().valueInt - 1];
            GameManager.Instance.selectedChannel = listChannel.FindAll(c => c.moneyType == GameManager.PlayGoldOrChip || string.IsNullOrEmpty(c.moneyType)).Find(x => x.roomName.StartsWith(go.GetComponent<UIContainerAnonymous>().valueString));

            Debug.Log(GameManager.Instance.selectedChannel.roomName);
            GameManager.Server.DoJoinRoom(GameManager.Instance.selectedChannel.zoneId, GameManager.Instance.selectedChannel.roomId);
        }
    }
    void SetIcon()
    {
        if (GameManager.PlayGoldOrChip == CHIP) //[0] = gold // [1] = chip
        {
            icon[1].spriteName = "IconChip2";
            icon[0].spriteName = "IconMoney2";
            background[1].spriteName = "tab_selected";
            background[0].spriteName = "tab_normal";
            tabIcon.selectedIndex = 1;
        }
        else
        {
            icon[1].spriteName = "IconChip";
            icon[0].spriteName = "IconMoney";
            background[1].spriteName = "tab_normal";
            background[0].spriteName = "tab_selected";
            tabIcon.selectedIndex = 0;
        }
    }
    void OnClickButtonChip(GameObject go)
    {
        GameManager.PlayGoldOrChip = CHIP;
        OnDrawListChannel();
        SetIcon();
    }
    void OnClickButtonGold(GameObject go)
    {
        //GameManager.PlayGoldOrChip = GOLD;
        //OnDrawListChannel();
        //SetIcon();
        NotificationView.ShowMessage("Chức năng đang được xây dựng, vui lòng quay lại sau", 3f);
    }
    public void OnUpdateUserInfo()
    {
        OnDrawListChannel();
        GetDataUserInfo();
    }
    void GetDataUserInfo()
    {
        username.text = Utility.Convert.ToTitleCase(GameManager.Instance.mInfo.username.Length > 20 ? GameManager.Instance.mInfo.username.Substring(0, 20) + "..." : GameManager.Instance.mInfo.username);
        chip.text = Utility.Convert.Chip(GameManager.Instance.mInfo.chip);
        gold.text = Utility.Convert.Chip(GameManager.Instance.mInfo.gold);
        //level.text = (level == null ? "LV " : "") + GameManager.Instance.mInfo.level;
        level.text = "LV " + GameManager.Instance.mInfo.level;
        GameManager.Instance.mInfo.AvatarTexture(delegate(Texture _texture) { if (textureAvatar != null) textureAvatar.mainTexture = _texture; });
    }
    void Start()
    {
        if (GameManager.PlayGoldOrChip != GOLD && GameManager.PlayGoldOrChip != CHIP)
            GameManager.PlayGoldOrChip = GOLD;
        GameManager.Server.DoRequestCommand(Fields.REQUEST.REQUEST_FULL);
        if (GameManager.Setting.IsFirstLogin)
        {
            GameManager.Setting.IsFirstLogin = false;
            NotificationView.ShowConfirm(
                Fields.MESSAGE.FIRST_LOGIN_NOTE, Fields.MESSAGE.FIRST_LOGIN_MESSAGE,
                delegate()
                {
                    StoreGame.SaveInt(StoreGame.EType.CHANGE_INFORMATION, 1);
                    ProfileView.Instance.CheckWhenStart();
                }
                , null, "CẬP NHẬT", "ĐỂ SAU"
            );
        }
        GetDataUserInfo();
        SetIcon();

        HeaderMenu.Instance.OnClickButtonBackCallBack = delegate()
        {
            GameManager.Server.DoJoinRoom(GameManager.Instance.hallRoom.zoneId, GameManager.Instance.hallRoom.roomId);
        };

    }


    void OnAfterJoinRoom(JoinRoomEvent e)
    {
        if (e.RoomName.Trim() == "chan_giaidau")
        {
            GameManager.Instance.currentRoomGiaiDau = new RoomInfo(e.ZoneId, e.RoomId);
            GameManager.LoadScene(ESceneName.ChannelLeague);
        }
        else if (e.RoomName == "hall")
            GameManager.LoadScene(ESceneName.HallSceen);
        else
            GameManager.LoadScene(ESceneName.LobbyChan);
    }

    void OnProcessPluginMessage(string command, string action, EsObject paremeters)
    {
        if (command == Fields.RESPONSE.FULL_UPDATE)
        {
            EsObject[] children = paremeters.getEsObjectArray("children");
            listChannel.Clear();
            foreach (EsObject obj in children)
                listChannel.Add(new ChannelChan(obj));
            OnDrawListChannel();
        }
    }

    List<GameObject> OpaticalObject = new List<GameObject>();
    public void OnDrawListChannel()
    {
        //money type=="" la giai dau
        List<ChannelChan> channelSelect = listChannel.FindAll(c => c.moneyType == GameManager.PlayGoldOrChip || string.IsNullOrEmpty(c.moneyType));
        //disable all button
        for (int i = 0; i < listButton.Count; i++)
        {
            DisableChannelButton(listButton[i].gameObject);
            NumberUserInChannel number = listButton[i].GetComponentInChildren<NumberUserInChannel>();
            if (number)
            {
                number.lblDecription.text = "";
                number.SetValue(0);
            }
        }

        if (channelSelect.Count == 0)
            return;

        for (int i = 0; i < channelSelect.Count; i++)
        {
            //GameObject button = listButton.Find(x => x.GetComponent<UIContainerAnonymous>().valueString == channelSelect[i].roomName).gameObject;
            GameObject button = listButton.Find(x => channelSelect[i].roomName.StartsWith(x.GetComponent<UIContainerAnonymous>().valueString)).gameObject;

            if (string.IsNullOrEmpty(channelSelect[i].moneyType) || channelSelect[i].minimumMoney <= (GameManager.PlayGoldOrChip == "gold" ? GameManager.Instance.mInfo.gold : GameManager.Instance.mInfo.chip))
            {
                if (button)
                    EnableChannelButton(button);
            }
            int index = checkNumberUser(channelSelect[i]);
            if (i == 4) // Làm mờ nút giải đấu
                DisableChannelButton(button);
            if (button)
            {
                NumberUserInChannel number = button.GetComponentInChildren<NumberUserInChannel>();
                if (number != null)
                {
                    number.SetValue(index);
                    string text = GameManager.PlayGoldOrChip == "chip" ? "Chip >" : "Gold >";
                    text += Utility.Convert.Chip(channelSelect[i].minimumMoney);
                    if (channelSelect[i].minimumMoney == 0)
                        text = "Phòng tự do";
                    number.lblDecription.text = text;
                }

            }

        }
    }

    public void EnableChannelButton(GameObject transform)
    {
        if (transform.gameObject.GetComponentInChildren<UILabel>() != null)
        {
            Color c = transform.gameObject.GetComponentInChildren<UILabel>().color;
            c.a = 255f / 255f;
            transform.gameObject.GetComponentInChildren<UILabel>().color = c;
        }
        Color gameObjectColor = new Color();
        gameObjectColor.r = 255f / 255f;
        gameObjectColor.g = 255f / 255f;
        gameObjectColor.b = 255f / 255f;
        gameObjectColor.a = 255f / 255f;
        Array.ForEach<UISprite>(transform.GetComponentsInChildren<UISprite>(), s => { s.color = gameObjectColor; });
        transform.collider.enabled = true;
    }
    public void DisableChannelButton(GameObject transform)
    {
        if (transform.gameObject.GetComponentInChildren<UILabel>() != null)
        {
            Color c = transform.gameObject.GetComponentInChildren<UILabel>().color;
            c.a = 70f / 255f;
            transform.gameObject.GetComponentInChildren<UILabel>().color = c;
        }
        Color gameObjectColor = new Color();
        gameObjectColor.r = 60f / 255f;
        gameObjectColor.g = 56f / 255f;
        gameObjectColor.b = 56f / 255f;
        gameObjectColor.a = 200f / 255f;
        Array.ForEach<UISprite>(transform.GetComponentsInChildren<UISprite>(), s => { s.color = gameObjectColor; });
        transform.collider.enabled = false;
    }
    public int checkNumberUser(ChannelChan channel)
    {
        double checkNumber = (channel.numberUsers / Convert.ToDouble(channel.maximumPlayers)) * 10;
        return Convert.ToInt32(System.Math.Ceiling(checkNumber));
    }
}