using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playmat : MonoBehaviour
{
    #region Unity Editor
    public Transform[] locationPlayer;
    public Transform[] locationTrash;
    public Transform[] locationStealCards;
    public Transform[] locationDealCard;
    public Transform[] locationRotation;
    public Transform parentCardTotation;

    public Transform locationHand;
    public Transform locationFullLaying;        
	
    public Transform locationViewNoc;
    #endregion

    /// <summary>
    /// Khoang cach giua 2 card tren ban
    /// </summary>
    const float CARD_STEAL_SPACE = 27f;

    public Vector3 GetPointCircle(Vector3 center, float radius, float angle)
    {
        angle = Mathf.Deg2Rad * angle;
        float x = center.x + Mathf.Cos(angle) * radius;
        float y = center.y - Mathf.Sin(angle) * radius;
        return new Vector3(x, y);
    }
	
    const float radius = 120f;
    const float radiusOnTouch = 150f;
    const float maxAngleBetween = 11.3f, minRange = 11.3f, maxRange = 200;

    public Vector3 GetLocationHand(PlayerControllerChan p, int i, bool isGetAngle, bool isTouch)
    {
        int numberCard = p.mCardHand.FindAll(c => c.gameObject != null).Count;
        float angleBetween = numberCard * maxAngleBetween;
        float range = Mathf.Clamp(angleBetween, minRange, maxRange);
        float angle = range / numberCard;
        float startAngle = 170f;
        float fixRotate = 100f;

        if (numberCard <= 17)
        {
            startAngle += (maxRange - range) / 2f;
            fixRotate -= (maxRange - range) / 2f;
        }

        if (isGetAngle)
            return new Vector3(0, 0, (fixRotate + angle) - angle * (i + 1 + 0.5f));

        Vector3 offset = GetPointCircle(locationHand.localPosition, isTouch ? radiusOnTouch : radius, angle * (i + 0.5f) + startAngle);
        offset.z = -5 + (-1) * i;

        //if (isTouch)
        //    offset.z = -20f;

        return offset;
    }

    public Vector3 GetLocationSteals(PlayerControllerChan p, int indexSteal, int index)
    {
        float CARD_STEAL_HEIGHT = -30f;

        if (p.mCardSteal[indexSteal].steals.Count == 4)
            CARD_STEAL_HEIGHT /= 3;

        float x = indexSteal * CARD_STEAL_SPACE;

        //if (index > 0)
        //    x += 1f;

        if (p.mSide == ESide.Slot_1 || (p.mSide == ESide.Slot_0 && GameModelChan.YourController == null))
            return new Vector3(-x, index * CARD_STEAL_HEIGHT, -index);
        
        return new Vector3(x, index * CARD_STEAL_HEIGHT, -index);
    }

    public Vector3 GetLocationTrash(PlayerControllerChan p, int i)
    {
        float space = CARD_STEAL_SPACE;
        int maxTrash = 12;
        if (p.mSide == ESide.Slot_0)
            maxTrash = 9;

        if (p.mCardTrash.Count > maxTrash)
            space = CARD_STEAL_SPACE * maxTrash / p.mCardTrash.Count;

        if (p.mSide == ESide.Slot_3 || p.mSide == ESide.Slot_0)
            return new Vector3(i * space, 0f, -i);
        else
            return new Vector3(-i * space, 0f, -i);
    }

    public Vector3 GetLocationFullLaying(int index, int i)
    {
        const float spaceY = -45f;
        const float spaceX = 45f;

        float startX = 0f;
        if (index >= GameModelChan.game.listFullLaying[0].Length / 2)
            startX += 30f;
         if (GameModelChan.game.listFullLaying[1].Length > 0 &&  index >= GameModelChan.game.listFullLaying[0].Length / 2 + GameModelChan.game.listFullLaying[1].Length / 2)
            startX += 30f;

        return new Vector3(index * spaceX + startX, i * spaceY, -i);
    }

    public Vector3 GetLocationFullLayingChiu(int index, int i)
    {
         float spaceY = -30f;
        if(GameModelChan.game.listFullLaying[2].Length==4)
            spaceY /= 3;
        // hard code temp
        if (i == 0)
            i = 6;
        else if (i == 1)
            i = 4;
        else if (i == 2)
            i = 2;
        else
            i = 0;
        return new Vector3(440, i * spaceY, -1);
    }

    public Vector3 GetLocationViewNoc(int i)
    {
        int total = GameModelChan.ListGameObjectNoc.Count;
        float startX = 0 - (total * CARD_STEAL_SPACE /2);
        return new Vector3(startX + CARD_STEAL_SPACE * i, 0f, -i + (-5));
    }
}
