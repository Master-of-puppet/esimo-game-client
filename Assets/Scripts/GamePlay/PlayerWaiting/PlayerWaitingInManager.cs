using UnityEngine;
using System.Collections;

public class PlayerWaitingInManager : MonoBehaviour
{

    #region Unity Editor
    public UILabel lbUsername, lbChip;
    public CUIHandle btnKickUser;
    #endregion
    [HideInInspector]
    public EPlayerController player;

    [HideInInspector]
    public EGPViewManager grandParent;
    bool hasSetData = false;
    public static PlayerWaitingInManager Create(EPlayerController player, UIGrid parent, EGPViewManager grandParent) 
    {
        GameObject gobj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/PlayerWaiting/PlayerWaitingInManager"));
        gobj.transform.parent = parent.transform;
        gobj.transform.localPosition = Vector3.zero;
        gobj.transform.localScale = Vector3.one;
        PlayerWaitingInManager item = gobj.GetComponent<PlayerWaitingInManager>();
        item.grandParent = grandParent;
        item.SetData(player);
        return item;
    }

    public void SetData(EPlayerController player)
    {
        this.player = player;
        StartCoroutine(_SetData());
    }
    IEnumerator _SetData() {
        lbUsername.text = player.username;
        if (GameManager.PlayGoldOrChip == "chip")
            lbChip.text = Utility.Convert.ChipToK(player.chip) + " chip";
        else if (GameManager.PlayGoldOrChip == "gold")
            lbChip.text = Utility.Convert.ChipToK(player.gold) + " gold";
        //GetComponent<UIToggle>().value = GameManager.CurrentScene == ESceneName.GameplayChan ? ((PlayerControllerChan)player).isPriority : GameManager.CurrentScene == ESceneName.GameplayPhom ? ((PlayerControllerPhom)player).isp : ((PlayerControllerChan)player).isPriority;
        yield return new WaitForSeconds(0.1f);
        hasSetData = true;
    }
    void Start() 
    {
        CUIHandle.AddClick(btnKickUser, OnClickKickUser);
    }
    void OnDestroy() 
    {
        CUIHandle.RemoveClick(btnKickUser, OnClickKickUser);
    }
    public void OnPreferPlayer() 
    {
        if (enabled && hasSetData) 
        {
            if (UIToggle.current.value)
            {
                UIToggle.current.value = false;
                NotificationView.ShowConfirm("Lưu ý", "Bạn có chắc chắn muốn ưu tiên người chơi " + player.username + " vào bàn chơi khi có người khác thoát ra? ", delegate()
                {
                    GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("play", new object[] { Fields.ACTION, "changePriorityInWaitingQueue", "userName", player.username }));
                }, delegate() {
                    grandParent.SetActiveListToggle();
                }, "Chắc chắn", "Không");
            }
        }
    }
    private void OnClickKickUser(GameObject targetObject)
    {
        NotificationView.ShowConfirm("Lưu ý", "Bạn có chắc chắn muốn đuổi " + player.username + " ra khỏi bàn chơi ?", delegate() {
            GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("play", new object[] { Fields.ACTION, "kickWaitingPlayer","userName",player.username }));
        }, null, "Chắc chắn", "Không");
    }
}
