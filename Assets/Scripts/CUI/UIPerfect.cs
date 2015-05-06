using UnityEngine;
using System.Collections;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Component để MakePixelPerfect
/// Nhưng bọn NGUI giờ nó làm cho sẵn rồi. Cái này chắc không cần xài nữa.
/// Trước viết ra cái này vì bọn NGUI cũ nó chưa có.
/// </summary>
[ExecuteInEditMode]
[AddComponentMenu("NGUI/dungnv/Perfect (Basic)")]
public class UIPerfect : MonoBehaviour {
	
	void Start()
	{
		MakePixelPerfect();	
	}
	// Use this for initialization
	public void MakePixelPerfect ()
	{
		Vector3 pos = transform.localPosition;
		pos.z = Mathf.RoundToInt(pos.z);
		pos.x = Mathf.RoundToInt(pos.x);
		pos.y = Mathf.RoundToInt(pos.y);
		transform.localPosition = pos;
	}
}
