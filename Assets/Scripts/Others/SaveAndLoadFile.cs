using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class SaveAndLoadFile : MonoBehaviour {

    private string result = "";
    string loadtext = "";
    static FileInfo file;
    void Start()
    {


    }

    //void OnGUI()
    //{
    //    GUILayout.BeginArea(new Rect(400, 50, 400, 400), "", "box");
    //    GUILayout.BeginVertical();
    //    result = GUILayout.TextField(result);

    //    if (GUILayout.Button("Save"))
    //    {
    //        Save(result);
    //    }

    //    if (GUILayout.Button("Load"))
    //    {
    //        loadtext = Load();
    //    }

    //    GUILayout.Label(loadtext);
    //    GUILayout.EndVertical();
    //    GUILayout.EndArea();

    //}

    public static void Save(string _value)
    {
        #if !UNITY_WEBPLAYER
        if (!Common.IsRelease)
            file = new FileInfo(Application.persistentDataPath + "\\" + "DebugLog" + DateTime.Now.Day +
                 "time" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + ".txt");
        else
            file = new FileInfo(Application.persistentDataPath + "\\" + "DebugLog.txt");
        StreamWriter writer;
        if (!file.Exists)
        {
            writer = file.CreateText();
        }
        else
        {
            file.Delete();
            writer = file.CreateText();

        }

        writer.WriteLine(_value);
        writer.Close();
        Debug.LogWarning(Application.persistentDataPath.ToString());
        StoreGame.SaveLog(StoreGame.EType.DEBUG_LOG,"");//Xóa luôn log sau khi đã lưu vào file
#endif
    }

    //string Load()
    //{
    //    if (!file.Exists)
    //    {
    //        StreamReader read = File.OpenText(Application.persistentDataPath + "\\" + "DebugLog.txt");
    //        string info = read.ReadToEnd();
    //        read.Close();
    //        return info;
    //    }
    //    return "File not exist";
    //}

}
