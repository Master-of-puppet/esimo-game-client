using UnityEngine;
using System.Collections;

public class ChannelChan : RoomInfo
{
    public string description;
    public int maximumPlayers;
    public int numberUsers;
    public int[] bettingValues;
    public int minimumMoney;
    public int[] timePlay;
    public string moneyType;
    public ChannelChan() { }
    public ChannelType type;
    public const int CHANNEL_BILLIONAIRE_BETTING_OTHER_VALUE = -1;
	
    public enum ChannelType
    {
        Amateur = 0,        //Nghiệp dư
        Professional = 1,   //Chuyên nghiệp
        Experts = 2,        //Cao thủ
        Giants = 3,         //Đại gia
        Tournament = 4      //Giải đấu
    }

    public ChannelChan(Electrotank.Electroserver5.Api.EsObject obj) 
    {
		if(obj.variableExists("zoneId"))
        	zoneId = obj.getInteger("zoneId");
		if(obj.variableExists("roomId"))
        	roomId = obj.getInteger("roomId");
		if(obj.variableExists("zoneName"))
        	zoneName = obj.getString("zoneName");
		if(obj.variableExists("roomName"))
        {
            roomName = obj.getString("roomName");
            switch (roomName)
            {
                case "chan_nendat":
                    this.type = ChannelType.Amateur;
                    break;
                case "chan_chieucoi":
                    this.type = ChannelType.Professional;
                    break;
                case "chan_phango":
                    this.type = ChannelType.Experts;
                    break;
                case "chan_sapgu":
                    this.type = ChannelType.Giants;
                    break;
                case "chan_giaidau":
                    this.type = ChannelType.Tournament;
                    break;
                default:
                    break;
            }
        }
		if(obj.variableExists("description"))
        	description = obj.getString("description");
		if(obj.variableExists("maximumPlayers"))
        	maximumPlayers = obj.getInteger("maximumPlayers");
		if(obj.variableExists("numberUsers"))
        	numberUsers = obj.getInteger("numberUsers");
		if(obj.variableExists("bettingValues"))
        	bettingValues = obj.getIntegerArray("bettingValues");
        if (obj.variableExists("minimumMoney"))
            minimumMoney = obj.getInteger("minimumMoney");
        if (obj.variableExists("playActionTime"))
            timePlay = obj.getIntegerArray("playActionTime");
        if (obj.variableExists("moneyType"))
            moneyType = obj.getString("moneyType");
    }
}
