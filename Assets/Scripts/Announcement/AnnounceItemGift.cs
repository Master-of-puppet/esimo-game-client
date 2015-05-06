using UnityEngine;
using System.Collections;

public class AnnounceItemGift : MonoBehaviour
{
    public UISprite image;
    public UILabel description, money;
    
    [HideInInspector]
    public Announcement item;
    public static AnnounceItemGift Create(Transform parent, Announcement model)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Announcement/AnnounceGiftPrefab"));
        obj.name = string.Format("Gift_{0:0000}", model.index);
        obj.transform.parent = parent;
        obj.transform.localPosition = new Vector3(0f, 0f, -1f);
        obj.transform.localScale = Vector3.one;
        AnnounceItemGift row = obj.GetComponent<AnnounceItemGift>();
        row.SetData(model);
        return row;
    }

    public void SetData(Announcement model)
    {
        item = model;
        description.text = item.description;
        money.text = Utility.Convert.Chip(item.money);
        if (item.currentDay)
        {
			description.text = "NHẬN NGAY";
            SetSprite("to_day");
            CUIHandle.AddClick(GetComponent<CUIHandle>(), DoRecieverGift);
        }
        else
            description.text = item.description;
		if (item.receivered) {
			Color colorGray = new Color();
			colorGray.r = 99f/255f;
			colorGray.g = 99f/255f;
			colorGray.b = 99f/255f;
			colorGray.a = 200f/255f;
			image.color = colorGray;
			Color colorTextDesAlpha = new Color(description.color.r,description.color.g,description.color.b,90f/255f);
			Color colorTextMoneyAlpha = new Color(1f,1f,1f,90f/255f);

			description.color = colorTextDesAlpha;
			money.color = colorTextMoneyAlpha;
        }
    }
    public void ChangeTextToToday(string txtDescription)
    {
        description.text = txtDescription;
        SetSprite("not_to_day");
    }
    private void DoRecieverGift(GameObject targetObject)
    {
        if (!flagRequest)
        {
            GameManager.Server.DoRequestCommand(Fields.REQUEST.DAYLY_GIFT);
            flagRequest = true;
        }

    }
    public void SetSprite(string spriteName)
    {
        image.spriteName = spriteName;
    }

    void OnClick() 
    {
        if(item != null)
            item.OpenUrl();
    }

    void Update()
    {
        float value = Mathf.Lerp(0.731f, 1f, 0.02f / Vector3.SqrMagnitude(transform.position - AnnouncementView.VectorGiftCenter));
        gameObject.transform.localScale = new Vector3(value, value, 1f);
    }


    public bool flagRequest = false;
}
