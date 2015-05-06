using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Component để xử lý xây dựng background cho input ghép từ nhiều file ảnh nhỏ
/// </summary>
public class UIBackgroundInput : MonoBehaviour
{
    public UISprite bgInputLeft;
    public UISprite bgInputCenter;
    public UISprite bgInputRight;

    public CUIHandle btClear;

    [SerializeField]
    public float InputWidth = 150f;
    [SerializeField]
    public float InputHeight = 40f;
    [SerializeField]
    public int depth = -1;
    [SerializeField]
    public float buttonClearSpace = 5f;
    [SerializeField]
    public bool IsHasClear = true;
    [SerializeField]
    public bool IsColorDefault = false;

    UIInput input;
    void Awake()
    {
        input = gameObject.transform.parent.GetComponent<UIInput>();
        CUIHandle.AddClick(btClear, OnClickClear);
    }

    public void Resize()
    {
        bgInputLeft.pivot = UIWidget.Pivot.Center;
        bgInputCenter.pivot = UIWidget.Pivot.Center;
        bgInputRight.pivot = UIWidget.Pivot.Center;

		float widthCenter = InputWidth- (bgInputLeft.transform.localScale.x + bgInputRight.transform.localScale.x);
		bgInputCenter.transform.localScale = new Vector3(InputWidth, InputHeight, 1f);
        bgInputCenter.transform.localPosition = new Vector3(0f, 0f, Math.Abs(depth) + 1f);

        bgInputLeft.transform.localScale = new Vector3(bgInputLeft.transform.localScale.x, InputHeight, 1f);
        bgInputRight.transform.localScale = new Vector3(bgInputRight.transform.localScale.x, InputHeight, 1f);

        float positionLeft = 0 - (widthCenter / 2) - (bgInputLeft.transform.localScale.x / 2f);
        float positionRight = 0 + (widthCenter / 2) + (bgInputRight.transform.localScale.x / 2f);
        bgInputLeft.transform.localPosition = new Vector3(positionLeft, 0f, Math.Abs(depth));
        bgInputRight.transform.localPosition = new Vector3(positionRight, 0f, Math.Abs(depth) );

        bgInputLeft.depth = depth;
        bgInputCenter.depth = depth - 1 ;
        bgInputRight.depth = depth;

        btClear.transform.localPosition = new Vector3(bgInputRight.transform.localPosition.x - btClear.transform.localScale.x / 2 - buttonClearSpace, 0, -1f);
		btClear.gameObject.SetActive(IsHasClear);

        if (gameObject.transform.parent != null && gameObject.transform.parent.GetComponent<UIInput>() != null)
        {
            input = gameObject.transform.parent.GetComponent<UIInput>();
            if (input != null)
            {
                if (!IsHasClear)
                    input.GetComponentInChildren<UILabel>().width = Convert.ToInt32(widthCenter);
                else
                    input.GetComponentInChildren<UILabel>().width = Convert.ToInt32(widthCenter - 3f - buttonClearSpace - btClear.transform.localScale.x);

                if (IsColorDefault)
                {
                    input.GetComponentInChildren<UILabel>().color = new Color(84 / 255f, 44 / 255f, 14 / 255f);
                    input.activeTextColor = Color.black;
                }
            }
        }
    }


    void OnClickClear(GameObject go)
    {
        if (input != null)
        {
            input.value = "";
            UICamera.selectedObject = input.gameObject;
        }
    }

    void Update()
    {
        if (IsHasClear)
            btClear.gameObject.SetActive(input != null && !string.IsNullOrEmpty(input.value));
    }
}
