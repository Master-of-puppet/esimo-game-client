using UnityEngine;
using System.Collections.Generic;
using Electrotank.Electroserver5.Api;

public class PopupTable : MonoBehaviour {

    public UILabel lblTableName, lblResult;
    public CUIHandle btnView;
    public GameObject popupUserPrefab;
    public UIGrid gridUser;
    [HideInInspector]
    public int gameId = -1;

    void Awake()
    {
        CUIHandle.AddClick(btnView, OnView);
    }

    void Destroy()
    {
        CUIHandle.RemoveClick(btnView, OnView);
    }

    void OnView(GameObject obj)
    {

    }

    public void SetData(EsObject es)
    {
        if (es.variableExists("result"))
            lblResult.text = "Kết quả: " + es.getString("result");
        if (es.variableExists("gameId"))
            gameId = es.getInteger("gameId");
        if (gameId <= 0)
        {
            //disable button view
        }
        if (es.variableExists("players"))
        {
            EsObject[] esObject = es.getEsObjectArray("players");
            for (int i = 0; i < esObject.Length; i++)
            {
                GameObject obj = (GameObject)GameObject.Instantiate(popupUserPrefab);
                PopupUser user = obj.GetComponent<PopupUser>();
                user.SetData(esObject[i]);
                obj.transform.parent = gridUser.transform;
            }
        }
        gridUser.Reposition();
    }
	
}
