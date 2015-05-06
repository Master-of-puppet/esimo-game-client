using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CUITextList : UITextList
{
    public int TotalLines { get { return mTotalLines; } }
	/// <summary>
	/// Delegate function called when the scroll bar's value changes.
	/// </summary>
	
	void OnScrollBar ()
	{
		mScroll = UIScrollBar.current.value;
		UpdateVisibleText();
	}
}

