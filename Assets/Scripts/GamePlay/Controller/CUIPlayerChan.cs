using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CUIPlayerChan : MonoBehaviour,ICUIPlayer {

    #region Unity Editor
    public Transform avatar;
    public Transform timerCountDown;
    public UILabel labelUserName, labelMoney;
    public CUIHandle btnPlus;
    #endregion

    static Vector3 scaleAvatar = new Vector3(0.887f, 0.887f, 1f);
    PlayerControllerChan player;
    GameObject imageReady;
    float oneSeconds = 1f;
    float oldRealTime = 0;
    float timeCountDown = 0;
    GameObject objLabelExchange;
    GameObject objEffect;
    GameObject objTinhDiem; 
    public static Dictionary<int, GameObject> listNoSlot = new Dictionary<int, GameObject>();
    int slotIndex = -1;

	// Use this for initialization
	void Start () {
        CUIHandle.AddClick(gameObject.GetComponentInChildren<CUIHandle>(), OnClicViewDetails);
        CUIHandle.AddClick(btnPlus, OnClickBtnPlus);
	}
    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnPlus, OnClickBtnPlus);
    }
    public static CUIPlayerChan CreateNoSlot(int slotIndex, Transform parentTransform)
    {
        if (listNoSlot.ContainsKey(slotIndex))
        {
            GameObject.Destroy(listNoSlot[slotIndex]);
            listNoSlot.Remove(slotIndex);
        }

        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/PlayerInfoPrefab"));
        obj.GetComponentInChildren<UISprite>().MakePixelPerfect();
        obj.name = "Side Null " + slotIndex;
        obj.transform.parent = parentTransform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = scaleAvatar;

        CUIPlayerChan p = obj.GetComponent<CUIPlayerChan>();
        p.slotIndex = slotIndex;
        if (GameModelChan.YourController != null && GameModelChan.YourController.isMaster)
            p.btnPlus.gameObject.SetActive(true);
        p.IsHasQuit();
        listNoSlot.Add(slotIndex, obj);
        return p;
    }
    public void IsHasQuitWithoutRemove()
    {
        if (ListCuocUView.IsShowing == false && ListResultXuongView.IsShowing == false)
        {
            if (GetComponentInChildren<UISprite>() != null)
            {
                GetComponentInChildren<UISprite>().spriteName = "bgAvatarPlayer";
                GetComponentInChildren<UISprite>().MakePixelPerfect();
            }

            if (avatar != null)
            {
                Transform mainTexture = avatar.FindChild("MainTexture");
                if (mainTexture != null)
                    mainTexture.GetComponent<UITexture>().alpha = 90f / 255f;

            }
        }
    }
    private void OnClickBtnPlus(GameObject targetObject)
    {
        GPGameManagerChan.Create(2);
    }
    void OnClicViewDetails(GameObject go)
    {
        if (GameModelChan.YourController != null && player.slotServer == GameModelChan.YourController.slotServer) return;

        ViewProfile.Create(player);
    }


	// Update is called once per frame
	void Update () {
        if (avatar == null || player == null) return;
        if (GameModelChan.CurrentState != GameModelChan.EGameState.finalizing)
        {
            if (objEffect != null) GameObject.Destroy(objEffect);
            if (objTinhDiem != null) GameObject.Destroy(objTinhDiem);
            if (objLabelExchange != null) GameObject.Destroy(objLabelExchange);
        }

        if (player.slotServer == GameModelChan.IndexInTurn && GameModelChan.CurrentState == GameModelChan.EGameState.playing
            &&
            (
                (player.PlayerState > EPlayerController.EPlayerState.waitingForTurn && player.PlayerState < EPlayerController.EPlayerState.fullLaying) || (GameModelChan.IsCanChiu && player.mSide == ESide.Slot_0)
            )
        )
        {
            timeCountDown -= Time.realtimeSinceStartup - oldRealTime;
            oldRealTime = Time.realtimeSinceStartup;
            oneSeconds -= Time.deltaTime;

            timerCountDown.FindChild("Foreground").transform.GetComponent<UISprite>().fillAmount = timeCountDown / GameModelChan.game.TimeCountDown;

            if (oneSeconds <= 0)
            {
                if (GameModelChan.YourController != null && GameModelChan.YourController.slotServer == player.slotServer)
                {
                    if (timerCountDown.FindChild("Foreground").transform.GetComponent<UISprite>().fillAmount < 0.3f)
                        AudioManager.Instance.Play(AudioManager.SoundEffect.OneSecondsRushed);
                    //else if (process.sliderValue < 0.5f)
                    //    AudioManager.Instance.Play(AudioManager.SoundEffect.OneSeconds);
                }
                oneSeconds = 1f;
            }
        }
        else if (timerCountDown.gameObject.activeSelf)
            timerCountDown.gameObject.SetActive(false);

        if (imageReady != null && (int)GameModelChan.CurrentState >= (int)GameModelChan.EGameState.deal)
            GameObject.Destroy(imageReady);
	}

    public void CheckIcon()
    {
        gameObject.transform.FindChild("IconMaster").gameObject.SetActive(player.isMaster);
        gameObject.transform.FindChild("IconWarning").gameObject.SetActive(!string.IsNullOrEmpty(player.warningMessage));
    }

    public void IsComeback()
    {
        if (player.mSide != ESide.Slot_0 && ListCuocUView.IsShowing == false && ListResultXuongView.IsShowing == false)
        {
            if (GetComponentInChildren<UISprite>() != null)
            {
                GetComponentInChildren<UISprite>().spriteName = "avatarPlayer";
                GetComponentInChildren<UISprite>().MakePixelPerfect();
            }
            if (avatar != null)
            {
                Transform mainTexture = avatar.FindChild("MainTexture");
                if (mainTexture != null)
                    mainTexture.GetComponent<UITexture>().alpha = 255f / 255f;
            }
        }
    }
    public static CUIPlayerChan Create(PlayerControllerChan p, Transform parentTransform)
    {
        if (listNoSlot.ContainsKey(p.slotServer))
            GameObject.Destroy(listNoSlot[p.slotServer]);

        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/PlayerInfoPrefab"));
        obj.name = "Player " + (int)p.mSide;
        obj.transform.parent = parentTransform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = scaleAvatar;
        CUIPlayerChan cui = obj.GetComponent<CUIPlayerChan>();

        cui.player = p;
        p.cuiPlayer = cui;

        p.AvatarTexture(delegate(Texture _texture)
        {
            if (cui != null && cui.avatar != null)
                //if(cui.avatar.GetComponentInChildren<UITexture>() !=null)
                cui.avatar.GetComponentInChildren<UITexture>().mainTexture = _texture;
        });
        if (p.mSide == ESide.Slot_0 && GameModelChan.YourController != null)
        {
            cui.avatar.GetComponentInChildren<UITexture>().enabled = false;

            for (int i = 0; i < cui.avatar.childCount; i++)
            {
                GameObject.Destroy(cui.avatar.GetChild(i).gameObject);
            }
            GameObject.Destroy(cui.GetComponentInChildren<UISprite>().gameObject);
            cui.timerCountDown.FindChild("Background").GetComponent<UISprite>().spriteName = "bgYourTimer";
            cui.timerCountDown.FindChild("Background").GetComponent<UISprite>().MakePixelPerfect();
            cui.timerCountDown.FindChild("Foreground").GetComponent<UISprite>().spriteName = "bgYourTimer-1";
            cui.timerCountDown.FindChild("Foreground").GetComponent<UISprite>().MakePixelPerfect();
            Transform iconWarning = cui.gameObject.transform.FindChild("IconWarning");
            Transform iconMaster = cui.gameObject.transform.FindChild("IconMaster");
            iconWarning.parent = GameModelChan.game.mPlaymat.locationHand.parent.parent;
            iconMaster.parent = GameModelChan.game.mPlaymat.locationHand.parent.parent;
            iconWarning.localPosition = new Vector3(40f, 10f, -5f);
            iconMaster.localPosition = new Vector3(-50f, 10f, -5f);
            iconWarning.GetComponent<UISprite>().depth = 22;

            iconWarning.parent = cui.transform;
            iconMaster.parent = cui.transform;
            cui.ChangePositionYouLevelBar();
        }
        else
        {
            cui.labelUserName.text = p.username;
        }
        cui.UpdateInfo();
        return cui;
    }
    public void ChangePositionYouLevelBar()
    {

        Array.ForEach<UISprite>(timerCountDown.GetComponentsInChildren<UISprite>(), s =>
        {
            s.depth = 23;
            s.transform.localPosition = new Vector3(s.transform.localPosition.x, s.transform.localPosition.y, -23f);
        });
        timerCountDown.gameObject.SetActive(false);
        timerCountDown.transform.parent = GameModelChan.game.btSorted.transform.parent;
        timerCountDown.localPosition = new Vector3(timerCountDown.localPosition.x, GameModelChan.game.btSorted.transform.localPosition.y, timerCountDown.localPosition.z);
        timerCountDown.transform.parent = gameObject.transform;
        timerCountDown.gameObject.SetActive(true);
    }

    public void IsHasQuit()
    {
        if (GetComponentInChildren<UISprite>())
        {
            GetComponentInChildren<UISprite>().spriteName = "bgAvatarPlayer";
            GetComponentInChildren<UISprite>().MakePixelPerfect();
        }

        if (timerCountDown)
            GameObject.Destroy(timerCountDown.gameObject);
        if (avatar)
            GameObject.Destroy(avatar.gameObject);
    }

    public void ShowEffect()
    {
        if (objEffect == null)
        {
            if (player.summary.result != PlayerControllerChan.FinishGame.ResultSprite.None)
            {
                objEffect = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/IconEffectPrefab"));

                objEffect.GetComponent<UITexture>().mainTexture = Resources.Load("Images/Gameplay/ICON/" + player.summary.result.ToString()) as Texture;

                Vector3 scale = new Vector3(143f, 104f, 1f); //Ù
                if (player.summary.result == PlayerControllerChan.FinishGame.ResultSprite.VE_NHAT || player.summary.result == PlayerControllerChan.FinishGame.ResultSprite.VE_NHI || player.summary.result == PlayerControllerChan.FinishGame.ResultSprite.VE_BA || player.summary.result == PlayerControllerChan.FinishGame.ResultSprite.VE_TU)
                    scale = new Vector3(97f, 104f, 1f);
                else if (player.summary.result == PlayerControllerChan.FinishGame.ResultSprite.MOM)
                    scale = new Vector3(100f, 100f, 1f);

                objEffect.transform.parent = transform;

                objEffect.GetComponent<UITexture>().MakePixelPerfect();

                if (player.mSide != ESide.Slot_0)
                    objEffect.GetComponent<UITexture>().transform.localScale /= 2;

                objEffect.transform.localPosition = (player.mSide == ESide.Slot_0) ? new Vector3(370f, -30f, -30f) : new Vector3(0f, 0f, -15f);

                if (player.mSide == ESide.Slot_0)
                    objEffect.GetComponent<UITexture>().MakePixelPerfect();
            }
        }
    }

    public void ShowTinhDiem()
    {
        if (objTinhDiem != null) return;
        if (player.mCardMelds.Count == 0) return;
        if (player.mCardHand.Count == 0) return;

        Vector3 position = new Vector3(385f, -55, -20f);
        if (player.mSide == ESide.Slot_1)
            position = new Vector3(-85f, -100f, -20f);
        else if (player.mSide == ESide.Slot_2)
            position = new Vector3(200f, 0f, -20f);
        else if (player.mSide == ESide.Slot_3)
            position = new Vector3(85f, -100f, -20f);

        objTinhDiem = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/tinhDiemPrefab"));
        objTinhDiem.transform.parent = transform;
        objTinhDiem.transform.localScale = Vector3.one;
        objTinhDiem.transform.localPosition = position;

        objTinhDiem.GetComponentInChildren<UILabel>().text = player.summary.sumRank + " Điểm";
    }

    public void StartRemainingTime(float remainingTime)
    {
        timeCountDown = remainingTime;
        timerCountDown.gameObject.SetActive(true);
        oneSeconds = 1f;
        oldRealTime = Time.realtimeSinceStartup;
    }

    public void StartTime(float total)
    {
        if (timerCountDown != null)
        {
            if (total == 0)
            {
                timerCountDown.gameObject.SetActive(false);
                return;
            }
            //UpdateLocationTimerCountDown();
            timeCountDown = total;
            timerCountDown.gameObject.SetActive(true);
            oneSeconds = 1f;
            oldRealTime = Time.realtimeSinceStartup;
        }
    }

    public void UpdateInfo()
    {
        PlayerControllerChan your = GameModelChan.YourController;

        if (avatar != null)
        {
            if (GameManager.PlayGoldOrChip == "chip")
                labelMoney.text = Utility.Convert.Chip(player.chip);
            else if (GameManager.PlayGoldOrChip == "gold")
                labelMoney.text = Utility.Convert.Chip(player.gold);
        }
        if ((int)GameModelChan.CurrentState <= (int)GameModelChan.EGameState.waitingForReady)
        {
            if (player.PlayerState == EPlayerController.EPlayerState.ready && player != your)
            {
                if (imageReady == null)
                {
                    imageReady = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/ReadyAvatarPrefab"));
                    imageReady.transform.parent = avatar;
                    imageReady.transform.localPosition = new Vector3(0f, 0f, -10f);
                    imageReady.transform.localScale = Vector3.one;
                }
            }
        }

        if (GameModelChan.CurrentState == GameModelChan.EGameState.finalizing)
        {
            if (GameModelChan.MiniState == GameModelChan.EGameStateMini.summary_point)
                ShowTinhDiem();
            else if (GameModelChan.MiniState == GameModelChan.EGameStateMini.summary_result || GameModelChan.MiniState == GameModelChan.EGameStateMini.summary_exchange)
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
}
