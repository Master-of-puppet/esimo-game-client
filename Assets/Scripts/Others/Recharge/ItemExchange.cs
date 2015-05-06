using UnityEngine;
using System.Collections;

public class ItemExchange : MonoBehaviour
{

    #region Unity Editor
    public UILabel numberPrefix, chip;
    #endregion
    [HideInInspector]
    RechargeModel model;
    public static ItemExchange Create(RechargeModel model, Transform parent)
    {
        GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/Recharge/Item Exchange"));
        go.transform.parent = parent;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        ItemExchange item = go.GetComponent<ItemExchange>();
        item.SetData(model);
        return item;
    }
    public void SetData(RechargeModel model)
    {
        this.model = model;
        if (model.Type == "mobile_card")
        {
            numberPrefix.text = "Thẻ cào " + Utility.Convert.Chip(model.CodeValue);
        }
        else
        {
            numberPrefix.text = "Tin nhắn đến " + model.Code;
        }
        chip.text = Utility.Convert.Chip(model.Value) + " chips";
    }
}
