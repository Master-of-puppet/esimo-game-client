using System;
using System.Collections.Generic;
using UnityEngine;

public class NativeManager : MonoBehaviour
{
    public delegate void DelegateChooseImageDone(Texture2D texture);
    public delegate void DelegateSendSMS();
    #region COMMON
    void Awake()
    {
#if UNITY_ANDROID
       	EtceteraAndroidManager.albumChooserSucceededEvent += OnChooseImageDoneAdnroid;
#elif UNITY_IPHONE
		EtceteraManager.imagePickerChoseImageEvent += OnChooseImageDoneIos;
#endif
    }

    void OnDestroy()
    {
#if UNITY_ANDROID
       	EtceteraAndroidManager.albumChooserSucceededEvent -= OnChooseImageDoneAdnroid;
#elif UNITY_IPHONE
		EtceteraManager.imagePickerChoseImageEvent -= OnChooseImageDoneIos;
#endif
    }
    
    static NativeManager _instance;
    public static NativeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("__Native Code Manager", typeof(NativeManager));
                _instance = obj.GetComponent<NativeManager>();
            }
            return _instance;
        }
    }
    #endregion

    #region CHỌN ẢNH TỪ ALBUM
    private DelegateChooseImageDone onChooseImageDone;
    private DelegateSendSMS onSendSms;
    /// <summary>
    /// Hiện prompt chọn album ảnh từ native
    /// </summary>
    public void PromptPictureFromAlbum(DelegateChooseImageDone whenDone)
    {
        onChooseImageDone = whenDone;

#if UNITY_ANDROID
        EtceteraAndroid.promptForPictureFromAlbum(512, 512, "esimoAvatar.jpg");
#elif UNITY_IPHONE
	    EtceteraBinding.promptForPhoto(0.25f, PhotoPromptType.Album);
#else
        NotificationView.ShowMessage("Hiện chỉ hỗ trợ đổi ảnh trên mobile", 2f);
#endif
    }

    void OnChooseImageDoneAdnroid(string url, Texture2D texture)
    {
        onChooseImageDone(texture);
    }
    void OnChooseImageDoneIos(string url)
    {
#if UNITY_IPHONE
	    StartCoroutine
        (   EtceteraManager.textureFromFileAtPath
            (
                "file://" + url, 
                delegate(Texture2D texture) { onChooseImageDone(texture); }  , 
                delegate(string error) { 
                    NotificationView.ShowMessage("Lỗi cập nhật ảnh", 3f);
                    GameObject.Destroy(gameObject);
            })
        );
#endif
    }
    #endregion

    /// <summary>
    /// Gửi tin nhắn đến một đầu số 
    /// </summary>
	public bool PromptSendMessage(DelegateSendSMS whenDone,string message,string[] recepent) 
    {
        onSendSms = whenDone;
#if UNITY_ANDROID
		EtceteraAndroid.showSMSComposer(message,recepent); 
		return EtceteraAndroid.isSMSComposerAvailable();
#elif UNITY_IPHONE
		if(isIphone())
			EtceteraBinding.showSMSComposer(recepent,message);
		return isIphone();
#else
		return false;
#endif
    }
	#if UNITY_IPHONE
	private bool isIphone() 
	{
		switch (iPhone.generation) 
		{
		case iPhoneGeneration.iPhone:
		case iPhoneGeneration.iPhone3G : 
		case iPhoneGeneration.iPhone3GS :
		case iPhoneGeneration.iPhone4:
		case iPhoneGeneration.iPhone4S:
		case iPhoneGeneration.iPhone5:
		case iPhoneGeneration.iPhone5S:
		case iPhoneGeneration.iPhone5C:
		case iPhoneGeneration.iPhoneUnknown:
			return true;
		default :
			return false;
		}
	}
	#endif
}
