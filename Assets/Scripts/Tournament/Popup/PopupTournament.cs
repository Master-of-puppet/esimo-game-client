using UnityEngine;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;

public class PopupTournament : MonoBehaviour {

    public UILabel lblTitle,lblHistory;
    public CUIHandle btnClose;
    public UITable gridTournament;
    public GameObject popUpItem;

    void Awake()
    {
        CUIHandle.AddClick(btnClose, OnClose);
    }

    void Destroy()
    {
        CUIHandle.RemoveClick(btnClose, OnClose);
    }

    private void OnClose(GameObject targetObject)
    {
        GameObject.Destroy(this.gameObject);
    }

	public static PopupTournament Create(EsObject es)
    {
        GameObject popuptournament = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Tournament/PopupTournamentOverview"));
        popuptournament.name = "___PopupTournamentOverview";
        popuptournament.transform.position = new Vector3(15, 0, 0);
        popuptournament.GetComponent<PopupTournament>().CreateItem(es);
        return popuptournament.GetComponent<PopupTournament>();
    }

    public void CreateItem(EsObject es)
    {
        if (es.variableExists("groupName"))
            this.lblTitle.text = es.getString("groupName");
        EsObject[] esObject = es.getEsObjectArray("matchs");
        for (int i = 0; i < esObject.Length; i++)
        {
            GameObject item = (GameObject)GameObject.Instantiate(popUpItem);
            PopupTournamentItem tournamentitem = item.GetComponent<PopupTournamentItem>();
            item.transform.parent = gridTournament.transform;
            tournamentitem.SetData(esObject[i]);
            item.name = "match" + i;
            tournamentitem.transform.localScale = Vector3.one;
            tournamentitem.transform.localPosition = Vector3.zero;
        }
        gridTournament.Reposition();
    }
}
