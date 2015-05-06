using System;
using System.Collections.Generic;
using UnityEngine;

public class SettingView : MonoBehaviour
{
    #region Unity Editor
    public UIToggle cbSoundEffect, cbSoundBackground, cbFullScreen,cbLockScreen;
    public CUIHandle btClose;
    #endregion

    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickBack;
        CUIHandle.AddClick(btClose, OnClickBack);
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(btClose, OnClickBack);
        //Transform obj = HeaderMenu.Instance.transform.FindChild("Camera");
        //obj.camera.depth = 51;
    }

    void OnClickBack(GameObject go)
    {
        GameObject.Destroy(gameObject);
    }


    bool wasStart = false;
    void Start()
    {
        //disable checkbox full screen on mobile
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
        cbFullScreen.GetComponent<BoxCollider>().enabled = false;
        cbFullScreen.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 90 / 255f);
#endif
        //chưa có âm thanh nền nên không cho người chơi tương tác
        cbSoundBackground.GetComponent<BoxCollider>().enabled = false;
        cbSoundBackground.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 90 / 255f);

        cbSoundEffect.value = StoreGame.LoadInt(StoreGame.EType.SOUND_EFFECT, (int)StoreGame.EToggle.ON) == (int)StoreGame.EToggle.ON;
        cbSoundBackground.value = StoreGame.LoadInt(StoreGame.EType.SOUND_BACKGROUND, (int)StoreGame.EToggle.ON) == (int)StoreGame.EToggle.ON;
        cbFullScreen.value = GameManager.Setting.IsFullScreen;
        cbLockScreen.value = GameManager.Setting.AllowLockScreen;
        wasStart = true;
    }

    public static SettingView Create()
    {

        GameObject setting = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/SettingPrefab"));
        setting.name = "__SettingPrefab";
        setting.transform.position = new Vector3(-1301, 240, -128f);
        //Transform obj = HeaderMenu.Instance.transform.FindChild("Camera");
        //obj.camera.depth = 5;
        return setting.GetComponent<SettingView>();
    }

    void OnActivateSoundBackground(bool _active)
    {
        AudioManager.Instance.SetVolumeBackground(!_active);
    }

    void OnActivateSoundEffect(bool _active)
    {
        AudioManager.Instance.SetVolumeEffect(!_active);
    }

    void OnActivateFullScreen(bool _active)
    {
        if(wasStart)
            GameManager.Setting.IsFullScreen = _active;
    }
    void OnActivateLockScreen(bool _active)
    {
        GameManager.Setting.AllowLockScreen = _active;
        StoreGame.SaveInt(StoreGame.EType.LOCK_SCREEN, _active ? (int)StoreGame.EToggle.ON : (int)StoreGame.EToggle.OFF);
    }
}
