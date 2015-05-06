using UnityEngine;
using System.Collections;

public class PlayerBettingView : MonoBehaviour
{

    private static int CARD_WIDTH = 40;
    private static int CARD_HIGHT = 180;

    #region Unity Editor
    public UITexture avatar;
    public UILabel userName, lbNoJoin, lbTypeLaying, lbMoney;
    public UISprite iconChicken;
	public UISprite iconChange;
    #endregion

    [HideInInspector]
    public PlayerBettingModel model;
    public static PlayerBettingView Create(PlayerBettingModel model, Transform parent)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/GaNgoai/PlayerBettingView"));
        obj.transform.parent = parent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        PlayerBettingView bettings = obj.GetComponent<PlayerBettingView>();
        bettings.SetData(model);
        return bettings;
    }

    public void SetData(PlayerBettingModel model)
    {
        this.model = model;
        userName.text = model.Player.username;
        model.Player.AvatarTexture(delegate(Texture texture)
        {
            avatar.mainTexture = texture;
        });
        ShowTypeAndMoney(model.ETypeLaying != ETypeLayingBetting.None);
        string textTypeGa = "";
        if (model.ETypeLaying != ETypeLayingBetting.None)
        {
            switch (model.ETypeLaying)
            {
                case ETypeLayingBetting.Rong:
					textTypeGa = "Rộng";
                    break;
                case ETypeLayingBetting.Hep:
					textTypeGa = "Hẹp";
                    break;
                case ETypeLayingBetting.RongHep:
					textTypeGa = "Rộng | Hẹp";
                    break;
            }
            lbTypeLaying.text = textTypeGa;
            lbMoney.text = Utility.Convert.Chip(model.ChipBetting);

            ECardTexture texture = gameObject.GetComponentInChildren<ECardTexture>();
            if (texture != null)
            {
                texture.card.CardId = model.CardId;
            }
            else
            {
                Transform cardManager = gameObject.transform.FindChild("CardManager");
                ChanCard card = new ChanCard();
                card.CardId = model.CardId;
                card.parent = cardManager;
                card.Instantiate();
                card.cardTexture.texture.width = CARD_WIDTH; 
                card.cardTexture.texture.height = CARD_HIGHT;
                GameObject.Destroy(card.gameObject.GetComponent<BoxCollider>());
            }
			
        }
    }
    public void ShowTypeAndMoney(bool isShow)
    {
        lbNoJoin.gameObject.SetActive(!isShow);
        lbTypeLaying.transform.parent.gameObject.SetActive(isShow);
    }
}
