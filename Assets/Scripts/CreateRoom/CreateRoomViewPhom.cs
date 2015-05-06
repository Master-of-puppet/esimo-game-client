using System;
using System.Collections.Generic;
using UnityEngine;
using Electrotank.Electroserver5.Api;

public class CreateRoomViewPhom : MonoBehaviour
{
    #region Unity Edtior Luật chơi
    public UIToggle[] betting;
    public UIInput txtRoomName, txtPassword;
    public UIToggle[] ruleMain;
    public UIToggle[] ruleSub;
    public UIInput txtBetting;
    #endregion

    #region Unity Editor Mời chơi
    public UIInput txtInviContent;
    public CUIHandle btCreate;
    #endregion

    void Awake()
    {
        GameManager.Server.EventPluginMessageOnProcess += OnProcessPluginMessage;
        CUIHandle.AddClick(btCreate, OnClickCreate);
        GameManager.Server.EventLoadSence += OnJoinGame;
    }

    void OnDestroy()
    {
        GameManager.Server.EventPluginMessageOnProcess -= OnProcessPluginMessage;
        GameManager.Server.EventLoadSence -= OnJoinGame;
        CUIHandle.RemoveClick(btCreate, OnClickCreate);
    }
    private void OnJoinGame()
    {
        WaitingView.Instance.Close();
        GameManager.Instance.selectedLobby = new LobbyPhom();
        GameManager.LoadScene(ESceneName.GameplayPhom);
    }
    void Start()
    {
        for (int i = 0; i < betting.Length; i++)
        {
            if (i > ((ChannelPhom)GameManager.Instance.selectedChannel).bettingValues.Length - 1)
            {
                GameObject.Destroy(betting[i].gameObject);
                betting[i] = null;
            }
            else
            {
                if (((ChannelPhom)GameManager.Instance.selectedChannel).bettingValues[i] == ChannelPhom.CHANNEL_BILLIONAIRE_BETTING_OTHER_VALUE)
                {
                    betting[i].gameObject.transform.FindChild("Label").gameObject.GetComponent<UILabel>().text = "Khác";
                    Utility.AddCollider(betting[i].gameObject);
                }
                else
                {
                    betting[i].gameObject.transform.FindChild("Label").gameObject.GetComponent<UILabel>().text = string.Format("{0:0,0}", ((ChannelPhom)GameManager.Instance.selectedChannel).bettingValues[i]).Replace(",", ".");
                    Utility.AddCollider(betting[i].gameObject);
                    if (CommonPhom.ValidateChipToBetting(((ChannelPhom)GameManager.Instance.selectedChannel).bettingValues[i]) == false)
                    {
                        betting[i].collider.enabled = false;
                        Color c = betting[i].GetComponentInChildren<UILabel>().color;
                        c.a = 100 / 255f;
                        betting[i].GetComponentInChildren<UILabel>().color = c;
                    }
                }
            }

        }
        HeaderMenu.Instance.OnClickButtonBackCallBack = delegate()
        {
            GameManager.LoadScene(ESceneName.LobbyPhom);
        };
    }
    void OnProcessPluginMessage(string command, string action, EsObject paremeters)
    {
        if (command == "error")
        {
            WaitingView.Instance.Close();
            int id = paremeters.getInteger("error");
            if (id == 5)
            {
                NotificationView.ShowMessage("Vui lòng chờ ván bài cũ của bạn kết thúc.\n\nTrước khi tạo bàn mới.");
            }
        }
    }

    void OnClickCreate(GameObject go)
    {
        int numBetting = 0;
        for (int i = 0; i < betting.Length; i++)
        {
            if (betting[i] == null) continue;

            if (betting[i].value == true && ((ChannelPhom)GameManager.Instance.selectedChannel).bettingValues[i] == ChannelPhom.CHANNEL_BILLIONAIRE_BETTING_OTHER_VALUE)
            {
                numBetting = Convert.ToInt32(txtBetting.value) * 10000;
                break;
            }
            else if (betting[i].value)
            {
                numBetting = ((ChannelPhom)GameManager.Instance.selectedChannel).bettingValues[i];
            }
        }
        if (GameManager.Instance.mInfo.chip < numBetting * CommonPhom.RULE_CHIP_COMPARE_BETTING || numBetting < 0)
        {
            NotificationView.ShowMessage("Mức tiền cược phải nhỏ hơn " + CommonPhom.RULE_CHIP_COMPARE_BETTING + " lần số chip của bạn, hoặc mức cược đang nhỏ hơn 0", 3f);
        }
        else
        {
            WaitingView.Show("Đang tạo bàn");
            DoCreateGame(numBetting);
        }
    }

    private void DoCreateGame(int betting)
    {
        Debug.Log("DoCreateGame");
        EsObject gameConfig = new EsObject();

        string roomName = txtRoomName.value;
        if (string.IsNullOrEmpty(roomName))
            roomName = "Bàn chơi của " + GameManager.Instance.mInfo.username;

        gameConfig.setString(LobbyPhom.DEFINE_LOBBY_NAME, roomName);
        gameConfig.setString(LobbyPhom.DEFINE_LOBBY_PASWORD, txtPassword.value.Trim());
        gameConfig.setInteger(LobbyPhom.DEFINE_BETTING, betting);

        gameConfig.setBoolean(LobbyPhom.DEFINE_USING_U_TRON_RULE, ruleSub[0].value); //Ù tròn
        gameConfig.setBoolean(LobbyPhom.DEFINE_USING_U_XUONG_RULE, ruleSub[1].value); //Ù Xuông
        gameConfig.setBoolean(LobbyPhom.DEFINE_USING_XAO_KHAN_RULE, ruleSub[2].value); // Xào Khan
        gameConfig.setBoolean(LobbyPhom.DEFINE_USING_DEN_CHONG_RULE, ruleSub[3].value); //Đền Chồng
        gameConfig.setBoolean(LobbyPhom.DEFINE_USING_CHOT_CHONG_RULE, ruleSub[4].value); //Chốt Chồng

        gameConfig.setBoolean(LobbyPhom.DEFINE_USING_TYPE_RULE, ruleMain[0].value);//True Cơ bản, False Nâng Cao

        gameConfig.setStringArray(LobbyPhom.DEFINE_INVITED_USERS, new string[0]);

        GameManager.Server.DoRequestPlugin(Utility.SetEsObject(Fields.RESPONSE.CREATE_GAME,
            new object[] { "config", gameConfig }));
    }


    #region Active On Scene
    void OnActivateNormal()
    {

        for (int i = 0; i < ruleSub.Length; i++)
        {
            if (i == 0)
            {
                ruleSub[i].value = false;
                ruleSub[i].collider.enabled = false;
            }
            else
            {
                ruleSub[i].value = true;
                ruleSub[i].collider.enabled = true;
            }
        }
    }
    void OnActivateRB()
    {

        for (int i = 0; i < betting.Length; i++)
        {
            if (betting[i] == null) continue;
            if (betting[i].value && ((ChannelPhom)GameManager.Instance.selectedChannel).bettingValues[i] == ChannelPhom.CHANNEL_BILLIONAIRE_BETTING_OTHER_VALUE)
            {
                txtBetting.gameObject.SetActive(true);
                txtBetting.gameObject.transform.parent.GetComponent<UITable>().repositionNow = true;
            }
            else
            {
                txtBetting.gameObject.SetActive(false);
            }
        }
    }
    void OnActivate(bool activate)
    {

        if (activate)
        {
            if (ruleMain[0].value)
            {
                for (int i = 0; i < ruleSub.Length; i++)
                {
                    if (i == 0)
                    {
                        ruleSub[i].value = false;
                        ruleSub[i].collider.enabled = false;
                    }
                    else
                    {
                        ruleSub[i].value = true;
                        ruleSub[i].collider.enabled = true;
                    }
                }
            }
        }
    }
    void OnActivateAdvance()
    {

        // Array.ForEach<UICheckbox>(ruleSub, cb => { cb.isChecked = true; cb.collider.enabled = true; });
        if (ruleMain[1].value)
        {
            for (int i = 0; i < ruleSub.Length; i++)
            {
                ruleSub[i].value = true;
                ruleSub[i].collider.enabled = true;
            }
        }
    }


    #endregion
}