using UnityEngine;
using Electrotank.Electroserver5.Api;

public class TournamentWinner : MonoBehaviour {

    public UILabel lblAward,lblName;
    public UITexture avatar;

	public void SetData(string award,string name,string url)
    {
        this.lblAward.text = award;
        this.lblName.text = name;
        GetAvatar(url);
    }

    void GetAvatar(string url)
    {
        new AvatarCacheOrDownload(url, delegate(Texture texture)
            {
                if (texture!=null)
                {
                    this.avatar.mainTexture = texture;
                }
            }, true);
    }
}
