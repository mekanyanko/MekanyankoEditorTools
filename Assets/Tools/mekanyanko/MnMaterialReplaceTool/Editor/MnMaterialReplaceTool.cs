using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace MekanekoTools
{
    public class MnMaterialReplaceTool : EditorWindow
    {
        private GameObject targetObject;
        private Vector2 scrollPosition;
        private Material[] targetMaterials;
        private Material[] newMaterials;
        private Material[] oldMaterials;

        // コンテキストメニューから呼び出す場合
        [MenuItem("GameObject/Material Replace Tool", false, 1)]
        public static void OpenMaterialReplacerFromContextMenu(MenuCommand menuCommand)
        {
            GameObject targetObject = menuCommand.context as GameObject;
            if (targetObject != null)
            {
                MnMaterialReplaceTool window = EditorWindow.GetWindow<MnMaterialReplaceTool>("Material Replace Tool");
                window.SetGameObject(targetObject);
                window.Show();
            }
        }

        public void SetGameObject(GameObject obj)
        {
            targetObject = obj;
            ResetMaterialsParam();
        }

        // メニューから呼び出す場合
        [MenuItem("Tools/Mekanyanko Tools/Material Replace Tool")]
        public static void OpenMaterialReplacerFromMenu()
        {
            MnMaterialReplaceTool window = EditorWindow.GetWindow<MnMaterialReplaceTool>("Material Replace Tool");
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Material Replace Tool", EditorStyles.boldLabel);

            EditorGUILayout.Space(10);

            // 変更チェックの実装
            EditorGUI.BeginChangeCheck();
            targetObject = EditorGUILayout.ObjectField("Replace Target(Avatar)", targetObject, typeof(GameObject), true) as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                ResetMaterialsParam();
            }

            if (targetObject == null) return;

            if (GUILayout.Button("マテリアル情報を再取得する"))
            {

                CheckMaterials(targetObject);
            }

            if (targetMaterials == null || newMaterials == null || oldMaterials == null) return;

            EditorGUILayout.Space(5);

            // Materialのリストを表示
            if (newMaterials.Length > 0)
            {
                EditorGUILayout.LabelField($"Target Name : " + targetObject.name);
                EditorGUILayout.LabelField($"Material Count : " + newMaterials.Length);
                EditorGUILayout.Space(5);
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = (Material)EditorGUILayout.ObjectField(newMaterials[i], typeof(Material), true);
                }
                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.LabelField("指定のオブジェクトので使用されているMaterialがありません。");
                return;
            }

            // スペース
            EditorGUILayout.Space(5);

            if (GUILayout.Button("Replace Materials"))
            {
                ReplaceMaterials();
            }
        }

        private void CheckMaterials(GameObject targetObject)
        {
            ResetMaterialsParam();

            var renderers = targetObject.GetComponentsInChildren<Renderer>();

            // Meshで使用されているMaterialを取得し、重複を削除。
            var materials = renderers.SelectMany(r => r.sharedMaterials).Distinct().ToArray();

            if (materials.Length > 0)
            {
                // マテリアルが取得できたら、newMaterialsにも格納
                if (materials != null && materials.Length > 0)
                {
                    targetMaterials = new Material[materials.Length];
                    newMaterials = new Material[materials.Length];
                    oldMaterials = new Material[materials.Length];
                    for (int i = 0; i < materials.Length; i++)
                    {
                        targetMaterials[i] = materials[i];
                        newMaterials[i] = materials[i];
                        oldMaterials[i] = materials[i];
                    }
                }
            }
        }

        private void ResetMaterialsParam()
        {
            // パラメーター初期化
            targetMaterials = null;
            newMaterials = null;
            oldMaterials = null;
        }

        private void ReplaceMaterials()
        {
            //oldMaterialsとnewMaterialsで変更されているものだけを取得
            var changedMaterials = targetMaterials.Where((m, i) => m != newMaterials[i]).ToArray();

            //変更されているMaterialを置き換える
            foreach (var renderer in targetObject.GetComponentsInChildren<Renderer>())
            {
                var materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (changedMaterials.Contains(materials[i]))
                    {
                        // Undoを記録
                        Undo.RecordObject(renderer, "Material Replace");

                        // マテリアルを置き換え
                        // oldMaterials内でmaterials[i]のインデックスを取得
                        var index = System.Array.IndexOf(targetMaterials, materials[i]);

                        // newMaterialsのindexにあるMaterialを代入
                        materials[i] = newMaterials[index];
                    }
                }
                renderer.sharedMaterials = materials;
            }
        }
    }
}
#endif