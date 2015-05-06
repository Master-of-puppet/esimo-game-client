using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Các biến Config và kiểm tra dùng chung trong game.
/// </summary>
public class Common
{
    #region GAME RULE
    /// <summary>
    /// Luật: Số tiền được phép vào bàn chơi (tiền có bằng 17 lần tiền cược) / mức này cho game chắn
    /// </summary>
    public static int RULE_CHIP_COMPARE_BETTING = 17;

    #endregion

    public static bool IsRelease
    {
        get
        {
            return Application.isEditor == false &&
            (Application.platform == RuntimePlatform.Android
            || Application.platform == RuntimePlatform.IPhonePlayer
            || Application.platform == RuntimePlatform.WindowsWebPlayer
            || Application.platform == RuntimePlatform.OSXWebPlayer);
        }
    }

    public static bool IsMobile
    {
        get
        {
            return Application.platform == RuntimePlatform.Android
            || Application.platform == RuntimePlatform.IPhonePlayer;
        }
    }


    public static bool IsExistsEscape
    {
        get
        {
            return Application.isEditor
            || Application.platform == RuntimePlatform.Android
            || Application.platform == RuntimePlatform.WindowsPlayer
            || Application.platform == RuntimePlatform.OSXWebPlayer
            || Application.platform == RuntimePlatform.WindowsWebPlayer;
        }
    }
    public static bool ValidateChipToBetting(int betting, string type)
    {
        if (type == "chip")
            return GameManager.Instance.mInfo.chip >= (betting * RULE_CHIP_COMPARE_BETTING);
        if (type == "gold")
            return GameManager.Instance.mInfo.gold >= (betting * RULE_CHIP_COMPARE_BETTING);
        return false;
    }
    public static bool ValidateChipToBetting(int betting)
    {
        return GameManager.Instance.mInfo.chip >= (betting * RULE_CHIP_COMPARE_BETTING);
    }
    public static void MessageRecharge(string message)
    {
        if(GameManager.Setting.Platform.EnableRecharge)
            NotificationView.ShowConfirm("Thông báo", message + "\n\nẤn \"Đồng ý\" để chuyển đến trang nạp tiền", delegate() { RechargeView.Create(); }, null);
        else
            NotificationView.ShowMessage(message);
    }

    public static void VersionNotSupport(string feature)
    {
        string mess;
        if (feature != null)
            mess = "Tính năng \"" + feature + "\" hiện chưa được hỗ trợ.\n\nTrên nền tảng " + Application.platform.ToString();
        else
            mess = "Tính năng này hiện chưa được hỗ trợ.\n\nTrên nền tảng " + Application.platform.ToString();

        NotificationView.ShowMessage(mess, 3f);
    }

    public static void MustUpdateGame()
    {
		 NotificationView.ShowConfirmMustUpdate("Thông báo từ hệ thông", "Phiên bản hiện tại của bạn đã lỗi thời.\n\nBạn phải cập nhật phiên bản mới để tiếp tục chơi!",
              delegate() { 
                  Application.OpenURL(GameManager.Setting.IsMustUpdate); 
              }, delegate() {
                  Application.OpenURL(GameManager.Setting.Home());
              }
            );
    }

    public static string GetDevice
    {
        get
        {
            return Application.platform.ToString() + " (" + SystemInfo.deviceModel + " - " + SystemInfo.deviceName + ") " + " - " + SystemInfo.deviceUniqueIdentifier;
        }
    }

    #region GOD MODE
    /// <summary>
    /// Cho phép xem bài của người chơi khác
    /// </summary>
    public static bool CanViewHand
    {
        get { return GameManager.Instance.mInfo.role >= User.ERole.Administrator && _canViewHand; }
        set { _canViewHand = value; }
    }
    static bool _canViewHand = false;

    /// <summary>
    /// Cho phép chế độ Tester
    /// </summary>
    public static bool CanTestMode
    {
        get { return !IsRelease || _showTestMode; }
        set { _showTestMode = value; }
    }
    static bool _showTestMode = false;
    #endregion
}