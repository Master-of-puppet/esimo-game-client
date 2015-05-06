using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/dungnv/Tabbar Button")]
/// <summary>
/// Nguyen Viet Dung: GUI Tabbar Button.
/// </summary>
public class UITabbarButton : MonoBehaviour
{
	public delegate void OnPressInDelegate(int index);
	public event OnPressInDelegate OnCPressIn;
	
	public bool pressEnable = true;

	public UISprite target;
	public UILabel text;
	[SerializeField]
	public string normalSprite;
	[SerializeField]
	public string pressedSprite;
    [SerializeField]
	public bool noneMakePixelPerfect;
	[SerializeField]
	public Color colorActive = Color.white;
	[SerializeField]
	public Color colorInActive = Color.white;

	private bool _isSelected = false;
	
	public bool isSelected {
		set
		{
			_isSelected = value;
			OnSelected(_isSelected);
		}
		
		get
		{
			return _isSelected;	
		}
	}
	public int index;
	

	void OnEnable ()
	{
		
	}

	void Start ()
	{
		if (target == null) target = GetComponentInChildren<UISprite>();
	}


	void OnPress (bool pressed)
	{
		if(!pressEnable) return;
		if (enabled && target != null)
		{
            if (pressed && !_isSelected && OnCPressIn != null)
				OnCPressIn(this.index);
		}
	}
	
	void OnSelected(bool selected)
	{
		target.spriteName = selected ? pressedSprite : normalSprite;
		if (text != null)
			text.color = selected ? colorActive : colorInActive;
		if (!noneMakePixelPerfect) 
		{
			if(target.type != UISprite.Type.Sliced)
				target.MakePixelPerfect ();
			else
				ChangeHeight(target);
		}

        OnSelectedChanged(selected);
	}
	void ChangeHeight(UISprite sprite){
		Texture tex = sprite.mainTexture;
		UISpriteData sp = sprite.GetAtlasSprite();
		UISprite.Type t = sprite.type;

		if (tex != null && sp != null)
		{
			int x = Mathf.RoundToInt(sprite.atlas.pixelSize * (sp.width + sp.paddingLeft + sp.paddingRight));
			int y = Mathf.RoundToInt(sprite.atlas.pixelSize * (sp.height + sp.paddingTop + sp.paddingBottom));
			
			if ((x & 1) == 1) ++x;
			if ((y & 1) == 1) ++y;
			
			sprite.height = y;
		}
	}
	protected virtual void OnSelectedChanged(bool selected) { }
	
	public static void AddPressIn(UITabbarButton[] array, OnPressInDelegate OnDelegate) {
		foreach(UITabbarButton handle in array) 
		{
			handle.OnCPressIn += new UITabbarButton.OnPressInDelegate(OnDelegate);
		}
	}
	
	public void SetText(string text)
	{
		gameObject.transform.GetComponentInChildren<UILabel>().text = text;
	}
}