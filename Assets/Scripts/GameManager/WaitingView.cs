using System;
using System.Collections.Generic;
using UnityEngine;

public class WaitingView : MonoBehaviour
{
    #region Unity Editor
    public EasyFontTextMesh message;
    public UIPanel panel;
    #endregion

    private string textShow = null;
    //timeout:20 miniseconds
    const float timeout = 20;

    void Start()
    {
        InvokeRepeating("ShowText", 0f, 0.5f);
        if (textShow == null)
            Close();
        if (Application.loadedLevelName != ESceneName.LoginScreen.ToString())
            StartCoroutine(TimeoutToDestroy());
    }   

    System.Collections.IEnumerator TimeoutToDestroy()
    {
        yield return new WaitForSeconds(timeout);
        NotificationView.ShowMessage("Không có phản hồi từ máy chủ. Vui lòng kiểm tra chất lượng mạng");
        this.Close();
    }

    void OnDestroy()
    {
        CancelInvoke("ShowText");
    }

    int index = 0;
    void ShowText()
    {
        string dot = "";
        for (int i = 0; i < index; i++)
            dot += ".";
        message.Text = textShow + dot;

        index++;
        if (index > 4)
            index = 0;
    }

    static WaitingView _instance;
    public static WaitingView Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject waiting = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/WaitingPrefab"));
                //waiting.transform.parent = GameManager.Instance.transform;
                waiting.name = "__Waiting";
                waiting.transform.position = new Vector3(103, -540, -108f);
                _instance = waiting.GetComponent<WaitingView>();
                DontDestroyOnLoad(waiting);
            }
            return _instance;
        }
    }

    public static void Show(string text)
    {
        Instance.textShow = text;
    }

    public static void Hide()
    {
        Instance.StartCoroutine(Instance.FadeTo(Instance.panel, 1f));
    }

    public void Close()
    {
        GameObject.Destroy(gameObject);
    }

    /// <summary>
    /// Ẩn dần thông báo
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    System.Collections.IEnumerator FadeTo(UIPanel panel, float time)
    {
        yield return new WaitForSeconds(time * 2 / 3f);
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / (time / 4))
        {
            panel.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        Close();
    }
}
