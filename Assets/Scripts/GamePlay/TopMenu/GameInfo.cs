using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    public UILabel lbName, lbBetting_chip, lblBetting_gold, lbGameType;

    void Awake()
    {

    }
    void Start()
    {
        lbName.text = ((LobbyChan)GameManager.Instance.selectedLobby).gameIndex + " - " + ((LobbyChan)GameManager.Instance.selectedLobby).nameLobby;

        lbBetting_chip.text = Utility.Convert.Chip(((LobbyChan)GameManager.Instance.selectedLobby).betting);
        lblBetting_gold.text = Utility.Convert.Chip(((LobbyChan)GameManager.Instance.selectedLobby).betting);
        if (GameManager.PlayGoldOrChip == "gold")
        {
            lblBetting_gold.gameObject.SetActive(true);
            lbBetting_chip.gameObject.SetActive(false);
        }
        else
        {
            lblBetting_gold.gameObject.SetActive(false);
            lbBetting_chip.gameObject.SetActive(true);
        }

        lbGameType.text = ((LobbyChan)GameManager.Instance.selectedLobby).config.RULE_FULL_PLAYING == 1 ? "Ù xuông" :
           ((LobbyChan)GameManager.Instance.selectedLobby).config.RULE_FULL_PLAYING == 2 ? "Không Ù Xuông" :
            ((LobbyChan)GameManager.Instance.selectedLobby).config.RULE_FULL_PLAYING == 3 ? "Ù 4-17 điểm" : "";    
    }

}
