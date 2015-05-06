using System;
using System.Collections.Generic;
using UnityEngine;

public class GameplayListener
{
    #region EVENT PLAYER LIST CHANGED
    public delegate void DelegatePlayerListChanged(EPlayerController p, bool isRemove);
    /// <summary>
    /// Event khi có người chơi tham gia hoặc thoát khỏi game.
    /// </summary>
    public event DelegatePlayerListChanged EventPlayerListChanged;
    public void RegisterEventPlayerListChanged(EPlayerController p, bool isRemove)
    {
        if (EventPlayerListChanged != null && GameObject.Find("__Prefab Manager Gameplay") != null)
            EventPlayerListChanged(p, isRemove);
        else
        {
            if(GameManager.CurrentScene==ESceneName.GameplayChan)
                GPInformationView.listMessage.Add(string.Format("[FFCC00]{0}[-]\n", "Người chơi \"" + p.username + "\" đã " + (isRemove ? "rời khởi bàn chơi." : "tham gia bàn chơi.")));
            else if(GameManager.CurrentScene==ESceneName.GameplayPhom)
                GPInformationViewPhom.listMessage.Add(string.Format("[FFCC00]{0}[-]\n", "Người chơi \"" + p.username + "\" đã " + (isRemove ? "rời khởi bàn chơi." : "tham gia bàn chơi.")));
            else if(GameManager.CurrentScene==ESceneName.GameplayTLMN)
                GPInformationViewTLMN.listMessage.Add(string.Format("[FFCC00]{0}[-]\n", "Người chơi \"" + p.username + "\" đã " + (isRemove ? "rời khởi bàn chơi." : "tham gia bàn chơi.")));
        }
            

    }
    #endregion

    #region EVENT ROOM MASTER CHANGE
    public delegate void DelegateRoomMasterChanged(EPlayerController p);
    /// <summary>
    /// Event khi thay đổi room master
    /// </summary>
    public event DelegateRoomMasterChanged EventRoomMasterChanged;
    public void RegisterEventRoomMasterChanged(EPlayerController p)
    {
        if (EventRoomMasterChanged != null)
            EventRoomMasterChanged(p);
    }
    #endregion

    #region EVENT CHANGE SLOT
    public delegate void DelegateSwapSlot(EPlayerController[] player);
    /// <summary>
    /// Event khi thay đổi room master
    /// </summary>
    public event DelegateSwapSlot EventSwapSlot;
    public void RegisterEventSwapSlot(EPlayerController[] player)
    {
        if (EventSwapSlot != null)
            EventSwapSlot(player);
    }
    #endregion


    #region EVENT NEW GAME
    /// <summary>
    /// Event khi thay đổi room master
    /// </summary>
    public event CallBackFunction EventNewGame;
    public void RegisterEventNewGame()
    {
        if (EventNewGame != null)
            EventNewGame();
    }
    #endregion


    #region EVENT LOG GAME
    public delegate void DelegateLogGameChange(string log);
    public event DelegateLogGameChange EventLogGameChange;

    public void RegisterEventLogGame(string log)
    {
        if (EventLogGameChange != null && GameObject.Find("__Prefab Manager Gameplay") != null)
            EventLogGameChange(log);
        else
        {
            if (GameManager.CurrentScene == ESceneName.GameplayChan)
                GPInformationView.listMessage.Add(log);
            else if (GameManager.CurrentScene == ESceneName.GameplayTLMN)
                GPInformationViewTLMN.listMessage.Add(log);
            else if (GameManager.CurrentScene == ESceneName.GameplayPhom)
                GPInformationViewPhom.listMessage.Add(log);
        }
    }
    #endregion


    public delegate void DelegatePlayerWaitingChangePriority();
    public event DelegatePlayerWaitingChangePriority EventPlayerWaitingChangePriority;
    internal void RegisterEventWaitingChangePriority()
    {
        if (EventPlayerWaitingChangePriority != null && GameObject.Find("__Prefab Manager Gameplay") != null)
            EventPlayerWaitingChangePriority();
    }
}
