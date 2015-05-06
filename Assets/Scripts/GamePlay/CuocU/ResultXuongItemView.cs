using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Electrotank.Electroserver5.Api;

public class ResultXuongItemView : MonoBehaviour
{
    public UILabel playerName;
    public UILabel money;
    public UISprite lineBottom, lineTop;

    void Start()
    {
        ReposionComponent();
    }
    public static ResultXuongItemView Create(int index, Transform parent, EsObject eso)
    {

        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/CuocU/ResultMoneyItemPrefab"));
        obj.name = string.Format("Result_{0:0000}", index);
        obj.transform.parent = parent;
        obj.transform.localPosition = new Vector3(0f, 0f, -1f);
        obj.transform.localScale = Vector3.one;
        ResultXuongItemView resultXuong = obj.GetComponent<ResultXuongItemView>();
        long money = long.Parse(eso.getString("moneyExchange"));
        string userName = eso.getString("userName");
        resultXuong.playerName.text = userName;
        resultXuong.money.color = (money < 0) ? new Color(255f / 255f, 204f / 255f, 0f) : new Color(0f / 255f, 255 / 255f, 0f);
        string summary = eso.getString("description");
        //List<List<Gameplay.Summary>> groupedSummaryByAction = GameModel.game.summaryGame.GroupBy(sum => sum.action).Select(grp => grp.ToList()).ToList();
        //string summary = "";
        if (money >= 0)
        {
            //foreach (List<Gameplay.Summary> lstSum in groupedSummaryByAction)
            //{
            //    string description = "";
            //    long moneyExchage = 0;
            //    foreach (Gameplay.Summary sum in lstSum)
            //    {
            //        if (string.IsNullOrEmpty(description))
            //        {
            //            if (sum.action == Gameplay.Summary.EAction.LOSS_FINISH_TYPE)
            //                description = "Thắng" + " : ";
            //            else
            //                description = sum.description + " : ";
            //        }
            //        moneyExchage += sum.moneyExchange;
            //    }
            //    summary += description + moneyExchage + "\n";
            //}
            summary = summary + "Tổng : " + Utility.Convert.Chip(System.Math.Abs(money));
        }
        else
        {
            //foreach (List<Gameplay.Summary> lstSum in groupedSummaryByAction)
            //{
            //    Gameplay.Summary sum = lstSum.Find(s => s.sourcePlayer == userName);
            //    if (sum != null)
            //        summary += sum.description + " : -" + sum.moneyExchange + "\n";
            //}
            summary = summary + "Tổng : -" + Utility.Convert.Chip(System.Math.Abs(money));
        }
        resultXuong.money.text = summary;
        resultXuong.ReposionComponent();
        return resultXuong;
    }
    public void ReposionComponent()
    {
        float heightMoney = money.height;
        float heightUserName = playerName.height;
        playerName.transform.localPosition = new Vector3(playerName.transform.localPosition.x, heightMoney / 2 + heightUserName / 2 + 10, playerName.transform.localPosition.z);
        lineBottom.transform.localPosition = new Vector3(lineBottom.transform.localPosition.x, -heightMoney / 2 - 20, lineBottom.transform.localPosition.z);
        lineTop.transform.localPosition = new Vector3(lineTop.transform.localPosition.x, heightMoney / 2 + heightUserName / 2 + 20, lineTop.transform.localPosition.z);
    }
}
