using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CuocItemView : MonoBehaviour
{
	public UILabel lbCuocName;
    public UISprite background;
    public UISprite backCuocItem;
    public UILabel lblNumber;
    public UISprite uisBg;

    [HideInInspector]
    public CuocUXuong cuoc;
    [HideInInspector]
    ListCuocUView cuocView;
    
    void Awake()
    {
        backCuocItem.gameObject.SetActive(false);
        uisBg.gameObject.SetActive(false);
    }

    public void setData(CuocUXuong item, ListCuocUView cuocView)
	{
		this.cuoc = item;
		lbCuocName.text = cuoc.name;
        this.cuocView = cuocView;
	}

	void OnClick ()
	{
		cuoc.countClick++;
        cuoc.countClick = cuoc.countClick > cuoc.max ? 0 : cuoc.countClick;
        lblNumber.text = cuoc.max > 1 && cuoc.countClick > 0 ? "x" + cuoc.countClick.ToString() : "";
        uisBg.gameObject.SetActive(cuoc.max > 1 && cuoc.countClick > 0);
        backCuocItem.gameObject.SetActive(cuoc.countClick > 0);

        SetChildDisable();
        cuocView.listCuocXuong();
	}

    public void SetChildDisable()
    {
        cuocView.listItem.FindAll(i => cuoc.blackList.Contains(i.cuoc.id)).ForEach(i => i.BlackList(cuoc.countClick > 0));
    }

    void BlackList(bool isDisable)
    {
        lbCuocName.color = !isDisable ? colorNormal : colorDisable;
        gameObject.collider.enabled = !isDisable;
    } 
    Color colorNormal = new Color(255 / 255f, 153 / 255f, 255 / 255f);
    Color colorDisable = new Color(120 / 255f, 120 / 255f, 120 / 255f);
}

public struct CuocUXuong
{
    public int id;
    public string name;
    public int point;
    public int subPoint;
    public int max;
    public int countClick;
    public List<int> blackList;

    public CuocUXuong(int id, string name, int point, int subPoint, int maxClick, int [] blackList)
    {
        this.id = id;
        this.name = name;
        this.point = point;
        this.subPoint = subPoint;
        this.max = maxClick;
        this.countClick = 0;
        if(blackList == null)
            blackList = new int[0];
        this.blackList = new List<int>(blackList);
    }
}

