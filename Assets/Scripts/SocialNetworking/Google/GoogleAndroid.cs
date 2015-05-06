using System;
using UnityEngine;

public class GoogleAndroid
{
	#if UNITY_ANDROID
	private static AndroidJavaObject _googlePlugin;
	
	static GoogleAndroid()
	{
		if( Application.platform != RuntimePlatform.Android )
			return;
		
		// find the plugin instance
		using( var pluginClass = new AndroidJavaClass( "com.esimo.plugingoogleplayservices.GooglePlugin" ) )
			_googlePlugin = pluginClass.CallStatic<AndroidJavaObject>( "getInstance" );
		
	}
	public static void addOtherUser(){
		if( Application.platform != RuntimePlatform.Android )
			return;
		_googlePlugin.Call( "AddOtherAccounts" );
	}
	
	public static string[] getAllUser()
	{
		if( Application.platform != RuntimePlatform.Android )
			return new string [] {""};
		
		return _googlePlugin.Call<string[]>( "getAccountNames" );
	}
	public static void login(string email)
	{
		if( Application.platform != RuntimePlatform.Android )
			return;
		_googlePlugin.Call("login",email);
	}
    #else
    public static void addOtherUser()
    {
        Debug.LogWarning("Platform not support");
    }

    public static void login(string email)
    {
        Debug.LogWarning("Platform not support");
    }

    public static string[] getAllUser()
    {
        Debug.LogWarning("Platform not support");
        return new string[] { };
    }
	#endif

}

