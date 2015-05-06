using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class SMSRechargeView : MonoBehaviour
{
    #region Unity Editor
    public UILabel chip , title;
    public UISprite backgroundSprite;
    public UITexture backgroundTexture;
    #endregion
    [HideInInspector]
    public RechargeModel model;
    private string[] regexSMSTemplate = { @"(\{\{[\w\|]+\}\})", @"\{\{[\w\d]+\}\}", @"\{\{([\w\d]*)\}\}" };

    void Awake()
    {
        CUIHandle.AddClick(gameObject.GetComponent<CUIHandle>(), OnClickSMSRecharge);
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(gameObject.GetComponent<CUIHandle>(), OnClickSMSRecharge);
    }
    private void OnClickSMSRecharge(GameObject targetObject)
    {
        ShowSMSComposer();
    }
    private void ShowSMSComposer()
    {
        List<string> listCode = new List<string>();
        listCode.Add(model.Code);
        NativeManager nativeCode = NativeManager.Instance;
        bool isSMSAvaiable = nativeCode.PromptSendMessage (
            delegate() {
						GameObject.Destroy (nativeCode.gameObject);
				}, GetSynTaxFromTemplate (model.Template), listCode.ToArray ()
				);
		if(!isSMSAvaiable)
			RechargePopup.ShowNotSupport(model);
    }

    public static SMSRechargeView Create(RechargeModel model, Transform parent)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/Recharge/SMSRechargeView"));
        obj.name = "sms " + model.Value;
        obj.transform.parent = parent;
        obj.transform.localPosition = new Vector3(0f, 0f, -1f);
        obj.transform.localScale = Vector3.one;
        SMSRechargeView sms = obj.GetComponent<SMSRechargeView>();
        sms.SetData(model);
        return sms;
    }
    public void SetData(RechargeModel model)
    {
        this.model = model;
        this.chip.text = Utility.Convert.Chip(model.Value) + " chips";
        new AvatarCacheOrDownload(model.ImagesUrl, delegate(Texture _avatarTexture)
        {
            if (_avatarTexture != null)
            {
                _avatarTexture.filterMode = FilterMode.Point;
                _avatarTexture.anisoLevel = 0;
                _avatarTexture.wrapMode = TextureWrapMode.Clamp;
                backgroundTexture.mainTexture = _avatarTexture;
                backgroundTexture.MakePixelPerfect();
            }
            else
            {
                title.gameObject.SetActive(true);
                chip.color = Color.black;
            }
        }, true);
      
    }
    private string GetSynTaxFromTemplate(string template)
    {
        string username = "";
        string syntax = "";
        string[] smsTemplateArr = Regex.Split(template, regexSMSTemplate[0]);
        foreach (string sms in smsTemplateArr)
        {
            if (!string.IsNullOrEmpty(sms))
            {
                if (Regex.IsMatch(sms, regexSMSTemplate[1]))
                {

                    string user = Regex.Match(sms, regexSMSTemplate[2]).Groups[1].Value;
                    switch (user)
                    {
                        case "username":
                            username = GameManager.Instance.mInfo.username;
                            break;
                        case "id":
                            username = GameManager.Instance.mInfo.id + "";
                            break;
                    }
                }
                else
                {
                    syntax = sms;
                }
            }
        }
        return syntax + username;
    }

}
