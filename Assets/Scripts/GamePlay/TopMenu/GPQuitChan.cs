using UnityEngine;
using System.Collections;

public class GPQuitChan : EGPQuit {
    void Awake()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickButtonClose;

        CUIHandle.AddClick(btClose, OnClickButtonClose);
        CUIHandle.AddClick(btQuit, OnClickButtonQuit);
        CUIHandle.AddClick(btXacNhan, OnClickButtonXacNhan);
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(btClose, OnClickButtonClose);
        CUIHandle.RemoveClick(btQuit, OnClickButtonQuit);
        CUIHandle.RemoveClick(btXacNhan, OnClickButtonXacNhan);

    }
    void OnClickButtonClose(GameObject go)
    {
        GameObject.Destroy(gameObject);
    }
    void OnClickButtonXacNhan(GameObject go)
    {
        GameModelChan.IsQuitWhenEndGame = cbQuitWhenEndGame.value;
        OnClickButtonClose(go);
    }
    void OnClickButtonQuit(GameObject go)
    {
        GameModelChan.game.OnQuitGame(false);
        OnClickButtonClose(go);
    }

    public static GPQuitChan Create()
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/TopMenu/GameplayQuitPrefab"));
        obj.name = "__Prefab Quit Gameplay";
        GPQuitChan component = obj.GetComponent<GPQuitChan>();
        if (GameModelChan.IsQuitWhenEndGame)
        {
            component.cbQuitWhenEndGame.value = GameModelChan.IsQuitWhenEndGame;
            component.isCreate = true;
        }
        //obj.transform.position = new Vector3(100f, 1000f, 0);
        return obj.GetComponent<GPQuitChan>();
    }
}
