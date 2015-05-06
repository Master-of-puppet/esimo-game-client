using UnityEngine;
using System.Collections;
using System;
using Prime31;

public class GoogleManager : AbstractManager {
	#if UNITY_ANDROID || UNITY_IPHONE
	public static event Action<string> loginSuccessfulEvent;
	public static event Action loginFailEvent;
	public static event Action waitResultEvent;
//	public static event Action<string> responseAccountsEvent;

	static GoogleManager(){
		AbstractManager.initialize( typeof( GoogleManager ) );
	}
	public void loginSuccessful(string accessToken){
		Google.instance.accessToken = accessToken;
		loginSuccessfulEvent.fire(accessToken);
	}
	public void loginFail(){
		loginFailEvent.fire();	
	}
	public void waitResult()
	{
		waitResultEvent.fire();
	}
//	public void getAccounts(string accounts)
//	{
//		responseAccountsEvent.fire(accounts);
//	}
	
	#endif
}
