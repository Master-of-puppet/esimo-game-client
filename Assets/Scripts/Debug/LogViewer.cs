using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;

/// <summary>
/// VIETDUNG
/// A console that displays the contents of Unity's debug log.
/// </summary>
public class LogViewer : MonoBehaviour
{
    private const string COMMAND_KEY = "$COMMAND$";
    private const string COMMAND_RETURN = "$RETURN$";

    #region Unity Editor
    public Texture background;
    public KeyCode toggleKey = KeyCode.BackQuote;
    public int LIMIT_LINE = 150;
    public int LIMIT_LINE_CONSOLE = 100;
    public int FONT_SIZE = 15;
    #endregion

    const int NUMBER_LINE = 16;


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
    static List<ConsoleMessage> entriesConsole = new List<ConsoleMessage>();
    bool isShowLogs, isCollapse, isBlockUI;
    bool isEnableLog = true, isEnableWarning = true, isEnableError = true, isEnableException = true;
    string command = "";
    Vector2 scrollPos;
    float margin = (Screen.height * 9 / 10) / NUMBER_LINE;
    Rect windowRect = new Rect();
    float windowHeight = 0;
    bool isEnableScrollButton, isEnableAdvanceControl, isFilterContent = true;

    public bool showLogs
    {
        get { return isShowLogs; }
        set
        {
            isShowLogs = value;
            if (value) BeginDraw(); else EndDraw();
            BlockUI(value);
        }
    }

    void Awake()
    {
        GameObject.DontDestroyOnLoad(gameObject);
        Application.RegisterLogCallback(HandleLog);
    }


    void OnDestroy()
    {
        Application.RegisterLogCallback(null);
    }

    void BeginDraw()
    {
        if (margin != (Screen.height * 9 / 10) / NUMBER_LINE)
            margin = (Screen.height * 9 / 10) / NUMBER_LINE;
    }

    void EndDraw()
    {
        windowHeight = 0;
        command = "";
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

    void CheckEnableLog()
    {
        float width = Screen.width / 4f, height = Screen.height / 4f;
        Rect topLeft = new Rect(1f, 0f, width, height);
        Rect topRight = new Rect(Screen.width - width, 0f, width, height);

        Vector2 detectDevice = Vector2.zero;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            detectDevice = new Vector2(Input.GetTouch(0).position.x, Screen.height - Input.GetTouch(0).position.y);
        else if (Input.GetButtonDown("Fire1"))
            detectDevice = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

        if (detectDevice != Vector2.zero)
        {
            if (CountTouch == 0 && topLeft.Contains(detectDevice))
                CountTouch++;
            else if (CountTouch > 0 && TimeCountDown > 0 && topRight.Contains(detectDevice))
            {
                CountTouch++;

                if (CountTouch >= 4)
                {
                    showLogs = true;
                    CountTouch = 0;
                }
            }
        }

        if (TimeCountDown > 0)
            TimeCountDown -= Time.deltaTime;
    }
    #endregion

    void Update()
    {

        if (Input.GetKeyDown(toggleKey) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
        {
            showLogs = !showLogs;
        }
        if (showLogs && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            scrollPos = new Vector2(0, entries.Count * margin);
            if (command != "")
                ExecuteCommand(command);
        }

        if (!showLogs) CheckEnableLog();
    }

    void OnGUI()
    {

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

        if (!showLogs) return;

        windowRect = new Rect(0, 0, Screen.width, Screen.height);
        windowRect = GUILayout.Window(99, windowRect, ConsoleWindow, "DEBUG VIEWER LOG");
        if (background != null)
            GUI.DrawTexture(windowRect, background);
        //GUI.FocusWindow(99);
    }

    /// <summary>
    /// A window displaying the logged messages.
    /// </summary>
    /// <param name="windowID">The window's ID.</param>
    void ConsoleWindow(int windowID)
    {
        GUI.skin.textField.fontSize = GUI.skin.button.fontSize = GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.toggle.fontSize = GUI.skin.textArea.fontSize = FONT_SIZE;
        float scrollWidth = (Screen.orientation == ScreenOrientation.Portrait) ? Screen.width / 16 : Screen.height / 32;

        GUI.skin.verticalScrollbar.fixedWidth = GUI.skin.verticalScrollbarThumb.fixedWidth = UnityEngine.Mathf.Clamp(scrollWidth, 30, 100);

        GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height - 40));
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, new GUILayoutOption[] { GUILayout.MaxHeight(Screen.height - 60) });
            {
                // Go through each logged entry
                for (int i = 0; i < entries.Count; i++)
                {
                    ConsoleMessage entry = entries[i];

                    // If this message is the same as the last one and the collapse feature is chosen, skip it
                    if (isCollapse && i > 0 && entry.message == entries[i - 1].message)
                        continue;

                    if (!isEnableLog && entry.type == LogType.Log)
                        continue;
                    if (!isEnableWarning && entry.type == LogType.Warning)
                        continue;
                    if (!isEnableError && entry.type == LogType.Error)
                        continue;
                    if (!isEnableException && entry.type == LogType.Exception)
                        continue;
                    if (isEnableAdvanceControl && isFilterContent && !string.IsNullOrEmpty(command) && !entry.message.Contains(command))
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
                    {
                        TextAnchor oldAnchor = GUI.skin.box.alignment;
                        GUI.skin.box.alignment = TextAnchor.MiddleLeft;
                        GUI.skin.box.wordWrap = true;
                        GUI.skin.box.richText = true;
                        GUILayout.Box(entry.stackTrace, GUILayout.Width(Screen.width - 50f - scrollWidth), GUILayout.ExpandHeight(true));
                        GUI.skin.box.alignment = oldAnchor;
                        //GUILayout.TextArea(entry.stackTrace);
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndScrollView();
        }
        GUI.EndGroup();

        GUI.contentColor = Color.white;
        GUILayout.Box("Filter Console Logger", GUILayout.ExpandWidth(true));
        Rect lastRectContent = GUILayoutUtility.GetLastRect();

        GUILayout.BeginHorizontal();
        {
            isEnableLog = GUILayout.Toggle(isEnableLog, "Log");

            isEnableWarning = GUILayout.Toggle(isEnableWarning, "Warning");

            isEnableError = GUILayout.Toggle(isEnableError, "Error");

            isEnableException = GUILayout.Toggle(isEnableException, "Exception");

            isCollapse = GUILayout.Toggle(isCollapse, "Collapse");

            bool boolValue = isBlockUI;
            isBlockUI = GUILayout.Toggle(isBlockUI, "Block UI");
            if (boolValue != isBlockUI)
                BlockUI(isBlockUI);
        }
        GUILayout.EndHorizontal();

        GUILayout.Box("Control Panel", GUILayout.ExpandWidth(true));

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Clear"))
                entries.Clear();

            if (GUILayout.Button(UnityEngine.Time.timeScale != 0 ? "[Pause] ||" : "[Resume] I>"))
                if (UnityEngine.Time.timeScale != 0) UnityEngine.Time.timeScale = 0f; else UnityEngine.Time.timeScale = 1f;

            if (GUILayout.Button(isEnableAdvanceControl ? "Hide Advance" : "Show Advance"))
                isEnableAdvanceControl = !isEnableAdvanceControl;

            if (GUILayout.Button(isEnableScrollButton ? "Hide Scroll" : "Show Scroll"))
                isEnableScrollButton = !isEnableScrollButton;

            if (GUILayout.Button("Close"))
                showLogs = !showLogs;
        }
        GUILayout.EndHorizontal();

        if (isEnableAdvanceControl)
        {
            GUILayout.BeginHorizontal();
            {
                //GUI.SetNextControlName("TextField");
                command = GUILayout.TextField(command, GUILayout.ExpandWidth(false), GUILayout.MinWidth(Screen.width * 1 / 2));
                //GUI.FocusControl("TextField");
                if (GUILayout.Button(isFilterContent ? "Search Log" : "Send Command", GUILayout.ExpandWidth(false)))
                {
                    if (!isFilterContent)
                    {
                        ExecuteCommand(command);
                        scrollPos = new Vector2(0, entries.Count * margin);
                        command = "";
                    }
                }

                isFilterContent = GUILayout.Toggle(isFilterContent, "Filter Log", GUILayout.ExpandWidth(false));
            }
            GUILayout.EndHorizontal();
        }

        if (isEnableScrollButton)
        {
            float guiHeight = lastRectContent.y / 8f;
            Rect button = new Rect(Screen.width - 170f, guiHeight, 120f, guiHeight);
            if (GUI.RepeatButton(button, "Scroll Up"))
                scrollPos.y -= margin;
            button.y += guiHeight * 1.1f;
            if (GUI.RepeatButton(button, "Scroll Down"))
                scrollPos.y += margin;
            button.y += guiHeight * 2f;
            if (GUI.RepeatButton(button, "Page Up"))
                scrollPos.y -= LIMIT_LINE * margin;
            button.y += guiHeight * 1.1f;
            if (GUI.RepeatButton(button, "Page Down"))
                scrollPos.y += LIMIT_LINE * margin;
        }

        GUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();

            GUI.contentColor = Color.green;
            string version = "Core version : " + GameSettings.Instance.CORE_VERSION + " - Build Version : " + GameSettings.Instance.BUILD_VERSION + " - Revision Build : " + GameSettings.Instance.CODE_VERSION_BUILD + " - Platform : " + PlatformSetting.GetPlatform.ToString() + " - Partner : " + GameSettings.Instance.ParnerCodeIdentifier;
            GUIContent versionContent = new GUIContent(version);
            GUILayout.Label(versionContent, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
            GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();

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
        if (entries.Count > LIMIT_LINE)
            entries.RemoveAt(0);

        if (GameManager.CurrentScene == ESceneName.GameplayChan && type == LogType.Exception)
        {
            WaitingView.Show("Có l?i x?y ra. Vui lòng d?i!");
            GameManager.Server.DoRequestGameCommand("refreshGame");
        }


        entriesConsole.Add(entry);
        if (entriesConsole.Count > LIMIT_LINE_CONSOLE)
            entriesConsole.RemoveAt(0);

        if (type == LogType.Error && !message.StartsWith("Coroutine couldn't be started because the the game object"))
        {
            StoreGame.ClearLog();
            for (int i = 0; i < entries.Count; i++)
            {
                StoreGame.SaveLog(StoreGame.EType.DEBUG_LOG, entries[i].message);
            }
            //SaveLogToFile(); //for test
            string url = ServerWeb.URL_REQUEST_ERROR;
            string data = ConvertStringToJson(StoreGame.LoadString(StoreGame.EType.DEBUG_LOG));
            WWWForm form = new WWWForm();
            form.AddField("app_id", GameManager.GAME.ToString());
            form.AddField("game_version", GameSettings.CurrentVersion);
            if (GameManager.CurrentScene != ESceneName.LoginScreen)
            {
                form.AddField("user_id", GameManager.Instance.mInfo.id);
                form.AddField("username", GameManager.Instance.mInfo.username);
            }
            form.AddField("scene", GameManager.CurrentScene.ToString());
            form.AddField("error", "");
            form.AddField("detail", "");
            form.AddField("environment", Common.GetDevice);
            form.AddField("debug_log", data);
            form.AddField(ServerWeb.PARAM_PARTNER_ID, GameSettings.Instance.ParnerCodeIdentifier);
            if (!listSendError.Contains(form))
            {
                listSendError.Add(form);
                ServerWeb.StartThread(url, form, null);
            }
        }
    }
    List<WWWForm> listSendError = new List<WWWForm>();
    public static void SendLogToServer()
    {
        StoreGame.ClearLog();
        for (int i = 0; i < entriesConsole.Count; i++)
        {
            StoreGame.SaveLog(StoreGame.EType.DEBUG_LOG, entriesConsole[i].message);
        }
        string url = ServerWeb.URL_REQUEST_ERROR;
        string data = ConvertStringToJson(StoreGame.LoadString(StoreGame.EType.DEBUG_LOG));
        WWWForm form = new WWWForm();
        form.AddField("app_id", GameManager.GAME.ToString());
        form.AddField("game_version", GameSettings.CurrentVersion);
        if (GameManager.CurrentScene != ESceneName.LoginScreen)
        {
            form.AddField("user_id", GameManager.Instance.mInfo.id);
            form.AddField("username", GameManager.Instance.mInfo.username);
        }
        form.AddField("scene", GameManager.CurrentScene.ToString());
        form.AddField("error", "");
        form.AddField("detail", "");
        form.AddField("environment", Common.GetDevice);
        form.AddField("debug_log", data);
        form.AddField(ServerWeb.PARAM_PARTNER_ID, GameSettings.Instance.ParnerCodeIdentifier);
        ServerWeb.StartThread(url, form, null);
    }
    private static string ConvertStringToJson(string str)
    {
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes(str);
        string strBase64 = System.Convert.ToBase64String(byteData);
        System.Collections.Hashtable hash = new System.Collections.Hashtable();
        hash.Add("debug_log", strBase64);
        return JSON.JsonEncode(hash);
    }
    #region For test
    public static void SaveLogToFile()
    {
        StoreGame.ClearLog();
        for (int i = 0; i < entriesConsole.Count; i++)
        {
            StoreGame.SaveLog(StoreGame.EType.DEBUG_LOG, entriesConsole[i].message);
        }
        if (StoreGame.Contains(StoreGame.EType.DEBUG_LOG))
        {
            SaveAndLoadFile.Save(StoreGame.LoadString(StoreGame.EType.DEBUG_LOG));
        }
    }
    #endregion

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
                break;
            case "watch":
                break;
            case "close":
                showLogs = false;
                break;
            case "font":
                if (command.ContainsKey("size"))
                {
                    int size = FONT_SIZE;
                    if (int.TryParse(command["size"].ToString(), out size)) FONT_SIZE = size;
                }
                break;
            case "setting":
                if (command.ContainsKey("limit"))
                {
                    int limit = LIMIT_LINE;
                    if (int.TryParse(command["limit"].ToString(), out limit)) LIMIT_LINE = limit;
                }
                break;
            case "help":
                Debug.Log(
                    "List commands enable with LogViewer \n\n" +
                    "\tclose                \t\t\t\tClose windows\n\n" +
                    "\tcls                  \t\t\t\tClear log\n\n" +
                    "\thelp                 \t\t\t\tShow help command\n\n" +
                    "\tfont size=15         \t\tChange font size\n\n" +
                    "\tsetting limit=100    \t\tLimit line of log viewer\n\n"
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
        //Debug.Log("==========Console command==========");
        //foreach (string key in result.Keys)
        //    Debug.Log(key + "=" + result[key]);
        //Debug.Log("==========End of Console command==========");
        return result;
    }

    string GetCommand(Dictionary<string, string> parameters)
    {
        return parameters[COMMAND_KEY];
    }

    string GetParam(Dictionary<string, string> parameters, string name)
    {
        return GetParam(parameters, name, "");
    }

    string GetParam(Dictionary<string, string> parameters, string name, object defaultvalue)
    {
        return GetParam(parameters, name, defaultvalue.ToString());
    }

    string GetParam(Dictionary<string, string> parameters, string name, string defaultvalue)
    {
        return parameters.ContainsKey(name.Trim().ToLower()) ? parameters[name.Trim().ToLower()] : defaultvalue;
    }

    List<Collider> colliderInScene = new List<Collider>();
    void BlockUI(bool isBlock)
    {
        if (isBlock)
        {
            System.Array.ForEach<Collider>(GameObject.FindObjectsOfType<Collider>(), c => { if (c != null) { c.enabled = false; colliderInScene.Add(c); } });
        }
        else
        {
            colliderInScene.ForEach(c => { if (c != null) c.enabled = true; });
            colliderInScene.Clear();
        }
        this.isBlockUI = isBlock;
    }

}
