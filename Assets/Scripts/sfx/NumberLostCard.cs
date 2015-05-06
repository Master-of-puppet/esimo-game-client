using System;
using System.Collections.Generic;
using UnityEngine;

public class NumberLostCard : MonoBehaviour
{
    private string[] spriteName = new string[] { "nb0", "nb1", "nb2", "nb3", "nb4", "nb5", "nb6", "nb7", "nb8", "nb9" };

    #region Unity Editor
    public UISprite number1;
    public UISprite number2;
    public UISprite number3;
    #endregion

    public static NumberLostCard Create(int value, Transform parent)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/sfx/NumberLostCard"));
        obj.name = "Numbers - Lost Card";
        obj.transform.parent = parent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<NumberLostCard>().Init(value);
        return obj.GetComponent<NumberLostCard>();
    }

    void Init(int value)
    {
        if (value < 10)
        {
            number1.transform.localPosition = new Vector3(0f, -15f, -1f);
            number1.spriteName = spriteName[value];

            number2.gameObject.SetActive(false);
            number3.gameObject.SetActive(false);
        }
        else if (value < 100)
        {
            number1.transform.localPosition = new Vector3(-18f, -15f, -1f);
            number2.transform.localPosition = new Vector3(17f, -15f, -1f);
            number1.spriteName = spriteName[value / 10];
            number2.spriteName = spriteName[value % 10];

            number3.gameObject.SetActive(false);
        }
        else
        {
            if (value > 999)
                value = 999;

            number1.transform.localPosition = new Vector3(-35f, -15f, -1f);
            number2.transform.localPosition = new Vector3(0f, -15f, -1f);
            number3.transform.localPosition = new Vector3(35f, -15f, -1f);

            number1.spriteName = spriteName[value / 100];
            number2.spriteName = spriteName[(value / 10) % 10];
            number3.spriteName = spriteName[(value % 100) % 10];
        }
    }
}
