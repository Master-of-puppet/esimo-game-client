using UnityEngine;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;

public class PopupTournamentItem : MonoBehaviour {

    public GameObject tablePrefab;
    public UIGrid gridTable;
    public UILabel lbldecription;
    public CUIHandle btnView;
    [HideInInspector]
    public int gameId = -1;

    void Awake()
    {
        CUIHandle.AddClick(btnView, OnClickView);
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnView, OnClickView);
    }

    void OnClickView(GameObject go)
    {
        GameManager.Server.DoJoinGameTournament(gameId);
    }

    public void SetData(EsObject es)
    {
        if (es.variableExists("description"))
            lbldecription.text = es.getString("description");
        if (es.variableExists("gameId"))
            this.gameId = es.getInteger("gameId");
        if (gameId < 0)
            btnView.gameObject.SetActive(false);
        GameObject obj = (GameObject)GameObject.Instantiate(tablePrefab);
        PopupTable tableitem = obj.GetComponent<PopupTable>();
        obj.transform.parent = gridTable.transform;
        tableitem.transform.localPosition = Vector3.zero;
        tableitem.transform.localScale = Vector3.one;
        tableitem.SetData(es);
        gridTable.Reposition();
    }
}
