using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public delegate void CallBackResponse(bool isDone, WWW response, IDictionary json);

public delegate void CallBackDownloadImage(Texture texture);

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Xử lý kết nối với server web PHP
/// </summary>
public class ServerWeb
{
    const string PROTOCOL_FIRST_REQUEST = "http://";
    const string PROTOCOL_RETRY_REQUEST = "https://";

    public static string URL_BASE = PROTOCOL_FIRST_REQUEST + Server + "/";
    //public const string URL_TEST_BASE = PROTOCOL_FIRST_REQUEST + "test.esimo.vn/";
    public static string URL_GETINFO_HELP = URL_BASE + "api/help";
    public static string URL_GETINFO_MESSAGE = URL_BASE + "api/getmessage";

    public static string URL_GETINFO_POLICY = URL_BASE + "api/getPolicy";

    public static string URL_GET_VERSION_GAME = URL_BASE + "api/checkversion";
    public static string URL_GET_CONFIG_GAME = URL_BASE + "api/getconfig?name=allow_recharge";
    public static string URL_FORGOT_PASSWORD = URL_BASE + "api/forgotpassword";
    public static string URL_SEND_FEEDBACK = URL_BASE + "api/feedback";
    public static string URL_QUICK_REGISTER = URL_BASE + "api/quickRegister";
    public static string URL_CHANGE_INFORMATION = URL_BASE + "api/ChangeUserInformation";
    public static string URL_GET_AVATAR_FROM_ID = URL_BASE + "site/getImageFile/file/";
    public static string URL_GET_ACCESS_TOKEN = URL_BASE + "api/GetAccessToken";
    public static string URL_SEND_RECHARGE = URL_BASE + "api/recharge";
    public static string URL_GET_ADS = URL_BASE + "api/getAds";
    public static string URL_GET_RECHARGE = URL_BASE + "api/getInfoRecharge";
    public static string URL_PING = PROTOCOL_FIRST_REQUEST + "test.chieuvuong.com/crossdomain.xml";
    public static string URL_GET_ALL_CONFIGURATION = URL_BASE + "/api/getConfigurationClient";
    public static string URL_GET_ALL_PARTNER = URL_BASE + "/api/getPartner"; // 
    public static string URL_REQUEST_ERROR = URL_BASE + "/api/gameerror";
	public static string URL_REQUEST_SAVE_ACCESSTOKEN = URL_BASE + "/api/saveDeviceToken";
    public static string URL_REQUEST_USER = URL_BASE +"/api/getUser";

	public static string URL_REQUEST_EVENT = URL_BASE + "/api/getPost";
    public static string URL_REQUEST_MARKET = URL_BASE + "/api/shop";

    public const string PARAM_TYPE = "type";
    public const string PARAM_DATA = "data";
    public const string PARAM_CONFIG_CODE_GAME = "code_game";
    public const string PARAM_CONFIG_CODE_PLATFORM = "code_platform";
    public const string PARAM_CONFIG_CORE_VERSION = "core_version";
    public const string PARAM_CONFIG_BUILD_VERSION = "build_version";
    public const string PARAM_CONFIG_CODE_REVISION = "code_revision";
    public const string PARAM_PARTNER_ID = "partner_code_identifier";

	public const string PARAM_DEVICE_TOKEN = "deviceToken";
	public const string PARAM_PLATFORM = "platform";

    public const string TYPE_CHANGE_SECURITY_INFO = "changeInformaionSpecial";
    public const string TYPE_CHANGE_INFO = "changeInfomation";
    public const string TYPE_CHANGE_AVATAR = "changeAvatar";
    public const string TYPE_CHANGE_PASSWORD = "changePassword";
    public const string TYPE_REGISTER = "register";
    public const string PARAMS_ACCESS_TOKEN = "accessToken";
    public const string PARAMS_APP_ID = "appId";
    public const string PHP_COOKIE = "SET-COOKIE";
    /// <summary>
    /// Tạo thread đẩy lên server php.
    /// </summary>
    /// <param name="url">Url truy cập</param>
    /// <param name="form">Form sẽ truyền đi</param>
    /// <param name="function">Hàm response trả về sau khi request thành công</param>
    private static string Server
    {
        get
        {
            return Debug.isDebugBuild ? "chieuvuong.com" : "chieuvuong.com";
        }
    }

    public static void StartThread(string url, WWWForm forms, CallBackResponse function, params object[] arrayParams)
    {
        GameManager.Instance.StartCoroutine(__Thread(url, forms, null, function, arrayParams));
    }
    public static void StartThread(string url, object[] forms, CallBackResponse function, params object[] arrayParams)
    {
        GameManager.Instance.StartCoroutine(__Thread(url, null, forms, function, arrayParams));
    }
    public static void StartThread(string url, CallBackResponse function, params object[] arrayParams)
    {
        GameManager.Instance.StartCoroutine(__Thread(url, null, null, function, arrayParams));
    }
    /// <summary>
    /// Tạo thread  đẩy lên server php . sử dụng ___ThreadHttp.
    /// </summary>
    /// <param name="url">Url truy cập</param>
    /// <param name="form">Form sẽ truyền đi</param>
    /// <param name="function">Hàm response trả về sau khi request thành công</param>
    public static void StartThreadHttp(string url, WWWForm forms, CallBackResponse function, params object[] arrayParams)
    {
        GameManager.Instance.StartCoroutine(__ThreadHttp(url, forms, null, function, arrayParams));
    }
    public static void StartThreadHttp(string url, object[] forms, CallBackResponse function, params object[] arrayParams)
    {
        GameManager.Instance.StartCoroutine(__ThreadHttp(url, null, forms, function, arrayParams));
    }
    public static void StartThreadHttp(string url, CallBackResponse function, params object[] arrayParams)
    {
        GameManager.Instance.StartCoroutine(__ThreadHttp(url, null, null, function, arrayParams));
    }

    /// <summary>
    /// Send một www request . mà khi send request thật bại sẽ dừng lại chứ không gửi thêm protocol https như hàm __Thread ở dưới
    /// </summary>
    /// <param name="url"></param>
    /// <param name="form"></param>
    /// <param name="forms"></param>
    /// <param name="function"></param>
    /// <param name="arrayParams"></param>
    /// <returns></returns>
    static IEnumerator __ThreadHttp(string url, WWWForm form, object[] forms, CallBackResponse function, params object[] arrayParams)
    {
        WWW www;

        if (forms != null && forms.Length > 0)
        {
            if (form == null)
                form = new WWWForm();

            if (forms.Length % 2 != 0)
            {
                Debug.LogError("Thông tin form sai, đề nghị truyền lại thông tin cho form\n" +
                "Một form hợp lệ. WWWForm.AddField(string, int or string);");
                yield break;
            }
            else
            {
                for (int i = 0; i < forms.Length; i += 2)
                {
                    string key = (string)forms[i];
                    object obj = forms[i + 1];
                    if (obj.GetType() == typeof(int))
                        form.AddField(key, Convert.ToInt32(obj));
                    else if (obj.GetType() == typeof(string))
                        form.AddField(key, obj.ToString());
                    else
                    {
                        Debug.LogError("WWWForm.AddField phải là kiểu int hoặc string");
                        yield break;
                    }
                }
            }
        }

        if (arrayParams != null && arrayParams.Length > 0)
        {
            Hashtable newHeader = arrayParams[0] as Hashtable;

            //Trên windows phone 8 cái headers nó là dùng Dictionary chứ không phải Hashtable nhé 
            //(sau này phát triển windows phone 8 thì anh em ở lại chú ý thêm nhé)
#if UNITY_WP8
            Dictionary<string, string> headers = form.headers;
            foreach (DictionaryEntry entry in newHeader)
                headers.Add(entry.Key.ToString(), entry.Value.ToString());
#else
            Hashtable headers = form.headers;
            foreach (DictionaryEntry entry in newHeader)
                headers.Add(entry.Key, entry.Value);
#endif
            www = (form == null) ? new WWW(url) : new WWW(url, form.data, headers);
            yield return www;
        }
        else
        {
            www = (form == null) ? new WWW(url) : new WWW(url, form);
            yield return www;
        }

        
        if (function != null)
        {
            bool isDone = www.isDone || string.IsNullOrEmpty(www.error);
            IDictionary json = !www.isDone || !string.IsNullOrEmpty(www.error) ? null : (IDictionary)JSON.JsonDecode(www.text);
            if(json != null)
                Debug.Log("ServerPHP Response: - " + url + " - " + Utility.Convert.TimeToStringFull(System.DateTime.Now) + "\n" + www.text);
            function(isDone, www, json);
        }
    }
    /// <summary>
    /// Đặt tên hàm là Thread cho nó nguy hiểm thôi. Chứ nó chỉ là send mỗi cái WWW đi thôi.
    /// </summary>
    static IEnumerator __Thread(string url, WWWForm form, object[] forms, CallBackResponse function, params object[] arrayParams)
    {
        WWW www;

        if (forms != null && forms.Length > 0)
        {
            if (form == null)
                form = new WWWForm();

            if (forms.Length % 2 != 0)
            {
                Debug.LogError("Thông tin form sai, đề nghị truyền lại thông tin cho form\n" +
                "Một form hợp lệ. WWWForm.AddField(string, int or string);");
                yield break;
            }
            else
            {
                for (int i = 0; i < forms.Length; i += 2)
                {
                    string key = (string)forms[i];
                    object obj = forms[i + 1];
                    if (obj.GetType() == typeof(int))
                        form.AddField(key, Convert.ToInt32(obj));
                    else if (obj.GetType() == typeof(string))
                        form.AddField(key, obj.ToString());
                    else
                    {
                        Debug.LogError("WWWForm.AddField phải là kiểu int hoặc string");
                        yield break;
                    }
                }
            }
        }

        if (arrayParams != null && arrayParams.Length > 0)
        {
            Hashtable newHeader = arrayParams[0] as Hashtable;

            //Trên windows phone 8 cái headers nó là dùng Dictionary chứ không phải Hashtable nhé 
            //(sau này phát triển windows phone 8 thì anh em ở lại chú ý thêm nhé)
#if UNITY_WP8
            Dictionary<string, string> headers = form.headers;
            foreach (DictionaryEntry entry in newHeader)
                headers.Add(entry.Key.ToString(), entry.Value.ToString());
#else
            Hashtable headers = form.headers;
            foreach (DictionaryEntry entry in newHeader)
                headers.Add(entry.Key, entry.Value);
#endif
            www = (form == null) ? new WWW(url) : new WWW(url, form.data, headers);
            yield return www;
        }
        else
        {
            www = (form == null) ? new WWW(url) : new WWW(url, form);
            yield return www;
        }

        if (!www.isDone || !string.IsNullOrEmpty(www.error))
        {
            Debug.LogWarning(string.Format(url + " - Fail Whale!\n{0}", www.error));

            if (url.StartsWith(PROTOCOL_FIRST_REQUEST))
            {
                url = url.Replace(PROTOCOL_FIRST_REQUEST, PROTOCOL_RETRY_REQUEST);
                Debug.Log("Retry request by protocal https: " + url);
                StartThread(url, form, function, arrayParams);
            }

            yield break;
        }
            Debug.Log("ServerPHP Response: - " + url + " - " + Utility.Convert.TimeToStringFull(System.DateTime.Now) + "\n" + www.text);
     

        if (function != null)
        {
            bool isDone = www.isDone || string.IsNullOrEmpty(www.error);
            IDictionary json = !www.isDone || !string.IsNullOrEmpty(www.error) ? null : (IDictionary)JSON.JsonDecode(www.text);
            function(isDone, www, json);
        }
    }
    /// <summary>
    /// Lấy ảnh từ ID
    /// </summary>
    public static void GetAvatarFromId(int id, CallBackDownloadImage callback)
    {
        if (id == 0)
        {
            if (callback != null)
                callback((Texture)Resources.Load("Images/Avatar/NoAvatar", typeof(Texture)));
            return;
        }
        new AvatarCacheOrDownload(ServerWeb.URL_GET_AVATAR_FROM_ID + id, callback);
    }

    /// <summary>
    /// Download ảnh từ Url web
    /// </summary>
    /// <param name="url">Url sẽ download ảnh</param>
    /// <param name="pathResourcesDefaultImage">Image mặc định nếu không thể download</param>
    /// <param name="callback">Callback method khi download xong</param>
    public static void GetImageFromUrl(string url, string pathResourcesDefaultImage, CallBackDownloadImage callback)
    {
        GameManager.Instance.StartCoroutine(_DownloadImage(url, pathResourcesDefaultImage, callback));
    }
    static IEnumerator _DownloadImage(string url, string pathResourcesDefaultImage, CallBackDownloadImage callback)
    {
        Texture _texture;

        if (string.IsNullOrEmpty(url))
            _texture = (Texture)Resources.Load(pathResourcesDefaultImage, typeof(Texture));
        else
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.error != null)
                _texture = (Texture)Resources.Load(pathResourcesDefaultImage, typeof(Texture));
            else
                _texture = www.texture;
        }
        callback(_texture);
    }

    //IEnumerator UploadTexture(string data)
    //{
    //    byte[] bytes = tex.EncodeToPNG ();
    //    string strBase64 = System.Convert.ToBase64String (bytes);

    //    //yield return new WaitForEndOfFrame();

    //    WWWForm form = new WWWForm ();
    //    form.AddBinaryData ("fileUpload", bytes, "filename.png", "image/png");
    //    form.AddField ("image", strBase64, System.Text.Encoding.ASCII);

    //    WWW www = new WWW ("URL", form);
    //    yield return www;
    //}

}
