using System;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;
using UnityEngine;

public class TestModeTLMN : MonoBehaviour
{
    public Texture background;
    const int MAX_CARD_ON_HAND = 13;

    List<TLMNCard> lstCard = new List<TLMNCard>();

    string strPick = "";
    int lastPick;
    List<int> lastPicks = new List<int>();
    string[] playerPick = new string[4];

    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnButtonClick;

        GameModelTLMN.game.Listener.EventNewGame += OnNewGame;
        GameManager.Server.EventPluginMessageOnProcess += Server_EventPluginMessageOnProcess;
    }

    void OnDestroy()
    {
        if (GameModelTLMN.game == null) return;

        GameModelTLMN.game.Listener.EventNewGame += OnNewGame;

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

    static TestModeTLMN _instance;
    public static TestModeTLMN Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/prefabTestModeTLMN"));
                obj.name = "__Prefab Test Pick TLMNCard";
                _instance = obj.GetComponent<TestModeTLMN>();
            }
            return _instance;
        }
    }

    List<ECard> cardPlaying = new List<ECard>();
    void OnEnable()
    {
        strPick = "";

        cardPlaying.Clear();
        foreach (PlayerControllerTLMN p in GameModelTLMN.ListPlayer)
        {
            cardPlaying.AddRange(p.mCardTrash);
            cardPlaying.AddRange(p.mCardHand);
        }
    }

    void Start()
    {
        for (int i = 0; i < 52; i++)
            lstCard.Add(new TLMNCard(0, i));
    }

    const float SPACE_TOP = 50f;
    const float SPACE_LEFT = 50f;
    void OnGUI()
    {
        float BOX_WIDTH = Screen.width - SPACE_LEFT * 2;
        float BOX_HEIGHT = Screen.height - SPACE_TOP * 2;

        float BUTTON_HEIGHT = (BOX_WIDTH - 200f) / 24f;

        Rect rect = new Rect(SPACE_LEFT, SPACE_TOP, BOX_WIDTH, BOX_HEIGHT);
        GUI.DrawTexture(rect, background);
        GUI.Box(rect, "");
        {
            GUILayout.BeginArea(rect, "");
            {
                #region LINE 1
                GUILayout.BeginHorizontal(GUILayout.Height(BOX_HEIGHT / 3));
                {
                    GUILayout.FlexibleSpace();
                    foreach (PlayerControllerTLMN p in GameModelTLMN.ListPlayer)
                    {
                        GUILayout.BeginVertical();
                        {
                            GUILayout.BeginHorizontal();
                            {
                                float contentWidth = BOX_WIDTH - 100f;
                                GUILayout.Space(30f);
                                GUILayout.Label(p.username, GUILayout.Width(100f));
                                GUILayout.TextField(playerPick[p.slotServer] == null ? "" : playerPick[p.slotServer], GUILayout.Width(contentWidth - 150f - (BUTTON_HEIGHT * 2)));

                                if (GUILayout.Button("CLEAR", new GUILayoutOption[] { GUILayout.Width(BUTTON_HEIGHT * 2) }))
                                    playerPick[p.slotServer] = "";
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.BeginVertical();
                    }
                }
                GUILayout.EndHorizontal();
                #endregion

                GUILayout.FlexibleSpace();

                #region LINE 2
                GUILayout.BeginHorizontal(GUILayout.Height(BOX_HEIGHT / 3));
                {
                    float contentWidth = BOX_WIDTH - 20f;
                    GUILayout.BeginVertical(GUILayout.Width(contentWidth / 4));
                    {
                        #region
                        if (GameModelTLMN.CurrentState > GameModelTLMN.EGameState.deal && GameManager.Instance.mInfo.role >= User.ERole.Tester)
                        {
                            //GUILayout.FlexibleSpace();
                            //GUILayout.BeginHorizontal(GUILayout.Width(BOX_WIDTH - 200f));
                            GUILayout.Space(BUTTON_HEIGHT * 2);
                            if (GUILayout.Button("YÊU CẦU ĐÁNH BÀI", new GUILayoutOption[] { GUILayout.Width(BUTTON_HEIGHT * 4), GUILayout.Height(BUTTON_HEIGHT) }))
                            {
                                OnClickRequestHandRobot();
                            }
                            if (GUILayout.Button("YÊU CẦU BỐC BÀI", new GUILayoutOption[] { GUILayout.Width(BUTTON_HEIGHT * 4), GUILayout.Height(BUTTON_HEIGHT) }))
                            {
                                IS_TYPE_FORCE_ROBOT = false;
                            }
                            //GUILayout.EndHorizontal();
                        }
                        else
                            GUILayout.Label(" ", GUILayout.Height(BUTTON_HEIGHT));
                        #endregion
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical(GUILayout.Width(contentWidth / 3));
                    {
                        string[] lst = strPick.Split(" ".ToCharArray(), StringSplitOptions.None);
                        GUILayout.Label("BẠN ĐANG CHỌN: " + (lst.Length - 1) + " TLMNCard.", GUILayout.Width(contentWidth / 3));
                        GUILayout.TextField(strPick);
                        if (GUILayout.Button("CLEAR", GUILayout.Height(BUTTON_HEIGHT)))
                            strPick = "";

                        foreach (PlayerControllerTLMN p in GameModelTLMN.ListPlayer)
                        {
                            if (GUILayout.Button("SET TO " + p.username, GUILayout.Height(BUTTON_HEIGHT)))
                            {
                                playerPick[p.slotServer] = strPick;
                                strPick = "";
                                lastPicks.Clear();
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
                #endregion

                GUILayout.FlexibleSpace();

                #region LINE 3
                GUILayout.BeginHorizontal(GUILayout.Height(BOX_HEIGHT / 3));
                {
                    int index = 0;
                    for (int i = 0; i < 13; i++)
                    {
                        GUILayout.BeginVertical();
                        for (int j = 0; j < 4; j++)
                        {
                            string str = GameModelTLMN.game.cardController.Deck.PeekCard(index).ToString();

                            if (strPick.IndexOf(str) >= 0)
                                str = "";

                            if (IS_TYPE_FORCE_ROBOT == false)
                            {
                                if (cardPlaying.Find(c => c.CardId == index) != null)
                                    str = "";
                            }
                            else
                            {
                                if (GameModelTLMN.CurrentState >= GameModelTLMN.EGameState.deal && GameModelTLMN.GetNextPlayer(GameModelTLMN.IndexInTurn).mCardHand.Find(c => c.CardId == index) == null)
                                    str = "";
                            }

                            if (Array.TrueForAll<string>(playerPick, p => p == null || (p != null && p.IndexOf(str) == -1)))
                            {
                                if (GUILayout.Button(str, new GUILayoutOption[] { GUILayout.Width(BUTTON_HEIGHT * 2), GUILayout.Height(BUTTON_HEIGHT) }))
                                    if (string.IsNullOrEmpty(str) == false &&

                                        ((GameModelTLMN.CurrentState >= GameModelTLMN.EGameState.deal)
                                        ?
                                        GameManager.GAME == EGame.Phom ?
                                        strPick.Split(" ".ToCharArray(), StringSplitOptions.None).Length <= 1
                                        :
                                        strPick.Split(" ".ToCharArray(), StringSplitOptions.None).Length <= MAX_CARD_ON_HAND
                                        :
                                        strPick.Split(" ".ToCharArray(), StringSplitOptions.None).Length <= MAX_CARD_ON_HAND)
                                    )
                                    {
                                        if (!lastPicks.Contains(index))
                                            lastPicks.Add(index);

                                        lastPick = index;
                                        strPick += str + " ";
                                    }
                            }
                            else
                                GUILayout.Button("", new GUILayoutOption[] { GUILayout.Width(BUTTON_HEIGHT * 2), GUILayout.Height(BUTTON_HEIGHT) });
                            index++;
                        }
                        GUILayout.EndVertical();
                    }
                }
                GUILayout.EndHorizontal();
                #endregion
            }
            GUILayout.EndArea();
        }
    }

    bool IS_TYPE_FORCE_ROBOT = false;


    void OnClickDone()
    {
        if (GameModelTLMN.CurrentState >= GameModelTLMN.EGameState.deal)
        {
            if (strPick.Trim() != "")
            {
                if (IS_TYPE_FORCE_ROBOT && lastPicks.Count > 0 && GameModelTLMN.CurrentState > GameModelTLMN.EGameState.deal)
                {
                    if (GameManager.GAME == EGame.TLMN)
                    {
                        GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("forceRobotDiscard", new object[] {
                            "cards", lastPicks.ToArray(),
                            "player", GameModelTLMN.GetNextPlayer(GameModelTLMN.IndexInTurn).username
                        }));
                    }
                    else
                    {
                        GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("forceRobotDiscard", new object[] {
                            "cardId", lastPick,
                            "player", GameModelTLMN.GetNextPlayer(GameModelTLMN.IndexInTurn).username
                        }));
                    }
                }
                else
                {
                    GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("orderNextCard", new object[] {
                        "cardId", lastPick
                    }));
                }
            }
        }
        else
        {
            List<EsObject> lst = new List<EsObject>();
            GameModelTLMN.ListPlayer.ForEach(p =>
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
        OnButtonClick(null);
    }

    void OnClickRequestHandRobot()
    {
        GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("getHandRobot"));
    }

    private void Server_EventPluginMessageOnProcess(string command, string action, EsObject eso)
    {
        if (command == "getHandRobot")
        {
            IS_TYPE_FORCE_ROBOT = true;

            if (eso.variableExists("players"))
            {
                foreach (EsObject obj in eso.getEsObjectArray("players"))
                {
                    string username = obj.getString("userName");
                    PlayerControllerTLMN p = GameModelTLMN.GetPlayer(username);
                    if (obj.getIntegerArray("hand").Length != p.mCardHand.Count)
                        Debug.LogWarning("Số lượng TLMNCard không phù hợp giũa client và server: " + p.username);
                    else
                    {
                        if (p.mCardHand.FindAll(c => c.CardId == -1).Count == 0) return;

                        foreach (int cardId in obj.getIntegerArray("hand"))
                        {
                            ECard TLMNCard = p.mCardHand.Find(c => c.CardId == cardId);
                            if (TLMNCard == null)
                                p.mCardHand.Find(c => c.CardId == -1).CardId = cardId;
                        }
                    }
                }
            }
        }
    }
}
