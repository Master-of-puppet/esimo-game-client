using System;
using System.Collections.Generic;
using UnityEngine;
using Electrotank.Electroserver5.Api;

public class CreateRoomViewTLMN : MonoBehaviour
{
    #region Unity Edtior Luật chơi
    public UIToggle[] betting;
    public UIInput txtRoomName, txtPassword;
    public UIToggle[] ruleMain;
    public UIToggle[] ruleSub;
    #endregion

    #region Unity Editor Mời chơi
    public UIInput txtInviContent, txtBetting;
    public CUIHandle btCreate;
    #endregion

    void Awake()
    {
        CUIHandle.AddClick(btCreate, OnClickCreate);
        GameManager.Server.EventLoadSence += OnJoinGame;
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(btCreate, OnClickCreate);
        GameManager.Server.EventLoadSence -= OnJoinGame;
    }
    private void OnJoinGame()
    {
        WaitingView.Instance.Close();
        GameManager.Instance.selectedLobby = new LobbyTLMN();
        GameManager.LoadScene(ESceneName.GameplayTLMN);
    }

    void CheckMoney()
    {
        for (int i = 0; i < betting.Length; i++)
        {
            if (betting[i] == null) continue;

            if (i > ((ChannelTLMN)GameManager.Instance.selectedChannel).bettingValues.Length - 1)
            {
                GameObject.Destroy(betting[i].gameObject);
                betting[i] = null;
            }
            else
            {
                if (((ChannelTLMN)GameManager.Instance.selectedChannel).bettingValues[i] == ChannelTLMN.CHANNEL_BILLIONAIRE_BETTING_OTHER_VALUE)
                {
                    betting[i].gameObject.transform.FindChild("Label").gameObject.GetComponent<UILabel>().text = "Khác";
                }
                else
                {
                    if (ruleMain[0].value)
                    {
                        betting[i].gameObject.transform.FindChild("Label").gameObject.GetComponent<UILabel>().text = string.Format("{0:0,0}", ((ChannelTLMN)GameManager.Instance.selectedChannel).bettingValues[i]).Replace(",", ".");
                    }
                    else
                    {
                        betting[i].gameObject.transform.FindChild("Label").gameObject.GetComponent<UILabel>().text = string.Format("{0:0,0}", ((ChannelTLMN)GameManager.Instance.selectedChannel).bettingValues[i] * 10).Replace(",", ".");
                    }

                    if (CommonTLMN.ValidateChipToBetting(((ChannelTLMN)GameManager.Instance.selectedChannel).bettingValues[i], ruleMain[0].value) == false)
                    {
                        betting[i].collider.enabled = false;
                        Color c = betting[i].GetComponentInChildren<UILabel>().color;
                        c.a = 100 / 255f;
                        betting[i].GetComponentInChildren<UILabel>().color = c;
                    }
                }
                Utility.AddCollider(betting[i].gameObject);

            }
        }

    }

    void Start()
    {
        CheckMoney();

        HeaderMenu.Instance.OnClickButtonBackCallBack = delegate()
        {
            GameManager.LoadScene(ESceneName.LobbyTLMN);
        };

        Invoke("OnActivateNormal", 0.005f);
    }



    void OnClickCreate(GameObject go)
    {
        if (((ChannelTLMN)GameManager.Instance.selectedChannel).type == ChannelTLMN.ChannelType.Giants)
        {
            int rule = 0;
            int money = 0;
            for (int i = 0; i < betting.Length; i++)
            {
                if (betting[i] == null) continue;
                if (betting[i].value && ((ChannelTLMN)GameManager.Instance.selectedChannel).bettingValues[i] == ChannelTLMN.CHANNEL_BILLIONAIRE_BETTING_OTHER_VALUE)
                {
                    money = Convert.ToInt32(txtBetting.value) * 10000;
                    break;
                }
                else if (betting[i].value)
                {
                    money = ruleMain[0].value ? ((ChannelTLMN)GameManager.Instance.selectedChannel).bettingValues[i] : ((ChannelTLMN)GameManager.Instance.selectedChannel).bettingValues[i] * CommonTLMN.MULTI_MONEY_OF_XEP_HANG;
                }
            }
            rule = ruleMain[0].value ? CommonTLMN.RULE_DEM_LA_CHIP_COMPARE_BETTING : CommonTLMN.RULE_XEP_HANG_CHIP_COMPARE_BETTING;
            if (money > 0 && GameManager.Instance.mInfo.chip > money * rule)
            {
                DoCreateGame(money);
            }
            else
            {
                NotificationView.ShowMessage("Mức tiền cược phải nhỏ hơn " + rule + " lần số chip của bạn hoặc mức cược đang nhỏ hơn hoặc bằng 0", 3f);
            }

        }
        else
        {
            for (int i = 0; i < betting.Length; i++)
            {
                if (betting[i].value)
                {
                    int money = ruleMain[0].value ? ((ChannelTLMN)GameManager.Instance.selectedChannel).bettingValues[i] : ((ChannelTLMN)GameManager.Instance.selectedChannel).bettingValues[i] * 10;
                    DoCreateGame(money);
                    break;
                }
            }
        }
    }

    private void DoCreateGame(int betting)
    {
        WaitingView.Show("Đang tạo bàn");
        Debug.Log("DoCreateGame");
        EsObject gameConfig = new EsObject();

        string roomName = txtRoomName.value;
        if (string.IsNullOrEmpty(roomName))
            roomName = "Bàn chơi của " + GameManager.Instance.mInfo.username;

        gameConfig.setInteger(LobbyTLMN.DEFINE_GAME_TYPE_TLMN,
            ruleMain[0].value ? (int)LobbyTLMN.GameConfig.GameTypeLTMN.DEM_LA : (int)LobbyTLMN.GameConfig.GameTypeLTMN.XEP_HANG);

        gameConfig.setString(LobbyTLMN.DEFINE_LOBBY_NAME, roomName);
        gameConfig.setString(LobbyTLMN.DEFINE_LOBBY_PASWORD, txtPassword.value.Trim());
        gameConfig.setInteger(LobbyTLMN.DEFINE_BETTING, betting);

        gameConfig.setBoolean(LobbyTLMN.DEFINE_USING_CHATCHONG_RULE, ruleSub[0].value);

        gameConfig.setBoolean(LobbyTLMN.DEFINE_USING_TYPE_RULE, ruleMain[0].value);//True Cơ bản, False Nâng Cao

        gameConfig.setStringArray(LobbyTLMN.DEFINE_INVITED_USERS, new string[0]);

        GameManager.Server.DoRequestPlugin(Utility.SetEsObject(Fields.RESPONSE.CREATE_GAME,
            new object[] { "config", gameConfig }));
    }

    #region Active On Scene
    void OnActivateNormal()
    {
        Array.ForEach<UIToggle>(ruleSub, cb => { cb.value = true; cb.collider.enabled = true; });
        CheckMoney();
    }
    void OnActivate(bool activate)
    {
        return;
    }
    void OnActivateAdvance()
    {
        Array.ForEach<UIToggle>(ruleSub, cb => { cb.value = true; cb.collider.enabled = true; });
        CheckMoney();
    }
    void OnActivateRB()
    {
        if (((ChannelTLMN)GameManager.Instance.selectedChannel).type == ChannelTLMN.ChannelType.Giants)
        {
            for (int i = 0; i < betting.Length; i++)
            {
                if (betting[i] == null) continue;
                if (betting[i].value && ((ChannelTLMN)GameManager.Instance.selectedChannel).bettingValues[i] == ChannelTLMN.CHANNEL_BILLIONAIRE_BETTING_OTHER_VALUE)
                {
                    txtBetting.gameObject.SetActive(true);
                    betting[i].transform.parent.GetComponent<UITable>().repositionNow = true;
                }
                else
                {
                    txtBetting.gameObject.SetActive(false);
                }
            }
        }
    }
    #endregion
}