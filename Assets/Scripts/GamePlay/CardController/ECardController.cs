using System.Collections;
using System.Collections.Generic;
public interface ECardController {
    CardLib.Model.Deck deck { get; }
    List<CardLib.Model.Card> CardHand { get; }
    List<CardLib.Model.Card> CardStolen { get; }
    List<List<int>> SortCard();
    bool CanDiscard(ECard card);
}
