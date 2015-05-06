using System;
using System.Collections.Generic;
using UnityEngine;

public class GameplayFullLaying
{
    List<GameObject> ListGameObjectFulllaying = new List<GameObject>();

    public void Destroy()
    {
        while (ListGameObjectFulllaying.Count > 0)
        {
            GameObject.Destroy(ListGameObjectFulllaying[0]);
            ListGameObjectFulllaying.RemoveAt(0);
        }
    }

    public void ShowCardFullLaying(bool isResult)
    {
        int cardid = 0;
        int x=0;
        LocationFullLaying(GameModelChan.game.listFullLaying[0], isResult);
        LocationFullLaying(GameModelChan.game.listFullLaying[1], isResult);
        // kiem tra hien thi chiu luc ket thuc game
        // se toi uu lai sau
        if (GameModelChan.game.listFullLaying[2] != null)
        {
            if (GameModelChan.game.listFullLaying[2].Length == 4)
            {
                for (int i = 0; i < GameModelChan.game.listFullLaying[2].Length; i++)
                {
                    Electrotank.Electroserver5.Api.EsObject obj = GameModelChan.game.listFullLaying[2][i];
                    cardid = obj.getInteger("id");
                }
                for (int j = 0; j < GameModelChan.game.listFullLaying[2].Length; j++)
                {
                    Electrotank.Electroserver5.Api.EsObject obj = GameModelChan.game.listFullLaying[2][j];
                    if (cardid == obj.getInteger("id"))
                    {
                        x++;
                    }
                }
                if (x == 4)
                    LocationFullLayingChiu(GameModelChan.game.listFullLaying[2], isResult);
                else
                    LocationFullLaying(GameModelChan.game.listFullLaying[2], isResult);
            }
            else
            {
                LocationFullLaying(GameModelChan.game.listFullLaying[2], isResult);
            }
        }
    }
    public void LocationFullLayingChiu(Electrotank.Electroserver5.Api.EsObject[] list, bool isResult)
    {
        int x = 0;
        int carid = 0;
        if (list == null || list.Length == 0) return;

        string textValue = "";
        int index = 0;
         if (GameModelChan.game.listFullLaying[2] == list)
        {
            index = (GameModelChan.game.listFullLaying[0].Length + GameModelChan.game.listFullLaying[1].Length) / 2;

            textValue = "Ù";
        }

        List<GameObject> listGameObject = new List<GameObject>();
        for (int i = 0; i < list.Length; i++)
        {
            Electrotank.Electroserver5.Api.EsObject obj = list[i];
            carid = obj.getInteger("id");
            if (carid == obj.getInteger("id"))
            {
                x++;
            }
            ChanCard c = new ChanCard(0, obj.getInteger("id"));

            c.Instantiate(GameModelChan.game.mPlaymat.locationFullLaying);

            if (obj.variableExists("cardDraw") && obj.getBoolean("cardDraw"))
                c.isDrawFromDeck = true;

                Vector3  moveToPosition = GameModelChan.game.mPlaymat.GetLocationFullLayingChiu(index, i);
            if (isResult)
                moveToPosition.x -= 230f;

            c.gameObject.transform.localPosition = moveToPosition;

            if (i % 2 == 1) index++;

            listGameObject.Add(c.gameObject);
        }

        int indexCenter = Mathf.FloorToInt(listGameObject.Count / 2) - 1;
        Vector3 centerObject = new Vector3(listGameObject[indexCenter].transform.localPosition.x, 110f, listGameObject[indexCenter].transform.localPosition.z);
        if (listGameObject.Count / 2 % 2 == 0)
            centerObject.x += 20f;

        GameObject objText = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/LablePrefab"));
        UILabel text = objText.GetComponent<UILabel>();
        objText.transform.parent = GameModelChan.game.mPlaymat.locationFullLaying;
        objText.transform.localScale = Vector3.one;
        objText.transform.localPosition = centerObject;
        text.color = Color.black;
        text.text = textValue;

        ListGameObjectFulllaying.Add(objText);
        ListGameObjectFulllaying.AddRange(listGameObject);
    }
    void LocationFullLaying(Electrotank.Electroserver5.Api.EsObject[] list, bool isResult)
    {
        if (list == null || list.Length == 0) return;

        string textValue = "Chắn";
        int index = 0;
        if (GameModelChan.game.listFullLaying[1] == list)
        {
            index = GameModelChan.game.listFullLaying[0].Length / 2;
            textValue = "Cạ";
        }
        else if (GameModelChan.game.listFullLaying[2] == list)
        {
            index = (GameModelChan.game.listFullLaying[0].Length + GameModelChan.game.listFullLaying[1].Length) / 2;

            textValue = "Ù";
        }

        List<GameObject> listGameObject = new List<GameObject>();
        for (int i = 0; i < list.Length; i++)
        {
            Electrotank.Electroserver5.Api.EsObject obj = list[i];

            ChanCard c = new ChanCard(0, obj.getInteger("id"));
            c.Instantiate(GameModelChan.game.mPlaymat.locationFullLaying);

            if (obj.variableExists("cardDraw") && obj.getBoolean("cardDraw"))
                c.isDrawFromDeck = true;

            Vector3 moveToPosition = GameModelChan.game.mPlaymat.GetLocationFullLaying(index, i % 2);
            if(isResult)
                moveToPosition.x -= 230f;

            c.gameObject.transform.localPosition = moveToPosition;

            if (i % 2 == 1) index++;

            listGameObject.Add(c.gameObject);
        }

        int indexCenter = Mathf.FloorToInt(listGameObject.Count / 2) - 1;
        Vector3 centerObject = new Vector3(listGameObject[indexCenter].transform.localPosition.x, 110f, listGameObject[indexCenter].transform.localPosition.z);
        if (listGameObject.Count / 2 % 2 == 0)
            centerObject.x += 20f;

        GameObject objText = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/LablePrefab"));
        UILabel text = objText.GetComponent<UILabel>();
        objText.transform.parent = GameModelChan.game.mPlaymat.locationFullLaying;
        objText.transform.localScale = Vector3.one;
        objText.transform.localPosition = centerObject;
        text.color = Color.black;
        text.text = textValue;

        ListGameObjectFulllaying.Add(objText);
        ListGameObjectFulllaying.AddRange(listGameObject);
    }
}
