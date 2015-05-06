using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Component để xử lý xây dựng background cho popup ghép từ nhiều file ảnh nhỏ
/// </summary>
[Serializable]
public class UIBackgroundPopup : UnityEngine.MonoBehaviour
{
    #region Unity Edtior
    public EasyFontTextMesh textTitlePopup;
    public UISprite popupTitleLeft;
    public UISprite popupTitleCenter;
    public UISprite popupTitleRight;

    public UISprite popupMidLeft;
    public UISprite popupMidCenter;
    public UISprite popupMidRight;

    public UISprite popupBottomLeft;
    public UISprite popupBottomCenter;
    public UISprite popupBottomRight;

    public UISprite spriteLineTitle;
    public GameObject buttonClose;
    #endregion

    [SerializeField]
    public string TitlePopup = "Title";
	[SerializeField]
    public float PopupWidth = 400f;
    [SerializeField]
    public float PopupHeight = 300f;
    [SerializeField]
    public int Depth = -1;
    [SerializeField]
    public bool IsHasTitle = false;
    [SerializeField]
    public float spaceTitleText = 0f;

    public void Resize()
    {
        popupTitleLeft.pivot = UIWidget.Pivot.TopLeft;
        popupTitleCenter.pivot = UIWidget.Pivot.TopLeft;
        popupTitleRight.pivot = UIWidget.Pivot.TopLeft;
        popupMidLeft.pivot = UIWidget.Pivot.TopLeft;
        popupMidCenter.pivot = UIWidget.Pivot.TopLeft;
        popupMidRight.pivot = UIWidget.Pivot.TopLeft;
        popupBottomLeft.pivot = UIWidget.Pivot.TopLeft;
        popupBottomCenter.pivot = UIWidget.Pivot.TopLeft;
        popupBottomRight.pivot = UIWidget.Pivot.TopLeft;

        float START_X = -(PopupWidth / 2);
        float START_Y = PopupHeight / 2;

        float scaleContentWidth = PopupWidth - popupTitleLeft.transform.localScale.x - popupTitleRight.transform.localScale.x;
        float scaleContentHeight = PopupHeight - popupTitleLeft.transform.localScale.y - popupBottomLeft.transform.localScale.y;

        popupTitleLeft.transform.localPosition = new Vector3(START_X, START_Y, Math.Abs(Depth));
        popupTitleCenter.transform.localPosition = new Vector3(START_X + popupTitleLeft.transform.localScale.x, START_Y, Math.Abs(Depth));
        popupTitleRight.transform.localPosition = new Vector3(START_X + popupTitleLeft.transform.localScale.x + scaleContentWidth, START_Y, Math.Abs(Depth));

        popupTitleCenter.transform.localScale = new Vector3(scaleContentWidth, popupTitleCenter.transform.localScale.y, 1f);
        popupMidLeft.transform.localScale = new Vector3(popupMidLeft.transform.localScale.x, scaleContentHeight, 1f);
        popupMidCenter.transform.localScale = new Vector3(scaleContentWidth, scaleContentHeight, 1f);
        popupMidRight.transform.localScale = new Vector3(popupMidRight.transform.localScale.x, scaleContentHeight, 1f);
        popupBottomCenter.transform.localScale = new Vector3(scaleContentWidth, popupBottomCenter.transform.localScale.y, 1f);

        popupMidLeft.transform.localPosition = new Vector3(START_X, START_Y - popupTitleLeft.transform.localScale.y , Math.Abs(Depth));
        popupMidCenter.transform.localPosition = new Vector3(START_X + popupMidLeft.transform.localScale.x, START_Y - popupTitleLeft.transform.localScale.y, Math.Abs(Depth));
        popupMidRight.transform.localPosition = new Vector3(START_X + popupMidLeft.transform.localScale.x + scaleContentWidth, START_Y - popupTitleLeft.transform.localScale.y, Math.Abs(Depth));

        popupBottomLeft.transform.localPosition = new Vector3(START_X, START_Y - popupTitleLeft.transform.localScale.y - scaleContentHeight, Math.Abs(Depth));
        popupBottomCenter.transform.localPosition = new Vector3(START_X + popupBottomLeft.transform.localScale.x, START_Y - popupTitleLeft.transform.localScale.y - scaleContentHeight, Math.Abs(Depth));
        popupBottomRight.transform.localPosition = new Vector3(START_X + popupBottomLeft.transform.localScale.x + scaleContentWidth, START_Y - popupTitleLeft.transform.localScale.y - scaleContentHeight, Math.Abs(Depth));

        if (spriteLineTitle != null)
        {
            spriteLineTitle.pivot = UIWidget.Pivot.TopLeft;
            if (IsHasTitle)
            {
                spriteLineTitle.gameObject.SetActive(true);
                spriteLineTitle.transform.localPosition = new Vector3(
                    START_X + (popupTitleLeft.transform.localScale.x / 2),
                    START_Y - (popupTitleLeft.transform.localScale.y),
                    -1f);

                spriteLineTitle.transform.localScale = new Vector3(18f + scaleContentWidth, 5f, 1f);
            }
            else
                spriteLineTitle.gameObject.SetActive(false);
        }

        if(buttonClose != null)
            buttonClose.transform.localPosition = new Vector3(Mathf.Abs(START_X) - 35f, START_Y - popupTitleLeft.transform.localScale.y/2f - 5f, -1f);

        if (textTitlePopup != null)
        {
            textTitlePopup.Text = TitlePopup;
            textTitlePopup.transform.localPosition = new Vector3(START_X + spaceTitleText + popupTitleLeft.transform.localScale.x, START_Y - popupTitleLeft.transform.localScale.y / 2f - 3f, Depth);
        }
    }
}
