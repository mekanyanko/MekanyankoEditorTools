using UnityEngine;
using UnityEditor;
using System.Linq;

public class RenameFromText : EditorWindow
{
    private string objectNames = "";
    private string newNames = "";
    private Vector2 scrollPos;
    private GameObject[] previousSelection;

    [MenuItem("Tools/Mekanyanko Tools/RenameFromText")]
    public static void ShowWindow()
    {
        GetWindow<RenameFromText>("Rename from Text");
    }

    void OnGUI()
    {
        if (SelectionChanged())
        {
            UpdateObjectNames();
        }

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Selected Objects", EditorStyles.boldLabel, GUILayout.Width(200));
        EditorGUILayout.LabelField($"Object Counts : {Selection.gameObjects.Length}", GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(600), GUILayout.Width(300));

        foreach (GameObject obj in Selection.gameObjects)
        {
            EditorGUILayout.LabelField(obj.name);
        }

        EditorGUILayout.EndScrollView();

        //EditorGUILayout.SelectableLabel(objectNames, GUILayout.Width(300), GUILayout.ExpandHeight(true));
        newNames = EditorGUILayout.TextArea(newNames, GUILayout.Height(600), GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();

        bool canExecute = Selection.gameObjects.Length == newNames.Split('\n').Length;

        if (GUILayout.Button("Get Name to Clipboad"))
        {
            EditorGUIUtility.systemCopyBuffer = objectNames;
        }

        if (!canExecute)
        {
            EditorGUILayout.HelpBox("The number of selected objects and the number of names do not match.", MessageType.Error);
        }

        EditorGUI.BeginDisabledGroup(!canExecute);
        if (GUILayout.Button("Rename"))
        {
            // ダイアログを表示して確認
            if (!EditorUtility.DisplayDialog("Rename", "Are you sure you want to rename?", "OK", "Cancel"))
            {
                return;
            }
            RenameSelectedObjects();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

    }

    private bool SelectionChanged()
    {
        if (previousSelection != Selection.gameObjects)
        {
            previousSelection = Selection.gameObjects;
            return true;
        }
        return false;
    }

    private void UpdateObjectNames()
    {
        objectNames = "";
        foreach (GameObject obj in Selection.gameObjects)
        {
            objectNames += obj.name + "\n";
        }
    }

    private void RenameSelectedObjects()
    {
        string[] namesArray = newNames.Split('\n');
        // Hierarchyの順序に基づいてソート
        var sortedGameObjects = Selection.gameObjects.OrderBy(go => go.transform.GetSiblingIndex()).ToArray();

        for (int i = 0; i < sortedGameObjects.Length; i++)
        {
            Undo.RecordObject(sortedGameObjects[i], "Rename Object");
            sortedGameObjects[i].name = namesArray[i];
        }
    }
}
