using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameplayChatManagerChan : EGameplayChatManager
{
    Dictionary<PlayerControllerChan, PlayerChat> playerChats = new Dictionary<PlayerControllerChan, PlayerChat>();
    public override void OnPublicMessage(Electrotank.Electroserver5.Api.PublicMessageEvent e)
    {
        #region VIEW CONTENT CHAT

        if (e.UserName != GameManager.Instance.mInfo.username)
        {
            PlayerControllerChan player = GameModelChan.GetPlayer(e.UserName);
            if (player != null)
            {
                if (playerChats.ContainsKey(player))
                {
                    if (playerChats[player] != null)
                        playerChats[player].DestroyMe();
                    playerChats.Remove(player);
                }
                playerChats.Add(GameModelChan.GetPlayer(e.UserName), PlayerChat.Create(e.Message, GameModelChan.GetPlayer(e.UserName)));
            }

        }
        #endregion
        str = string.Format("{0}" + e.UserName.ToUpper() + ":[-] " + e.Message + "\n", GameModelChan.ListWaitingPlayer.Find(plc => plc.username == e.UserName) != null ? "[FFCC00]" : "[FFCC00]");
        base.OnPublicMessage(e);
    }
}
