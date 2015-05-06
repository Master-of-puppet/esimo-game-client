using UnityEngine;
using System.Collections;

public abstract class ECardTexture : MonoBehaviour {
    //dung cho phom va tlml
    public const int CARD_WITH = 50;
    public const int CARD_HEIGHT = 70;

    public UITexture texture;

    [HideInInspector]
    public ECard card;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        OnUpdate();
    }

    public void SetCollider(bool active)
    {
        gameObject.collider.enabled = active;
    }

    public abstract void OnUpdate();
    public abstract void SetValue();
    public abstract void OnClick();
    public virtual void OnDoubleClick()
    {

    }
    public abstract int getWith();
    public abstract int getHeight();
}
