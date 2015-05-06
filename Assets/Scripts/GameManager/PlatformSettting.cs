using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public enum PlatformType
{
    none = -1,
    allow_recharge = 0,
    support_skype = 1,
    support_yahoo = 2,
    support_phone = 3,
    support_email = 4,
    broadcast_message = 5,
    url_ping = 6,
    realtime_server = 7,
	sms_recharge_provider = 8,
    sms_recharge_template = 9,
    ping_interval = 10,
}

public enum Platform
{
    web = 5,
    fb = 6,
    android = 10,
    android_official = 11,
    android_unofficial = 12,
    editor = 2,
    ios = 7,
    ios_official = 8,
    ios_unofficial = 9,
    pc = 4
}
/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Class xử lý các setting mà có cho mỗi platform
/// </summary>
public class PlatformSetting
{

    public static Platform GetPlatform
    {
        get
        {
            Platform plat = Platform.editor;
            switch (UnityEngine.Application.platform)
            {
#if UNITY_ANDROID
                case UnityEngine.RuntimePlatform.Android:
                    switch (GameSettings.Instance.TypeBuildFor)
                    {
                        case GameSettings.EBuildType.android_official:
                            plat = Platform.android_official;
                            break;
                        case GameSettings.EBuildType.esimo:
                        case GameSettings.EBuildType.android_unofficial:
                            plat = Platform.android;
                            break;
                    }
                    break;
#elif UNITY_IPHONE
                case UnityEngine.RuntimePlatform.IPhonePlayer:
                    switch (GameSettings.Instance.TypeBuildFor)
                    {
                        case GameSettings.EBuildType.ios_official:
                            plat = Platform.ios_official;
                            break;
                        case GameSettings.EBuildType.esimo:
                        case GameSettings.EBuildType.ios_unofficial:
                            plat = Platform.ios;
                            break;
                    }
                    break;
#endif
                case UnityEngine.RuntimePlatform.WindowsPlayer:
                case UnityEngine.RuntimePlatform.OSXPlayer:
                case UnityEngine.RuntimePlatform.LinuxPlayer:
                    plat = Platform.pc;
                    break;
#if UNITY_WEBPLAYER
                case UnityEngine.RuntimePlatform.WindowsWebPlayer:
                case UnityEngine.RuntimePlatform.OSXWebPlayer:
                    switch (GameSettings.Instance.TypeBuildFor)
                    {
                        case GameSettings.EBuildType.esimo:
                        case GameSettings.EBuildType.web_esimo:
                            plat = Platform.web;
                            break;
                        case GameSettings.EBuildType.web_facebook:
                            plat = Platform.fb;
                            break;

                    }
                    break;
#endif
            }

            return plat;
        }
    }

		public static Platform GetSamplePlatform
		{
			get
			{
				Platform plat = Platform.editor;
				switch (UnityEngine.Application.platform)
				{
				case UnityEngine.RuntimePlatform.Android:
					plat = Platform.android;
					break;
				case UnityEngine.RuntimePlatform.IPhonePlayer:
					plat = Platform.ios;
					break;
				case UnityEngine.RuntimePlatform.WindowsPlayer:
				case UnityEngine.RuntimePlatform.OSXPlayer:
				case UnityEngine.RuntimePlatform.LinuxPlayer:
					plat = Platform.pc;
					break;
				case UnityEngine.RuntimePlatform.WindowsWebPlayer:
				case UnityEngine.RuntimePlatform.OSXWebPlayer:
						plat = Platform.web;
					break;
				}
				return plat;
			}
		}

    List<PlatformConfig> configs = new List<PlatformConfig>();

    public List<PlatformConfig> Configs
    {
        get { return configs; }
    }
    public bool EnableRecharge
    {
        get
        {
            PlatformConfig config = configs.Find(c => c.Type == PlatformType.allow_recharge);
            if (config != null)
                return config.Enable;
            return false;
        }
    }

    public static bool GetEnablePlatform(string json)
    {
        Hashtable hash = (Hashtable)JSON.JsonDecode(json);
        return true;
    }
    public void AddOrUpdatePlatformConfig(Hashtable obj)
    {
        PlatformConfig newConfig = new PlatformConfig(obj["name"].ToString(), obj["value"].ToString());
        PlatformConfig oldConfig = configs.Find(cfg => cfg.Type == newConfig.Type);
        if (oldConfig != null)
            oldConfig.Value = newConfig.Value;
        else
            configs.Add(newConfig);
    }
    public PlatformConfig GetConfigByType(PlatformType type)
    {
        PlatformConfig config = configs.Find(c => c.Type == type);
        return config;
    }
}
#region CONFIG GAME
public class PlatformConfig
{
    public PlatformConfig(string name, string value)
    {
        this.name = name;
        this.platform = PlatformSetting.GetPlatform;
        this.value = value;
        this.type = ConvertNameToType(name);
    }
    public Platform platform;
    string name;
    private PlatformType type;

    public PlatformType Type
    {
        get { return type; }
        set { type = value; }
    }
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    private string value;

    public string Value
    {
        get { return this.value; }
        set { this.value = value; }
    }
    public bool Enable
    {
        get
        {
            int number;
            bool isNumber = int.TryParse(value, out number);
            if (isNumber)
            {
                if (number > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public PlatformType ConvertNameToType(string name)
    {
        MemberInfo[] memberInfos = typeof(PlatformType).GetMembers(BindingFlags.Public | BindingFlags.Static);
        for (int i = 0; i < memberInfos.Length; i++)
        {
            if (memberInfos[i].Name == name)
            {
                return (PlatformType)Enum.Parse(typeof(PlatformType), memberInfos[i].Name);
            }
        }
        return PlatformType.none;
    }
}
#endregion

