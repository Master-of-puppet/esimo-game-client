using System;
using UnityEngine;
using System.Collections;

public class SocialCommon : MonoBehaviour
{
    //	public static string[] FB_PERMISSION = { "email", "publish_stream", "user_birthday", "user_location", "user_about_me" ,"basic_info"};
    public static string[] FB_PERMISSION = { "email", "publish_stream", "user_birthday", "user_location", "user_about_me", "basic_info" };
    string cookie;

    public enum ETypeSocial
    {
        facebook,
        google,
        yahoo,
        twitter
    }

    static SocialCommon _instance;

    static public SocialCommon Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("___Social Common", typeof(SocialCommon));
                _instance = obj.GetComponent<SocialCommon>();

            }
            return _instance;
        }
    }

    public void LoginWithAccessToken(ETypeSocial social)
    {
        CallBackResponse callBack = null;
        WWWForm form = new WWWForm();
        form.AddField(ServerWeb.PARAM_TYPE, social.ToString());
        form.AddField(ServerWeb.PARAMS_ACCESS_TOKEN, (social == ETypeSocial.facebook ? FB.AccessToken : social == ETypeSocial.google ? Google.instance.accessToken : social == ETypeSocial.twitter ? "" : ""));
        form.AddField(ServerWeb.PARAMS_APP_ID, Convert.ToString(GameManager.GAME));
        switch (social)
        {
            case ETypeSocial.facebook:
                callBack = ProressAfterLoginFacebook;
                break;
            case ETypeSocial.google:
                callBack = ProressAfterLoginGoogle;
                break;
        }
        form.AddField(ServerWeb.PARAM_PARTNER_ID, GameSettings.Instance.ParnerCodeIdentifier);
        ServerWeb.StartThread(ServerWeb.URL_GET_ACCESS_TOKEN, form, callBack);
    }

    string GetAccessToken(IDictionary json)
    {
        string accessToken = json["accessToken"].ToString();
        if (isSaveAccessToken)
            StoreGame.SaveString(StoreGame.EType.SAVE_ACCESSTOKEN, accessToken);
        return accessToken;
    }

    void ProressAfterLoginFacebook(bool isDone, WWW textResponse, IDictionary json)
    {
        StartCoroutine(_ProressAfterLoginFacebook(isDone, textResponse, json));
    }
    IEnumerator _ProressAfterLoginFacebook(bool isDone, WWW textResponse, IDictionary json)
    {
        if (isDone)
        {
            WaitingView.Hide();
            switch (json["code"].ToString())
            {
                case "1":
                    WaitingView.Show("Đang đăng nhập");
                    GameManager.Instance.accessToken = json["accessToken"].ToString();
                    GameManager.Instance.userNameLogin = json["username"].ToString();
                    GameManager.Server.DoLogin();
                    break;
                case "-1":
                    reLoginFaceBook();
                    break;
                case "-2":
                    if (Application.isWebPlayer)
                    {
                        GameManager.Instance.UpdateCookieString();
                        while (GameManager.Instance.Cookie.Length == 0)
                        {
                            yield return new WaitForEndOfFrame();
                        }
                        string[] arr = GameManager.Instance.Cookie.Split(';');
                        Array.ForEach(arr, c => { if (c.Contains("PHPSESSID")) { cookie = c; } });
                    }
                    else
                    {
                        cookie = textResponse.responseHeaders[ServerWeb.PHP_COOKIE];
                    }
                    if (GameObject.Find("LoginScreen Code") != null)
                    {
                        GameObject.Find("LoginScreen Code").gameObject.GetComponent<LoginScreenView>().formRegister.GetComponent<RegisterView>().SessionId = cookie;
                        GameObject.Find("LoginScreen Code").gameObject.GetComponent<LoginScreenView>().formRegister.GetComponent<RegisterView>().iUsername.value = json["suggestUser"].ToString() ;
                        if(GameObject.Find("LoginScreen Code").gameObject.GetComponent<LoginScreenView>().IsFormRegisterHide){
                            GameObject.Find("LoginScreen Code").gameObject.GetComponent<LoginScreenView>().ShowFormRegister();
                        }
                    }
                    //RegisterView.Create(cookie, json["suggestUser"].ToString());
                    break;
            }

        }
    }

    void ProressAfterLoginGoogle(bool isDone, WWW textResponse, IDictionary json)
    {
        if (isDone)
        {
            if (json["code"].ToString() == "1")
            {
                GameManager.Server.DoLogin(GetAccessToken(json), GameManager.Instance.deviceToken);
            }
            else
            {
                //RegisterView.Create(textResponse.responseHeaders[ServerWeb.PHP_COOKIE], json["suggestUser"].ToString());
            }
        }
    }

    void ProressAfterLoginYahoo(bool isDone, string textResponse, IDictionary json)
    {
        if (isDone)
        {

            if (json["code"].ToString() == "1")
            {
                GameManager.Server.DoLogin(GetAccessToken(json), GameManager.Instance.deviceToken);
            }
            else
            {
                //reLoginFaceBook();
            }
        }
    }

    public void LoginFaceBook(bool isSaveAccessToken)
    {
        this.isSaveAccessToken = isSaveAccessToken;
//        if (FB.IsLoggedIn || !string.IsNullOrEmpty(FB.AccessToken))
//        {
//            SocialCommon.Instance.LoginWithAccessToken(SocialCommon.ETypeSocial.facebook);
//        }
//        else
//        {
            reLoginFaceBook();
//        }
    }
    private void CallLogin()
    {
        FB.Login("email,publish_actions,publish_stream", callBack);
    }

    private void callBack(FBResult result)
    {
        if (result.Error != null)
            print("Error Response:\n" + result.Error);
        else if (!FB.IsLoggedIn)
        {
            print("Login cancelled by Player");
        }
        SocialCommon.Instance.LoginWithAccessToken(SocialCommon.ETypeSocial.facebook);
    }

    private void reLoginFaceBook()
    {
		WaitingView.Show("Đang lấy thông tin từ Facebook");
#if UNITY_WEBPLAYER
        FacebookWeb.login("email,publish_actions,publish_stream");
#else
            CallLogin();   
#endif

        //#if UNITY_ANDROID
        //        FacebookAndroid.logout();
        //        FacebookAndroid.loginWithPublishPermissions(FB_PERMISSION);
        //#elif UNITY_IPHONE
        //		FacebookBinding.logout();
        //		FacebookBinding.loginWithReadPermissions(FB_PERMISSION);

        //        WaitingView.Show("Chờ phản hồi từ Facebook");
        //        FacebookWeb.login("email,publish_actions,publish_stream");
        //#endif
    }
    public void LoginGoogle()
    {
        string[] accounts = GoogleAndroid.getAllUser();
        LoginGoogleView.Create(accounts);
    }


    public bool isSaveAccessToken { get; set; }
}

