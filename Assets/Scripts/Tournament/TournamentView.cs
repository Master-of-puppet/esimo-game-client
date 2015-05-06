using UnityEngine;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;

public class TournamentView : MonoBehaviour {

    public UIGrid gridTournament;
    public TournamentDetail detail;
    public GameObject tournamentItemPrefab;
    static TournamentView _instance;
    public CUIHandle btnRegister,btnShow;
    public UILabel lblStatus;
    [HideInInspector]
    public TournamentItem currentShow;
    [HideInInspector]
    public List<TournamentItem> tournamentItems = new List<TournamentItem>();
    public GameObject tournamentDetail, tournamentWinner;
    public static TournamentView Instance
    {
        get
        {
            if (_instance==null)
            {
                GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Tournament/Tournament"));
                _instance = obj.GetComponent<TournamentView>();
            }
            return _instance;
        }
    }

    public void RegisterHeaderMenu()
    {
        HeaderMenu.Instance.OnClickButtonBackCallBack = delegate()
        {
            WaitingView.Show("Đang thoát");
            GameManager.Instance.currentTournamentInfo = null;
            GameManager.Server.DoJoinRoom(GameManager.Instance.channelRoom.zoneId, GameManager.Instance.channelRoom.roomId);
        };
    }

    void Awake()
    {
        _instance = this;
        CUIHandle.AddClick(btnRegister, JoinTournament);
        CUIHandle.AddClick(btnShow, JoinTournament);
        GameManager.Server.EventPluginMessageOnProcess += OnProcessPluginMessage;
        GameManager.Server.EventJoinRoom += OnAfterJoinRoom;
        RegisterHeaderMenu();
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnRegister, JoinTournament);
        CUIHandle.RemoveClick(btnShow, JoinTournament);
        GameManager.Server.EventPluginMessageOnProcess -= OnProcessPluginMessage;
        GameManager.Server.EventJoinRoom -= OnAfterJoinRoom;
    }

    private void OnAfterJoinRoom(JoinRoomEvent e)
    {
        if (e.RoomId == currentShow.info.roomId && e.ZoneId==currentShow.info.zoneId)
        {
            GameManager.Instance.currentTournamentInfo = currentShow.info;
            TournamentInfo info = GameManager.Instance.currentTournamentInfo;
            GameManager.LoadScene(ESceneName.Tournament);
        }
    }

    // Use this for initialization
    void Start()
    {
        WaitingView.Show("Đang lấy dữ liệu. Vui lòng đợi");
        GameManager.Server.DoRequestCommand(Fields.REQUEST.COMMAND_TOURNAMENT);
        gridTournament.GetComponent<UICenterOnChild>().onCenter = ShowDetail;
    }

    void OnClickRegister(GameObject go)
    {
        TournamentRegister.Create();
    }

    void JoinTournament(GameObject go)
    {
        GameManager.Server.DoJoinRoom(currentShow.info.zoneId, currentShow.info.roomId);
    }

    void ShowDetail(GameObject obj)
    {
        
        TournamentItem currentselect = obj.GetComponent<TournamentItem>();
        CheckWinner(currentselect.info);

        if (currentShow != null)
        {
            if (currentShow.info.tournamentId != currentselect.info.tournamentId)
            {
                currentShow = currentselect;
                detail.SetData(currentShow.info);
                GameManager.Instance.currentTournamentInfo = currentShow.info;
            }
        }
        else
        {
            currentShow = obj.GetComponent<TournamentItem>();
            detail.SetData(currentShow.info);
        }
        
        if (currentShow.info.zoneId<0 || currentShow.info.roomId<0)
        {
            btnShow.gameObject.SetActive(false);
            btnRegister.gameObject.SetActive(false);
            lblStatus.gameObject.SetActive(false);
        }
        else
        {
            btnShow.gameObject.SetActive(true);
            if (!currentShow.info.isRegister)
            {
                if (currentShow.info.remainStartTime > 0)
                {
                    btnRegister.gameObject.SetActive(true);
                    lblStatus.gameObject.SetActive(false);
                }
                else
                {
                    btnRegister.gameObject.SetActive(false);
                    lblStatus.text = "(Hết thời gian đăng ký)";
                }
            }
            else
            {
                btnRegister.gameObject.SetActive(false);
                lblStatus.gameObject.SetActive(true);
                lblStatus.text = "(Đã đang ký)";
            }
        }
    }

    void CheckWinner(TournamentInfo info)
    {
        Debug.Log("Check winner...");
        if (string.IsNullOrEmpty(info.userNameWin))
        {
            tournamentDetail.SetActive(true);
            tournamentWinner.SetActive(false);
        }
        else
        {
            tournamentDetail.SetActive(false);
            tournamentWinner.SetActive(true);
            tournamentWinner.GetComponent<TournamentWinner>().SetData(info.award, info.userNameWin, info.avatarWinner);
        }
    }

    private void OnProcessPluginMessage(string command, string action, EsObject paremeters)
    {
        if (command==Fields.RESPONSE.COMMAND_TOURNAMENT)
        {
            EsObject[] esObject = paremeters.getEsObjectArray("tournaments");
            for (int i = 0; i < esObject.Length; i++)
            {
                TournamentInfo info = new TournamentInfo(esObject[i]);
                GameObject obj = NGUITools.AddChild(gridTournament.gameObject, tournamentItemPrefab);
                obj.name = "Tournament" + info.tournamentId;
                TournamentItem item = obj.GetComponent<TournamentItem>();
                item.UpdateInfo(info);
                tournamentItems.Add(item);
            }
            gridTournament.Reposition();
            gridTournament.GetComponent<UICenterOnChild>().Recenter();
            WaitingView.Instance.Close();
        }
        if (command=="updateTournament")
        {
            if (paremeters.variableExists("tournamentId"))
            {
                int id = paremeters.getInteger("tournamentId");
                TournamentItem item = this.tournamentItems.Find(x => x.info.tournamentId == id);
                if (item!=null)
                {
                    if (paremeters.variableExists("zoneId"))
                        item.info.zoneId = paremeters.getInteger("zoneId");
                    if (paremeters.variableExists("roomId"))
                        item.info.roomId = paremeters.getInteger("roomId");
                }
            }
            NGUITools.SetActive(btnShow.gameObject, currentShow.info.roomId >= 0);
        }
    }
}
