using UnityEngine;
using System.Collections;

public class RechargePopup : MonoBehaviour
{
    public enum TypeRechargePopup
    {
        Card = 1 ,
        Exchange = 2 ,
        NotSupport = 3,
    }
    #region Unity Editor
    public PanelRechargeCard panelRechargeCard;
    public PanelExchange panelExchange;
    public PanelNotSupport panelNotSupport;
    #endregion
    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickClose;
    }

    public void OnClickClose(GameObject targetObject)
    {
        GameObject.Destroy(gameObject);
    }
    public static RechargePopup Instance
    {
        get
        {
            GameObject go = GameObject.Find("__RechargePopup");
            if (go == null)
            {
                go = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/Recharge/RechargePopup"));
                go.name = "__RechargePopup";
                go.transform.localPosition = new Vector3(100f, 100f, 130f);
            }
            return go.GetComponent<RechargePopup>();
        }
    }
    public void Init(TypeRechargePopup _type)
    {
        switch (_type) 
        {
            case TypeRechargePopup.Card :
                panelExchange.gameObject.SetActive(false);
                panelRechargeCard.gameObject.SetActive(true);
                panelNotSupport.gameObject.SetActive(false);
                break;
            case TypeRechargePopup.Exchange :
                panelExchange.gameObject.SetActive(true);
                panelRechargeCard.gameObject.SetActive(false);
                panelNotSupport.gameObject.SetActive(false);
                break;
            case TypeRechargePopup.NotSupport:
                panelRechargeCard.gameObject.SetActive(false);
                panelExchange.gameObject.SetActive(false);
                panelNotSupport.gameObject.SetActive(true);
                break;
        }
    }
    public static void ShowExchange() 
    {
        Instance.Init(TypeRechargePopup.Exchange);
    }
    public static void ShowRechargeCard(RechargeModel model) 
    {
        Instance.Init(TypeRechargePopup.Card);
        Instance.panelRechargeCard.Model = model;
    }
    public static void ShowNotSupport(RechargeModel model) 
    {
        Instance.Init(TypeRechargePopup.NotSupport);
        Instance.panelNotSupport.Model = model;
    }
}
