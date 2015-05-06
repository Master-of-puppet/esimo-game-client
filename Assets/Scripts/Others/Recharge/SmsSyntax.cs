using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmsSyntax : MonoBehaviour
{
    public UISprite border, iconSpace;
    public UILabel label;
    public static List<SmsSyntax> ListSmsSyntax = new List<SmsSyntax>();
    public static SmsSyntax Create(string text,UITable table,string name,bool isSpaceHide)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/Recharge/smsSyntax"));
        obj.name = name;
        obj.transform.parent = table.transform;
        obj.transform.localPosition = new Vector3(0f, 0f, -1f);
        obj.transform.localScale = Vector3.one;
        SmsSyntax syntax = obj.GetComponent<SmsSyntax>();

        syntax.SetData(text,isSpaceHide);
        int length = syntax.label.text.Length;
        float scaleLable = syntax.label.transform.localScale.x - 4f;
        SmsSyntax.ListSmsSyntax.Add(syntax);
        return syntax;
    }
    public void SetData(string data,bool isSpaceHide){
        if (isSpaceHide)
            iconSpace.gameObject.SetActive(false);
        label.text = data;
		iconSpace.transform.localPosition = new Vector3(label.width / 2 + iconSpace.width / 2 + 5f, iconSpace.transform.localPosition.y, iconSpace.transform.localPosition.z);
    }   
}
