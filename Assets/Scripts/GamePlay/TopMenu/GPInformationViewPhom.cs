using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GPInformationViewPhom : MonoBehaviour
{
    public UILabel lbName, lbBetting, lbMaster, lbRule, lblSubRule;
    public CUITextList textInfor;

    public static List<string> listMessage = new List<string>();
    void Start()
    {
        lbMaster.text = ((LobbyPhom)GameManager.Instance.selectedLobby).roomMasterUsername;
        lbRule.text = ((LobbyPhom)GameManager.Instance.selectedLobby).config.isAdvanceGame ? "Nâng cao" : "Cơ bản";
        lblSubRule.text = ((LobbyPhom)GameManager.Instance.selectedLobby).config.strGameRule;
        ChannelPhom.ChannelType type = ((ChannelPhom)GameManager.Instance.selectedChannel).type;
        string channel = type == ChannelPhom.ChannelType.Amateur ? "Nghiệp dư"
            : type == ChannelPhom.ChannelType.Professional ? " Chuyên nghiệp"
            : type == ChannelPhom.ChannelType.Experts ? "Phản gỗ"
            : type == ChannelPhom.ChannelType.Giants ? "Sập gụ" : "Chiếu vương";
        lbName.text = channel + " " + ((LobbyPhom)GameManager.Instance.selectedLobby).gameIndex + " - " + ((LobbyPhom)GameManager.Instance.selectedLobby).nameLobby;

        if (GameManager.PlayGoldOrChip == "chip")
            lbBetting.text = Utility.Convert.Chip(((LobbyPhom)GameManager.Instance.selectedLobby).betting) + " Chip";
        else if (GameManager.PlayGoldOrChip == "gold")
            lbBetting.text = Utility.Convert.Chip(((LobbyPhom)GameManager.Instance.selectedLobby).betting) + " Gold";


        listMessage.ForEach(str =>
        {
            textInfor.Add(str);
            Utility.AutoScrollChat(textInfor);
        });
    }
    public void NewLogWhenOpen(string textChat)
    {
        textInfor.Add(textChat);
        Utility.AutoScrollChat(textInfor);
    }
}
