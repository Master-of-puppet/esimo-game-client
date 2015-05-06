using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;

/// <summary>
/// VIETDUNG
/// A console that displays the contents of Unity's debug log.
/// </summary>
public class DebugConsoleLog : MonoBehaviour
{
    public static string abc = "Test";
    private const string COMMAND_KEY = "$COMMAND$";
    private const string COMMAND_RETURN = "$RETURN$";
    public Texture background;
    public KeyCode toggleKey = KeyCode.BackQuote; //Key show Debug Console
    const int limitLine = 150;

    class ConsoleMessage
    {
        public readonly string message;
        public readonly string stackTrace;
        public readonly LogType type;
        public bool showStack;

        public ConsoleMessage(string message, string stackTrace, LogType type)
        {
            this.message = message;
            this.stackTrace = stackTrace;
            this.type = type;
            this.showStack = false;
        }
    }

    List<ConsoleMessage> entries = new List<ConsoleMessage>();
    Vector2 scrollPos;
    float margin = (Screen.height * 3 / 4) / 16;
    bool _showLogs;
    bool collapse;
    string command = "";


    Rect windowRect = new Rect();
    float windowHeight = 0;

    //GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

    void OnEnable() { Application.RegisterLogCallback(HandleLog); }
    void OnDisable() { Application.RegisterLogCallback(null); }

    public bool showLogs
    {
        get { return _showLogs; }
        set
        {
            _showLogs = value;
            collider.enabled = value;
        }
    }

    DebugVersion version;
    void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);

        version = new DebugVersion(this);

        collider.enabled = false;
    }

    void Update()
    {
        if (margin != (Screen.height * 3 / 4) / 16)
            margin = (Screen.height * 3 / 4) / 16;

        if (Input.GetKeyDown(toggleKey))
        {
            if (CountTouch == 2)
            {
                showLogs = !showLogs;
                CountTouch = 0;
            }
            else
                CountTouch++;
        }

        if (showLogs && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            scrollPos = new Vector2(0, entries.Count * margin);
            if (command != "")
                ExecuteCommand(command);
        }

        version.Update();
        if (TimeCountDown > 0)
            TimeCountDown -= Time.deltaTime;
    }
    #region VAR
    int _countTouch = 0;
    int CountTouch
    {
        get { return _countTouch; }
        set
        {
            _countTouch = value;
            if (value > 0) TimeCountDown = 1f;
        }
    }
    float _timeCountDown = 0f;
    float TimeCountDown
    {
        get { return _timeCountDown; }
        set
        {
            _timeCountDown = value;
            if (value <= 0) CountTouch = 0;
        }
    }
    #endregion
    void OnGUI()
    {
        version.OnGUI();

        if (Common.CanTestMode
            && GameManager.CurrentScene == ESceneName.GameplayChan && GameModelChan.model != null
            && GameModelChan.YourController != null && GameModelChan.YourController.isMaster && GameModelChan.ListPlayer.Count > 1
            && !showLogs && GUI.Button(new Rect(Screen.width - (Screen.width - Screen.width * 0.9f), Screen.height - 35f, Screen.width - Screen.width * 0.9f, 35f), "TEST MODE"))
            TestModeGUI.Instance.gameObject.SetActive(true);

        if (Common.CanTestMode
           && GameManager.CurrentScene == ESceneName.GameplayPhom && GameModelPhom.model != null
           && GameModelPhom.YourController != null && GameModelPhom.YourController.isMaster && GameModelPhom.ListPlayer.Count > 1
           && !showLogs && GUI.Button(new Rect(Screen.width - (Screen.width - Screen.width * 0.9f), Screen.height - 35f, Screen.width - Screen.width * 0.9f, 35f), "TEST MODE"))
            TestModePhom.Instance.gameObject.SetActive(true);

        if (Common.CanTestMode
           && GameManager.CurrentScene == ESceneName.GameplayTLMN && GameModelTLMN.model != null
           && GameModelTLMN.YourController != null && GameModelTLMN.YourController.isMaster && GameModelTLMN.ListPlayer.Count > 1
           && !showLogs && GUI.Button(new Rect(Screen.width - (Screen.width - Screen.width * 0.9f), Screen.height - 35f, Screen.width - Screen.width * 0.9f, 35f), "TEST MODE"))
            TestModeTLMN.Instance.gameObject.SetActive(true);

        if (!showLogs)
        {
            windowHeight = 0;
            command = "";
            return;
        }

        if (windowHeight < margin * 16)
            windowHeight += margin;

        windowRect = new Rect(0, 0, Screen.width, windowHeight);
        windowRect = GUILayout.Window(99, windowRect, ConsoleWindow, "");
        if (background != null)
            GUI.DrawTexture(windowRect, background);
        GUI.FocusWindow(99);
    }

    /// <summary>
    /// A window displaying the logged messages.
    /// </summary>
    /// <param name="windowID">The window's ID.</param>
    void ConsoleWindow(int windowID)
    {
        GUI.BeginGroup(new Rect(0, 0, Screen.width, windowHeight - 60));
        scrollPos = GUILayout.BeginScrollView(scrollPos, new GUILayoutOption[] { GUILayout.MaxHeight(windowHeight - 60) });

        // Go through each logged entry
        for (int i = 0; i < entries.Count; i++)
        {
            ConsoleMessage entry = entries[i];

            // If this message is the same as the last one and the collapse feature is chosen, skip it
            if (collapse && i > 0 && entry.message == entries[i - 1].message)
                continue;

            // Change the text colour according to the log type
            switch (entry.type)
            {
                case LogType.Error:
                case LogType.Exception:
                    GUI.contentColor = Color.red;
                    break;
                case LogType.Warning:
                    GUI.contentColor = Color.yellow;
                    break;
                default:
                    GUI.contentColor = Color.white;
                    break;
            }
            GUILayout.BeginVertical(GUILayout.MaxWidth(Screen.width));
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width));
            if (GUILayout.Button(entry.showStack ? "Hide" : "Show", GUILayout.MaxWidth(50)))
                entry.showStack = !entry.showStack;
            GUILayout.Label("[" + i + "]" + entry.message);
            GUILayout.EndHorizontal();
            if (entry.showStack)
                GUILayout.Label(entry.stackTrace);
            GUILayout.EndVertical();
        }

        GUILayout.EndScrollView();
        GUI.EndGroup();
        {
            GUI.contentColor = Color.white;
            GUI.SetNextControlName("TextField");
            command = GUI.TextField(new Rect(20, windowHeight - 30, Screen.width - 300, 25), command);
            GUI.FocusControl("TextField");
        }

        #region Version Show
        GUI.contentColor = Color.green;
        string version = "Core version : " + GameSettings.Instance.CORE_VERSION + " - Build Version : " + GameSettings.Instance.BUILD_VERSION + " - Revision Build : " + GameSettings.Instance.CODE_VERSION_BUILD + " - Platform : " + PlatformSetting.GetPlatform.ToString() + " - Partner : " + GameSettings.Instance.ParnerCodeIdentifier;
        GUIContent versionContent = new GUIContent(version);
        Vector2 sizeOfVersion = GUI.skin.GetStyle("Label").CalcSize(versionContent);
        GUILayout.BeginArea(new Rect(20f, windowHeight - 60, sizeOfVersion.x, sizeOfVersion.y));
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(versionContent);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        #endregion

        GUI.contentColor = Color.white;
        if (GUI.Button(new Rect(Screen.width - 270, windowHeight - 30, 50, 25), "Enter"))
        {
            if (command != "")
            {
                ExecuteCommand(command);
                scrollPos = new Vector2(0, entries.Count * margin);
                command = "";
            }
        }

        if (GUI.Button(new Rect(Screen.width - 210, windowHeight - 30, 120, 25), UnityEngine.Time.timeScale != 0 ? "[PAUSE] ||" : "[RESUME] I>"))
        {
            if (UnityEngine.Time.timeScale != 0)
                UnityEngine.Time.timeScale = 0f;
            else
                UnityEngine.Time.timeScale = 1f;
        }
        #region OTHER BUTTON


        if (GUI.Button(new Rect(0f, 0f, 140f, 35f), "CLEAR"))
            entries.Clear();
        if (GUI.Button(new Rect(Screen.width / 2 - 140f / 2, 0f, 140f, 35f), "CLOSE"))
            showLogs = !showLogs;
        if (GUI.RepeatButton(new Rect(Screen.width - 170f, 50f, 140f, 80f), "Scroll Up"))
            scrollPos.y -= 30;
        if (GUI.RepeatButton(new Rect(Screen.width - 170f, 130f, 140f, 80f), "Scroll Down"))
            scrollPos.y += 30;
        if (GUI.RepeatButton(new Rect(Screen.width - 170f, 250f, 140f, 80f), "Page Up"))
            scrollPos.y -= 14 * margin;
        if (GUI.RepeatButton(new Rect(Screen.width - 170f, 330f, 140f, 80f), "Page Down"))
            scrollPos.y += 14 * margin;
        #endregion

        collapse = GUI.Toggle(new Rect(Screen.width - 80, windowHeight - 30, 100, 25), collapse, collapseLabel);

        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

    /// <summary>
    /// Logged messages are sent through this callback function.
    /// </summary>
    /// <param name="message">The message itself.</param>
    /// <param name="stackTrace">A trace of where the message came from.</param>
    /// <param name="type">The type of message: error/exception, warning, or assert.</param>
    void HandleLog(string message, string stackTrace, LogType type)
    {
        ConsoleMessage entry = new ConsoleMessage(message, stackTrace, type);
        entries.Add(entry);

        if (type == LogType.Error && !message.StartsWith("Coroutine couldn't be started because the the game object"))
        {
            string url = ServerWeb.URL_REQUEST_ERROR;
            WWWForm form = new WWWForm();
            form.AddField("app_id", GameManager.GAME.ToString());
            form.AddField("game_version", GameSettings.CurrentVersion);
            if (GameManager.CurrentScene != ESceneName.LoginScreen)
            {
                form.AddField("user_id", GameManager.Instance.mInfo.id);
                form.AddField("username", GameManager.Instance.mInfo.username);
            }
            form.AddField("scene", GameManager.CurrentScene.ToString());
            form.AddField("error", message);
            form.AddField("detail", stackTrace);
            form.AddField("environment", Common.GetDevice);
            form.AddField(ServerWeb.PARAM_PARTNER_ID, GameSettings.Instance.ParnerCodeIdentifier);
            if (!listSendError.Contains(form))
            {
                listSendError.Add(form);
                ServerWeb.StartThread(url, form, null);
            }
        }

        if (entries.Count > limitLine)
            entries.RemoveAt(0);
    }
    List<WWWForm> listSendError = new List<WWWForm>();

    void ExecuteCommand(string text)
    {
        Dictionary<string, string> command = ParseCommandText(text);

        switch (GetCommand(command))
        {
            case "cls":
                entries.Clear();
                break;
            case "check":
                int entryId = int.Parse(GetParam(command, "entry", 0).Trim().ToLower());
                Debug.Log(entries[entryId].stackTrace);
                break;
            case "watch":

                break;
            case "canViewHand":
                Common.CanViewHand = !Common.CanViewHand;
                break;
            case "canTestMode":
                Common.CanTestMode = !Common.CanTestMode;
                break;
            case "close":
                showLogs = false;
                break;
            //case "bot":
            //    int number = 0;
            //    if (int.TryParse(command["max"], out number))
            //        if (number > 0 && number <= 3)
            //            GameSettings.MAX_NUMBER_BOT = number;
            //    break;
            case "call":
                if (command.ContainsKey("url"))
                    ServerWeb.StartThread(command["url"].ToString(), null);
                break;
            case "help":
                Debug.Log(
                    "Danh sách các command hỗ trợ trong Test Mode \n\n" +
                    "\tclose           \t\t\tĐóng cửa sổ\n\n" +
                    "\tcls             \t\t\tClear danh sách\n\n" +
                    "\thelp            \t\t\tHiện thị các command\n\n" +
                    "\tdisableTouch    \t\tDùng chuột trên mobile\n\n" +
                    "\tcanViewHand     \tCho phép xem bài của đối thủ\n\n" +
                    "\tcanTestMode     \tCho phép chế độ Test Mode\n\n" +
                    "\tbot max=3       \t\tCho phép 3 bot khi chơi\n\n"
                );
                break;
        }
    }

    private static Dictionary<string, string> ParseCommandText(string text)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        text = text.Trim();
        int position = text.IndexOf(' ');
        if (position > 0)
        {
            result.Add(COMMAND_KEY, text.Substring(0, position).Trim().ToLower());
            text = text.Substring(position, text.Length - position);

            string[] parameters = text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string parameter in parameters)
            {
                string[] temp = parameter.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                result.Add(temp[0].Trim().ToLower(), temp[1]);
            }
        }
        else
        {
            result.Add(COMMAND_KEY, text);
        }
        Debug.Log("==========Console command==========");
        foreach (string key in result.Keys)
            Debug.Log(key + "=" + result[key]);
        Debug.Log("==========End of Console command==========");
        return result;
    }

    private static string GetCommand(Dictionary<string, string> parameters)
    {
        return parameters[COMMAND_KEY];
    }

    private static string GetParam(Dictionary<string, string> parameters, string name)
    {
        return GetParam(parameters, name, "");
    }

    private static string GetParam(Dictionary<string, string> parameters, string name, object defaultvalue)
    {
        return GetParam(parameters, name, defaultvalue.ToString());
    }

    private static string GetParam(Dictionary<string, string> parameters, string name, string defaultvalue)
    {
        return parameters.ContainsKey(name.Trim().ToLower()) ? parameters[name.Trim().ToLower()] : defaultvalue;
    }
}
