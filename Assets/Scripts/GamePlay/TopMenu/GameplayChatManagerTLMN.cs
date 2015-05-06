using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameplayChatManagerTLMN : EGameplayChatManager
{
    Dictionary<PlayerControllerTLMN, PlayerChat> playerChats = new Dictionary<PlayerControllerTLMN, PlayerChat>();
    public override void OnPublicMessage(Electrotank.Electroserver5.Api.PublicMessageEvent e)
    {
        #region VIEW CONTENT CHAT

        if (e.UserName != GameManager.Instance.mInfo.username)
        {
            PlayerControllerTLMN player = GameModelTLMN.GetPlayer(e.UserName);
            if (player != null)
            {
                if (playerChats.ContainsKey(player))
                {
                    if (playerChats[player] != null)
                        playerChats[player].DestroyMe();
                    playerChats.Remove(player);
                }
                playerChats.Add(GameModelTLMN.GetPlayer(e.UserName), PlayerChat.Create(e.Message, GameModelTLMN.GetPlayer(e.UserName)));
            }

        }
        #endregion
        str = "[#FFCC00]" + e.UserName.ToUpper() + ":[-] " + e.Message + "\n";
        base.OnPublicMessage(e);
    }
}
