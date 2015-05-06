using UnityEngine;
using System.Collections;



public class Wifi : MonoBehaviour
{

    /// <summary>
    /// Hoang Viet Duc
    /// Class hiện độ mạnh yếu của mạng thông qua việc ping đến server
    /// </summary>
    public UISprite statusWifi;
    private Ping pinger;

    static Wifi _instance;

    private float screenWidth;
    private float screenHeight;

    bool IsSaveFileLog = false;
    bool dialogIsShowing = false;

    private string WIFI_EXCELLENT = "excellent";
    private string WIFI_NORMAL = "normal";
    private string WIFI_LOW = "low";
    private string WIFI_DISCONNECT = "disconnect";

    /*public static Wifi Instance{
        get{
            if(_instance==null){
                GameObject goWifi = GameObject.Find("__WifiPrefab");

                if(goWifi==null){
                    goWifi = (GameObject)GameObject.Instantiate(Resources.Load ("Prefabs/Ping/WifiPrefab"));
                    goWifi.name = "__WifiPrefab";
                    goWifi.transform.position = new Vector3 (-10000,300,0);
                }
                _instance = goWifi.gameObject.GetComponent<Wifi>();
                _instance.Init ();
            }
            return _instance;
        }
    }*/

    void Init()
    {


    }
    void Start()
    {

        statusWifi.spriteName = WIFI_NORMAL;
        if (CServer.HOST_NAME != "")
            StartCoroutine(checkConnection(CServer.HOST_NAME));
    }

    public void ShowWifi()
    {

    }

    IEnumerator checkConnection(string ipAddress)
    {
        pinger = new Ping(ipAddress);
        //Debug.LogWarning("pos: " + this.transform.localPosition);
        yield return new WaitForSeconds(5f);
        //Debug.LogWarning(pinger.time);
        if (!pinger.isDone)
        {
            statusWifi.spriteName = WIFI_DISCONNECT;
            StoreGame.SaveLog(StoreGame.EType.DEBUG_LOG, "==========================[Ping time out]================================");
            //if (!Common.IsRelease)
            //{
            //    if (IsSaveFileLog == false)
            //        SaveAndLoadFile.Save(StoreGame.LoadString(StoreGame.EType.DEBUG_LOG));//SAVE DEBUG_LOG TO FILE
            //}
            //else
            //{
            //    StoreGame.SaveString(StoreGame.EType.BOOL_SEND_LOG_TO_SERVER, "true");
            //}

            StartCoroutine(checkConnection(ipAddress));
            IsSaveFileLog = true;
        }
        else
        {
            if (pinger.time < 40) statusWifi.spriteName = WIFI_EXCELLENT;
            if (pinger.time > 40 && pinger.time < 100) statusWifi.spriteName = WIFI_NORMAL;
            if (pinger.time > 100) statusWifi.spriteName = WIFI_LOW;
            StartCoroutine(checkConnection(ipAddress));
        }
    }

}
