using UnityEngine;
using System.Collections;
using Electrotank.Electroserver5.Api;

public class HallItem : MonoBehaviour {

    public UITexture textureLogo;

    [HideInInspector]
    public HallInfo info;

    public void SetData(EsObject es)
    {
        this.info = new HallInfo(es);
        DisplayData();
    }

    void DisplayData()
    {
        new AvatarCacheOrDownload(info.icon, delegate(Texture texture) {
            textureLogo.mainTexture = texture;
        });
    }

    void OnClick()
    {
        WaitingView.Show("Đang vào phòng");
        GameManager.Instance.hallRoom.gameId = this.info.gameId;
        GameManager.Server.DoRequestPlugin(Utility.SetEsObject("getLevel", new object[]{
                "appId", GameManager.Instance.hallRoom.gameId
            }));
        GameManager.Server.DoJoinRoom(info.zoneId, info.roomId);
    }
}
