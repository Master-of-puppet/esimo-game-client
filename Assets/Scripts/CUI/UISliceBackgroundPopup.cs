using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Component để xử lý xây dựng background cho popup dung slice sprite
/// </summary>
[Serializable]
public class UISliceBackgroundPopup : UnityEngine.MonoBehaviour
{
    #region Unity Edtior
    public UILabel textTitlePopup;
    public UISprite popupBackground;
    public UISprite popupTitleBackground;    
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
    public bool IsShowBtnClose = true;
	[ContextMenu("Resize")]
    public void Resize()
    {
        popupBackground.transform.localScale = Vector3.one;
        popupTitleBackground.transform.localScale = Vector3.one;
        popupBackground.transform.localPosition = new Vector3(0f, 0f, 99f);

        popupTitleBackground.depth = -98;
        popupBackground.depth = -99;
		popupBackground.pivot = UIWidget.Pivot.Center;

        float START_X = -(PopupWidth / 2);
        float START_Y = PopupHeight / 2;
		popupBackground.width = (int)PopupWidth;
		popupBackground.height = (int) PopupHeight;

        if(buttonClose != null)
            buttonClose.transform.localPosition = new Vector3(Mathf.Abs(START_X) - 37f, START_Y - buttonClose.GetComponent<BoxCollider>().size.y/2 - 11f, -1f);
        
        if (popupTitleBackground != null)
        {
            popupTitleBackground.pivot = UIWidget.Pivot.Left;
            popupTitleBackground.transform.localPosition = new Vector3(START_X + 12f, START_Y - 36f, 98f);
            if (IsShowBtnClose)
            {
                buttonClose.gameObject.SetActive(true);
				popupTitleBackground.width =(int)(PopupWidth - 24f - buttonClose.GetComponent<BoxCollider>().size.x -1 );
				popupTitleBackground.height =(int)popupTitleBackground.height;
            }
            else
            {
                buttonClose.gameObject.SetActive(false);
				popupTitleBackground.width = (int)(PopupWidth - 25f);
				popupTitleBackground.height = (int)popupTitleBackground.height;
               
            }
                
        }

        if (textTitlePopup != null)
        {
			textTitlePopup.text= TitlePopup;
			textTitlePopup.transform.localPosition = new Vector3(START_X + 25f + textTitlePopup.localSize.x/2, START_Y - 36f, -1f);
        }
    }
}
