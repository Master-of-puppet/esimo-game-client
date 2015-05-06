using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Electrotank.Electroserver5.Api;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Chế độ test trong gameplay
/// </summary>
public class TestModeGUI : MonoBehaviour
{
    public Texture background;
    const int MAX_CARD_ON_HAND = 20;
    const int MAX_CARD_ON_NOC = 23;

    List<ChanCard> lstCard = new List<ChanCard>();

    string strPick = "";
    int lastPick;
    List<int> lastPicks = new List<int>();
    string[] playerPick = new string[4];

    float scaleWidth, scaleHeight;
    int SETNOC_BOCBAI_SETCAI = 1;
  
	void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnButtonClick;

        GameModelChan.game.Listener.EventNewGame += OnNewGame;
        GameManager.Server.EventPluginMessageOnProcess += Server_EventPluginMessageOnProcess;
    }

    void OnDestroy()
    {
        if (GameModelChan.game == null) return;

        GameModelChan.game.Listener.EventNewGame += OnNewGame;

        if (!GameManager.IsExist) return;

        GameManager.Server.EventPluginMessageOnProcess -= Server_EventPluginMessageOnProcess;
    }

    void OnNewGame()
    {
        playerPick = new string[4];
        strPick = "";
    }

    void OnButtonClick(GameObject obj)
    {
        gameObject.SetActive(false);
    }

    static TestModeGUI _instance;
    public static TestModeGUI Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/prefabTestMode"));
                obj.name = "__Prefab Test Pick Card";
                _instance = obj.GetComponent<TestModeGUI>();
            }
            return _instance;
        }
    }

    List<ECard> cardPlaying = new List<ECard>();
    void OnEnable()
    {
        strPick = "";

        cardPlaying.Clear();
        foreach (PlayerControllerChan p in GameModelChan.ListPlayer)
        {
            cardPlaying.AddRange(p.mCardTrash);
            cardPlaying.AddRange(p.mCardHand);
        }
    }

    void Start()
    {
        for (int i = 0; i < 25; i++)
            lstCard.Add(new ChanCard(0, i));
    }

    const float SPACE_TOP = 50f;
    const float SPACE_LEFT = 50f;
    void OnGUI()
    {
        scaleWidth = Screen.width / 960f;
        scaleHeight = Screen.height / 640f;
        float BOX_WIDTH = (Screen.width - (SPACE_LEFT * 2)) - scaleWidth;
        float BOX_HEIGHT = (Screen.height - (SPACE_TOP * 2)) - scaleHeight;
        int totalLine = 16;
        float BUTTON_HEIGHT = BOX_HEIGHT / totalLine;

        Rect rect = new Rect(SPACE_LEFT , SPACE_TOP, BOX_WIDTH, BOX_HEIGHT);
        GUI.DrawTexture(rect, background);
        GUI.Box(rect, "");
        {
            GUILayout.BeginArea(rect, "");
            {
                if (GameModelChan.CurrentState > GameModelChan.EGameState.deal)
                {
                    GUILayout.Label("Chọn bốc bài ", GUILayout.Height(BUTTON_HEIGHT));
                }
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginVertical();
                    {
                        if (GameModelChan.CurrentState < GameModelChan.EGameState.deal)
                        {
                            foreach (PlayerControllerChan p in GameModelChan.ListPlayer)
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    float contentWidth = BOX_WIDTH - 200f;
                                    GUILayout.Label(p.username, GUILayout.Width(100f));
                                    GUILayout.TextField(playerPick[p.slotServer] == null ? "" : playerPick[p.slotServer], GUILayout.Width(contentWidth - 150f - (BUTTON_HEIGHT * 2)));
                                    if (GUILayout.Button("CLEAR", new GUILayoutOption[] { GUILayout.Width(BUTTON_HEIGHT * 3) }))
                                        playerPick[p.slotServer] = "";
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    float contentWidth = BOX_WIDTH - 20f;
                    GUILayout.BeginVertical(GUILayout.Width(contentWidth / 4));
                    {
                        if (GameModelChan.CurrentState < GameModelChan.EGameState.deal)
                        {
                            GUILayout.Space(BUTTON_HEIGHT * 2);
                            if (GUILayout.Button("YÊU CẦU CHIA BÀI", new GUILayoutOption[] { GUILayout.Width(BUTTON_HEIGHT * 4), GUILayout.Height(BUTTON_HEIGHT) }))
                            {
                                SETNOC_BOCBAI_SETCAI = 1;
                                strPick = "";
                            }
                            if (GUILayout.Button("YÊU CẦU SÉT CÁI", new GUILayoutOption[] { GUILayout.Width(BUTTON_HEIGHT * 4), GUILayout.Height(BUTTON_HEIGHT) }))
                            {
                                SETNOC_BOCBAI_SETCAI = 2;
                                strPick = "";
                            }
                            if (GUILayout.Button("YÊU CẦU SÉT NỌC", new GUILayoutOption[] { GUILayout.Width(BUTTON_HEIGHT * 4), GUILayout.Height(BUTTON_HEIGHT) }))
                            {
                                SETNOC_BOCBAI_SETCAI = 3;
                                strPick = "";
                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(GUILayout.Width(contentWidth / 3));
                    {
                        string[] lst = strPick.Split(" ".ToCharArray(), StringSplitOptions.None);
                        GUILayout.Label("BẠN ĐANG CHỌN: " + (lst.Length - 1) + " Card.", GUILayout.Width(contentWidth / 3));
                        GUILayout.TextField(strPick);
                        if (GUILayout.Button("CLEAR", GUILayout.Height(BUTTON_HEIGHT)))
                            strPick = "";
                        if (GameModelChan.CurrentState < GameModelChan.EGameState.deal)
                        {
                            foreach (PlayerControllerChan p in GameModelChan.ListPlayer)
                            {
                                if (GUILayout.Button("SET TO " + p.username, GUILayout.Height(BUTTON_HEIGHT / 2)))
                                {
                                    playerPick[p.slotServer] = strPick;
                                    strPick = "";
                                    lastPicks.Clear();
                                }
                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(GUILayout.Width(contentWidth / 3));
                    {
                        GUILayout.Label(" ");

                        if (GUILayout.Button("ĐÃ XONG", new GUILayoutOption[] { GUILayout.Width(contentWidth / 3 / 2), GUILayout.Height(contentWidth / 3 / 2) }))
                        {
                            OnClickDone();
                            lastPicks.Clear();
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    string str = "";
                    string strName = "";
                    int index = 0;
                  
                    for (int i = 0; i < 9; i++)
                    {
                        GUILayout.BeginVertical();
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (index <= 24)
                                {
                                    str = lstCard[index].ConvertToString;
                                    strName = lstCard[index].ToString();
                                }

                                if (Array.FindAll<string>(strPick.Split(" ".ToCharArray(), StringSplitOptions.None), s => s == str).Length > 3)
                                {
                                    str = "";
                                    strName = "";
                                }
                                if (GameModelChan.CurrentState >= GameModelChan.EGameState.deal)
                                {
                                    if (checkDeal(index) == 4)
                                    {
                                        str = "";
                                        strName = "";
                                    }
                                }
                                
                                if (index <= 24)
                                {
                                        if(checkSame(str)<4)
                                        {
                                            if (GUILayout.Button(str == "" ? " " : str.Substring(0,str.Length-1) + "\n" + strName, new GUILayoutOption[] { GUILayout.Width(BUTTON_HEIGHT * 2), GUILayout.Height(BUTTON_HEIGHT) }))
                                              if (GameModelChan.CurrentState >= GameModelChan.EGameState.deal)
                                                {
                                                    if (strPick.Split(" ".ToCharArray(), StringSplitOptions.None).Length <= 1)
                                                    {
                                                        if (!lastPicks.Contains(index))
                                                            lastPicks.Add(index);
                                                        lastPick = index;
                                                        strPick += str + " ";
                                                        return;
                                                    }
                                                }
                                               
                                                else
                                                {
                                                    Debug.Log(SETNOC_BOCBAI_SETCAI);
                                                    if (string.IsNullOrEmpty(str) == false &&
                                                         (SETNOC_BOCBAI_SETCAI == 3 ?
                                                         GameManager.GAME == EGame.Chan ?
                                                          strPick.Split(" ".ToCharArray(), StringSplitOptions.None).Length <= MAX_CARD_ON_NOC
                                                         : strPick.Split(" ".ToCharArray(), StringSplitOptions.None).Length <= MAX_CARD_ON_HAND
                                                         : strPick.Split(" ".ToCharArray(), StringSplitOptions.None).Length <= MAX_CARD_ON_HAND)
                                                    ) 
                                                    {
                                                        if (!lastPicks.Contains(index))
                                                            lastPicks.Add(index);
                                                        lastPick = index;
                                                        strPick += str + " ";
                                                        return;
                                                    }
                                                }
                                        }
                                        else
                                        {
                                            GUILayout.Button("", new GUILayoutOption[] { GUILayout.Width(BUTTON_HEIGHT * 2), GUILayout.Height(BUTTON_HEIGHT) });
                                        }
                                }
                                index++;
                            }
                        }

                        GUILayout.EndVertical();

                    }
                    GUILayout.FlexibleSpace();
                }

                GUILayout.EndHorizontal();

            }
            GUILayout.EndArea();
        }
    }
    int checkDeal(int cardid)
    {
        int count = 0;
        foreach (ChanCard c in cardPlaying)
        {
            if (c.CardId == cardid)
            {
                count++;
            }
        }
        return count;
    }
    int checkSame(string str)
    {
        int count = 0;
        int countpick = 0;
        string[] chartext = null;
        string[] strText = null;
        for (int k = 0; k < playerPick.Length; k++)
        {
            if (playerPick[k] != null)
            {
                chartext = playerPick[k].Split("".ToCharArray(), StringSplitOptions.None);
                for (int x = 0; x < chartext.Length; x++)
                {
                    if (chartext[x].Equals(str))
                    {
                        count++;
                    }
                }
            }
        }
        if (strPick != null)
        {
            strText = strPick.Split("".ToCharArray(), StringSplitOptions.None);
            for (int i = 0; i < strText.Length; i++)
            {
                if (strText[i].Equals(str))
                {
                    countpick++;
                }
            }
        }
        return count + countpick;
    }

    void OnClickDone()
    {
        if (GameModelChan.CurrentState >= GameModelChan.EGameState.deal)
        {
            if (strPick.Trim() != "")
            {
                GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("orderNextCard", new object[] {
                        "cardId", lastPick
                    }));
            }
        }
        else
        {
            if (SETNOC_BOCBAI_SETCAI==1)
            {
                List<EsObject> lst = new List<EsObject>();
                GameModelChan.ListPlayer.ForEach(p =>
                {
                    EsObject obj = new EsObject();
                    obj.setString(Fields.GAMEPLAY.PLAYER, p.username);
                    obj.setString("cards", string.IsNullOrEmpty(playerPick[p.slotServer]) ? "" : playerPick[p.slotServer].Trim());
                    lst.Add(obj);
                });

                GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("orderHands", new object[] {
                "handsOrdered", lst.ToArray()
            }));
            }
            else if (SETNOC_BOCBAI_SETCAI == 2)
            {
                Debug.Log("orderFirstCard");
                GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("orderFirstCard", new object[] {
                        "cardId", lastPick
                    }));
                SETNOC_BOCBAI_SETCAI = 1;
            }
            else
            {
                Debug.Log("danh sach noc " + strPick);
                GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("orderNoc", new object[] {
                        "cards", strPick
                    }));
                SETNOC_BOCBAI_SETCAI = 1;
            }
        }
        OnButtonClick(null);
    }

    private void Server_EventPluginMessageOnProcess(string command, string action, EsObject eso)
    {
        if (command == "getHandRobot")
        {
            if (eso.variableExists("players"))
            {
                foreach (EsObject obj in eso.getEsObjectArray("players"))
                {
                    string username = obj.getString("userName");
                    PlayerControllerChan p = GameModelChan.GetPlayer(username);
                    if (obj.getIntegerArray("hand").Length != p.mCardHand.Count)
                        Debug.LogWarning("Số lượng card không phù hợp giũa client và server: " + p.username);
                    else
                    {
                        if (p.mCardHand.FindAll(c => c.CardId == -1).Count == 0) return;

                        foreach (int cardId in obj.getIntegerArray("hand"))
                        {
                            ECard card = p.mCardHand.Find(c => c.CardId == cardId);
                            if (card == null)
                                p.mCardHand.Find(c => c.CardId == -1).CardId = cardId;
                        }
                    }
                }
            }
        }
    }
}
