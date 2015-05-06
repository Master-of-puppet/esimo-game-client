using System;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;

public class RoomInfo
{
	public int zoneId;
	public int roomId;
	public string zoneName;
	public string roomName;
    public int gameId;

	public RoomInfo ()
	{
	}

	public RoomInfo(int zoneId, int roomId) {
		this.zoneId = zoneId;			
		this.roomId = roomId;			
	}
	
	public RoomInfo(Electrotank.Electroserver5.Api.EsObject obj)
	{
        SetDataRoom(obj);
	}

    public void SetDataRoom(Electrotank.Electroserver5.Api.EsObject obj)
    {
        if (obj.variableExists("zoneId"))
            zoneId = obj.getInteger("zoneId");
        if (obj.variableExists("roomId"))
            roomId = obj.getInteger("roomId");
        if (obj.variableExists("zoneName"))
            zoneName = obj.getString("zoneName");
        if (obj.variableExists("roomName"))
            roomName = obj.getString("roomName");
    }

    public virtual void SetDataJoinLobby(EsObject obj) { }
}

