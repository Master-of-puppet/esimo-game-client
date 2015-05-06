using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public sealed class CardControllerTLMN : ECardController
{
    private CardLib.Model.Deck deck;

    CardLib.Model.Deck ECardController.deck
    {
        get { return deck; }
    }
    public CardLib.Model.Deck Deck
    {
        get { return deck; }
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

    public CardControllerTLMN()
    {
        deck = new CardLib.Model.Deck(PhomLib.Utility.PhomLogicCenter.RANK_VALUES, PhomLib.Utility.PhomLogicCenter.SUIT_VALUES);
        deck.renew();
    }

    public List<CardLib.Model.Card> CardHand
    {
        get { return deck.PeekCards((from c in GameModelTLMN.YourController.mCardHand select c.CardId).ToArray<int>()); }
    }

    public System.Collections.Generic.List<CardLib.Model.Card> CardStolen
    {
        get { return deck.PeekCards((from c in GameModelTLMN.YourController.mCardHand where c.originSide != c.currentSide select c.CardId).ToArray<int>()); }
    }
    /// <summary>
    /// Kiểm tra xem trước đó đánh là cạ gì
    /// </summary>
    public EMultiType TypeLastDiscard
    {
        get
        {
            List<ECard> lastDiscard = new List<ECard>(GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp][GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp].Count - 1]);
            lastDiscard.Sort((c1, c2) => c1.CompareTo(c2));
            return TickMultiCard(lastDiscard);
        }
    }

    /// <summary>
    /// Lấy danh sách card bốn đôi thông từ tay
    /// </summary>
    public List<ECard> GetFourPairs()
    {
        List<ECard> list = new List<ECard>();

        List<CardLib.Model.Card> listCard = TLMNLogicCenter.GetPair(CardHand);
        if (listCard.Count < 8)
            return list;

        foreach (CardLib.Model.Card card in listCard)
            list.Add((TLMNCard)GameModelTLMN.YourController.mCardHand.Find(c => c.parentCard == card));

        return list;
    }

    /// <summary>
    /// Tự động pick các card
    /// </summary>
    public void AutoPick()
    {
        List<ECard> listCard = new List<ECard>();

        if (GameModelTLMN.game.ListClickCard.Count == 2)
        {
            List<ECard> listClickCard = new List<ECard>(GameModelTLMN.game.ListClickCard);
            listClickCard.Sort((c1, c2) => c1.CompareTo(c2));
            ECard cardStart = listClickCard[0], cardEnd = listClickCard[1];
            List<ECard> pickCardOnHand = (from c in GameModelTLMN.YourController.mCardHand where c.CompareTo(cardStart) >= 0 && c.CompareTo(cardEnd) <= 0 select c).ToList<ECard>();
            pickCardOnHand.Sort((c1, c2) => c1.CompareTo(c2));

            listCard = new List<ECard>(GetStaight(pickCardOnHand));

            if (listCard.Count >= 3)
            {
                if (listCard[listCard.Count - 1].parentCard.Suit.Value != cardEnd.parentCard.Suit.Value)
                {
                    listCard.RemoveAt(listCard.Count - 1);
                    listCard.Add(cardEnd);
                }
                if (listCard[0].parentCard.Suit.Value != cardStart.parentCard.Suit.Value)
                {
                    listCard.RemoveAt(0);
                    listCard.Add(cardStart);
                }
            }
            //string str = "";
            //foreach (Card c in listCard)
            //    str += c.parentCard.ToString();
            //Debug.Log(str);
        }

        foreach (TLMNCard c in listCard)
            if (!GameModelTLMN.game.ListClickCard.Contains(c))
                GameModelTLMN.game.CardClick(c);
    }

    /// <summary>
    /// Kiểm tra xem card click có hợp lệ hay không ?
    /// </summary>
    /// <returns></returns>
    public bool IsValid_TLMN
    {
        get { return IsValidInTLMN(GameModelTLMN.game.ListClickCard); }
    }
    public bool IsValidInTLMN(List<ECard> checkList)
    {
        List<ECard> clickCard = new List<ECard>(checkList);

        if (clickCard.Count == 0)
            return false;

        clickCard.Sort((c1, c2) => c1.CompareTo(c2));

        if (GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp].Count == 0 || GameModelTLMN.game.newTurn) //Turn mới
            return TickMultiCard(clickCard) != EMultiType.None;
        else
        {
            List<ECard> lastDiscard = new List<ECard>(GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp][GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp].Count - 1]);
            lastDiscard.Sort((c1, c2) => c1.CompareTo(c2));

            if (Contains2(lastDiscard)) //Nếu có hai thì chắc hàng (3,4 đôi thông) hoặc hai to hơn
            {
                if (lastDiscard.Count == 1) //Là đang chọn ba đôi thông hoặc là hai to hơn hoặc tứ quý
                {
                    EMultiType state = TickMultiCard(clickCard);

                    //Debug.Log(state);

                    return state == EMultiType.MoreThreePairs
                        || state == EMultiType.FourPairs
                        || (state == EMultiType.Single && lastDiscard[0].CompareTo(clickCard[0]) < 0)
                        || (state == EMultiType.Horizontal && clickCard.Count == 4);
                }
                else if (lastDiscard.Count == 2)
                {
                    //Nếu trước đó đánh đôi hai thì có thể chặt bằng tứ quý hoặc bốn đôi thông.
                    if (TickMultiCard(clickCard) == EMultiType.FourPairs || (TickMultiCard(clickCard) == EMultiType.Horizontal && clickCard.Count == 4))
                        return true;
                }
                else return false;
            }
            else if (lastDiscard.Count == 6 && TickMultiCard(lastDiscard) == EMultiType.MoreThreePairs)
            {
                if (TickMultiCard(clickCard) == EMultiType.FourPairs || (TickMultiCard(clickCard) == EMultiType.Horizontal && clickCard.Count == 4))
                    return true;
            }
            else if (TickMultiCard(clickCard) == EMultiType.Horizontal && clickCard.Count == 4)
            {
                //Trước đó tứ quý có thể chặt bằng 4 đôi thông
                if (TickMultiCard(clickCard) == EMultiType.FourPairs)
                    return true;
            }

            return lastDiscard.Count == clickCard.Count
                && lastDiscard[lastDiscard.Count - 1].CompareTo(clickCard[clickCard.Count - 1]) < 0
                && TickMultiCard(lastDiscard) == TickMultiCard(clickCard);
        }
    }

    /// <summary>
    /// Lấy sảnh  dọc từ các card truyền vào
    /// </summary>
    /// <param name="list">Danh sách card truyền vào</param>
    /// <returns>Sảnh dọc trả về sau khi kiểm tra</returns>
    public List<ECard> GetStaight(List<ECard> list)
    {
        List<ECard> lst = new List<ECard>();

        if (list.Count < 3) return lst;

        List<List<ECard>> listOfList = ListOfListCard(list);
        foreach (List<ECard> l in listOfList)
        {
            if (lst.Count > 0 && ((TLMNCard)lst[lst.Count - 1]).IsNextRank((TLMNCard)l[0]) == false)
                return new List<ECard>();

            lst.Add(l[0]);
        }

        ///Phòng trường hợp lấy được có hai cây.
        if (lst.Count < 3) return new List<ECard>();

        lst.Sort((c1, c2) => c1.CompareTo(c2));
        return lst;
    }

    public List<List<ECard>> ListOfListCard(List<ECard> list)
    {
        Dictionary<int, List<ECard>> dict = new Dictionary<int, List<ECard>>();

        List<ECard> listCard = new List<ECard>(list);
        listCard.Sort((c1, c2) => c1.CompareTo(c2));
        foreach (ECard c in listCard)
        {
            int key = c.parentCard.Rank.Value;

            if (dict.ContainsKey(key))
                dict[key].Add(c);
            else
                dict.Add(key, new List<ECard>(new ECard[] { c }));
        }

        List<List<ECard>> listSort = new List<List<ECard>>();
        foreach (List<ECard> lst in dict.Values)
            listSort.Add(lst);

        return listSort;
    }

    public List<GamePlayTLMN.GiveCard> ListGiveCard()
    {
        List<GamePlayTLMN.GiveCard> listGiveCard = new List<GamePlayTLMN.GiveCard>();

        if (GameModelTLMN.YourController.mCardMelds.Count == 0)
            return listGiveCard;

        List<ECard> CardCheckInHand = GameModelTLMN.YourController.mCardHand.FindAll(card => PhomLib.Utility.PhomLogicCenter.RequireCardInMeld(card.parentCard, CardHand, CardStolen) == false);

        foreach (PlayerControllerTLMN player in GameModelTLMN.ListPlayerInGame)
        {
            foreach (Meld melds in player.mCardMelds)
            {
                foreach (TLMNCard card in CardCheckInHand)
                {
                    List<CardLib.Model.Card> listCheck = deck.PeekCards((from m in melds.meld select m.CardId).ToArray<int>());
                    listCheck.Add(deck.PeekCard(card.CardId));

                    if (PhomLib.Utility.PhomLogicCenter.caculateMeldType(listCheck) != PhomLib.Models.PhomMeldType.NONE)
                    {
                        GamePlayTLMN.GiveCard giveCard = new GamePlayTLMN.GiveCard();
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

    public List<List<int>> SortCard()
    {
        List<List<int>> listWasSort = new List<List<int>>();

        //Danh sách đã sắp xếp.
        List<CardLib.Model.Card[]> sorts;
        if (GameManager.GAME == EGame.Phom)
            sorts = PhomLib.Utility.PhomLogicCenter.arrangeCardsRankAndSuit(CardHand, CardStolen);
        else
            sorts = TLMNLogicCenter.arrangeCards(CardHand);

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

    public enum EMultiType
    {
        None,               //Dây không hợp lệ
        Single,             //Một cây
        Horizontal,         //Cạ nằm ngang (đôi, ba)
        Vertical,           //Cạ nằm dọc (sảnh)
        MoreThreePairs,     //Hơn Ba đôi thông
        FourPairs           //Bốn đôi thông
    }
    EMultiType TickMultiCard(List<ECard> lst)
    {
        if (lst.Count == 1)
            return EMultiType.Single;
        else if (IsHorizontal(lst))
            return EMultiType.Horizontal;
        else if (IsVertical(lst))
            return EMultiType.Vertical;
        else if (IsFourPairs(lst))
            return EMultiType.FourPairs;
        else if (IsThreePairs(lst))
            return EMultiType.MoreThreePairs;
        else
            return EMultiType.None;
    }
    bool IsVertical(List<ECard> lst)
    {
        if (lst.Count <= 2) return false;
        if (Contains2(lst)) return false;

        int index = 0;
        while (index < lst.Count - 1)
        {
            int nextIndex = index + 1;

            if (lst[index].parentCard.Rank.Value == 12 && lst[nextIndex].parentCard.Rank.Value == 0)
            {
                index = nextIndex;
                continue;
            }
            if (((TLMNCard)lst[index]).IsNextRank((TLMNCard)lst[nextIndex]) == false)
                return false;
            index = nextIndex;
        }
        return true;
    }
    bool IsHorizontal(List<ECard> lst)
    {
        if (lst.Count > 4)
            return false;
        if (lst[0].parentCard.Rank.Value != lst[lst.Count - 1].parentCard.Rank.Value)
            return false;
        return true;
    }
    bool IsThreePairs(List<ECard> lst)
    {
        if (lst.Count % 2 != 0) return false;
        if (lst.Count < 6) return false;
        if (Contains2(lst)) return false;

        int index = 0;
        while (index < lst.Count)
        {
            int nextIndex = index + 1;
            if (lst[index].parentCard.Rank.Value != lst[nextIndex].parentCard.Rank.Value)
                return false;

            if (nextIndex + 1 < lst.Count)
                if (((TLMNCard)lst[nextIndex]).IsNextRank((TLMNCard)lst[nextIndex + 1]) == false)
                    return false;

            index += 2;
        }
        return true;
    }
    bool IsFourPairs(List<ECard> lst)
    {
        return IsThreePairs(lst) && lst.Count == 8;
    }
    /// <summary>
    /// Kiểm tra xem card đánh có phải là 2 hay không ?
    /// </summary>
    public bool Contains2(List<ECard> lst)
    {
        return lst.Find(c => c.CardId >= 4 && c.CardId <= 7) != null;
    }
}
