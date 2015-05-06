using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;


[CustomEditor (typeof (EasyFontTextMesh))]
public class EasyFontCustomEditor : Editor {
	
	
	private bool	wasPrefabModified;
	private bool 	isFirstTime = true;
	
	void OnEnable()
	{
		EasyFontTextMesh customFont = target as EasyFontTextMesh;
		
		if (customFont.GUIChanged || isFirstTime)
		{
			//customFont.RefreshMeshEditor();
			RefreshAllSceneText(); //Refresh all test to solve the duplicate command issue (Text is not seeing when duplicating). Comment this line an use line above if you have a lot of text
									// and are sufferig slowdonws in the editor when selecting texts
			isFirstTime = false; //This is a hack because on enable is called a lot of times because of the porpertie font
		}
	}
	
	void OnDisable()
	{
		EasyFontTextMesh customFont = target as EasyFontTextMesh;
		
		if (customFont.GUIChanged) //Hack because of the properties is calling this even if there is no OnDisable
			isFirstTime = true;
	}
	
	public override void OnInspectorGUI()
	{

		EditorGUIUtility.LookLikeInspector();
		DrawDefaultInspector();
		
		EasyFontTextMesh customFont = target as EasyFontTextMesh;
		
		SerializedObject serializedObject = new SerializedObject(customFont);
		
		SerializedProperty serializedText 				= serializedObject.FindProperty("_privateProperties.text");
		SerializedProperty serializedFontType 			= serializedObject.FindProperty("_privateProperties.font");
		SerializedProperty serializedFontFillMaterial 	= serializedObject.FindProperty("_privateProperties.customFillMaterial");
		SerializedProperty serializedFontSize 			= serializedObject.FindProperty("_privateProperties.fontSize");
		SerializedProperty serializedCharacterSize 		= serializedObject.FindProperty("_privateProperties.size");
		SerializedProperty serializedTextAnchor 		= serializedObject.FindProperty("_privateProperties.textAnchor");
		SerializedProperty serializedTextAlignment 		= serializedObject.FindProperty("_privateProperties.textAlignment");
		SerializedProperty serializedLineSpacing 		= serializedObject.FindProperty("_privateProperties.lineSpacing");
		SerializedProperty serializedFontColorTop 		= serializedObject.FindProperty("_privateProperties.fontColorTop");
		SerializedProperty serializedFontColorBottom 	= serializedObject.FindProperty("_privateProperties.fontColorBottom");
		SerializedProperty serializedEnableShadow 		= serializedObject.FindProperty("_privateProperties.enableShadow");
		SerializedProperty serializedShadowColor 		= serializedObject.FindProperty("_privateProperties.shadowColor");
		SerializedProperty serializedShadowDistance 	= serializedObject.FindProperty("_privateProperties.shadowDistance");
		SerializedProperty serializedEnableOutline 		= serializedObject.FindProperty("_privateProperties.enableOutline");
		SerializedProperty serializedOutlineColor 		= serializedObject.FindProperty("_privateProperties.outlineColor");
		SerializedProperty serializedOutlineWidth 		= serializedObject.FindProperty("_privateProperties.outLineWidth");
		
		SerializedProperty[] allSerializedProperties = new SerializedProperty[16]
		{	
			serializedText, serializedFontType, serializedFontFillMaterial , serializedFontSize, serializedCharacterSize,serializedTextAnchor, serializedTextAlignment,
			serializedLineSpacing, serializedFontColorTop, serializedFontColorBottom, serializedEnableShadow, serializedShadowColor, serializedShadowDistance,
			serializedEnableOutline, serializedOutlineColor, serializedOutlineWidth
		};
        
		#region properties
		
		
		//Text
		if(serializedText.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedText.prefabOverride);
		
		customFont.Text =  EditorGUILayout.TextField("Text", customFont.Text);
		
		//Font
		
		if(serializedFontType.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedFontType.prefabOverride);
		
		customFont.FontType = EditorGUILayout.ObjectField("Font", customFont.FontType, typeof(Font), false) as Font;
		
		
		if (customFont.FontType == null)
		{
			customFont.FontType = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font; 
		}
		
		//Font material
		if(serializedFontFillMaterial.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedFontFillMaterial.prefabOverride);
		
		customFont.CustomFillMaterial = EditorGUILayout.ObjectField("Custom Fill material", customFont.CustomFillMaterial, typeof(Material), false) as Material;
		
		if (customFont.FontType == null)
		{
			customFont.FontType = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font; 
		}
		
		//Font Size
		if(serializedFontSize.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedFontSize.prefabOverride);
			
		customFont.FontSize = EditorGUILayout.IntField("Font size", customFont.FontSize);
		
		//CharacterSize
		if(serializedCharacterSize.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedCharacterSize.prefabOverride);
		
		customFont.Size = EditorGUILayout.FloatField("Character size", customFont.Size); 
		
		
		//Text acnhor
		if(serializedTextAnchor.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedTextAnchor.prefabOverride);
		
		customFont.Textanchor = (EasyFontTextMesh.TEXT_ANCHOR)EditorGUILayout.EnumPopup("Text Anchor", customFont.Textanchor);
		
		//Text alignment
		if(serializedTextAlignment.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedTextAlignment.prefabOverride);

		customFont.Textalignment = (EasyFontTextMesh.TEXT_ALIGNMENT)EditorGUILayout.EnumPopup("Text alignment", customFont.Textalignment);
		
		//Line spacing
		if(serializedLineSpacing .isInstantiatedPrefab)
			SetBoldDefaultFont(serializedLineSpacing.prefabOverride);
		
		customFont.LineSpacing = EditorGUILayout.FloatField("Line spacing", customFont.LineSpacing); 
		
		// Font color
		if(serializedFontColorTop.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedFontColorTop.prefabOverride);
		
		customFont.FontColorTop = EditorGUILayout.ColorField ("Top Color", customFont.FontColorTop);
		
		if(serializedFontColorBottom.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedFontColorBottom.prefabOverride);
		
		customFont.FontColorBottom = EditorGUILayout.ColorField ("Bottom Color", customFont.FontColorBottom);
		
		
		// Shadow
		if(serializedEnableShadow.isInstantiatedPrefab)
			SetBoldDefaultFont(serializedEnableShadow.prefabOverride);
		
			customFont.EnableShadow =  EditorGUILayout.Toggle("Enable Shadow", customFont.EnableShadow);
		
		if (customFont.EnableShadow) //Only show the options when enabled
		{
			if(serializedShadowColor.isInstantiatedPrefab)
				SetBoldDefaultFont(serializedShadowColor.prefabOverride);
			
			customFont.ShadowColor 		= EditorGUILayout.ColorField("Shadow color", customFont.ShadowColor);
			
			if(serializedShadowDistance.isInstantiatedPrefab)
				SetBoldDefaultFont(serializedShadowDistance.prefabOverride);
			
			customFont.ShadowDistance 	= EditorGUILayout.Vector3Field("Shadow distance", customFont.ShadowDistance);
		}
		
		
		//Outline
		if(serializedEnableOutline.isInstantiatedPrefab)
				SetBoldDefaultFont(serializedEnableOutline.prefabOverride);

		 customFont.EnableOutline = EditorGUILayout.Toggle("Enable Outline", customFont.EnableOutline);
		
		if (customFont.EnableOutline) //Only show the options when enabled
		{
			if(serializedOutlineColor.isInstantiatedPrefab)
				SetBoldDefaultFont(serializedOutlineColor.prefabOverride);
			
			customFont.OutlineColor = EditorGUILayout.ColorField("Outline color", customFont.OutlineColor);
			
			if(serializedOutlineWidth.isInstantiatedPrefab)
				SetBoldDefaultFont(serializedOutlineWidth.prefabOverride);
			
			customFont.OutLineWidth = EditorGUILayout.FloatField("Outline width", customFont.OutLineWidth);
		}
		
		#endregion
		
		#region buttons and info
		
		if (GUILayout.Button("Refresh"))
		{
			Debug.Log("Refreshing Text mesh");
			customFont.RefreshMeshEditor();
			
		} 
		
		if (GUILayout.Button("Refresh all"))
		{
			RefreshAllSceneText();
			//OnPlayModeChanged();
		}
		
		GUIStyle buttonStyleRed = new GUIStyle("button");
		buttonStyleRed.normal.textColor = Color.red;
		
		if (GUILayout.Button("Destroy Text component",buttonStyleRed))
		{
			Renderer tempRenderer = customFont.gameObject.renderer;
			MeshFilter	tempMeshFilter = customFont.GetComponent<MeshFilter>();
			DestroyImmediate(customFont);
			DestroyImmediate(tempRenderer);
			DestroyImmediate(tempMeshFilter.sharedMesh);
			DestroyImmediate(tempMeshFilter);
			return;
		}
		
		GUIStyle greenText = new GUIStyle();
		greenText.normal.textColor = Color.green;
		EditorGUILayout.LabelField (string.Format("Vertex count {0}", customFont.GetVertexCount().ToString()),greenText);
		EditorGUILayout.LabelField (string.Format("Font Texture Size {0} x {1}", customFont.renderer.sharedMaterial.mainTexture.width.ToString(),customFont.renderer.sharedMaterial.mainTexture.height.ToString()),greenText);
		
		
		#endregion
		
		#region prefab checks
		//Check if the prefab has changed to refresh the text
		bool checkCurrentPrefabModification = false;
		
		PropertyModification[] modifiedProperties = PrefabUtility.GetPropertyModifications((Object)customFont);
		if (modifiedProperties != null && modifiedProperties.Length > 0)
		{
			for (int i = 0; i<modifiedProperties.Length; i++)
			{
				foreach (SerializedProperty serializerPropertyIterator in allSerializedProperties)
				{
					if (serializerPropertyIterator.propertyPath == modifiedProperties[i].propertyPath)
					{
						wasPrefabModified = true;
					checkCurrentPrefabModification = true;
					}
				}
			}
			
		}
		else
		{
			checkCurrentPrefabModification = false;			
		}
		
		if (wasPrefabModified && !checkCurrentPrefabModification)
		{
			RefreshAllSceneText();
			wasPrefabModified = false;
		}
		
		//Security check. If the mesh is null a prefab revert has been made
		if (customFont.GetComponent<MeshFilter>().sharedMesh == null)
			customFont.RefreshMeshEditor();
			
		#endregion
		customFont.GUIChanged = GUI.changed;
		if (customFont.GUIChanged)
		{
			customFont.RefreshMeshEditor();
			EditorUtility.SetDirty(customFont);
		}
		
	}

	
	void RefreshAllSceneText()
	{
		Object[] customFonts = Resources.FindObjectsOfTypeAll(typeof(EasyFontTextMesh));
		
		if (customFonts.Length > 0)
		{
			for (int i= 0; i < customFonts.Length; i++)
			{
				if (AssetDatabase.GetAssetPath(customFonts[i]) == "") //Only affect the scene assets
				{
					EasyFontTextMesh tempCustomFont =  (EasyFontTextMesh)customFonts[i];	
					tempCustomFont.RefreshMeshEditor(); 
				}
			}
		}
		//GameObject.Find("Perrete2").GetComponent<CustomTextMesh>().name = "Perrete1";
		
		
	}
	
	private MethodInfo boldFontMethodInfo = null;
 
	private void SetBoldDefaultFont(bool value) {
	    
		boldFontMethodInfo = typeof(EditorGUIUtility).GetMethod("SetBoldDefaultFont", BindingFlags.Static | BindingFlags.NonPublic);
		boldFontMethodInfo.Invoke(null, new[] { value as object });
	}
	
	
	
	
}
