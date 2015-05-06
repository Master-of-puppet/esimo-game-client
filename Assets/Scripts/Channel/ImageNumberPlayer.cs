using System;
using System.Collections.Generic;
using UnityEngine;

public class ImageNumberPlayer : MonoBehaviour
{
    #region UnityEditor
    public UISprite targetEmpty;
    public UISprite targetFull;
    #endregion
    
    public void SetValue(int value)
    {
        targetEmpty.gameObject.SetActive(value >= 0);
        targetFull.gameObject.SetActive(value > 1);
        if (value < 0) return;
        targetFull.fillAmount = value / 4f;
    }
}

