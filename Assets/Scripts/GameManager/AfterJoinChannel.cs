using System;
using System.Collections.Generic;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Class xử lý sau khi join vào channel
/// </summary>
public class AfterJoinChannel
{
    public AfterJoinChannel()
    {
        OnLoadAnnounceDone();

        GameManager.Instance.applicationStart.EventLoadAnnounce += OnLoadAnnounceDone;
    }

    void OnLoadAnnounceDone()
    {
        if (GameManager.Instance.ListAnnouncement.FindAll(a => a.show == Announcement.Scene.announce && a.type == Announcement.Type.Gift).Count > 0)
        {
            LoadAnnounceInDay();
        }
    }

    void LoadAnnounceInDay()
    {
        //if (StoreGame.Contains(StoreGame.EType.ANNOUNCEMENT_IN_DAY))
        //{
        //    DateTime timeCompare = DateTime.Parse(StoreGame.LoadString(StoreGame.EType.ANNOUNCEMENT_IN_DAY));

        //    //if (Math.Floor(Math.Abs(timeCompare.Subtract(DateTime.Now).TotalDays)) != 0)
        //    if (timeCompare.DayOfYear != DateTime.Now.DayOfYear)
        //    {
        //        StoreGame.SaveString(StoreGame.EType.ANNOUNCEMENT_IN_DAY, DateTime.Now.ToLongTimeString());
                AnnouncementView.Instance.Create();
        //    }
        //}
        //else
        //{
        //    StoreGame.SaveString(StoreGame.EType.ANNOUNCEMENT_IN_DAY, DateTime.Now.ToLongTimeString());
        //    AnnouncementView.Instance.Create();
        //}
    }
}
