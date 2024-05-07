using UnityEditor;
using UnityEngine;

namespace MekanyankoTools
{
    public class GameObjectNames : Editor
    {
        [MenuItem("Tools/Mekanyanko Tools/GameObjectNames")]
        static void GameObjectName()
        {
            Debug.Log($"object Count : {Selection.gameObjects.Length}");

            var ObjName = "";
            foreach (GameObject go in Selection.gameObjects)
            {
                if (ObjName == "")
                {
                    ObjName = go.name;
                }
                else
                {
                    ObjName = ObjName + "\n" + go.name;
                }
            }
            Debug.Log(ObjName);
        }
    }
}