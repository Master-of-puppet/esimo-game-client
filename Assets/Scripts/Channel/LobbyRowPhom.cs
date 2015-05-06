using System;
using System.Collections.Generic;
using UnityEngine;
using Electrotank.Electroserver5.Api;

public class LobbyRowPhom : MonoBehaviour
{
    #region UnityEditor
    public UILabel numberPlayer;
    public UILabel lbIndex, lbName, lbBetting, lbRule;
    public UISprite spriteStatus, spriteBackground;
    #endregion

    //trạng thái của bàn chơi
    string WAITING = "waiting";
    string LOCK = "lock";
    string PLAYING = "playing";

    [HideInInspector]
    public LobbyPhom lobby;
    [HideInInspector]
    public bool gamePlaying;

    static List<LobbyRowPhom> _list = new List<LobbyRowPhom>();
    public static List<LobbyRowPhom> List
    {
        get { return _list; }
    }
    public static void Remove(LobbyRowPhom row)
    {
        if (List.Contains(row))
        {
            if (row.gameObject != null)
                GameObject.Destroy(row.gameObject);
            List.Remove(row);
        }
    }

    public static LobbyRowPhom Create(UIPanel panel, Transform parent, LobbyPhom lobby)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Channel/LobbyRowPhom"));

        obj.name = string.Format("Lobby_{0:0000}", lobby.gameIndex);
        obj.transform.parent = parent;
        obj.transform.localPosition = new Vector3(0f, 0f, -2f);
        obj.transform.localScale = Vector3.one;
        LobbyRowPhom row = obj.GetComponent<LobbyRowPhom>();
        row.SetData(lobby);
        List.Add(row);
        SetSize(panel, row);
        return row;
    }

    void SetData(LobbyPhom lobby)
    {
        this.lobby = lobby;
        UpdateIndex(lobby.gameIndex);
        numberPlayer.text = lobby.numberUserInRoom + "/4";
        lbBetting.text = Utility.Convert.Chip(lobby.betting);
        lbName.text = lobby.nameLobby;
        lbRule.text = lobby.config.isAdvanceGame ? "Nâng cao" : "Cơ bản";
        UpdateImageLock(lobby);
        if (lobby.numberUserInRoom == 4)
        {
            Color fullColor = new Color(254f / 255f, 102f / 255f, 0f / 255f, 255f / 255f);
            numberPlayer.color = fullColor;
            lbIndex.color = fullColor;
            lbBetting.color = fullColor;
            lbName.color = fullColor;
            lbRule.color = fullColor;
        }
       // transform.FindChild("bgNumber").GetComponent<UISprite>().spriteName = lobby.gameIndex < 10 ? "bgmbtab1" : lobby.gameIndex < 100 ? "bgmbtab2" : "bgmbtab3";
    }

    public void UpdateData(EsObject obj)
    {
        if (obj.variableExists("config"))
        {
            this.lobby = new LobbyPhom(obj);
        }
        else
        {
            if (obj.variableExists("gameId"))
                this.lobby.gameId = obj.getInteger("gameId");
            if (obj.variableExists("description"))
                this.lobby.nameLobby = obj.getString("description");
            if (obj.variableExists("maximumPlayers"))
                this.lobby.maxNumberPlayer = obj.getInteger("maximumPlayers");
            if (obj.variableExists("numberUsers"))
                this.lobby.numberUserInRoom = obj.getInteger("numberUsers");
            if (obj.variableExists("betting"))
                this.lobby.betting = obj.getInteger("betting");
            if (obj.variableExists("gameIndex"))
                this.lobby.gameIndex = obj.getInteger("gameIndex");
            if (obj.variableExists("password"))
                this.lobby.isPassword = obj.getBoolean("password");
            if (obj.variableExists("gamePlaying"))
                this.lobby.gamePlaying = obj.getBoolean("gamePlaying");
        }
        //    this.lobby = lobby;

        numberPlayer.text = lobby.numberUserInRoom + "/4";
        if (lobby.numberUserInRoom == 4)
        {
            Color fullColor = new Color(254f / 255f, 102f / 255f, 0f / 255f, 255f / 255f);
            numberPlayer.color = fullColor;
            lbIndex.color = fullColor;
            lbBetting.color = fullColor;
            lbName.color = fullColor;
            lbRule.color = fullColor;
        }
        UpdateImageLock(lobby);
    }
  
    void UpdateImageLock(LobbyPhom lobby)
    {
        this.lobby.isPassword = lobby.isPassword;
        this.lobby.gamePlaying = lobby.gamePlaying;
        if (lobby.isPassword == true)
        {
            spriteStatus.spriteName = LOCK;
        }
        else
        {
            if (lobby.gamePlaying)
            {
                spriteStatus.spriteName = PLAYING;
            }
            else
            {
                spriteStatus.spriteName = WAITING;
            }
        }
    }

    public void UpdateIndex(int index)
    {
        lbIndex.text = index.ToString();
    }

    void ProcessInputPassword(string inputDialog)
    {
        Debug.Log("Calling join room :" + GameManager.Instance.selectedLobby.gameId);
        if (PlaySameDevice.IsCanJoinGameplay)
            GameManager.Server.DoJoinGame(inputDialog.Trim());
    }

    void Awake()
    {
        CUIHandle.AddClick(GetComponent<CUIHandle>(), DoJoinLobby);
    }

    public void DoJoinLobby(GameObject go)
    {
        if (CommonPhom.ValidateChipToBetting(lobby.betting) == false)
        {
            int rule = CommonPhom.RULE_CHIP_COMPARE_BETTING;
            Common.MessageRecharge("Số tiền của bạn phải lớn hơn hoặc bằng " + rule + " lần tiền cược.");
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
    static void SetSize(UIPanel parent, LobbyRowPhom row)
    {
        //Debug.Log(parent.GetViewSize().x);
        //row.spriteBackground.width = (int)parent.GetViewSize().x;
        //row.spriteBackground.SetAnchor(parent.transform);
        parent.clipOffset = Vector2.zero;
        row.spriteBackground.leftAnchor.target = parent.transform;
        row.spriteBackground.leftAnchor.absolute = 0;
        row.spriteBackground.rightAnchor.target = parent.transform;
        row.spriteBackground.rightAnchor.absolute = 0;

    }
}
