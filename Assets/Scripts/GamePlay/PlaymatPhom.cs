using UnityEngine;
using System.Collections;

public class PlaymatPhom : MonoBehaviour
{
    #region Unity Editor
    public Transform[] locationPlayer;
    public Transform[] locationTrash;
    public Transform[] locationHand;
    public Transform[] locationMelds;
    #endregion

    public Vector3 GetLocationHand(PlayerControllerPhom p, int i)
    {
        float CARD_SPACE = 20f;
        Vector3 first = locationHand[(int)p.mSide].localPosition;

        int totalCard =
            (p.mSide == ESide.You || p.mSide == ESide.Enemy_2) ?
            p.mCardHand.FindAll(card => card.gameObject != null).Count :
            p.mCardHand.FindAll(card => card.gameObject != null && card.originSide == card.currentSide).Count;

        float z = -1f + ((i - 1) * -1);

        if (p.mSide == ESide.You)
        {
            if (totalCard == 10)
                CARD_SPACE = 82f;
            else
                CARD_SPACE = 90f;

            if (totalCard < 9)
            {
                first.x = CARD_SPACE / 2 - (CARD_SPACE * totalCard / 2);
                //z -= 9 - totalCard;
            }

            return new Vector3(first.x + (CARD_SPACE * i), first.y, z);
        }
        else if (p.mSide == ESide.Enemy_1 || p.mSide == ESide.Enemy_3)
        {
            if (GameModelPhom.CurrentState == GameModelPhom.EGameState.finalizing)
                CARD_SPACE *= 1.5f;

            float firstY = first.y + (CARD_SPACE * (totalCard / 2));
            if (totalCard / 2 == 0) firstY -= CARD_SPACE / 2;

            return new Vector3(first.x, firstY - (CARD_SPACE * i), z);
        }
        else
        {
            float firstX = first.x - (CARD_SPACE * (totalCard / 2));
            if (totalCard / 2 == 0) firstX += CARD_SPACE / 2;

            return new Vector3(firstX + (CARD_SPACE * i), first.y, z);
        }
    }

    public Vector3 GetLocationTrash(PlayerControllerPhom p, int i)
    {
        float CARD_SPACE = 15f;
        float z = -1f + ((i - 1) * -1);

        Vector3 first = locationTrash[(int)p.mSide].localPosition;
        int totalCard = p.mCardTrash.FindAll(card => card.gameObject != null).Count;

        float firstX = first.x;

        if (totalCard == 2)
            firstX = first.x - CARD_SPACE / 2;
        else if (totalCard == 3)
            firstX = first.x - CARD_SPACE;
        else if (totalCard == 4)
            firstX = first.x - CARD_SPACE - CARD_SPACE / 2;

        return new Vector3(firstX + (CARD_SPACE * i), first.y, z);
    }

    public Vector3 GetRotateHand(PlayerControllerPhom p)
    {
        if (p.mSide == ESide.Enemy_1 || p.mSide == ESide.Enemy_3)
            return new Vector3(0f, 0f, 90f);
        else
            return Vector3.zero;
    }

    public Vector3 GetLocationMeld(PlayerControllerPhom p, int indexMeld, int i)
    {
        float CARD_SPACE = 15f;

        float SPACE_MELD_HEIGHT = ECardTexture.CARD_HEIGHT * 2 / 3;

        Vector3 position = locationMelds[(int)p.mSide].localPosition;
        Vector3 meld_1 = position, meld_2 = position, meld_3 = position;

        if (p.mCardMelds.Count == 1 && p.mSide == ESide.You)
        {
            #region 1 MELD
            meld_1.x -= p.mCardMelds[0].meld.Count / 2f + CARD_SPACE / 2f;
            #endregion
        }
        else if (p.mCardMelds.Count == 2)
        {
            #region 2 MELDS
            if (p.mSide == ESide.Enemy_1 || p.mSide == ESide.Enemy_3)
            {
                meld_1 = new Vector3(position.x, position.y + SPACE_MELD_HEIGHT, position.z);
                meld_2 = new Vector3(position.x, position.y - SPACE_MELD_HEIGHT, position.z);
            }
            else if (p.mSide == ESide.Enemy_2)
            {
                meld_1 = new Vector3(position.x - CARD_SPACE * 4, position.y, position.z);
                meld_2 = new Vector3(position.x + CARD_SPACE * 4, position.y, position.z);
            }
            else
            {
                int totalCardAllMeld = p.mCardMelds[0].meld.Count + p.mCardMelds[1].meld.Count;
                meld_1.x -= ((totalCardAllMeld - 1) / 2f * CARD_SPACE) + (SPACE_MELD_HEIGHT / 2f);
                meld_2.x = meld_1.x + SPACE_MELD_HEIGHT + p.mCardMelds[0].meld.Count * CARD_SPACE;
            }
            #endregion
        }
        else if (p.mCardMelds.Count == 3)
        {
            #region 3 MELDS
            if (p.mSide == ESide.Enemy_1)
            {
                meld_1.x -= p.mCardMelds[0].meld.Count == 3 ? (ECardTexture.CARD_WITH / 6) : (ECardTexture.CARD_WITH / 6 + CARD_SPACE);
                meld_2 = new Vector3(position.x + ECardTexture.CARD_WITH * 1.5f, position.y + SPACE_MELD_HEIGHT, position.z);
                meld_3 = new Vector3(position.x + ECardTexture.CARD_WITH * 1.5f, position.y - SPACE_MELD_HEIGHT, position.z);
            }
            else if (p.mSide == ESide.Enemy_3)
            {
                meld_1.x += ECardTexture.CARD_WITH / 6;
                meld_2 = new Vector3(position.x - ECardTexture.CARD_WITH * 1.5f, position.y + SPACE_MELD_HEIGHT, position.z);
                meld_3 = new Vector3(position.x - ECardTexture.CARD_WITH * 1.5f, position.y - SPACE_MELD_HEIGHT, position.z);
            }
            else if (p.mSide == ESide.Enemy_2)
            {
                meld_2 = new Vector3(position.x - CARD_SPACE * 4, position.y + ECardTexture.CARD_HEIGHT, position.z);
                meld_3 = new Vector3(position.x + CARD_SPACE * 4, position.y + ECardTexture.CARD_HEIGHT, position.z);
            }
            else
            {
                meld_2 = new Vector3(position.x - CARD_SPACE * 4, position.y - ECardTexture.CARD_HEIGHT, position.z);
                meld_3 = new Vector3(position.x + CARD_SPACE * 4, position.y - ECardTexture.CARD_HEIGHT, position.z);
            }
            #endregion
        }

        Vector3 first;
        if (indexMeld == 1)
            first = meld_2;
        else if (indexMeld == 2)
            first = meld_3;
        else
            first = meld_1;

        float z = -2f + ((i - 1) * -1);

        #region NUMBER CARD IN LINE
        int totalCard = p.mCardMelds[indexMeld].meld.Count;
        int numberCardInLine = 5;

        if (p.mSide == ESide.You)
            numberCardInLine = totalCard;
        else
        {
            if (totalCard == 6)
                numberCardInLine = 3;
            else if (totalCard == 7 || totalCard == 8 || totalCard == 11 || totalCard == 12)
                numberCardInLine = 4;
        }

        if (i > numberCardInLine - 1)
        {
            i -= numberCardInLine;
            first.y -= ECardTexture.CARD_HEIGHT * 2 / 5;
        }
        #endregion

        return new Vector3(first.x + (CARD_SPACE * i), first.y, z);
    }

    public Vector3 GetLocationFulllaying(PlayerControllerPhom p, int indexMeld, int i)
    {
        float CARD_SPACE = 35f;

        float SPACE_MELD_HEIGHT = ECardTexture.CARD_HEIGHT * 4 / 5;

        Vector3 position = locationMelds[(int)p.mSide].localPosition;
        Vector3 meld_1 = position, meld_2 = position, meld_3 = position;

        if (p.mCardMelds.Count == 1)
        {
            meld_1.x -= p.mCardMelds[0].meld.Count / 2f + CARD_SPACE / 2f;
        }
        else if (p.mCardMelds.Count == 2)
        {
            int totalCardAllMeld = p.mCardMelds[0].meld.Count + p.mCardMelds[1].meld.Count;
            meld_1.x -= ((totalCardAllMeld - 1) / 2f * CARD_SPACE) + (SPACE_MELD_HEIGHT / 2f);
            meld_2.x = meld_1.x + SPACE_MELD_HEIGHT + p.mCardMelds[0].meld.Count * CARD_SPACE;
        }
        else if (p.mCardMelds.Count == 3)
        {
            int totalCardAllMeld = p.mCardMelds[0].meld.Count + p.mCardMelds[1].meld.Count + p.mCardMelds[2].meld.Count;
            meld_1.x -= ((totalCardAllMeld) / 2f * CARD_SPACE) + (SPACE_MELD_HEIGHT / 2f);
            meld_2.x = meld_1.x + SPACE_MELD_HEIGHT + p.mCardMelds[0].meld.Count * CARD_SPACE;
            meld_3.x = meld_2.x + SPACE_MELD_HEIGHT + p.mCardMelds[1].meld.Count * CARD_SPACE;
        }

        Vector3 first;
        if (indexMeld == 1)
            first = meld_2;
        else if (indexMeld == 2)
            first = meld_3;
        else
            first = meld_1;

        float z = -15f + ((i - 1) * -1);
        first.y = -45f;

        return new Vector3(first.x + (CARD_SPACE * i), first.y, z);
    }
}