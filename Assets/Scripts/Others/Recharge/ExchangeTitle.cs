using UnityEngine;
using System.Collections;

public class ExchangeTitle : MonoBehaviour
{
    #region Unity Editor
    public UILabel title;
    #endregion
    public static ExchangeTitle Create(string text, Transform parent)
    {
        GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/Recharge/ExchangeTitle"));
        go.transform.parent = parent;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.GetComponent<ExchangeTitle>().SetData(text);
        return go.GetComponent<ExchangeTitle>();

    }
    public void SetData(string title)
    {
        switch (title)
        {
            case "sms" :
                this.title.text = "SMS";
                break;
            case "mobile_card":
                this.title.text = "Thẻ cào";
                break;
        }

    }
}

