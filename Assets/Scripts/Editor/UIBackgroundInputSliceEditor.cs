using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(UIBackgroundInputSlice))]
public class UIBackgroundInputSliceEditor : Editor {

	private UIBackgroundInputSlice currentTarget;
    public override void OnInspectorGUI() {

        EditorGUIUtility.LookLikeControls();

   
        currentTarget.bgSliceInputBackground = EditorGUILayout.ObjectField("Slice Background", currentTarget.bgSliceInputBackground, typeof(UISprite), true) as UISprite;
        currentTarget.label = EditorGUILayout.ObjectField("Label", currentTarget.label, typeof(UILabel), true) as UILabel;

        currentTarget.btClear = EditorGUILayout.ObjectField("Button Clear", currentTarget.btClear, typeof(CUIHandle), true) as CUIHandle;
        currentTarget.depth = EditorGUILayout.IntField("Depth", currentTarget.depth);
        if (currentTarget.depth >= 0)
            EditorGUILayout.HelpBox("Depth value can't greater than Zero", MessageType.Error, false);

        EditorGUILayout.Space();

        if (EditorGUILayout.Toggle("MakePixelPerfect", false))
        {
			currentTarget.bgSliceInputBackground.MakePixelPerfect();
        }
        EditorGUILayout.Space();

        currentTarget.InputWidth = EditorGUILayout.IntField("Set Width", currentTarget.InputWidth);
		currentTarget.InputHeight = EditorGUILayout.IntField("Set Height", currentTarget.InputHeight);
        currentTarget.IsHasClear = EditorGUILayout.Toggle("Is Show Button Clear", currentTarget.IsHasClear);

        if(currentTarget.IsHasClear)
            currentTarget.buttonClearSpace = EditorGUILayout.FloatField("Space Button Clear", currentTarget.buttonClearSpace);

        currentTarget.IsColorDefault = EditorGUILayout.Toggle("Set Color Is Default", currentTarget.IsColorDefault);
		currentTarget.IsRePositionLabel = EditorGUILayout.Toggle("Set Reposition Label", currentTarget.IsRePositionLabel);
        EditorGUILayout.Space();
        
        if (GUI.changed)
            currentTarget.Resize();
    }
 
	public void OnEnable()
	{
		currentTarget = (UIBackgroundInputSlice)target;
	}

}
