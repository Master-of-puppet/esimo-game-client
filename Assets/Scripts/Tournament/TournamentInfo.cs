using UnityEngine;
using System.Collections;
using Electrotank.Electroserver5.Api;
using System;

public class TournamentInfo {

    public int tournamentId { get; private set; }
    public string tournamentName { get; private set; }
    public string decription { get; private set; }
    public string startDate { get; private set; }
    public string endDate { get; private set; }
    public string startTime { get; private set; }
    public int costRegistration { get; private set; }
    public string moneyType { get; private set; }
    public string consdition { get; private set; }
    public string award { get; private set; }
    public string awardLink { get; private set; }
    public int maxPlayers { get; private set; }
    public long remainStartTime { get; set; }
    public bool isRegister { get; set; }
    public int numPlayersRegister { get; private set; }
    public int zoneId { get; set; }
    public int roomId { get; set; }
    public string userNameWin { get; set; }
    public string avatarWinner { get; set; }

	public TournamentInfo(EsObject es)
    {
        zoneId = roomId = -1;
        if (es.variableExists("id"))
            this.tournamentId = es.getInteger("id");
        if (es.variableExists("roomId"))
            this.roomId = es.getInteger("roomId");
        if (es.variableExists("zoneId"))
            this.zoneId = es.getInteger("zoneId");
        if (es.variableExists("displayName"))
            this.tournamentName = es.getString("displayName");
        if (es.variableExists("description"))
            this.decription = es.getString("description");
        if (es.variableExists("startTimeRegistration"))
            startDate =es.getString("startTimeRegistration");
        if (es.variableExists("endTimeRegistration"))
            endDate = es.getString("endTimeRegistration");
        if (es.variableExists("startTime"))
            startDate = es.getString("startTime");
        if (es.variableExists("config"))
        {
            SetConfig((IDictionary)JSON.JsonDecode(es.getString("config")));
        }
        if (es.variableExists("maxPlayers"))
            this.maxPlayers = es.getInteger("maxPlayers");
        if (es.variableExists("remainStartTime"))
            this.remainStartTime = es.getLong("remainStartTime");
        if (es.variableExists("isRegister"))
            this.isRegister = es.getBoolean("isRegister");
        if (es.variableExists("numPlayersRegister"))
            this.numPlayersRegister = es.getInteger("numPlayersRegister");
        if(es.variableExists("winner"))
        {
            EsObject obj = es.getEsObject("winner");
            if (obj.variableExists("userName"))
                this.userNameWin = obj.getString("userName");
            if (obj.variableExists("avatar"))
                this.avatarWinner = obj.getString("avatar");
        }
    }

    void SetConfig(IDictionary config)
    {
        this.costRegistration = int.Parse(config["costRegister"].ToString());
        this.moneyType = config["moneyType"].ToString();
        this.consdition = config["condition"].ToString();
        this.award = config["award"].ToString();
        this.awardLink = config["awardLink"].ToString();
    }
}
