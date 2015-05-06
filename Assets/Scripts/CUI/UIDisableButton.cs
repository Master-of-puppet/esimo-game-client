using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Component các button có thêm trạng thái Disable
/// </summary>
public class UIDisableButton : MonoBehaviour
{
    public event CUIHandle.OnClickDelegate onEventClick; 

    public UISprite target;
    public string normalSprite;
    public string hoverSprite;
    public string disableSprite;

    public void SetStatus(bool isActive, bool isEnable)
    {
        if (isActive)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(isActive);

            Collider col = collider;
            col.enabled = isEnable;
            target.spriteName = (!isEnable) ? disableSprite : UICamera.IsHighlighted(gameObject) ? hoverSprite : normalSprite;

        }
        else
            gameObject.SetActive(false);
    }

    void Awake() 
    { 
        if (target == null) target = GetComponentInChildren<UISprite>();
    }

    void OnHover(bool isOver)
    {
        if (enabled && target != null && collider.enabled)
            target.spriteName = isOver ? hoverSprite : normalSprite;
    }

    void OnClick()
    {
        if (timeCountdown > 0) return;

        //if (!IsEnable) return;
        if (enabled && target != null && onEventClick != null)
            onEventClick(gameObject);
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

    public static void AddClick(UIDisableButton button, CUIHandle.OnClickDelegate OnDelegate)
    {
        button.onEventClick += OnDelegate;
    }

    public static void RemoveClick(UIDisableButton button, CUIHandle.OnClickDelegate OnDelegate)
    {
        button.onEventClick -= OnDelegate;
    }

    public void SetText(string text)
    {
        gameObject.transform.GetComponentInChildren<UILabel>().text = text;
    }
}

