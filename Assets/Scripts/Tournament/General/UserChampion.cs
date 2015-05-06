using UnityEngine;
using System.Collections;
using Electrotank.Electroserver5.Api;

public class UserChampion : MonoBehaviour {
    [HideInInspector]
    public int userId;
    public UITexture avatar;
    public UISprite gonext, win, error;
    public GameObject defaultAvatar;

    public void SetData(EsObject es)
    {
        if (es.variableExists("id"))
            this.userId = es.getInteger("id");
        if (es.variableExists("continuous"))
        {
            int state = es.getInteger("continuous");
            switch (state)
            {
                case -1:
                    avatar.color = Color.gray;
                    break;
                case 1:
                    NGUITools.SetActive(win.gameObject, true);
                    break;
                default:
                    break;
            }
        }
        if (es.variableExists("avatar"))
        {
            string urlavatar = es.getString("avatar");
            if (string.IsNullOrEmpty(urlavatar))
            {
                this.avatar.mainTexture = (Texture)Resources.Load("Images/Avatar/NoAvatar", typeof(Texture));
                NGUITools.SetActive(this.avatar.gameObject, true);
                NGUITools.SetActive(defaultAvatar, false);
            }
            else
            {
                new AvatarCacheOrDownload(urlavatar, delegate(Texture image)
                {
                    if (image != null)
                    {
                        this.avatar.mainTexture = image;
                        NGUITools.SetActive(this.avatar.gameObject, true);
                        NGUITools.SetActive(defaultAvatar, false);
                    }
                });
            }
        }
    }
}
