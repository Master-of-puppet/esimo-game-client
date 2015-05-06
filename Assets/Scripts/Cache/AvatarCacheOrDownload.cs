using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Class xử lý việc tạo cache để không phải download lại ảnh từ server web.
/// Việc tạo cache ở đây cũng chỉ là lưu file ảnh ra Base64String thôi. Không có gì to tát cả.
/// Xong rồi dùng PlayerPrefs để lưu lại xuống thiết bị.
/// </summary>
public class AvatarCacheOrDownload
{
    public static Dictionary<string, List<CallBackDownloadImage>> downloading = new Dictionary<string, List<CallBackDownloadImage>>();

    public AvatarCacheOrDownload(string url, CallBackDownloadImage callback)
    {
        if (PlayerPrefs.HasKey(url))
        {
            byte[] bytes = System.Convert.FromBase64String(PlayerPrefs.GetString(url));
            Texture2D text = new Texture2D(0, 0, TextureFormat.ARGB32, false);
            text.LoadImage(bytes);

            if (text == null)
            {
                PlayerPrefs.DeleteKey(url);
                goto LOADNULL;
            }

            if (callback != null)
                callback(text);
            return;
        }

    LOADNULL:
        if (downloading.ContainsKey(url))
            downloading[url].Add(callback);
        else
        {
            downloading.Add(url, new List<CallBackDownloadImage>() { callback });
            GameManager.Instance.StartCoroutine(_Download(url, false));
        }
    }

    public AvatarCacheOrDownload(string url, CallBackDownloadImage callback, bool needTextureNull)
    {
        if (PlayerPrefs.HasKey(url))
        {
            byte[] bytes = System.Convert.FromBase64String(PlayerPrefs.GetString(url));
            Texture2D text = new Texture2D(0, 0, TextureFormat.ARGB32, false);
            text.LoadImage(bytes);

            if (text == null)
            {
                PlayerPrefs.DeleteKey(url);
                goto LOADNULL;
            }

            if (callback != null)
                callback(text);
            return;
        }

    LOADNULL:
        if (downloading.ContainsKey(url))
            downloading[url].Add(callback);
        else
        {
            downloading.Add(url, new List<CallBackDownloadImage>() { callback });
            GameManager.Instance.StartCoroutine(_Download(url, needTextureNull));
        }
    }

    IEnumerator _Download(string url, bool needTextureNull)
    {
        Texture _avatarTexture;
        if (string.IsNullOrEmpty(url))
            if (!needTextureNull)
                _avatarTexture = (Texture)Resources.Load("Images/Avatar/NoAvatar", typeof(Texture));
            else
                _avatarTexture = null;
        else
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.error != null)
                if (!needTextureNull)
                    _avatarTexture = (Texture)Resources.Load("Images/Avatar/NoAvatar", typeof(Texture));
                else
                    _avatarTexture = null;
            else
            {
                _avatarTexture = www.texture;
                byte[] bytes = ((Texture2D)_avatarTexture).EncodeToPNG();
                string strBase64 = System.Convert.ToBase64String(bytes);
                PlayerPrefs.SetString(url, strBase64);
            }
        }

        downloading[url].ForEach(method => { if (method != null) method(_avatarTexture); });
        downloading.Remove(url);
    }
}
