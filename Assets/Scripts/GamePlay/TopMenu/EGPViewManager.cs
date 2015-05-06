using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EGPViewManager : MonoBehaviour {

    #region Unity Editor
    public UIInput txtChangePassword, txtRoomName;
    public UITexture[] swappedAvatar;
    public UILabel[] swappedUsername;
    public UITexture[] kickAvatar;
    public UILabel[] kickUsername;
    public CUIHandle[] kickButton;
    public CUIHandle btSave;
    public UIGrid gridWaitingPlayer;
    [HideInInspector]
    public List<PlayerWaitingInManager> waitingPlayers = new List<PlayerWaitingInManager>();
    #endregion
    void Awake()
    {
        CUIHandle.AddClick(kickButton, DoRequestKickPlayer);
        CUIHandle.AddClick(btSave, OnClickSave);
        gridWaitingPlayer.onReposition += OnRepositionGridComplete;
    }
    void Start()
    {
        OnStart();
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(kickButton, DoRequestKickPlayer);
        CUIHandle.RemoveClick(btSave, OnClickSave);
        gridWaitingPlayer.onReposition -= OnRepositionGridComplete;
    }
    void Update()
    {
        OnUpdate();
    }
    public virtual void DrawListWaitingPlayer()
    {

    } 
    public virtual void OnPlayerListChanged(EPlayerController p, bool isRemove)
    {

    }
    private void OnRepositionGridComplete()
    {
        gridWaitingPlayer.transform.localPosition = new Vector3(gridWaitingPlayer.transform.localPosition.x, -170f, gridWaitingPlayer.transform.localPosition.z);
    }
    public virtual void OnSwapSlot(EPlayerController[] listPlayer)
    {
       
    }
    public virtual void SetDataPlayer(EPlayerController p, int i)
    {
        if (p != null)
        {
            UIContainerAnonymous anony = kickButton[i].gameObject.GetComponent<UIContainerAnonymous>();
            if(anony==null)
                anony = kickButton[i].gameObject.AddComponent<UIContainerAnonymous>();
            anony.intermediary = p;
        }

        Texture avatar = null;
        if (p != null)
        {
            p.AvatarTexture(delegate(Texture _texture) { avatar = _texture; });
        }

        string username = p != null ? p.username : "";
        kickAvatar[i].mainTexture = swappedAvatar[i].mainTexture = avatar;

        Color white = Color.white;
        if (p == null)
            white.a = 0f;
        kickAvatar[i].color = swappedAvatar[i].color = white;

        kickUsername[i].text = swappedUsername[i].text = username;

        kickButton[i].gameObject.SetActive(p != null);
        kickAvatar[i].gameObject.SetActive(p != null);
    }
    public virtual void DoRequestKickPlayer(GameObject go)
    {
        UIContainerAnonymous anony = go.GetComponent<UIContainerAnonymous>();
        if (anony != null)
        {
            NotificationView.ShowConfirm("Xác nhận.", "Bạn có chắc rằng muốn đuổi người chơi " + ((EPlayerController)anony.intermediary).username + "\n\nRa khỏi bàn chơi không ?",
                delegate()
                {
                    GameManager.Server.DoRequestPluginGame(Utility.SetEsObject(Fields.GAMEPLAY.PLAY, new object[] {
                        Fields.ACTION, "kickPlayer",
                        Fields.PLAYER.USERNAME, ((EPlayerController)anony.intermediary).username
                    }));
                }, null);
        }
        else
            Debug.LogError("Không tìm được người chơi");
    }
    public virtual void OnClickSave(GameObject go)
    {
        GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("changeConfigGame", new object[] 
            { 
                Fields.ACTION, "changeConfig",
                "description",txtRoomName.value,
                "password", txtChangePassword.value
            }));
    }
    public virtual void OnStart()
    {

    }
    public virtual void OnUpdate()
    {

    }
    public virtual void SetActiveListToggle()
    {
        
    }  
}
