using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;

public class GPGameManagerChan : MonoBehaviour
{
    public CUIHandle btnClose;
    public UITabbarController controller;


    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickButtonClose;
        CUIHandle.AddClick(btnClose, OnClickButtonClose);
        GameModelChan.game.Listener.EventLogGameChange += OnLogGame;
        GameModelChan.game.Listener.EventPlayerListChanged += OnPlayerListChanged;
        GameModelChan.game.Listener.EventSwapSlot += OnSwapSlot;
        GameModelChan.game.Listener.EventPlayerWaitingChangePriority += OnLoadPlayerWaiting;
		controller.tabbarButtons [1].gameObject.SetActive (GameModelChan.YourController !=null && GameModelChan.YourController.isMaster);
		controller.tabbarButtons [2].gameObject.SetActive (GameModelChan.YourController !=null && GameModelChan.YourController.isMaster);
    }

    private void OnLoadPlayerWaiting()
    {
        UITabbarPanel manager = Array.Find<UITabbarPanel>(controller.tabbarPanel, s => s.GetComponent<GPManagerView>() != null);
        manager.GetComponent<GPManagerView>().DrawListWaitingPlayer();
    }
    private void OnLogGame(string log)
    {
        UITabbarPanel info = Array.Find<UITabbarPanel>(controller.tabbarPanel, s => s.GetComponent<GPInformationView>() != null);
        info.GetComponent<GPInformationView>().NewLogWhenOpen(log);
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnClose, OnClickButtonClose);
        if (!GameManager.IsExist) return;
        GameModelChan.game.Listener.EventPlayerWaitingChangePriority -= OnLoadPlayerWaiting;
        GameModelChan.game.Listener.EventLogGameChange -= OnLogGame;
        GameModelChan.game.Listener.EventPlayerListChanged -= OnPlayerListChanged;
        GameModelChan.game.Listener.EventSwapSlot -= OnSwapSlot;
    }

    public static GPGameManagerChan Create()
    {
		GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/TopMenu/GameplayManagerPrefabs"));
        obj.name = "__Prefab Manager Gameplay";
        obj.transform.localPosition = new Vector3(1600f, 1700f, 500f);
        return obj.GetComponent<GPGameManagerChan>();
    }
    public static GPGameManagerChan Create(int indexTab)
    {
        GPGameManagerChan gpm = Create();
        gpm.ShowTabByIndex(indexTab);
    	return gpm;
    }
    public void ShowTabByIndex(int index)
    {
        controller.OnSelectTabbar(index);
    }
    void OnClickButtonClose(GameObject go)
    {
        GameObject.Destroy(gameObject);
    }

    void OnSwapSlot(EPlayerController[] listPlayer)
    {
        UITabbarPanel manager = Array.Find<UITabbarPanel>(controller.tabbarPanel, s => s.GetComponent<GPManagerView>() != null);
        manager.GetComponent<GPManagerView>().OnSwapSlot(listPlayer);
    }

    void OnPlayerListChanged(EPlayerController p, bool isRemove)
    {
        OnLogGame(string.Format("[FFCC00]{0}[-]\n", "Người chơi \"" + p.username + "\" đã " + (isRemove ? "rời khởi bàn chơi." : "tham gia bàn chơi.")));
        UITabbarPanel manager = Array.Find<UITabbarPanel>(controller.tabbarPanel, s => s.GetComponent<GPManagerView>() != null);
        manager.GetComponent<GPManagerView>().OnPlayerListChanged(p, isRemove);
    }
    #region PROCESS USERS ONLINE
    /// <summary>
    /// Xử lý khi request lên server để lấy về danh sách những người đang online trong Lobby ngoài
    /// </summary>
    private void OnPluginMessageOnProcess(string command, string action, Electrotank.Electroserver5.Api.EsObject eso)
    {
        UITabbarPanel manager = Array.Find<UITabbarPanel>(controller.tabbarPanel, s => s.GetComponent<GPInviteUserView>() != null);
        manager.GetComponent<GPInviteUserView>().OnPluginMessageOnProcess(command, action, eso);
    }

    /// <summary>
    /// Xủ lý khi có sự thay đổi về danh sách người online, thêm người mới vào lobby, có người thoát ra khỏi lobby
    /// </summary>
    void OnPluginUpdateUserOnline(string command, string action, EsObject eso)
    {
        UITabbarPanel manager = Array.Find<UITabbarPanel>(controller.tabbarPanel, s => s.GetComponent<GPInviteUserView>() != null);
        manager.GetComponent<GPInviteUserView>().OnPluginUpdateUserOnline(command, action, eso);
    }
    #endregion

}
