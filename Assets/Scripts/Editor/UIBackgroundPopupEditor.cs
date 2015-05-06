using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIBackgroundPopup))]
public class UIBackgroundPopupEditor : Editor
{
    private UIBackgroundPopup currentTarget;

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeControls();
        currentTarget.TitlePopup = EditorGUILayout.TextField("Title Popup", currentTarget.TitlePopup);
        if (!string.IsNullOrEmpty(currentTarget.TitlePopup))
            currentTarget.spaceTitleText = EditorGUILayout.FloatField("Space Title Text", currentTarget.spaceTitleText);

        currentTarget.Depth = EditorGUILayout.IntField("Depth", currentTarget.Depth);
        if (currentTarget.Depth >= 0)
            EditorGUILayout.HelpBox("Depth value can't greater than Zero", MessageType.Error, false);

        currentTarget.PopupWidth = EditorGUILayout.FloatField("Popup Width", currentTarget.PopupWidth);
        currentTarget.PopupHeight = EditorGUILayout.FloatField("Popup Height", currentTarget.PopupHeight);
        currentTarget.IsHasTitle = EditorGUILayout.Toggle("Is Draw Title", currentTarget.IsHasTitle);
        EditorGUILayout.Space();

        if (GUI.changed)
            currentTarget.Resize();
    }

    public void OnEnable() 
    {
        currentTarget = (UIBackgroundPopup)target;
    }
}