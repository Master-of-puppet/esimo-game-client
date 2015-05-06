using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameplayChatManagerPhom : EGameplayChatManager
{
    Dictionary<PlayerControllerPhom, PlayerChat> playerChats = new Dictionary<PlayerControllerPhom, PlayerChat>();
    public override void OnPublicMessage(Electrotank.Electroserver5.Api.PublicMessageEvent e)
    {
        #region VIEW CONTENT CHAT

        if (e.UserName != GameManager.Instance.mInfo.username)
        {
            PlayerControllerPhom player = GameModelPhom.GetPlayer(e.UserName);
            if (player != null)
            {
                if (playerChats.ContainsKey(player))
                {
                    if (playerChats[player] != null)
                        playerChats[player].DestroyMe();
                    playerChats.Remove(player);
                }
                playerChats.Add(GameModelPhom.GetPlayer(e.UserName), PlayerChat.Create(e.Message, GameModelPhom.GetPlayer(e.UserName)));
            }

        }
        #endregion
        str = "[#FFCC00]" + e.UserName.ToUpper() + ":[-] " + e.Message + "\n";
        base.OnPublicMessage(e);
    }
}
