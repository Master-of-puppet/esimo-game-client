using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class NumberUserInChannel : MonoBehaviour
{
#region UnityEditor
    public UISprite target = null;
    private float defaultWidthOfSprite = 0f;
    public UILabel lblDecription;
#endregion
    void Start()
    {
        if (target == null) target = GetComponentInChildren<UISprite>();
        defaultWidthOfSprite = target.width;
    }

    public void SetValue(int value)
    {
		target.width = Mathf.RoundToInt((defaultWidthOfSprite / 10) * value) < 97 ? Mathf.RoundToInt((defaultWidthOfSprite / 10) * value): 97;
    }
}
