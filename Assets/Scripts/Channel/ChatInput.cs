using UnityEngine;

public class ChatInput : MonoBehaviour
{
    public CUITextList textList;
    
    UIInput mInput;
    bool mIgnoreNextEnter = false;
    /// <summary>
    /// Add some dummy text to the text list.
    /// </summary>

    void Start()
    {
        mInput = GetComponent<UIInput>();
    }

    /// <summary>
    /// Pressing 'enter' should immediately give focus to the input field.
    /// </summary>

    void Update()
    {
//        if (Input.GetKeyUp(KeyCode.Return))
//        {
//            if (!mIgnoreNextEnter && !mInput.isSelected)
//            {
//                mInput.isSelected = true;
//            }
//            mIgnoreNextEnter = false;
//        }
    }

    /// <summary>
    /// Submit notification is sent by UIInput when 'enter' is pressed or iOS/Android keyboard finalizes input.
    /// </summary>
    /// 
    void Submit()
    {
        if (textList != null)
        {
            // It's a good idea to strip out all symbols as we don't want user input to alter colors, add new lines, etc
            string text = NGUIText.StripSymbols(mInput.value);

            if (!string.IsNullOrEmpty(text))
            {
                SendChatToServer(text);

                if (GameManager.CurrentScene == ESceneName.GameplayChan)
                    text = string.Format("{0}" + GameManager.Instance.mInfo.username.ToUpper() + ":[-] " + text + "\n", GameModelChan.ListWaitingPlayer.Find(plc => plc.username == GameManager.Instance.mInfo.username) != null ? "[FFCC00]" : "[FFCC00]");
                else
                    text = string.Format("[FFCC00]" + GameManager.Instance.mInfo.username.ToUpper() + ":[-] " + text + "\n");

                textList.Add(text);
                Utility.AutoScrollChat(textList);

                mInput.value = "";
                mInput.isSelected = false;
            }
        }
        mIgnoreNextEnter = true;
    }

    void SendChatToServer(string message)
    {
        Electrotank.Electroserver5.Api.PublicMessageRequest request = new Electrotank.Electroserver5.Api.PublicMessageRequest();
        request.ZoneId = GameManager.Instance.currentRoom.zoneId;
        request.RoomId = GameManager.Instance.currentRoom.roomId;
        request.Message = message;
        GameManager.Server.DoRequest(request);
    }
}