using UnityEngine;
using System.Collections;
using Electrotank.Electroserver5.Api;

public class HallSenceCode : MonoBehaviour
{
    public UIGrid gridHall;
    public GameObject hallPrefab;

    void Awake()
    {
        GameManager.Server.EventPluginMessageOnProcess += OnPluginProcess;
        GameManager.Server.EventJoinRoom += OnAfterJoinRoom;
        HeaderMenu.Instance.ReDraw();
        GameManager.PlayGoldOrChip = Fields.StatusPlayer.CHIP;
    }

    private void OnAfterJoinRoom(JoinRoomEvent e)
    {
        WaitingView.Hide();
        //lưu lại thông tin channel select to back button
        GameManager.Instance.channelRoom = new RoomInfo(e.ZoneId, e.RoomId);
        switch (e.RoomName.ToLower())
        {
            case "phom":
                GameManager.LoadScene(ESceneName.ChannelPhom);
                break;
            case "tlmn":
                GameManager.LoadScene(ESceneName.ChannelTLMN);
                break;
            case "chan":
                GameManager.LoadScene(ESceneName.ChannelChan);
                break;
            default:
                Debug.LogError("Cannot find any game");
                break;
        }
    }

    void OnDestroy()
    {
        GameManager.Server.EventPluginMessageOnProcess -= OnPluginProcess;
        GameManager.Server.EventJoinRoom -= OnAfterJoinRoom;
    }

    private void OnPluginProcess(string command, string action, EsObject parameters)
    {
        if (command == Fields.RESPONSE.FULL_UPDATE)
        {
            if (parameters.variableExists("children"))
            {
                EsObject[] children = parameters.getEsObjectArray("children");
                for (int i = 0; i < children.Length; i++)
                {
                    GameObject obj = NGUITools.AddChild(gridHall.gameObject, hallPrefab);
                    obj.name = "hall" + i;
                    obj.GetComponent<HallItem>().SetData(children[i]);
                }
                gridHall.Reposition();
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        GameManager.Server.DoRequestCommand(Fields.REQUEST.REQUEST_FULL);
        HeaderMenu.Instance.OnClickButtonBackCallBack = delegate()
        {
            NotificationView.ShowConfirm("Lưu ý", "Bạn có chắc chắn muốn đăng xuất ? ", delegate() { GameManager.Server.DoLogOut(); }, null, "Đồng ý", "Hủy");
        };

    }
}
