using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Component để xử lý xây dựng background cho button ghép từ nhiều file ảnh nhỏ
/// </summary>
public class UIBackgroundButton : MonoBehaviour
{
    public UILabel textValue;
    public UISprite bgButtonLeft;
    public UISprite bgButtonCenter;
    public UISprite bgButtonRight;

    [SerializeField]
    public string TextValue = "BUTTON";
    [SerializeField]
    public float ButtonWidth = 190f;
    [SerializeField]
    public float ButtonHeight = 70f;
    [SerializeField]
    public int depth = -1;

    public void Resize()
    {
        bgButtonLeft.pivot = UIWidget.Pivot.Center;
        bgButtonCenter.pivot = UIWidget.Pivot.Center;
        bgButtonRight.pivot = UIWidget.Pivot.Center;

        float widthCenter = ButtonWidth - (bgButtonLeft.transform.localScale.x + bgButtonRight.transform.localScale.x);
		bgButtonCenter.transform.localScale = new Vector3(ButtonWidth, ButtonHeight, 1f);
        bgButtonCenter.transform.localPosition = new Vector3(0f, 0f, Math.Abs(depth) + 1f);

        bgButtonLeft.transform.localScale = new Vector3(bgButtonLeft.transform.localScale.x, ButtonHeight, 1f);
        bgButtonRight.transform.localScale = new Vector3(bgButtonRight.transform.localScale.x, ButtonHeight, 1f);

        float positionLeft = 0 - (widthCenter / 2) - (bgButtonLeft.transform.localScale.x / 2f);
        float positionRight = 0 + (widthCenter / 2) + (bgButtonRight.transform.localScale.x / 2f);
        bgButtonLeft.transform.localPosition = new Vector3(positionLeft, 0f, Math.Abs(depth));
        bgButtonRight.transform.localPosition = new Vector3(positionRight, 0f, Math.Abs(depth));
		bgButtonCenter.depth = depth - 1;
        bgButtonLeft.depth = depth;
        bgButtonRight.depth = depth;

        if (textValue != null && !string.IsNullOrEmpty(TextValue))
        {
            textValue.transform.localPosition = new Vector3(0f, 0f, depth);
            textValue.text = TextValue;
            textValue.depth = Math.Abs(depth);
        }
    }
}
