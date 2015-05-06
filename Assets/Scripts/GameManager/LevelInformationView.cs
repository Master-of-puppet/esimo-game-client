using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInformationView : MonoBehaviour
{
    #region Unity Editor

    public UILabel lbLevel, lbUserName, lbChip, lbGold;
    public UILabel lbTextLevel;
    public UISlider processbar;
    public bool isGameplayBar = false;
    public UITexture textureAvatar;
    #endregion

    void Awake()
    {
        GameManager.Server.EventUpdateUserInfo += OnUpdateUserInfo;
        //GameManager.Server.EventPluginMessageOnProcess += OnProcessMessage;
    }

    void Start()
    {
        SetData();
        //GameManager.Server.DoRequestPlugin(Utility.SetEsObject("getLevel", new object[]{
        //        "appId", GameManager.Instance.hallRoom.gameId
        //    }));
    }
    void OnDestroy()
    {
        if (!GameManager.IsExist) return;

        GameManager.Server.EventUpdateUserInfo -= OnUpdateUserInfo;
        //GameManager.Server.EventPluginMessageOnProcess -= OnProcessMessage;
    }

    void OnUpdateUserInfo()
    {
        _OnUpdateUserInfo();
    }

    void _OnUpdateUserInfo()
    {
        //if(isGameplayBar)
        //    yield return new WaitForSeconds(10f);
        SetData();
    }
    //private void OnProcessMessage(string command, string action, Electrotank.Electroserver5.Api.EsObject parameters)
    //{
    //    if (command == "getLevel")
    //    {
    //        if (parameters.variableExists("level"))
    //            GameManager.Instance.mInfo.level = parameters.getInteger("level");
    //        if (parameters.variableExists("experience"))
    //            GameManager.Instance.mInfo.experience = parameters.getInteger("experience");
    //        if (parameters.variableExists("expMinCurrentLevel"))
    //            GameManager.Instance.mInfo.expMinCurrentLevel = parameters.getInteger("expMinCurrentLevel");
    //        if (parameters.variableExists("expMinNextLevel"))
    //            GameManager.Instance.mInfo.expMinNextLevel = parameters.getInteger("expMinNextLevel");
    //        SetData();
    //    }
    //}
    void SetData()
    {
        if (lbLevel != null)
            lbLevel.text = (lbTextLevel == null ? "LV " : "") + GameManager.Instance.mInfo.level;
        if (processbar!=null)
            processbar.value = (GameManager.Instance.mInfo.experience - GameManager.Instance.mInfo.expMinCurrentLevel) / (float)GameManager.Instance.mInfo.expMinNextLevel;
        if (lbTextLevel != null)
            lbTextLevel.text = GameManager.Instance.mInfo.experience + "/" + GameManager.Instance.mInfo.expMinNextLevel;
        if (lbUserName != null)
            //lbUserName.text = string.IsNullOrEmpty(GameManager.Instance.mInfo.FullName) ? GameManager.Instance.mInfo.username : GameManager.Instance.mInfo.FullName.Trim();
            lbUserName.text = string.IsNullOrEmpty(GameManager.Instance.mInfo.FullName) ? GameManager.Instance.mInfo.username.Trim().Length > 20 ? GameManager.Instance.mInfo.username.Trim().Substring(0, 20) + "..." : GameManager.Instance.mInfo.username : GameManager.Instance.mInfo.FullName.Trim().Length > 20 ? GameManager.Instance.mInfo.FullName.Trim().Substring(0, 20) + "..." : GameManager.Instance.mInfo.FullName.Trim();

        if (lbChip != null) lbChip.text = Utility.Convert.Chip(GameManager.Instance.mInfo.chip);
        if (lbGold != null) lbGold.text = Utility.Convert.Chip(GameManager.Instance.mInfo.gold);
        if (GameManager.PlayGoldOrChip == "gold")
        {
            lbGold.gameObject.SetActive(true);
            lbChip.gameObject.SetActive(false);
        }
        else if (GameManager.PlayGoldOrChip == "chip")
        {
            if (lbGold)
                lbGold.gameObject.SetActive(false);
          
            if (lbChip)
                lbChip.gameObject.SetActive(true);
        }
        if (this.name == "LevelInfoProfile" || this.name == "__GroupInfoGoldAndChip")
        {
            lbGold.gameObject.SetActive(true);
            lbChip.gameObject.SetActive(true);
            GameManager.Instance.mInfo.AvatarTexture(delegate(Texture _texture) { if (textureAvatar != null) textureAvatar.mainTexture = _texture; });
            lbUserName.text = string.IsNullOrEmpty(GameManager.Instance.mInfo.username) ? GameManager.Instance.mInfo.username.Trim().Length > 20 ? GameManager.Instance.mInfo.username.Trim().Substring(0, 20) + "..." : GameManager.Instance.mInfo.username : GameManager.Instance.mInfo.username.Trim().Length > 20 ? GameManager.Instance.mInfo.username.Trim().Substring(0, 20) + "..." : GameManager.Instance.mInfo.username.Trim();
        }
    }
}
