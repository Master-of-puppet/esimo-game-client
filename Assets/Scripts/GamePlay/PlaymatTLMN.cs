using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaymatTLMN : MonoBehaviour
{
    #region Unity Editor
    public Transform[] locationPlayer;
    public Transform[] locationTrash;
    public Transform[] locationHand;
    public Transform[] locationMelds;
    #endregion
    public Vector3 GetLocationEffect(int i, List<TLMNCard> listEffect)
    {
        float CARD_SPACE = 18f;
        Vector3 first = locationHand[(int)ESide.You].localPosition;
        first.y = 0;

        int totalCard = listEffect.FindAll(card => card.gameObject != null).Count;

        float z = -15f + ((i - 1) * -1);

        if (totalCard >= 12)
            CARD_SPACE = 35f;
        else if (totalCard >= 10)
            CARD_SPACE = 37f;
        else if (totalCard >= 8)
            CARD_SPACE = 42f;
        else
            CARD_SPACE = 50f + 50f / totalCard;

        CARD_SPACE = UnityEngine.Mathf.Clamp(CARD_SPACE, 30f, 60f);

        first.x = CARD_SPACE / 2 - (CARD_SPACE * totalCard / 2);

        return new Vector3(first.x + (CARD_SPACE * i), first.y, z);
    }
    public Vector3 GetLocationHand(PlayerControllerTLMN p, int i)
    {
        float CARD_SPACE = 18f;
        Vector3 first = locationHand[(int)p.mSide].localPosition;

        int totalCard = (p.mSide == ESide.You || p.mSide == ESide.Enemy_2) ?
            p.mCardHand.FindAll(card => card.gameObject != null).Count :
            p.mCardHand.FindAll(card => card.gameObject != null && card.originSide == card.currentSide).Count;

        float z = -1f + ((i - 1) * -1);

        if (p.mSide == ESide.You)
        {
            if (totalCard == 13)
                CARD_SPACE = 60f;
            else if (totalCard < 13)
                CARD_SPACE = 60f + 60f / totalCard;

            CARD_SPACE = UnityEngine.Mathf.Clamp(CARD_SPACE, 60f, 95f);

            first.x = CARD_SPACE / 2 - (CARD_SPACE * totalCard / 2);

            return new Vector3(first.x + (CARD_SPACE * i), first.y, z);
        }
        else if (p.mSide == ESide.Enemy_1 || p.mSide == ESide.Enemy_3)
        {
            if (GameModelTLMN.CurrentState == GameModelTLMN.EGameState.finalizing)
                CARD_SPACE *= 1.5f;

            float firstY = first.y + (CARD_SPACE * (totalCard / 2));

            if (totalCard > 10)
                firstY += CARD_SPACE * 2;

            if (totalCard / 2 == 0) firstY -= CARD_SPACE / 2;

            return new Vector3(first.x, firstY - (CARD_SPACE * i), z);
        }
        else
        {
            float firstX = first.x - ((CARD_SPACE * totalCard) / 2) + CARD_SPACE;
            if (totalCard < 13)
                firstX -= ((13 - totalCard) * CARD_SPACE / 2.0f);
            return new Vector3(firstX + (CARD_SPACE * i), first.y, z);
        }
    }

    public Vector3 GetLocationTrashFaceUp(int indexOfTurn, int indexOfList, int indexInTotal)
    {
        float CARD_SPACE = 15f;
        float CARD_WIDTH = ECardTexture.CARD_WITH * 1 / 3;
        float CARD_HEIGHT = ECardTexture.CARD_HEIGHT / 2f;

        //int totalCardFaceUp = indexInTotal = indexOfTurn > 0 ? (GameModel.model.listDiscard[(int)GameModel.EDiscard.FaceUp][indexOfTurn].Count) * (indexOfTurn) + indexOfList : indexOfList;
        float z = -1f + (-1 * indexInTotal);
        Vector3 first = new Vector3(0f, ECardTexture.CARD_HEIGHT / 4f, 0f);

        int size = GameModelTLMN.model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp].Count;
        int index = indexOfTurn < 4 ? indexOfTurn : indexOfTurn % 4;
        int sizeBound = size < 4 ? size : size % 4;

        //first.x -= GameModel.model.listDiscard[(int)GameModel.EDiscard.FaceUp][indexOfTurn].Count * CARD_VERTICAL / 2f;
        first.x -= size * CARD_WIDTH / 2f;

        if (index > 0)
            first.y += index * CARD_HEIGHT / 2f;

        return new Vector3(
            first.x + (CARD_SPACE * indexOfList) + (CARD_WIDTH * indexOfTurn),
            first.y - (CARD_HEIGHT * index),
            z);
    }

    public Vector3 GetLocationTrash(PlayerControllerTLMN p, int i)
    {
        float CARD_SPACE = 15f;
        float z = -13f + ((i - 1) * -1);

        Vector3 first = locationTrash[4].localPosition;
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

    public Vector3 GetRotateHand(PlayerControllerTLMN p)
    {
        if (p.mSide == ESide.Enemy_1 || p.mSide == ESide.Enemy_3)
            return new Vector3(0f, 0f, 90f);
        else
            return Vector3.zero;
    }

    public Vector3 GetLocationMeld(PlayerControllerTLMN p, int indexMeld, int i)
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

    public Vector3 GetLocationEffect(int i, List<ECard> listEffect)
    {
        float CARD_SPACE = 18f;
        Vector3 first = locationHand[(int)ESide.You].localPosition;
        first.y = 0;

        int totalCard = listEffect.FindAll(card => card.gameObject != null).Count;

        float z = -15f + ((i - 1) * -1);

        if (totalCard >= 12)
            CARD_SPACE = 35f;
        else if (totalCard >= 10)
            CARD_SPACE = 37f;
        else if (totalCard >= 8)
            CARD_SPACE = 42f;
        else
            CARD_SPACE = 50f + 50f / totalCard;

        CARD_SPACE = UnityEngine.Mathf.Clamp(CARD_SPACE, 30f, 60f);

        first.x = CARD_SPACE / 2 - (CARD_SPACE * totalCard / 2);

        return new Vector3(first.x + (CARD_SPACE * i), first.y, z);
    }
}
