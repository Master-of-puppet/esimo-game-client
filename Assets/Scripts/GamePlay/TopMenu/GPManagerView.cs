using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Electrotank.Electroserver5.Api;

public class GPManagerView : MonoBehaviour
{
    #region Unity Editor
    public UIInput txtChangePassword , txtRoomName;
    public UITexture[] swappedAvatar;
    public UILabel[] swappedUsername;
    public UITexture[] kickAvatar;
    public UILabel[] kickUsername;
    public CUIHandle[] kickButton;
    public CUIHandle btSave;
    public UIGrid gridWaitingPlayer;
    List<PlayerWaitingInManager> waitingPlayers = new List<PlayerWaitingInManager>();
    #endregion
    void Awake()
    {
        CUIHandle.AddClick(kickButton, DoRequestKickPlayer);
        CUIHandle.AddClick(btSave, OnClickSave);
        gridWaitingPlayer.onReposition += OnRepositionGridComplete;
    }
    
    IEnumerator _DrawListWaitingPlayer() 
    {
        while (waitingPlayers.Count > 0) 
        {
            GameObject.Destroy(waitingPlayers[0].gameObject);
            waitingPlayers.RemoveAt(0);
        }
        yield return new WaitForEndOfFrame();
        int i = 0;
        foreach (PlayerControllerChan player in GameModelChan.ListWaitingPlayer)
        {
            //PlayerWaitingInManager playerWaiting = PlayerWaitingInManager.Create(player, gridWaitingPlayer,this);
            //playerWaiting.gameObject.name = i + " " + player.username;
            //waitingPlayers.Add(playerWaiting);
            //i++;

        }
        gridWaitingPlayer.Reposition();
    }
    public void DrawListWaitingPlayer() 
    {
        StartCoroutine(_DrawListWaitingPlayer());
    }
    private void OnRepositionGridComplete()
    {
        gridWaitingPlayer.transform.localPosition = new Vector3(gridWaitingPlayer.transform.localPosition.x, -170f, gridWaitingPlayer.transform.localPosition.z);
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(kickButton, DoRequestKickPlayer);
        CUIHandle.RemoveClick(btSave, OnClickSave);
        gridWaitingPlayer.onReposition -= OnRepositionGridComplete;
    }

    void Update()
    {
        if ((string.IsNullOrEmpty(txtRoomName.value) || txtRoomName.value == ((LobbyChan)GameManager.Instance.selectedLobby).nameLobby) && string.IsNullOrEmpty(txtChangePassword.value))
        {
            if (!btSave.gameObject.collider.enabled) return;
            btSave.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 90 / 255f);
            btSave.gameObject.collider.enabled = false;
        }
        else
        {
            if (btSave.gameObject.collider.enabled) return;
            btSave.GetComponentInChildren<UISprite>().color = Color.white;
            btSave.gameObject.collider.enabled = true;
        }
    }

    void Start()
    {
        OnSwapSlot(GameModelChan.ListPlayer.ToArray());
        DrawListWaitingPlayer();
        txtRoomName.value = ((LobbyChan)GameManager.Instance.selectedLobby).nameLobby;
        txtChangePassword.value = ((LobbyChan)GameManager.Instance.selectedLobby).config.password;
    }
    void SetDataPlayer(EPlayerController p, int i)
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
        
        int slot = GameModelChan.YourController.slotServer;
        slot += (i + 1);
        if (slot >= 4)
            slot -= 4;

        swappedAvatar[i].gameObject.GetComponent<UIContainerAnonymous>().valueInt = slot;
    }

    /// <summary>
    /// Gửi request kick người chơi
    /// </summary>
    void DoRequestKickPlayer(GameObject go)
    {
        if (GameModelChan.CurrentState >= GameModelChan.EGameState.dealClient)
        {
            NotificationView.ShowMessage("Bạn không thể đuổi người chơi khác trong khi ván bài đang diễn ra.");
            return;
        }

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

    public void OnSwapSlot(EPlayerController[]listPlayer)
    {
        if (GameModelChan.game != null)
        {
            for (int i = 0; i <= 2; i++)
            {
                EPlayerController p = Array.Find<EPlayerController>(listPlayer, player => player != null && player.mSide == (ESide)(i + 1));
                SetDataPlayer(p, i);
            }
        }
    }
    internal void SetActiveListToggle()
    {
        foreach (PlayerWaitingInManager toggle in waitingPlayers) 
        {
            toggle.GetComponent<UIToggle>().value = ((PlayerControllerChan)toggle.player).isPriority;
        }
    }   
    public void OnPlayerListChanged(EPlayerController p, bool isRemove)
    {
        if (isRemove)
        {
            if (p.mSide == ESide.None)
            {
                PlayerWaitingInManager playerView = waitingPlayers.Find(plc => plc.player.username == p.username);
                if (playerView != null) 
                {
                    GameObject.Destroy(playerView.gameObject);
                    waitingPlayers.Remove(playerView);
                }
                GameManager.Instance.FunctionDelay(delegate() { gridWaitingPlayer.Reposition(); }, 0.01f);
            }
            else 
            {
                if (GameModelChan.CurrentState == GameModelChan.EGameState.waitingForPlayer || GameModelChan.CurrentState == GameModelChan.EGameState.waitingForReady)
                {
                    int i = ((int)p.mSide - 1);
                    kickAvatar[i].mainTexture = swappedAvatar[i].mainTexture = null;
                    kickAvatar[i].color = swappedAvatar[i].color = Color.black;
                    kickUsername[i].text = swappedUsername[i].text = "";
                    kickButton[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (p.mSide == ESide.None) 
            {
                //waitingPlayers.Add(PlayerWaitingInManager.Create(p, gridWaitingPlayer,this));
                //gridWaitingPlayer.Reposition();
            }
            else
            {
                int i = 0;
                if (p.mSide != ESide.Slot_0)
                    i = ((int)p.mSide - 1);

                SetDataPlayer(p, i);
            }
        }
    }
    
    void OnClickSave(GameObject go)
    {
        if(string.IsNullOrEmpty(txtRoomName.value)) return;

        if (((string.IsNullOrEmpty(((LobbyChan)GameManager.Instance.selectedLobby).config.password)) && (string.IsNullOrEmpty(txtChangePassword.value) || txtChangePassword.value == ((LobbyChan)GameManager.Instance.selectedLobby).config.password))
            && txtRoomName.value == GameManager.Instance.selectedLobby.roomName )
            return;
        else
        {
            GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("changeConfigGame", new object[] 
            { 
                Fields.ACTION, "changeConfig",
                "description",txtRoomName.value,
                "password", txtChangePassword.value
            }));
        }
        //List<string> lstStr = new List<string>();
        //listUserOnline.ForEach(u => {
        //    if(u.value) lstStr.Add(u.mUser.username);
        //});

        //if (lstStr.Count > 0)
        //{
        //    GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("invitePlayGame", new object[] {
        //        "invitedUsers", lstStr.ToArray(),
        //        "zoneId", GameManager.Instance.selectedChannel.zoneId,
        //        "roomId", GameManager.Instance.selectedChannel.roomId
        //    }));
        //}		
    }




}
