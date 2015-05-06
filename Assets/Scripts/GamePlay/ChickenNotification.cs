using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ChickenNotification : MonoBehaviour
{
    #region Unity Editor
    public UISprite background;
    public UILabel content;
    #endregion
    [HideInInspector]
    public bool isFadeDone = false;
    public void ShowNotification(string text, float time)
    {
        //gameObject.SetActive(true);
        this.content.text = text;
        SetAlphaAllUI();
        ReDraw();
        StartCoroutine(FadeTo(time));
    }
    private void ReDraw()
    {
        float width = content.width * content.transform.localScale.x;
		background.width = (int)(width + 30);
    }
    System.Collections.IEnumerator FadeTo(float time)
    {
        yield return new WaitForSeconds(time * 3 / 4f);
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / (time / 4))
        {
            if (gameObject.GetComponent<UIPanel>() == null)
                gameObject.AddComponent(typeof(UIPanel));
            gameObject.GetComponent<UIPanel>().alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        isFadeDone = true;
        //gameObject.SetActive(false);
    }
    public void SetAlphaAllUI()
    {
        isFadeDone = false;
        gameObject.GetComponent<UIPanel>().alpha = 1f;
    }
    public void SetTranparentAllUI() 
    {
        gameObject.GetComponent<UIPanel>().alpha = 0f;
    }
}

