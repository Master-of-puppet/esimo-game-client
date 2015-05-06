using UnityEngine;
using System.Collections;

public class NotificationBackground : MonoBehaviour {

	void OnClick () {
        NotificationView obj = GameObject.FindObjectOfType<NotificationView>();
        if (obj)
        {
            obj.OnBackgroundClick();
        }
	}
}
