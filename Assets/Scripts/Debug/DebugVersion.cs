using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Class này chỉ để vẽ version lên góc màn hình thôi
/// </summary>
class DebugVersion
{
    public DebugConsoleLog consoleLog;

    public DebugVersion(DebugConsoleLog console)
    {
        this.consoleLog = console;
    }

    public void OnGUI()
    {
		if (GameManager.CurrentScene != ESceneName.LoginScreen) return;
        #region VERSION
        GUI.contentColor = new Color(200 / 255f, 200 / 255f, 200 / 255f, 150 / 255f);
        float footerHeight = Screen.height / 10f;
        Rect rectFooter = new Rect(0, Screen.height - footerHeight, Screen.width, footerHeight);
        GUILayout.BeginArea(rectFooter);
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                {
                    GUILayout.FlexibleSpace();
					GUILayout.Label("v" + GameSettings.CurrentVersion + "(" + GameSettings.Instance.ParnerCodeIdentifier + ")");

                    if (GameManager.CurrentScene == ESceneName.LoginScreen)
                    {
                        //if (GUILayout.Button("Clears Cache"))
                            //StoreGame.ClearCache();
                    }
                    else if (Debug.isDebugBuild && Application.isEditor == false)
                        GUILayout.Space(25f);
                }
                GUILayout.EndVertical();
                GUILayout.Space(5f);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
        #endregion
    }

    public void Update()
    {
        #region Show Debug Log Content
        if (!consoleLog.showLogs && Common.IsMobile)
        {
            float width = Screen.width / 8f, height = Screen.height / 8f;
            Rect topLeft = new Rect(1f, 0f, width, height);
            Rect topRight = new Rect(Screen.width - width, 0f, width, height);

            Vector2 detectDevice = Vector2.zero;
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                detectDevice = new Vector2(Input.GetTouch(0).position.x, Screen.height - Input.GetTouch(0).position.y);
            else if (Input.GetButtonDown("Fire1"))
                detectDevice = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

            if (detectDevice != Vector2.zero)
            {
                if (CountTouch == 0 && topLeft.Contains(detectDevice))
                    CountTouch++;
                else if (CountTouch > 0 && TimeCountDown > 0 && topRight.Contains(detectDevice))
                {
                    CountTouch++;

                    if (CountTouch >= 4)
                    {
                        consoleLog.showLogs = !consoleLog.showLogs;
                        CountTouch = 0;
                    }
                }
            }

            if (TimeCountDown > 0)
                TimeCountDown -= Time.deltaTime;
        }
        #endregion
    }
    
    #region VAR
    int _countTouch = 0;
    int CountTouch
    {
        get { return _countTouch; }
        set { _countTouch = value;
            if(value > 0) TimeCountDown = 1f;
        }
    }
    float _timeCountDown = 0f;
    float TimeCountDown
    {
        get { return _timeCountDown; }
        set { _timeCountDown = value; 
            if(value <= 0) CountTouch = 0;
        }
    }
    #endregion
}
