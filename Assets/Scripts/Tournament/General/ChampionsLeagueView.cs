using UnityEngine;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;

public class ChampionsLeagueView : MonoBehaviour {

    List<TableChampion> table = new List<TableChampion>();
    public UITextList textNews;

    void Awake()
    {
        GameManager.Server.EventPluginMessageOnProcess += OnProcessPluginMessage;
    }

    void OnDestroy()
    {
        GameManager.Server.EventPluginMessageOnProcess -= OnProcessPluginMessage;
    }

    private void OnProcessPluginMessage(string command, string action, EsObject parameters)
    {
        if (command == Fields.REQUEST.COMMAND_GETGENERAL)
        {
            EsObject[] esObject = parameters.getEsObjectArray("users");
            for (int i = 0; i < esObject.Length; i++)
            {

            }
        }
    }

	// Use this for initialization
	void Start () {
        GameManager.Server.DoRequestCommand(Fields.REQUEST.COMMAND_GETGENERAL);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
