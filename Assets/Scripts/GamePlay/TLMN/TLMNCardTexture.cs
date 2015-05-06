using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class TLMNCardTexture : ECardTexture {
    
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
            if (GameModelTLMN.CurrentState != GameModelTLMN.EGameState.playing)
                return;

            //if (GameManager.GAME == EGame.Phom)
            //{
            //    if (//GameManager.Instance.selectedLobby.config.isAdvanceGame && 
            //        GameModel.MiniState == GameModel.EGameStateMini.lay_meld
            //        && GameModel.game.meldList.Count > 0)
            //        GameModel.game.CardClick(card);
            //    else
            //        GameModel.game.CardClick(card.gameObject);
            //}
            //else
            GameModelTLMN.game.CardClick(card);
        }
    }
    void OnDoubleClick()
    {
        if (gameObject.GetComponent<iTween>() && gameObject.GetComponent<iTween>().isRunning)
            return;

        if (enabled && card.CardId != -1)
        {
            if (GameModelTLMN.CurrentState != GameModelTLMN.EGameState.playing)
                return;

            int index = 0;
            int indexOfList = 0;
            List<List<int>> allList = GameModelTLMN.model.CardCollections.FindAll(l => l.Contains(card.CardId));
            if (allList.Count > 0)
            {
                while (GameModelTLMN.game.ListClickCard.Count > 0)
                    GameModelTLMN.game.CardClick(GameModelTLMN.game.ListClickCard[0]);

                for (int i = 0; i < allList.Count; i++)
                {
                    List<int> list = allList[i];
                    if (list.IndexOf(card.CardId) < indexOfList || i == 0)
                    {
                        indexOfList = list.IndexOf(card.CardId);
                        index = i;
                    }
                }

                allList[index].ForEach(cardId =>
                {
                    ECard cardClick = GameModelTLMN.YourController.mCardHand.Find(c => c.CardId == cardId);
                    if (cardClick != null)
                        GameModelTLMN.game.CardClick(cardClick);
                });
            }
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
