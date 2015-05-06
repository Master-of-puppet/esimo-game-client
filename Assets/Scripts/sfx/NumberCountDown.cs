using System;
using System.Collections.Generic;
using UnityEngine;

public class NumberCountDown : MonoBehaviour
{
    private string[] spriteName = new string[] { "nb0", "nb1", "nb2", "nb3", "nb4", "nb5", "nb6", "nb7", "nb8", "nb9" };

    public UISprite tensOf;
    public UISprite unitOf;
    public UISprite tramOf;

    [HideInInspector]
    public float countDownValue;
    public UILabel lblNtime;
    public static NumberCountDown Create(float _value, Transform parent)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/sfx/NumberCountDownPrefab"));
        obj.name = "Count Down - Numbers";
        obj.transform.parent = parent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<NumberCountDown>().StartCountDown(_value);
        return obj.GetComponent<NumberCountDown>();
    }

    public void StartCountDown(float _value)
    {
        countDownValue = _value;
        InvokeRepeating("CountDown", 0f, 1f);
        isRunning = true;
    }
    void OnDestroy() {
        isRunning = false;
    }
    private bool isRunning = false;

    public bool IsRunning
    {
        get { return isRunning; }
    }
    void CountDown()
    {
        int value = UnityEngine.Mathf.FloorToInt(countDownValue);
        countDownValue -= 1f;
        if (countDownValue < 0)
            countDownValue = 0;

        if (countDownValue < 99 && value > 0)
        {
            tensOf.spriteName = value > 9 ? spriteName[value / 10] : spriteName[0];
            unitOf.spriteName = value > 9 ? spriteName[value % 10] : spriteName[value];
        }
        else
        {
            Debug.Log("Current time:" + value);
            tensOf.spriteName = spriteName[9];
            unitOf.spriteName = spriteName[9];
        }
    }
}
