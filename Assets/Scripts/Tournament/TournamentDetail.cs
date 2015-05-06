using UnityEngine;
using System.Collections;

public class TournamentDetail : MonoBehaviour {
    int idTournament;
    public UITexture imgAward;
    public UILabel lblName, lblDecription, lblCost, lblCondition, lblAward, lblEndRegistration, lblStatus, lblNumberUser, lblTournamentStatus, lblRemainTime;
    public CUIHandle btnStatus;
    float remainTime;
    vp_Timer.Handle m_Timer = new vp_Timer.Handle();
    
    void Update()
    {
        if (m_Timer.Active)
            ShowCountDownTimer();
    }

	public void SetData(TournamentInfo info)
    {
        this.idTournament = info.tournamentId;
        lblName.text = info.tournamentName;
        lblDecription.text = info.decription;
        lblCost.text = Utility.Convert.Chip(info.costRegistration);
        lblCondition.text = info.consdition;
        lblEndRegistration.text = info.endDate;
        this.remainTime = (float)info.remainStartTime;
        if (info.isRegister)
        {
            NGUITools.SetActive(lblTournamentStatus.gameObject, true);
            lblTournamentStatus.text = "(Đã đăng ký)";
        }
        else
        {
            lblTournamentStatus.gameObject.SetActive(false);
            //if (info.remainStartTime<=0)
            //{
            //    if (info.remainStartTime==-1)
            //    {
            //        NGUITools.SetActive(lblTournamentStatus.gameObject, true);
            //        lblTournamentStatus.text = "Đã kết thúc";
            //    }
            //    else
            //    {
            //        NGUITools.SetActive(lblTournamentStatus.gameObject, true);
            //        lblTournamentStatus.text = "Đang diễn ra";
            //    }
            //}
            //else
            //{
            //    NGUITools.SetActive(lblTournamentStatus.gameObject, false);
            //}
        }
        //lblStatus.text=info.st
        lblNumberUser.text = info.numPlayersRegister + "/" + info.maxPlayers;
        new AvatarCacheOrDownload(info.awardLink, delegate(Texture awardtexture)
            {
                if (awardtexture!=null)
                {
                    imgAward.mainTexture = awardtexture;
                    imgAward.SetDimensions(128, 128);
                }
            }
            );
        CountDownTime();
    }

    void CountDownTime()
    {
        vp_Timer.In(remainTime, delegate()
        {
            if (string.IsNullOrEmpty(TournamentView.Instance.currentShow.info.userNameWin))
                lblRemainTime.text = "Đang thi đấu";
            else
                lblRemainTime.text = "Đã diễn ra";
        },m_Timer
        );
    }

    void ShowCountDownTimer()
    {
        lblRemainTime.text = vp_TimeUtility.TimeToString(m_Timer.DurationLeft, true, true, true, false, false, false);
    }
}
