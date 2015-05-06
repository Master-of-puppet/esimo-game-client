using UnityEngine;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;

public class RoundChampion : MonoBehaviour {

    public List<TableChampion> tableChampion = new List<TableChampion>();
    public UILabel lblNews;
    [HideInInspector]
    public int roundId;
    [HideInInspector]
    public string roundName;
	public void SetData(EsObject es)
    {
        if (es.variableExists("id"))
            this.roundId = es.getInteger("id");
        if (es.variableExists("name"))
            this.roundName = es.getString("name");
        EsObject[] tables = es.getEsObjectArray("groups");
        for (int i = 0; i < tables.Length; i++)
        {
            tableChampion[i].SetData(tables[i]);
        }
    }
}
