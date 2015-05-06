using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardRechargeView : MonoBehaviour {

    public UITexture backgroundTexture;
    public UILabel textAlt;
    [HideInInspector]
    public List<RechargeModel> models;
    void Awake()
    {
        CUIHandle.AddClick(gameObject.GetComponent<CUIHandle>(), OnClickOpenRechargeCard);
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(gameObject.GetComponent<CUIHandle>(), OnClickOpenRechargeCard);
    }

    private void OnClickOpenRechargeCard(GameObject targetObject)
    {
        RechargePopup.ShowRechargeCard(models[0]);
    }
    public static CardRechargeView Create(List<RechargeModel> models, Transform parent) 
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HeaderMenu/Recharge/CardRechargeView"));
        obj.transform.parent = parent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        CardRechargeView card = obj.GetComponent<CardRechargeView>();
        card.SetData(models);
        return card;
    }
    public void SetData(List<RechargeModel> models)
    {
        this.models = models;
        RechargeModel model = models[0];
        new AvatarCacheOrDownload(model.ImagesUrl, delegate(Texture avatar) { 
            if (avatar != null)  {

                avatar.filterMode = FilterMode.Point;
                avatar.anisoLevel = 0;
                avatar.wrapMode = TextureWrapMode.Clamp;
                backgroundTexture.mainTexture = avatar;
                backgroundTexture.MakePixelPerfect();
            }
            else
            {
                textAlt.gameObject.SetActive(true);
                textAlt.text = model.Provider;
            }
        },true);
        //backgroundSprite.spriteName = model.Provider;
    }
}
