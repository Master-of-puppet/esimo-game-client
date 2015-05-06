using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YourButtonControllerPhom : EYourButtonController
{
    GameObject objStealCard = null;
    public override void UpdateButton()
    {
        GameModelPhom.game.deck.SetActive((int)GameModelPhom.CurrentState >= (int)GameModelPhom.EGameState.deal && GameModelPhom.DeckCount > 0);

        GameModelPhom.game.deck.transform.FindChild("btDrawInDeck").gameObject.SetActive(GameModelPhom.IsYourTurn && GameModelPhom.MiniState == GameModelPhom.EGameStateMini.stealCard_or_draw);

        if (GameModelPhom.YourController.PlayerState == PlayerControllerPhom.EPlayerState.waiting) return;

        if (GameModelPhom.DealCardDone == false) return;

        GameModelPhom.game.btSorted.gameObject.SetActive(GameModelPhom.CurrentState == GameModelPhom.EGameState.playing);

        #region NHOM 2
        UpdateButtonLayMeld();
        #endregion

        GameModelPhom.game.btStealCard.gameObject.SetActive(GameModelPhom.IsYourTurn && GameModelPhom.game.stolen);

        if (objStealCard == null && GameModelPhom.IsYourTurn && GameModelPhom.game.stolen)
        {
            int side = (int)GameModelPhom.GetPlayer(GameModelPhom.IndexLastInTurn).mSide;
            Transform trans = GameModelPhom.game.mPlaymat.locationTrash[side];
            objStealCard = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/btStealCardPrefab"));
            objStealCard.GetComponent<UIButtonMessage>().target = GameModelPhom.game.gameObject; //SendTo Gameplay Method OnProcessStealCard
            objStealCard.transform.parent = trans.parent;
            objStealCard.transform.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, -5);
            objStealCard.transform.localScale = Vector3.one;
        }
        else if (objStealCard != null) GameObject.Destroy(objStealCard);

        UpdateButtonDiscard();
    }

    public override void UpdateDeck()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateButtonDiscard()
    {
        GameModelPhom.game.btDiscard.SetStatus(GameModelPhom.CurrentState == GameModelPhom.EGameState.playing
           && GameModelPhom.game.btStealCard.gameObject.activeSelf == false
           ,
           GameModelPhom.game.ListClickCard.Count == 1 && GameModelPhom.CanDiscard(GameModelPhom.game.ListClickCard[0])
       );
    }
    public void UpdateButtonLayMeld()
    {
        GameModelPhom.game.btHaBai.gameObject.SetActive(GameModelPhom.IsYourTurn && GameModelPhom.MiniState == GameModelPhom.EGameStateMini.lay_meld && GameModelPhom.game.meldList.Count > 0
            &&
            (
                (((LobbyPhom)GameManager.Instance.selectedLobby).config.isAdvanceGame == false && GameModelPhom.game.ListClickCard.Count == 0) ||
                GameModelPhom.game.cardController.IsMeld
            )
        );

        #region GUI BAI
        GameModelPhom.game.btGuiBai.gameObject.SetActive(GameModelPhom.IsYourTurn && GameModelPhom.YourController.mCardMelds.Count > 0
            && GameModelPhom.game.btHaBai.gameObject.activeSelf == false
            && GameModelPhom.YourController.PlayerState == PlayerControllerPhom.EPlayerState.laying
            && GameModelPhom.game.listGiveCard.Count > 0
            &&
            (
                (((LobbyPhom)GameManager.Instance.selectedLobby).config.isAdvanceGame == false) ||
                (((LobbyPhom)GameManager.Instance.selectedLobby).config.isAdvanceGame && GameModelPhom.game.ListClickCard.Count == 1
                && GameModelPhom.game.listGiveCard.Find(gc => gc.cardId == GameModelPhom.game.ListClickCard[0].CardId) != null)
            )
        );

        GameModelPhom.game.btFulllaying.gameObject.SetActive(GameModelPhom.IsYourTurn && GameModelPhom.game.fullLaying
            && GameModelPhom.game.btHaBai.gameObject.activeSelf == false
            && GameModelPhom.YourController.mCardTrash.Count < 3
            && GameModelPhom.CurrentState == GameModelPhom.EGameState.playing);

        GameModelPhom.game.btDraw.SetStatus(
            GameModelPhom.CurrentState == GameModelPhom.EGameState.playing
            && GameModelPhom.game.btFulllaying.gameObject.activeSelf == false
            && GameModelPhom.game.btGuiBai.gameObject.activeSelf == false
            && GameModelPhom.game.btHaBai.gameObject.activeSelf == false
            ,
            GameModelPhom.IsYourTurn && GameModelPhom.MiniState == GameModelPhom.EGameStateMini.stealCard_or_draw);

        if (((LobbyPhom)GameManager.Instance.selectedLobby).config.isAdvanceGame)
            UpdateButtonAddMeld();
        #endregion
    }
    List<KeyValuePair<GamePlayPhom.GiveCard, GameObject>> listObjAddMeld = new List<KeyValuePair<GamePlayPhom.GiveCard, GameObject>>();
    public void UpdateButtonAddMeld()
    {
        while (listObjAddMeld.Count > 0)
        {
            GameObject.Destroy(listObjAddMeld[0].Value);
            listObjAddMeld.Remove(listObjAddMeld[0]);
        }

        if (GameModelPhom.game.listGiveCard.Count == 0) return;
        if (GameModelPhom.YourController.mCardMelds.Count == 0) return;

        if (GameModelPhom.game.ListClickCard.Count == 1 && GameModelPhom.game.listGiveCard.Find(gc => gc.cardId == GameModelPhom.game.ListClickCard[0].CardId) != null)
        #region CO BAN
        #endregion
        {
            #region ADVANCE
            foreach (GamePlayPhom.GiveCard give in GameModelPhom.game.listGiveCard.FindAll(gc => gc.cardId == GameModelPhom.game.ListClickCard[0].CardId))
            {
                PlayerControllerPhom p = GameModelPhom.GetPlayer(give.slotIndex);
                Meld meld = p.GetMeld(give.meldResponse);
                int indexMeld = p.mCardMelds.FindIndex(m => m == meld);

                int side = (int)GameModelPhom.GetPlayer(give.slotIndex).mSide;
                Transform trans = GameModelPhom.game.mPlaymat.locationMelds[side];
                Vector3 position = GameModelPhom.game.mPlaymat.GetLocationMeld(GameModelPhom.GetPlayer(give.slotIndex), indexMeld, 2);
                position.z = -17;

                GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/btAddMeldPrefab"));
                obj.GetComponent<UIButtonMessage>().target = GameModelPhom.game.gameObject; //SendTo Gameplay Method OnClickAddMeldCard
                UIContainerAnonymous contaniner = obj.AddComponent<UIContainerAnonymous>();
                contaniner.intermediary = give;

                obj.transform.parent = trans.parent;
                obj.transform.localPosition = position;
                obj.transform.localScale = Vector3.one;

                listObjAddMeld.Add(new KeyValuePair<GamePlayPhom.GiveCard, GameObject>(give, obj));
            }
            #endregion
        }
    }
}
