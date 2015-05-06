using System.Collections.Generic;

public class Meld
{
    public List<ECard> meld = new List<ECard>();
    public EPlayerController player;
    public Meld(int[] lst, EPlayerController p)
    {
        player = p;
        foreach (int value in lst)
        {
            ECard card = p.mCardHand.Find(c => c.CardId == value);
            if (card == null)
            {
                card = p.mCardHand.Find(c => c.CardId == -1);
                if (card != null)
                    card.CardId = value;
            }
            p.mCardHand.Remove(card);
            meld.Add(card);
        }
    }
}