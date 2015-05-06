using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ListResultXuongView : MonoBehaviour
{
    public UITable panelListResult;
    public UITable panelListText;
    public UILabel resultXuongCorrect, resultXuongIncorrect, resultPoint,reultInfoFullaying;

    static ListResultXuongView _instance;
    public static ListResultXuongView Create(string correct, string incorrect, int point,string textGa, Electrotank.Electroserver5.Api.EsObject [] listObj,string infoFullaying)
    {
        if (_instance == null)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/CuocU/ListResultXuongPrefab"));
            obj.name = "__ListResult";
            obj.transform.position = new Vector3(3, 68, -129f);
            _instance = obj.GetComponent<ListResultXuongView>();
            _instance.Init(correct, incorrect, point,textGa, listObj, infoFullaying);
			GameModelChan.HideAvatarPlayer(true);
        }
        return _instance;
    }
	
    public static void Close()
    {
        if (_instance != null)
		{
			GameModelChan.game.ShowAllOtherObjectWhenFullLaying();
			GameModelChan.ShowAvatarPlayer();
            GameObject.Destroy(_instance.gameObject);
		}
    }
    private string splitMessageResultsIncorrect(string msg)
    {
        string[] arrayIncorrect = msg.Split('.');
        string textIncorrect = "";
        Array.ForEach(arrayIncorrect, arrI => textIncorrect += "\n" + arrI);
        return textIncorrect;
    }
    void Init(string correct, string incorrect, int point , string textGa, Electrotank.Electroserver5.Api.EsObject[] listObj,string infoFullaying)
    {
        resultXuongCorrect.text = "[FFCC00]" + correct + "[-]\n\n";
        resultXuongCorrect.text += "[FFFFFF]Tổng điểm: " + point + (string.IsNullOrEmpty(textGa) ? "" : " - " + textGa) + "[-]\n\n";
        resultXuongCorrect.text += string.IsNullOrEmpty(incorrect) ? "" : "[FF9900]" + splitMessageResultsIncorrect(incorrect) + "[-]\n\n";
        resultXuongCorrect.text += "[00CC00]" + infoFullaying + "[-]";

        int i = 0;
        foreach (Electrotank.Electroserver5.Api.EsObject obj in listObj)
        {
            i++;
            long money = long.Parse(obj.getString("moneyExchange")); 
            ResultXuongItemView item = ResultXuongItemView.Create(i, panelListResult.transform, obj);
            if (money > 0)
                item.gameObject.name = "__0";
        }

		panelListText.Reposition ();
		panelListText.transform.parent.GetComponent<UIScrollView> ().ResetPosition ();
		panelListResult.Reposition ();
    }

    public static bool IsShowing
    {
        get { return _instance != null; }
    }
}
