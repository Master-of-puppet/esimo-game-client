using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDetailsTutorialView : MonoBehaviour
{
    #region Unity Editor
    public UILabel lbDetails;
	public UILabel lbTitle;
    public CUIHandle btClose;
    #endregion

    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickBack;

        CUIHandle.AddClick(btClose, OnClickBack);
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(btClose, OnClickBack);
    }

    void OnClickBack(GameObject go)
    {
        GameObject.Destroy(gameObject);
    }

    public static LineDetailsTutorialView Create(Hashtable content)
    {
        GameObject setting = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/Support/LineDetialTutorialPrefab"));
        setting.name = "__Line Detail Tutorial Prefab";
        setting.transform.position = new Vector3(-101, 240, -1208f);
        setting.GetComponent<LineDetailsTutorialView>().SetData(content);
        return setting.GetComponent<LineDetailsTutorialView>();
    }

    void SetData(Hashtable content)
    {
        lbTitle.text = content["title"].ToString();
        lbDetails.text = content["content"].ToString().Replace("\n","\n\n");
    }

}
