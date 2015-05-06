using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class CardControllerChan : ECardController
{
    private CardLib.Model.Deck deck;
    public CardControllerChan()
    {
        deck = new CardLib.Model.Deck(PhomLib.Utility.PhomLogicCenter.RANK_VALUES, PhomLib.Utility.PhomLogicCenter.SUIT_VALUES);
        deck.renew();
    }
    CardLib.Model.Deck ECardController.deck
    {
        get { return deck ; }
    }

    public List<CardLib.Model.Card> CardHand
    {
        get { return deck.PeekCards((from c in GameModelChan.YourController.mCardHand select c.CardId).ToArray<int>()); }
    }

    public List<CardLib.Model.Card> CardStolen
    {
        get { throw new System.NotImplementedException(); }
    }

    public List<List<int>> SortCard()
    {
        return null;
    }

    public void SortChan()
    {
        List<List<ECard>> listOfListCard = new List<List<ECard>>();
        List<ECard> CardOnHand = new List<ECard>(GameModelChan.YourController.mCardHand);
        //Nhóm các quân bài dạng chắn
        FiltersPairs(CardOnHand, listOfListCard, c => c.CardId == CardOnHand[0].CardId);
        //lọc các chắn chờ chíu
        Filters(listOfListCard, CardOnHand);
        //Lọc các chắn chỉ có đôi
        Filters(listOfListCard, CardOnHand, l => l.Count > 0 && l.Count % 2 == 0);
        //Lấy các quân lẻ còn lại không tạo thành chắn
        List<ECard> listOfCard = (from list in listOfListCard select list[0]).ToList<ECard>();
        //Nhóm các quân bài dạng cạ
        FiltersPairs(listOfCard, listOfListCard, c => c.CardValue == listOfCard[0].CardValue);
        //Lọc các cạ chỉ có đôi 
        Filters(listOfListCard, CardOnHand, l => l.Count > 0 && l.Count % 2 == 0);
        //Lọc ba đầu
        Filters(listOfListCard, CardOnHand, l => l.Count == 3);
        //Lọc các quân bài què còn lại
        Filters(listOfListCard, CardOnHand, l => l.Count == 1);
        if (GameModelChan.YourController != null)
           GameModelChan.YourController.mCardHand = CardOnHand;
    }


    public bool CanDiscard(ECard card)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Trường hợp 3 con giống nhau chờ chíu
    /// </summary>
    void Filters(List<List<ECard>> fromList, List<ECard> toList)
    {
        while (fromList.FindAll(l => l.Count == 3).Count > 0)
        {
            List<ECard> list = fromList.Find(l => l.Count == 3);
            list.Sort((c1, c2) => c1.CompareTo(c2));
            toList.Add(list[0]);
            toList.Add(list[1]);
            fromList.Add(new List<ECard>(new ECard[] { list[2] }));
            fromList.Remove(list);
        }
    }
    /// <summary>
    /// Trường hợp lọc lấy chắn hoặc cạ với điều kiện
    /// </summary>
    void Filters(List<List<ECard>> fromList, List<ECard> toList, Predicate<List<ECard>> pre)
    {
        while (fromList.FindAll(pre).Count > 0)
        {
            List<ECard> list = fromList.Find(pre);
            list.Sort((c1, c2) => c1.CompareTo(c2));
            toList.AddRange(list);
            fromList.Remove(list);
        }
    }
    /// <summary>
    /// Trường hợp lọc tách ra theo nhóm (để lấy chắn hoặc cạ)
    /// </summary>
    void FiltersPairs(List<ECard> fromList, List<List<ECard>> toList, Predicate<ECard> pre)
    {
        toList.Clear();
        fromList.Sort((c1, c2) => c1.CompareTo(c2));
        while (fromList.Count > 0)
        {
            List<ECard> list = fromList.FindAll(pre);
            list.Sort((c1, c2) => c1.CompareTo(c2));
            foreach (ECard c in list)
                fromList.Remove(c);
            toList.Add(list);
        }
        fromList.Clear();
    }

    
}
