using Electrotank.Electroserver5.Api;
public class HallInfo {
    public int hallId;
    public string roomName;
    public string zoneName;
    public int roomId;
    public int zoneId;
    public string decription;
    public int maximumPlayers;
    public int numberUsers;
    public string icon;
    public string logo;
    public int gameIndex;
    public int gameId;
    public HallInfo(EsObject es)
    {
        if (es.variableExists("childId"))
            this.hallId = es.getInteger("childId");
        if (es.variableExists("roomName"))
            this.roomName = es.getString("roomName");
        if (es.variableExists("zoneName"))
            this.zoneName = es.getString("zoneName");
        if (es.variableExists("roomId"))
            this.roomId = es.getInteger("roomId");
        if (es.variableExists("zoneId"))
            this.zoneId = es.getInteger("zoneId");
        if (es.variableExists("description"))
            this.decription = es.getString("description");
        if (es.variableExists("maximumPlayers"))
            this.maximumPlayers = es.getInteger("maximumPlayers");
        if (es.variableExists("numberUsers"))
            this.numberUsers = es.getInteger("numberUsers");
        if (es.variableExists("icon"))
            this.icon = es.getString("icon");
        if (es.variableExists("logo"))
            this.logo = es.getString("logo");
        if (es.variableExists("gameIndex"))
            this.gameIndex = es.getInteger("gameIndex");
        if (es.variableExists("appId"))
            this.gameId = es.getInteger("appId");
    }
}
