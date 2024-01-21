#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TaskAtlasNamespace
{
    [ExecuteInEditMode]
    public class TaskAtlasGizmos : MonoBehaviour
    {
        public List<Vector3> timeBallPos;
        public List<float> timeBallRadius;
        public List<Color> timeBallColor;

        public bool showTimeBall;

        float timer;

        void Start()
        {
        }

        void Update()
        {
            timer += Time.deltaTime;
        }

        public void EnableTimeBalls()
        {
            showTimeBall = true;
        }

        public void DisableTimeBalls()
        {
            showTimeBall = false;
        }

        public void NewTimeBalls()
        {
            timeBallPos = new List<Vector3>();
            timeBallRadius = new List<float>();
            timeBallColor = new List<Color>();
        }

        public void AddTimeBall(Vector3 pos, float radius, Color color)
        {
            timeBallPos.Add(pos);
            timeBallRadius.Add(radius);
            timeBallColor.Add(color);
        }

        private void OnDrawGizmos()

        {
            if (Application.isEditor)
            {
                if (Camera.current == GetComponent<Camera>() || Camera.current == SceneView.lastActiveSceneView.camera)
                    Gizmos.DrawSphere(transform.position, 1);

                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();
            }

            {
                for (int i = 0; i < timeBallPos.Count; i++)
                {
                    Color color = Handles.color;
                    Handles.color = new Color(timeBallColor[i].r, timeBallColor[i].g, timeBallColor[i].b, 0.5f);

                    Handles.DrawWireDisc(timeBallPos[i], Quaternion.Euler(-(float)timer * 8, 0, 0) * Vector3.up, timeBallRadius[i]);
                    Handles.DrawWireDisc(timeBallPos[i], Quaternion.Euler(45 + (float)timer * 4, 0, 0) * Vector3.up, timeBallRadius[i]);
                    Handles.DrawWireDisc(timeBallPos[i], Quaternion.Euler(90 - (float)timer * 8, 0, 0) * Vector3.up, timeBallRadius[i]);
                    Handles.DrawWireDisc(timeBallPos[i], Quaternion.Euler(135 + (float)timer * 4, 0, 0) * Vector3.up, timeBallRadius[i]);

                    Handles.DrawWireDisc(timeBallPos[i], Quaternion.Euler(0, (float)timer * 8, 0) * Vector3.right, timeBallRadius[i]);
                    Handles.DrawWireDisc(timeBallPos[i], Quaternion.Euler(0, 45 - (float)timer * 4, 0) * Vector3.right, timeBallRadius[i]);
                    Handles.DrawWireDisc(timeBallPos[i], Quaternion.Euler(0, 90 + (float)timer * 8, 0) * Vector3.right, timeBallRadius[i]);
                    Handles.DrawWireDisc(timeBallPos[i], Quaternion.Euler(0, 135 - (float)timer * 4, 0) * Vector3.right, timeBallRadius[i]);

                    Handles.color = new Color(timeBallColor[i].r, timeBallColor[i].g, timeBallColor[i].b, 0.1f);
                    Handles.SphereHandleCap(0, timeBallPos[i], Quaternion.identity, timeBallRadius[i] * 2, EventType.Repaint);
                    Handles.color = color;
                }
            }
        }
    }
}
#endif