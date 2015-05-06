using System;
public class Google
{
	public string accessToken;
	private static Google _instance = null;

	public static Google instance
	{
		get
		{
			if( _instance == null )
				_instance = new Google();

			return _instance;
		}
	}
}


