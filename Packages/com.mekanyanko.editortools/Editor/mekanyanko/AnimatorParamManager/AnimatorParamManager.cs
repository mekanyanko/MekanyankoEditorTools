using MekanekoTools;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace MekanekoTools
{
    public class AnimatorParamManager : EditorWindow
    {
        #region 変数の宣言
        private AnimatorController targetObject;
        private Vector2 scrollPosition;
        private int all_States_Count;
        private int writeDefaults_ON_Count;
        private int writeDefaults_OFF_Count;
        private int emptyStates_Empty_Count;
        private AnimationClip emptyAnimationClip;
        #endregion 変数の宣言

        // メニューから呼び出す場合
        [MenuItem("Tools/Mekanyanko Tools/Animator Param Manager")]
        public static void OpenAnimatorParamManager()
        {
            AnimatorParamManager window = EditorWindow.GetWindow<AnimatorParamManager>("Animator Param Manager");
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Animator Param Manager", EditorStyles.boldLabel);

            EditorGUILayout.Space(10);

            // 変更チェックの実装
            targetObject = EditorGUILayout.ObjectField("Target Animator", targetObject, typeof(AnimatorController), true) as AnimatorController;


            if (targetObject == null) return;

            if (GUILayout.Button("AnimatorControllerの情報を取得する"))
            {

                CheckAnimatorController(targetObject);
            }

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField($"ALL States Count : {all_States_Count}");
            EditorGUILayout.LabelField($"Write Defaults ON Count : {writeDefaults_ON_Count}");
            EditorGUILayout.LabelField($"Write Defaults OFF Count : {writeDefaults_OFF_Count}");
            EditorGUILayout.LabelField($"emptyStates Count : {emptyStates_Empty_Count}");

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("WriteDefaultsをすべてONにする。") && writeDefaults_OFF_Count > 0)
            {
                // 確認のダイアログを表示する
                if (EditorUtility.DisplayDialog("確認", "WriteDefaultsをすべてONにしますか？", "OK", "Cancel"))
                {
                    SetWriteDefaults(targetObject, true);
                }
            }

            if (GUILayout.Button("WriteDefaultsをすべてOFFにする。") && writeDefaults_ON_Count > 0)
            {
                // 確認のダイアログを表示する
                if (EditorUtility.DisplayDialog("確認", "WriteDefaultsをすべてOFFにしますか？", "OK", "Cancel"))
                {
                    SetWriteDefaults(targetObject, false);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // 空のStateの処理

            if (GUILayout.Button("空のStateに空のアニメーションをセットする。") && emptyStates_Empty_Count > 0)
            {
                SetEmptyAnimationClip();

                emptyAnimationClip = GetOrCreateEmptyAnimation();
                AnimatorControllerLayer[] layers = targetObject.layers;
                foreach (AnimatorControllerLayer layer in layers)
                {
                    ChildAnimatorState[] animatorState = layer.stateMachine.states;

                    foreach (ChildAnimatorState state in animatorState)
                    {
                        if (state.state.motion == null)
                        {
                            state.state.motion = emptyAnimationClip;
                        }
                    }
                }

            }

            // AnimatorControllerの変更内容を保存して反映する。
            if (GUILayout.Button("変更を保存する。"))
            {
                EditorUtility.SetDirty(targetObject);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// AnimatorControllerの中のすべてのStateの情報を取得する
        /// </summary>
        /// <param name="animatorController"></param>
        private void CheckAnimatorController(AnimatorController animatorController)
        {
            all_States_Count = 0;
            writeDefaults_ON_Count = 0;
            writeDefaults_OFF_Count = 0;
            emptyStates_Empty_Count = 0;
            AnimatorControllerLayer[] layers = animatorController.layers;
            foreach (AnimatorControllerLayer layer in layers)
            {
                ChildAnimatorState[] animatorState = layer.stateMachine.states;

                foreach (ChildAnimatorState state in animatorState)
                {
                    all_States_Count++;

                    if (state.state.writeDefaultValues)
                    {
                        writeDefaults_ON_Count++;
                    }
                    else
                    {
                        writeDefaults_OFF_Count++;
                    }

                    if (state.state.motion == null)
                    {
                        emptyStates_Empty_Count++;
                    }

                }
            }
        }

        private void SetWriteDefaults(AnimatorController animatorController, bool value)
        {
            AnimatorControllerLayer[] layers = animatorController.layers;
            foreach (AnimatorControllerLayer layer in layers)
            {
                ChildAnimatorState[] animatorState = layer.stateMachine.states;

                foreach (ChildAnimatorState state in animatorState)
                {
                    state.state.writeDefaultValues = value;
                }
            }
            EditorUtility.SetDirty(targetObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void SetEmptyAnimationClip()
        {
            emptyAnimationClip = GetOrCreateEmptyAnimation();
            AnimatorControllerLayer[] layers = targetObject.layers;
            foreach (AnimatorControllerLayer layer in layers)
            {
                ChildAnimatorState[] animatorState = layer.stateMachine.states;

                foreach (ChildAnimatorState state in animatorState)
                {
                    if (state.state.motion == null)
                    {
                        state.state.motion = emptyAnimationClip;
                    }
                }
            }

            EditorUtility.SetDirty(targetObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static AnimationClip GetOrCreateEmptyAnimation()
        {
            string emptyAnimationPath = "Assets/Empty.anim";
            AnimationClip emptyAnimationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(emptyAnimationPath);

            if (emptyAnimationClip == null)
            {
                emptyAnimationClip = new AnimationClip();
                AssetDatabase.CreateAsset(emptyAnimationClip, emptyAnimationPath);
                emptyAnimationClip.name = "Empty";
                EditorUtility.SetDirty(emptyAnimationClip);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return emptyAnimationClip;
        }
    }
}