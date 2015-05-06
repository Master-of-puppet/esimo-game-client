using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CardControllerPhom : ECardController
{
    [HideInInspector]
    private CardLib.Model.Deck deck;

    CardLib.Model.Deck ECardController.deck
    {
        get { return deck; }
    }

    public CardLib.Model.Deck Deck
    {
        get { return deck; }
    }

    public List<CardLib.Model.Card> CardHand
    {
        get
        {
            return deck.PeekCards((from c in GameModelPhom.YourController.mCardHand select c.CardId).ToArray<int>());
        }
    }

    public List<CardLib.Model.Card> CardStolen
    {
        get { return deck.PeekCards((from c in GameModelPhom.YourController.mCardHand where c.originSide != c.currentSide select c.CardId).ToArray<int>()); }
    }

    public CardControllerPhom()
    {
        deck = new CardLib.Model.Deck(PhomLib.Utility.PhomLogicCenter.RANK_VALUES, PhomLib.Utility.PhomLogicCenter.SUIT_VALUES);
        deck.renew();
    }

    /// <summary>
    /// Lấy về danh sách các card có thể gửi
    /// </summary>
    public List<GamePlayPhom.GiveCard> ListGiveCard()
    {
        List<GamePlayPhom.GiveCard> listGiveCard = new List<GamePlayPhom.GiveCard>();

        if (GameModelPhom.YourController.mCardMelds.Count == 0)
            return listGiveCard;

        List<ECard> CardCheckInHand = GameModelPhom.YourController.mCardHand.FindAll(card => PhomLib.Utility.PhomLogicCenter.RequireCardInMeld(card.parentCard, CardHand, CardStolen) == false);

        foreach (PlayerControllerPhom player in GameModelPhom.ListPlayerInGame)
        {
            foreach (Meld melds in player.mCardMelds)
            {
                foreach (ECard card in CardCheckInHand)
                {
                    List<CardLib.Model.Card> listCheck = deck.PeekCards((from m in melds.meld select m.CardId).ToArray<int>());
                    listCheck.Add(deck.PeekCard(card.CardId));

                    if (PhomLib.Utility.PhomLogicCenter.caculateMeldType(listCheck) != PhomLib.Models.PhomMeldType.NONE)
                    {
                        GamePlayPhom.GiveCard giveCard = new GamePlayPhom.GiveCard();
                        giveCard.cardId = card.CardId;
                        giveCard.meld = melds;
                        giveCard.meldResponse = (from m in listCheck select m.Id).ToArray<int>();
                        giveCard.slotIndex = player.slotServer;

                        listGiveCard.Add(giveCard);
                    }
                }
            }
        }
        return listGiveCard;
    }
    /// <summary>
    /// Lấy về danh sách các phỏm có thể hạ
    /// </summary>
    public List<List<int>> ListMeld()
    {
        List<List<int>> listMeld = new List<List<int>>();

        foreach (PhomLib.Models.MeldsAndRemainingCards summary in PhomLib.Utility.PhomLogicCenter.caculateExistingMeldsInAllWay(CardHand, CardStolen))
        {
            foreach (PhomLib.Models.PhomMeld meld in summary.Melds)
            {
                listMeld.Add(new List<int>(meld.toIntArray()));
            }
        }
        return listMeld;
    }

    /// <summary>
    /// Lấy danh sách các trương hợp có thể sắp xếp
    /// </summary>
    public List<List<int>> SortCard()
    {
        List<List<int>> listWasSort = new List<List<int>>();

        //Danh sách đã sắp xếp.
        List<CardLib.Model.Card[]> sorts = PhomLib.Utility.PhomLogicCenter.arrangeCardsRankAndSuit(CardHand, CardStolen);

        foreach (CardLib.Model.Card[] s in sorts)
        {
            //string str = "";
            List<int> sortList = new List<int>();
            foreach (CardLib.Model.Card c in s)
            {
                sortList.Add(c.Id);
                //str += c.ToString() + " ";
            }
            //Debug.Log("** Sort: " + str);
            listWasSort.Add(sortList);
        }
        return listWasSort;
    }

    public bool CanDiscard(ECard card)
    {
        if (CardStolen.Find(c => c.Id == card.CardId) != null)
            return false;

        List<CardLib.Model.Card> cardHand = CardHand;
        cardHand.Remove(CardHand.Find(c => c.Id == card.CardId));
        try
        {
            PhomLib.Models.MeldsAndRemainingCards summary = PhomLib.Utility.PhomLogicCenter.caculateExistingMelds(cardHand, CardStolen);
            foreach (CardLib.Model.Card c in CardStolen)
            {
                if (summary == null)
                    return false;
                else if (summary.RemainingCards.Find(c1 => c1 != null && c1.Id == c.Id) != null)
                    return false;
            }
            return true;
        }
        catch { return false; }
    }
    public bool IsMeld
    {
        get
        {
            if (GameModelPhom.game.ListClickCard.Count == 0)
                return false;
            List<CardLib.Model.Card> clickedCard = deck.PeekCards((from c in GameModelPhom.game.ListClickCard select c.CardId).ToArray<int>());
            return PhomLib.Utility.PhomLogicCenter.CheckMeldAndStolensCard(clickedCard, CardHand, CardStolen);
        }
    }
}
