using System;
using System.Collections.Generic;
using UnityEngine;

public class GameplayWarningView : MonoBehaviour
{
    public UILabel lable;
    public UIPanel panel;

    static GameplayWarningView _instance;

    public static GameplayWarningView Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject warning = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/WarningPrefab"));
                warning.name = "__Warning";
                warning.transform.position = new Vector3(803, -71, 123f);
                _instance = warning.GetComponent<GameplayWarningView>();
            }
            return _instance;
        }
    }

    public static void Warning(string str, float time)
    {
        Instance.lable.text = str;
        Instance.panel.alpha = 1f;
        Instance.StopAllCoroutines();
        Instance.StartCoroutine(Instance.FadeTo(Instance.panel, time));
    }

    /// <summary>
    /// Ẩn dần thông báo
    /// </summary>
    System.Collections.IEnumerator FadeTo(UIPanel panel, float time)
    {
        yield return new WaitForSeconds(time * 2 / 3f);
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / (time / 4))
        {
            panel.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        GameObject.Destroy(gameObject);
    }
}
