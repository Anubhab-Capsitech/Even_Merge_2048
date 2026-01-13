// Assets/Editor/FindMissingScripts.cs
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class FindMissingScripts : EditorWindow
{
    [MenuItem("Tools/Find Missing Scripts In Scene")]
    static void ShowWindow() => GetWindow<FindMissingScripts>("Find Missing Scripts");

    void OnGUI()
    {
        if (GUILayout.Button("Scan Open Scene for Missing Scripts"))
            ScanOpenScene();
        if (GUILayout.Button("Scan Project (prefabs) for Missing Scripts"))
            ScanProjectPrefabs();
    }

    static void ScanOpenScene()
    {
        var objects = GameObject.FindObjectsOfType<GameObject>();
        int count = 0;
        foreach (var go in objects)
        {
            var components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.Log($"Missing script in GameObject '{go.name}' at path: {GetFullPath(go)}", go);
                    count++;
                }
            }
        }
        Debug.Log($"Scan complete. Missing scripts found: {count}");
    }

    static void ScanProjectPrefabs()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        int count = 0;
        foreach (string g in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;
            var comps = prefab.GetComponentsInChildren<Component>(true);
            foreach (var c in comps)
            {
                if (c == null)
                {
                    Debug.Log($"Missing script in Prefab '{path}'", prefab);
                    count++;
                    break;
                }
            }
        }
        Debug.Log($"Prefab scan complete. Prefabs with missing scripts: {count}");
    }

    static string GetFullPath(GameObject go)
    {
        string path = go.name;
        Transform t = go.transform;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}
