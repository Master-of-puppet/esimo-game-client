using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Component chuyển đổi ảnh
/// </summary>
public class CUISwitchTexture  : MonoBehaviour
{
    #region Unity Editor
    public UITexture mainTexture;
    public Texture [] textures;

    public EType type = EType.Normal_1000_MS;
    #endregion

    public enum EType
    {
        EACH_FRAME,
        Normal_0025_MS = 250,
        Normal_0500_MS = 500,
        Normal_1000_MS = 1000
    }
    private int indexSwtich;

    void OnEnable()
    {
        float time = type == EType.EACH_FRAME ? Time.deltaTime : (int)type / 1000f;
        InvokeRepeating("StartSwitch", 0f, time);
        indexSwtich = 0;
    }

    void StartSwitch()
    {
        mainTexture.mainTexture = textures[indexSwtich];

        indexSwtich++;
        if (indexSwtich >= textures.Length)
            indexSwtich = 0;
    }
}
