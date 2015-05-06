using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Interface Model for GamePlay
/// </summary>
public interface IGameModel  {
    List<EPlayerController> ListPlayer { get; }
    bool DealCardDone { get; set; }
    int DeckCount { get; set; }
    bool IsQuitWhenEndGame { get; set; }
    bool IsYourTurn { get; }
    List<EPlayerController> ListJoinGameWhenPlaying { get; set; }
    List<EPlayerController> ListPlayerInGame { get; }
    List<EPlayerController> ListPlayerInGameEnemy { get; }
    List<int> ListSlotEmpty { get; }
    void CreateNewGame();
    IEnumerator OnFinishGame();
    bool CanDiscard(ECard check);
    bool CanDiscard(List<ECard> cards);
    void Discard(int soundId, int index, int cardValue, params string[] discardToPlayer);
    void Discard(int index, int[] cardValue);
    void DrawCard(int index, int cardValue, int timeExpire);
    void DestroyObject();
    void DrawInfoPlayerNoSlot();
    ECard GetCard_FromHandPlayer(int indexPlayer, int cardId);
    EPlayerController GetLastPlayer(int slotCheck);
    EPlayerController GetNextPlayer(int slotCheck);
    EPlayerController GetNextPlayer(string username);
    EPlayerController GetPlayer(int index);
    EPlayerController GetPlayer(string username);
    void HideAvatarPlayer(bool isResult);
    IEnumerator PlayerLeftOut(EPlayerController p);
    void SetPlayer(int index, EPlayerController p);
    void SetPlayer(EPlayerController[] _newListPlayer);
    ESide SlotToSide(int slotServer);
    void SortHand();
    void StealCard(int index, int indexLast, int[] cardId);
    void StealCard(int index, int indexLast);
    void UpdatePlayerSide();
}
