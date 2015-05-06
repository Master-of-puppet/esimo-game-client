using System;
using UnityEngine;
using System.Collections;

public class MoneyExchangeView : MonoBehaviour
{
    public const float TIME_EFFECT = 1f;

    public static void Create(string fromPos, string toPos)
    {
        #region MOVE EFFECT MONEY EXCHANGE
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/ChipEffectPrefab"));
        obj.transform.parent = GameModelChan.game.mPlaymat.locationPlayer[(int)GameModelChan.GetPlayer(fromPos).mSide];
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.parent = GameModelChan.game.mPlaymat.transform.parent.parent;
        obj.AddComponent<MoneyExchangeView>();
        obj.name = "Money Exchange";
        Vector3 posFrom = obj.transform.localPosition;
        Vector3 posTo = Vector3.zero;
        if (toPos == "ga")
        {
            posTo = GameModelChan.game.deck.transform.localPosition;
        }
        else
        {
            posTo = GameModelChan.game.mPlaymat.locationPlayer[(int)GameModelChan.GetPlayer(toPos).mSide].localPosition;
			if (GameModelChan.YourController !=null && toPos == GameModelChan.YourController.username)
                posTo.y = -290f;
        }

        posFrom.z -= 30f;
        posTo.z -= 30f;
        TweenPosition tpos = obj.AddComponent<TweenPosition>();
        tpos.from = posFrom;
        tpos.to = posTo;
        tpos.duration = TIME_EFFECT;

        //TweenScale tScale = obj.AddComponent<TweenScale>();
        //tScale.from = new Vector3(30f, 30f, 1f);
        //tScale.to = new Vector3(58, 36f, 1f);
        //tScale.duration = TIME_EFFECT / 3f;
        #endregion
    }
    public static void Create(int indexFrom, int indexTo, int money)
    {
        #region MOVE EFFECT MONEY EXCHANGE
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/ChipEffectPrefab"));
        obj.AddComponent<MoneyExchangeView>();

        obj.name = "Money Exchange";
        obj.transform.parent = GameModelPhom.game.mPlaymat.locationPlayer[0].parent;
        obj.transform.localScale = Vector3.one;
        //obj.transform.localScale = new Vector3(40f, 40f, 40f);

        Vector3 posFrom = GameModelPhom.game.mPlaymat.locationPlayer[(int)GameModelPhom.GetPlayer(indexFrom).mSide].localPosition;
        posFrom.z -= 30f;
        Vector3 posTo = GameModelPhom.game.mPlaymat.locationPlayer[(int)GameModelPhom.GetPlayer(indexTo).mSide].localPosition;
        posTo.z -= 30f;
        if (indexTo == GameModelPhom.YourController.slotServer)
            posTo.y = -290f;


        TweenPosition tpos = obj.AddComponent<TweenPosition>();
        tpos.from = posFrom;
        tpos.to = posTo;
        tpos.duration = TIME_EFFECT;

        //TweenScale tScale = obj.AddComponent<TweenScale>();
        //tScale.from = new Vector3(30f, 30f, 1f);
        //tScale.to = new Vector3(58, 36f, 1f);
        //tScale.duration = TIME_EFFECT / 3f;
        #endregion
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(TIME_EFFECT + 0.25f);
        GameObject.Destroy(gameObject);
    }
}
