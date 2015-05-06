using System;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;
using UnityEngine;
using System.Collections;

public class ChannelListViewPhom : MonoBehaviour
{
    #region UnityEditor
    /// <summary>
    /// Vị trí các ChannelPhom và thứ tự các ChannelPhom
    /// </summary>
    public List<CUIHandle> listButton;
    public List<UILabel> listChip;
    public CUIHandle btRecharge;

    //Button
    public CUIHandle btnChip, btnGold;

    public List<UISprite> icon;
    public List<UISprite> background;
    public UITabbarController tabIcon;
    #endregion

    /// <summary>
    /// Danh Sách Các Phòng đấu (Chuyên nghiệp, nghiệp dư ...)
    /// </summary>
    List<ChannelPhom> listChannel = new List<ChannelPhom>();

    void Awake()
    {
        GameManager.Server.EventUpdateUserInfo += OnUpdateUserInfo;
        CUIHandle.AddClick(listButton.ToArray(), OnClickButtonChannel);
        //CUIHandle.AddClick(btRecharge, OnClickButtonRecharge);

        GameManager.Server.EventPluginMessageOnProcess += OnProcessPluginMessage;
        GameManager.Server.EventJoinRoom += OnAfterJoinRoom;
        GameManager.Server.EventConfigClientChanged += ActiveButtonRecharge;
        HeaderMenu.Instance.ReDraw();
        CUIHandle.AddClick(btnChip, OnClickButtonChip);
        CUIHandle.AddClick(btnGold, OnClickButtonGold);

        BroadcastView.Instance.Show(GameManager.Setting.BroadcastMessage);
        ActiveButtonRecharge();
        new AfterJoinChannel();
    }

    void OnDestroy()
    {
        //CUIHandle.RemoveClick(btRecharge, OnClickButtonRecharge);
        GameManager.Server.EventUpdateUserInfo -= OnUpdateUserInfo;
        CUIHandle.RemoveClick(listButton.ToArray(), OnClickButtonChannel);
        GameManager.Server.EventConfigClientChanged -= ActiveButtonRecharge;
        if (!GameManager.IsExist) return;
        CUIHandle.RemoveClick(btnChip, OnClickButtonChip);
        CUIHandle.RemoveClick(btnGold, OnClickButtonGold);

        GameManager.Server.EventPluginMessageOnProcess -= OnProcessPluginMessage;
        GameManager.Server.EventJoinRoom -= OnAfterJoinRoom;
    }
    public void ActiveButtonRecharge(IDictionary obj = null)
    {
        //if (!GameManager.Setting.Platform.EnableRecharge)
        //    btRecharge.gameObject.SetActive(false);
        //else
        //    btRecharge.gameObject.SetActive(true);
    }

    private void OnClickButtonRecharge(GameObject targetObject)
    {
        RechargeView.Create();
    }

    void OnClickButtonChannel(GameObject go)
    {
        if (listChannel.Count > 0)
        {
            //GameManager.Instance.selectedChannel = listChannel[go.GetComponent<UIContainerAnonymous>().valueInt - 1];
            GameManager.Instance.selectedChannel = listChannel.FindAll(c => c.moneyType == GameManager.PlayGoldOrChip || string.IsNullOrEmpty(c.moneyType)).Find(x => x.roomName.StartsWith(go.GetComponent<UIContainerAnonymous>().valueString));

            GameManager.Server.DoJoinRoom(GameManager.Instance.selectedChannel.zoneId, GameManager.Instance.selectedChannel.roomId);
        }
        else
            OnAfterJoinRoom(new JoinRoomEvent()); //For Test
    }

    public void OnUpdateUserInfo()
    {
        OnDrawListChannel();
    }
    void Start()
    {
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
        SetIcon();
        HeaderMenu.Instance.OnClickButtonBackCallBack = delegate()
        {
            GameManager.Server.DoJoinRoom(GameManager.Instance.hallRoom.zoneId, GameManager.Instance.hallRoom.roomId);
        };
    }

    void OnAfterJoinRoom(JoinRoomEvent e)
    {
        if (e.RoomName == "hall")
            GameManager.LoadScene(ESceneName.HallSceen);
        else
            GameManager.LoadScene(ESceneName.LobbyPhom);
    }

    void OnProcessPluginMessage(string command, string action, EsObject paremeters)
    {
        if (command == Fields.RESPONSE.FULL_UPDATE)
        {
            EsObject[] children = paremeters.getEsObjectArray("children");
            listChannel.Clear();
            foreach (EsObject obj in children)
                listChannel.Add(new ChannelPhom(obj));
            OnDrawListChannel();
        }
    }

    List<GameObject> OpaticalObject = new List<GameObject>();
    public void OnDrawListChannel()
    {
        //for (int i = 0; i < listChannel.Count; i++)
        //{
        //    if (listChannel[i].minimumMoney <= GameManager.Instance.mInfo.chip)
        //    {
        //        EnableChannelButton(listButton[i].gameObject);
        //    }
        //    else
        //    {
        //        DisableChannelButton(listButton[i].gameObject);
        //    }
        //}

        //for (int i = 0; i < listChip.Count; i++)
        //{
        //    listChannel[i].type = (ChannelPhom.ChannelType)i;
        //    int index = checkNumberUser(listChannel[i]);
        //    listButton[i].GetComponentInChildren<NumberUserInChannel>().SetValue(index);
        //    string text = "Chip > " + Utility.Convert.Chip(listChannel[i].minimumMoney);
        //    if (listChannel[i].minimumMoney == 0)
        //        text = "Phòng tự do";

        //    listChip[i].text = text;
        //}

        List<ChannelPhom> channelSelect = listChannel.FindAll(c => c.moneyType == GameManager.PlayGoldOrChip || string.IsNullOrEmpty(c.moneyType));

        //disable all button
        for (int i = 0; i < listButton.Count; i++)
        {
            DisableChannelButton(listButton[i].gameObject);
            NumberUserInChannel number = listButton[i].GetComponentInChildren<NumberUserInChannel>();
            if (number)
            {
                //number.lblDecription.text = "";
                number.SetValue(0);
            }
        }

        if (channelSelect.Count == 0)
            return;

        for (int i = 0; i < channelSelect.Count; i++)
        {
            //GameObject button = listButton.Find(x => x.GetComponent<UIContainerAnonymous>().valueString == channelSelect[i].roomName).gameObject;
            GameObject button = listButton.Find(x => channelSelect[i].roomName.StartsWith(x.GetComponent<UIContainerAnonymous>().valueString)).gameObject;

            if (string.IsNullOrEmpty(channelSelect[i].moneyType) || channelSelect[i].minimumMoney <= (GameManager.PlayGoldOrChip == Fields.StatusPlayer.GOLD ? GameManager.Instance.mInfo.gold : GameManager.Instance.mInfo.chip))
            {
                if (button)
                    EnableChannelButton(button);
            }
            if (i == 4) // Làm mờ nút giải đấu
                DisableChannelButton(button);
            int index = checkNumberUser(channelSelect[i]);
            if (button)
            {
                NumberUserInChannel number = button.GetComponentInChildren<NumberUserInChannel>();
                if (number != null)
                {
                    number.SetValue(index);
                    string text = GameManager.PlayGoldOrChip == Fields.StatusPlayer.CHIP ? "Chip >" : "Gold >";
                    text += Utility.Convert.Chip(channelSelect[i].minimumMoney);
                    if (channelSelect[i].minimumMoney == 0)
                        text = "Phòng tự do";
                    //number.lblDecription.text = text;
                    listChip[i].text = text;
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
    public int checkNumberUser(ChannelPhom channel)
    {

        double checkNumber = (channel.numberUsers / Convert.ToDouble(channel.maximumPlayers)) * 10;
        return Convert.ToInt32(System.Math.Ceiling(checkNumber));
    }

    void SetIcon()
    {
        if (GameManager.PlayGoldOrChip == Fields.StatusPlayer.CHIP) //[0] = gold // [1] = chip
        {
            icon[1].spriteName = "IconChip2";
            icon[0].spriteName = "IconMoney2";
            background[1].spriteName = "tab_selected";
            background[0].spriteName = "tab_normal";
            tabIcon.selectedIndex = 1;
            btnChip.transform.FindChild("Label").GetComponent<UILabel>().color = new Color(24f / 255f, 51f / 255f, 47f / 255f, 255f / 255f);
            btnGold.transform.FindChild("Label").GetComponent<UILabel>().color = new Color(165f / 255f, 220f / 255f, 215f / 255f, 255f / 255f);
        }
        else
        {
            icon[1].spriteName = "IconChip";
            icon[0].spriteName = "IconMoney";
            background[1].spriteName = "tab_normal";
            background[0].spriteName = "tab_selected";
            tabIcon.selectedIndex = 0;
            btnGold.transform.FindChild("Label").GetComponent<UILabel>().color = new Color(24f / 255f, 51f / 255f, 47f / 255f, 255f / 255f);
            btnChip.transform.FindChild("Label").GetComponent<UILabel>().color = new Color(165f / 255f, 220f / 255f, 215f / 255f, 255f / 255f);
        }
    }
    void OnClickButtonChip(GameObject go)
    {
        GameManager.PlayGoldOrChip = Fields.StatusPlayer.CHIP;
        OnDrawListChannel();
        SetIcon();
    }
    void OnClickButtonGold(GameObject go)
    {
        //GameManager.PlayGoldOrChip = Fields.StatusPlayer.GOLD;
        //OnDrawListChannel();
        //SetIcon();
        NotificationView.ShowMessage("Chức năng đang được xây dựng, vui lòng quay lại sau", 3f);
    }

}
