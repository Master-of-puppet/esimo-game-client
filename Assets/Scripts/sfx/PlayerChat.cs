using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NGUYÊN VIỆT DŨNG
/// Class xử lý Chát trong gameplay
/// Fix cứng một vài giá trị nhé ^^
/// </summary>
public class PlayerChat : MonoBehaviour
{
    public UISprite spriteBackground;
    public UILabel textChat;

    public enum Type
    {
        Brackets_Left,
        Brackets_Right,
    }
    public Type type = Type.Brackets_Left;
    void ReDraw()
    {
        if (type == Type.Brackets_Left)
        {
            textChat.transform.localPosition = new Vector3(17f, 4f, -1f);
            textChat.pivot = UIWidget.Pivot.Left;
        }
        else if (type == Type.Brackets_Right)
        {
            textChat.pivot = UIWidget.Pivot.Right;
            textChat.transform.localPosition = new Vector3(-17f, 4f, -1f);
            spriteBackground.transform.localEulerAngles = new Vector3(0, 180f, 0f);
         
        }
        spriteBackground.width = (int)textChat.localSize.x + 30;
      
        Invoke("DestroyMe", 5f);
    }

    public void DestroyMe()
    {
        if (gameObject != null)
            GameObject.Destroy(gameObject);
    }

    public static PlayerChat Create(string chatContent, PlayerControllerChan player)
    {
        if(chatContent.Length > 21)
            chatContent = chatContent.Substring(0, 20) + "...";

        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/sfx/PlayerChatPrefab"));
        obj.name = "Chat Content";
        obj.transform.parent = player.cuiPlayer.gameObject.transform;
        obj.transform.localPosition = new Vector3(player.mSide == ESide.Slot_1 ? -40f : 40f, 70f, -10f);
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<PlayerChat>().textChat.text = chatContent;
        obj.GetComponent<PlayerChat>().type = player.mSide == ESide.Slot_1 ? Type.Brackets_Right : Type.Brackets_Left;
        obj.GetComponent<PlayerChat>().ReDraw();
        return obj.GetComponent<PlayerChat>();
    }

    public static PlayerChat Create(string chatContent, PlayerControllerPhom player)
    {
        if (chatContent.Length > 21)
            chatContent = chatContent.Substring(0, 20) + "...";

        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/sfx/PlayerChatPrefab"));
        obj.name = "Chat Content";
        obj.transform.parent = player.cuiPlayer.gameObject.transform;
        obj.transform.localPosition = new Vector3(player.mSide == ESide.Slot_1 ? -40f : 40f, 70f, -10f);
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<PlayerChat>().textChat.text = chatContent;
        obj.GetComponent<PlayerChat>().type = player.mSide == ESide.Slot_1 ? Type.Brackets_Right : Type.Brackets_Left;
        obj.GetComponent<PlayerChat>().ReDraw();
        return obj.GetComponent<PlayerChat>();
    }

    public static PlayerChat Create(string chatContent, PlayerControllerTLMN player)
    {
        if (chatContent.Length > 21)
            chatContent = chatContent.Substring(0, 20) + "...";

        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/sfx/PlayerChatPrefab"));
        obj.name = "Chat Content";
        obj.transform.parent = player.cuiPlayer.gameObject.transform;
        obj.transform.localPosition = new Vector3(player.mSide == ESide.Slot_1 ? -40f : 40f, 70f, -10f);
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<PlayerChat>().textChat.text = chatContent;
        obj.GetComponent<PlayerChat>().type = player.mSide == ESide.Slot_1 ? Type.Brackets_Right : Type.Brackets_Left;
        obj.GetComponent<PlayerChat>().ReDraw();
        return obj.GetComponent<PlayerChat>();
    }
}
