using UnityEngine;
using System.Collections;

public class itemConvertMoney : MonoBehaviour {

	// Use this for initialization
    public UILabel lblvnd, lblchip;

	void Start () {
	
	}
    public static itemConvertMoney Create(RechargeModel model, Transform parent)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/itemRecharPrefab"));
        obj.name = model.Code;
        obj.transform.parent = parent;
        obj.transform.localPosition = new Vector3(0f, 0f, -1f);
        obj.transform.localScale = Vector3.one;
        itemConvertMoney convert = obj.GetComponent<itemConvertMoney>();
        convert.SetData(model);
        return convert;
    }
    public void SetData(RechargeModel item)
    {
        lblvnd.text = "Thẻ " + Utility.Convert.Chip(item.CodeValue) + " VND";
        lblchip.text = Utility.Convert.Chip(item.Value) + " Chip";
    }
}
