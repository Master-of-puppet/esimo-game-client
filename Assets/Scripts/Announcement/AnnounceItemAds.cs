using UnityEngine;
using System.Collections;

public class AnnounceItemAds : MonoBehaviour
{
    public UITexture image;
    
    [HideInInspector]
    public Announcement item;

    public static AnnounceItemAds Create(Transform parent, Announcement model)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Announcement/AnnounceAdsPrefab"));
        obj.name = string.Format("Ads_{0:0000}", model.index);
        obj.transform.parent = parent;
        obj.transform.localPosition = new Vector3(0f, 0f, -1f);
        obj.transform.localScale = Vector3.one;
        AnnounceItemAds row = obj.GetComponent<AnnounceItemAds>();
        row.SetData(model);
        return row;
    }

    void SetData(Announcement model)
    {
        item = model;

        item.LoadTexture(delegate(Texture texture) { if (image != null) image.mainTexture = texture; });

        image.MakePixelPerfect();

        image.transform.localScale = new Vector3(image.transform.localScale.x, 150f, image.transform.localScale.z);

        Utility.AddCollider(gameObject);
    }

    void OnClick() 
    {
        if(item != null)
            item.OpenUrl();
    }

}
