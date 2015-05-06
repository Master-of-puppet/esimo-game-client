using UnityEngine;
using System.Collections;

public class PrefabMessageSystem : MonoBehaviour
{

    #region Unity Editor
    public UILabel txtContent, txtTime;
    public UISprite line;
    #endregion
    [HideInInspector]
    public Messages message;
    public static PrefabMessageSystem Create(Messages message , UITable parent,UIAnchor[] leftRight) 
    {
        GameObject row = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/Message/ProfileMessageSystemPrefab"));
        row.transform.parent = parent.transform;
        row.transform.localPosition = Vector3.zero;
        row.transform.localScale = Vector3.one;
        PrefabMessageSystem item = row.GetComponent<PrefabMessageSystem>();
        item.SetData(message);

        return item;
    }
    
    public void SetData(Messages message)
    {
        this.message = message;
        txtTime.text = Utility.Convert.MessageTime(message.time_sent);
        txtContent.text = message.content;
        line.leftAnchor.target = transform.parent.parent;
        line.leftAnchor.absolute = 20;
        line.rightAnchor.target = transform.parent.parent;
        line.rightAnchor.absolute = -10;
        line.ResetAnchors();
        line.UpdateAnchors();
        line.MakePixelPerfect();

    }
    
}
