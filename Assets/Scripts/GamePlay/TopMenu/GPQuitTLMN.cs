using UnityEngine;
using System.Collections;

public class GPQuitTLMN : EGPQuit {
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
        GameModelTLMN.IsQuitWhenEndGame = cbQuitWhenEndGame.value;
        OnClickButtonClose(go);
    }
    void OnClickButtonQuit(GameObject go)
    {
        GameModelTLMN.game.OnQuitGame(false);
        OnClickButtonClose(go);
    }

    public static GPQuitTLMN Create()
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/TopMenu/GameplayQuitTLMNPrefab"));
        obj.name = "__Prefab Quit Gameplay";
        GPQuitTLMN component = obj.GetComponent<GPQuitTLMN>();
        if (GameModelTLMN.IsQuitWhenEndGame)
        {
            component.cbQuitWhenEndGame.value = GameModelTLMN.IsQuitWhenEndGame;
            component.isCreate = true;
        }

        //obj.transform.position = new Vector3(100f, 1000f, 0);
        return obj.GetComponent<GPQuitTLMN>();
    }
}
