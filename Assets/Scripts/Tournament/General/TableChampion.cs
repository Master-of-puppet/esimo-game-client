using UnityEngine;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;

public class TableChampion : MonoBehaviour {

    [HideInInspector]
    public int tableId;
    [HideInInspector]
    public bool isPlaying;
    public UILabel tableName;
    public List<UserChampion> userInTable = new List<UserChampion>();
    public UISprite stateTable;
	void Awake () {
        CUIHandle.AddClick(GetComponent<CUIHandle>(), OnShowDetail);
	}
	
	void OnDestroy () {
        CUIHandle.RemoveClick(GetComponent<CUIHandle>(), OnShowDetail);
	}

    void OnShowDetail(GameObject obj)
    {
        if(tableId>0)
        {
            WaitingView.Show("Vui lòng đợi");
            GameManager.Server.DoRequestPlugin(Utility.SetEsObject(Fields.REQUEST.COMMAND_MATCH_LIST,
           new object[] { "groupId", tableId, "tournamentId", GameManager.Instance.currentTournamentInfo.tournamentId }));
        }
    }
    public void SetData(EsObject es)
    {
        if (es.variableExists("id"))
            this.tableId = es.getInteger("id");
        if (es.variableExists("name"))
            tableName.text = es.getString("name");
        else
            tableName.text = "";
        EsObject[] players = es.getEsObjectArray("players");
        for (int i = 0; i < players.Length; i++)
        {
            userInTable[i].SetData(players[i]);
        }
    }
}
