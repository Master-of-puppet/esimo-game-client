using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Component để xử lý xây dựng background cho input co background la sliced
/// </summary>
public class UIBackgroundInputSlice : MonoBehaviour
{
	public UILabel label;
    public UISprite bgSliceInputBackground;
    public CUIHandle btClear;
	
    [SerializeField]
    public int InputWidth = 150;
    [SerializeField]
    public int InputHeight = 40;
    [SerializeField]
    public int depth = -1;
    [SerializeField]
    public float buttonClearSpace = 5f;
    [SerializeField]
    public bool IsHasClear = true;
    [SerializeField]
    public bool IsColorDefault = false;
	[SerializeField]
	public bool IsRePositionLabel = true;

    UIInput input;
    void Awake()
    {
        input = gameObject.transform.parent.GetComponent<UIInput>();
		if (input == null)
			input = gameObject.GetComponent<UIInput> ();
        CUIHandle.AddClick(btClear, OnClickClear);
    }

    public void Resize()
    {
		if (bgSliceInputBackground != null) {
			bgSliceInputBackground.pivot = UIWidget.Pivot.Center;
			bgSliceInputBackground.width = InputWidth;
			bgSliceInputBackground.height = InputHeight;
			bgSliceInputBackground.transform.localPosition = new Vector3 (0f, 0f, Math.Abs (depth) + 1f);		
		}
		btClear.gameObject.SetActive(IsHasClear);

        if (gameObject.transform.parent != null && gameObject.transform.parent.GetComponent<UIInput>() != null || label != null)
        {
			input = gameObject.transform.parent.GetComponent<UIInput>();
			if(input == null)
				input = gameObject.GetComponent<UIInput> ();
            if (input != null)
            {
				if(label == null)
					label = input.GetComponentInChildren<UILabel>();
				if(IsRePositionLabel)
					label.gameObject.transform.localPosition = new Vector3((InputWidth/2 - buttonClearSpace) * -1 ,0f,-1f);
				if (!IsHasClear)
					label.width = Convert.ToInt32(InputWidth - 50f);
				else
					label.width = Convert.ToInt32(InputWidth - 3f - buttonClearSpace - btClear.transform.localScale.x - 50f);
				if (IsColorDefault)
				{
					label.color = new Color(255f/ 255f, 153f / 255f, 0f / 255f);
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
            input.Submit();
        }
    }
    void Update()
    {
        if (IsHasClear)
            btClear.gameObject.SetActive(input != null && !string.IsNullOrEmpty(input.value));
    }
}
