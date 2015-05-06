using UnityEngine;


[AddComponentMenu("NGUI/dungnv/UIButtonTouch")]
/// <summary>
/// Events.
/// Author: Nguyen Viet Dung
/// Date  : 15/05/2013
/// Xử lý các touch trên cảm ứng
/// </summary>
public class UIButtonTouch : MonoBehaviour
{
    public Transform tweenTarget;
    public Vector3 hover = new Vector3(0.95f, 0.95f, 0.95f);
    public Vector3 pressed = new Vector3(1f, 1f, 1f);
    public float duration = 0.2f;

    Vector3 mScale;
    bool mStarted = false;
    bool mHighlighted = false;

    void Start()
    {
        if (!mStarted)
        {
            mStarted = true;
            if (tweenTarget == null) tweenTarget = transform;
            mScale = tweenTarget.localScale;
        }
    }

    void OnEnable() { if (mStarted && mHighlighted) OnHover(UICamera.IsHighlighted(gameObject)); }

    void OnDisable()
    {
        if (mStarted && tweenTarget != null)
        {
            TweenScale tc = tweenTarget.GetComponent<TweenScale>();

            if (tc != null)
            {
                tc.value = mScale;
                tc.enabled = false;
            }
        }
    }

    void OnPress(bool isPressed)
    {
        if (enabled)
        {
            if (!mStarted) Start();
            TweenScale.Begin(tweenTarget.gameObject, duration, isPressed ? Vector3.Scale(mScale, pressed) :
                (UICamera.IsHighlighted(gameObject) ? Vector3.Scale(mScale, hover) : mScale)).method = UITweener.Method.EaseInOut;
        }
    }

    void OnHover(bool isOver)
    {
        if (enabled)
        {
            if (!mStarted) Start();
            TweenScale.Begin(tweenTarget.gameObject, duration, isOver ? Vector3.Scale(mScale, hover) : mScale).method = UITweener.Method.EaseInOut;
            mHighlighted = isOver;
        }
    }

    void OnClick()
    {
        OnTouchMobile();
        AudioManager.Instance.Play(AudioManager.SoundEffect.Button);
    }

    void OnTouchMobile()
    {
        if(Application.platform == RuntimePlatform.Android 
            || Application.platform == RuntimePlatform.IPhonePlayer)
            StartCoroutine(_OnTouchMobile());
    }

    System.Collections.IEnumerator _OnTouchMobile()
    {
        if (tweenTarget.gameObject != null)
            TweenScale.Begin(tweenTarget.gameObject, duration, Vector3.Scale(mScale, hover)).method = UITweener.Method.EaseInOut;

        yield return new WaitForSeconds(duration);

        if(tweenTarget.gameObject != null)
            TweenScale.Begin(tweenTarget.gameObject, duration, mScale).method = UITweener.Method.EaseInOut;
    }
}
