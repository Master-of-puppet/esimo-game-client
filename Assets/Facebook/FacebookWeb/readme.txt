Facebook integration for Unity web player

by using this tool there is an almost immediate connection and access to Facebook’s API

How to use it:

* Before using this tool a facebook app must have been created. Here http://developers.facebook.com
* once the app has been created take note of the “App ID/ API key” given by Facebook.
* Edit the app configuration and choose "Website with Facebook Login", 
	Add “URL site” it is important that the assigned address are the same where the App will be found.
	This is step is very important, facebook will only allow the authentication to be completed if the log is
	since the address give here. Otherwise there will be an error and log in will not be possible

once these steps have been completed we can proceed:
	
	1) Create a project in UNITY
	2) import the package
	3) Switch plataform to "Web Player" (Files > Build settings, select Web Player and press the button "Swith plataform")
	4) In this same screen select the first escene "Example Unity 3x" or "Example Unity 4" according with your unity version.
	5) Open “"Example Unity 3x" or "Example Unity 4" according with your unty version.
	6) Put the "App ID/API Key" in the field “App ID” of the Facebook object
	7) Build the project and publish it.
	
	It is important that the App while be published in the same address that you indicate in Facebook. (http://developers.facebook.com)	
	If you want to work locally it can be published in the locashost of the computer , on Facebook must specify that app is going to access from locashost.	

You can see the video http://youtu.be/2vnpxxHmNow

For support, assistance and bug reports please write to ctellom@hotmail.com
			
3rd party libraries included:
	LitJson - http://litjson.sourceforge.net/
	JSon-js - http://www.json.org/js.html