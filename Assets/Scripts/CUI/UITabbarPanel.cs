using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Nguyen Viet Dung: Tabbar Panel.
/// </summary>
public class UITabbarPanel : MonoBehaviour
{
	List<KeyValuePair<GameObject, bool>> list = new List<KeyValuePair<GameObject, bool>>();
	
	public void OnTabbarActive()
	{
		gameObject.SetActive(true);
		RemoveList(gameObject);
	}
	public void OnTabbarDeactive()
	{
		list = new List<KeyValuePair<GameObject, bool>>();
		gameObject.SetActive(false);
		AddList(gameObject);
	}
	
	void AddList(GameObject go)
	{
		for(int i = 0 ; i < go.transform.childCount; i++)
		{
			GameObject obj = go.transform.GetChild(i).gameObject;
			list.Add(new KeyValuePair<GameObject, bool>(obj, obj.activeSelf));
			AddList(obj);
		}
	}
	
	void RemoveList(GameObject go)
	{
		foreach(KeyValuePair<GameObject, bool> _value in list)
			if(_value.Key != null)
				_value.Key.SetActive(_value.Value);	
		
		list.Clear();
	}
	
	
	
}

