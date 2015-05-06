using UnityEngine;
using System.Collections;

public class ChannelTLMN : RoomInfo
{
    public string description;
    public int maximumPlayers;
    public int numberUsers;
    public int[] bettingValues;
    public int minimumMoney;
    public string moneyType;
    public ChannelTLMN() { }
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

    public ChannelTLMN(Electrotank.Electroserver5.Api.EsObject obj) 
    {
		if(obj.variableExists("zoneId"))
        	zoneId = obj.getInteger("zoneId");
		if(obj.variableExists("roomId"))
        	roomId = obj.getInteger("roomId");
		if(obj.variableExists("zoneName"))
        	zoneName = obj.getString("zoneName");
        if (obj.variableExists("roomName"))
        {
            roomName = obj.getString("roomName");
            roomName = obj.getString("roomName");
            switch (roomName)
            {
                case "tilm_nghiepdu":
                    this.type = ChannelType.Amateur;
                    break;
                case "tlmn_chuyennghiep":
                    this.type = ChannelType.Professional;
                    break;
                case "tlmn_caothu":
                    this.type = ChannelType.Experts;
                    break;
                case "tlmn_daigia":
                    this.type = ChannelType.Giants;
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
        if (obj.variableExists("moneyType"))
            moneyType = obj.getString("moneyType");
    }
}
