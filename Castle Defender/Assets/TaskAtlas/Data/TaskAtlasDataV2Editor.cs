#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TaskAtlasNamespace
{
    [CustomEditor(typeof(TaskAtlasDataV2))]
    public class TaskAtlasDataV2Editor : Editor
    {
        SerializedProperty TaskAtlasDataV2;
        string taPath = "";
        string[] allPaths;

        void OnEnable()
        {
            TaskAtlasDataV2 = serializedObject.FindProperty("TaskAtlasDataV2");
            if (taPath == "" | !AssetDatabase.IsValidFolder(taPath))
            {
                allPaths = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);
                for (int i = 0; i < allPaths.Count(); i++)
                {
                    allPaths[i] = allPaths[i].Replace("\\", "/");
                    if (allPaths[i].Contains("TaskAtlasDataDemo.asset"))
                    {

                        string p = Application.dataPath;
                        p.Replace("Assets", "");

                        taPath = allPaths[i].Substring(allPaths[i].IndexOf("Assets/"));
                        taPath = taPath.Substring(0, taPath.IndexOf("/TaskAtlas/") + 11);
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Do NOT delete this object, it contains ALL YOUR DATA!");
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            var logo = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/logo.png", typeof(Texture2D));
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Box(logo, GUIStyle.none, GUILayout.Width((681 + 64) / 3), GUILayout.Height((800 + 64) / 3));
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            //Rect r = GUILayoutUtility.GetLastRect();
            //DrawTexture(r, logo);

            if (GUILayout.Button("Open Landmark Editor"))
            {
                EditorApplication.ExecuteMenuItem("Window/ShrinkRay Entertainment/TaskAtlas/Landmark Editor");
            }

            if (GUILayout.Button("Contact ShrinkRay Entertainment"))
            {
                Application.OpenURL("mailto:shrinkrayentertainment@gmail.com?subject=Scene%20Pilot%20Question&body=Questions?%20Comments?%20Issues?");
            }

            if (GUILayout.Button("Get all ShrinkRay Assets at a HUGE discount"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/better-editor-deluxe-192015?aid=1011lf9gY&pubref=taskatlas");
            }

            if (GUILayout.Button("Email Support with Any Question"))
            {
                Application.OpenURL("mailto:shrinkrayentertainment@gmail.com?subject=Scene%20Pilot%20Question&body=Questions?%20Comments?%20Issues?");
            }

            if (GUILayout.Button("Loving it?  Leave a rating, it really helps!"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/task-atlas-ultimate-task-manager-sticky-notes-bookmarking-refere-185959#reviews");
            }
            serializedObject.ApplyModifiedProperties();
        }

        static void DrawTexture(Rect r, Texture2D texture)
        {
            if (texture == null) return;
            GUI.DrawTexture(r, texture);
        }
    }
}
#endif