using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CUIPlayerPhom : MonoBehaviour,ICUIPlayer {
    #region Unity Editor
    public Transform avatar;
    public Transform timerCountDown;
    public CUIHandle btnPlus;
    #endregion

    PlayerControllerPhom player;
    GameObject imageReady;
    GameObject objLabelExchange;
    GameObject objEffect;
    GameObject objTinhDiem;
    float timeCountDown = 0;
    float oneSeconds = 1f;
    float oldRealTime = 0;

    void Start()
    {
        if (gameObject.GetComponentInChildren<CUIHandle>()!=null)
            CUIHandle.AddClick(gameObject.GetComponentInChildren<CUIHandle>(), OnClicViewDetails);

        CUIHandle.AddClick(btnPlus, OnClickButtonPlus);
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnPlus, OnClickButtonPlus);
    }
    void Update()
    {
        if (avatar == null || player == null) {
            NGUITools.SetActive(btnPlus.gameObject, true);
            return; 
        }

        if (GameModelPhom.CurrentState != GameModelPhom.EGameState.finalizing)
        {
            if (objEffect != null) GameObject.Destroy(objEffect);
            if (objTinhDiem != null) GameObject.Destroy(objTinhDiem);
            if (objLabelExchange != null) GameObject.Destroy(objLabelExchange);
        }

        if (player.slotServer == GameModelPhom.IndexInTurn && GameModelPhom.CurrentState == GameModelPhom.EGameState.playing
            && (player.PlayerState == PlayerControllerPhom.EPlayerState.inTurn || player.PlayerState == PlayerControllerPhom.EPlayerState.laying))
        {

            timeCountDown -= Time.realtimeSinceStartup - oldRealTime;
            oldRealTime = Time.realtimeSinceStartup;
            oneSeconds -= Time.deltaTime;

            UISlider process = timerCountDown.GetComponent<UISlider>();
            process.value = timeCountDown / GameModelPhom.game.TimeCountDown;
            if (process.value < 0.3f)
                timerCountDown.FindChild("Foreground").GetComponent<UISprite>().spriteName = "timeRed";
            //timerCountDown.FindChild("Foreground").transform.GetComponent<UISprite>().fillAmount = timeCountDown / GameModelPhom.game.TimeCountDown;

            if (oneSeconds <= 0)
            {
                if (GameModelPhom.YourController.slotServer == player.slotServer)
                {
                    if (process.value < 0.3f)
                        AudioManager.Instance.Play(AudioManager.SoundEffect.OneSecondsRushed);
                    //else if (process.sliderValue < 0.5f)
                    //    AudioManager.Instance.Play(AudioManager.SoundEffect.OneSeconds);
                }
                oneSeconds = 1f;
            }
        }
        else if (timerCountDown.gameObject.activeSelf)
            timerCountDown.gameObject.SetActive(false);

        if (imageReady != null && (int)GameModelPhom.CurrentState >= (int)GameModelPhom.EGameState.deal)
            GameObject.Destroy(imageReady);
    }

    //void OnGUI()
    //{
    //    if ((avatar == null || player == null) && Camera.main != null && player == null && ((ChannelPhom)GameManager.Instance.selectedChannel).type == ChannelPhom.ChannelType.Amateur)
    //    {
    //        if (!HeaderMenu.Instance.IsHidden) return;
    //        if (GameManager.Instance.ListPopup.Count > 0) return;
    //        if (GameModelPhom.CurrentState >= GameModelPhom.EGameState.deal) return;
    //        if (GameModelPhom.YourController == null || GameModelPhom.YourController.isMaster == false) return;
    //        if (GameModelPhom.NumberBot >= ((LobbyPhom)GameManager.Instance.selectedLobby).numberRobotAllowed) return;

    //        Vector3 thisRect = Camera.main.WorldToScreenPoint(gameObject.GetComponentInChildren<UISprite>().transform.position);
    //        if (GUI.Button(new Rect(thisRect.x - 80f / 2, Screen.height - thisRect.y - 80f / 2, 80f, 80f), "THÊM MÁY"))
    //        {
    //            GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("addRobot", new object[] {
    //                "slotId", slotIndex
    //            }));
    //        }
    //    }
    //}
    void OnClickButtonPlus(GameObject obj)
    {
         //GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("addRobot", new object[] {
         //           "slotId", slotIndex
         //       }));
        GPGameManagerPhom.Create(2);
    }


    void OnClicViewDetails(GameObject go)
    {
        if (player.slotServer == GameModelPhom.YourController.slotServer) return;
        ViewProfile.Create(player);
    }
    public static Dictionary<int, GameObject> listNoSlot = new Dictionary<int, GameObject>();
    int slotIndex = -1;
    public static CUIPlayerPhom CreateNoSlot(int slotIndex, Transform parentTransform)
    {
        if (listNoSlot.ContainsKey(slotIndex))
        {
            GameObject.Destroy(listNoSlot[slotIndex]);
            listNoSlot.Remove(slotIndex);
        }

        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/PlayerInfoPhomPrefab"));
        obj.GetComponentInChildren<UISprite>().MakePixelPerfect();
        obj.name = "Side Null " + slotIndex;
        obj.transform.parent = parentTransform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        CUIPlayerPhom p = obj.GetComponent<CUIPlayerPhom>();
        p.slotIndex = slotIndex;
        p.IsHasQuit();

        listNoSlot.Add(slotIndex, obj);
        return p;
    }
    public static CUIPlayerPhom Create(PlayerControllerPhom p, Transform parentTransform)
    {
        if (listNoSlot.ContainsKey(p.slotServer))
            GameObject.Destroy(listNoSlot[p.slotServer]);

        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/PlayerInfoPhomPrefab"));
        obj.name = "Player " + (int)p.mSide;
        obj.transform.parent = parentTransform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        CUIPlayerPhom cui = obj.GetComponent<CUIPlayerPhom>();

        cui.player = p;
        p.cuiPlayer = cui;

        p.AvatarTexture(delegate(Texture _texture) { if (cui != null) cui.avatar.GetComponentInChildren<UITexture>().mainTexture = _texture; });

        if (p.mSide == ESide.You)
        {
            //cui.avatar.GetComponentInChildren<UITexture>().enabled = false;
            NGUITools.SetActive(cui.avatar.gameObject, false);
            //GameObject.Destroy(cui.avatar.GetComponentInChildren<CUIHandle>().gameObject.collider);
            GameObject.Destroy(cui.GetComponentInChildren<UISprite>().gameObject);

            //cui.avatar.FindChild("lbMoney").GetComponent<UILabel>().pivot = UIWidget.Pivot.Left;
            //cui.avatar.FindChild("lbUsername").GetComponent<UILabel>().pivot = UIWidget.Pivot.Left;

            //cui.avatar.FindChild("lbMoney").transform.localPosition = new Vector3(-95f, -117f, -5f);
            //cui.avatar.FindChild("lbUsername").transform.localPosition = new Vector3(-95f, -97f, -5f);

            //cui.avatar.FindChild("lbUsername").transform.localScale = cui.avatar.FindChild("lbMoney").transform.localScale = new Vector3(19f, 19f, 1f);
            //cui.avatar.FindChild("lbUsername").GetComponent<UILabel>().width = cui.avatar.FindChild("lbMoney").GetComponent<UILabel>().width = 250;
            //cui.avatar.FindChild("lbMoney").GetComponent<UILabel>().color = Color.green;
            cui.gameObject.transform.FindChild("IconWarning").localPosition = new Vector3(35f, -46f, -5f);
            cui.gameObject.transform.FindChild("IconMaster").localPosition = new Vector3(125f, -138f, -5f);
        }
        cui.avatar.Find("lbUsername").GetComponent<UILabel>().text = p.mSide == ESide.You ? (string.IsNullOrEmpty(GameManager.Instance.mInfo.FullName) ? p.username : GameManager.Instance.mInfo.FullName.Trim()) : p.username; //(p.mSide == ESide.You ? "Username: " : "") + p.username;

        cui.UpdateInfo();

        return cui;
    }
    public void CheckIcon()
    {
        gameObject.transform.FindChild("IconMaster").gameObject.SetActive(player.isMaster);
    }

    public void IsComeback()
    {
        throw new System.NotImplementedException();
    }

    public void IsHasQuit()
    {
        GetComponentInChildren<UISprite>().spriteName = "bgAvatarPlayer";
        GetComponentInChildren<UISprite>().MakePixelPerfect();
        GameObject.Destroy(timerCountDown.gameObject);
        GameObject.Destroy(avatar.gameObject);
    }

    public void ShowEffect()
    {
        if (objEffect == null)
        {
            if (player.summary.result != PlayerControllerPhom.FinishGame.ResultSprite.None)
            {
                objEffect = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/IconEffectPrefab"));

                objEffect.GetComponent<UITexture>().mainTexture = Resources.Load("Images/Gameplay/ICON/" + player.summary.result.ToString()) as Texture;
                objEffect.GetComponent<UITexture>().depth = 20;
                objEffect.transform.parent = transform;

                objEffect.GetComponent<UITexture>().MakePixelPerfect();

                if (player.mSide != ESide.You)
                    objEffect.GetComponent<UITexture>().transform.localScale /= 2;

                objEffect.transform.localPosition = (player.mSide == ESide.You) ? new Vector3(370f, -30f, -30f) : new Vector3(0f, 0f, -15f);

                if (player.mSide == ESide.You)
                    objEffect.GetComponent<UITexture>().MakePixelPerfect();
            }
        }

        if (objLabelExchange == null)
        {
            if (GameModelPhom.MiniState == GameModelPhom.EGameStateMini.summary_exchange)
            {
                objLabelExchange = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/LableMoneyExchangePhomPrefab"));
                objLabelExchange.transform.parent = transform;


                objLabelExchange.GetComponent<EasyFontTextMesh>().FontSize = player.mSide == ESide.You ? 45 : 25;
                objLabelExchange.GetComponent<EasyFontTextMesh>().Size = player.mSide == ESide.You ? 45 : 25;

                objLabelExchange.transform.localScale = Vector3.one;
                objLabelExchange.transform.localPosition = (player.mSide == ESide.You) ? new Vector3(370f, -160f, -125f) : new Vector3(0f, -65f, -120f);

                objLabelExchange.GetComponent<EasyFontTextMesh>().Text = (player.summary.moneyExchange >= 0 ? "+" : "") + player.summary.moneyExchange.ToString();
                Debug.Log("objLabelExchange.text: " + objLabelExchange.GetComponent<EasyFontTextMesh>().Text.ToString());
                if (player.summary.moneyExchange < 0)
                {
                    objLabelExchange.GetComponent<EasyFontTextMesh>().FontColorTop = new Color(255f / 255f, 153f / 255f, 0f / 255f);
                    objLabelExchange.GetComponent<EasyFontTextMesh>().FontColorBottom = new Color(197 / 255f, 197 / 255f, 197 / 255f);
                }
            }
        }
    }

    public void ShowTinhDiem()
    {
        if (objTinhDiem != null) return;
        if (player.mCardMelds.Count == 0) return;
        if (player.mCardHand.Count == 0) return;

        Vector3 position = new Vector3(385f, -55, -20f);
        if (player.mSide == ESide.Enemy_1)
            position = new Vector3(-85f, -100f, -20f);
        else if (player.mSide == ESide.Enemy_2)
            position = new Vector3(200f, 0f, -20f);
        else if (player.mSide == ESide.Enemy_3)
            position = new Vector3(85f, -100f, -20f);

        objTinhDiem = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/tinhDiemPrefab"));
        objTinhDiem.transform.parent = transform;
        objTinhDiem.transform.localScale = Vector3.one;
        objTinhDiem.transform.localPosition = position;

        objTinhDiem.GetComponentInChildren<UILabel>().text = player.summary.sumRank + " Điểm";
    }

    public void StartRemainingTime(float remainingTime)
    {
        UpdateLocationTimerCountDown();
        timeCountDown = remainingTime;
        timerCountDown.gameObject.SetActive(true);
        timerCountDown.FindChild("Foreground").GetComponent<UISprite>().spriteName = "timeYellow";
        oneSeconds = 1f;
        oldRealTime = Time.realtimeSinceStartup;
    }

    public void StartTime(float total)
    {
        if (total == 0)
        {
            timerCountDown.gameObject.SetActive(false);
            return;
        }
        UpdateLocationTimerCountDown();
        timeCountDown = total;
        timerCountDown.gameObject.SetActive(true);
        timerCountDown.FindChild("Foreground").GetComponent<UISprite>().spriteName = "timeYellow";
        oneSeconds = 1f;
        oldRealTime = Time.realtimeSinceStartup;
    }

    public void UpdateInfo()
    {
        PlayerControllerPhom your = GameModelPhom.YourController;

        if (avatar != null)
        {
            avatar.Find("lbMoney").GetComponent<UILabel>().text = Utility.Convert.Chip(player.chip) + (player.mSide == ESide.You ? " Chip" : "");
            NGUITools.SetActive(btnPlus.gameObject, false);
        }

        if (timerCountDown != null)
            UpdateLocationTimerCountDown();

        if ((int)GameModelPhom.CurrentState <= (int)GameModelPhom.EGameState.waitingForReady)
        {
            if (player.PlayerState == PlayerControllerPhom.EPlayerState.ready && player != your)
            {
                if (imageReady == null)
                {
                    imageReady = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/ReadyAvatarPrefab"));
                    imageReady.transform.parent = avatar;
                    imageReady.transform.localPosition = (player.mSide == ESide.You) ? new Vector3(376f, 0f, 0f) : Vector3.zero;
                    imageReady.transform.localScale = Vector3.one;
                }
            }
        }

        if (GameModelPhom.CurrentState == GameModelPhom.EGameState.finalizing)
        {
            if (GameModelPhom.MiniState == GameModelPhom.EGameStateMini.summary_point)
                ShowTinhDiem();
            else if (GameModelPhom.MiniState == GameModelPhom.EGameStateMini.summary_result || GameModelPhom.MiniState == GameModelPhom.EGameStateMini.summary_exchange)
            {
                if (objTinhDiem != null)
                    GameObject.Destroy(objTinhDiem);
                ShowEffect();
            }
            else
            {
                if (objEffect != null)
                    GameObject.Destroy(objEffect);
                if (objLabelExchange != null)
                    GameObject.Destroy(objLabelExchange);
            }
        }
        CheckIcon();
    }
    public void UpdateLocationTimerCountDown()
    {
        if (player.mSide == ESide.You)
        {
            int totalCard = player.mCardHand.Count;
            float x = -75f;
            float z = 0f;
            if (totalCard < 9 && totalCard > 0)
            {
                x += 90f * ((9 - totalCard) / 2);
                if (totalCard == 1)
                    x -= 90f / 2;

                z -= 9 - totalCard;
            }
            timerCountDown.localPosition = new Vector3(x, 0f, z);
        }
        else
            timerCountDown.localPosition = player.mSide == ESide.Enemy_1 ? new Vector3(60f, 0f, 0f) : new Vector3(-75f, 0f, 0f);
    }
}
