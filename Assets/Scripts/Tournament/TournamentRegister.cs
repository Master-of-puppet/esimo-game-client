using UnityEngine;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;

public class TournamentRegister : MonoBehaviour
{

    #region Unity Editor
    public CUIHandle btnRegister,btnBack;
    public UIToggle cbPolicy;
    public TournamentInfo tournamentInfo ;
    public UILabel lblStatus, lblName, lblDecripton, lblAward, lblTimer;
    public UIInput txtUserName, txtEmail, txtFullName, txtCMT, txtSdt;
    public UILabel lblHeader, lblCost, lblCondition, lblBottom, lblBDecription;
    public UITexture imgAward,imgBottom;
    vp_Timer.Handle m_Timer = new vp_Timer.Handle();
    public UIGrid gridUser;
    public GameObject userTournamentPrefab, RegisterForm, TableTournament;
    public RoundChampion round1, round2, roud3, round4;

    #endregion
    List<UserTournament> users = new List<UserTournament>();
    void Awake()
    {
        this.tournamentInfo = GameManager.Instance.currentTournamentInfo;
        CUIHandle.AddClick(btnRegister, OnRegister);
        CUIHandle.AddClick(btnBack, OnClickBack);
        GameManager.Server.EventPluginMessageOnProcess += OnProcessPluginMessage;
        GameManager.Server.EventJoinRoom += OnAfterJoinRoom;
        
        if (GameManager.CurrentScene == ESceneName.Tournament)
        {
            HeaderMenu.Instance.OnClickButtonBackCallBack = delegate()
            {
                WaitingView.Show("Đang thoát");
                GameManager.Instance.currentTournamentInfo = null;
                GameManager.Server.DoJoinRoom(GameManager.Instance.currentRoomGiaiDau.zoneId, GameManager.Instance.currentRoomGiaiDau.roomId);
            };
        }

        GameManager.Instance.displayTournamentMenu = true;    
        HeaderMenu.Instance.ReDraw();
    }

    private void OnAfterJoinRoom(JoinRoomEvent e)
    {
        if (e.RoomName=="chan_giaidau")
        {
            GameManager.LoadScene(ESceneName.ChannelLeague);
        }
    }

    void OnClickBack(GameObject go)
    {
        GameObject.Destroy(gameObject);
    }

    void Start()
    {
        this.SetData();
        InitCountDownTime();

        if (GameManager.Instance.currentTournamentInfo.isRegister)
        {
            NGUITools.SetActive(TableTournament, true);
            NGUITools.SetActive(RegisterForm, false);
            WaitingView.Show("Đang lấy dữ liệu");
            GameManager.Server.DoRequestPlugin(Utility.SetEsObject(Fields.REQUEST.COMMAND_GETGENERAL,
          new object[] { "id", this.tournamentInfo.tournamentId }));
        }
        else
        {
            if (GameManager.Instance.currentTournamentInfo.remainStartTime>0)
            {
                NGUITools.SetActive(TableTournament, false);
                NGUITools.SetActive(RegisterForm, true);
            }
            else
            {
                NGUITools.SetActive(TableTournament, true);
                NGUITools.SetActive(RegisterForm, false);
                WaitingView.Show("Đang lấy dữ liệu");
                GameManager.Server.DoRequestPlugin(Utility.SetEsObject(Fields.REQUEST.COMMAND_GETGENERAL,
              new object[] { "id", this.tournamentInfo.tournamentId }));
            }
        }
    }

    void Update()
    {
        if (m_Timer.Active)
            CountDownTimer();
    }

    void InitCountDownTime()
    {
        vp_Timer.In(tournamentInfo.remainStartTime, delegate()
        {
            if (!this.tournamentInfo.isRegister)
            {
                NotificationView.ShowMessage("Đã hết thời gian đăng ký");
                NGUITools.SetActive(btnRegister.gameObject, false);
            }
        }, m_Timer
        );
    }

    void CountDownTimer()
    {
        lblTimer.text = vp_TimeUtility.TimeToString(m_Timer.DurationLeft, true, true, true, false, false, false);
    }

    void OnDestroy()
    {
        GameManager.Instance.displayTournamentMenu = false;
        HeaderMenu.Instance.ReDraw();
        CUIHandle.RemoveClick(btnRegister, OnRegister);
        GameManager.Server.EventPluginMessageOnProcess -= OnProcessPluginMessage;
        GameManager.Server.EventJoinRoom -= OnAfterJoinRoom;
        CUIHandle.RemoveClick(btnBack, OnClickBack);
    }

    void OnRegister(GameObject obj)
    {
        if (!cbPolicy.value)
        {
            NotificationView.ShowMessage("Bạn phải đồng ý với điều khoản!");
            return;
        }
        WaitingView.Show("Đăng đăng ký");
        GameManager.Server.DoRequestPlugin(Utility.SetEsObject(Fields.REQUEST.COMMAND_REGISTERTOURNAMENT,
            new object[] { "id", tournamentInfo.tournamentId }));
    }

    private void OnProcessPluginMessage(string command, string action, EsObject Parameters)
    {
        WaitingView.Instance.Close();
        if (command == Fields.REQUEST.COMMAND_REGISTERTOURNAMENT)
        {
            string message = "";
            if (Parameters.variableExists("message"))
                message = Parameters.getString("message");
            if (Parameters.getBoolean("result"))
            {
                TournamentView.Instance.btnRegister.gameObject.SetActive(false);
                TournamentView.Instance.detail.lblTournamentStatus.gameObject.SetActive(true);
                TournamentView.Instance.currentShow.info.isRegister = true;
                GameObject.Destroy(gameObject);
          //      NGUITools.SetActive(TableTournament, true);
          //      NGUITools.SetActive(RegisterForm, false);
          //      GameManager.Server.DoRequestPlugin(Utility.SetEsObject(Fields.REQUEST.COMMAND_GETGENERAL,
          //new object[] { "id", this.tournamentInfo.tournamentId }));
            }
            if(!string.IsNullOrEmpty(message))
                NotificationView.ShowMessage(message);
        }
        if (command == Fields.REQUEST.COMMAND_GETLISTREGISTER)
        {
            EsObject[] esObject = Parameters.getEsObjectArray("users");
            for (int i = 0; i < esObject.Length; i++)
            {
                users.Add(UserTournament.Create(esObject[i], gridUser.transform, userTournamentPrefab));
            }
            gridUser.Reposition();
        }
        if (command == Fields.REQUEST.COMMAND_GETGENERAL)
        {
            EsObject[] esObject = Parameters.getEsObjectArray("rounds");
            round1.SetData(esObject[0]);
            round2.SetData(esObject[1]);
            roud3.SetData(esObject[2]);
            round4.SetData(esObject[3]);
        }

        if (command == Fields.REQUEST.COMMAND_MATCH_LIST)
        {
            PopupTournament.Create(Parameters);
        }
    }

    public static TournamentRegister Create()
    {
        GameObject register = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Tournament/TournamentRegister"));
        register.name = "___TournamentRegister";
        register.transform.position = new Vector3(-5301, 240, -128f);
        //register.GetComponent<TournamentRegister>().tournamentInfo = info;
        register.GetComponent<TournamentRegister>().SetData();
        return register.GetComponent<TournamentRegister>();
    }

    public void SetData()
    {
        lblAward.text = tournamentInfo.award;
        lblBDecription.text= lblDecripton.text = tournamentInfo.decription;
        lblBottom.text = lblName.text = tournamentInfo.tournamentName;
        txtCMT.value = GameManager.Instance.mInfo.cmtnd;
        txtEmail.value = GameManager.Instance.mInfo.email;
        txtFullName.value = string.IsNullOrEmpty(GameManager.Instance.mInfo.FullName) ? GameManager.Instance.mInfo.username : GameManager.Instance.mInfo.FullName;
        txtSdt.value = GameManager.Instance.mInfo.phone;
        txtUserName.value = GameManager.Instance.mInfo.username;
        lblHeader.text = "Đăng ký giải " + tournamentInfo.tournamentName;
        lblCost.text = Utility.Convert.Chip(tournamentInfo.costRegistration);
        lblCondition.text = tournamentInfo.consdition;

        GameManager.Server.DoRequestPlugin(Utility.SetEsObject(Fields.REQUEST.COMMAND_GETLISTREGISTER,
           new object[] { "id", tournamentInfo.tournamentId, "gameId", (int)GameManager.GAME }));
    }
}
