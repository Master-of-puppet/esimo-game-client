using UnityEngine;
using System.Collections;

public class GPQuitPhom : EGPQuit {
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
        GameModelPhom.IsQuitWhenEndGame = cbQuitWhenEndGame.value;
        OnClickButtonClose(go);
    }
    void OnClickButtonQuit(GameObject go)
    {
        GameModelPhom.game.OnQuitGame(false);
        OnClickButtonClose(go);
    }
    public static GPQuitPhom Create()
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/TopMenu/GameplayQuitPhomPrefab"));
        obj.name = "__Prefab Quit Gameplay";
        GPQuitPhom component = obj.GetComponent<GPQuitPhom>();
        if (GameModelPhom.IsQuitWhenEndGame)
        {
            component.cbQuitWhenEndGame.value = GameModelPhom.IsQuitWhenEndGame;
            component.isCreate = true;
        }

        //obj.transform.position = new Vector3(100f, 1000f, 0);
        return obj.GetComponent<GPQuitPhom>();
    }
}
