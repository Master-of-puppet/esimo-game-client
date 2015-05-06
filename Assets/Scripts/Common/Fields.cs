using System;
using System.Collections.Generic;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Các trường đang dùng rất nhiểu text.
/// Tới có thời gian phải sửa lại theo field
/// Trước lười nên không tạo field để làm cho nhanh ^^
/// </summary>
public static class Fields
{
    public const string COMMAND = "command";
    public const string ACTION = "action";

    public struct REQUEST
    {
        public const string APP_ID = "appId";
		public const string ACCESS_TOKEN = "accessToken";
        public const string NODE_PLUGIN = "NodePlugin";
        public const string GAME_PLUGIN = "GamePlugin";
        public const string LOGIN_PLUGIN = "LoginAuthenticationPlugin";

        public const string REQUEST_FULL = "requestFull";
        public const string GET_USER_ONLINE = "getUserOnline";
        public const string DAYLY_GIFT = "getDailyGift";
        public const string LAY_MELDS = "layMelds";
        public const string GA_NGOAI = "bettingGaNgoai";
        public const string GET_BUDDIES = "getBuddies";
        public const string GET_MESSAGE = "getMessage";

        public const string COMMAND_LOGIN = "loginAuthentication";
        public const string COMMAND_TOURNAMENT = "getTournamentList";
        public const string COMMAND_REGISTERTOURNAMENT = "register";
        public const string COMMAND_GETLISTREGISTER = "getRegisterList";
        public const string COMMAND_GETGENERAL = "getRoundList";
        public const string COMMAND_MATCH_LIST = "getMatchList";
    }

    public struct RESPONSE
    {
        public const string FULL_UPDATE = "fullUpdate";
        public const string USER_ONLINE_UPDATE = "updateUserOnline";

        public const string LOBBY_ADD = "childAdded";
        public const string LOBBY_REMOVE = "childRemoved";
        public const string LOBBY_UPDATE = "childUpdated";
        public const string DAYLY_GIFT = "getDailyGift";
        public const string CREATE_GAME = "createGame";
        public const string GET_BUDDIES = "getBuddies";
        public const string GET_MESSAGE = "getMessage";

        public const string PHP_RESPONSE_CODE = "code";
        public const string PHP_RESPONSE_MESSAGE = "message";
        public const string PHP_RESPONSE_ITEMS= "items";

        public const string LOGIN_RESPONSE = "loginAuthentication";
        public const string COMMAND_TOURNAMENT = "getTournamentList";
    }

    public struct CARD
    {
        public const string CARD_ID = "cardId";
    }

    public struct PLAYER
    {
        public const string USERNAME = "userName";
        public const string USERS = "users";
    }

    public struct GAMEPLAY
    {
        public const string PLAY = "play";
        public const string MELDS = "melds";
        public const string PLAYER = "player";
        
    }
	public struct MESSAGE
	{
		public const string FIRST_LOGIN_NOTE = "Khuyến cáo";
        public const string FIRST_LOGIN_MESSAGE = "Đây là lần đầu đăng nhập của bạn, chúng tôi khuyến cáo bạn cập nhật thông tin cá nhân đầy đủ để đảm bảo quyền lợi về sau";
    }
    public struct CONFIGCLIENT
    {
        public const string KEY_TYPE_REAL_TIME = "typeRealTime";
        public const string VALUE_ADS = "ads";
        public const string VALUE_HELP = "help";
        public const string VALUE_CONFIG_CLIENT = "configuration_client";
    }
    public struct LobbyFilter
    {
        public const string KEY_NAME = "name";
        public const string KEY_ROOM_NUMBER= "room_number";
        public const string KEY_MONEY = "money";
        public const string KEY_RULE = "rule";
        public const string KEY_USER = "user";
        public const string KEY_TYPE = "type";

        public const string OBJECT_REFRENCE = "reference";
    }

    public struct StatusPlayer
    {
        public const string GOLD = "gold";
        public const string CHIP = "chip";
    }
}

