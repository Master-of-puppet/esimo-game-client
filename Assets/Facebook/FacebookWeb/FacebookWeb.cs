using UnityEngine;
using LitJson;
using System.Collections;

/// In this Script, You can find help in English and Spanish
public class FacebookWeb : MonoBehaviour
{

    //Archivo con las funciones del API de Facebook
    //Java script File with the API'S Facebook Functions
    public TextAsset FBjavascriptfile;
    
    //Extern file Json
    //Archivo extern Json
	public TextAsset jsJson;	
	public string appID ;
	
    //Id de la aplicacion proporcionado por Facebook
    //App ID provided by Facebook
	#if UNITY_WEBPLAYER

    //Token de sesion proporcionado por la Graph APi
    //Token session provided by Graph API
	public static string Token;

    //DELEGATES - DELEGADO
    public delegate void FacebookEventDelegate(bool resultado, JsonData respuesta);
	
    //Events
	public static event FacebookEventDelegate onLogin;	
	public static event FacebookEventDelegate onLogout;	
	public static event FacebookEventDelegate onGetLoginStatus;	
	public static event FacebookEventDelegate onPostFeed;	
	public static event FacebookEventDelegate onAppRequest;	
	public static event FacebookEventDelegate onFeedRequest;	
	public static event FacebookEventDelegate onGraphRequest;
	
	
	public delegate void FacebookEventDelegatedos(bool resultado, string status);
	public static event FacebookEventDelegatedos onGetLoginStatusdos;	
	
	
    #region FACEBOOK METHODS

    /// Function for the login and Facebook authorization
    /// input the necessary permissions separated by “,”  
    /// for permissions information visit
    /// https://developers.facebook.com/docs/authentication/permissions/
    /// once the authorization is made, Facebook assign a token that validates the app permissions, 
    /// which is captured by the fuction defineToken() in the region "EVENTS FOR RESPONSE Facebook API"
    
    /// Funcion para el login y autorizacion de Facebook    
    /// ingrese los permisos necesarios separados por ","    
    /// para informacion de los permisos consulte    
    /// https://developers.facebook.com/docs/authentication/permissions/    
    /// una Vez se realice la autorizacion, Facebook asigna un token que valida los permisos de la aplicación,
    /// el cual es capturado por la funcion defineToken() de la region "EVENTS FOR RESPONSE Facebook API"   


    public static void login(string permisos)
    {
        Debug.Log("Application.ExternalCall(login, permisos);");
        Application.ExternalCall("login", permisos);
    }

    //Function for facebook logout
    public static void logout()
    {
        Application.ExternalCall("logout");
    }

    ///Function get conexion status    
    ///funcion para obtener el estatus de conexion    
    ///https://developers.facebook.com/docs/reference/javascript/FB.getLoginStatus/
    public static void getLoginStatus()
    {
        Application.ExternalCall("getLoginStatus");
    }
	
	public static void getLoginStatusdos()
    {
        Application.ExternalCall("getLoginStatusdos");
    }

    /// Function to read Facebook information. Through Graph API.
    /// the queries are made to he URL https://graph.facebooassignedk.com followed by the querystring
    /// and the token assigned.
    /// Para conocer las diferentes posiblidades de consulta puede usar el Graph API Explorer
    /// https://developers.facebook.com/tools/explorer     

    /// Funcion utilizada para obtener informacion de Facebook. Mediante el uso de la Graph API.
    /// Las consultas se realizan a la url https://graph.facebook.com seguida de la cadena
    /// de consulta y el token asignado.
    /// To know other ways to consult use the Graph API explorer
    /// https://developers.facebook.com/tools/explorer     
    public static string getInfo(string text)
    {
        return "https://graph.facebook.com" + text + "&access_token=" + Token;
    }


    ///function to make a post in a wall
    ///The post are done without a request.

    ///funcion usada para hacer post en el muro deseado
    ///Los post son realizados directamente sin consulta previa.    
    public static void postear(string to, string message, string name, string description, string picture, string caption, string link)
    {
        string final = "/" + to + "/feed";
        Application.ExternalCall("postear", final, message, name, description, picture, caption, link);
    }
	
	public static void postearv(string to, string message, string name, string description, string picture, string caption, string source, string link)
    {
        string final = "/" + to + "/feed";
        Application.ExternalCall("postearv", final, message, name, description, picture, caption, source, link);
    }
	
	public static void photo(string message, string link)
    {
        Application.ExternalCall("photo", message, link);
    }
	   

    /// Functions to get Facebook information, 
    /// Used in the "Example" in the button "User Information"
    /// We recommend getInfo() 

    /// Funciones para obtener informacion de Facebook, 
    /// Usada en el "Example" mediante el boton "User Information"
    /// Se recomiendo el uso de la funcion getInfo()
    public static void userGraphRequest( string _user, string _request )
	{
		graphRequest( _user + _request );
	}		
	public static void graphRequest( string _request )
	{
		Application.ExternalCall( "graphRequest", _request );
	}

    /// Funcion used to send invitations, emerge a pop pop to select recipients
    /// Funcion usada para enviar invitaciones, genera una ventana emergente donde se seleccionan los destinatarios.		
	public static void uiAppRequest( string title, string message )
	{
		Application.ExternalCall( "uiAppRequest", title,  message );
	}

    // Function to generate a post with pop up confirmation
    // Funcion para generar post con pop up de confirmacion
	public static void uiFeedRequest( string link, string picture, string name, string caption, string description )
	{
		Application.ExternalCall( "uiFeedRequest", link, picture, name, caption, description );
	}

    ///function used to generate a post without request window.
    /// We recommend function postear()
    ///funcion usada para hacer post directamente sin consulta previa.
    /// se recomienda la funcion postear()
	public static void postFeed( string message, string picture, string link )
	{
		Application.ExternalCall( "postFeed", message, picture, link );
	}
	#endregion

    #region EVENTS FOR RESPONSE Facebook API

    /// Functions call by the Facebook js API
    /// to rollback the answers.

    /// Funciones llamadas por el API js de Facebook 
    /// mediante las cuales regresan las respuestas.
    
    void defineToken(string token)
	{
		Token = token;
		FB.AccessToken = token;
        SocialCommon.Instance.LoginWithAccessToken(SocialCommon.ETypeSocial.facebook);        
	}

    void onLoginExitoso(string message)
    {
        WaitingView.Show("Đang đăng nhập");
        if (onLogin != null)
            onLogin(true, JsonMapper.ToObject(message));
    }

    void onLoginFallido(string message)
    {
        Debug.Log("onLoginFallido " + message);
        if (onLogin != null)
            onLogin(false, JsonMapper.ToObject(message));
    }

    void onLogoutExitoso(string message)
    {
        if (onLogout != null)
            onLogout(true, JsonMapper.ToObject(message));
    }

    void onLogoutFallido(string message)
    {
        if (onLogout != null)
            onLogout(false, JsonMapper.ToObject(message));
    }

    void onGetLoginStatusExitoso(string message)
    {
        if (onGetLoginStatus != null)
            onGetLoginStatus(true, JsonMapper.ToObject(message));
    }

    void onGetLoginStatusFallido(string message)
    {
        if (onGetLoginStatus != null)
            onGetLoginStatus(false, JsonMapper.ToObject(message));
    }
	
	//
	void onGetLoginStatusconnected(string message)
    {
        if (onGetLoginStatusdos != null)
            onGetLoginStatusdos(true, "connected");
    }
	
	void onGetLoginStatusnot_authorized(string message)
    {
        if (onGetLoginStatusdos != null)
            onGetLoginStatusdos(true, "not_authorized");
    }
	
	void onGetLoginStatusnotlogged(string message)
    {
        if (onGetLoginStatusdos != null)
            onGetLoginStatusdos(true, "notlogged");
    }
	//

    void onPostFeedExitoso(string message)
    {
        if (onPostFeed != null)
            onPostFeed(true, JsonMapper.ToObject(message));
    }

    void onPostFeedFallido(string message)
    {
        if (onFeedRequest != null)
            onFeedRequest(false, JsonMapper.ToObject(message));
    }

    void onUiAppRequestExitoso(string message)
    {
        if (onAppRequest != null)
            onAppRequest(true, JsonMapper.ToObject(message));
    }

    void onUiAppRequestFallido(string message)
    {
        if (onAppRequest != null)
            onAppRequest(false, JsonMapper.ToObject(message));
    }

    void onUiFeedRequestExitoso(string message)
    {
        if (onFeedRequest != null)
            onFeedRequest(true, JsonMapper.ToObject(message));
    }

    void onUiFeedRequestFallido(string message)
    {
        if (onFeedRequest != null)
            onFeedRequest(false, JsonMapper.ToObject(message));
    }

    void onGraphRequestExitoso(string message)
	{
		if( onGraphRequest != null )
			onGraphRequest( true, JsonMapper.ToObject (message) );		
	}

    void onGraphRequestFallido(string message)
	{
		if( onGraphRequest != null )
			onGraphRequest( false, JsonMapper.ToObject (message) );		
	}    
	#endregion	
    
    #region START INICIO
    void Start()
    {
        /// Initial function to update the App ID
        /// in the Facebook js API
        /// Funcion para actualizar el id de la aplicacion
        /// en el API js de Facebook
    #if UNITY_WEBPLAYER
        Application.ExternalEval(jsJson.text);
        Application.ExternalEval(FBjavascriptfile.text.Replace("%applicationID%", appID));
    #endif
    }
    #endregion
#endif
}
