using Electrotank.Electroserver5.Api;
using System;
using System.Collections.Generic;

public class Announcement
{
    public enum Type
    {
        Gift,
        Event,
        Advertisement,
    }

    /// <summary>
    /// Chỉ số hiện thị
    /// </summary>
    public int index;
    public string description;
    /// <summary>
    /// Url Target OnClick
    /// </summary>
    public string url;
    /// <summary>
    /// Kiểu hiện thị thông báo
    /// </summary>
    public string imageUrl;
    public Type type;
    public int money;
    public bool currentDay;
	public bool receivered = false;
    public enum Scene
    {
        lobby,
        announce,
        login,
    }
    public Scene show = Scene.announce;

    /// <summary>
    /// Khởi tạo cho gift qua đối tượng esobject
    /// </summary>
    public Announcement(EsObject eso)
    {
        //this.index = index;
        //this.description = description;
        this.money = eso.getInteger("moneyGift");
        type = Type.Gift;
        this.currentDay = eso.getBoolean("currentDate");
    }
    /// <summary>
    /// Khởi tạo cho Gift
    /// </summary>
    public Announcement(int index, string description, int money,bool currentDay) 
    {
        this.index = index;
        this.description = description;
        this.money = money;
        type = Type.Gift;
        this.currentDay = currentDay;
    }

    /// <summary>
    /// Khởi tạo cho sự kiện hoặc quảng cáo
    /// </summary>
    public Announcement(int index, string description, Scene show, string gotoUrl, string imageUrl, Type type)
    {
        this.index = index;
        this.description = description;
        this.url = gotoUrl;
        this.type = type;
        this.show = show;
        this.imageUrl = imageUrl;

        ServerWeb.GetImageFromUrl(imageUrl, "", delegate(UnityEngine.Texture texture) { _image = texture; });
    }

    UnityEngine.Texture _image;
    public void LoadTexture(CallBackDownloadImage callback)
    {
        if (_image != null)
            callback(_image);
        else
            ServerWeb.GetImageFromUrl(imageUrl, "", delegate(UnityEngine.Texture texture) 
            { 
                _image = texture;
                callback(_image);
            });
    }

    public void OpenUrl()
    {
        if(!string.IsNullOrEmpty(url) && url.Trim().ToLower().StartsWith("http"))
            UnityEngine.Application.OpenURL(url);
    }
}
