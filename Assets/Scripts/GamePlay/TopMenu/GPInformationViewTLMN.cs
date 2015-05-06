using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GPInformationViewTLMN : MonoBehaviour {

    public UILabel lbName, lbBetting, lbGameType, lbGameRule, lbMasterRoom;
    public CUITextList textInfor;

    public static List<string> listMessage = new List<string>();
    void Start()
    {
        ChannelTLMN.ChannelType type = ((ChannelTLMN)GameManager.Instance.selectedChannel).type;
        string channel = type == ChannelTLMN.ChannelType.Amateur ? "Nghiệp dư"
            : type == ChannelTLMN.ChannelType.Professional ? " Chuyên nghiệp"
            : type == ChannelTLMN.ChannelType.Experts ? "Cao Thủ"
            : type == ChannelTLMN.ChannelType.Giants ? "Đại Gia" : "Giải Đấu";
        lbName.text = channel + " " + ((LobbyTLMN)GameManager.Instance.selectedLobby).gameIndex + " - " + ((LobbyTLMN)GameManager.Instance.selectedLobby).nameLobby;

        if (GameManager.PlayGoldOrChip == "chip")
            lbBetting.text = Utility.Convert.Chip(((LobbyTLMN)GameManager.Instance.selectedLobby).betting) + " Chip";
        else if (GameManager.PlayGoldOrChip == "gold")
            lbBetting.text = Utility.Convert.Chip(((LobbyTLMN)GameManager.Instance.selectedLobby).betting) + " Gold";

        lbName.text = ((LobbyTLMN)GameManager.Instance.selectedLobby).gameIndex + " - " + ((LobbyTLMN)GameManager.Instance.selectedLobby).nameLobby;
        lbMasterRoom.text = ((LobbyTLMN)GameManager.Instance.selectedLobby).roomMasterUsername;
        lbGameType.text = ((LobbyTLMN)GameManager.Instance.selectedLobby).config.GAME_TYPE_TLMN == LobbyTLMN.GameConfig.GameTypeLTMN.DEM_LA ? "Đếm lá" : "Xếp hạng";
        lbGameRule.text = ((LobbyTLMN)GameManager.Instance.selectedLobby).config.CHAT_CHUONG_RULE ? ((LobbyTLMN)GameManager.Instance.selectedLobby).config.strGameRule : ""; 

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
