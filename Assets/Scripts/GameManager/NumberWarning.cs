using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Class view các số thông báo về tin nhắn, bạn bè...
/// </summary>
public class NumberWarning : MonoBehaviour
{
    public delegate bool AlwayDisable();
    public int increaseValue = 0;

    #region Unity Editor
    public enum EWarning
    {
        TOTAL,
        TOTAL_PROFILE,
        TOTAL_PROFILE_MESSAGE,
        TOTAL_PROFILE_MESSAGE_MESSAGE_SYSTEM,
        TOTAL_PROFILE_MESSAGE_PENDING_BUDDIES,
        TOTAL_PROFILE_MESSAGE_MESSAGE_CHAT_PLAYER,
    }
    public EWarning type;
    #endregion

    public AlwayDisable isDisable = null;

    void Awake()
    {
        GameManager.Server.EventMessageChanged += OnUpdateMessage;
        GameManager.Server.EventFriendPendingChanged += OnRequestFriend;
        MessageSystemCache.EventResponseMessageSystem += OnMessageSystemLoadCache;
        GameManager.Instance.ReadedMessageCallBack += ReadedMessageHandler;

    }
	void Update()
	{
		if (getNumber == 0)
			gameObject.SetActive(false);
		else
			gameObject.SetActive(true);

	}
    void OnDestroy()
    {
        if (!GameManager.IsExist) return;

        GameManager.Server.EventMessageChanged -= OnUpdateMessage;
        GameManager.Server.EventFriendPendingChanged -= OnRequestFriend;
        MessageSystemCache.EventResponseMessageSystem -= OnMessageSystemLoadCache;
        GameManager.Instance.ReadedMessageCallBack -= ReadedMessageHandler;
    }
    public void ReadedMessageHandler()
    {
        gameObject.SetActive(getNumber > 0);
        OnEnable();
    }
    int Number_New_Message
    {
        get { return GameManager.Instance.mInfo.messages.FindAll(m => m.receiver_name == GameManager.Instance.mInfo.username && m.read == false).Count; }
    }

    int Number_New_Message_System
    {
        get { return GameManager.Instance.ListMessageSystem.FindAll(m => m.read == false).Count; }
    }

    int getNumber
    {
        get
        {
            int number = 0;
            if (type == EWarning.TOTAL)
                number = GameManager.Server.totalMesseageCount > 0 ? GameManager.Server.totalMesseageCount : Number_New_Message
                    + GameManager.Instance.mInfo.pendingBuddies.Count
                    + Number_New_Message_System;
            else if (type == EWarning.TOTAL_PROFILE)
            {
                number = Number_New_Message + GameManager.Instance.mInfo.pendingBuddies.Count + Number_New_Message_System;
            }
            else if (type == EWarning.TOTAL_PROFILE_MESSAGE)
                number = Number_New_Message
                    + GameManager.Instance.mInfo.pendingBuddies.Count
                    + Number_New_Message_System;
            else if (type == EWarning.TOTAL_PROFILE_MESSAGE_PENDING_BUDDIES)
                number = GameManager.Instance.mInfo.pendingBuddies.Count;
            else if (type == EWarning.TOTAL_PROFILE_MESSAGE_MESSAGE_SYSTEM)
                number = Number_New_Message_System;

            number += increaseValue;

            if (number == 0)
                gameObject.SetActive(false);

            return number;
        }
    }

    void OnEnable()
    {
        if (GetComponentInChildren<UILabel>() != null)
            GetComponentInChildren<UILabel>().text = "" + getNumber;
    }

    void OnUpdateMessage(Messages mess, bool isOutGoing)
    {
        if (isDisable != null && isDisable())
            return;

        /// Nếu là tin nhắn hệ thống thì chỉ update tin nhắn hệ thống thôi không làm gì cả
        if (type == EWarning.TOTAL_PROFILE_MESSAGE_MESSAGE_SYSTEM && mess.sender != 0) { return; }

        if (!isOutGoing)
        {
            gameObject.SetActive(getNumber > 0);
            OnEnable();
        }
    }

    void OnRequestFriend(User friend, bool isRemove)
    {
        if (isDisable != null && isDisable())
            return;

        gameObject.SetActive(getNumber > 0);
        OnEnable();
    }

    void OnMessageSystemLoadCache()
    {
        if (isDisable != null && isDisable())
            return;

        gameObject.SetActive(getNumber > 0);
        OnEnable();
    }
    void OnReadMessage()
    {
//		gameObject.SetActive(false);
	}
}
