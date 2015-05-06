using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Electrotank.Electroserver5.Api;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Model của thông tin người chơi (bạn bè, ....)
/// </summary>
public class User
{
    public int id;
    public string username;
    public string password;
    public string email;
    public string firstName, middleName, lastName;
    public string cmtnd;
    public string phone;
    public string fullAddress;
    public string accessToken;
    public string FullName
    {
        get
        {
            //return ((string.IsNullOrEmpty(lastName) ? "" : lastName.Trim() + " ") + (string.IsNullOrEmpty(middleName) ? "" : middleName.Trim() + " ") + (string.IsNullOrEmpty(lastName) ? "" : firstName.Trim())).Trim();
            return string.Join(" ", new string[] { lastName, middleName, firstName }).Trim();
        }
    }
    public DateTime brithday = new DateTime(0);
    public string gender;
    public string address;
    public bool isRobot = false;
    string _avatarUrl = "";

    Texture _avatarTexture;
	
	public void SetTexture(Texture2D texture)
	{
		_avatarTexture = (Texture)texture;
	}

    public void AvatarTexture(CallBackDownloadImage callback)
    {
        if (_avatarTexture != null)
            callback(_avatarTexture);
        else
            new AvatarCacheOrDownload(_avatarUrl, callback);
    }

    public string avatarUrl
    {
        get { return _avatarUrl; }
        set
        {
            if (_avatarUrl == value) return;

            _avatarUrl = value;
			new AvatarCacheOrDownload(_avatarUrl, null);
        }
    }

    public DateTime timeRequest;
    public DateTime createTime;
    public enum ERole
    {
        User = 1,
        Player = 2,
        Tester = 3,
        Administrator = 4,
        System = 5
    }
    public ERole role;
    public int level;
    public int experience;
    public int expMinCurrentLevel;
    public int expMinNextLevel;

    public List<User> buddies = new List<User>();
    public List<User> pendingBuddies = new List<User>();
    public List<User> requestBuddies = new List<User>();

    public List<Messages> messages = new List<Messages>();

    public long chip;
    public long gold;

    public User() { }
    public User(Electrotank.Electroserver5.Api.EsObject obj)
    {
        SetDataUser(obj);
    }

    public void SetDataUser(Electrotank.Electroserver5.Api.EsObject obj)
    {
        if (obj.variableExists("id"))
            id = obj.getInteger("id");

        if (obj.variableExists("username"))
            username = obj.getString("username");
        else if (obj.variableExists(Fields.PLAYER.USERNAME))
            username = obj.getString(Fields.PLAYER.USERNAME);

        if (obj.variableExists("email"))
            email = obj.getString("email");
        if (obj.variableExists("first_name"))
            firstName = obj.getString("first_name");
        if (obj.variableExists("middle_name"))
            middleName = obj.getString("middle_name");
        if (obj.variableExists("last_name"))
            lastName = obj.getString("last_name");
        if (obj.variableExists("birthday"))
            System.DateTime.TryParse(obj.getString("birthday").Replace(":","-"), out brithday);
        if (obj.variableExists("gender"))
            gender = obj.getString("gender");
        if (obj.variableExists("address"))
            address = obj.getString("address");
        if (obj.variableExists("identity_card_number"))
            cmtnd = obj.getString("identity_card_number");
        if (obj.variableExists("mobile"))
            phone = obj.getString("mobile");
        if (obj.variableExists("avatar"))
        {
            if (obj.getDataType("avatar") == DataType.String)
                avatarUrl = obj.getString("avatar");
            else if (obj.getDataType("avatar") == DataType.Integer)
                ServerWeb.GetAvatarFromId(obj.getInteger("avatar"), delegate(Texture _texture) { _avatarTexture = _texture; });
        }

        if (obj.variableExists("create_time"))
            System.DateTime.TryParse(obj.getString("create_time"), out createTime);
        if (obj.variableExists("time_request"))
            System.DateTime.TryParse(obj.getString("time_request"), out timeRequest);
        if (obj.variableExists("numBuddies"))
            numberBuddies = obj.getInteger("numBuddies");
        if (obj.variableExists("role"))
            role = (ERole)obj.getInteger("role");
        if (obj.variableExists("level"))
            level = obj.getInteger("level");
        if (obj.variableExists("experience"))
            experience = obj.getInteger("experience");
        if (obj.variableExists("expMinCurrentLevel"))
            expMinCurrentLevel = obj.getInteger("expMinCurrentLevel");
        if (obj.variableExists("expMinNextLevel"))
            expMinNextLevel = obj.getInteger("expMinNextLevel");

        if (obj.variableExists("buddies"))
        {
			if(buddies !=null && buddies.Count > 0) buddies.Clear();
            EsObject[] array = obj.getEsObjectArray("buddies");
            Array.ForEach<EsObject>(array, o => { buddies.Add(new User(o)); });
        }
        if (obj.variableExists("pendingBuddies"))
        {
			if(pendingBuddies !=null &&pendingBuddies.Count > 0 ) pendingBuddies.Clear();
            EsObject[] array = obj.getEsObjectArray("pendingBuddies");
            Array.ForEach<EsObject>(array, o => { pendingBuddies.Add(new User(o)); });
        }
        if (obj.variableExists("requestBuddies"))
        {
            EsObject[] array = obj.getEsObjectArray("requestBuddies");
            Array.ForEach<EsObject>(array, o => { requestBuddies.Add(new User(o)); });
        }

        if (obj.variableExists("chip"))
        {
            if (obj.getDataType("chip") == DataType.String)
                long.TryParse(obj.getString("chip"), out chip);
            else if (obj.getDataType("chip") == DataType.Long)
                chip = obj.getLong("chip");
        }

        if (obj.variableExists("gold"))
        {
            if (obj.getDataType("gold") == DataType.String)
                long.TryParse(obj.getString("gold"), out gold);
            else if (obj.getDataType("gold") == DataType.Long)
                gold = obj.getLong("gold");
        }

        if (obj.variableExists("accessToken"))
            accessToken = obj.getString("accessToken");
    }

    private int numberBuddies;

    public int NumberBuddies
    {
        get { return numberBuddies; }
        set { numberBuddies = value; }
    }
}
