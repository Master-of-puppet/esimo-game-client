using UnityEngine;
using System.Collections;

public sealed class PhomCardTexture : ECardTexture {
    public override void OnUpdate()
    {
        
    }

    public override void SetValue()
    {
        texture.mainTexture = (Texture)Resources.Load("Images/BigCard/" + card.CardId, typeof(Texture));
        texture.shader = (Shader)Resources.Load("Shaders/Unlit - Transparent Colored", typeof(Shader));
    }

    public override void OnClick()
    {
        if (gameObject.GetComponent<iTween>() && gameObject.GetComponent<iTween>().isRunning)
            return;

        if (enabled && card.CardId != -1)
        {
            if (GameModelPhom.CurrentState != GameModelPhom.EGameState.playing)
                return;

            //if (GameManager.GAME == EGame.Phom)
            //{
            if (//GameManager.Instance.selectedLobby.config.isAdvanceGame && 
                GameModelPhom.MiniState == GameModelPhom.EGameStateMini.lay_meld
                && GameModelPhom.game.meldList.Count > 0)
                GameModelPhom.game.CardClick(card);
            else
                GameModelPhom.game.CardClick(card.gameObject);
            //}
            //else
            //    GameModelPhom.game.CardClick(card);
        }
    }

    public override int getWith()
    {
        throw new System.NotImplementedException();
    }

    public override int getHeight()
    {
        throw new System.NotImplementedException();
    }
}
