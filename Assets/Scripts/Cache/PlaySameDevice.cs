using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Coder: NGUYỄN VIỆT DŨNG
/// Lưu thông tin những người chơi trên cùng một thiết bị
/// (Không được sửa linh tinh không lỗi các Version phát hành nhé)
/// </summary>
public class PlaySameDevice
{
    const string DATA_ROW = "☻";
    const string DATA_SAPCE = "☺";

    public static bool IsCanJoinGameplay
    {
        get
        {
            if (!Application.isWebPlayer)
                return true;
            
            if (ContainsValue)
            {
                NotificationView.ShowMessage("Bạn không được phép tham gia cùng bàn chơi chơi trên một thiết bị");
                return false;
            }
            return true;
        }
    }

    public static void SaveDeviceWhenJoinGame() 
    {
        string str = "";
        if (StoreGame.Contains(StoreGame.EType.PLAY_THE_SAME_DEVICE))
            str = StoreGame.LoadString(StoreGame.EType.PLAY_THE_SAME_DEVICE) + DATA_ROW;
        str += GameManager.Instance.mInfo.username + DATA_SAPCE + GameManager.Instance.selectedLobby.gameId;

        Debug.LogWarning("IsCanJoinGameplay SAVED: " + str);

        StoreGame.SaveString(StoreGame.EType.PLAY_THE_SAME_DEVICE, str);
        StoreGame.Save();
    }
    static bool ContainsValue
    {
        get
        {
            if (StoreGame.Contains(StoreGame.EType.PLAY_THE_SAME_DEVICE))
            {
                string[] arrays = StoreGame.LoadString(StoreGame.EType.PLAY_THE_SAME_DEVICE).Split(DATA_ROW.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in arrays)
                {
                    string[] arr = str.Split(DATA_SAPCE.ToCharArray(), StringSplitOptions.None);
                    if (System.Convert.ToInt32(arr[1]) == GameManager.Instance.selectedLobby.gameId)
                        return true;
                }
            }
            return false;
        }
    }

    public static void ClearCache()
    {
        if (!Application.isWebPlayer)
            return;

        if (GameManager.CurrentScene == ESceneName.GameplayChan)
            return;

        if (StoreGame.Contains(StoreGame.EType.PLAY_THE_SAME_DEVICE))
        {
            string newValue = "";
            string[] arrays = StoreGame.LoadString(StoreGame.EType.PLAY_THE_SAME_DEVICE).Split(DATA_ROW.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            Debug.LogWarning(StoreGame.LoadString(StoreGame.EType.PLAY_THE_SAME_DEVICE) + "\n" + arrays.Length);
            
            foreach (string str in arrays)
            {
                string[] arr = str.Split(DATA_SAPCE.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (arr[0] == GameManager.Instance.mInfo.username)
                {
                    Debug.LogWarning("IsCanJoinGameplay Remove: " + str);
                    continue;
                }
                newValue += str + DATA_ROW;
            }

            if (newValue != StoreGame.LoadString(StoreGame.EType.PLAY_THE_SAME_DEVICE))
            {
                if (string.IsNullOrEmpty(newValue))
                    StoreGame.Remove(StoreGame.EType.PLAY_THE_SAME_DEVICE);
                else
                    StoreGame.SaveString(StoreGame.EType.PLAY_THE_SAME_DEVICE, newValue);
                StoreGame.Save();
            }
        }
    }
	public static void Clear()
	{
		StoreGame.Remove(StoreGame.EType.PLAY_THE_SAME_DEVICE);
		StoreGame.Save();
	}
}