using Electrotank.Electroserver5.Api;
public class CommandEsObject
{
    public string command;
    public string action;
    public EsObject eso;
    public PluginMessageEvent e;

    public CommandEsObject(string command, string action, EsObject eso)
    {
        this.command = command;
        this.action = action;
        this.eso = eso;
    }

    public CommandEsObject(PluginMessageEvent e, string command, string action, EsObject eso)
    {
        this.command = command;
        this.action = action;
        this.eso = eso;
        this.e = e;
    }
}