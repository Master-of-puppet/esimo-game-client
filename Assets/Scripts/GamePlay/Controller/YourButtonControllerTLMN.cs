using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YourButtonControllerTLMN : EYourButtonController {

    public override void UpdateButton()
    {
        GameModelTLMN.game.deck.SetActive((int)GameModelTLMN.CurrentState >= (int)GameModelTLMN.EGameState.deal && GameModelTLMN.DeckCount > 0);
        GameModelTLMN.game.deck.transform.FindChild("btDrawInDeck").gameObject.SetActive(GameModelTLMN.IsYourTurn && GameModelTLMN.MiniState == GameModelTLMN.EGameStateMini.stealCard_or_draw);
        if (GameModelTLMN.YourController.PlayerState == PlayerControllerTLMN.EPlayerState.waiting) return;
        if (GameModelTLMN.DealCardDone == false) return;
        GameModelTLMN.game.btChatHang.gameObject.SetActive(GameModelTLMN.game.allowChatHang != null && GameModelTLMN.game.allowChatHang == true);
        #region NHOM 2
        GameModelTLMN.game.btDraw.SetStatus( //Bt Bỏ qua
            GameModelTLMN.CurrentState == GameModelTLMN.EGameState.playing
            && GameModelTLMN.YourController.PlayerState != PlayerControllerTLMN.EPlayerState.finish
            ,
            GameModelTLMN.game.allowChatHang == null && GameModelTLMN.game.newTurn == false &&
            GameModelTLMN.IsYourTurn && GameModelTLMN.MiniState == GameModelTLMN.EGameStateMini.discard);

        #endregion

        UpdateButtonDiscard();
    }

    public override void UpdateDeck()
    {
        
    }

    public override void UpdateButtonDiscard()
    {
        if (!GameModelTLMN.game.isHideOneCard)
            GameModelTLMN.game.cardController.AutoPick();

        GameModelTLMN.game.btDiscard.SetStatus(GameModelTLMN.CurrentState == GameModelTLMN.EGameState.playing
            && GameModelTLMN.game.btChatHang.gameObject.activeSelf == false
            && GameModelTLMN.YourController.PlayerState != PlayerControllerTLMN.EPlayerState.finish
            ,
            GameModelTLMN.game.allowChatHang == null //Chặt hàng
            && GameModelTLMN.IsYourTurn
            && GameModelTLMN.game.ListClickCard.Count >= 1
            && (GameModelTLMN.MiniState == GameModelTLMN.EGameStateMini.discard)
            && GameModelTLMN.game.cardController.IsValid_TLMN
        );
        GameModelTLMN.game.btSorted.SetStatus(GameModelTLMN.CurrentState == GameModelTLMN.EGameState.playing && GameModelTLMN.YourController.PlayerState != PlayerControllerTLMN.EPlayerState.finish && GameModelTLMN.game.ListClickCard.Count == 0, GameModelTLMN.YourController.mCardHand.Count > 2);
        GameModelTLMN.game.btBoChon.gameObject.SetActive(GameModelTLMN.CurrentState == GameModelTLMN.EGameState.playing && GameModelTLMN.YourController.PlayerState != PlayerControllerTLMN.EPlayerState.finish && GameModelTLMN.game.ListClickCard.Count > 0);
    }
    List<KeyValuePair<GamePlayTLMN.GiveCard, GameObject>> listObjAddMeld = new List<KeyValuePair<GamePlayTLMN.GiveCard, GameObject>>();
    public override void UpdateButtonAddMeld()
    {
        while (listObjAddMeld.Count > 0)
        {
            GameObject.Destroy(listObjAddMeld[0].Value);
            listObjAddMeld.Remove(listObjAddMeld[0]);
        }

        if (GameModelTLMN.game.listGiveCard.Count == 0) return;
        if (GameModelTLMN.YourController.mCardMelds.Count == 0) return;

        if (GameModelTLMN.game.ListClickCard.Count == 1 && GameModelTLMN.game.listGiveCard.Find(gc => gc.cardId == GameModelTLMN.game.ListClickCard[0].CardId) != null)
        {
            #region ADVANCE
            foreach (GamePlayTLMN.GiveCard give in GameModelTLMN.game.listGiveCard.FindAll(gc => gc.cardId == GameModelTLMN.game.ListClickCard[0].CardId))
            {
                PlayerControllerTLMN p = GameModelTLMN.GetPlayer(give.slotIndex);
                Meld meld = p.GetMeld(give.meldResponse);
                int indexMeld = p.mCardMelds.FindIndex(m => m == meld);

                int side = (int)GameModelTLMN.GetPlayer(give.slotIndex).mSide;
                Transform trans = GameModelTLMN.game.mPlaymat.locationMelds[side];
                Vector3 position = GameModelTLMN.game.mPlaymat.GetLocationMeld(GameModelTLMN.GetPlayer(give.slotIndex), indexMeld, 2);
                position.z = -12;

                GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/btAddMeldPrefab"));
                obj.GetComponent<UIButtonMessage>().target = GameModelTLMN.game.gameObject; //SendTo Gameplay Method OnClickAddMeldCard
                UIContainerAnonymous contaniner = obj.AddComponent<UIContainerAnonymous>();
                contaniner.intermediary = give;

                obj.transform.parent = trans.parent;
                obj.transform.localPosition = position;
                obj.transform.localScale = Vector3.one;

                listObjAddMeld.Add(new KeyValuePair<GamePlayTLMN.GiveCard, GameObject>(give, obj));
            }
            #endregion
        }
    }
    public void UpdateButtonLayMeld()
    {
        if (GameManager.GAME == EGame.TLMN) return;

        GameModelTLMN.game.btHaBai.gameObject.SetActive(GameModelTLMN.IsYourTurn && GameModelTLMN.MiniState == GameModelTLMN.EGameStateMini.lay_meld && GameModelTLMN.game.meldList.Count > 0
            &&
            (
                (((LobbyTLMN)GameManager.Instance.selectedLobby).config.isAdvanceGame == false) ||
                (((LobbyTLMN)GameManager.Instance.selectedLobby).config.isAdvanceGame && GameModelTLMN.game.ListClickCard.Count > 2)
            )
        );

        #region GUI BAI
        GameModelTLMN.game.btGuiBai.gameObject.SetActive(GameModelTLMN.IsYourTurn && GameModelTLMN.YourController.mCardMelds.Count > 0
            && GameModelTLMN.game.btHaBai.gameObject.activeSelf == false
            && GameModelTLMN.YourController.PlayerState == PlayerControllerTLMN.EPlayerState.laying
            && GameModelTLMN.game.listGiveCard.Count > 0
            &&
            (
                (((LobbyTLMN)GameManager.Instance.selectedLobby).config.isAdvanceGame == false) ||
                (((LobbyTLMN)GameManager.Instance.selectedLobby).config.isAdvanceGame && GameModelTLMN.game.ListClickCard.Count == 1
                && GameModelTLMN.game.listGiveCard.Find(gc => gc.cardId == GameModelTLMN.game.ListClickCard[0].CardId) != null)
            )
        );

        if (((LobbyTLMN)GameManager.Instance.selectedLobby).config.isAdvanceGame)
            UpdateButtonAddMeld();
        #endregion
    }
}
