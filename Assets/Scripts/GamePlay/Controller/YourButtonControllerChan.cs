using UnityEngine;
using System.Collections;

public class YourButtonControllerChan : EYourButtonController
{
    public override void UpdateButton()
    {
        UpdateDeck();

        if (GameModelChan.YourController == null || GameModelChan.YourController.PlayerState == EPlayerController.EPlayerState.waiting) return;

        if (GameModelChan.DealCardDone == false) return;
        UpdateButtonDiscard();
    }

    public override void UpdateDeck()
    {
        GameModelChan.game.deck.SetActive(GameModelChan.CurrentState >= GameModelChan.EGameState.playing && GameModelChan.DealCardDone && ListCuocUView.IsShowing == false && ListResultXuongView.IsShowing == false);
        if (GameModelChan.game.deck.activeSelf)
        {
            GameModelChan.game.deck.transform.FindChild("3. Number").GetComponent<UILabel>().text = GameModelChan.DeckCount.ToString();
            GameModelChan.game.deck.transform.FindChild("2. Card").gameObject.SetActive(true);
        }
    }

    public override void UpdateButtonDiscard()
    {
        GameModelChan.game.btDiscard.SetStatus(GameModelChan.CurrentState == GameModelChan.EGameState.playing,
           GameModelChan.IsYourTurn && GameModelChan.game.ListClickCard.Count == 1
           && GameModelChan.MiniState == GameModelChan.EGameStateMini.discard);

        GameModelChan.game.btDuoi.SetStatus(GameModelChan.CurrentState == GameModelChan.EGameState.playing
            ,
            GameModelChan.IsYourTurn && GameModelChan.MiniState == GameModelChan.EGameStateMini.stealCard_or_skip);

        GameModelChan.game.btChiu.SetStatus(GameModelChan.CurrentState == GameModelChan.EGameState.playing
            , GameModelChan.game.ListClickCard.Count == 0);

        GameModelChan.game.btSorted.gameObject.SetActive(GameModelChan.CurrentState == GameModelChan.EGameState.playing
            && GameModelChan.game.canRequestSort);

        GameModelChan.game.btStealCard.gameObject.SetActive(GameModelChan.CurrentState == GameModelChan.EGameState.playing
            && (GameModelChan.MiniState == GameModelChan.EGameStateMini.stealCard_or_draw || GameModelChan.MiniState == GameModelChan.EGameStateMini.stealCard_or_skip)
            && GameModelChan.IsYourTurn && GameModelChan.game.ListClickCard.Count == 1);

        GameModelChan.game.btFulllaying.gameObject.SetActive(
            GameModelChan.game.btSorted.gameObject.activeSelf == false
            && GameModelChan.game.btStealCard.gameObject.activeSelf == false
            && (GameModelChan.CurrentState == GameModelChan.EGameState.playing
                || (GameModelChan.CurrentState == GameModelChan.EGameState.playerFullLaying && GameModelChan.MiniState == GameModelChan.EGameStateMini.player_full_laying)
            ));

        GameModelChan.game.btDraw.gameObject.SetActive(GameModelChan.CurrentState == GameModelChan.EGameState.playing
            && GameModelChan.IsYourTurn && GameModelChan.MiniState == GameModelChan.EGameStateMini.stealCard_or_draw);

        GameModelChan.game.btView.gameObject.SetActive(GameModelChan.MiniState == GameModelChan.EGameStateMini.wait_player_xuong
            && GameModelChan.game.listCardInNoc.Count > 0 && ListCuocUView.IsShowing == false && ListResultXuongView.IsShowing == false);
    }
}
