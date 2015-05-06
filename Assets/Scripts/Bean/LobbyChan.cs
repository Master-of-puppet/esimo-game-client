using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;

public class LobbyChan : RoomInfo, ISimilarComparable
{
    public enum EGaRule
    {
        none = 0,
        NuoiGa = 1,
        GaNhai = 2,

    }
    public int betting;
    public bool isPassword;
    public string nameLobby;
    public int numberUserInRoom;
    public int maxNumberPlayer;
    public int gameIndex;
    public int timePlay;
    public bool autoBatBao;
    public bool autoU;
    public EGaRule ruleGa;
    //Others
    public RoomInfo parent = new RoomInfo();
    public GameConfig config;
    public bool gamePlaying = false;
    public string roomMasterUsername;
    public string[] invitedUsers;
    public int numberRobotAllowed = 1;
    public bool hasRobot;
    public LobbyChan() { }

    public LobbyChan(int zoneId, int roomId, int gameId)
    {
        this.zoneId = zoneId;
        this.roomId = roomId;
        this.gameId = gameId;
    }
    public LobbyChan(int gameId)
    {
        this.gameId = gameId;
    }
    //chi de test
    public LobbyChan(int betting, bool ispassword, string namelobby, int numberUserInRoom, int maxNumberPlayer, int timePlay, bool autoBatBao, bool autoU, int nuoiga)
    {
        this.betting = betting;
        this.isPassword = ispassword;
        this.nameLobby = namelobby;
        this.numberUserInRoom = numberUserInRoom;
        this.maxNumberPlayer = maxNumberPlayer;
        this.timePlay = timePlay;
        this.autoBatBao = autoBatBao;
        this.autoU = autoU;
        this.ruleGa = (EGaRule)nuoiga;
    }
    public LobbyChan(EsObject obj)
    {
        SetDataChannelLobby(obj);
    }

    public void SetDataChannelLobby(EsObject obj)
    {
        base.SetDataRoom(obj);
        if (obj.variableExists("gameId"))
            gameId = obj.getInteger("gameId");
        if (obj.variableExists("description"))
            nameLobby = obj.getString("description");
        if (obj.variableExists("maximumPlayers"))
            maxNumberPlayer = obj.getInteger("maximumPlayers");
        if (obj.variableExists("numberUsers"))
            numberUserInRoom = obj.getInteger("numberUsers");
        if (obj.variableExists("betting"))
            betting = obj.getInteger("betting");
        if (obj.variableExists("playActionTime"))
            timePlay = obj.getInteger("playActionTime");
        if (obj.variableExists("gameIndex"))
            gameIndex = obj.getInteger("gameIndex");
        if (obj.variableExists("hasRobot"))
            hasRobot = obj.getBoolean("hasRobot");
        isPassword = false;
        if (obj.variableExists("password"))
            isPassword = obj.getBoolean("password");

        if (obj.variableExists("config"))
            SetingConfig(obj.getEsObject("config"));

        if (obj.variableExists("gamePlaying"))
            gamePlaying = obj.getBoolean("gamePlaying");

        autoBatBao = false;
        if (obj.variableExists("usingAutoBatBaoRule"))
            gamePlaying = obj.getBoolean("usingAutoBatBaoRule");

        autoU = false;
        if (obj.variableExists("usingAutoURule"))
            gamePlaying = obj.getBoolean("usingAutoURule");

        if (obj.variableExists("usingNuoiGaRule"))
            ruleGa = (EGaRule)obj.getInteger("usingNuoiGaRule");
    }

    public void UpdateData(LobbyChan lobby)
    {
        gameId = lobby.gameId;
        nameLobby = lobby.nameLobby;
        gameIndex = lobby.gameIndex;
        gamePlaying = lobby.gamePlaying;
		isPassword = lobby.isPassword;
        hasRobot = lobby.hasRobot;
        numberUserInRoom = lobby.numberUserInRoom;
    }
    public override void SetDataJoinLobby(EsObject obj)
    {
        SetDataChannelLobby(obj);
        if (obj.variableExists("gameDetails"))
        {
            EsObject gameDetails = obj.getEsObject("gameDetails");

            if (gameDetails.variableExists("config"))
            {
                EsObject esConfig = gameDetails.getEsObject("config");
                SetingConfig(esConfig);
            }

            if (gameDetails.variableExists("parent"))
                parent = new RoomInfo(gameDetails.getEsObject("parent"));
            if (gameDetails.variableExists(DEFINE_INVITED_USERS))
                invitedUsers = gameDetails.getStringArray(DEFINE_INVITED_USERS);
            if (gameDetails.variableExists("roomMasterUsername"))
                roomMasterUsername = gameDetails.getString("roomMasterUsername");
            if (gameDetails.variableExists("gameIndex"))
                gameIndex = gameDetails.getInteger("gameIndex");

            if (gameDetails.variableExists("defaultInfo"))
            {
                EsObject defaultInfo = gameDetails.getEsObject("defaultInfo");
                numberRobotAllowed = defaultInfo.getInteger("numRobotAllowed");
            }
        }
    }

    public void SetingConfig(EsObject esConfig)
    {
        if (esConfig.variableExists(DEFINE_LOBBY_NAME))
            nameLobby = esConfig.getString(DEFINE_LOBBY_NAME);
        if (esConfig.variableExists(DEFINE_BETTING))
            betting = esConfig.getInteger(DEFINE_BETTING);
        if (esConfig.variableExists(DEFINE_INVITED_USERS))
            invitedUsers = esConfig.getStringArray(DEFINE_INVITED_USERS);
        if (esConfig.variableExists(DEFINE_PLAY_ACTION_TIME))
            timePlay = esConfig.getInteger(DEFINE_PLAY_ACTION_TIME);
        if (esConfig.variableExists(DEFINE_USING_AUTO_BAT_BAO))
            autoBatBao = esConfig.getBoolean(DEFINE_USING_AUTO_BAT_BAO);
        if (esConfig.variableExists(DEFINE_USING_AUTO_U))
            autoU = esConfig.getBoolean(DEFINE_USING_AUTO_U);
        if (esConfig.variableExists(DEFINE_USING_NUOI_GA))
            ruleGa = (EGaRule)esConfig.getInteger(DEFINE_USING_NUOI_GA);

        GameConfig config = new GameConfig(
        esConfig.getInteger(DEFINE_RULE_FULL_PLAYING),
        esConfig.getInteger(DEFINE_USING_NUOI_GA),
        esConfig.getBoolean(DEFINE_USING_AUTO_BAT_BAO), esConfig.getInteger(DEFINE_PLAY_ACTION_TIME), esConfig.getBoolean(DEFINE_USING_AUTO_U));

        if (esConfig.variableExists(DEFINE_LOBBY_PASWORD))
            config.password = esConfig.getString(DEFINE_LOBBY_PASWORD);
        this.config = config;

    }

    public const string DEFINE_LOBBY_NAME = "description";
    public const string DEFINE_LOBBY_PASWORD = "password";
    public const string DEFINE_BETTING = "betting";
    public const string DEFINE_RULE_FULL_PLAYING = "ruleFullLaying";
    public const string DEFINE_USING_NUOI_GA = "usingNuoiGaRule";
    public const string DEFINE_USING_AUTO_BAT_BAO = "usingAutoBatBaoRule";
    public const string DEFINE_USING_AUTO_U = "usingAutoURule";
    public const string DEFINE_INVITED_USERS = "invitedUsers";
    public const string DEFINE_PLAY_ACTION_TIME = "playActionTime";

    public class GameConfig
    {
        public string password;

        /// <summary>
        /// Kiểu bàn chơi server trả về
        /// </summary>
        public string strGameRule;
        public int RULE_FULL_PLAYING;
        public EGaRule NUOI_GA_RULE;
        public bool AUTO_BAT_BAO_RULE;
        public int DEFINE_PLAY_ACTION_TIME;
        public bool AUTO_U_RULE;

        public GameConfig(int RULE_FULL_PLAYING, int NUOI_GA_RULE, bool AUTO_BAT_BAO_RULE, int DEFINE_PLAY_ACTION_TIME, bool AUTO_U_RULE)
        {
            this.RULE_FULL_PLAYING = RULE_FULL_PLAYING;
            this.NUOI_GA_RULE = (EGaRule)NUOI_GA_RULE;
            this.AUTO_BAT_BAO_RULE = AUTO_BAT_BAO_RULE;
            this.DEFINE_PLAY_ACTION_TIME = DEFINE_PLAY_ACTION_TIME;
            this.AUTO_U_RULE = AUTO_U_RULE;

            strGameRule = GetGameRule(this.NUOI_GA_RULE);
        }

        public static string GetGameRule(EGaRule rule)
        {
            switch (rule)
            {
                case EGaRule.GaNhai:
                    return "Gà nhái";
                case EGaRule.NuoiGa:
                    return "Gà nuôi";
                default:
                    return "Không chơi";
            }
        }
    }


    public bool Compare(Dictionary<string, object> dicCompare)
    {
        return false;
    }
    public Dictionary<string, object> iComperable;

    public Dictionary<string, object> ToDictionary()
    {
        iComperable = new Dictionary<string, object>();
        iComperable.Add(Fields.LobbyFilter.KEY_ROOM_NUMBER,gameIndex);
        iComperable.Add(Fields.LobbyFilter.KEY_NAME, nameLobby.ToLower());
        iComperable.Add(Fields.LobbyFilter.KEY_MONEY, betting);
        iComperable.Add(Fields.LobbyFilter.KEY_RULE, config.RULE_FULL_PLAYING);
        iComperable.Add(Fields.LobbyFilter.KEY_USER, numberUserInRoom);
        iComperable.Add(Fields.LobbyFilter.KEY_TYPE, gamePlaying);
        iComperable.Add(Fields.LobbyFilter.OBJECT_REFRENCE, this);
        return iComperable;
    }
}