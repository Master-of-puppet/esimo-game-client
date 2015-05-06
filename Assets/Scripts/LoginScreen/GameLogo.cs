using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLogo : MonoBehaviour
{

    #region Unity Editor
    public UITexture mainTexture;
    public CUIHandle btnLogo;
    #endregion
    private GameLogoModel model;
    [HideInInspector]
    public GameLogoModel Model
    {
        get { return model; }
        set { model = value; }
    }
    void Awake()
    {
        CUIHandle.AddClick(btnLogo, OnClickBtnLogo);
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnLogo, OnClickBtnLogo);
    }
    public static GameLogo Create(GameLogoModel model, Transform parent)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/LoginScreen/GameLogo"));
        //obj.name = "";
        obj.transform.parent = parent;
        obj.transform.localPosition = new Vector3(0f, 0f, -1f);
        obj.transform.localScale = Vector3.one;
        GameLogo logo = obj.GetComponent<GameLogo>();
        logo.gameObject.SetActive(false);
        logo.SetData(model);
        return logo;
    }
    public void SetData(GameLogoModel model)
    {
        this.model = model;
        this.model.LogoTexture(delegate(Texture _texture)
        {
            if (mainTexture != null) 
            { 
                mainTexture.mainTexture = _texture; 
                gameObject.SetActive(true);
                gameObject.transform.parent.GetComponent<UITable>().repositionNow = true;
            }
        });
    }
    private void OnClickBtnLogo(GameObject targetObject)
    {
        Application.OpenURL(model.Url);
    }
}
