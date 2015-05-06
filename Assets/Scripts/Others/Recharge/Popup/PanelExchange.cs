using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class PanelExchange : MonoBehaviour
{

    #region Unity Editor
    public UIGrid exchangeGrid;
    public CUIHandle btnClose;
    public GameObject parent;
    #endregion
    void Awake() 
    {
        CUIHandle.AddClick(btnClose, parent.GetComponent<RechargePopup>().OnClickClose);
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnClose, parent.GetComponent<RechargePopup>().OnClickClose);
    }
    void Start()
    {
        ShowExchange();
    }
    public void ShowExchange()
    {   
        List<RechargeModel> listType = new List<RechargeModel>();
        foreach (RechargeModel model in GameManager.Instance.ListRechargeModel)
        {
            if (listType.Count == 0)
                listType.Add(model);
            else
            {
                if (listType.Find(m => m.Type == model.Type) == null)
                    listType.Add(model);
            }
        }

       
        ExchangeTitle.Create("sms",exchangeGrid.transform).transform.name = "1";
        List<RechargeModel> smsRecharge = GameManager.Instance.ListRechargeModel.FindAll(m => m.Type == "sms");
        for(int j = 0; j < smsRecharge.Count;j++)
        {
            ItemExchange.Create(smsRecharge[j], exchangeGrid.transform).transform.name ="1." + j;
        }
   
		ExchangeTitle.Create("mobile_card", exchangeGrid.transform).transform.name = "2";
        List<List<RechargeModel>> lstGroup = GroupCardCodeValue();
        for (int j = 0; j < lstGroup.Count; j++) 
        {
            ItemExchange.Create(lstGroup[j][0], exchangeGrid.transform).transform.name =  "2." + j;
        }
 
     
        exchangeGrid.repositionNow = true;
		exchangeGrid.transform.parent.GetComponent<UIScrollView> ().ResetPosition ();
    }
    public List<List<RechargeModel>> GroupCardCodeValue()
    {
        List<RechargeModel> cardRecharge = GameManager.Instance.ListRechargeModel.FindAll(m => m.Type == "mobile_card");
        List<List<RechargeModel>> listGroupProvider = new List<List<RechargeModel>>();
        foreach (RechargeModel model in cardRecharge)
        {
            if (listGroupProvider.Count == 0)
            {
                List<RechargeModel> models = new List<RechargeModel>();
                models.Add(model);
                listGroupProvider.Add(models);
            }
            else
            {
                List<RechargeModel> item = listGroupProvider.Find(lst => lst.Find(m => m.CodeValue == model.CodeValue) != null);
                if (item != null)
                    item.Add(model);
                else
                {
                    List<RechargeModel> models = new List<RechargeModel>();
                    models.Add(model);
                    listGroupProvider.Add(models);
                }
            }
        }
        return listGroupProvider;
    }
    private void OnClickBtnClose(GameObject targetObject)
    {
        GameObject.Destroy(parent);
    }

}
