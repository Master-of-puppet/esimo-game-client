using UnityEngine;
using System.Collections;
using Electrotank.Electroserver5.Api;
using System.Collections.Generic;

public class ForgotPasswordView : MonoBehaviour
{
    #region UnityEditor
    public CUIHandle btSend, btClose;
    public UIInput txtEmail;
    #endregion

    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickButtonClose;

        CUIHandle.AddClick(btSend, OnClickButtonForgot);
        CUIHandle.AddClick(btClose, OnClickButtonClose);
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(btSend, OnClickButtonForgot);
        CUIHandle.RemoveClick(btClose, OnClickButtonClose);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnClickButtonForgot(GameObject go)
    {
        btSend.StopImpact(2f);

        if (!Utility.Input.IsEmail(txtEmail.value))
        {
            NotificationView.ShowMessage("Đề nghị nhập một email hợp lệ !", 3f);
            return;
        }

        _Send();
    }

    void OnClickButtonClose(GameObject go)
    {
        GameObject.Destroy(gameObject);
    }

    public static RegisterView Create()
    {
        if (GameObject.Find("__Forgot Password Root") != null)
            return null;

        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/ForgotPasswordPrefab"));
        obj.name = string.Format("__Forgot Password Root");
        obj.transform.localPosition = new Vector3(500f, 0f, -5f);
        return obj.GetComponent<RegisterView>();
    }

    void _Send()
    {
        ServerWeb.StartThread(ServerWeb.URL_FORGOT_PASSWORD, new object[] { "email", txtEmail.value }, ProressAfterSend);
    }

    void ProressAfterSend(bool isDone, WWW textResponse, IDictionary json)
    {
        if (gameObject == null) return;

        if (isDone)
        {
            NotificationView.ShowMessage(json["message"].ToString());
            if (json["code"].ToString() == "1")
                OnClickButtonClose(gameObject);
        }
        else
            Debug.LogError(textResponse);
    }
}

