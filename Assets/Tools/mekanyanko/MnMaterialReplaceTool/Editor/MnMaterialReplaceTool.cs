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

        // �R���e�L�X�g���j���[����Ăяo���ꍇ
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

        // ���j���[����Ăяo���ꍇ
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

            // �ύX�`�F�b�N�̎���
            EditorGUI.BeginChangeCheck();
            targetObject = EditorGUILayout.ObjectField("Replace Target(Avatar)", targetObject, typeof(GameObject), true) as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                ResetMaterialsParam();
            }

            if (targetObject == null) return;

            if (GUILayout.Button("�}�e���A�������Ď擾����"))
            {

                CheckMaterials(targetObject);
            }

            if (targetMaterials == null || newMaterials == null || oldMaterials == null) return;

            EditorGUILayout.Space(5);

            // Material�̃��X�g��\��
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
                EditorGUILayout.LabelField("�w��̃I�u�W�F�N�g�̂Ŏg�p����Ă���Material������܂���B");
                return;
            }

            // �X�y�[�X
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

            // Mesh�Ŏg�p����Ă���Material���擾���A�d�����폜�B
            var materials = renderers.SelectMany(r => r.sharedMaterials).Distinct().ToArray();

            if (materials.Length > 0)
            {
                // �}�e���A�����擾�ł�����AnewMaterials�ɂ��i�[
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
            // �p�����[�^�[������
            targetMaterials = null;
            newMaterials = null;
            oldMaterials = null;
        }

        private void ReplaceMaterials()
        {
            //oldMaterials��newMaterials�ŕύX����Ă�����̂������擾
            var changedMaterials = targetMaterials.Where((m, i) => m != newMaterials[i]).ToArray();

            //�ύX����Ă���Material��u��������
            foreach (var renderer in targetObject.GetComponentsInChildren<Renderer>())
            {
                var materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (changedMaterials.Contains(materials[i]))
                    {
                        // Undo���L�^
                        Undo.RecordObject(renderer, "Material Replace");

                        // �}�e���A����u������
                        // oldMaterials����materials[i]�̃C���f�b�N�X���擾
                        var index = System.Array.IndexOf(targetMaterials, materials[i]);

                        // newMaterials��index�ɂ���Material����
                        materials[i] = newMaterials[index];
                    }
                }
                renderer.sharedMaterials = materials;
            }
        }
    }
}
#endif