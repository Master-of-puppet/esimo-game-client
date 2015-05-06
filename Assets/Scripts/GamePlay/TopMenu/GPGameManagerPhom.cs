using UnityEngine;
using System.Collections;
using System;
using Electrotank.Electroserver5.Api;

public class GPGameManagerPhom : MonoBehaviour
{
    public CUIHandle btnClose;
    public UITabbarController controller;


    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickButtonClose;
        CUIHandle.AddClick(btnClose, OnClickButtonClose);
        GameModelPhom.game.Listener.EventLogGameChange += OnLogGame;
        GameModelPhom.game.Listener.EventPlayerListChanged += OnPlayerListChanged;
        GameModelPhom.game.Listener.EventSwapSlot += OnSwapSlot;
        GameModelPhom.game.Listener.EventPlayerWaitingChangePriority += OnLoadPlayerWaiting;
        controller.tabbarButtons[1].gameObject.SetActive(GameModelPhom.YourController != null && GameModelPhom.YourController.isMaster);
        controller.tabbarButtons[2].gameObject.SetActive(GameModelPhom.YourController != null && GameModelPhom.YourController.isMaster);
    }

    private void OnLoadPlayerWaiting()
    {
        UITabbarPanel manager = Array.Find<UITabbarPanel>(controller.tabbarPanel, s => s.GetComponent<EGPViewManager>() != null);
        manager.GetComponent<EGPViewManager>().DrawListWaitingPlayer();
    }
    private void OnLogGame(string log)
    {
        Debug.Log("On write log phom.......");
        UITabbarPanel info = Array.Find<UITabbarPanel>(controller.tabbarPanel, s => s.GetComponent<GPInformationViewPhom>() != null);
        info.GetComponent<GPInformationViewPhom>().NewLogWhenOpen(log);
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnClose, OnClickButtonClose);
        if (!GameManager.IsExist) return;
        GameModelPhom.game.Listener.EventPlayerWaitingChangePriority -= OnLoadPlayerWaiting;
        GameModelPhom.game.Listener.EventLogGameChange -= OnLogGame;
        GameModelPhom.game.Listener.EventPlayerListChanged -= OnPlayerListChanged;
        GameModelPhom.game.Listener.EventSwapSlot -= OnSwapSlot;
    }

    public static GPGameManagerPhom Create()
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/TopMenu/GameplayManagerPhomPrefabs"));
        obj.name = "__Prefab Manager Gameplay";
        obj.transform.localPosition = new Vector3(1600f, 1700f, 500f);
        return obj.GetComponent<GPGameManagerPhom>();
    }
    public static GPGameManagerPhom Create(int indexTab)
    {
        GPGameManagerPhom gpm = Create();
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
        UITabbarPanel manager = Array.Find<UITabbarPanel>(controller.tabbarPanel, s => s.GetComponent<EGPViewManager>() != null);
        manager.GetComponent<EGPViewManager>().OnSwapSlot(listPlayer);
    }

    void OnPlayerListChanged(EPlayerController p, bool isRemove)
    {
        OnLogGame(string.Format("[FFCC00]{0}[-]\n", "Người chơi \"" + p.username + "\" đã " + (isRemove ? "rời khởi bàn chơi." : "tham gia bàn chơi.")));
        UITabbarPanel manager = Array.Find<UITabbarPanel>(controller.tabbarPanel, s => s.GetComponent<GPManagerViewPhom>() != null);
        manager.GetComponent<GPManagerViewPhom>().OnPlayerListChanged(p, isRemove);
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
