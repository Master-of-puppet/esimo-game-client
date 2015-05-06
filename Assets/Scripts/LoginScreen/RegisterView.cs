using UnityEngine;
using System.Collections;
using Electrotank.Electroserver5.Api;
using System.Collections.Generic;

public class RegisterView : MonoBehaviour
{
    #region UnityEditor
    public CUIHandle btRegister;
    public UIInput iUsername, iPassword, iRepassword;
	public UIToggle cbPolicy;
	public CUIHandle[] btnDieuKhoan;
    #endregion
	
	[HideInInspector]
    private string sessionId;
	private string email = null;

	public string Email {
		get {
			return email;
		}set{
			email=value;
		}
	}

    [HideInInspector]
    public string SessionId
    {
        get { return sessionId; }
        set { sessionId = value; }
    }
    void Awake()
    {
        CUIHandle.AddClick(btRegister, OnClickButtonRegister);
        CUIHandle.AddClick(btnDieuKhoan, OnClickBtnDieuKhoan);
    }

    private void OnClickBtnDieuKhoan(GameObject targetObject)
    {
        PolicyView.Create();
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(btRegister, OnClickButtonRegister);
        CUIHandle.RemoveClick(btnDieuKhoan, OnClickBtnDieuKhoan);
    }

    void OnClickButtonRegister(GameObject go)
    {
        if (!Utility.Input.IsStringValid(iUsername.value, 4, 32))
            NotificationView.ShowMessage("Username không hợp lệ.\n\nUsername phải từ 4-32 ký tự!");
        else if (!Utility.Input.IsStringValid(iPassword.value, 6, 32))
			NotificationView.ShowMessage("Mật khẩu phải dài từ 6 đến 32 ký tự");
        else if (iPassword.value != iRepassword.value)
            NotificationView.ShowMessage("Xác nhận mật khẩu không phù hợp.");
        else if(!cbPolicy.value)
            NotificationView.ShowMessage("Bạn chưa đồng ý với điều khoản sử dụng của XÓM CỜ BẠC");
        else
            _Register();
    }
    void _Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", iUsername.value);
        form.AddField("password", iPassword.value);
		if(email !=null)
			form.AddField ("email", email);
		if(!string.IsNullOrEmpty(sessionId))
		{
			Hashtable hashTable = new Hashtable();
			string cookie = sessionId.Split(";".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries)[0];
            if (Application.isWebPlayer)
            {
                Application.ExternalEval("document.cookie = " + cookie);
            }
            else
            {
                hashTable.Add("Cookie", cookie);
            }
			form.AddField(ServerWeb.PARAM_TYPE,ServerWeb.TYPE_REGISTER);
            form.AddField(ServerWeb.PARAM_PARTNER_ID,GameSettings.Instance.ParnerCodeIdentifier);
			ServerWeb.StartThread(ServerWeb.URL_GET_ACCESS_TOKEN, form, ProressAfterRegister, hashTable);
        	return;
		}
        form.AddField(ServerWeb.PARAM_PARTNER_ID, GameSettings.Instance.ParnerCodeIdentifier);
		ServerWeb.StartThread(ServerWeb.URL_QUICK_REGISTER, form, ProressAfterRegister);
    }
    void ProressAfterRegister(bool isDone, WWW textResponse, IDictionary json)
    {
        if (isDone)
        {
            NotificationView.ShowMessage(json["message"].ToString(),3f);
            if (json["code"].ToString() == "1")
            {
                GameManager.Setting.IsFirstLogin = true;
                GameManager.Instance.userNameLogin = iUsername.value;
                GameManager.Instance.passwordLogin = iPassword.value;
                GameManager.Server.DoLogin();
            }
        }
        else
            Debug.LogError(textResponse);
    }
}

