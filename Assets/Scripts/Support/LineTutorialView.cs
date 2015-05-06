using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTutorialView : MonoBehaviour
{
    #region Unity Editor
    public UILabel title;
    string titlemesseage,contentlink;
    #endregion
    public static List<LineTutorialView> ListTutorialView = new List<LineTutorialView>();
    void Awake()
    {
        CUIHandle.AddClick(gameObject.GetComponent<CUIHandle>(), OnClickMe);
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(gameObject.GetComponent<CUIHandle>(), OnClickMe);
    }

    void OnClickMe(GameObject go)
    {
        //LineDetailsTutorialView.Create(content);
        #if UNITY_ANDROID || UNITY_IPHONE || UNITY_EDITOR 
            EventView.Create(titlemesseage, contentlink);
        #endif
        #if UNITY_WEBPLAYER
            Application.OpenURL(contentlink);
        #endif
    }
    Hashtable content;

    public static LineTutorialView Create(Hashtable content, Transform parent)
    {
        GameObject line = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/Support/LineTutorialPrefab"));
        line.name = "Line Support";
        line.transform.parent = parent;
        line.transform.localPosition = Vector3.zero;
        line.transform.localScale = Vector3.one;
        line.GetComponent<LineTutorialView>().SetData(content);
        ListTutorialView.Add(line.GetComponent<LineTutorialView>());
        return line.GetComponent<LineTutorialView>();
    }

    void SetData(Hashtable content)
    {
        this.content = content;
        title.text = content["title"].ToString();
        titlemesseage = content["title"].ToString();
        contentlink = content["content"].ToString();
        //content
    }
}
