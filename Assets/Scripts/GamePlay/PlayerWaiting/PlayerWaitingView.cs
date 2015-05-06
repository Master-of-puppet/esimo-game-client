using UnityEngine;
using System.Collections;

public class PlayerWaitingView : MonoBehaviour
{

    #region Unity Editor
    public UITexture avatar;
    public UILabel lbUserName, lbChip;
    #endregion

    [HideInInspector]
    public PlayerControllerChan player;
    public static PlayerWaitingView Create(PlayerControllerChan player, UIGrid parent) 
    {
        GameObject gobj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/PlayerWaiting/PlayerWaitingView"));
        gobj.transform.parent = parent.transform;
        gobj.transform.localPosition = Vector3.zero;
        gobj.transform.localScale = Vector3.one;
        PlayerWaitingView item = gobj.GetComponent<PlayerWaitingView>();
        item.SetData(player);
        return item;
    }
    public void SetData(PlayerControllerChan player) {
        this.player = player;
        player.AvatarTexture(delegate(Texture texture) { avatar.mainTexture = texture; });
        lbUserName.text = player.username;
        if (GameManager.PlayGoldOrChip == "chip")
            lbChip.text = Utility.Convert.ChipToK(player.chip) + " chip";
        else if (GameManager.PlayGoldOrChip == "gold")
            lbChip.text = Utility.Convert.ChipToK(player.gold) + " gold";
        lbUserName.transform.parent.GetComponent<UITable>().Reposition();
        GetComponent<UIToggle>().value = player.isPriority;
    } 
}
