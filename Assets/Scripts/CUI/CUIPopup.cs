using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Component để định nghĩa và xử lý các popup trong game
/// </summary>
public class CUIPopup : MonoBehaviour
{
    public CUIHandle.OnClickDelegate buttonClose;

    void Start()
    {
        GameManager.Instance.ListPopup.Add(this);
    }

    void OnDestroy()
    {
        if (!GameManager.IsExist) return;

        GameManager.Instance.ListPopup.Remove(this);
    }
}
