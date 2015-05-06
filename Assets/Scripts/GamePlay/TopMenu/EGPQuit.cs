using UnityEngine;
using System.Collections;

public class EGPQuit : MonoBehaviour {

    public CUIHandle btClose;
    public UIToggle cbQuitWhenEndGame;
    public CUIHandle btQuit;
    public CUIHandle btXacNhan;
    public bool isCreate = false;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.01f);
        if (isCreate)
        {
            btQuit.gameObject.SetActive(true);
            btXacNhan.gameObject.SetActive(false);
            isCreate = false;
        }
    }
}
