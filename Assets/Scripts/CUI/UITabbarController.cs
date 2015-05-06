using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/dungnv/Tabbar Controller")]

/// <summary>
/// Nguyen Viet Dung: UITabbarController
/// </summary>

    public delegate void OnTabbarSelecteCallBack(int index);
public class UITabbarController : MonoBehaviour {
    #region UnityEditor
	public UITabbarButton[] tabbarButtons;
	public UITabbarPanel [] tabbarPanel;
    #endregion
    public event OnTabbarSelecteCallBack OnTabbarSelectEvent;
    public int selectedIndex = 0;
	public bool enable = true;
	public bool isStartDestroyChild = false;
	// Use this for initialization
	public void Start () {

		OnStart();
		if(isStartDestroyChild)
			foreach(UITabbarPanel panel in tabbarPanel)
				for(int z=0;z<panel.transform.childCount;z++)
					GameObject.DestroyImmediate(panel.transform.GetChild(0).gameObject, true);
		
		if(tabbarButtons == null)
		{
			enable = false;
			return;
		}
		
		int i = 0;
		foreach(UITabbarButton bt in tabbarButtons)
		{
			bt.index = i++;
			bt.pressEnable = enable;
		}
		foreach(UITabbarPanel panel in tabbarPanel)
			panel.OnTabbarDeactive();
		
		UITabbarButton.AddPressIn(tabbarButtons,OnSelectTabbar);
		OnSelectTabbar(selectedIndex);
	}
	public void OnSelectTabbar(int index)
	{
		int i = 0;
		foreach(UITabbarButton bt in tabbarButtons)
		{
			if(i == index)
				bt.isSelected = true;
			else
				bt.isSelected = false;
			i++;
		}
        tabbarPanel[selectedIndex].OnTabbarDeactive();
        OnTabbarPanelDeactive(selectedIndex);

        selectedIndex = index;

        tabbarPanel[selectedIndex].OnTabbarActive();
        OnTabbarPanelActive(selectedIndex);

        OnSelectTabbarAtIndex(index);
        if (OnTabbarSelectEvent != null)
            OnTabbarSelectEvent(index);
      
	}
	
	public virtual void OnSelectTabbarAtIndex(int index)
	{
		
	}
	
	public virtual void OnTabbarPanelDeactive(int index)
	{
		
	}
	
	public virtual void OnTabbarPanelActive(int index)
	{
		
	}
	
	public virtual void OnStart() { }
}
