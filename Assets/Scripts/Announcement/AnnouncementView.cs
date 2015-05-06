using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AnnouncementView : MonoBehaviour
{
    #region Unity Edtior
    public CUIHandle btClose;
    public UIGrid tableGift;
    public UILabel txtFooter;
    #endregion

    static AnnouncementView _instance;
    public static AnnouncementView Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject announcement = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Announcement/AnnouncementPrefab"));
                announcement.name = "__Announcement";
                announcement.transform.position = new Vector3(-210, -553, 128f);
                _instance = announcement.GetComponent<AnnouncementView>();
                _instance.Init();
            }
            return _instance;
        }
    }

    public void Create() { }

    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickButtonClose;
        CUIHandle.AddClick(btClose, OnClickButtonClose);
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(btClose, OnClickButtonClose);
    }

    void OnClickButtonClose(GameObject go)
    {
        Close();
    }
    public void Close()
    {
        GameObject.Destroy(gameObject);
    }
    public static Vector3 VectorGiftCenter = Vector3.zero;
    void OnDragFinishGift()
    {
        VectorGiftCenter = tableGift.GetComponent<UICenterOnChild>().centeredObject.transform.position;
    }

    void Init()
    {
        //List<Announcement> events = new List<Announcement>(GameManager.Instance.ListAnnouncement.FindAll(a => a.show == Announcement.Scene.announce && a.type == Announcement.Type.Event));
        //if (events.Count > 0)
        //{
        //    foreach (Announcement ev in events)
        //        AnnounceItemEvent.Create(tableEvent.transform, ev, tableEvent.transform.parent.GetComponent<UIDraggablePanel>());
        //    tableEvent.repositionNow = true;
        //}

        //List<Announcement> advertisement = new List<Announcement>(GameManager.Instance.ListAnnouncement.FindAll(a => a.show == Announcement.Scene.announce && a.type == Announcement.Type.Advertisement));
        //if (advertisement.Count > 0)
        //{
        //    foreach (Announcement ads in advertisement)
        //        AnnounceItemAds.Create(tableAds.transform, ads, tableAds.transform.parent.GetComponent<UIDraggablePanel>());
        //    tableAds.repositionNow = true;
        //}

        #region QUÀ TẶNG MỖI NGÀY

        List<Announcement> gifts = new List<Announcement>(GameManager.Instance.ListAnnouncement.FindAll(a => a.show == Announcement.Scene.announce && a.type == Announcement.Type.Gift));
        gifts.ForEach(g => AnnounceItemGift.Create(tableGift.transform, g));

        tableGift.repositionNow = true;


        GameManager.Instance.FunctionDelay(delegate()
        {
			tableGift.GetComponent<UICenterOnChild>().Recenter();
        }, 0.05f);
        GameManager.Instance.FunctionDelay(delegate()
        {
			tableGift.GetComponent<UICenterOnChild>().onFinished = OnDragFinishGift;
            OnDragFinishGift();
        }, 0.1f);
        GameManager.Instance.FunctionDelay(delegate()
        {
            AnnounceItemGift itemCurrentDay = Array.Find<AnnounceItemGift>(tableGift.GetComponentsInChildren<AnnounceItemGift>(), g => g.item.currentDay == true);
            if (itemCurrentDay != null)
				tableGift.GetComponent<UICenterOnChild>().CenterOn(itemCurrentDay.transform);
        }, 0.15f);
        #endregion
    }
    internal void SetCenterOnNextCurrentDay()
    {
        AnnounceItemGift itemCurrentDay = Array.Find<AnnounceItemGift>(tableGift.GetComponentsInChildren<AnnounceItemGift>(), g => g.item.currentDay == true);
        int index = Array.IndexOf<AnnounceItemGift>(tableGift.GetComponentsInChildren<AnnounceItemGift>(), itemCurrentDay, 0);
        tableGift.transform.GetChild(index).GetComponent<AnnounceItemGift>().ChangeTextToToday("Hôm nay");
        txtFooter.text = "Bạn đã nhận quà hôm nay";
        AnnounceItemGift tommorrow = index == tableGift.transform.childCount - 1 ? tableGift.transform.GetChild(0).GetComponent<AnnounceItemGift>() : tableGift.transform.GetChild(index + 1).GetComponent<AnnounceItemGift>();
        tommorrow.GetComponent<AnnounceItemGift>().ChangeTextToToday("Ngày Mai");
		tableGift.GetComponent<UICenterOnChild>().CenterOn(tommorrow.transform);

    }

}
