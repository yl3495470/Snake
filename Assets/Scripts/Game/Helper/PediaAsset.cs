using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;
using Config;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CustomAssetUtility
{
#if UNITY_EDITOR
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        SaveAsset(asset, typeof(T).ToString());
    }

    public static T CreateAsset<T>(string name) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        SaveAsset(asset, name);
        return asset;
    }

    public static void SaveAsset(UnityEngine.Object asset, string name)
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        AssetDatabase.Refresh();
    }
#endif
}

public class PediaAsset
{
#if UNITY_EDITOR
    [MenuItem("Assets/Create/ParachuteTpl")]
    public static void CreateParachuteTplAsset()
    {
        CustomAssetUtility.CreateAsset<ParachuteTpl>();
    }
#endif
}
