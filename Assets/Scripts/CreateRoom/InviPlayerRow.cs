using System;
using System.Collections.Generic;
using UnityEngine;

public class InviPlayerRow : MonoBehaviour
{
    #region Unity Editor
    public UILabel lbName, lbMoney, lbLevel;
    #endregion
    public UIToggle checkbox;

    public static InviPlayerRow Create(int index, Transform parent, User user)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/CreateRoom/InviPlayerPrefab"));
        obj.name = string.Format("Invi_{0:0000}", index);
        obj.transform.parent = parent;
        obj.transform.localPosition = new Vector3(0f, 0f, -1f);
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<InviPlayerRow>().SetData(user);
        return obj.GetComponent<InviPlayerRow>();
    }

    public void SetData(User user)
    {
        if(checkbox == null)
            checkbox = GetComponent<UIToggle>();
        lbName.text = user.username;
        lbMoney.text = string.Format("{0:0,0}", user.chip).Replace(",",".");
        lbLevel.text = "Level " + user.level;
    }
}
