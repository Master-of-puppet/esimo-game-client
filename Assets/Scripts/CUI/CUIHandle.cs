using UnityEngine;
using System.Collections;

[AddComponentMenu("NGUI/dungnv/CUIHandle")]

/// <summary>
/// Events.
/// Author: Nguyen Viet Dung
/// Date  : 22/03/2013
/// Lop xu ly bat su kien button cua NGUI
/// </summary>
public class CUIHandle : MonoBehaviour
{
	#region Khai báo các delegate & event
	public delegate void OnPressInDelegate(GameObject targetObject);
	public delegate void OnPressOutDelegate(GameObject targetObject);
	
	public delegate void OnHoverInDelegate(GameObject targetObject);
	public delegate void OnHoverOutDelegate(GameObject targetObject);

    public delegate void OnClickDelegate(GameObject targetObject);
	
	public event OnPressInDelegate OnCPressIn;
	public event OnPressOutDelegate OnCPressOut;
	
	public event OnHoverInDelegate OnCHoverIn;
	public event OnHoverOutDelegate OnCHoverOut;

    public event OnClickDelegate OnCClick;
	#endregion
	
	#region Called when active
	void OnPress (bool isPressed)
	{
        if (timeCountdown > 0) return;

		if(isPressed && OnCPressIn != null)
			OnCPressIn(gameObject);
		if(!isPressed && OnCPressOut != null)
			OnCPressOut(gameObject);
	}

	void OnHover (bool isHover)
	{
        if (timeCountdown > 0) return;

        if(isHover && OnCHoverIn != null)
			OnCHoverIn(gameObject);
		if(!isHover && OnCHoverOut != null)
			OnCHoverOut(gameObject);
	}

    void OnClick()
    {
        if (timeCountdown > 0) return;
        if (Input.touchCount > 1) return;
        if (OnCClick != null)
            OnCClick(gameObject);
    }

    /// <summary>
    /// Không thể click sau bao nhiêu giây nữa ?
    /// </summary>
    /// <param name="time"></param>
    public void StopImpact(float time)
    {
        timeCountdown = time;
    }

    protected float timeCountdown = 0f;
    void Update()
    {
        if (timeCountdown > 0)
            timeCountdown -= Time.deltaTime;
    }
	
	/*
	void OnDrag (Vector2 delta){}
	void OnScroll (float delta){}
	void OnClick (){}
	void OnDoubleClick () {}
	void OnBecameVisible() {} //OnBecameVisible is called when the renderer became visible by any camera.
	void OnBecameInvisible() {} //OnBecameInvisible is called when the renderer is no longer visible by any camera.
	void OnLevelWasLoaded(int level) {}
	void OnEnable() {}
	void OnDisable() {}
	void OnDestroy() {}
	*/
	#endregion
		
	#region Event Listener & Đăng ký các Event

    public static void AddClick(CUIHandle handle, OnClickDelegate OnDelegate) {
        handle.OnCClick += OnDelegate;
    }
    public static void AddClick(CUIHandle[] array, OnClickDelegate OnDelegate) {
        if (array == null) return;
        foreach (CUIHandle handle in array) handle.OnCClick += OnDelegate;
    }
    public static void RemoveClick(CUIHandle handle, OnClickDelegate OnDelegate) {
        handle.OnCClick -= OnDelegate;
    }
    public static void RemoveClick(CUIHandle[] array, OnClickDelegate OnDelegate) {
        if (array == null) return;
        foreach (CUIHandle handle in array) handle.OnCClick += OnDelegate;
    }


	public static void AddPressIn(CUIHandle handle, OnPressInDelegate OnDelegate) {
		handle.OnCPressIn += new CUIHandle.OnPressInDelegate(OnDelegate);
	}
	public static void AddPressIn(CUIHandle[] array, OnPressInDelegate OnDelegate) {
		if(array == null) return;
		foreach(CUIHandle handle in array) handle.OnCPressIn += new CUIHandle.OnPressInDelegate(OnDelegate);
	}
	
	public static void AddPressOut(CUIHandle handle, OnPressOutDelegate OnDelegate) {
		handle.OnCPressOut += OnDelegate;
	}
    public static void AddPressOut(CUIHandle[] array, OnPressOutDelegate OnDelegate) {
        if (array == null) return;
        foreach (CUIHandle handle in array) handle.OnCPressOut += OnDelegate;
    }
    public static void RemovePressOut(CUIHandle handle, OnPressOutDelegate OnDelegate) {
        handle.OnCPressOut -= OnDelegate;
    }
    public static void RemovePressOut(CUIHandle[] array, OnPressOutDelegate OnDelegate) {
        if (array == null) return;
        foreach (CUIHandle handle in array) handle.OnCPressOut -= OnDelegate;
    }
	
	public static void AddHoverIn(CUIHandle handle, OnHoverInDelegate OnDelegate) {
		handle.OnCHoverIn += new CUIHandle.OnHoverInDelegate(OnDelegate);
	}
	public static void AddHoverOut(CUIHandle handle, OnHoverOutDelegate OnDelegate) {
		handle.OnCHoverOut += new CUIHandle.OnHoverOutDelegate(OnDelegate);
	}
	#endregion
	
	public void SetTextButton(string text)
	{
		UILabel lable = gameObject.GetComponentInChildren<UILabel>();
		if(lable != null)
			lable.text = text;
	}
}