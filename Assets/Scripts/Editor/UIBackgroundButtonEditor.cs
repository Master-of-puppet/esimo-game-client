using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(UIBackgroundButton))]
public class UIBackgroundButtonEditor : Editor
{
    private UIBackgroundButton currentTarget;
    public override void OnInspectorGUI() {

        EditorGUIUtility.LookLikeControls();

        currentTarget.bgButtonLeft = EditorGUILayout.ObjectField("Image Left", currentTarget.bgButtonLeft, typeof(UISprite), true) as UISprite;
        currentTarget.bgButtonCenter = EditorGUILayout.ObjectField("Image Center", currentTarget.bgButtonCenter, typeof(UISprite), true) as UISprite;
        currentTarget.bgButtonRight = EditorGUILayout.ObjectField("Image Right", currentTarget.bgButtonRight, typeof(UISprite), true) as UISprite;
        currentTarget.depth = EditorGUILayout.IntField("Depth", currentTarget.depth);
        if (currentTarget.depth >= 0)
            EditorGUILayout.HelpBox("Depth value can't greater than Zero", MessageType.Error, false);
        EditorGUILayout.Space();

        if (EditorGUILayout.Toggle("MakePixelPerfect", false))
        {
            currentTarget.bgButtonLeft.MakePixelPerfect();
            currentTarget.bgButtonCenter.MakePixelPerfect();
            currentTarget.bgButtonRight.MakePixelPerfect();
        }
        EditorGUILayout.Space();

        currentTarget.ButtonWidth = EditorGUILayout.FloatField("Set Width", currentTarget.ButtonWidth);
        currentTarget.ButtonHeight = EditorGUILayout.FloatField("Set Height", currentTarget.ButtonHeight);
        currentTarget.TextValue = EditorGUILayout.TextField("Text Button", currentTarget.TextValue);
        EditorGUILayout.Space();

        if (GUI.changed)
            currentTarget.Resize();
    }
 
	public void OnEnable()
	{
        currentTarget = (UIBackgroundButton)target;
	}

}
