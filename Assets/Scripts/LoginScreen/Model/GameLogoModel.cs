using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class GameLogoModel
{
    string platform;
    public string Platform
    {
        get { return platform; }
        set { platform = value; }
    }
    string game;

    public string Game
    {
        get { return game; }
        set { game = value; }
    }
    string imageUrl;
    Texture _logoTexture;

    public void SetTexture(Texture2D texture)
    {
        _logoTexture = (Texture)texture;
    }

    public void LogoTexture(CallBackDownloadImage callback)
    {
        if (_logoTexture != null)
            callback(_logoTexture);
        else
            new AvatarCacheOrDownload(imageUrl, callback);
    }


    public string ImageUrl
    {
        get { return imageUrl; }
        set {
            if (imageUrl == value) return;
            imageUrl = value;
            new AvatarCacheOrDownload(imageUrl, null);
        }
    }
    string bundle;

    public string Bundle
    {
        get { return bundle; }
        set { bundle = value; }
    }
    string url;

    public string Url
    {
        get { return url; }
        set { url = value; }
    }
	
    public void ParseFromJson(string json)
    {
        IDictionary dic = (IDictionary)JSON.JsonDecode(json);
        this.platform = dic["platform"].ToString();
        this.game = dic["game"].ToString();
        this.imageUrl = dic["image"].ToString();
        this.bundle = dic["bundle"].ToString();
        this.url = dic["url"].ToString();
    }
    public void ParseFromHasble(Hashtable obj)
    {
        this.platform = obj["platform"].ToString();
        this.game = obj["game"].ToString();
        this.imageUrl = obj["image"].ToString();
        this.bundle = obj["bundle"].ToString();
        this.url = obj["url"].ToString();
    }
    public GameLogoModel(Hashtable obj)
    {
        ParseFromHasble(obj);
    }
}

