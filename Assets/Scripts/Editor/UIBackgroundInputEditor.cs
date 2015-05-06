using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(UIBackgroundInput))]
public class UIBackgroundInputEditor : Editor {

    private UIBackgroundInput currentTarget;
    public override void OnInspectorGUI() {

        EditorGUIUtility.LookLikeControls();

        currentTarget.bgInputLeft = EditorGUILayout.ObjectField("Image Left", currentTarget.bgInputLeft, typeof(UISprite), true) as UISprite;
        currentTarget.bgInputCenter = EditorGUILayout.ObjectField("Image Center", currentTarget.bgInputCenter, typeof(UISprite), true) as UISprite;
        currentTarget.bgInputRight = EditorGUILayout.ObjectField("Image Right", currentTarget.bgInputRight, typeof(UISprite), true) as UISprite;
        currentTarget.btClear = EditorGUILayout.ObjectField("Image Right", currentTarget.btClear, typeof(CUIHandle), true) as CUIHandle;
        currentTarget.depth = EditorGUILayout.IntField("Depth", currentTarget.depth);
        if (currentTarget.depth >= 0)
            EditorGUILayout.HelpBox("Depth value can't greater than Zero", MessageType.Error, false);

        EditorGUILayout.Space();

        if (EditorGUILayout.Toggle("MakePixelPerfect", false))
        {
            currentTarget.bgInputLeft.MakePixelPerfect();
            currentTarget.bgInputCenter.MakePixelPerfect();
            currentTarget.bgInputRight.MakePixelPerfect();
        }
        EditorGUILayout.Space();

        currentTarget.InputWidth = EditorGUILayout.FloatField("Set Width", currentTarget.InputWidth);
        currentTarget.InputHeight = EditorGUILayout.FloatField("Set Height", currentTarget.InputHeight);
        currentTarget.IsHasClear = EditorGUILayout.Toggle("Is Show Button Clear", currentTarget.IsHasClear);

        if(currentTarget.IsHasClear)
            currentTarget.buttonClearSpace = EditorGUILayout.FloatField("Space Button Clear", currentTarget.buttonClearSpace);

        currentTarget.IsColorDefault = EditorGUILayout.Toggle("Set Color Is Default", currentTarget.IsColorDefault);
        EditorGUILayout.Space();
        
        if (GUI.changed)
            currentTarget.Resize();
    }
 
	public void OnEnable()
	{
		currentTarget = (UIBackgroundInput)target;
	}

}
