using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UISliceBackgroundPopup))]
public class UISliceBackgroundPopupEditor : Editor
{
	private UISliceBackgroundPopup currentTarget;

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeControls();
        currentTarget.TitlePopup = EditorGUILayout.TextField("Title Popup", currentTarget.TitlePopup);
        currentTarget.Depth = EditorGUILayout.IntField("Depth", currentTarget.Depth);
        if (currentTarget.Depth >= 0)
            EditorGUILayout.HelpBox("Depth value can't greater than Zero", MessageType.Error, false);

        currentTarget.PopupWidth = EditorGUILayout.FloatField("Popup Width", currentTarget.PopupWidth);
        currentTarget.PopupHeight = EditorGUILayout.FloatField("Popup Height", currentTarget.PopupHeight);

        currentTarget.IsShowBtnClose = EditorGUILayout.Toggle("Is Has Button Close", currentTarget.IsShowBtnClose);
        EditorGUILayout.Space();

        if (GUI.changed)
            currentTarget.Resize();
    }

    public void OnEnable() 
    {
		currentTarget = (UISliceBackgroundPopup)target;
    }
}