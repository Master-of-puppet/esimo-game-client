using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[CanEditMultipleObjects]
[CustomEditor(typeof(GameSettings))]
public class GameSettingsEditor : Editor
{
    private GameSettings setting;
    private SerializedObject uniEd;

    void OnEnable()
    {
        setting = (GameSettings)target;
        if (string.IsNullOrEmpty(setting.UrlPartner))
        {
            setting.UrlPartner = Utility.EnumUtility.GetAttribute<GameSettings.AttributeServer>(setting.HOST).Domain + "/api/getPartner";
        }

        setting.GetListPartner();
    }
    public override void OnInspectorGUI()
    {

        //uniEd = new SerializedObject(target);// target is the object that you are trying to make editor changes for. In this case Uni.
        //test1 = uniEd.FindProperty("test1");
      
        GameVersionGUI();
        NumberBotGUI();

    }

    bool foldVerion = true;
    private void GameVersionGUI()
    {
        foldVerion = EditorGUILayout.Foldout(foldVerion, "Version của game \"" + GameManager.GAME + "\" hiện tại: " + GameSettings.CurrentVersion);
        if (foldVerion)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Core Version: ");
                setting.CORE_VERSION = EditorGUILayout.TextField(setting.CORE_VERSION);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Build Version: ");
                setting.BUILD_VERSION = EditorGUILayout.IntField(setting.BUILD_VERSION);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Code Version (*): ");
                setting.CODE_VERSION_BUILD = EditorGUILayout.IntField(setting.CODE_VERSION_BUILD);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("\"CODE VERSION\" là phiên bản của AssetServer", MessageType.Info);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Server");
                setting.HOST = (GameSettings.EServer)EditorGUILayout.EnumPopup(setting.HOST);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Đối tác");
                setting.IndexPartner = EditorGUILayout.Popup(setting.IndexPartner, setting.ListPartnerName.ToArray());
                if (GUILayout.Button("Refresh"))
                    setting.GetListPartner();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Build Version For");
                setting.TypeBuildFor = (GameSettings.EBuildType)EditorGUILayout.EnumPopup(setting.TypeBuildFor);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("\"Build Version For\": Để biết đang build phiên bản này cho hệ thống nào . \n \"official\" là phiên bản build cho appstore , googleplay .\n \"unofficial\" là phiên bản build cho các đối tác khác ", MessageType.Info);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Website:");
                setting.WEB_SITE = EditorGUILayout.TextField(setting.WEB_SITE);
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void NumberBotGUI()
    {
        //foldBot = EditorGUILayout.Foldout(foldBot, "Số lượng bot tối đa được chơi trong trận: " + GameSettings.MAX_NUMBER_BOT);
        //if (foldBot)
        //{
        //    EditorGUILayout.BeginHorizontal();
        //    {
        //        EditorGUILayout.LabelField("Số lượng máy tối đa cho phép: ");
        //        GameSettings.MAX_NUMBER_BOT = EditorGUILayout.IntField(GameSettings.MAX_NUMBER_BOT);
        //    }
        //    EditorGUILayout.EndHorizontal();

        //    if(GameSettings.MAX_NUMBER_BOT < 1 || GameSettings.MAX_NUMBER_BOT > 3)
        //        EditorGUILayout.HelpBox("Giá trị tối đa được phép chỉ từ 1 đến 3", MessageType.Error);

        //    EditorGUILayout.Space();
        //}
    }


}
