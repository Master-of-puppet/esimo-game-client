using UnityEngine;
using System.Collections;
using System;

public class GPManagerViewChan : EGPViewManager {
    public override void OnStart()
    {
        OnSwapSlot(GameModelChan.ListPlayer.ToArray());
        DrawListWaitingPlayer();
        txtRoomName.value = ((LobbyChan)GameManager.Instance.selectedLobby).nameLobby;
        txtChangePassword.value = ((LobbyChan)GameManager.Instance.selectedLobby).config.password;
    }
    public override void OnUpdate()
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
    public override void OnSwapSlot(EPlayerController[] listPlayer)
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
    public override void DoRequestKickPlayer(GameObject go)
    {
        if (GameModelChan.CurrentState >= GameModelChan.EGameState.dealClient)
        {
            NotificationView.ShowMessage("Bạn không thể đuổi người chơi khác trong khi ván bài đang diễn ra.");
            return;
        }
        base.DoRequestKickPlayer(go);
    }
    public override void OnClickSave(GameObject go)
    {
        if (string.IsNullOrEmpty(txtRoomName.value)) return;

        if (((string.IsNullOrEmpty(((LobbyChan)GameManager.Instance.selectedLobby).config.password)) && (string.IsNullOrEmpty(txtChangePassword.value) || txtChangePassword.value == ((LobbyChan)GameManager.Instance.selectedLobby).config.password))
            && txtRoomName.value == GameManager.Instance.selectedLobby.roomName)
            return;
        base.OnClickSave(go);
    }
    public override void OnPlayerListChanged(EPlayerController p, bool isRemove)
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
                waitingPlayers.Add(PlayerWaitingInManager.Create(p, gridWaitingPlayer, this));
                gridWaitingPlayer.Reposition();
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
    public override void SetDataPlayer(EPlayerController p, int i)
    {
        base.SetDataPlayer(p, i);
        int slot = GameModelChan.YourController.slotServer;
        slot += (i + 1);
        if (slot >= 4)
            slot -= 4;

        swappedAvatar[i].gameObject.GetComponent<UIContainerAnonymous>().valueInt = slot;
    }
    public override void DrawListWaitingPlayer()
    {
        StartCoroutine(_DrawListWaitingPlayer());
    }
    public override void SetActiveListToggle()
    {
        foreach (PlayerWaitingInManager toggle in waitingPlayers)
        {
            toggle.GetComponent<UIToggle>().value = ((PlayerControllerChan)toggle.player).isPriority;
        }
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
            PlayerWaitingInManager playerWaiting = PlayerWaitingInManager.Create(player, gridWaitingPlayer, this);
            playerWaiting.gameObject.name = i + " " + player.username;
            waitingPlayers.Add(playerWaiting);
            i++;
        }
        gridWaitingPlayer.Reposition();
    }
}
