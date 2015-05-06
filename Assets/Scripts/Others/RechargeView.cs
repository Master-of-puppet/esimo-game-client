using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;




public class RechargeView : MonoBehaviour
{
    // Use this for initialization
    public CUIHandle btBack, btExchange;
    public UITable tableSMS;
	public UIGrid tableCard;
    private string[] regexSMSTemplate = { @"(\{\{[\w\|]+\}\})", @"\{\{[\w\d]+\}\}", @"\{\{([\w\d]*)\}\}" };
    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickBack;
        CUIHandle.AddClick(btBack, OnClickBack);
        CUIHandle.AddClick(btExchange, OnClickExchange);
    }
     
    private void OnClickExchange(GameObject targetObject)
    {
        RechargePopup.ShowExchange();
    }
    void OnDestroy()
    {
        HeaderMenu.Instance.ReDraw();
        CUIHandle.RemoveClick(btBack, OnClickBack);
        CUIHandle.RemoveClick(btExchange, OnClickExchange);
    }
    void Start()
    {
        CreateListCardRecharge();
        CreateListSMSRecharge();
    }  
    void CreateListSMSRecharge()
    {
        List<RechargeModel> smsRecharge = GameManager.Instance.ListRechargeModel.FindAll(m => m.Type == "sms");
        foreach (RechargeModel model in smsRecharge)
        {
            SMSRechargeView.Create(model, tableSMS.gameObject.transform);
        }
        tableSMS.Reposition();

        StartCoroutine(Reposition());
    }

    void CreateListCardRecharge()
    {
        List<List<RechargeModel>> listGroupProvider = groupCardSameProvider();
        foreach(List<RechargeModel> models in listGroupProvider)
        {
            CardRechargeView.Create(models, tableCard.transform);
        }
        tableCard.Reposition();
	
    }
    private List<List<RechargeModel>> groupCardSameProvider()
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
                List<RechargeModel> item = listGroupProvider.Find(lst => lst.Find(m => m.Provider == model.Provider) != null);
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
    void OnClickBack(GameObject go)
    {
        //GameObject.Destroy(GameObject.Find("__RechargePrefab"));
        GameObject.Destroy(gameObject);
    }

    IEnumerator Reposition()
    {
        yield return new WaitForEndOfFrame();
        tableCard.Reposition();
        tableSMS.Reposition();
    }

    private void ShowSMSTemplate(string template)
    {
        //string username = "";
        
        //tableSMSTemplate.repositionNow = true;
        //float widthTable = 0f;

        //for (int i = 0; i < tableSMSTemplate.transform.childCount; i++)
        //{

        //    if (i != tableSMSTemplate.transform.childCount - 1)
        //        widthTable += NGUITools.AddWidgetCollider(tableSMSTemplate.transform.GetChild(i).gameObject).size.x + 10f;
        //    else
        //        widthTable += NGUITools.AddWidgetCollider(tableSMSTemplate.transform.GetChild(i).gameObject).size.x;

        //    GameObject.Destroy(tableSMSTemplate.transform.GetChild(i).GetComponent<BoxCollider>());

        //}
        //tableSMSTemplate.transform.localPosition = new Vector3(-widthTable / 2, tableSMSTemplate.transform.localPosition.y, tableSMSTemplate.transform.localPosition.z);

    }
    

    public static RechargeView Create()
    {
        GameObject recharge = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/RechargePrefab"));
        recharge.name = "__RechargePrefab";
        recharge.transform.position = new Vector3(-1301, 240, -128f);
        //Transform obj = HeaderMenu.Instance.transform.FindChild("Camera");
        //obj.camera.depth = 5;
        return recharge.GetComponent<RechargeView>();

    }

}

