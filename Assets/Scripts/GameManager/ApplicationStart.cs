using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Class xử lý sau ứng dụng được khởi chạy
/// </summary>
public class ApplicationStart
{
    public event CallBackFunction EventLoadAnnounce;
    public event CallBackFunction EventLoadConfig;

    public ApplicationStart()
    {
        #region CLEAR CACHE WITH OLD VERSION
        if (!StoreGame.Contains(StoreGame.EType.VERSION) || (StoreGame.LoadString(StoreGame.EType.VERSION) != GameSettings.CurrentVersion))
        {
            StoreGame.ClearCache();
            StoreGame.SaveString(StoreGame.EType.VERSION, GameSettings.CurrentVersion);
        }
        #endregion

        #region KIỂM TRA CẤU HÌNH CÁC CONFIGURATION CỦA SERVER
        GameManager.Instance.StartCoroutine(DoRequestAllConfiguration("Kiểm tra kết nối đến server"));
        #endregion

        #region KIỂM TRA THÔNG TIN HỖ TRỢ (1)
        if (!StoreGame.Contains(StoreGame.EType.CACHE_HELP))
            GameManager.Instance.StartCoroutine(DoRequest_AllHelp());
        else
             GameManager.Instance.StartCoroutine(onProcessGetInfoRule(StoreGame.LoadString(StoreGame.EType.CACHE_HELP), true));
        #endregion

        #region KIỂM TRA THÔNG TIN ĐIỀU KHOẢN SỬ DỤNG (1)
        //if (!StoreGame.Contains(StoreGame.EType.CACHE_POLICY))
        //    GameManager.Instance.StartCoroutine(DoRequest_Policy());
        //else
        //    GameManager.Instance.StartCoroutine(onProcessGetInfoRule(StoreGame.LoadString(StoreGame.EType.CACHE_POLICY), true));
        #endregion

        #region REQUEST CHECK VERSION
        GameManager.Instance.StartCoroutine(DoRequestVersion());
        #endregion

        #region REQUEST GET ADS
        GameManager.Instance.StartCoroutine(DoRequestAds());
        #endregion

        #region REQUEST RECHARGE
        GameManager.Instance.StartCoroutine(DoRequestRecharge());
        #endregion
    }

    IEnumerator DoRequestAds()
    {
        string isReRequest = null;
        ServerWeb.StartThreadHttp(ServerWeb.URL_GET_ADS, new object[] { ServerWeb.PARAM_PARTNER_ID,GameSettings.Instance.ParnerCodeIdentifier }, delegate(bool isDone, WWW response, IDictionary json)
        {
            if (isDone)
            {
                if (string.IsNullOrEmpty(response.error))
                {
                    isReRequest = "0";
                    if (json["code"].ToString() == "1")
                    {

                        ArrayList list = (ArrayList)json["item"];
                        foreach (Hashtable obj in list)
                        {
                            Announcement announce = new Announcement(
                                Convert.ToInt32(obj["index"]),
                                obj["description"].ToString(),
                                obj["scenes"].ToString() == "lobby"
                                    ? Announcement.Scene.lobby
                                    : obj["scenes"].ToString() == "login"
                                    ? Announcement.Scene.login
                                    : Announcement.Scene.announce,
                                obj["url"].ToString(),
                                obj["image"].ToString(),
                                obj["type"].ToString() == "Ads" ? Announcement.Type.Advertisement : Announcement.Type.Event
                            );
                            GameManager.Instance.ListAnnouncement.Add(announce);
                        }
                        if (EventLoadAnnounce != null) EventLoadAnnounce();
                    }
                }
                else
                {
                    isReRequest = "1";
                }
            }

        },null);
        while (string.IsNullOrEmpty(isReRequest))
            yield return new WaitForEndOfFrame();
        if (isReRequest == "1")
        {
            yield return new WaitForSeconds(2f);
            GameManager.Instance.StartCoroutine(DoRequestAds());
        }
    }
    IEnumerator DoRequestVersion()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.isEditor)
        {
            bool? reRequest = null;
            WWWForm form = new WWWForm();
            form.AddField("version", (GameSettings.Instance.CODE_VERSION_BUILD));
			form.AddField("offical_version", GameSettings.CurrentVersion);
            form.AddField("game_id", (int)GameManager.GAME);
            form.AddField("device", PlatformSetting.GetPlatform.ToString());
            form.AddField(ServerWeb.PARAM_PARTNER_ID, GameSettings.Instance.ParnerCodeIdentifier);
            ServerWeb.StartThreadHttp(ServerWeb.URL_GET_VERSION_GAME, form, delegate(bool isDone, WWW response, IDictionary json)
            {
                if (isDone)
                {
                    if (string.IsNullOrEmpty(response.error))
                    {
                        reRequest = false;
                        if (json["code"].ToString() == "1")
                        {
                            string url = json["link"].ToString();
                            GameManager.Setting.IsMustUpdate = url;
                            Common.MustUpdateGame();
                        }
                        else if (json["code"].ToString() == "2")
                        {
                            string newVersion = json["core_version"].ToString() + "." + json["build_version"].ToString() + "." + json["code_version_build"].ToString();
                            string url = json["link"].ToString();

                            NotificationView.ShowConfirm("Cập nhật phiên bản mới", "Hiện tại chúng tôi đã có phiên bản mới v" + newVersion + "\n\n" + "Hãy cập nhật và cảm nhận những tính năng mới.",
                                delegate() { Application.OpenURL(url); },
                                null, "CẬP NHẬT", "ĐỂ SAU");
                        }
                    }
                    else
                        reRequest = true;
                }
            });
            while (reRequest == null)
                yield return new WaitForEndOfFrame();
            if (reRequest == true)
            {
                yield return new WaitForSeconds(2f);
                GameManager.Instance.StartCoroutine(DoRequestVersion());
            }
        }
    }

    #region REQUEST ALL CONFIGURATION
    IEnumerator DoRequestAllConfiguration(string message)
    {

        int getConfigurationCount = 0;
        WaitingView.Show(message);
        bool? reRequest = null;
        WWWForm form = new WWWForm();
        form.AddField(ServerWeb.PARAM_CONFIG_CODE_GAME, GameManager.GAME.ToString());
        form.AddField(ServerWeb.PARAM_CONFIG_CODE_PLATFORM, PlatformSetting.GetPlatform.ToString());
        form.AddField(ServerWeb.PARAM_CONFIG_CORE_VERSION, GameSettings.Instance.CORE_VERSION);
        form.AddField(ServerWeb.PARAM_CONFIG_BUILD_VERSION, GameSettings.Instance.BUILD_VERSION);
        form.AddField(ServerWeb.PARAM_CONFIG_CODE_REVISION, GameSettings.Instance.CODE_VERSION_BUILD);
        form.AddField(ServerWeb.PARAM_PARTNER_ID, GameSettings.Instance.ParnerCodeIdentifier);
        ServerWeb.StartThreadHttp(ServerWeb.URL_GET_ALL_CONFIGURATION, form, delegate(bool isDone, WWW response, IDictionary json)
        {
            if (isDone)
            {
                GameManager.Instance.FunctionDelay(delegate()
                {
                    if (string.IsNullOrEmpty(response.error))
                    {
                        WaitingView.Instance.Close();
                        reRequest = false;
                        if (json[Fields.RESPONSE.PHP_RESPONSE_CODE].ToString() == "0")
                        {
                            ArrayList items = (ArrayList)json[Fields.RESPONSE.PHP_RESPONSE_ITEMS];
                            foreach (Hashtable obj in items)
                            {
                                GameManager.Setting.Platform.AddOrUpdatePlatformConfig(obj);
                            }
                            if (GameManager.CurrentScene == ESceneName.ChannelChan)
                            {
                                //HeaderMenu.Instance.ActiveButtonRecharge();
                            }
                            if (GameManager.Setting.Platform.GetConfigByType(PlatformType.url_ping) != null)
                            {
                                ServerWeb.URL_PING = GameManager.Setting.Platform.GetConfigByType(PlatformType.url_ping).Value;
                            }

                            if (GameManager.Setting.Platform.GetConfigByType(PlatformType.realtime_server) != null)
                            {
                                CServer.HOST_NAME = GameManager.Setting.Platform.GetConfigByType(PlatformType.realtime_server).Value;
                            }

                            if (EventLoadConfig != null)
                                EventLoadConfig();
                        }
                    }
                    else
                    {
                        reRequest = true;
                    }

                }, 0.2f);

                
            }
        });
        while (reRequest == null)
            yield return new WaitForEndOfFrame();
        if (reRequest == true)
        {
            getConfigurationCount++;
            if (getConfigurationCount < 3)
            {
                yield return new WaitForSeconds(1f);
                GameManager.Instance.StartCoroutine(DoRequestAllConfiguration("Kết nối bị lỗi , Đang kiểm tra lại kết nối "));
            }
            else
            {
                NotificationView.ShowConfirm("Thông báo", "Không thể kết nối đến server của chúng tôi . Bấm đồng ý để tiếp tục kiểm tra lại ", delegate()
                {
                    getConfigurationCount = 0;
                    GameManager.Instance.StartCoroutine(DoRequestAllConfiguration("Kiểm tra kết nối đến server"));
                }, null);
            }
        }
    }
    #endregion
    #region REQUEST TẤT CẢC MỨC NẠP TIỀN TRÊN SERVER
    IEnumerator DoRequestRecharge()
    {

        string reRequest = null;
		WWWForm form = new WWWForm();
		form.AddField(ServerWeb.PARAM_CONFIG_CODE_GAME, GameManager.GAME.ToString());
		form.AddField(ServerWeb.PARAM_CONFIG_CODE_PLATFORM, PlatformSetting.GetPlatform.ToString());
		form.AddField(ServerWeb.PARAM_CONFIG_CORE_VERSION, GameSettings.Instance.CORE_VERSION);
		form.AddField(ServerWeb.PARAM_CONFIG_BUILD_VERSION, GameSettings.Instance.BUILD_VERSION);
		form.AddField(ServerWeb.PARAM_CONFIG_CODE_REVISION, GameSettings.Instance.CODE_VERSION_BUILD);
		form.AddField(ServerWeb.PARAM_PARTNER_ID, GameSettings.Instance.ParnerCodeIdentifier);
        ServerWeb.StartThreadHttp(ServerWeb.URL_GET_RECHARGE, form, delegate(bool isDone, WWW response, IDictionary json)
        {
            if (isDone)
            {

                if (string.IsNullOrEmpty(response.error))
                {
                    reRequest = "0";
                    if (json["code"].ToString() == "1")
                    {
                        ArrayList items = (ArrayList)json["items"];
                        foreach (Hashtable obj in items)
                        {
                            RechargeModel model = new RechargeModel(obj);
                            GameManager.Instance.ListRechargeModel.Add(model);
                        }
                    }
                }
                else
                    reRequest = "1";
            }
        }, null);

        while (string.IsNullOrEmpty(reRequest))
            yield return new WaitForEndOfFrame();
        if (reRequest == "1")
        {
            yield return new WaitForSeconds(2f);
            GameManager.Instance.StartCoroutine(DoRequestRecharge());
        }
    }
    #endregion

    #region KIỂM TRA THÔNG TIN HỖ TRỢ (1)
    IEnumerator DoRequest_AllHelp()
    {

        string reRequest = null;
        ServerWeb.StartThreadHttp(ServerWeb.URL_GETINFO_HELP, new object[] { "game_id", (int)GameManager.GAME ,ServerWeb.PARAM_PARTNER_ID, GameSettings.Instance.ParnerCodeIdentifier }, delegate(bool isDone, WWW response, IDictionary json)
        {
            if (isDone)
            {
                if (string.IsNullOrEmpty(response.error))
                {
                    reRequest = "0";
                    if (json["code"].ToString() == "1")
                    {
                        string strSaveRule = JSON.JsonEncode(json["help"]);
                        StoreGame.SaveString(StoreGame.EType.CACHE_HELP, strSaveRule);
                        GameManager.Instance.StartCoroutine(onProcessGetInfoRule(strSaveRule, false));
                    }
                }
                else
                    reRequest = "1";
            }
        });
        while (string.IsNullOrEmpty(reRequest)) 
            yield return new WaitForEndOfFrame();
        if (reRequest == "1")
        {
            yield return new WaitForSeconds(2f);
            GameManager.Instance.StartCoroutine(DoRequest_AllHelp());
        }
    }
    IEnumerator onProcessGetInfoRule(string textJson, bool isCache)
    {
        string reRequest = null;

        ArrayList list = (ArrayList)JSON.JsonDecode(textJson);
        foreach (Hashtable obj in list)
            GameManager.Instance.ListHelp.Add(obj);

        if (list.Count > 0)
        {
            //GameManager.Instance.ListHelp.Sort((x, y) => DateTime.Parse(x["time"].ToString()).CompareTo(DateTime.Parse(y["time"].ToString())));
            GameManager.Instance.ListHelp.Sort((x, y) => int.Parse(x["id"].ToString()).CompareTo(int.Parse(y["id"].ToString())));

            if (isCache)
            {
                WWWForm form = new WWWForm();
                string time = string.Format("{0:yyyy-MM-dd HH:mm:ss}", GameManager.Instance.ListHelp[list.Count - 1]["time"].ToString());
                form.AddField("time", time);
                form.AddField("game_id", (int)GameManager.GAME);
                form.AddField(ServerWeb.PARAM_PARTNER_ID, GameSettings.Instance.ParnerCodeIdentifier);
                ServerWeb.StartThreadHttp(ServerWeb.URL_GETINFO_HELP, form, delegate(bool isDone, WWW response, IDictionary json)
                {
                    if (isDone)
                    {
                        if (string.IsNullOrEmpty(response.error))
                        {
                            reRequest = "0";
                            if (json["code"].ToString() == "1")
                            {
                                StoreGame.Remove(StoreGame.EType.CACHE_HELP);
                                GameManager.Instance.ListHelp.Clear();
                                GameManager.Instance.StartCoroutine(DoRequest_AllHelp());
                            }
                        }
                        else
                        {

                            reRequest = "1";
                        }
                    }
                });
                while (string.IsNullOrEmpty(reRequest))
                    yield return new WaitForEndOfFrame();
                if (reRequest == "1")
                {
                    yield return new WaitForSeconds(2f);
                    GameManager.Instance.StartCoroutine(DoRequest_AllHelp());
                }
            }
        }

    }
    #endregion

    #region KIỂM TRA THÔNG TIN ĐIỀU KHOẢN SỬ DỤNG DỊCH VỤ
    //IEnumerator OnProcessPolicy(string textJson, bool isCache)
    //{
    //    string reRequest = null;

    //    Hashtable list = (Hashtable)JSON.JsonDecode(textJson);
    //    GameManager.Instance.PolicyModel = new PolicyModel(list);

    //    if (list != null)
    //    {

    //        if (isCache)
    //        {
    //            WWWForm form = new WWWForm();
    //            string time = string.Format("{0:yyyy-MM-dd HH:mm:ss}", GameManager.Instance.PolicyModel.CreateDate);
    //            form.AddField("update_time", time);
    //            ServerWeb.StartThreadHttp(ServerWeb.URL_GETINFO_POLICY, form, delegate(bool isDone, WWW response, IDictionary json)
    //            {
    //                if (isDone)
    //                {
    //                    if (string.IsNullOrEmpty(response.error))
    //                    {
    //                        reRequest = "1";
    //                        if (json["code"].ToString() == "1")
    //                        {
    //                            StoreGame.Remove(StoreGame.EType.CACHE_POLICY);
    //                            GameManager.Instance.ListHelp.Clear();
    //                            GameManager.Instance.StartCoroutine(DoRequest_Policy());
    //                        }
    //                    }
    //                    else
    //                    {

    //                        reRequest = "0";
    //                    }
    //                }
    //            });
    //            while (string.IsNullOrEmpty(reRequest))
    //                yield return new WaitForEndOfFrame();
    //            if (reRequest == "0")
    //            {
    //                yield return new WaitForSeconds(2f);
    //                GameManager.Instance.StartCoroutine(onProcessGetInfoRule(StoreGame.LoadString(StoreGame.EType.CACHE_HELP), true));
    //            }
    //        }
    //    }
    //}


    //IEnumerator DoRequest_Policy()
    //{
    //    string reRequest = null;
    //    ServerWeb.StartThreadHttp(ServerWeb.URL_GETINFO_POLICY, delegate(bool isDone, WWW response, IDictionary json)
    //    {
    //        if (isDone)
    //        {
    //            if (string.IsNullOrEmpty(response.error))
    //            {
    //                reRequest = "1";
    //                if (json["code"].ToString() == "1")
    //                {
    //                    StoreGame.SaveString(StoreGame.EType.CACHE_HELP, JSON.JsonEncode(json));
    //                    GameManager.Instance.StartCoroutine(OnProcessPolicy(JSON.JsonEncode(json), false));
    //                }
    //            }
    //            else
    //                reRequest = "0";
    //        }
    //    });
    //    while (string.IsNullOrEmpty(reRequest))
    //        yield return new WaitForEndOfFrame();
    //    if (reRequest == "0")
    //    {
    //        yield return new WaitForSeconds(2f);
    //        GameManager.Instance.StartCoroutine(DoRequest_Policy());
    //    }
    //}


    #endregion
}
