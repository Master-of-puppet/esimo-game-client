using UnityEngine;
using System.Collections;
public class PolicyView : MonoBehaviour {

    public CUIHandle btnClose;

    void Awake()
    {

        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickClose;
        CUIHandle.AddClick(btnClose, OnClickClose);
    }
    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnClose, OnClickClose);
    }

    private void OnClickClose(GameObject targetObject)
    {
        GameObject.Destroy(gameObject);
    }
    public static PolicyView Create()
    {
        GameObject obj = (GameObject)(GameObject)GameObject.Instantiate(Resources.Load("Prefabs/LoginScreen/Policy"));
        PolicyView policy = obj.GetComponent<PolicyView>();
        return policy;
    }

}
