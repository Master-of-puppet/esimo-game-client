using System;
using UnityEngine;
using System.Collections;
public class GoogleEventListener : MonoBehaviour
{
	#if UNITY_IPHONE || UNITY_ANDROID
	// Listens to all the events.  All event listeners MUST be removed before this object is disposed!
	void OnEnable()
	{
		GoogleManager.loginSuccessfulEvent += loginSuccessfulEvent;
		GoogleManager.loginFailEvent += loginFailEvent;
		GoogleManager.waitResultEvent += waitResultEvent;
//		GoogleManager.responseAccountsEvent += responseAccount;
	}
	
	
	
	void OnDisable()
	{
		GoogleManager.loginSuccessfulEvent -= loginSuccessfulEvent;
		GoogleManager.loginFailEvent -=loginFailEvent;
		GoogleManager.waitResultEvent -= waitResultEvent;

	}
	void loginSuccessfulEvent(string accessToken)
	{
        
        switch (Application.platform)
        {
            case RuntimePlatform.Android : 
                SocialCommon.Instance.LoginWithAccessToken(SocialCommon.ETypeSocial.google);
                break;
        }
        	    	
	}
	void loginFailEvent()
	{
        
	}
	void waitResultEvent(){
        //testGUI = "Doi ket qua";
		Debug.Log("-----> waitResult");
	}

	#endif
}


