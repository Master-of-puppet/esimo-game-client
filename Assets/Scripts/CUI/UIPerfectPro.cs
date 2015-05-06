using UnityEngine;
using System.Collections;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Component để MakePixelPerfect
/// Nhưng bọn NGUI giờ nó làm cho sẵn rồi. Cái này chắc không cần xài nữa.
/// Trước viết ra cái này vì bọn NGUI cũ nó chưa có.
/// </summary>
[ExecuteInEditMode]
[AddComponentMenu("NGUI/dungnv/Perfect (Pro)")]
public class UIPerfectPro : MonoBehaviour {
	
	// Use this for initialization
	public void MakePixelPerfect ()
	{
		Vector3 pos = transform.localPosition;
		pos.z = Mathf.RoundToInt(pos.z);
		pos.x = Mathf.RoundToInt(pos.x);
		pos.y = Mathf.RoundToInt(pos.y);
		transform.localPosition = pos;
		
		UIPerfect[] perfectList = GetComponentsInChildren<UIPerfect>();
		Debug.Log(perfectList + "max pixe" + perfectList.Length);
		if(perfectList == null) return;
		Debug.Log("max pixe");
		foreach(UIPerfect compo in perfectList)
			compo.MakePixelPerfect();	
	}
}
