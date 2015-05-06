using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Component để xử lý các input có thêm cái nút clear
/// </summary>
public class UIInputClear : UIInput
{
    public event CUIHandle.OnClickDelegate EventOnClickClear;

    public CUIHandle btClear;

    void Awake()
    {
        if(btClear == null)
            btClear = transform.GetComponentInChildren<CUIHandle>();

        CUIHandle.AddClick(btClear, OnClickClear);
    }

    void OnClickClear(GameObject go)
    {
        value = "";

        if (EventOnClickClear != null)
            EventOnClickClear(gameObject);
    }


}
