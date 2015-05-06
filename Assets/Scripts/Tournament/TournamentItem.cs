using UnityEngine;
using System.Collections;

public class TournamentItem : MonoBehaviour {

    public UILabel lblTitle, lblAward, lblTime;
    public UITexture mainavartar;
    public GameObject avatarObject;
    vp_Timer.Handle m_Timer = new vp_Timer.Handle();

    public TournamentInfo info { get; private set; }

    void Update()
    {
        if (m_Timer.Active)
            CountDownTimer();
    }

    public void UpdateInfo(TournamentInfo _info)
    {
        this.info = _info;
        lblAward.text = info.award;
        lblTime.text = info.startDate + " - " + info.endDate;
        lblTitle.text = info.tournamentName;

        if (string.IsNullOrEmpty(info.userNameWin))
        {
            avatarObject.SetActive(false);
            lblTime.gameObject.SetActive(true);
            lblAward.gameObject.SetActive(true);
        }
        else
        {
            avatarObject.SetActive(true);
            lblTime.gameObject.SetActive(false);
            lblAward.gameObject.SetActive(false);
            DownloadAvatar();
        }

        InitCountDownTime();
    }

    void DownloadAvatar()
    {
        new AvatarCacheOrDownload(info.avatarWinner, delegate(Texture texture)
            {
                this.mainavartar.mainTexture = texture;
            },true);
    }

    void InitCountDownTime()
    {
        vp_Timer.In(info.remainStartTime, delegate()
        {
            
        }, m_Timer
        );
    }

    void CountDownTimer()
    {
        info.remainStartTime = (long)m_Timer.DurationLeft;
    }
}
