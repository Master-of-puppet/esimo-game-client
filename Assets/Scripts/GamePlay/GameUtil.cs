using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
   public class GameUtil:MonoBehaviour
    {

        public enum ListCardID
        {
            NHI_VAN_J=0,
	        NHI_VAN_W=1,
	        NHI_SACH=2,
	
	        TAM_VAN_J=3,
	        TAM_VAN_W=4,
	        TAM_SACH=5,
	
	        TU_VAN_J=6,
	        TU_VAN_W=7,
	        TU_SACH=8,
	
	        NGU_VAN_J=9,
	        NGU_VAN_W=10,
	        NGU_SACH=11,
	
	        LUC_VAN_J=12,
	        LUC_VAN_W=13,
	        LUC_SACH=14,
	
	        THAT_VAN_J=15,
	        THAT_VAN_W=16,
	        THAT_SACH=17,
	
	        BAT_VAN_J=18,
	        BAT_VAN_W=19,
	        BAT_SACH=20,
	
	        CUU_VAN_J=21,
	        CUU_VAN_W=22,
	        CUU_SACH=23,
	
	        CHI_CHI=24
        }

       public enum CARD_RANK
       {
           CHI = 8,
           NHI = 0,
           TAM = 1,
           TU = 2,
           NGU = 3,
           LUC = 4,
           THAT = 5,
           BAT = 6,
           CUU = 7,
       }


       public enum CARD_SUIT
       {
           VAN_J,
           VAN_W,
           SACH,
           CHI,
       }

        public GameUtil()
        {
        }

        public static Boolean IscardRed(int cardId)
        {
            ListCardID[] RedCards = {ListCardID.BAT_VAN_J, ListCardID.BAT_SACH, ListCardID.CUU_VAN_J, ListCardID.CUU_SACH, ListCardID.CHI_CHI};
            for (int i = 0; i < RedCards.Length; i++)
            {
                if (cardId == (int)RedCards[i])
                    return true;
            }
            return false;
        }

        public static CARD_RANK getRank(int cardId)// hang
        {
            if (cardId == (int) ListCardID.CHI_CHI)
                return CARD_RANK.CHI;
            else return (CARD_RANK) (cardId / 3);
        }

        public static CARD_SUIT getSuit(int cardId)// hang
        {
            if (cardId == (int)ListCardID.CHI_CHI)
                return CARD_SUIT.CHI;
            else return (CARD_SUIT)(cardId % 3);
        }
        public void test(int cardId)
        {
            CARD_RANK rank = getRank(cardId);
            CARD_SUIT suit = getSuit(cardId);
        }
    }
}
