using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TaskAtlasNamespace
{
    [InitializeOnLoad]
    [CustomEditor(typeof(TaskAtlasType))]
    public class TaskAtlasSceneView : Editor
    {
        static public TA scene;

        static List<string> tags;
        static List<string> tagList, tagListCopy;

        static GUIStyle sBackgroundPanels;

        static Color32
        cBackground = new Color32(128, 0, 0, 255);

        static float screenWidth, screenHeight;

        public static Atlas atlas;
        static bool modeDebug = false;
        static Bounds boundsScene;

        static GUIStyle s;
        static public bool isInit;


        static float lerp = 1.0f, lerpMin = 1f, lerpMax = 1.5f, lerpTarget = 1.2f;

        #region TimeTracking
        public enum TimeTracking { active, idle, sleeping };
        static public TimeTracking timeTracking;
        static public double timer, timer360, timerTrackPosition, timerIdleTimeout = 60 * 15, timerSleepTimeout = 60 * 60;
        static public Vector3 prevPosition;
        static public double editorDeltaTime = 0f;
        static public double lastTimeSinceStartup = 0f;
        #endregion


        static TaskAtlasSceneView()
        {
            EditorApplication.update += EditorUpdate;
            EditorApplication.quitting += EditorQuit;

            SceneView.duringSceneGui -= OnScene;
            SceneView.duringSceneGui += OnScene;

            isInit = false;
        }


        static void Init()
        {
            if (!Core.isActive) return;

            if (Core.dataV2 == null ||
                (Core.isV2 &&
                Core.dataV2.scene.Count == 0 &&
                Core.dataV2 != null))
            {
                Core.isV2 = false;
                if (Core.dataV2 != null) Core.dataV2.isMigrated = false;
                Core.Init();
            }

            scene = Core.dataV2.scene[Core.dataV2.sceneIndex];

            RefreshSettings();
            scene.UpdateAllProgress(scene.stickyFont);
            scene.UpdateAllThumbnails();

            scene.name = SceneManager.GetActiveScene().path;

            //for (int i = 0; i < scene.history.Count; i++)
            //{
            //    var d = scene.history[i];
            //    d.GetThumbnail();
            //}
            for (int i = 0; i < scene.landmarks.Count; i++)
            {
                var d = scene.landmarks[i];
                d.GetThumbnail();
                d.SetColor(d.GizmoColor);

                // var dd = scene.landmarks[i].gallery;
                // dd.GetThumbnails();

            }
            for (int x = 0; x < scene.landmarks.Count; x++)
            {
                scene.landmarks[x].UpdateProgress(scene.stickyFont);
                for (int y = 0; y < scene.landmarks[x].tasks.Count; y++)
                {
                    var d = scene.landmarks[x].tasks[y];
                    d.SetColor(d.color);
                    if (d.createdDateDT == 0)
                    {
                        if (d.createdDate != "")
                        {
                            d.createdDateDT = long.Parse(d.createdDate);
                        }
                        else
                        {
                            d.createdDateDT = DateTime.Now.Ticks;
                        }
                    }
                    if (d.dueDateDT == 0)
                    {
                        if (d.dueDate != "")
                        {
                            d.dueDateDT = long.Parse(d.dueDate);
                        }
                        else
                        {
                            d.dueDateDT = DateTime.Now.Ticks;
                            d.alert = false;
                        }
                    }
                }
            }

            GUISkin skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
            cBackground = EditorGUIUtility.isProSkin ?
                (Color)new Color32(56, 56, 56, 255) :
                (Color)new Color32(194, 194, 194, 255);
            Core.tBackgroundMain = MakeTex(1, 1, cBackground);

            var col = EditorGUIUtility.isProSkin ?
                (Color)new Color32(68, 68, 68, 255) :
                (Color)new Color32(233, 233, 233, 255);
            Core.tBackgroundPanels = MakeTex(1, 1, col);

            sBackgroundPanels = StyleCheck();
            if (EditorGUIUtility.isProSkin)
            {
                sBackgroundPanels.normal.background = Core.tBackgroundPanels;
            }
            else
            {
                sBackgroundPanels.normal.background = Core.tBackgroundPanels;
            }
            isInit = true;
        }


        //
        static void EditorUpdate()
        {
            if (scene == null) return;
            if (Core.dataV2 == null) { Core.Init(); Init(); }
            if (Core.isBuild) return;
            SetEditorDeltaTime();
            Vector3 currentPosition = Core.GetSVCameraPosition();
            timer += editorDeltaTime;
            timer360 += editorDeltaTime;

            if (prevPosition == currentPosition)
            {
                timerTrackPosition += editorDeltaTime;
                if (timerTrackPosition > timerIdleTimeout) timeTracking = TimeTracking.idle;
                if (timerTrackPosition > timerSleepTimeout) timeTracking = TimeTracking.sleeping;
            }
            else
            {
                timerTrackPosition = 0;
                timeTracking = TimeTracking.active;
                prevPosition = Core.GetSVCameraPosition();
            }
            if (timer > 60)
            {
                timer = 0;

                for (int l = 0; l < scene.landmarks.Count; l++)
                {
                    scene.landmarks[l].UpdateTaskDistances((int)timeTracking);
                }
            }

            if (timer360 > 360)
            {
                timer360 = 0;
            }
        }

        static void EditorQuit()
        {
            scene.UpdateAllProgress(scene.stickyFont);
            //Core.RemoveHelperCams();
            //Core.SaveData();
        }

        static private void SetEditorDeltaTime()
        {
            if (lastTimeSinceStartup == 0f)
            {
                lastTimeSinceStartup = EditorApplication.timeSinceStartup;
            }
            editorDeltaTime = (EditorApplication.timeSinceStartup - lastTimeSinceStartup);
            lastTimeSinceStartup = EditorApplication.timeSinceStartup;
        }

        static public void RefreshSettings()
        {
            if (Core.dataV2 == null) Core.Init();

            //Core.RefreshHelperCams();
            //Core.RefreshIcons();

            if (scene == null) Init();
            if (atlas == null) atlas = scene.atlas;

            tags = new List<string>();
            tagList = new List<string>();
            tagList.Add("Show All");

            if (Core.dataV2.tags == null) Core.dataV2.tags = new List<Tags>();
            for (int i = 0; i < Core.dataV2.tags.Count; i++)
            {
                tagList.Add(Core.dataV2.tags[i].name);
            }
            tagList.Add("Has Any Tag");

            if (sBackgroundPanels == null)
            {
                sBackgroundPanels = StyleCheck();
                sBackgroundPanels.normal.background = Core.tBackgroundMain;
            }
        }

        static void HorizontalLine(Color color)
        {
            if (EditorGUIUtility.isProSkin)
            {
                if (color == Color.white) color = Color.black;
                if (color == Color.black) color = Color.white;
            }

            GUIStyle horizontalLine;
            horizontalLine = new GUIStyle();
            horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
            horizontalLine.margin = new RectOffset(0, 0, 4, 4);
            horizontalLine.fixedHeight = 1;
            var c = GUI.color;
            GUI.color = color;
            GUILayout.Box(GUIContent.none, horizontalLine);
            GUI.color = c;
        }


        static GUIStyle style;
        static bool atlasCancelled = false, wasRepaint = false;
        static public void OnScene(SceneView sceneview)
        {
            if (Application.isPlaying) return;
            if (!TaskAtlasEditorWindowNew.sceneFound) return;

            if (Core.tTaskSticky == null) Core.RefreshIcons();
            if (!isInit)
            {
                Core.Init();
                Init();
            }
            if (Core.isBuild || !Core.isInstalled || atlas == null) return;
            if (scene.landmarks.Count == 0)
            {
                if (scene.name != SceneManager.GetActiveScene().path)
                {
                    Core.dataV2.sceneIndex = Core.CheckSceneExists();
                    Core.Init();
                    Init();
                    return;
                }
            }


            screenWidth = sceneview.position.width;
            screenHeight = sceneview.position.height;

            Handles.BeginGUI();

            RefreshSettings();
            lerp = Mathf.Lerp(lerp, lerpTarget, 0.05f);
            if (lerp >= lerpTarget - 0.1f) lerpTarget = lerpMin;
            if (lerp <= lerpTarget + 0.1f) lerpTarget = lerpMax;

            Core.gizmos.NewTimeBalls();

            //GUI.Label(new Rect(0, 0, 200, 20), Core.GetSVCameraPosition().ToString());

            #region Landmarks.Gizmos
            for (int i = 0; i < scene.landmarks.Count; i++)
            {
                var landmarks = scene.landmarks[i];

                if (scene.showLandmarkLabels)
                {
                    if (!atlas.enabled)
                    {
                        if (landmarks.showGizmo)
                        {
                            Color c = Color.white;
                            float dist;
                            if (sceneview.in2DMode)
                            {
                                dist = Vector2.Distance(Core.GetSVCameraPosition(), landmarks.position);
                            }
                            else
                            {
                                dist = Vector3.Distance(Core.GetSVCameraPosition(), landmarks.position);
                            }


                            float alpha;

                            if (dist < landmarks.fadeStart)
                            {
                                float f = dist / landmarks.fadeStart;
                                if (f > 1) alpha = 1; else alpha = f;
                            }
                            else
                            {
                                float f = 1 - ((dist - landmarks.fadeStart) / (landmarks.fadeEnd - landmarks.fadeStart));
                                if (f > 0) alpha = f; else alpha = 0f;
                            }

                            if (landmarks.fadeGizmo)
                            {
                                c = new Color(landmarks.GizmoColor.r, landmarks.GizmoColor.g, landmarks.GizmoColor.b, alpha);
                            }
                            else
                            {
                                float f = dist / 15;
                                if (f > 1) alpha = 1; else alpha = f;
                                c = new Color(landmarks.GizmoColor.r, landmarks.GizmoColor.g, landmarks.GizmoColor.b, alpha);
                            }

                            s = StyleCheck();
                            s.fontSize = 18;
                            s.fontStyle = FontStyle.Bold;
                            s.alignment = TextAnchor.UpperCenter;
                            s.wordWrap = true;
                            s.normal.textColor = c;

                            if (Event.current.type == EventType.Layout)
                                if (landmarks.isFloating && landmarks.floatingPositionObject != null)
                                {
                                    landmarks.wsPosition = HandleUtility.WorldToGUIPointWithDepth(landmarks.floatingPositionObject.transform.position);
                                }
                                else
                                {
                                    landmarks.wsPosition = HandleUtility.WorldToGUIPointWithDepth(landmarks.position);
                                }
                            landmarks.wsPosition += landmarks.wsOffset;

                            Vector3 svc = Core.GetSVCameraPosition(), lpos = landmarks.position;
                            Vector3 svcFloor = new Vector3(Mathf.Floor(svc.x), Mathf.Floor(svc.y), Mathf.Floor(svc.z)), lposFloor = new Vector3(Mathf.Floor(lpos.x), Mathf.Floor(lpos.y), Mathf.Floor(lpos.z));

                            if (atlasCancelled)
                            {
                                if (Event.current.type == EventType.Repaint) wasRepaint = true;
                                if (Event.current.type == EventType.Layout && wasRepaint)
                                {
                                    atlasCancelled = false;
                                    wasRepaint = false;
                                }
                            }

                            if (!atlasCancelled)
                            {
                                if (svcFloor != lposFloor)
                                    if (sceneview.in2DMode && landmarks.viewState == Landmark.ViewState.is2D)
                                    {
                                        if (landmarks.wsPosition.z > 0)
                                        {
                                            s = StyleCheck();

                                            s.fontSize = (int)(14 * (alpha + 0.5));
                                            s.fontStyle = FontStyle.Italic;
                                            s.alignment = TextAnchor.LowerCenter;
                                            s.normal.textColor = c;

                                            var size = s.CalcSize(new GUIContent(dist.ToString("F1") + "m"));
                                            if (landmarks.wsPosition.x < (size.x / 2) - 16) landmarks.wsPosition.x = (size.x / 2) - 16;
                                            if (landmarks.wsPosition.x > screenWidth - (size.x * 2)) landmarks.wsPosition.x = screenWidth - (size.x * 2);
                                            if (landmarks.wsPosition.y < 0) landmarks.wsPosition.y = size.x - 30;
                                            if (landmarks.wsPosition.y > screenHeight) landmarks.wsPosition.y = screenHeight - size.x;
                                            GUI.Label(new Rect(landmarks.wsPosition.x - 64, landmarks.wsPosition.y - 16, 256, 20), dist.ToString("F1") + "m", s);

                                            size = s.CalcSize(new GUIContent(landmarks.title));
                                            GUI.Label(new Rect(landmarks.wsPosition.x - 64, landmarks.wsPosition.y, 256, 20), landmarks.title, s);
                                        }
                                    }
                                    else if (!sceneview.in2DMode && landmarks.viewState == Landmark.ViewState.is3D)
                                    {
                                        if (landmarks.wsPosition.z > 0)
                                        {
                                            GUI.Label(new Rect(landmarks.wsPosition.x - 64, landmarks.wsPosition.y, 128, 20), landmarks.title, s);
                                            s = StyleCheck();
                                            s.fontSize = 14;
                                            s.fontStyle = FontStyle.Italic;
                                            s.alignment = TextAnchor.LowerCenter;
                                            s.normal.textColor = c;
                                            GUI.Label(new Rect(landmarks.wsPosition.x - 64, landmarks.wsPosition.y - 16, 128, 20), dist.ToString("F1") + "m", s);
                                        }
                                    }
                            }
                            else
                            {


                            }
                        }
                    }
                }
                #region HandlesStickyTasks

                if (!atlas.enabled)
                {
                    Dictionary<int, float> sortOrder = new Dictionary<int, float>();

                    for (int t = 0; t < landmarks.tasks.Count; t++)
                    {
                        var tasks = landmarks.tasks[t];

                        if (scene.showStickies)
                        {
                            if (tasks.isSticky)
                            {
                                tasks.isVisible = false;
                                tasks.wsPosition = HandleUtility.WorldToGUIPointWithDepth(tasks.position);

                                if (landmarks.viewState == Landmark.ViewState.is2D && sceneview.in2DMode)
                                {
                                    tasks.wsPosition = new Vector3(tasks.wsPosition.x, tasks.wsPosition.y, Mathf.Abs(tasks.position.z));
                                    if ((tasks.wsPosition.x > 0 && tasks.wsPosition.y > 0) &&
                                        (tasks.wsPosition.x < screenWidth && tasks.wsPosition.y < screenHeight) &&
                                        (sceneview.camera.orthographicSize < landmarks.orthographicSize * 5 && sceneview.camera.orthographicSize > landmarks.orthographicSize / 5))
                                        tasks.isVisible = true;
                                    else tasks.isVisible = false;
                                }
                                else if (landmarks.viewState == Landmark.ViewState.is3D && !sceneview.in2DMode)
                                {
                                    if (tasks.wsPosition.z > 0)
                                        tasks.isVisible = true;
                                    else tasks.isVisible = false;
                                }

                                if (landmarks.viewState == Landmark.ViewState.is2D)
                                {
                                    tasks.dist = 0.01f;
                                    float offsetX, offsetY;
                                    offsetX = Mathf.Abs(((screenWidth / 2) - tasks.wsPosition.x));
                                    offsetY = Mathf.Abs(((screenHeight / 2) - tasks.wsPosition.y));
                                    tasks.dist = (offsetX / screenWidth + offsetY / screenHeight) / 1.1f;
                                    float scale = (Mathf.Abs(1 - (sceneview.camera.orthographicSize / landmarks.orthographicSize)) + 2);

                                    if (tasks.dist < 0.1f) tasks.dist = 0.1f;

                                    if (scale > 2.2)
                                    {
                                        tasks.dist *= scale;
                                        tasks.alpha = 0.25f;
                                    }
                                    else
                                    {
                                        tasks.alpha = 1;
                                    }
                                }
                                else
                                {
                                    tasks.dist = Vector3.Distance(Core.GetSVCameraPosition(), tasks.position);
                                }
                            }
                            else
                            {
                                tasks.dist = 999999;
                            }

                        }

                        sortOrder.Add(t, tasks.dist);


                    }

                    var sortedDict = from entry in sortOrder orderby entry.Value descending select entry;

                    foreach (var kvp in sortedDict)
                    {
                        int t = kvp.Key;
                        var tasks = landmarks.tasks[t];


                        #region DistanceGizmo
                        if (scene.showTaskGizmos)
                        {
                            if (Core.gizmos != null)
                                if (tasks.showDistanceSphere | (tasks.showDistanceSphere & scene.atlas.enabled & scene.atlas.landmarkSelected != -1))
                                {
                                    Core.gizmos.EnableTimeBalls();
                                    Core.gizmos.AddTimeBall(tasks.position, tasks.timeTrackingDistance, tasks.color);
                                }
                                else
                                {
                                    Core.gizmos.DisableTimeBalls();
                                }
                        }
                        #endregion

                        if (tasks.selectedSubTask > tasks.subTasks.Count - 1) tasks.selectedSubTask = -1;
                        if (scene.showStickies)
                        {
                            if (tasks.isSticky)
                            {

                                if (tasks.progress < 0) tasks.progress = 0;
                                float size = HandleUtility.GetHandleSize(tasks.position) * 2f;
                                Vector3 snap = Vector3.one * 0.5f;

                                //Debug.Log(tasks.moveMode);
                                //Debug.Log(scene.atlas.enabled);
                                if (tasks.moveMode & !scene.atlas.enabled)
                                {
                                    Vector3 cameraToTarget = Core.GetSVCameraPosition() - tasks.position;
                                    Quaternion billboardOrientation = Quaternion.LookRotation(cameraToTarget, SceneView.lastActiveSceneView.camera.transform.up);

                                    Handles.EndGUI();
                                    tasks.position = Handles.PositionHandle(tasks.position, Quaternion.identity);
                                    Handles.BeginGUI();
                                }


                                if (tasks.isVisible)
                                {
                                    float height = 100f, width = 100f;

                                    float Scaled(float f)
                                    {
                                        if (landmarks.viewState == Landmark.ViewState.is2D)
                                        {
                                            return ((f / (tasks.dist / f)) / 300);
                                        }
                                        else
                                        {
                                            return (f / (tasks.dist / f)) * (tasks.scale / 100);
                                        }
                                    }

                                    Rect AddRect(Rect r1, Rect r2)
                                    {
                                        Rect ret = new Rect(r1.x + r2.x, r1.y + r2.y, r1.width + r2.width, r1.height + r2.height);
                                        return ret;
                                    }

                                    float sWidth = Scaled(width);
                                    float sHeight = Scaled(height);

                                    {
                                        Color guiColor = GUI.color;
                                        //Debug.Log(tasks.name);
                                        if (tasks.moveMode)
                                        {
                                            GUI.color = new Color(guiColor.r, guiColor.g, guiColor.b, .1f);
                                            GUI.enabled = false;
                                        }
                                        else
                                        {
                                            if (landmarks.viewState == Landmark.ViewState.is2D && sceneview.in2DMode)
                                            {
                                                GUI.color = new Color(guiColor.r, guiColor.g, guiColor.b, tasks.alpha);
                                            }
                                            else
                                            {
                                                float alpha = 0f;
                                                float fs = 0f, fe = 0f;
                                                if (tasks.useDefaultFadeDistance)
                                                {
                                                    fs = landmarks.taskFadeStart;
                                                    fe = landmarks.taskFadeEnd;
                                                }
                                                else
                                                {
                                                    fs = tasks.fadeStart;
                                                    fe = tasks.fadeEnd;
                                                }

                                                if (tasks.dist >= fs && tasks.dist <= fe) alpha = 1f;

                                                GUI.color = new Color(guiColor.r, guiColor.g, guiColor.b, alpha);
                                            }
                                            GUI.enabled = true;
                                        }

                                        #region StickyImage
                                        Rect areaRect = new Rect(tasks.wsPosition.x - (sHeight / 2), tasks.wsPosition.y - (sWidth / 2), sWidth, sHeight);
                                        Rect areaRectPadding = AddRect(areaRect, new Rect(Scaled(32), Scaled(32), -Scaled(32), -Scaled(32)));
                                        if (scene.stickyBackground == StickyBackground.note) GUI.DrawTexture(areaRect, Core.tTaskSticky);
                                        else
                                        {
                                            if (areaRect.width != 0)
                                                GUI.DrawTexture(areaRect, Core.MakeTex(1, 1, tasks.color));
                                        }

                                        #endregion


                                        Rect rPos;
                                        Color guiColor2 = GUI.color;
                                        GUI.color = new Color(guiColor2.r, guiColor2.g, guiColor2.b, .3f);

                                        #region BackgroundImage
                                        rPos = areaRectPadding;
                                        if (tasks.priority == Task.Priority.urgent)
                                        {

                                            GUI.DrawTexture(rPos, Core.tIconExclaim);

                                        }
                                        else
                                        {
                                            GUI.DrawTexture(rPos, Core.tIconCheck);
                                        }
                                        GUI.color = guiColor2;
                                        #endregion

                                        #region TopStats
                                        Color c;
                                        c = tasks.tColor.GetPixel(0, 0);
                                        if (c == null) c = Color.black;
                                        float f = 1.5f;
                                        c = new Color(c.r * f, c.g * f, c.b * f);
                                        Texture2D cTex = Core.MakeTex(1, 1, c);

                                        rPos = new Rect(areaRect.x + Scaled(22), areaRect.y + Scaled(36), Scaled(52), Scaled(24));
                                        //Debug.Log(rPos);

                                        if (scene.stickyBackground == StickyBackground.note) { GUI.DrawTexture(rPos, Core.tBlue); }
                                        else { if ((int)rPos.width != 0 & (int)rPos.height != 0) GUI.DrawTexture(rPos, Core.MakeTex(1, 1, c)); }

                                        rPos = AddRect(areaRect, new Rect(-Scaled(56), Scaled(33), 0, 0));
                                        if (tasks.tStage == null) landmarks.UpdateProgress(scene.stickyFont);
                                        GUI.DrawTexture(rPos, tasks.tStage);


                                        rPos = new Rect(areaRect.x + Scaled(59), areaRect.y + Scaled(36), Scaled(53), Scaled(24));
                                        if (scene.stickyBackground == StickyBackground.note) { GUI.DrawTexture(rPos, Core.tTeal); }
                                        else { if ((int)rPos.width != 0 & (int)rPos.height != 0) GUI.DrawTexture(rPos, Core.MakeTex(1, 1, c)); }


                                        rPos = AddRect(areaRect, new Rect(-Scaled(8), Scaled(33), 0, 0));
                                        GUI.DrawTexture(rPos, tasks.tPriority);


                                        rPos = new Rect(areaRect.x + Scaled(81), areaRect.y + Scaled(36), Scaled(52), Scaled(24));
                                        if (scene.stickyBackground == StickyBackground.note) { GUI.DrawTexture(rPos, Core.tRed); }
                                        else { if ((int)rPos.width != 0 & (int)rPos.height != 0) GUI.DrawTexture(rPos, Core.MakeTex(1, 1, c)); }


                                        rPos = AddRect(areaRect, new Rect(Scaled(53), Scaled(33), 0, 0));
                                        GUI.DrawTexture(rPos, tasks.tActiveTime);
                                        #endregion

                                        #region ProgressBar
                                        rPos = new Rect(areaRect.x + Scaled(32), areaRect.y + areaRect.height - Scaled(30), sWidth - Scaled(48), Scaled(16));

                                        if (scene.stickyBackground == StickyBackground.note) { ProgressBar(rPos, ((float)tasks.progress / 100), Core.tGreenDark, Core.tGreenP); }
                                        else { ProgressBar(rPos, ((float)tasks.progress / 100), tasks.tColorDark, cTex); }
                                        //, tasks.tColorDark);
                                        #endregion

                                        #region MainText
                                        if (tasks.selectedSubTask == -1)
                                        {
                                            if (scene.stickyBackground == StickyBackground.note) { rPos = AddRect(areaRect, new Rect(Scaled(24), Scaled(20), -Scaled(40), -Scaled(54))); }
                                            else { rPos = AddRect(areaRect, new Rect(Scaled(24), Scaled(14), -Scaled(40), -Scaled(54))); }

                                            GUI.DrawTexture(rPos, tasks.tTaskName);
                                            rPos = AddRect(areaRect, new Rect(Scaled(24), Scaled(40), -Scaled(40), -Scaled(60)));
                                            GUI.DrawTexture(rPos, tasks.tTaskDescription);
                                        }
                                        else
                                        {
                                            if (scene.stickyBackground == StickyBackground.note) { rPos = new Rect(areaRect.x + Scaled(32), areaRect.y + Scaled(24), sWidth - Scaled(48), Scaled(24)); }
                                            else { rPos = new Rect(areaRect.x + Scaled(32), areaRect.y + Scaled(16), sWidth - Scaled(48), Scaled(24)); }

                                            s = new GUIStyle("button");

                                            if (scene.stickyBackground == StickyBackground.note) { s.normal.background = Core.tYellow; }
                                            else { s.normal.background = tasks.tColorDark; }


                                            if (GUI.Button(rPos, "", s))
                                            {
                                                tasks.selectedSubTask = -1;
                                            }
                                            if (tasks.selectedSubTask != -1)
                                            {
                                                if (scene.stickyBackground == StickyBackground.note) { rPos = new Rect(areaRect.x, areaRect.y + Scaled(20), sWidth, sHeight); }
                                                else { rPos = new Rect(areaRect.x, areaRect.y + Scaled(12), sWidth, sHeight); }

                                                GUI.DrawTexture(rPos, tasks.tExit);

                                                rPos = AddRect(areaRect, new Rect(Scaled(24), Scaled(43), -Scaled(40), -Scaled(64)));
                                                GUI.DrawTexture(rPos, tasks.subTasks[tasks.selectedSubTask].tTaskName);
                                            }
                                        }
                                        #endregion

                                        #region ShowSubTasks
                                        if (tasks.subTasks.Count > 0)
                                        {
                                            if (tasks.selectedSubTask == -1)
                                            {
                                                rPos = new Rect(areaRect.x + Scaled(32), areaRect.y + areaRect.height - Scaled(48), sWidth - Scaled(48), Scaled(24));
                                                s = new GUIStyle("button");

                                                if (scene.stickyBackground == StickyBackground.note) { s.normal.background = Core.tYellow; }
                                                else { s.normal.background = tasks.tColorDark; }

                                                if (GUI.Button(rPos, "", s))
                                                {
                                                    tasks.selectedSubTask = 0;
                                                }
                                                rPos = new Rect(areaRect.x + Scaled(16), areaRect.y + areaRect.height - Scaled(50), sWidth, sHeight);
                                                GUI.DrawTexture(rPos, tasks.tSubTasks);
                                            }
                                            else
                                            {
                                                rPos = new Rect(areaRect.x + Scaled(32), areaRect.y + areaRect.height - Scaled(48), Scaled(32), Scaled(24));
                                                s = new GUIStyle("button");
                                                if (scene.stickyBackground == StickyBackground.note) { s.normal.background = Core.tYellow; }
                                                else { s.normal.background = tasks.tColorDark; }

                                                if (GUI.Button(rPos, Core.tArrowLeft, s))
                                                {
                                                    if (tasks.selectedSubTask != 0) tasks.selectedSubTask--;
                                                }

                                                rPos = new Rect(areaRect.x + Scaled(48), areaRect.y + areaRect.height - Scaled(48), Scaled(72), Scaled(24));
                                                s = StyleCheck("button");
                                                bool prev = tasks.subTasks[tasks.selectedSubTask].complete;

                                                s.normal.background = Core.tRedDark;
                                                if (prev)
                                                {
                                                    if (scene.stickyBackground == StickyBackground.note) { s.normal.background = Core.tGreenP; }
                                                    else { s.normal.background = cTex; }
                                                }
                                                tasks.subTasks[tasks.selectedSubTask].complete = GUI.Toggle(rPos, tasks.subTasks[tasks.selectedSubTask].complete, "", s);
                                                rPos = new Rect(areaRect.x, areaRect.y + areaRect.height - Scaled(50), sWidth, sHeight);
                                                GUI.DrawTexture(rPos, landmarks.tCompleted);
                                                if (prev != tasks.subTasks[tasks.selectedSubTask].complete)
                                                {
                                                    landmarks.UpdateProgress(scene.stickyFont);
                                                    TaskAtlasEditorWindowNew.QueueRepaint();
                                                }

                                                rPos = new Rect(areaRect.x + sWidth - Scaled(48), areaRect.y + areaRect.height - Scaled(48), Scaled(32), Scaled(24));
                                                s = new GUIStyle("button");

                                                if (scene.stickyBackground == StickyBackground.note) { s.normal.background = Core.tYellow; }
                                                else { s.normal.background = tasks.tColorDark; }

                                                if (GUI.Button(rPos, Core.tArrowRight, s))
                                                {
                                                    if (tasks.selectedSubTask != tasks.subTasks.Count - 1) tasks.selectedSubTask++;
                                                }
                                            }
                                        }
                                        #endregion

                                        #region GoToTask
                                        rPos = new Rect(areaRect.x + Scaled(32), areaRect.y + areaRect.height - Scaled(40), (sWidth - Scaled(48)) / 2, Scaled(24));
                                        s = new GUIStyle("button");
                                        if (scene.stickyBackground == StickyBackground.note) { s.normal.background = Core.tYellow; }
                                        else { s.normal.background = tasks.tColorDark; }

                                        if (GUI.Button(rPos, "", s))
                                        {
                                            scene.landmarkDetailOpen = true;
                                            scene.selectedLandmark = i;
                                            scene.landmarkDetailState = TA.LandmarkDetailState.tasks;
                                            scene.zoomToTaskID = tasks.createdDate;
                                            TaskAtlasEditorWindowNew.QueueRepaint();
                                        }
                                        rPos = new Rect(areaRect.x - Scaled(45), areaRect.y + areaRect.height - Scaled(42), sWidth, sHeight);
                                        GUI.DrawTexture(rPos, landmarks.tGoToTask);
                                        #endregion

                                        #region MoveSticky
                                        s = StyleCheck("button");
                                        rPos = new Rect(areaRect.x + Scaled(70), areaRect.y + areaRect.height - Scaled(40), (sWidth - Scaled(48)) / 2, Scaled(24));
                                        bool p = GUI.enabled; c = GUI.color;
                                        GUI.enabled = true;
                                        if (tasks.moveMode) GUI.color = Color.white;

                                        if (scene.stickyBackground == StickyBackground.note) { s.normal.background = Core.tYellow; }
                                        else { s.normal.background = tasks.tColorDark; }


                                        tasks.moveMode = GUI.Toggle(rPos, tasks.moveMode, "", s);
                                        rPos = new Rect(areaRect.x + Scaled(43), areaRect.y + areaRect.height - Scaled(42), sWidth, sHeight);
                                        GUI.DrawTexture(rPos, landmarks.tMoveSticky);
                                        GUI.enabled = p;
                                        if (tasks.moveMode) GUI.color = c;
                                        #endregion

                                        GUI.color = guiColor;
                                        GUI.enabled = true;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion



                #endregion
                #region AtlasMenu
                if (atlas.enabled)
                    if (atlas.state == AtlasState.tasks)
                        GUI.DrawTexture(new Rect(screenWidth - 128, 128 + 64, 128, 240), Core.tGreen);
                int yy = 128;
                if (Core.GetSceneView().in2DMode) yy = 8;
                GUILayout.BeginArea(new Rect(screenWidth - 128, yy + 64, 128, 128 * 4));
                {

                    if (atlas.enabled)
                    {
                        if (atlas.isZooming)
                        {
                            if (Core.GetSVCameraPosition() == atlas.legitPosition) atlas.isZooming = false;
                        }
                        else
                        {
                            Vector3 svc = Core.GetSVCameraPosition(), apos = atlas.legitPosition;
                            Vector3 svcFloor = new Vector3(Mathf.Floor(svc.x), Mathf.Floor(svc.y), Mathf.Floor(svc.z)),
                                    aposFloor = new Vector3(Mathf.Floor(apos.x), Mathf.Floor(apos.y), Mathf.Floor(apos.z));

                            if (sceneview.in2DMode)
                            {
                                svcFloor = aposFloor;

                            }

                            if (svcFloor != aposFloor && Event.current.type == EventType.Layout)
                            {
                                scene.atlas.landmarkSelected = -1;
                                EndAtlasMode(false);
                                atlasCancelled = true;
                                goto endAtlas;
                            }
                        }

                        switch (atlas.state)
                        {
                            case AtlasState.landmarks:
                                #region landmarks
                                GUILayout.BeginHorizontal();
                                {
                                    if (!SceneView.lastActiveSceneView.in2DMode)
                                    {
                                        GUILayout.FlexibleSpace();
                                        if (GUILayout.Button(Core.tAtlasRotLeft, GUIStyle.none, GUILayout.Width(24), GUILayout.Height(24)))
                                        {
                                            atlas.rotCurrent++;
                                            if (atlas.rotCurrent >= atlas.rotAngles) atlas.rotCurrent = 0;
                                            AtlasLandmarksZoom();
                                        }
                                        GUILayout.Space(8);
                                        if (GUILayout.Button(Core.tAtlasRotRight, GUIStyle.none, GUILayout.Width(24), GUILayout.Height(24)))
                                        {
                                            atlas.rotCurrent--;
                                            if (atlas.rotCurrent < 0) atlas.rotCurrent = atlas.rotAngles - 1;
                                            AtlasLandmarksZoom();
                                        }

                                        GUILayout.FlexibleSpace();
                                    }
                                }
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();

                                scene.scrollPosAtlasLandmarkZoom = GUILayout.BeginScrollView(scene.scrollPosAtlasLandmarkZoom, GUI.skin.horizontalScrollbar, GUIStyle.none, GUILayout.Height(16));
                                {
                                    GUILayout.BeginHorizontal(GUILayout.Width(500));
                                    GUILayout.Label(" ");
                                    GUILayout.EndHorizontal();

                                }
                                GUILayout.EndScrollView();
                                int prev = atlas.zoomCurrent;
                                atlas.zoomCurrent = (int)(scene.scrollPosAtlasLandmarkZoom.x / 500 * 50);
                                if (atlas.zoomCurrent < 1) atlas.zoomCurrent = 1;

                                if (prev != atlas.zoomCurrent)
                                {
                                    AtlasLandmarksZoom();
                                }
                                GUILayout.EndHorizontal();

                                break;
                            #endregion
                            case AtlasState.tasks:
                                #region tasks

                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.BeginHorizontal();

                                    scene.scrollPosAtlasTaskZoom = GUILayout.BeginScrollView(scene.scrollPosAtlasTaskZoom, GUI.skin.horizontalScrollbar, GUIStyle.none, GUILayout.Height(16));
                                    {
                                        GUILayout.BeginHorizontal(GUILayout.Width(500));
                                        GUILayout.Label(" ");

                                        GUILayout.EndHorizontal();

                                    }
                                    GUILayout.EndScrollView();
                                    prev = atlas.taskZoom;
                                    atlas.taskZoom = (int)(scene.scrollPosAtlasTaskZoom.x / 500 * atlas.taskZoomLevels);
                                    if (atlas.taskZoom < 1) atlas.taskZoom = 0;

                                    if (prev != atlas.taskZoom)
                                    {
                                        AtlasTasksZoom(i);
                                    }
                                    GUILayout.EndHorizontal();
                                }
                                GUILayout.EndHorizontal();

                                GUILayout.Space(8);
                                GUIStyle s = StyleCheck("button");
                                s.fontSize = 14;
                                s.alignment = TextAnchor.MiddleCenter;
                                s.margin = new RectOffset(0, 0, 0, 0);
                                Color prevColor = GUI.color;
                                GUI.color = Color.white;
                                if (GUILayout.Button("Exit Task View", s, GUILayout.Width(128), GUILayout.Height(18)))
                                {
                                    atlas.landmarkSelected = -1;
                                    AtlasSpawnLandmarkPoints();
                                    AtlasLandmarksZoom();
                                }
                                GUI.color = prevColor;
                                GUILayout.Space(4);
                                s = StyleCheck("label");
                                s.fontSize = 10;
                                s.fontStyle = FontStyle.Bold;
                                s.alignment = TextAnchor.MiddleCenter;
                                s.padding = s.margin = new RectOffset(0, 0, 0, 0);

                                scene.scrollPosAtlasTasks = GUILayout.BeginScrollView(scene.scrollPosAtlasTasks, GUILayout.Width(126), GUILayout.Height(200));
                                {
                                    GUILayout.BeginVertical();
                                    {
                                        if (scene.atlas.landmarkSelected < 0)
                                        {
                                            goto end;
                                        }
                                        var localData = scene.landmarks[scene.atlas.landmarkSelected];
                                        string text = localData.title;

                                        if (text.Length > 18) text = text.Substring(0, 15) + "...";

                                        s = StyleCheck();
                                        s.fontSize = 10;
                                        s.fontStyle = FontStyle.Bold;
                                        s.alignment = TextAnchor.MiddleCenter;
                                        s.normal.background = Core.tTransparent;
                                        if (EditorGUIUtility.isProSkin) sBackgroundPanels.normal.background = Core.tGreenDark; else sBackgroundPanels.normal.background = Core.tGreenBright;

                                        HorizontalLine(Color.white);
                                        GUILayout.BeginVertical(sBackgroundPanels);
                                        GUILayout.BeginHorizontal(sBackgroundPanels);
                                        GUILayout.FlexibleSpace();
                                        GUILayout.Label(text, s, GUILayout.Height(24), GUILayout.Width(108));
                                        GUILayout.FlexibleSpace();
                                        GUILayout.EndHorizontal();
                                        GUILayout.EndVertical();
                                        HorizontalLine(Color.white);
                                        GUILayout.BeginHorizontal();
                                        {
                                            s.fontSize = 10;
                                            s.fontStyle = FontStyle.Bold;
                                            s.normal.textColor = Color.white;
                                            s.alignment = TextAnchor.MiddleLeft;
                                            s.normal.background = Core.tTransparent;
                                            GUILayout.FlexibleSpace();
                                            GUILayout.Label("Open Tasks:  " + localData.tasksOpen.ToString(), s);
                                            GUILayout.Space(4);
                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();

                                            GUILayout.FlexibleSpace();
                                            GUILayout.Label("Urgent:  " + localData.tasksUrgent.ToString(), s);
                                            GUILayout.Space(4);
                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();

                                            GUILayout.FlexibleSpace();
                                            GUILayout.Label("Past Due:  " + localData.tasksOverdue.ToString(), s);
                                            GUILayout.Space(4);
                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();

                                            if (localData.tasks.Count <= 1) GUILayout.Space(14); else GUILayout.Space(8);
                                            GUILayout.Box(Core.tTeal, GUILayout.Width(98), GUILayout.Height(8));
                                            EditorGUI.ProgressBar(GUILayoutUtility.GetLastRect(), localData.progress, "");
                                            GUILayout.FlexibleSpace();
                                        }
                                        GUILayout.EndHorizontal();
                                        GUILayout.Space(4);

                                        s = StyleCheck("label");
                                        s.fontSize = 10;
                                        s.fontStyle = FontStyle.Bold;
                                        s.alignment = TextAnchor.MiddleCenter;
                                        if (EditorGUIUtility.isProSkin) sBackgroundPanels.normal.background = s.normal.background = Core.tGreenDark; else sBackgroundPanels.normal.background = s.normal.background = Core.tGreenBright;
                                        HorizontalLine(Color.white);
                                        GUILayout.BeginVertical(sBackgroundPanels, GUILayout.Height(24));
                                        GUILayout.FlexibleSpace();
                                        GUILayout.BeginHorizontal(sBackgroundPanels);
                                        GUILayout.FlexibleSpace();
                                        s.normal.background = Core.tTransparent;
                                        GUILayout.Box(Core.tTaskSticky, GUIStyle.none, GUILayout.Width(20), GUILayout.Height(20));
                                        GUILayout.Label("Stickies", s);
                                        GUILayout.Box(Core.tTaskSticky, GUIStyle.none, GUILayout.Width(20), GUILayout.Height(20));
                                        GUILayout.FlexibleSpace();
                                        GUILayout.EndHorizontal();
                                        GUILayout.FlexibleSpace();
                                        GUILayout.EndVertical();
                                        HorizontalLine(Color.white);

                                        s = StyleCheck("toolbar");
                                        s.fontSize = 8;
                                        s.alignment = TextAnchor.MiddleCenter;
                                        List<int> nonSticky = new List<int>();
                                        int sticky = 0;
                                        for (int t = 0; t < scene.landmarks[scene.atlas.landmarkSelected].tasks.Count; t++)
                                        {
                                            var localDataTasks = scene.landmarks[scene.atlas.landmarkSelected].tasks[t];
                                            text = localDataTasks.name;
                                            if (text.Length > 18) text = text.Substring(0, 15) + "...";

                                            if (localDataTasks.isSticky)
                                            {
                                                GUILayout.BeginHorizontal();
                                                {
                                                    GUILayout.FlexibleSpace();
                                                    if (GUILayout.Button(new GUIContent(text), s, GUILayout.Width(98), GUILayout.Height(20)))
                                                    {
                                                        scene.atlas.taskFocus = sticky;
                                                        AtlasSpawnTaskPoints(atlas.landmarkSelected);
                                                        AtlasTasksZoom(atlas.landmarkSelected);
                                                    }
                                                    sticky++;
                                                    GUILayout.FlexibleSpace();
                                                    GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                                    if (localData.tasks.Count <= 1) GUILayout.Space(14); else GUILayout.Space(8);
                                                    GUILayout.Box(Core.tTeal, GUILayout.Width(98), GUILayout.Height(8));
                                                    EditorGUI.ProgressBar(GUILayoutUtility.GetLastRect(), (float)localDataTasks.progress / (float)100, "");
                                                    GUILayout.FlexibleSpace();
                                                }
                                                GUILayout.EndHorizontal();
                                                GUILayout.Space(4);
                                            }
                                            else
                                            {
                                                nonSticky.Add(t);
                                            }
                                        }


                                        if (sticky == 0)
                                        {
                                            s = StyleCheck("label");
                                            s.fontSize = 10;
                                            s.fontStyle = FontStyle.BoldAndItalic;
                                            s.normal.textColor = Color.yellow;
                                            s.normal.background = Core.tTransparent;
                                            s.alignment = TextAnchor.MiddleCenter;
                                            GUILayout.BeginHorizontal();
                                            GUILayout.FlexibleSpace();
                                            GUILayout.Box(Core.tIconX, GUIStyle.none, GUILayout.Width(20), GUILayout.Height(20));
                                            GUILayout.Label("No Stickies!", s);
                                            GUILayout.FlexibleSpace();
                                            GUILayout.EndHorizontal();
                                        }

                                        if (nonSticky.Count > 0)
                                        {
                                            s = StyleCheck("label");
                                            s.fontSize = 10;
                                            s.fontStyle = FontStyle.Bold;
                                            s.alignment = TextAnchor.MiddleCenter;
                                            s.normal.background = Core.tTransparent;
                                            if (EditorGUIUtility.isProSkin) sBackgroundPanels.normal.background = Core.tGreenDark; else sBackgroundPanels.normal.background = Core.tGreenBright;
                                            HorizontalLine(Color.white);
                                            GUILayout.BeginVertical(sBackgroundPanels, GUILayout.Height(24));
                                            GUILayout.FlexibleSpace();
                                            GUILayout.BeginHorizontal(sBackgroundPanels);
                                            GUILayout.FlexibleSpace();
                                            GUILayout.Box(Core.tIconTask, GUIStyle.none, GUILayout.Width(20), GUILayout.Height(20));
                                            GUILayout.Label("Virtual", s);
                                            GUILayout.Box(Core.tIconTask, GUIStyle.none, GUILayout.Width(20), GUILayout.Height(20));
                                            GUILayout.FlexibleSpace();
                                            GUILayout.EndHorizontal();
                                            GUILayout.FlexibleSpace();
                                            GUILayout.EndVertical();
                                            HorizontalLine(Color.white);

                                            s = StyleCheck("toolbar");
                                            s.fontSize = 8;
                                            s.alignment = TextAnchor.MiddleCenter;
                                            for (int t = 0; t < nonSticky.Count; t++)
                                            {
                                                var localDataTasks = scene.landmarks[scene.atlas.landmarkSelected].tasks[nonSticky[t]];
                                                text = localDataTasks.name;
                                                if (text.Length > 20) text = text.Substring(0, 15) + "...";
                                                GUILayout.BeginHorizontal();
                                                {
                                                    GUILayout.FlexibleSpace();
                                                    if (GUILayout.Button(text, s, GUILayout.Width(98), GUILayout.Height(16)))
                                                    {

                                                    }
                                                    GUILayout.FlexibleSpace();
                                                    GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                                    GUILayout.Box(Core.tTeal, GUILayout.Width(98), GUILayout.Height(8));
                                                    EditorGUI.ProgressBar(GUILayoutUtility.GetLastRect(), (float)localDataTasks.progress / (float)100, "");
                                                    GUILayout.FlexibleSpace();
                                                }
                                                GUILayout.EndHorizontal();
                                                GUILayout.Space(4);
                                            }
                                        }
                                    end:;
                                    }
                                    GUILayout.EndVertical();
                                    GUILayout.FlexibleSpace();
                                }
                                GUILayout.EndScrollView();
                                s.stretchWidth = true;
                                break;
                                #endregion
                        }

                    }
                endAtlas:;
                }
                GUILayout.EndArea();
                if (atlas.enabled && atlas.state == AtlasState.tasks)
                {
                    GUI.DrawTexture(new Rect(screenWidth - 128, 128 + 32, 128, 32), Core.tGradClear2Grey, ScaleMode.StretchToFill);
                    GUI.DrawTexture(new Rect(screenWidth - 128, 128 + 82 + 222, 128, 32), Core.tGradGrey2Clear, ScaleMode.StretchToFill);

                }
                #endregion

                #region AtlasButton
                //SceneView sv = Core.GetSceneView();
                //yy = 128;
                //if (sv.in2DMode) yy = 8;
                GUILayout.BeginArea(new Rect(screenWidth - 128, yy, 128, (128 * 4) + 8));
                {
                    if (scene.atlas == null) RefreshSettings();
                    GUILayout.Space(8);

                    if (atlas.enabled)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button(Core.tAtlasEnable, GUIStyle.none, GUILayout.Width(64), GUILayout.Height(64)))
                            {
                                //Debug.Log(scene);
                                //Debug.Log(scene.atlas);
                                //Debug.Log(scene.atlas.landmarkSelected);
                                scene.atlas.landmarkSelected = -1;
                                EndAtlasMode();
                            }
                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(32);
                            if (GUILayout.Button(Core.tAtlasDisable, GUIStyle.none, GUILayout.Width(64), GUILayout.Height(64)))
                            {
                                atlas.enabled = true;
                                scene.UpdateAllProgress(scene.stickyFont);
                                atlas.prevPosition = SceneView.currentDrawingSceneView.camera.transform.position;
                                atlas.prevRotation = SceneView.currentDrawingSceneView.camera.transform.rotation;
                                atlas.prevSize = SceneView.currentDrawingSceneView.size;
                                StartAtlasMode();
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    Rect pos = GUILayoutUtility.GetLastRect();
                    GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();

                    if (GUI.Button(pos, Core.tAtlasSettings, GUIStyle.none))
                    {
                        atlas.settings = !atlas.settings;
                    }

                }
                GUILayout.EndArea();
                #endregion

                #region Settings

                GUILayout.BeginArea(new Rect(screenWidth - 128 - 64, yy, 64, 128));
                {
                    if (atlas.settings)
                    {

                        GUILayout.BeginHorizontal();
                        {
                            s = new GUIStyle("toggle");
                            s.fontSize = 10;
                            GUILayout.Space(8);
                            scene.showLandmarkLabels = GUILayout.Toggle(scene.showLandmarkLabels, "Labels", s, GUILayout.Height(18));
                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                            GUILayout.Space(8);
                            scene.showTaskGizmos = GUILayout.Toggle(scene.showTaskGizmos, "Gizmos", s, GUILayout.Height(18));
                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                            GUILayout.Space(8);
                            scene.showStickies = GUILayout.Toggle(scene.showStickies, "Sticky", s, GUILayout.Height(18));
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndArea();
                #endregion

                if (atlas.enabled)
                {
                    float dist = Vector3.Distance(Core.GetSVCameraPosition(), landmarks.position);
                    Handles.color = landmarks.GizmoColor;

                    GUIStyle localStyle = StyleCheck();
                    localStyle.alignment = TextAnchor.MiddleCenter;
                    localStyle.fontSize = 12;
                    localStyle.fontStyle = FontStyle.Bold;
                    localStyle.normal.textColor = landmarks.GizmoColor;

                    float
                    x = landmarks.position.x,
                    y = landmarks.position.y,
                    z = landmarks.position.z;

                    Bounds tb = landmarks.taskBounds;

                    if (i == atlas.landmarkSelected)
                    {

                        if (tb == null)
                        {
                        }
                        else
                        {
                            Handles.DrawWireCube(tb.center, tb.size);
                            x = tb.center.x;
                            y = tb.size.y;
                            z = tb.center.z;
                        }
                    }
                    #region AtlasHandles
                    {
                        #region LandmarkView
                        Vector3 ws;
                        if (sceneview.in2DMode)
                        {
                            ws = HandleUtility.WorldToGUIPointWithDepth(new Vector3(landmarks.position.x, landmarks.position.y, -0.1f));
                        }
                        else
                        {
                            ws = HandleUtility.WorldToGUIPointWithDepth(landmarks.position);
                        }
                        atlas.landmarkFocusCount = scene.landmarks.Count;
                        if (atlas.landmarkFocus > atlas.landmarkFocusCount) atlas.landmarkFocus = atlas.landmarkFocusCount;

                        bool showLandmark = false;
                        switch (landmarks.viewState)
                        {
                            case Landmark.ViewState.is2D: if (sceneview.in2DMode) showLandmark = true; break;
                            case Landmark.ViewState.is3D: if (!sceneview.in2DMode) showLandmark = true; break;
                            case Landmark.ViewState.isOrthographic: if (!sceneview.in2DMode && sceneview.orthographic) showLandmark = true; break;
                        }

                        if (ws.z > 0 && showLandmark)
                            if (atlas.landmarkSelected == -1)
                            {
                                Color col = landmarks.GizmoColor;
                                if (i == atlas.landmarkFocus)
                                {
                                    localStyle.normal.background = landmarks.tGizmoColor;
                                    localStyle.normal.textColor = WhiteOrBlack(col);
                                }

                                atlas.state = AtlasState.landmarks;

                                EditorGUI.ProgressBar(new Rect(ws.x - 16, ws.y - 44, 32, 8), (float)landmarks.progress, "");


                                if (showLandmark)
                                    if (GUI.Button(new Rect(ws.x - 16, ws.y - 32, 32, 32), new GUIContent(Core.tIconPin), GUIStyle.none))
                                    {
                                        if (i == atlas.landmarkFocus)
                                        {
                                            if (atlas.landmarkSelected == i)
                                            {
                                                atlas.landmarkSelected = -1;
                                            }
                                            if (atlas.landmarkSelected == -1)
                                            {
                                                int o = 0;
                                                for (int t = 0; t < landmarks.tasks.Count; t++)
                                                    if (landmarks.tasks[t].isSticky) o++;
                                                if (o == 0) tb = new Bounds(landmarks.position, Vector3.one);
                                                atlas.landmarkSelected = i;
                                                atlas.taskFocus = 0;
                                                AtlasSpawnTaskPoints(atlas.landmarkSelected);
                                                AtlasTasksZoom(atlas.landmarkSelected, 2);
                                            }
                                        }
                                        else
                                        {
                                            localStyle.normal.textColor = Color.black;
                                            atlas.landmarkFocus = i;
                                            AtlasSpawnLandmarkPoints();
                                            AtlasLandmarksZoom();
                                        }
                                    }
                                string text = landmarks.title;
                                var rs = localStyle.CalcSize(new GUIContent(text));
                                rs = new Vector2(rs.x + 8, rs.y + 4);
                                GUI.Label(new Rect(ws.x - (rs.x / 2), ws.y + 4, rs.x, rs.y), text, localStyle);

                                if (landmarks.tasksHasAlert)
                                {
                                    DrawAlert(ws.x - 8 - 16, ws.y - 8 - 32, 24, 24, Core.tIconExclaim);
                                }

                            }
                        #endregion

                        #region TaskView
                        if (i == atlas.landmarkSelected)
                        {
                            atlas.state = AtlasState.tasks;
                            var localDataTasks = landmarks.tasks;
                            string text;

                            #region LandmarkHeader
                            style = StyleCheck();
                            style.fontSize = 20;
                            style.fontStyle = FontStyle.Bold;
                            style.alignment = TextAnchor.MiddleCenter;

                            if (landmarks.tGizmoColor50 == null) landmarks.SetColor(landmarks.GizmoColor);
                            style.normal.background = landmarks.tGizmoColor50;
                            style.normal.textColor = WhiteOrBlack(landmarks.GizmoColor);

                            text = landmarks.title;
                            if (text.Length > 23) text = text.Substring(0, 20) + "..";
                            GUI.Label(new Rect(screenWidth / 2 - 128, 16, 256, 64), text, style);
                            if (GUI.Button(new Rect(screenWidth / 2 - 128 - 64, 16, 64, 64), landmarks.tScreenshot, GUIStyle.none))
                            {
                                Core.LandmarkCamera.transform.position = scene.landmarks[i].position;
                                Core.LandmarkCamera.transform.rotation = scene.landmarks[i].rotation;

                                Core.EnableLandmarkCamera();
                                SceneView.lastActiveSceneView.AlignViewToObject(Core.LandmarkCamera.transform);

                                if (scene.landmarks[i].viewState == Landmark.ViewState.is2D)
                                {
                                    SceneView.lastActiveSceneView.in2DMode = true;
                                }
                                else
                                {
                                    SceneView.lastActiveSceneView.in2DMode = false;
                                    if (scene.landmarks[i].viewState == Landmark.ViewState.isOrthographic)
                                    {
                                        SceneView.lastActiveSceneView.orthographic = true;
                                    }
                                    else
                                    {
                                        SceneView.lastActiveSceneView.orthographic = false;
                                    }
                                }
                                SceneView.lastActiveSceneView.AlignViewToObject(Core.LandmarkCamera.transform);
                                SceneView.lastActiveSceneView.size = scene.landmarks[i].orthographicSize;
                                SceneView.lastActiveSceneView.Repaint();
                                Core.DisableLandmarkCamera();

                                scene.atlas.landmarkSelected = -1;
                                EndAtlasMode(false);
                                atlasCancelled = true;
                            }
                            #endregion

                            for (int t = 0; t < localDataTasks.Count; t++)
                            {
                                if (!localDataTasks[t].isSticky) continue;
                                localStyle.alignment = TextAnchor.MiddleCenter;
                                localStyle.fontSize = 10;

                                localStyle.normal.textColor = WhiteOrBlack(localDataTasks[t].color);
                                localStyle.normal.background = localDataTasks[t].tColor;

                                ws = HandleUtility.WorldToGUIPoint(localDataTasks[t].position);

                                if (t == atlas.taskFocus)
                                {
                                    if (GUI.Button(new Rect(ws.x - 24, ws.y + 28, 24, 24), Core.tAtlasEditPen, GUIStyle.none))
                                    {
                                        scene.landmarkDetailOpen = true;
                                        scene.selectedLandmark = i;
                                        scene.landmarkDetailState = TA.LandmarkDetailState.tasks;
                                        scene.zoomToTaskID = localDataTasks[t].createdDate;
                                        TaskAtlasEditorWindowNew.QueueRepaint();
                                    }

                                    if (GUI.Button(new Rect(ws.x + 4, ws.y + 28, 24, 24), Core.tAtlasGO, GUIStyle.none))
                                    {
                                        atlas.prevPosition = localDataTasks[t].position + (Vector3.forward * -10);
                                        scene.atlas.landmarkSelected = -1;
                                        bool is2d = false;
                                        if (landmarks.viewState == Landmark.ViewState.is2D) is2d = true;
                                        EndAtlasMode(localDataTasks[t].position + (Vector3.forward * -10), localDataTasks[t].position, is2d);
                                    }
                                }



                                EditorGUI.ProgressBar(new Rect(ws.x - 16, ws.y - 44, 32, 8), (float)localDataTasks[t].progress / (float)100, "");
                                #region PinClick
                                if (GUI.Button(new Rect(ws.x - 16, ws.y - 32, 32, 32), new GUIContent(Core.tTaskSticky), GUIStyle.none))
                                {
                                    for (int f = 0; f < atlas.focusPositions.Count; f++)
                                    {
                                        if (localDataTasks[t].position == atlas.focusPositions[f]) atlas.taskFocus = f;
                                    }

                                    AtlasSpawnTaskPoints(atlas.landmarkSelected);
                                    AtlasTasksZoom(atlas.landmarkSelected);
                                }
                                #endregion

                                text = localDataTasks[t].name;
                                if (text.Length > 20) text = text.Substring(0, 20) + "...";
                                var rs = localStyle.CalcSize(new GUIContent(text));
                                rs = new Vector2(rs.x + 8, rs.y + 4);
                                GUI.Label(new Rect(ws.x - (rs.x / 2), ws.y + 4, rs.x, rs.y), text, localStyle);

                                if (localDataTasks[t].stage != Task.Stage.complete)
                                {
                                    if (localDataTasks[t].priority == Task.Priority.urgent)
                                    {
                                        DrawAlert(ws.x - 8 - 16, ws.y - 8 - 32, 24, 24, Core.tIconExclaim);
                                    }
                                    else if (localDataTasks[t].alert && localDataTasks[t].dueDateDT <= DateTime.Now.Ticks)
                                    {
                                        DrawAlert(ws.x - 8 - 16, ws.y - 8 - 32, 24, 24, Core.tIconExclaim);
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
                #endregion

                #region DebugHandles
                if (modeDebug)
                {
                    Handles.color = Color.white;
                    Handles.DrawWireCube(boundsScene.center, boundsScene.size);

                    if (atlas.viewPointsLandmarks != null)
                        for (int t = 0; t < atlas.tiltAngles; t++)
                            for (int r = 0; r < atlas.rotAngles; r++)
                                for (int z = 0; z < atlas.zoomLevels; z++)
                                {
                                    float a = 1 - (Vector3.Distance(atlas.viewPointsLandmarks[t, r, z], Core.GetSVCameraPosition()));
                                    Handles.color = Color.black;
                                    if (z == atlas.zoomCurrent) Handles.color = new Color(1, 0, 0, 1);
                                    if (r == atlas.rotCurrent) Handles.color = new Color(0, 1, 0, 1);
                                    if (t == atlas.tiltCurrent) Handles.color = new Color(0, 0, 1, 1);
                                    Handles.DrawWireDisc(atlas.viewPointsLandmarks[t, r, z], Vector3.up, boundsScene.extents.z / 5);
                                    Handles.color = Color.white;
                                    Handles.DrawWireCube(boundsScene.center, boundsScene.size);
                                }
                }
                #endregion

            }

            if (scene.landmarks.Count == 0)
            {

                GUILayout.BeginArea(new Rect(screenWidth - 128, 128, 128, 128 * 4));
                {
                    if (scene.atlas == null) RefreshSettings();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(32);
                        if (GUILayout.Button(Core.tAtlasDisable, GUIStyle.none, GUILayout.Width(64), GUILayout.Height(64)))
                        {
                            Debug.Log("Atlas Mode requires at least one Landmark to be added, open Window > TaskAtlas > Landmark Editor to add one!");
                        }
                    }
                    GUILayout.EndHorizontal();

                }
                GUILayout.EndArea();
            }
            Handles.EndGUI();
        }



        static void LoadViewLandmarkDetail()
        {
        }

        static public void StartAtlasMode(bool findNearest = true)
        {
            atlas.state = AtlasState.landmarks;
            int minT = 9999, minR = 9999, minZ = 9999;
            float currentDist, minDist = 9999;
            var svc = Core.GetSVCameraPosition();

            atlas.landmarkFocusCount = scene.landmarks.Count;
            if (atlas.landmarkFocus < 0) atlas.landmarkFocus = 0;

            if (findNearest)
                for (int i = 0; i < atlas.landmarkFocusCount; i++)
                {
                    currentDist = Vector3.Distance(svc, scene.landmarks[i].wsPosition);
                    if (currentDist < minDist)
                    {
                        minDist = currentDist;
                        atlas.landmarkFocus = i;
                    }
                }

            AtlasSpawnLandmarkPoints();
            minDist = 9999;
            int t = 5;
            {
                for (int r = 0; r < atlas.rotAngles; r++)
                {
                    for (int z = 1; z < atlas.zoomLevels; z++)
                    {
                        if (SceneView.lastActiveSceneView.in2DMode)
                        {
                            currentDist = Vector2.Distance(svc, atlas.viewPointsLandmarks[0, 0, z]);
                        }
                        else
                        {
                            currentDist = Vector3.Distance(svc, atlas.viewPointsLandmarks[t, r, z]);
                        }
                        if (currentDist < minDist)
                        {
                            minDist = currentDist;
                            minT = t;
                            minR = r;
                            minZ = z;
                        }
                    }
                }
            }
            atlas.zoomCurrent = minZ;
            atlas.rotCurrent = minR;
            atlas.tiltCurrent = minT;
            AtlasLandmarksZoom();
        }

        static public void EndAtlasMode(Vector3 pos, Vector3 lookAt, bool is2d, bool moveCamera = true)
        {
            atlas.enabled = false;
            if (moveCamera)
            {
                Core.LandmarkCamera.transform.position = pos;
                if (is2d)
                {
                    Core.LandmarkCamera.transform.rotation = Quaternion.identity;
                    SceneView.lastActiveSceneView.AlignViewToObject(Core.LandmarkCamera.transform);
                    SceneView.lastActiveSceneView.size = atlas.prevSize;
                    SceneView.lastActiveSceneView.Repaint();
                }
                else
                {
                    Core.LandmarkCamera.transform.LookAt(lookAt);

                    SceneView.lastActiveSceneView.AlignViewToObject(Core.LandmarkCamera.transform);
                }
            }
        }

        static public void EndAtlasMode(bool returnToPrevPosition = true)
        {
            atlas.enabled = false;
            if (returnToPrevPosition)
            {
                Core.LandmarkCamera.transform.position = atlas.prevPosition;
                Core.LandmarkCamera.transform.rotation = atlas.prevRotation;
                SceneView.lastActiveSceneView.AlignViewToObject(Core.LandmarkCamera.transform);
                SceneView.lastActiveSceneView.size = atlas.prevSize;
                SceneView.lastActiveSceneView.Repaint();
            }
        }

        static public void AtlasLandmarksZoom()
        {
            if (atlas.viewPointsLandmarks == null) AtlasSpawnLandmarkPoints();
            Vector3 pos;

            float size = 0;
            if (SceneView.lastActiveSceneView.in2DMode)
            {
                pos = atlas.viewPointsLandmarks[0, 0, atlas.zoomCurrent];
                size = pos.z;
                pos = new Vector3(pos.x, pos.y, -1000f);
            }
            else
            {
                pos = atlas.viewPointsLandmarks[atlas.tiltCurrent, atlas.rotCurrent, atlas.zoomCurrent];
            }

            Core.LandmarkCamera.SetActive(true);
            Core.LandmarkCamera.transform.position = pos;
            Core.LandmarkCamera.transform.rotation = scene.landmarks[scene.selectedLandmark].rotation; ;

            Core.LandmarkCamera.transform.LookAt(scene.landmarks[atlas.landmarkFocus].position);

            SceneView.lastActiveSceneView.AlignViewToObject(Core.LandmarkCamera.transform);

            if (SceneView.lastActiveSceneView.in2DMode)
            {
                SceneView.lastActiveSceneView.size = size;
            }

            SceneView.lastActiveSceneView.Repaint();
            atlas.isZooming = true;

            Core.LandmarkCamera.SetActive(false);
            atlas.legitPosition = pos;
        }


        static Vector3 prevViewPos;
        static public void AtlasSpawnLandmarkPoints(bool farZoom = true)
        {
            if (atlas.landmarkFocusCount > scene.landmarks.Count()) atlas.landmarkFocusCount = scene.landmarks.Count();
            if (atlas.landmarkFocus >= atlas.landmarkFocusCount) atlas.landmarkFocus = atlas.landmarkFocusCount - 1;
            Transform f = SceneView.lastActiveSceneView.camera.transform;
            f.position = scene.landmarks[scene.selectedLandmark].position;
            f.rotation = scene.landmarks[scene.selectedLandmark].rotation;

            if (prevViewPos == f.transform.position) return;

            if (SceneView.lastActiveSceneView.in2DMode)
            {
                atlas.viewPointsLandmarks = new Vector3[1, 1, atlas.zoomLevels];
                atlas.viewPointSizesLandmarks = new float[atlas.zoomLevels];
                for (int z = 0; z < atlas.zoomLevels; z++)
                {
                    var pos = scene.landmarks[atlas.landmarkFocus].position;

                    atlas.viewPointsLandmarks[0, 0, z] = new Vector3(pos.x, pos.y, 3 * z);
                    atlas.viewPointSizesLandmarks[z] = Mathf.Abs(f.position.z) / 2;
                }
                atlas.tiltCurrent = atlas.rotCurrent = 0;
                atlas.zoomCurrent = 7;

            }
            else
            {
                atlas.viewPointsLandmarks = new Vector3[atlas.tiltAngles, atlas.rotAngles, atlas.zoomLevels];
                int i = 0;
                for (int t = 0; t < atlas.tiltAngles; t++)
                {
                    for (int r = 0; r < atlas.rotAngles; r++)
                    {
                        for (int z = 0; z < atlas.zoomLevels; z++, i++)
                        {
                            var pos = scene.landmarks[atlas.landmarkFocus].position;
                            f.position = (pos - ((20/*boundsScene.extents.x*/ * (atlas.taskZoom + 1)) / 2) * z * Vector3.forward);
                            f.RotateAround(pos, Vector3.left, (float)t / atlas.tiltAngles * 360);
                            f.RotateAround(pos, Vector3.up, (float)r / atlas.rotAngles * 360);
                            atlas.viewPointsLandmarks[t, r, z] = f.position;
                        }
                    }
                }
                prevViewPos = f.transform.position;
            }
        }

        static public void AtlasTasksZoom(int landmark, float size = 3)
        {
            Vector3 pos = atlas.viewPointsTasks[atlas.taskFocus];

            var sp = HandleUtility.WorldToGUIPoint(atlas.focusPositions[atlas.taskFocus]);

            GUI.Label(new Rect(sp.x, sp.y, 24, 24), "X");

            if (SceneView.lastActiveSceneView.in2DMode)
            {
                Core.LandmarkCamera.transform.position = pos;

                Core.LandmarkCamera.transform.rotation = Quaternion.identity;

                SceneView.lastActiveSceneView.AlignViewToObject(Core.LandmarkCamera.transform);
                SceneView.lastActiveSceneView.size = (atlas.taskZoom + .1f) * 3;

                SceneView.lastActiveSceneView.Repaint();
                atlas.isZooming = true;
                sp = HandleUtility.WorldToGUIPoint(atlas.focusPositions[atlas.taskFocus]);
                atlas.legitPosition = pos;
            }
            else
            {
                Core.LandmarkCamera.transform.position = pos;

                Core.LandmarkCamera.transform.LookAt(atlas.focusPositions[atlas.taskFocus]);

                SceneView.lastActiveSceneView.AlignViewToObject(Core.LandmarkCamera.transform);
                atlas.isZooming = true;
                sp = HandleUtility.WorldToGUIPoint(atlas.focusPositions[atlas.taskFocus]);
                atlas.legitPosition = pos;
            }
        }

        static public void AtlasSpawnTaskPoints(int landmark)
        {
            atlas.focusPositions = new List<Vector3>();
            atlas.viewPointSizesTasks = new float[atlas.taskZoomLevels];
            atlas.taskFocusCount = 0;
            var localData = scene.landmarks[landmark].tasks;

            if (SceneView.lastActiveSceneView.in2DMode)
            {

                atlas.viewPointSizesTasks = new float[atlas.taskZoomLevels];
                for (int t = 0; t < localData.Count; t++)
                {
                    if (!localData[t].isSticky) continue;
                    atlas.taskFocusCount++;
                    atlas.focusPositions.Add(localData[t].position);
                }
                if (atlas.focusPositions.Count == 0)
                {
                    atlas.taskFocusCount++;
                    atlas.focusPositions.Add(scene.landmarks[landmark].position);
                }
                atlas.viewPointsTasks = new List<Vector3>();
                for (int i = 0; i < atlas.focusPositions.Count; i++)
                {
                    Vector3 p = atlas.focusPositions[i];
                    atlas.viewPointsTasks.Add(new Vector3(p.x, p.y, i * 3));
                    atlas.viewPointSizesTasks[i] = Mathf.Abs(p.z) / 2;
                }
            }
            else
            {
                for (int t = 0; t < localData.Count; t++)
                {
                    if (!localData[t].isSticky) continue;
                    atlas.taskFocusCount++;
                    atlas.focusPositions.Add(localData[t].position);
                }
                if (atlas.focusPositions.Count == 0)
                {
                    atlas.taskFocusCount++;
                    atlas.focusPositions.Add(scene.landmarks[landmark].position);
                }
                atlas.viewPointsTasks = new List<Vector3>();
                for (int i = 0; i < atlas.focusPositions.Count; i++)
                {
                    var p = atlas.focusPositions[i];
                    atlas.viewPointsTasks.Add(new Vector3(p.x, p.y + ((20 * (atlas.taskZoom + 1)) / 2), p.z));
                }
            }
        }

        static public void DrawAlert(float x, float y, float width, float height, Texture2D tex)
        {
            int xx = Mathf.FloorToInt(x + ((width - (width * lerp)) / 2));
            int yy = Mathf.FloorToInt(y + ((height - (height * lerp)) / 2));
            GUI.DrawTexture(new Rect(xx, yy, width * lerp, height * lerp), Core.tIconExclaim);
        }

        static private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        static GUIStyle StyleCheck()
        {
            GUIStyle style = new GUIStyle();
            if (EditorGUIUtility.isProSkin)
            {
                style.normal.textColor = Color.white;
            }
            return style;
        }

        static GUIStyle StyleCheck(string type)
        {
            GUIStyle style = new GUIStyle(type);
            if (EditorGUIUtility.isProSkin)
            {
                style.normal.textColor = Color.white;
                style.normal.background = Core.tBackgroundPanels;
            }
            return style;
        }

        static Color WhiteOrBlack(Color col)
        {
            float H, S, V;
            Color.RGBToHSV(col, out H, out S, out V);
            col = Color.white;
            if (V > .5f)
            {
                if (V > .8f) col = Color.black;
                if (S < .5f) col = Color.black;
            }
            return (col);
        }

        static void ProgressBar(Rect rect, float progress, Texture2D background, Texture2D foreground)
        {
            GUI.DrawTexture(rect, background);
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width * progress, rect.height), foreground);
        }

        static void DrawBox(Rect position, Color color)
        {
            Color oldColor = GUI.color;

            GUI.color = color;
            GUI.Box(position, "");

            GUI.color = oldColor;
        }
    }
}