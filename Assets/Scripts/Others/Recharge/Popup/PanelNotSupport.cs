using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class PanelNotSupport : MonoBehaviour
{

    #region Unity Editor
    public CUIHandle btClose;
    public UILabel numberPrefix;
    public UITable tableSynTax;
    public GameObject parent;
    #endregion
    private string[] regexSMSTemplate = { @"(\{\{[\w\|]+\}\})", @"\{\{[\w\d]+\}\}", @"\{\{([\w\d]*)\}\}" };
    [HideInInspector]
    private RechargeModel model;
    public RechargeModel Model
    {
        get { return model; }
        set 
        {
            model = value;
            ShowSyntax();
        }
    }
    private int _countDownValue;
    void Awake()
    {
        CUIHandle.AddClick(btClose, parent.GetComponent<RechargePopup>().OnClickClose);
    }

    private void OnClickClose(GameObject targetObject)
    {
        GameObject.Destroy(parent);
    }
    void OnDestroy()
    {
        CUIHandle.AddClick(btClose, parent.GetComponent<RechargePopup>().OnClickClose);
    }
    private void ShowSyntax()
    {
        ShowSMSTemplate();
        btClose.GetComponentInChildren<UILabel>().text = btClose.GetComponentInChildren<UILabel>().text + "(" + _countDownValue + ")";
        StartCountDown(5);
    }
    private void ShowSMSTemplate()
    {
        if (model != null)
        {
            string template = model.Template;
            string[] smsTemplateArr = Regex.Split(template, regexSMSTemplate[0]);

            foreach (string sms in smsTemplateArr)
            {
                if (!string.IsNullOrEmpty(sms))
                {
                    if (Regex.IsMatch(sms, regexSMSTemplate[1]))
                    {

                        string user = Regex.Match(sms, regexSMSTemplate[2]).Groups[1].Value;
                        string username = "";
                        switch (user)
                        {
                            case "username":
                                    username = GameManager.Instance.mInfo.username;
                                break;
                            case "id":
                                    username = GameManager.Instance.mInfo.id + "";
                                break;
                        }
                        SmsSyntax.Create(username, tableSynTax, 100 + " " + username, true);
                    }
                    else
                    {
                        string[] syntaxArr = Regex.Split(sms, " ");
                        for (int i = 0; i < syntaxArr.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(syntaxArr[i]))
                            {
                                SmsSyntax.Create(syntaxArr[i].ToUpper(), tableSynTax, i + " " + syntaxArr[i], false);
                            }
                        }


                    }
                }
            }
            numberPrefix.text = model.Code;
            tableSynTax.repositionNow = true;
            float widthTable = 0f;

            for (int i = 0; i < tableSynTax.transform.childCount; i++)
            {
                NGUITools.AddWidgetCollider(tableSynTax.transform.GetChild(i).gameObject);
                if (i != tableSynTax.transform.childCount - 1)
                    widthTable += tableSynTax.transform.GetChild(i).gameObject.GetComponent<BoxCollider>().size.x + 10f;
                else
                    widthTable += tableSynTax.transform.GetChild(i).gameObject.GetComponent<BoxCollider>().size.x;

                GameObject.Destroy(tableSynTax.transform.GetChild(i).GetComponent<BoxCollider>());

            }
            tableSynTax.transform.localPosition = new Vector3(-widthTable / 2, tableSynTax.transform.localPosition.y, tableSynTax.transform.localPosition.z);
        }
    }
    public void StartCountDown(int countDownValue)
    {
        _countDownValue = countDownValue;
        InvokeRepeating("CountDown", 0f, 1f);
    }
    public void CountDown()
    {
        _countDownValue -= 1;
        if (_countDownValue < 0)
        {
            _countDownValue = 0;
            btClose.gameObject.collider.enabled = btClose.gameObject.collider.enabled = true;
            btClose.GetComponentInChildren<UILabel>().text = "ĐÓNG";
            CancelInvoke("CountDown");
        }
        else
        {
            btClose.gameObject.collider.enabled = false;
            btClose.GetComponentInChildren<UILabel>().text = "ĐÓNG (" + _countDownValue + ")";
        }
    }
}
