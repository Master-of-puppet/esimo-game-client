using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Class này là để xử lý lưu trữ giả cache để lưu thông tin lại
/// Đọc chi tiết sẽ thấy. Nó cũng rất dễ xài.
/// Set giá trị truyền thêm cái Enum là được.
/// </summary>
public class StoreGame
{
    #region STORE
    public enum EType
    {
        [AttributeCache("Để lưu cache, nhưng hiện tại chưa sử dụng")]
        CACHE,
        [AttributeCache("Kiểm tra phiên bản để clear cache", true)]
        VERSION,
        [AttributeCache("Lưu thông tin hỗ trợ người dùng từ server", true)]
        CACHE_HELP,
        [AttributeCache("Lưu thông tin điều khoản sử dụng game từ server", true)]
        CACHE_POLICY,
        [AttributeCache("Lưu tin nhắn hệ thống", false, true)]
        CACHE_MESSAGE_SYSTEM,
        [AttributeCache("Lưu thông tin trên cùng một device có chơi chung một bàn chơi không ?", true)]
        PLAY_THE_SAME_DEVICE,
        [AttributeCache("Thời gian tài khoản đã xem thông báo trong ngày của game", true, true)]
        ANNOUNCEMENT_IN_DAY,

        [AttributeCache("Save Setting Full Screen", true)]
        FULL_SCREEN,
        [AttributeCache("Save Setting Sound Effect", true)]
        SOUND_EFFECT,
        [AttributeCache("Save Setting Sound Background", true)]
        SOUND_BACKGROUND,
        [AttributeCache("Save Setting Lock Screen", true)]
        LOCK_SCREEN,

        [AttributeCache("Thông tin máy chủ đăng nhập", true)]
        SAVE_SERVER,
        [AttributeCache("Thông tin tài khoản đăng nhập", true)]
        SAVE_USERNAME,
        [AttributeCache("Thông tin mật khẩu đăng nhập", true)]
        SAVE_PASSWORD,
        [AttributeCache("Thông tin access token mạng xã hội", true)]
        SAVE_ACCESSTOKEN,

        [AttributeCache("Thông tin chuyển scenes", true)]
		SEND_FRIEND_MESSAGE,
        [AttributeCache("Thông tin chuyển scenes", true)]
        CHANGE_INFORMATION,

        [AttributeCache("Lưu log để gửi về server khi game gặp lỗi", true)]
        DEBUG_LOG,
        [AttributeCache("Biến kiểm tra có gửi DEBUG_LOG cho server không, true/false", true)]
        BOOL_SEND_LOG_TO_SERVER
    }





    public static string GetKeyName(EType _type)
    {
        AttributeCache attribute = Utility.EnumUtility.GetAttribute<AttributeCache>(_type);

        string str = "";

        if (attribute.EachUsername)
            str += GameManager.Instance.mInfo.username + "_";
        if (attribute.EachGame)
            str += GameManager.GAME.ToString() + "_";

        return str += _type.ToString();
    }
    
    public static void SaveInt(EType _type, int _value)
    {
        PlayerPrefs.SetInt(GetKeyName(_type), _value);
    }

    public static void SaveString(EType _type, string _value)
    {
        PlayerPrefs.SetString(GetKeyName(_type), _value);
    }

    #region HoangVietDuc DEBUG_LOG
    public static void SaveLog(EType _type, string _value)
    {
        if (_type == EType.DEBUG_LOG)
        {
            if (StoreGame.Contains(StoreGame.EType.DEBUG_LOG))
            {
                _value = StoreGame.LoadString(StoreGame.EType.DEBUG_LOG) + "\n\n" + _value;
                PlayerPrefs.SetString(GetKeyName(_type), _value);
            }
        }

    }
    public static void ClearLog() {
        if (StoreGame.Contains(StoreGame.EType.DEBUG_LOG))
        {
            PlayerPrefs.SetString(GetKeyName(StoreGame.EType.DEBUG_LOG), "");
        }
    }
    #endregion





    public static int LoadInt(EType _type)
    {
        return PlayerPrefs.GetInt(GetKeyName(_type));
    }
    public static int LoadInt(EType _type, int _default)
    {
        return PlayerPrefs.GetInt(GetKeyName(_type), _default);
    }
    public static string LoadString(EType _type)
    {
        return PlayerPrefs.GetString(GetKeyName(_type));
    }






    public static bool Contains(EType _type)
    {
        return PlayerPrefs.HasKey(GetKeyName(_type));
    }

    public static void Remove(EType _type)
    {
        if (Contains(_type))
            PlayerPrefs.DeleteKey(GetKeyName(_type));
    }
    
    public static void ClearCache()
    {
        PlayerPrefs.DeleteAll();
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }
    #endregion





    #region VALUE
    public enum EToggle
    {
        OFF = 0,
        ON = 1
    }
    #endregion




    #region Attribute For Enum Cache
    /// <summary>
    /// Mô tả các Enum cache để lưu trữ
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class AttributeCache : Attribute
    {
        public string Description;
        public bool EachGame;
        public bool EachUsername;

        /// <summary>
        /// Lưu trữ theo game và username theo tham số truyền vào
        /// </summary>
        public AttributeCache(string description, bool perGame, bool perUsername)
        {
            Description = description;
            EachGame = perGame;
            EachUsername = perUsername;
        }

        /// <summary>
        /// Không cần lưu trữ theo username nhưng lại lưu trữ theo game với tham số truyền vào
        /// </summary>
        public AttributeCache(string description, bool perGame)
        {
            Description = description;
            EachGame = perGame;
            EachUsername = false;
        }

        /// <summary>
        /// Không cần lưu trữ theo game và username
        /// </summary>
        public AttributeCache(string description)
        {
            Description = description;
            EachGame = false;
            EachUsername = false;
        }
    }
    #endregion
}
