using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

public class EventModel
{
    private string urlWeb;
    private string imageUrl;
    private string title;
    public string Title
    {
        get { return title; }
        set { title = value; }
    }
    Texture texture;

    public void SetTexture(Texture2D texture1)
    {
        texture = (Texture)texture1;
    }

    public void GetTexture(CallBackDownloadImage callback)
    {
        if (texture != null)
            callback(texture);
        else
            new AvatarCacheOrDownload(imageUrl, callback);
    }

    public string ImageUrl
    {
        get { return imageUrl; }
        set
        {
            if (imageUrl == value) return;
            imageUrl = value;
            new AvatarCacheOrDownload(imageUrl, null);
        }
    }
    public string UrlWeb
    {
        get { return urlWeb; }
        set { urlWeb = value; }
    }
    public EventModel(Hashtable obj) 
    {
        if (obj[KEY_TITLE] != null)
            Title = obj[KEY_TITLE].ToString();
        if (obj[KEY_IMAGE] != null)
            ImageUrl = obj[KEY_IMAGE].ToString();
        if (obj[KEY_URL] != null)
            UrlWeb = obj[KEY_URL].ToString();
    }
    private static string KEY_TITLE = "title";
    private static string KEY_IMAGE = "image";
    private static string KEY_URL = "url";
}

