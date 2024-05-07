using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MekanyankoTools
{
    internal static class ExtensionPreviewerEditor
    {
        private const string rootString = "Assets";

        private static readonly int m_RemoveCount = rootString.Length;
        private static readonly string colorSavedKey = "EP_ColorKey";

        internal static Color defaultColor = new Color(0.35f, 0.35f, 0.35f, 1.0f);
        internal static Color m_Color { get; set; }
        internal static bool m_ShowExtension = true;

        [InitializeOnLoadMethod]
        private static void EnableExtensionPreviewer()
        {
            LoadEPSettings();
            EditorApplication.projectWindowItemOnGUI += OnGUI;
        }

        private static void OnGUI(string guid, Rect selectionRect)
        {
            if (m_ShowExtension)
            {
                var dataPath = Application.dataPath;
                var startIndex = dataPath.LastIndexOf(rootString);
                var dir = dataPath.Remove(startIndex, m_RemoveCount);
                var path = dir + AssetDatabase.GUIDToAssetPath(guid);

                if (!File.Exists(path)){ return; }

                var label = EditorStyles.label;

                // 拡張子を取得
                var fileInfo = new FileInfo(path);
                string extension = fileInfo.Extension;

                // 拡張子がない場合、何も表示しない
                if (string.IsNullOrEmpty(extension)){ return; }

                // 拡張子を表示するラベルの準備
                var extContent = new GUIContent(extension);
                var extWidth = label.CalcSize(extContent).x + 3.0f;

                // 拡張子ラベルの位置
                var extPos = selectionRect;
                extPos.x = extPos.xMax - extWidth;
                extPos.width = extWidth;
                extPos.yMin++;

                // ラベルを描画
                var color = GUI.color;
                GUI.color = m_Color;
                GUI.DrawTexture(extPos, EditorGUIUtility.whiteTexture);
                GUI.color = color;
                GUI.Label(extPos, extension);
            }
        }

        internal static void SaveEPSettings(Color color, bool showExtension)
        {
            string key = colorSavedKey;
            EditorPrefs.SetFloat(key + "_r", color.r);
            EditorPrefs.SetFloat(key + "_g", color.g);
            EditorPrefs.SetFloat(key + "_b", color.b);
            EditorPrefs.SetFloat(key + "_a", color.a);
            EditorPrefs.SetBool(key + "_showExtension", showExtension);
        }

        internal static (Color , bool) LoadEPSettings()
        {
            string key = colorSavedKey;
            try
            {
                Color color = new Color();
                color.r = EditorPrefs.GetFloat(key + "_r", defaultColor.r);
                color.g = EditorPrefs.GetFloat(key + "_g", defaultColor.g);
                color.b = EditorPrefs.GetFloat(key + "_b", defaultColor.b);
                color.a = EditorPrefs.GetFloat(key + "_a", defaultColor.a);
                m_Color = color;
                m_ShowExtension = EditorPrefs.GetBool(key + "_showExtension", true);
                return (color, m_ShowExtension);
            }
            catch
            {
                m_Color = defaultColor;
                m_ShowExtension = true;
                return (defaultColor, true);
            }

        }
    }

    public class ExtensionPreviewerSettingsWindow : EditorWindow
    {
        private Color savedColor;
        private bool showExtension;

        [MenuItem("Tools/Mekanyanko Tools/Extension Previewer Settings")]
        public static void ShowWindow()
        {
            GetWindow<ExtensionPreviewerSettingsWindow>("Extension Previewer Settings");
        }

        private void OnGUI()
        {
            GUILayout.Label("Settings for File Previewer", EditorStyles.boldLabel);

            savedColor = EditorGUILayout.ColorField("BackGround Color", savedColor);
            showExtension = EditorGUILayout.Toggle("Show Extension", showExtension);

            if (GUILayout.Button("Save"))
            {
                // Save settings
                ExtensionPreviewerEditor.SaveEPSettings(savedColor, showExtension);
                Repaint();
            }
        }

        private void OnEnable()
        {
            var Result = ExtensionPreviewerEditor.LoadEPSettings();
            savedColor = Result.Item1;
            showExtension = Result.Item2;
        }
    }
}