using System;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;

public class LobbyPhom : RoomInfo
{
    public int betting;
    public bool isPassword;
    public string nameLobby;
    public int numberUserInRoom;
    public int maxNumberPlayer;
    public int gameIndex;

    //Others
    public RoomInfo parent = new RoomInfo();
    public GameConfig config;
    public bool gamePlaying;
    public string roomMasterUsername;
    public string[] invitedUsers;
    public int numberRobotAllowed = 1;

    public LobbyPhom() { }

    public LobbyPhom(int zoneId, int roomId, int gameId)
    {
        this.zoneId = zoneId;
        this.roomId = roomId;
        this.gameId = gameId;
    }
    public LobbyPhom(int gameId)
    {
        this.gameId = gameId;
    }

    public LobbyPhom(EsObject obj)
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
        if (obj.variableExists("gameIndex"))
            gameIndex = obj.getInteger("gameIndex");

        isPassword = false;
        if (obj.variableExists("password"))
            isPassword = obj.getBoolean("password");

        if(obj.variableExists("config"))
            SetingConfig(obj.getEsObject("config"));

        if (obj.variableExists("gamePlaying"))
            gamePlaying = obj.getBoolean("gamePlaying");
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

        GameConfig config = new GameConfig(
        esConfig.getBoolean(DEFINE_USING_TYPE_RULE),

        esConfig.getBoolean(DEFINE_USING_U_TRON_RULE),
        esConfig.getBoolean(DEFINE_USING_U_XUONG_RULE),
        esConfig.getBoolean(DEFINE_USING_XAO_KHAN_RULE),
        esConfig.getBoolean(DEFINE_USING_DEN_CHONG_RULE),
        esConfig.getBoolean(DEFINE_USING_CHOT_CHONG_RULE));

        if (esConfig.variableExists(DEFINE_LOBBY_PASWORD))
            config.password = esConfig.getString(DEFINE_LOBBY_PASWORD);
        this.config = config;
    }

    public const string DEFINE_LOBBY_NAME = "description";
    public const string DEFINE_LOBBY_PASWORD = "password";
    public const string DEFINE_BETTING = "betting";
    public const string DEFINE_USING_TYPE_RULE = "optionBaseGame";
    public const string DEFINE_USING_U_TRON_RULE = "usingUTronRule";
    public const string DEFINE_USING_U_XUONG_RULE = "usingUXuongRule";
    public const string DEFINE_USING_XAO_KHAN_RULE = "usingXaoKhanRule";
    public const string DEFINE_USING_DEN_CHONG_RULE = "usingDenChongRule";
    public const string DEFINE_USING_CHOT_CHONG_RULE = "usingChotChongRule";
    public const string DEFINE_INVITED_USERS = "invitedUsers";
    
    public class GameConfig
    {
        public string password;

        /// <summary>
        /// Kiểu bàn chơi server trả về
        /// </summary>
        public string strGameRule;
        public bool isAdvanceGame;
        
        public bool U_TRON_RULE;
        public bool U_XUONG_RULE;
        public bool XAO_KHAN_RULE;
        public bool DEN_CHONG_RULE;
        public bool CHOT_CHONG_RULE;

        public GameConfig(bool BASE_RULE, bool U_TRON_RULE, bool U_XUONG_RULE, bool XAO_KHAN_RULE, bool DEN_CHONG_RULE, bool CHOT_CHONG_RULE)
        {
            this.isAdvanceGame = !BASE_RULE;

            this.U_TRON_RULE = U_TRON_RULE;

            this.U_XUONG_RULE = U_XUONG_RULE;
            this.XAO_KHAN_RULE = XAO_KHAN_RULE;
            this.DEN_CHONG_RULE = DEN_CHONG_RULE;
            this.CHOT_CHONG_RULE = CHOT_CHONG_RULE;

            strGameRule = GetGameRule(U_TRON_RULE, U_XUONG_RULE, XAO_KHAN_RULE, DEN_CHONG_RULE, CHOT_CHONG_RULE);
        }

        public static string GetGameRule(bool U_TRON_RULE, bool U_XUONG_RULE, bool XAO_KHAN_RULE, bool DEN_CHONG_RULE, bool CHOT_CHONG_RULE)
        {
            System.Text.StringBuilder ruleGame = new System.Text.StringBuilder("");
            if (U_TRON_RULE)
                ruleGame.Append("Ù tròn, ");
            if (U_XUONG_RULE)
                ruleGame.Append("Ù xuông, ");
            if (XAO_KHAN_RULE)
                ruleGame.Append("Xào khan, ");
            if (DEN_CHONG_RULE)
                ruleGame.Append("Đền chồng, ");
            if (CHOT_CHONG_RULE)
                ruleGame.Append("Chốt chồng, ");

            return string.IsNullOrEmpty(ruleGame.ToString()) ? "Cơ bản" : ruleGame.ToString().Substring(0, ruleGame.ToString().Length-2);
        }
    }
}
