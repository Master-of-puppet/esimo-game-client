using UnityEngine;
using System.Collections;

public class AnnounceItemEvent : MonoBehaviour
{
    public UITexture image;
    public UISprite spriteTrans;
    public UILabel description;

    [HideInInspector]
    public Announcement item;

    public static AnnounceItemEvent Create(Transform parent, Announcement model)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Announcement/AnnounceEventPrefab"));
        obj.name = string.Format("Announcement_{0:0000}", model.index);
        obj.transform.parent = parent;
        obj.transform.localPosition = new Vector3(0f, 0f, -1f);
        obj.transform.localScale = Vector3.one;
        AnnounceItemEvent row = obj.GetComponent<AnnounceItemEvent>();
        row.SetData(model);
        return row;
    }

    void SetData(Announcement model)
    {
        item = model;

        item.LoadTexture(delegate(Texture texture) { if(image != null) image.mainTexture = texture; });

        image.transform.localScale = new Vector3(240f, 180f, 1f);

        description.text = item.description;

        int totalLine = description.processedText.Split("\n".ToCharArray(), System.StringSplitOptions.None).Length;
        if (totalLine > 2)
            spriteTrans.transform.localScale = new Vector3(spriteTrans.transform.localScale.x, 50f + (totalLine - 2) * 20f, spriteTrans.transform.localScale.z);
    }

    void OnClick() 
    {
        if(item != null)
            item.OpenUrl();
    }

}
