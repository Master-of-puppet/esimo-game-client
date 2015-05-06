using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Component để lưu các thông tin khác trên scenes nếu cần thiết
/// </summary>
public class UIContainerAnonymous : MonoBehaviour
{
    [HideInInspector]
    public System.Object intermediary;
    public int valueInt;
    public string valueString;
    public float valueFloat;
}
