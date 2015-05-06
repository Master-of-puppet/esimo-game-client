using UnityEngine;
using System.Collections;
using Electrotank.Electroserver5.Api;

public class PopupUser : MonoBehaviour {

    public UILabel lblPoint, lblName;
    public UITexture avatar;
    public UISprite spPoint;
    [HideInInspector]
    public int userId;
    [HideInInspector]
    public string avatarUrl;

    public void SetData(EsObject es)
    {
        if (es.variableExists("userId"))
            this.userId = es.getInteger("userId");
        if (es.variableExists("userName"))
            lblName.text = es.getString("userName");
        if (es.variableExists("avatar"))
            this.avatarUrl = es.getString("avatar");
        if(es.variableExists("point"))
        {
            int point = es.getInteger("point");
            lblPoint.text = point.ToString();
            if (point > 0)
                spPoint.spriteName = "";
            else
                spPoint.spriteName = "";
            NGUITools.SetActive(spPoint.gameObject, true);
        }
        OnShowAvatar();
    }

    void OnShowAvatar()
    {
        if (string.IsNullOrEmpty(this.avatarUrl))
        {
            this.avatar.mainTexture = (Texture)Resources.Load("Images/Avatar/NoAvatar", typeof(Texture));
            NGUITools.SetActive(this.avatar.gameObject, true);
        }
        else
        {
            new AvatarCacheOrDownload(avatarUrl, delegate(Texture image)
            {
                if (image != null)
                {
                    this.avatar.mainTexture = image;
                    NGUITools.SetActive(this.avatar.gameObject, true);
                }
            });
        }
    }
}
