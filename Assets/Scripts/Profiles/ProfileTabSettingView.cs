using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ProfileTabSettingView : MonoBehaviour
{
    #region Unity Editor
    public UITexture avatar;
    public CUIHandle btChangeAvatar, btChangePassword, btEditGeneral, btSaveGeneral, btEditSecurity, btSaveSecurity,btnClearEmail,btClearcmnd,btnClearPhone;
    public UIInput txtOldPass, txtNewPass, txtRenewPass;
    public UILabel lbFullname, lbBrithday, lbSex, lbAddress, lbEmail, lbCMTND, lbPhone, lbAddressDetails;
    public UIInput txtFullname, txtDay, txtMonth, txtYear, txtAddress, txtEmail, txtCMTND, txtPhone, txtAddressDetails;
    public GameObject panelShowGeneral, panelEditGeneral, panelShowSecurity, panelEditSecurity,InputEmail,InputCMTND,InputPhone;
    public UIToggle cbMale, cbFemale;
    private bool isChangedInformation = false;
    private bool isChangedPass = false;
    private bool isChangeSecurityInfor = false;
    #endregion

    void Awake()
    {
        CUIHandle.AddClick(btEditGeneral, OnClickBeginEditGeneral);
        CUIHandle.AddClick(btEditSecurity, OnClickBeginEditSecurity);
        CUIHandle.AddClick(btSaveGeneral, OnClickSaveGeneral);
        CUIHandle.AddClick(btSaveSecurity, OnClickSaveSecurity);
        CUIHandle.AddClick(btChangePassword, OnClickChangePass);
        CUIHandle.AddClick(btChangeAvatar, OnClickChangeAvatar);
        panelShowGeneral.SetActive(true);
        panelEditGeneral.SetActive(false);

    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(btEditGeneral, OnClickBeginEditGeneral);
        CUIHandle.RemoveClick(btEditSecurity, OnClickBeginEditSecurity);
        CUIHandle.RemoveClick(btSaveGeneral, OnClickSaveGeneral);
        CUIHandle.RemoveClick(btSaveSecurity, OnClickSaveSecurity);
        CUIHandle.RemoveClick(btChangePassword, OnClickChangePass);
        CUIHandle.RemoveClick(btChangeAvatar, OnClickChangeAvatar);
    }

    private void checkSecurityInformation()
    {
        User user = GameManager.Instance.mInfo;
        if (string.IsNullOrEmpty(user.email) || string.IsNullOrEmpty(user.cmtnd) || string.IsNullOrEmpty(user.phone))
        {
            btEditSecurity.gameObject.SetActive(true);
        }
        else
        {
            btEditSecurity.gameObject.SetActive(false);
        }
    }
    private void OnClickChangeAvatar(GameObject targetObject)
    {
        NativeManager nativeCode = NativeManager.Instance;
        nativeCode.PromptPictureFromAlbum(
            delegate(Texture2D texture)
            {
                avatar.mainTexture = texture;
                GameManager.Instance.mInfo.SetTexture(texture);
                _ChangeAvatar(texture);
                GameObject.Destroy(nativeCode.gameObject);
            }
        );
    }

    void OnClickChangePass(GameObject targetObject)
    {
        if (!Utility.Input.IsStringValid(txtOldPass.value, 4, 32))
            NotificationView.ShowMessage("Mật khẩu cũ không hợp lệ. \n Mật khẩu phải từ 4-32 ký tự! ");
        else if (!Utility.Input.IsStringValid(txtNewPass.value, 8, 32))
            NotificationView.ShowMessage("Mật khẩu phải dài từ 8-32 ký tự!");
        else if (txtNewPass.value != txtRenewPass.value)
            NotificationView.ShowMessage("Mật khẩu nhập lại không trùng nhau!");
        else
        {
            if (!isChangedPass)
            {
                _ChangePassword();
                isChangedPass = !isChangedPass;
            }
        }
    }
    void _ChangePassword()
    {
        User user = GameManager.Instance.mInfo;
        Hashtable hash = new Hashtable();
        hash.Add("username", user.username);
        hash.Add("password", txtOldPass.value);
        hash.Add("newPassword", txtNewPass.value);
        hash.Add("renewPassword", txtRenewPass.value);
        string data = JSON.JsonEncode(hash);
        ServerWeb.StartThread(ServerWeb.URL_CHANGE_INFORMATION, new object[] { ServerWeb.PARAM_TYPE, ServerWeb.TYPE_CHANGE_PASSWORD, ServerWeb.PARAM_DATA, data }, ProcessAfterChangePass);
    }

    private void ProcessAfterChangePass(bool isDone, WWW response, IDictionary json)
    {
        if (isDone)
        {
            isChangedPass = !isChangedPass;
            NotificationView.ShowMessage(json["message"].ToString(), 3f);
            if (json["code"].ToString() == "1")
            {
                txtOldPass.value = "";
                txtNewPass.value = "";
                txtRenewPass.value = "";
            }
        }
    }
    private void ProcessAfterChangeAvatar(bool isDone, WWW response, IDictionary json)
    {
        if (isDone)
        {
            NotificationView.ShowMessage(json["message"].ToString(), 3f);
            //if (json["code"].ToString() == "1")
            //{
            //    txtOldPass.text = "";
            //    txtNewPass.text = "";
            //    txtRenewPass.text = "";
            //}
        }
    }
	public void ShowEditGeneral()
	{
		User user = GameManager.Instance.mInfo;
		panelShowGeneral.SetActive(false);
		panelEditGeneral.SetActive(true);
		txtFullname.value = lbFullname.text;
        txtDay.value = Convert.ToString(user.brithday.Day);
        txtMonth.value = Convert.ToString(user.brithday.Month);
        txtYear.value = Convert.ToString(user.brithday.Year);
		//txtBrithday.text = Convert.ToString(Utility.Convert.TimeToAge(user.brithday));
        txtAddress.value = lbAddress.text;
		cbMale.value = lbSex.text == "Nam";
		cbFemale.value = !cbMale.value;		
	}
    void OnClickBeginEditGeneral(GameObject go)
    {
		ShowEditGeneral ();
    }

    void OnClickSaveGeneral(GameObject go)
    {
        Debug.Log("click click");
        string birthday = (txtDay.value.Length == 1 ? "0" + txtDay.value : txtDay.value) + "/" + (txtMonth.value.Length == 1 ? "0" + txtMonth.value : txtMonth.value) + "/" + txtYear.value;
        if (!Utility.Input.IsBrithday(birthday))
        {
            NotificationView.ShowMessage("Ngày sinh không có thật.\nĐề nghị nhập ngày sinh chính xác.");
            return;
        }
        else
        {
            if (!isChangedInformation)
            {
                _ChangeInformation();
                isChangedInformation = true;
            }
        }


    }
    void _ChangeInformation()
    {
        //string[] array = txtFullname.value.Split(" ".ToCharArray(), StringSplitOptions.None);
        string[] array = txtFullname.value.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        User user = GameManager.Instance.mInfo;
        user.firstName = user.lastName = user.middleName = "";
        if (array.Length > 2)
        {
            user.lastName = array[0];
            user.firstName = array[array.Length - 1];
            user.middleName = "";
            for (int i = 1; i < array.Length - 1; i++)
            {
                user.middleName += array[i] + " ";
            }
        }
        else if (array.Length == 1)
        {
            user.firstName = array[0];
        }
        else if (array.Length == 2)
        {
            user.lastName = array[0];
            user.firstName = array[array.Length - 1];
        }

        string birthday = txtDay.value + "/" + txtMonth.value + "/" + txtYear.value;
        user.brithday = Utility.Convert.StringToTime(birthday, "vi-VN");
        user.address = txtAddress.value;
        user.gender = cbMale.value ? "male" : "female";
        String json = convertUserInfoToJson(user);

        WWWForm form = new WWWForm();
        form.AddField(ServerWeb.PARAM_TYPE, ServerWeb.TYPE_CHANGE_INFO);
        form.AddField(ServerWeb.PARAM_DATA, json);
        form.AddField(ServerWeb.PARAM_PARTNER_ID, GameSettings.Instance.ParnerCodeIdentifier);
        WaitingView.Show("Đang lưu thông tin");
        ServerWeb.StartThread(ServerWeb.URL_CHANGE_INFORMATION, form, ProcessAfterChangeInformation);
    }
    public void _ChangeAvatar(Texture2D texture)
    {
        User user = GameManager.Instance.mInfo;
        string data = convertUserInfoToJson(user, texture);
        WWWForm form = new WWWForm();
        form.AddField(ServerWeb.PARAM_TYPE, ServerWeb.TYPE_CHANGE_AVATAR);
        form.AddField(ServerWeb.PARAM_DATA, data);
        form.AddField(ServerWeb.PARAM_PARTNER_ID, GameSettings.Instance.ParnerCodeIdentifier);
        ServerWeb.StartThread(ServerWeb.URL_CHANGE_INFORMATION, form, ProcessAfterChangeAvatar);
    }
    public void initInformation()
    {
        User user = GameManager.Instance.mInfo;
        lbFullname.text = user.FullName;

        lbBrithday.text = Utility.Convert.TimeToString(user.brithday);

        lbAddress.text = user.address;
        lbSex.text = user.gender != null ? (user.gender.ToLower().Equals("male") ? "Nam" : "Nữ") : "Nam";

        user.AvatarTexture(delegate(Texture _texture) { if (avatar != null) avatar.mainTexture = _texture; });
        lbCMTND.text = Utility.EndcodeNumber(user.cmtnd);
        lbPhone.text = Utility.EndCodePhoneNumber(user.phone);
        lbAddressDetails.gameObject.SetActive(false);   
        //lbAddressDetails.text = user.address;
        lbEmail.text = Utility.EncodeEmail(user.email);
		checkSecurityInformation();
		//ShowTextHintPassword ();

	}
	public void ShowTextHintPassword()
	{

        //txtOldPass.label.password = false;
        //txtRenewPass.label.password = false;
        //txtRenewPass.label.password = false;
//		txtNewPass.selected = true;
//		txtRenewPass.selected = true;
//		GameManager.Instance.FunctionDelay (delegate() {
//			txtRenewPass.selected = false;
//			txtNewPass.selected = false;
//			txtOldPass.selected = false;
//			#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
//
//			if(txtRenewPass.KeyBoard !=null&&txtRenewPass.KeyBoard.active  && txtRenewPass.KeyBoard.wasCanceled == false)
//			{
//				Debug.Log("Keyboard of txtRenewPass");
//			}
//			if(txtNewPass.KeyBoard !=null && txtNewPass.KeyBoard.active  && txtNewPass.KeyBoard.wasCanceled == false)
//			{
//				Debug.Log("Keyboard of txtNewPass");
//				txtNewPass.KeyBoard.active = false;
//			}
//			if(txtOldPass.KeyBoard !=null && txtOldPass.KeyBoard.active  && txtOldPass.KeyBoard.wasCanceled == false)
//			{
//				Debug.Log("Keyboard of txtOldPass");
//				txtOldPass.KeyBoard.active = false;
//			}
//
//			#endif
//			
//		},0.1f);

	}
    void ProcessAfterChangeInformation(bool isDone, WWW textResponse, IDictionary json)
    {
        isChangedInformation = false;
        if (isDone)
        {
            NotificationView.ShowMessage(json["message"].ToString(), 3f);
            if (json["code"].ToString() == "1")
            {
                Hashtable user = (Hashtable)json["user"];
                panelShowGeneral.SetActive(true);
                panelEditGeneral.SetActive(false);
                string gender = user["gender"].ToString().Equals("male") ? "Nam" : "Nữ";
                lbSex.text = gender;

                string[] arr = user["birthday"].ToString().Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                DateTime birthday = new DateTime(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]));

                lbBrithday.text = Utility.Convert.TimeToString(birthday);
                lbAddress.text = user["address"].ToString();
                GameManager.Instance.mInfo.firstName = user["first_name"].ToString();
                GameManager.Instance.mInfo.lastName = user["last_name"].ToString();
                GameManager.Instance.mInfo.middleName = user["middle_name"].ToString();
                GameManager.Instance.mInfo.gender = lbSex.text;
                GameManager.Instance.mInfo.brithday = birthday;
                GameManager.Instance.mInfo.address = lbAddress.text;

                lbFullname.text = GameManager.Instance.mInfo.FullName;
            }
        }
        WaitingView.Instance.Close();
    }
    void ProcessAfterChangeSecurityInformation(bool isDone, WWW textResponse, IDictionary json)
    {
        isChangeSecurityInfor = false;
        if (isDone)
        {
            if (json["code"].ToString() == "1")
            {
                string message = "";
                if (json["code_email"].ToString() == "-1")
                {
                    message += "\n Email đã có trong hệ thống";
                }
                if (json["code_cmt"].ToString() == "-1")
                {
                    message += "\n CMT đã có trong hệ thống";
                }
                if (json["code_mobile"].ToString() == "-1")
                {
                    message += "\n Số điện thoại đã có trong hệ thống";
                }

                if (json["code_email"].ToString() == "-1" && json["code_cmt"].ToString() == "-1" && json["code_mobile"].ToString() == "-1")
                {
                    message += "\nThay đổi thông tin không thành công";
                }
                else
                {
                    message += "\n" + json["message"].ToString();


                    Hashtable user = (Hashtable)json["user"];
                    panelShowSecurity.SetActive(true);
                    panelEditSecurity.SetActive(false);
                    if (user["email"] != null)
                        GameManager.Instance.mInfo.email = user["email"].ToString();
                    if (user["identity_card_number"] != null)
                        GameManager.Instance.mInfo.cmtnd = user["identity_card_number"].ToString();
                    if (user["mobile"] != null)
                        GameManager.Instance.mInfo.phone = user["mobile"].ToString();
                    lbEmail.text = GameManager.Instance.mInfo.email;
                    lbCMTND.text = GameManager.Instance.mInfo.cmtnd;
                    lbPhone.text = GameManager.Instance.mInfo.phone;
                    checkSecurityInformation();
                }
                NotificationView.ShowMessage(message, 3f);

            }
           
        }
    }


    string convertUserInfoToJson(User user)
    {
        System.Collections.Hashtable hash = new System.Collections.Hashtable();
        hash.Add("username", user.username);
        hash.Add("first_name", user.firstName);
        hash.Add("last_name", user.lastName);
        hash.Add("middle_name", user.middleName);
        hash.Add("gender", user.gender);
        hash.Add("address", user.address);
        hash.Add("birthday", user.brithday.Year + ":" + user.brithday.Month + ":" + user.brithday.Day);
        hash.Add("email", user.email);
        hash.Add("identity_card_number", user.cmtnd);
        hash.Add("mobile", user.phone);
        return JSON.JsonEncode(hash);
    }
    string convertUserInfoToJson(User user, Texture2D texture2D)
    {
        byte[] bytes = texture2D.EncodeToPNG();
        string strBase64 = System.Convert.ToBase64String(bytes);

        System.Collections.Hashtable hash = new System.Collections.Hashtable();
        hash.Add("username", user.username);
        hash.Add("avatar", strBase64);
        //hash.Add("last_name", user.lastName);
        //hash.Add("middle_name", user.middleName);
        //hash.Add("gender", user.gender);
        //hash.Add("address", user.address);
        //hash.Add("birthday", user.brithday.Year + ":" + user.brithday.Month + ":" + user.brithday.Day);
        return JSON.JsonEncode(hash);
    }

    void OnClickBeginEditSecurity(GameObject go)
    {
        User user = GameManager.Instance.mInfo;
        panelShowSecurity.SetActive(false);
        panelEditSecurity.SetActive(true);

        txtEmail.value = lbEmail.text;
        txtCMTND.value = lbCMTND.text;
        txtPhone.value = lbPhone.text;

        txtAddressDetails.collider.enabled = false;

        if (!string.IsNullOrEmpty(user.email))
        {
            txtEmail.gameObject.collider.enabled = false;
            btnClearEmail.gameObject.collider.enabled = false;
            txtEmail.GetComponentInChildren<UILabel>().color = new Color(242 / 255f, 120 / 255f, 38 / 255f, 255f);
            InputEmail.gameObject.SetActive(false);
        }
        if (!string.IsNullOrEmpty(user.cmtnd))
        {
            txtCMTND.gameObject.collider.enabled = false;
            btClearcmnd.gameObject.collider.enabled = false;
            txtCMTND.GetComponentInChildren<UILabel>().color = new Color(242 / 255f, 120 / 255f, 38 / 255f, 255f);
            InputCMTND.gameObject.SetActive(false);
        }
        if (!string.IsNullOrEmpty(user.phone))
        {
            txtPhone.gameObject.collider.enabled = false;
            btnClearPhone.gameObject.collider.enabled = false;
            txtPhone.GetComponentInChildren<UILabel>().color = new Color(242 / 255f, 120 / 255f, 38 / 255f, 255f);
            InputPhone.gameObject.SetActive(false);
        }
    }

    void OnClickSaveSecurity(GameObject go)
    {
        User user = GameManager.Instance.mInfo;
        if (user.email == null)
        {
            if (!Utility.Input.IsEmail(txtEmail.value))
            {
                NotificationView.ShowMessage("Email không đúng định dạng. Đề nghị nhập lại.\nVí dụ: abc@company.com");
                return;
            }
        }
        if (user.cmtnd == null)
        {
            if (!Utility.Input.IsCMTND(txtCMTND.value))
            {
                NotificationView.ShowMessage("Số chứng minh thư không có thật.\nSố chứng minh thư là số có 9 chữ số.");
                return;
            }
        }
        if (user.phone == null)
        {
            if (!Utility.Input.IsPhone(txtPhone.value))
            {
                NotificationView.ShowMessage("Số điện thoại không có thật.\nSố điện thoại đúng định dạng là số có 10-11 chữ số.");
                return;
            }
        }
        if (!isChangeSecurityInfor)
        {
            _ChangeSecurityInfor();
            isChangeSecurityInfor = true;
        }




    }

    void _ChangeSecurityInfor()
    {
        User user = new User();
        user.username = GameManager.Instance.mInfo.username;
        user.email = txtEmail.value;
        user.phone = txtPhone.value;
        user.cmtnd = txtCMTND.value;

        String json = convertUserInfoToJson(user);

        WWWForm form = new WWWForm();
        form.AddField(ServerWeb.PARAM_TYPE, ServerWeb.TYPE_CHANGE_SECURITY_INFO);
        form.AddField(ServerWeb.PARAM_DATA, json);
        form.AddField(ServerWeb.PARAM_PARTNER_ID, GameSettings.Instance.ParnerCodeIdentifier);
        ServerWeb.StartThread(ServerWeb.URL_CHANGE_INFORMATION, form, ProcessAfterChangeSecurityInformation);
    }
}
