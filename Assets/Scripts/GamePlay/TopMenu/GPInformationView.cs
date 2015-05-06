using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GPInformationView : MonoBehaviour
{
    public UILabel lbName, lbBetting, lbGameType, lbGa,lbTime;
    public CUITextList textInfor;

    public static List<string> listMessage = new List<string>();
    void Start()
    {
        ChannelChan.ChannelType type = ((ChannelChan)GameManager.Instance.selectedChannel).type;
        string channel = type == ChannelChan.ChannelType.Amateur ? "Nền đất"
            : type == ChannelChan.ChannelType.Professional ? " Chiếu cói"
            : type == ChannelChan.ChannelType.Experts ? "Phản gỗ"
            : type == ChannelChan.ChannelType.Giants ? "Sập gụ" : "Chiếu vương";
        lbName.text = channel + " " + ((LobbyChan)GameManager.Instance.selectedLobby).gameIndex + " - " + ((LobbyChan)GameManager.Instance.selectedLobby).nameLobby;

        if (GameManager.PlayGoldOrChip == "chip")
            lbBetting.text = Utility.Convert.Chip(((LobbyChan)GameManager.Instance.selectedLobby).betting) + " Chip";
        else if (GameManager.PlayGoldOrChip == "gold")
            lbBetting.text = Utility.Convert.Chip(((LobbyChan)GameManager.Instance.selectedLobby).betting) + " Gold";

        lbGameType.text = ((LobbyChan)GameManager.Instance.selectedLobby).config.RULE_FULL_PLAYING == 1 ? "Ù xuông" :
            ((LobbyChan)GameManager.Instance.selectedLobby).config.RULE_FULL_PLAYING == 2 ? "Ù không xuông" :
            ((LobbyChan)GameManager.Instance.selectedLobby).config.RULE_FULL_PLAYING == 3 ? "Ù 4-17 điểm" : "";
        lbGa.text = ((LobbyChan)GameManager.Instance.selectedLobby).config.strGameRule;
        lbTime.text = ((LobbyChan)GameManager.Instance.selectedLobby).timePlay + "s";
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
