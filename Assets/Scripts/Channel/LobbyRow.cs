using System;
using System.Collections.Generic;
using UnityEngine;
using Electrotank.Electroserver5.Api;
public class LobbyRow : MonoBehaviour
{
    #region UnityEditor
    public UILabel numberPlayer;
    public UILabel lbIndex, lbName, lbBetting, lbRule,lblTime;
    public UISprite spriteBackground;
    public UISprite ga;

    #endregion

    [HideInInspector]
    public LobbyChan lobby;
    [HideInInspector]
    public bool gamePlaying;
    [HideInInspector]
    public static float WIDTH = 160f;
    static List<LobbyRow> _list = new List<LobbyRow>();
    public static List<LobbyRow> List
    {
        get { return _list; }
    }
    public static void Remove(LobbyRow row)
    {
        if(List.Contains(row))
        {
            if(row.gameObject != null)
                GameObject.Destroy(row.gameObject);
            List.Remove(row);
        }
    }

    public static LobbyRow Create(UIPanel panel,Transform parent, LobbyChan lobby)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Channel/LobbyRow"));

        //obj.name = string.Format("Lobby_{0:0000}", List.Count + 1);
        obj.name = string.Format("Lobby_{0:0000}", lobby.gameIndex);
        obj.transform.parent = parent;
        obj.transform.localPosition = new Vector3(0f, 0f, -2f);
        obj.transform.localScale = Vector3.one;
        LobbyRow row = obj.GetComponent<LobbyRow>();
        row.SetData(lobby);
        List.Add(row);
        SetSize(panel, row);
        return row;
    }

    void SetData(LobbyChan lobby)
    {
        this.lobby = lobby;
        UpdateIndex(lobby.gameIndex);
        //numberPlayer.SetValue(lobby.numberUserInRoom);
        numberPlayer.text = lobby.numberUserInRoom + "/4";
        lbBetting.text = Utility.Convert.Chip(lobby.betting);
        lbName.text = lobby.nameLobby;
        lblTime.text = lobby.timePlay + "s";
        lbRule.text = lobby.config.RULE_FULL_PLAYING == 1 ? "Ù xuông" :
            lobby.config.RULE_FULL_PLAYING == 2 ? "Không Ù Xuông" :
            lobby.config.RULE_FULL_PLAYING == 3 ? "Ù 4-17 điểm" : "";
        UpdateImageLock(lobby);
        updateRule(lobby);
        updateGa(lobby);
        if (lobby.numberUserInRoom == 4)
        {
            Color fullColor = new Color(254f / 255f, 102f / 255f, 0f / 255f, 255f / 255f);
            numberPlayer.color = fullColor;
            lbIndex.color = fullColor;
            lbBetting.color = fullColor;
            lbName.color = fullColor;
            lbRule.color = fullColor;
        }
    }

    public void UpdateData(LobbyChan lobby)
    {
        this.lobby.UpdateData(lobby);
        lbName.text = lobby.nameLobby;
        //numberPlayer.SetValue(lobby.numberUserInRoom);
        numberPlayer.text = lobby.numberUserInRoom + "/4";
        UpdateImageLock(lobby);
        if (lobby.numberUserInRoom == 4)
        {
            Color fullColor = new Color(254f/255f, 102f/255f, 0f/255f, 255f/255f);
            numberPlayer.color = fullColor;
            lbIndex.color = fullColor;
            lbBetting.color = fullColor;
            lbName.color = fullColor;
            lbRule.color = fullColor;
        }
    }
    public void updateGa(LobbyChan lobby)
    {
      // lobby.config. = GameManager.Instance.selectedLobby.config.NUOI_GA_RULE;
        ga.GetComponent<UISprite>().spriteName = lobby.ruleGa == LobbyChan.EGaRule.GaNhai ? "gamai" : lobby.ruleGa == LobbyChan.EGaRule.NuoiGa ? "gahai" : "none";
    }
    public void updateRule(LobbyChan lobby)
    {
       transform.FindChild("batbao").GetComponent<UISprite>().spriteName = lobby.autoBatBao == true ? "nguoichoibatbao" : "none";
       transform.FindChild("nguoichoiu").GetComponent<UISprite>().spriteName = lobby.autoU == true ? "nguoichoiu" : "none";
    }
    void UpdateImageLock(LobbyChan lobby)
    {
		this.lobby.isPassword = lobby.isPassword;
		this.lobby.gamePlaying = lobby.gamePlaying;
        if (lobby.isPassword == true)
        {
            transform.FindChild("roomState").GetComponent<UISprite>().spriteName = "lobby_lock_icon";
         }
        else
        {
            if (lobby.gamePlaying)
            {
                transform.FindChild("roomState").GetComponent<UISprite>().spriteName = "lobby_play_icon";
            }
            else
            {
                transform.FindChild("roomState").GetComponent<UISprite>().spriteName = "lobby_waitting_icon";
            }
        }
    }

    public void UpdateIndex(int index)
    {
        lbIndex.text = index.ToString();
    }

    void ProcessInputPassword(string inputDialog)
    {
        if (PlaySameDevice.IsCanJoinGameplay)
        {
            WaitingView.Show("Đang vào phòng");
            GameManager.Server.DoJoinGame(inputDialog.Trim());
        }
    }

    void Awake()
    {
        CUIHandle.AddClick(GetComponent<CUIHandle>(), DoJoinLobby);
    }

    public void DoJoinLobby(GameObject go)
    {
        if (Common.ValidateChipToBetting(lobby.betting, GameManager.PlayGoldOrChip) == false)
        {
            int rule = Common.RULE_CHIP_COMPARE_BETTING;
            Common.MessageRecharge("Số tiền của bạn phải lớn hơn hoặc bằng "+ rule +" lần tiền cược.");
            return;
        }

        GetComponent<CUIHandle>().StopImpact(2f);

        if (lobby.numberUserInRoom == lobby.maxNumberPlayer)
        {
            NotificationView.ShowMessage("Bàn chơi đã đầy. Xin vui lòng tìm bàn chơi khác.", 2f);
            return;
        }
        GameManager.Instance.selectedLobby = lobby;

        if (lobby.isPassword)
            NotificationView.ShowDialog("Nhập mật khẩu để truy cập bàn chơi", ProcessInputPassword);
        else
            ProcessInputPassword("");
    }

    static void SetSize(UIPanel parent, LobbyRow row)
    {
        Debug.Log(parent.GetViewSize().x);
        //row.spriteBackground.width = (int)parent.GetViewSize().x;
        //row.spriteBackground.SetAnchor(parent.transform);
        row.spriteBackground.leftAnchor.target = parent.transform;
        row.spriteBackground.leftAnchor.absolute = 0;
        row.spriteBackground.rightAnchor.target = parent.transform;
        row.spriteBackground.rightAnchor.absolute = 0;

    }
}
