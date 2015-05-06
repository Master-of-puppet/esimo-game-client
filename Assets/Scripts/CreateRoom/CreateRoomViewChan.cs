using UnityEngine;
using System.Collections;
using Electrotank.Electroserver5.Api;
using System;

public class CreateRoomViewChan : MonoBehaviour
{
    #region Unity Edtior Luật chơi
    public UIToggle[] betting;
    public UIInput txtRoomName, txtPassword;
    public UIToggle[] ruleMain;
    public UIToggle[] timePlay;
    public UIInput txtBetting;
    public UIToggle cbNuoiGa, cbGaNhai;
    public UISprite selectedChannel;
    #endregion

    #region Unity Editor Mời chơi
    public UIInput txtInviContent;
    public CUIHandle btCreate;
    ChannelChan channelSelected { get { return (ChannelChan)GameManager.Instance.selectedChannel; } }
    #endregion

    int showGa = 0;
    void Awake()
    {
        GameManager.Server.EventPluginMessageOnProcess += OnProcessPluginMessage;
        CUIHandle.AddClick(btCreate, OnClickCreate);
        // show name channel
        showNameChannel();
        GameManager.Server.EventLoadSence += OnJoinGame;
    }

    private void OnJoinGame()
    {
        GameManager.Instance.selectedLobby = new LobbyChan();
        GameManager.LoadScene(ESceneName.GameplayChan);
    }
    void OnDestroy()
    {
        GameManager.Server.EventPluginMessageOnProcess -= OnProcessPluginMessage;
        GameManager.Server.EventLoadSence -= OnJoinGame;
        CUIHandle.RemoveClick(btCreate, OnClickCreate);
    }

    void Start()
    {
        //ResizeInput();
        // lay thoi gian sever tra ve
        for (int i = 0; i < timePlay.Length; i++)
        {
            timePlay[i].gameObject.transform.FindChild("Label").gameObject.GetComponent<UILabel>().text = channelSelected.timePlay[i].ToString() + " Giây";
            Utility.AddCollider(timePlay[i].gameObject);
            timePlay[i].collider.enabled = true;
        }
        // lay muc tien cuoc sever tra ve
        for (int i = 0; i < betting.Length; i++)
        {
            if (i > channelSelected.bettingValues.Length - 1)
            {
                GameObject.Destroy(betting[i].gameObject);
                betting[i] = null;
            }
            else
            {
                if (channelSelected.bettingValues[i] == ChannelChan.CHANNEL_BILLIONAIRE_BETTING_OTHER_VALUE)
                {
                    betting[i].gameObject.transform.FindChild("Label").gameObject.GetComponent<UILabel>().text = "Khác";
                    Utility.AddCollider(betting[i].gameObject);
                }
                else
                {
                    betting[i].gameObject.transform.FindChild("Label").gameObject.GetComponent<UILabel>().text = string.Format("{0:0,0}", channelSelected.bettingValues[i]).Replace(",", ".");
                    Utility.AddCollider(betting[i].gameObject);
                    if (Common.ValidateChipToBetting(((ChannelChan)GameManager.Instance.selectedChannel).bettingValues[i], GameManager.PlayGoldOrChip) == false)
                    {
                        Array.ForEach<UISprite>(betting[i].GetComponentsInChildren<UISprite>(), s => s.color = new Color(128f / 255f, 128f / 255f, 128f / 255f));
                        betting[i].collider.enabled = false;
                        Color c = betting[i].GetComponentInChildren<UILabel>().color;
                        c.a = 100f / 255f;
                        betting[i].GetComponentInChildren<UILabel>().color = c;
                    }
                }
            }
        }
        HeaderMenu.Instance.OnClickButtonBackCallBack = delegate()
        {
            GameManager.LoadScene(ESceneName.LobbyChan);
        };
    }
    void OnProcessPluginMessage(string command, string action, EsObject paremeters)
    {
        if (command == "error")
        {
            int id = paremeters.getInteger("error");
            if (id == 5)
            {
                NotificationView.ShowMessage("Vui lòng chờ ván bài cũ của bạn kết thúc.\n\nTrước khi tạo bàn mới.");
            }
        }
    }
    public void showNameChannel()
    {
        ChannelChan.ChannelType type = channelSelected.type;
        selectedChannel.spriteName = type == ChannelChan.ChannelType.Amateur ? "nendat"
            : type == ChannelChan.ChannelType.Professional ? "chieucoi"
            : type == ChannelChan.ChannelType.Experts ? "phango"
            : type == ChannelChan.ChannelType.Giants ? "sapgu" : "chieuvuong";
        selectedChannel.MakePixelPerfect();
    }
    void OnClickCreate(GameObject go)
    {
        int numBetting = 0;
        int numTimePlaying = 0;
        for (int i = 0; i < timePlay.Length; i++)
        {
            if (timePlay[i].value)
            {
                numTimePlaying = channelSelected.timePlay[i];
            }
        }
        for (int i = 0; i < betting.Length; i++)
        {
            if (betting[i] == null) continue;

            if (betting[i].value == true && channelSelected.bettingValues[i] == ChannelChan.CHANNEL_BILLIONAIRE_BETTING_OTHER_VALUE)
            {
                numBetting = Convert.ToInt32(txtBetting.value) * 10000;
                break;
            }
            else if (betting[i].value)
            {
                numBetting = channelSelected.bettingValues[i];
            }
        }
        int total = numBetting * Common.RULE_CHIP_COMPARE_BETTING;// đang là kiểu int nên số quá to sẽ bị < 0
        if (GameManager.PlayGoldOrChip == "chip")
        {
            if (GameManager.Instance.mInfo.chip < total || total < 0)
            {
                NotificationView.ShowMessage("Mức tiền cược phải nhỏ hơn " + Common.RULE_CHIP_COMPARE_BETTING + " lần số chip của bạn, hoặc mức tiền cược đang nhỏ hơn 0", 3f);
            }
            else
            {
                DoCreateGame(numBetting, numTimePlaying);
            }
        }
        else if (GameManager.PlayGoldOrChip == "gold")
        {

            if (GameManager.Instance.mInfo.gold < total || total < 0)
            {
                NotificationView.ShowMessage("Mức tiền cược phải nhỏ hơn " + Common.RULE_CHIP_COMPARE_BETTING + " lần số gold của bạn, hoặc mức tiền cược đang nhỏ hơn 0", 3f);
            }
            else
            {
                DoCreateGame(numBetting, numTimePlaying);
            }
        }
    }

    private void DoCreateGame(int betting, int timePlay)
    {
        WaitingView.Show("Đang tạo phòng");
        int rulePlaying = 0;
        Debug.Log("DoCreateGame");
        EsObject gameConfig = new EsObject();

        string roomName = txtRoomName.value;
        if (string.IsNullOrEmpty(roomName))
            roomName = "Bàn chơi của " + GameManager.Instance.mInfo.username;

        gameConfig.setString(LobbyChan.DEFINE_LOBBY_NAME, roomName);
        gameConfig.setString(LobbyChan.DEFINE_LOBBY_PASWORD, txtPassword.value.Trim());
        gameConfig.setInteger(LobbyChan.DEFINE_BETTING, betting);

        if (!cbGaNhai.value && !cbNuoiGa.value)
            showGa = (int)LobbyChan.EGaRule.none;

        gameConfig.setInteger(LobbyChan.DEFINE_USING_NUOI_GA, showGa);//true nuoi ga false ga nhai
        gameConfig.setBoolean(LobbyChan.DEFINE_USING_AUTO_BAT_BAO, true);//true 
        gameConfig.setBoolean(LobbyChan.DEFINE_USING_AUTO_U, true);//true 
        rulePlaying = ruleMain[0].value ? 1 : ruleMain[1].value ? 2 : ruleMain[2].value ? 3 : 1;
        gameConfig.setInteger(LobbyChan.DEFINE_RULE_FULL_PLAYING, rulePlaying);
        gameConfig.setStringArray(LobbyChan.DEFINE_INVITED_USERS, new string[0]);
        gameConfig.setInteger(LobbyChan.DEFINE_PLAY_ACTION_TIME, timePlay);// defalt 20s

        GameManager.Server.DoRequestPlugin(Utility.SetEsObject(Fields.RESPONSE.CREATE_GAME,
            new object[] { "config", gameConfig }));
    }

    #region Active On Scene
    void OnActivateNormal()
    {
        //for (int i = 0; i < ruleSub.Length; i++)
        //{
        //    ruleSub[i].isChecked = false;
        //    ruleSub[i].collider.enabled = false;
        //}
    }
    void OnActivateRB()
    {
        for (int i = 0; i < betting.Length; i++)
        {
            if (betting[i] == null) continue;
            if (betting[i].value && channelSelected.bettingValues[i] == ChannelChan.CHANNEL_BILLIONAIRE_BETTING_OTHER_VALUE)
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
    void OnActivateBatBao()
    {
        //if (activate)
        //{
        //    if (ruleMain[0].isChecked)
        //    {
        //        for (int i = 0; i < ruleSub.Length; i++)
        //        {
        //            ruleSub[i].isChecked = false;
        //            ruleSub[i].collider.enabled = false;
        //        }
        //    }
        //}
    }
    void OnActivateBoU()
    {
    }
    void OnActivateNuoiGa()
    {
        if (cbNuoiGa.value)
        {
            cbGaNhai.value = false;
            showGa = (int)LobbyChan.EGaRule.NuoiGa;
        }
    }
    void OnActivateGaNhai()
    {
        if (cbGaNhai.value)
        {
            cbNuoiGa.value = false;
            showGa = (int)LobbyChan.EGaRule.GaNhai;
        }
    }
    #endregion
}
