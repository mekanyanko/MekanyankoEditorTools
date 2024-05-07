using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace MekanekoTools
{
    public class AnimatorCreateTool
    {
        [MenuItem("Assets/Create/AnimatorFromSelectedClips")]
        public static void CreateAnimatorFromSelectedClips()
        {
            var selectedClips = Selection.GetFiltered(typeof(AnimationClip), SelectionMode.DeepAssets).OfType<AnimationClip>();

            if (!selectedClips.Any())
            {
                Debug.LogWarning("No AnimationClips selected.");
                return;
            }

            string path = AssetDatabase.GetAssetPath(selectedClips.First());
            string animatorControllerPath = System.IO.Path.GetDirectoryName(path) + "/GeneratedAnimatorController.controller";

            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(animatorControllerPath);

            foreach (var clip in selectedClips)
            {
                AnimatorState state = controller.layers[0].stateMachine.AddState("State_" + clip.name);
                state.motion = clip;
            }

            AssetDatabase.SaveAssets();
        }
    }
}