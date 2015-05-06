using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;

#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif
public class GameSettings : ScriptableObject
{
    public enum EServer
    {
        [AttributeServer("210.211.102.97", "chieuvuong.com")]
        MAIN = 0,
        [AttributeServer("210.211.102.121", "test.chieuvuong.com")]
        TEST = 1,
    }
    public enum EBuildType
    {
#if UNITY_IPHONE
        ios_official = 0,
        ios_unofficial = 1,
#elif UNITY_ANDROID
        android_official = 0,
        android_unofficial = 1,
#elif UNITY_WEBPLAYER
        web_esimo = 0 ,
        web_facebook = 1,
#endif
        esimo = 2,
    }
    const string settingsAssetName = "GameSetting";
    const string settingsAssetFolder = "GameSettings";
    const string settingsAssetExtension = ".asset";
    [SerializeField]
    private string urlPartner;

    public string UrlPartner
    {
        get { return urlPartner; }
        set { urlPartner = value; }
    }
    private static GameSettings _instance;

    public static GameSettings Instance
    {
        get
        {

            if (_instance == null)
            {
                _instance = Resources.Load(settingsAssetName) as GameSettings;

#if UNITY_EDITOR
                //UnityEditor.PlayerSettings.Android.keystorePass = "eposi123@123a";
                //UnityEditor.PlayerSettings.Android.keyaliasPass = "eposi123@123a";
                UnityEditor.PlayerSettings.Android.keystorePass = "android";
                UnityEditor.PlayerSettings.Android.keyaliasPass = "android";
#endif

                if (_instance == null)
                {
                    // If not found, autocreate the asset object.
                    _instance = CreateInstance<GameSettings>();
                    _instance.GetListPartner();
#if UNITY_EDITOR
                    string properPath = Path.Combine(Application.dataPath, settingsAssetFolder);
                    if (!Directory.Exists(properPath))
                    {
                        AssetDatabase.CreateFolder("Assets", settingsAssetFolder);
                        properPath = Path.Combine(properPath, "Resources");
                        if (!Directory.Exists(properPath))
                            AssetDatabase.CreateFolder("Assets" + "/" + settingsAssetFolder, "Resources");
                    }
                    string fullPath = Path.Combine(Path.Combine("Assets", settingsAssetFolder + "/" + "Resources"), settingsAssetName + settingsAssetExtension);
                    AssetDatabase.CreateAsset(_instance, fullPath);
#endif
                }
            }
            return _instance;
        }
    }

    //void Awake()
    //{
    //    GetListPartner();
    //}
    #region MENU IN EDITOR
#if UNITY_EDITOR

    [MenuItem("GameSettings/Sửa thông tin game")]
    public static void Edit()
    {
        Selection.activeObject = Instance;
    }

    [MenuItem("GameSettings/Mở trình duyệt web/Mở trang quản trị")]
    public static void OpenWebPlayerAdmin()
    {
        if (!String.IsNullOrEmpty(GameSettings.Instance.WEB_SITE))
        {
            Application.OpenURL(GameSettings.Instance.WEB_SITE + "/admin/");
        }
        else
        {
            string url = "http://xomcobac.com/admin/";
            Application.OpenURL(url);
        }
    }

    [MenuItem("GameSettings/Mở trình duyệt web/Game Phỏm")]
    public static void OpenWebPlayerPhom()
    {
        if (!String.IsNullOrEmpty(GameSettings.Instance.WEB_SITE))
        {
            Application.OpenURL(GameSettings.Instance.WEB_SITE + "/phom/");
        }
        else
        {
            string url = "http://xomcobac.com/phom/";
            Application.OpenURL(url);
        }
    }

    [MenuItem("GameSettings/Mở trình duyệt web/Game TLMN")]
    public static void OpenWebPlayerTLMN()
    {
        if (!String.IsNullOrEmpty(GameSettings.Instance.WEB_SITE))
        {
            Application.OpenURL(GameSettings.Instance.WEB_SITE + "/tlmn/");
        }
        else
        {
            string url = "http://xomcobac.com/tlmn/";
            Application.OpenURL(url);
        }
    }

    [MenuItem("GameSettings/Mở trình duyệt web/Game Chắn")]
    public static void OpenWebPlayerChan()
    {
        if (!String.IsNullOrEmpty(GameSettings.Instance.WEB_SITE))
        {
            Application.OpenURL(GameSettings.Instance.WEB_SITE + "/chan/");
        }
        else
        {
            string url = "http://xomcobac.com/chan/";
            Application.OpenURL(url);
        }
    }

    [MenuItem("GameSettings/Report Bug &&Support", true)]
    public static void OpenWebPlayerEmail()
    {
        string url = "mailto:vietdungvn88@gmail.com";
        Application.OpenURL(url);
    }
#endif
    #endregion

    #region VERSION

    [SerializeField]
    string _coreVersion = "1.0";
    [SerializeField]
    int _buildVersion = 1;
    [SerializeField]
    int _codeVersionBuild = 4000;
    [SerializeField]
    EServer _host = EServer.MAIN;
    public string CORE_VERSION
    {
        get { return _coreVersion; }
        set
        {
            if (_coreVersion == value) return;

            _coreVersion = value;
            SaveVersionEditor();
            DirtyEditor();
        }
    }

    public int BUILD_VERSION
    {
        get { return _buildVersion; }
        set
        {
            if (_buildVersion == value) return;

            _buildVersion = value;
            SaveVersionEditor();
            DirtyEditor();
        }
    }
    public int CODE_VERSION_BUILD
    {
        get { return _codeVersionBuild; }
        set
        {
            if (_codeVersionBuild == value) return;

            _codeVersionBuild = value;
            SaveVersionEditor();
            DirtyEditor();
        }
    }



    public static string CurrentVersion
    {
        get
        {
            return Instance.CORE_VERSION + "."
                + Instance.BUILD_VERSION + "."
                + Instance.CODE_VERSION_BUILD;
        }
    }

    public static void SaveVersionEditor()
    {
#if UNITY_EDITOR
        UnityEditor.PlayerSettings.bundleVersion = CurrentVersion;
        UnityEditor.PlayerSettings.Android.bundleVersionCode = Instance.CODE_VERSION_BUILD;
#endif
    }
    #endregion


    #region OTHER SETTING
    [SerializeField]
    List<Partner> listPartner;
    public List<Partner> ListPartner
    {
        get { return listPartner; }
    }
    [SerializeField]
    List<string> listPartnerName;
    [SerializeField]
    private int indexPartner = 0;
    public int IndexPartner
    {
        get { return indexPartner; }
        set
        {
            indexPartner = value;
            ParnerCodeIdentifier = listPartner[indexPartner].CodeIdentifier;
            DirtyEditor();
        }
    }
    [SerializeField]
    Partner currentPartner;

    [SerializeField]
    private string parnerCodeIdentifier = "esimo";

    public string ParnerCodeIdentifier
    {
        get { return parnerCodeIdentifier; }
        set
        {
            parnerCodeIdentifier = value;
            DirtyEditor();
        }
    }
    [SerializeField]
    private EBuildType typeBuildFor = EBuildType.esimo;


    public EBuildType TypeBuildFor
    {
        get { return typeBuildFor; }
        set { typeBuildFor = value; DirtyEditor(); }
    }

    public List<string> ListPartnerName
    {
        get
        {
            if (Instance.listPartnerName == null) Instance.listPartnerName = new List<string>();
            else Instance.listPartnerName.Clear();
            Instance.ListPartner.ForEach(p => Instance.listPartnerName.Add(p.Name));
            return listPartnerName;
        }
    }

    public void GetListPartner()
    {
        WWW www = new WWW(urlPartner);
        while (!www.isDone) ;
        if (www.isDone && string.IsNullOrEmpty(www.error))
        {
            if (listPartner == null) listPartner = new List<Partner>();
            else listPartner.Clear();
            if (listPartnerName == null) listPartnerName = new List<string>();
            else listPartnerName.Clear();
            bool isDone = www.isDone || string.IsNullOrEmpty(www.error);
            IDictionary json = !www.isDone || !string.IsNullOrEmpty(www.error) ? null : (IDictionary)JSON.JsonDecode(www.text);
            if (json != null)
                Debug.Log("ServerPHP Response: - " + ServerWeb.URL_GET_ALL_PARTNER + " - " + Utility.Convert.TimeToStringFull(System.DateTime.Now) + "\n" + www.text);
            if (json["code"].ToString() == "1")
            {
                ArrayList list = (ArrayList)json["items"];
                foreach (Hashtable obj in list)
                {
                    Partner parner = new Partner(obj);
                    listPartner.Add(parner);
                }
                DirtyEditor();
            }
        }
    }
    public EServer HOST
    {
        get { return _host; }
        set
        {
            if (_host == value) return;
            _host = value;
            urlPartner = Utility.EnumUtility.GetAttribute<AttributeServer>(_host).Domain + "/api/getPartner";
            GetListPartner();
            //if (!Common.IsRelease)
            //    StoreGame.SaveString(StoreGame.EType.SAVE_SERVER, CServer.HOST_NAME);

        }
    }
    #endregion

    #region WEB_SITE
    [SerializeField]
    string _website;
    public string WEB_SITE
    {
        get{return _website;}
        set{_website=value;}
    }
    
    #endregion
    private static void DirtyEditor()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(Instance);
#endif
    }

    public class AttributeServer : System.Attribute
    {
        private string host;
        public string Host
        {
            get { return host; }
            set { host = value; }
        }
        private string domain;

        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }
        public AttributeServer(string host, string domain)
        {
            this.host = host;
            this.domain = domain;
        }
    }


}

