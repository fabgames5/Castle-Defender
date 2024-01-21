using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TaskAtlasNamespace
{
    public class TaskAtlasAPI : Editor
    {
        public static TA scene;


        public static void AddLandmark()
        {
            TaskAtlasEditorWindowNew.scene.landmarks.Add(
            new Landmark(
                Core.rtSS,
                Core.LandmarkCamera.transform.position,
                Core.LandmarkCamera.transform.rotation,
                Core.GetSVCOrthographicSize())
            );
        }

        public static void AddTaskToLandmark(string LandmarkName)
        {
            TaskAtlasEditorWindowNew.scene.landmarks.Add(
            new Landmark(
                Core.rtSS,
                Core.LandmarkCamera.transform.position,
                Core.LandmarkCamera.transform.rotation,
                Core.GetSVCOrthographicSize())
            );
        }
    }
}
