using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TaskAtlasNamespace
{
    [InitializeOnLoad]

    public class TaskAtlasEditorWindow : EditorWindow
    {
//        public static EditorWindow window;

//        static private ReorderableList ROLLandmarks;
//        static List<string> landmarkNames;

//        public static TA scene;

//        static List<string> tags;
//        static List<string> tagList, tagListCopy;
//        static int tagSelected;

//        static string newTag;

//        static GUIStyle sBackgroundPanels, sBackground, sTaskHeader, s;

//        static float thumbnailSize = 128 + 32;
//        static string text;
//        static int panelHeight = 128;
//        static float screenWidth, screenHeight, screenWidthMargin;

//        static float lerp = 1.0f, lerpMin = 1f, lerpMax = 1.5f, lerpTarget = 1.2f;

//        static float localWidth;

//        static int queueRepaint = 0;

//        [MenuItem("Window/ShrinkRay Entertainment/TaskAtlas/Landmark Editor")]
//        private static void OpenLandmarksWindow()
//        {
//            window = GetWindow<TaskAtlasEditorWindow>();

//            window.minSize = new Vector2(400, 600);
//            // window.maxSize = new Vector2(640, 4000);
//            window.Show();
//            window.titleContent.text = "Task Atlas";

//            RefreshSettings();
//        }

//        [MenuItem("Window/ShrinkRay Entertainment/Discord Server")]
//        private static void Discord()
//        {
//            Application.OpenURL("https://discord.gg/W8MKrRH");
//        }

//        [MenuItem("Window/ShrinkRay Entertainment/Email Us")]
//        private static void email()
//        {
//            Application.OpenURL("mailto:shrinkrayentertainment@gmail.com?subject=Scene%20Pilot%20Question&body=Questions?%20Comments?%20Issues?");
//        }

//        [MenuItem("Window/ShrinkRay Entertainment/TaskAtlas/Rate TaskAtlas")]
//        private static void Rate()
//        {
//            Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/task-atlas-ultimate-task-manager-sticky-notes-bookmarking-refere-185959#reviews");
//        }

//        [MenuItem("Window/ShrinkRay Entertainment/Get all ShrinkRay Assets at a HUGE discount")]
//        private static void MoreAssets()
//        {
//            Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/better-editor-deluxe-192015?aid=1011lf9gY&pubref=taskatlas");
//        }

//        public static void QueueRepaint()
//        {
//            if (window == null) window = GetWindow<TaskAtlasEditorWindow>();
//            window.minSize = new Vector2(380, 490);
//            window.maxSize = new Vector2(600, 196);
//            window.titleContent.text = "Task Atlas";
//            queueRepaint = 1;
//        }

//        public void OnInspectorUpdate()
//        {
//            if (queueRepaint > 0 && queueRepaint < 10)
//            {
//                Repaint();
//            }
//        }

//        private void OnEnable()
//        {

//        }

//        static public void RefreshSettings()
//        {
//            RefreshTaglist();
//        }

//        static void RefreshTaglist()
//        {
//            if (Core.dataV2 == null) Core.Init();
//            if (tags == null) tags = new List<string>();
//            if (tagList == null) { tagList = new List<string>(); } else { tagList.Clear(); }
//            tagList.Add("Show All");
//            for (int i = 0; i < Core.dataV2.tags.Count; i++)
//            {
//                tagList.Add(Core.dataV2.tags[i].name);
//            }
//            tagList.Add("Has Any Tag");
//        }


//        public static bool isInit = false;
//        static public void Init()
//        {
//            if (Core.isBuild) return;

//            Core.RefreshIcons();
//            Core.RefreshHelperCams();

//            sBackgroundPanels = new GUIStyle();
//            sBackgroundPanels.normal.background = Core.tBackgroundMain;

//            scene = Core.dataV2.scene[Core.dataV2.sceneIndex];
//            RefreshSettings();
//            scene.UpdateAllProgress(scene.stickyFont);
//            scene.UpdateAllThumbnails();

//            //for (int i = 0; i < scene.history.Count; i++)
//            //{
//            //    var d = scene.history[i];
//            //    d.GetThumbnail();
//            //}
//            for (int i = 0; i < scene.landmarks.Count; i++)
//            {
//                var d = scene.landmarks[i];
//                d.GetThumbnail();
//            }
//            for (int x = 0; x < scene.landmarks.Count; x++)
//            {
//                for (int y = 0; y < scene.landmarks[x].tasks.Count; y++)
//                {
//                    var d = scene.landmarks[x].tasks[y];
//                    if (d.createdDateDT == 0)
//                    {
//                        if (d.createdDate != "")
//                        {
//                            d.createdDateDT = long.Parse(d.createdDate);
//                        }
//                        else
//                        {
//                            d.createdDateDT = DateTime.Now.Ticks;
//                        }
//                    }
//                    if (d.dueDateDT == 0)
//                    {
//                        if (d.dueDate != "")
//                        {
//                            d.dueDateDT = long.Parse(d.dueDate);
//                        }
//                        else
//                        {
//                            d.dueDateDT = DateTime.Now.Ticks;
//                            d.alert = false;
//                        }
//                    }
//                }
//            }
//            isInit = true;
//        }

//        void OnGUI()
//        {
//            if (!isInit)
//            {
//                Core.Init();
//                Init();
//            }

//            if (Core.isBuild || !Core.isInstalled) return;

//            lerp = Mathf.Lerp(lerp, lerpTarget, 0.05f);
//            if (lerp >= lerpTarget - 0.1f) lerpTarget = lerpMin;
//            if (lerp <= lerpTarget + 0.1f) lerpTarget = lerpMax;


//            if (window == null)
//            {
//                screenWidth = 0;
//                screenHeight = 0;
//                QueueRepaint();
//            }
//            else
//            {
//                screenWidth = window.position.width;
//                screenHeight = window.position.height;
//            }

//            if (screenWidth > 600)
//            {
//                screenWidthMargin = (screenWidth - 600) / 2;
//                screenWidth = 600;
//            }
//            else
//            {
//                screenWidthMargin = 0;
//            }

//            #region Landmarks

//            if (!scene.landmarkDetailOpen)
//            {
//                LoadViewLandmarks();
//            }
//            else
//            {
//                LoadViewLandmarkDetail();
//            }

//            #endregion
//            queueRepaint++;
//        }

//        static void LoadViewLandmarkSort()
//        {
//            #region LoadViewLandmarkSort
//            GUIStyle s = StyleCheck();
//            sBackgroundPanels.normal.background = Core.tBackgroundMain;
//            GUILayout.BeginArea(new Rect(0, 8, screenWidth, panelHeight));
//            {
//                GUILayout.BeginHorizontal();
//                {
//                    GUILayout.FlexibleSpace();
//                    #region bAddToLandmarks
//                    if (GUILayout.Button(Core.tAdd, GUILayout.Width(48), GUILayout.Height(48)))
//                    {
//                        AddCurrentViewToLandmarks();
//                    }
//                    #endregion
//                    GUILayout.FlexibleSpace();
//                }
//                GUILayout.EndHorizontal();
//                HorizontalLine(Color.black);

//                GUILayout.BeginHorizontal();
//                {
//                    GUILayout.FlexibleSpace();
//                    s.fontSize = 8;
//                    #region bSortDist
//                    if (GUILayout.Button("Sort Mode", GUILayout.Width(24), GUILayout.Height(24)))
//                    {
//                        scene.sortMode = !scene.sortMode;
//                    }
//                    #endregion
//                    #region bSortDist
//                    if (GUILayout.Button(Core.tSortDist, GUILayout.Width(24), GUILayout.Height(24)))
//                    {
//                        for (int i = 0; i < scene.landmarks.Count; i++)
//                        {
//                            scene.landmarks[i].currentDistance = Vector3.Distance(Core.GetSVCameraPosition(), scene.landmarks[i].position);
//                        }
//                        scene.landmarks = scene.landmarks.OrderByDescending(x => x.currentDistance).ToList();
//                    }
//                    #endregion
//                    #region bSortAZ
//                    if (GUILayout.Button(Core.tSortAZ, GUILayout.Width(24), GUILayout.Height(24)))
//                    {
//                        scene.landmarks = scene.landmarks.OrderBy(x => x.title).ToList();
//                    }
//                    #endregion
//                    #region bSortDate
//                    if (GUILayout.Button(Core.tSortDate, GUILayout.Width(24), GUILayout.Height(24)))
//                    {
//                        scene.landmarks = scene.landmarks.OrderByDescending(x => x.timeStamp).ToList();
//                    }
//                    #endregion
//                    GUILayout.FlexibleSpace();
//                }
//                GUILayout.EndHorizontal();
//                HorizontalLine(Color.black);
//            }
//            GUILayout.EndArea();

//            thumbnailSize = (screenWidth / 2);

//            Rect parentRect = new Rect(0, panelHeight, screenWidth, screenHeight - panelHeight - 24);

//            GUILayout.BeginArea(parentRect);
//            {
//                scene.scrollPosLandmarkSort = GUILayout.BeginScrollView(scene.scrollPosLandmarkSort, GUILayout.Width(screenWidth - 4));
//                GUILayout.BeginVertical();
//                {

//                }
//                GUILayout.EndVertical();
//                GUILayout.EndScrollView();
//            }
//            GUILayout.EndArea();
//            #endregion

//        }

//        static void LoadViewLandmarks()
//        {
//            #region LoadViewLandmarks
//            GUIStyle s = StyleCheck(), s2 = StyleCheck();
//            sBackgroundPanels.normal.background = Core.tBackgroundMain;
//            int offset = 0;
//            if (screenWidth < 400) offset = 24;


//            GUILayout.BeginArea(new Rect(screenWidthMargin, 8, screenWidth, panelHeight + offset));
//            {
//                if (DrawTaskHeader("Add Landmark Here", Core.tAdd, Core.tGreen, true, 130, 24, new GUIContent("New Landmark")))
//                {
//                    AddCurrentViewToLandmarks();
//                }
//                HorizontalLine(Color.black);

//                #region SortBy
//                GUILayout.BeginHorizontal();
//                {
//                    GUILayout.Label("Sort By:");

//                    s = StyleCheck();
//                    s.fontSize = 8;
//                    if (screenWidth > 470)
//                    {
//                        #region Dist
//                        if (GUILayout.Button(new GUIContent("    Distance", Core.tSortDist), GUILayout.Height(18), GUILayout.Width(96)))
//                        {
//                            for (int i = 0; i < scene.landmarks.Count; i++)
//                            {
//                                scene.landmarks[i].currentDistance = Vector3.Distance(Core.GetSVCameraPosition(), scene.landmarks[i].position);
//                            }
//                            scene.landmarks = scene.landmarks.OrderBy(x => x.currentDistance).ToList();
//                        }
//                        #endregion
//                        #region AZ
//                        if (GUILayout.Button(new GUIContent("    Alpha", Core.tSortAZ), GUILayout.Height(18), GUILayout.Width(96)))
//                        {
//                            scene.landmarks = scene.landmarks.OrderBy(x => x.title).ToList();
//                        }
//                        #endregion
//                        #region tDate
//                        if (GUILayout.Button(new GUIContent("    Created", Core.tSortDate), GUILayout.Height(18), GUILayout.Width(96)))
//                        {
//                            scene.landmarks = scene.landmarks.OrderByDescending(x => x.timeStamp).ToList();
//                        }
//                        #endregion
//                        #region Manual
//                        GUILayout.FlexibleSpace();
//                        s = new GUIStyle("button");

//                        scene.sortMode = GUILayout.Toggle(scene.sortMode, "Manual Sort", s, GUILayout.Height(18), GUILayout.Width(112));

//                        #endregion
//                        GUILayout.Space(16);
//                    }
//                    else
//                    {
//                        #region Dist
//                        if (GUILayout.Button(new GUIContent(Core.tSortDist), GUILayout.Height(18), GUILayout.Width(32)))
//                        {
//                            for (int i = 0; i < scene.landmarks.Count; i++)
//                            {
//                                scene.landmarks[i].currentDistance = Vector3.Distance(Core.GetSVCameraPosition(), scene.landmarks[i].position);
//                            }
//                            scene.landmarks = scene.landmarks.OrderBy(x => x.currentDistance).ToList();
//                        }
//                        #endregion
//                        #region AZ
//                        if (GUILayout.Button(new GUIContent(Core.tSortAZ), GUILayout.Height(18), GUILayout.Width(32)))
//                        {
//                            scene.landmarks = scene.landmarks.OrderBy(x => x.title).ToList();
//                        }
//                        #endregion
//                        #region tDate
//                        if (GUILayout.Button(new GUIContent(Core.tSortDate), GUILayout.Height(18), GUILayout.Width(32)))
//                        {
//                            scene.landmarks = scene.landmarks.OrderByDescending(x => x.timeStamp).ToList();
//                        }
//                        #endregion
//                        #region Manual
//                        GUILayout.FlexibleSpace();
//                        s = new GUIStyle("button");

//                        scene.sortMode = GUILayout.Toggle(scene.sortMode, "Manual Sort", s, GUILayout.Height(18), GUILayout.Width(112));

//                        #endregion
//                        GUILayout.Space(16);
//                    }
//                }
//                GUILayout.EndHorizontal();
//                #endregion

//                HorizontalLine(Color.black);
//                GUILayout.BeginHorizontal();
//                {
//                    DrawTagFilterMask();

//                    GUILayout.Label("Show:");
//                    s = new GUIStyle("toggle");
//                    if (screenWidth < 400) { s.fontSize = 8; }
//                    scene.showLandmarkLabels = GUILayout.Toggle(scene.showLandmarkLabels, "Labels", s, GUILayout.Height(18));
//                    scene.showTaskGizmos = GUILayout.Toggle(scene.showTaskGizmos, "Gizmos", s, GUILayout.Height(18));
//                    scene.showStickies = GUILayout.Toggle(scene.showStickies, "Sticky", s, GUILayout.Height(18));
//                    GUILayout.FlexibleSpace();
//                }
//                GUILayout.EndHorizontal();
//            }
//            GUILayout.EndArea();

//            thumbnailSize = (screenWidth / 2);

//            Rect parentRect = new Rect(screenWidthMargin, panelHeight + offset, screenWidth, screenHeight - panelHeight - 24 - offset);

//            GUILayout.BeginArea(parentRect);
//            {
//                scene.scrollPosLandmark = GUILayout.BeginScrollView(scene.scrollPosLandmark, GUILayout.Width(screenWidth - 4));
//                GUILayout.BeginVertical();
//                {
//                    GUILayout.Space(2);

//                    for (int i = 0, ii = 1; i < scene.landmarks.Count; i++, ii++)
//                    {
//                        var landmarks = scene.landmarks;
//                        if (landmarks[i].tGizmoColor == null) scene.landmarks[i].SetColor(landmarks[i].GizmoColor);

//                        if (!scene.sortMode)
//                        {
//                            #region CheckActiveTags
//                            bool noTag = true;
//                            if (tags != null)
//                                for (int t = 0; t < tags.Count; t++)
//                                {
//                                    for (int r = 0; r < scene.landmarks[i].tags.Count; r++)
//                                    {
//                                        if (scene.landmarks[i].tags[r].name == tags[t])
//                                        {
//                                            if (scene.landmarks[i].tags[r].active)
//                                            {
//                                                noTag = false;
//                                            }
//                                        }
//                                    }
//                                }
//                            if (tagSelected == 0 & noTag) goto skipcheck;
//                            if (noTag) continue;
//                            skipcheck:
//                            #endregion

//                            if (i < scene.landmarks.Count - 1) HorizontalLine(Color.black);
//                            if (!LoadViewLandmarkButtons(i))
//                            {
//                                continue;
//                            }

//                            GUILayout.Space(16);

//                            GUILayout.BeginHorizontal();
//                            {
//                                var prevFloat = landmarks[i].isFloating;
//                                landmarks[i].isFloating = GUILayout.Toggle(landmarks[i].isFloating, "Dynamically position this Landmark to the following GameObject:");
//                                if (prevFloat != landmarks[i].isFloating)
//                                {
//                                    RefreshLandmarkThumbnail(i);
//                                }
//                                var prevObj = landmarks[i].floatingPositionObject;
//                                landmarks[i].floatingPositionObject = (GameObject)EditorGUILayout.ObjectField(landmarks[i].floatingPositionObject, typeof(GameObject), true);
//                                if (prevObj != landmarks[i].floatingPositionObject && landmarks[i].isFloating)
//                                {
//                                    RefreshLandmarkThumbnail(i);
//                                }
//                            }
//                            GUILayout.EndHorizontal();

//                            GUILayout.Space(16);
//                            text = landmarks[i].description;
//                            s = new GUIStyle();
//                            s = GetTextAreaStyle(text, screenWidth - 28);
//                            GUILayout.Label(text, s, GUILayout.Width(screenWidth - 28), GUILayout.Height(s.fixedHeight));
//                            GUILayout.Space(16);

//                            s = StyleCheck();
//                            GUILayout.BeginHorizontal();
//                            {
//                                GUILayout.Label("Open Tasks:  " + landmarks[i].tasksOpen.ToString(), s);
//                                GUILayout.FlexibleSpace();
//                                if (landmarks[i].tasksUrgent > 0)
//                                {
//                                    s.normal.textColor = Color.red;
//                                }
//                                GUILayout.Label("Urgent:  " + landmarks[i].tasksUrgent.ToString(), s);
//                                GUILayout.FlexibleSpace();
//                                if (landmarks[i].tasksOverdue > 0)
//                                {
//                                    s.normal.textColor = Color.red;
//                                }
//                                GUILayout.Label("Past Due:  " + landmarks[i].tasksOverdue.ToString(), s);
//                                GUILayout.FlexibleSpace();
//                                if (landmarks[i].tasksOverdue > 0)
//                                {
//                                    s.normal.textColor = Color.red;
//                                }
//                                GUILayout.Space(9);
//                            }
//                            GUILayout.EndHorizontal();
//                            GUILayout.Space(16);

//                            int activeCount = 0;
//                            for (int t = 0; t < landmarks[i].tags.Count; t++)
//                                if (landmarks[i].tags[t].active) activeCount++;

//                            if (activeCount > 0)
//                            {
//                                GUILayout.BeginHorizontal();
//                                {
//                                    DrawTaskHeader("Tags", Core.tIconTag, Core.tTeal);

//                                    GUILayout.EndHorizontal();
//                                    s2 = StyleCheck();
//                                    s2.normal.background = Core.tTeal;
//                                    s2.margin = new RectOffset(9, 9, 0, 0);
//                                    GUILayout.BeginHorizontal(s2);

//                                    for (int t = 0; t < landmarks[i].tags.Count; t++)
//                                    {

//                                        if (landmarks[i].tags[t].active)
//                                        {
//                                            GUILayout.EndHorizontal();
//                                            GUILayout.BeginHorizontal(s2);
//                                            s = StyleCheck("button");
//                                            GUILayout.Label(landmarks[i].tags[t].name, s);
//                                        }
//                                    }
//                                }
//                                GUILayout.EndHorizontal();
//                                GUILayout.Space(24);
//                            }
//                        }
//                        else
//                        {
//                            s = StyleCheck();
//                            s.fontSize = 20;
//                            s.fontStyle = FontStyle.Bold;
//                            s.alignment = TextAnchor.MiddleLeft;

//                            s.normal.textColor = WhiteOrBlack(landmarks[i].GizmoColor);
//                            s.normal.background = landmarks[i].tGizmoColor;
//                            GUILayout.BeginVertical(GUILayout.Height(32));
//                            {
//                                GUILayout.BeginHorizontal(s);
//                                GUILayout.Space(8);
//                                text = landmarks[i].title;
//                                if (text.Length > 25) text = text.Substring(0, 15);

//                                text = (i + 1) + ". " + text;

//                                GUILayout.BeginVertical(); GUILayout.FlexibleSpace();
//                                GUILayout.Label(text, s);
//                                GUILayout.FlexibleSpace(); GUILayout.EndVertical();

//                                GUILayout.FlexibleSpace();

//                                GUILayout.BeginVertical(); GUILayout.FlexibleSpace();
//                                if (GUILayout.Button(Core.tArrowUp, GUILayout.Height(24), GUILayout.Width(24)))
//                                {
//                                    landmarks.Move(i, i - 1);
//                                }
//                                GUILayout.FlexibleSpace(); GUILayout.EndVertical();

//                                GUILayout.BeginVertical(); GUILayout.FlexibleSpace();
//                                if (GUILayout.Button(Core.tArrowDown, GUILayout.Height(24), GUILayout.Width(24)))
//                                {
//                                    landmarks.Move(i, i + 1);

//                                }
//                                GUILayout.FlexibleSpace(); GUILayout.EndVertical();

//                                GUILayout.Space(8);

//                                GUILayout.EndHorizontal();
//                                HorizontalLine(Color.black);
//                            }
//                            GUILayout.EndVertical();
//                        }
//                    }
//                }
//                GUILayout.EndVertical();
//                GUILayout.EndScrollView();
//            }
//            GUILayout.EndArea();
//            #endregion
//        }

//        static bool LoadViewLandmarkButtons(int i)
//        {
//            GUIStyle s = StyleCheck();
//            bool exists = true;

//            GUILayout.BeginHorizontal();
//            {
//                {
//                    s = StyleCheck();
//                    s.fontSize = 20;
//                    s.fontStyle = FontStyle.Bold;
//                    s.alignment = TextAnchor.MiddleCenter;

//                    s.normal.textColor = WhiteOrBlack(scene.landmarks[i].GizmoColor);
//                    s.normal.background = scene.landmarks[i].tGizmoColor;
//                    GUILayout.BeginHorizontal(s);
//                    GUILayout.FlexibleSpace();

//                    text = scene.landmarks[i].title;
//                    if (text.Length > 25) text = text.Substring(0, 15);

//                    GUILayout.Label(text, s);
//                    GUILayout.FlexibleSpace();

//                    GUILayout.EndHorizontal();
//                    GUILayout.EndHorizontal();
//                    GUILayout.BeginHorizontal();
//                }
//                GUILayout.BeginVertical();
//                {
//                    #region bLandmarkThumbnail            
//                    // if (GUILayout.Button(new GUIContent(Core.tRefresh, "Refresh Image"), GUILayout.Height(20)))
//                    // {
//                    //     RefreshLandmarkThumbnail(i);
//                    // }
//                    if (GUILayout.Button(scene.landmarks[i].tScreenshot, GUIStyle.none, GUILayout.Width(thumbnailSize), GUILayout.Height(thumbnailSize)))
//                    {
//                        scene.selectedLandmark = i;
//                        ZoomToLocation(i);
//                    }
//                    Rect pos = GUILayoutUtility.GetLastRect();
//                    // if (GUI.Button(new Rect(pos.x + 4, pos.y + 4, 32, 32), new GUIContent(Core.tRefresh, "Refresh Image")))
//                    // {
//                    //     RefreshLandmarkThumbnail(i);
//                    // }
//                    EditorGUI.ProgressBar(new Rect(pos.x, pos.y + pos.height, screenWidth - 20, 8f), scene.landmarks[i].progress, "");
//                    Color prevGUI = GUI.color;
//                    GUI.color = new Color(prevGUI.r, prevGUI.g, prevGUI.b, 0.5f);
//                    if (scene.landmarks[i].viewState == Landmark.ViewState.is2D)
//                        DrawTexture(new Rect(pos.x + pos.width - 50, pos.y + pos.height - 26, 48, 24), Core.tIcon2D);
//                    else if (scene.landmarks[i].viewState == Landmark.ViewState.is3D)
//                        DrawTexture(new Rect(pos.x + pos.width - 50, pos.y + pos.height - 26, 48, 24), Core.tIcon3D);
//                    else if (scene.landmarks[i].viewState == Landmark.ViewState.isOrthographic)
//                        DrawTexture(new Rect(pos.x + pos.width - 50, pos.y + pos.height - 26, 48, 24), Core.tIconISO);
//                    GUI.color = prevGUI;
//                    #endregion
//                }
//                GUILayout.EndVertical();

//                GUILayout.Space(2);
//                thumbnailSize = (screenWidth / 2);
//                float bSize = (thumbnailSize * 0.7f) / 6;
//                s = StyleCheck();
//                s.fontStyle = FontStyle.Bold;
//                s.fontSize = (int)(bSize * 0.5);
//                s.alignment = TextAnchor.MiddleLeft;
//                sBackground = new GUIStyle();

//                GUILayout.BeginVertical();
//                {
//                    #region GotoDetails
//                    sBackground.normal.background = Core.tGrey;
//                    GUILayout.BeginVertical(sBackground);
//                    {
//                        GUILayout.Space(((bSize * 1.2f) - bSize) / 2);
//                        GUILayout.BeginHorizontal(GUILayout.Height(bSize * 1.2f));
//                        {
//                            GUILayout.Space(4);
//                            if (GUILayout.Button(Core.tEditPen, GUILayout.Width(bSize), GUILayout.Height(bSize)))
//                            {
//                                scene.selectedLandmark = i;
//                                scene.landmarkDetailOpen = true;
//                                scene.landmarkDetailState = TA.LandmarkDetailState.general;
//                            }
//                            GUILayout.Label("Details", s, GUILayout.Width(screenWidth / 2 - bSize - 32), GUILayout.Height(bSize));
//                        }
//                        GUILayout.EndHorizontal();
//                    }
//                    GUILayout.EndVertical();
//                    #endregion

//                    GUILayout.Space(4);
//                    #region GotoTasks
//                    sBackground.normal.background = Core.tGreen;
//                    GUILayout.BeginVertical(sBackground);
//                    {
//                        GUILayout.Space(((bSize * 1.2f) - bSize) / 2);
//                        GUILayout.BeginHorizontal(GUILayout.Height(bSize * 1.2f));
//                        {
//                            GUILayout.Space(4);
//                            if (GUILayout.Button(Core.tIconTask, GUILayout.Width(bSize), GUILayout.Height(bSize)))
//                            {
//                                scene.selectedLandmark = i;
//                                scene.landmarkDetailOpen = true;
//                                scene.landmarkDetailState = TA.LandmarkDetailState.tasks;
//                            }
//                            GUILayout.Label("Tasks", s, GUILayout.Width(screenWidth / 2 - bSize - 32), GUILayout.Height(bSize));
//                        }
//                        GUILayout.EndHorizontal();
//                    }
//                    GUILayout.EndVertical();
//                    #endregion
//                    GUILayout.Space(4);
//                    #region RefreshImage
//                    sBackground.normal.background = Core.tGreenBright;
//                    GUILayout.BeginVertical(sBackground);
//                    {
//                        GUILayout.Space(((bSize * 1.2f) - bSize) / 2);
//                        GUILayout.BeginHorizontal(GUILayout.Height(bSize * 1.2f));
//                        {
//                            GUILayout.Space(4);
//                            if (GUILayout.Button(Core.tGallery, GUILayout.Width(bSize), GUILayout.Height(bSize)))
//                            {
//                                scene.selectedLandmark = i;
//                                scene.landmarkDetailOpen = true;
//                                scene.landmarkDetailState = TA.LandmarkDetailState.gallery;
//                            }
//                            GUILayout.Label("Gallery", s, GUILayout.Width(screenWidth / 2 - bSize - 32), GUILayout.Height(bSize));
//                        }
//                        GUILayout.EndHorizontal();
//                    }
//                    GUILayout.EndVertical();
//                    #endregion
//                    GUILayout.Space(4);
//                    #region RefreshImage
//                    sBackground.normal.background = Core.tTeal;
//                    GUILayout.BeginVertical(sBackground);
//                    {
//                        GUILayout.Space(((bSize * 1.2f) - bSize) / 2);
//                        GUILayout.BeginHorizontal(GUILayout.Height(bSize * 1.2f));
//                        {
//                            GUILayout.Space(4);
//                            if (GUILayout.Button(Core.tRefresh, GUILayout.Width(bSize), GUILayout.Height(bSize)))
//                            {
//                                RefreshLandmarkThumbnail(i);
//                            }
//                            GUILayout.Label("Refresh Image", s, GUILayout.Width(screenWidth / 2 - bSize - 32), GUILayout.Height(bSize));
//                        }
//                        GUILayout.EndHorizontal();
//                    }
//                    GUILayout.EndVertical();
//                    #endregion
//                    GUILayout.Space(4);
//                    #region MovetoSceneview
//                    sBackground.normal.background = Core.tBlue;
//                    GUILayout.BeginVertical(sBackground);
//                    {
//                        GUILayout.Space(((bSize * 1.2f) - bSize) / 2);
//                        GUILayout.BeginHorizontal(GUILayout.Height(bSize * 1.2f));
//                        {
//                            GUILayout.Space(4);
//                            if (GUILayout.Button(Core.tPosition, GUILayout.Width(bSize), GUILayout.Height(bSize)))
//                            {
//                                RefreshLandmarkPosition(i);
//                            }
//                            GUILayout.Label("Reposition Here", s, GUILayout.Width(screenWidth / 2 - bSize - 32), GUILayout.Height(bSize));
//                        }
//                        GUILayout.EndHorizontal();
//                    }
//                    GUILayout.EndVertical();
//                    #endregion
//                    GUILayout.Space(4);
//                    #region Delete
//                    sBackground.normal.background = Core.tRed;
//                    GUILayout.BeginVertical(sBackground);
//                    {
//                        GUILayout.Space(((bSize * 1.2f) - bSize) / 2);
//                        GUILayout.BeginHorizontal(GUILayout.Height(bSize * 1.2f));
//                        {
//                            GUILayout.Space(4);

//                            if (GUILayout.Button(Core.tTrashCan, GUILayout.Width(bSize), GUILayout.Height(bSize)))
//                            {
//                                if (scene.landmarks[i].tasks.Count() == 0)
//                                {
//                                    if (EditorUtility.DisplayDialog("Delete Landmark?", "Delete Landmark " + scene.landmarks[i].title + "? This cannot be undone!", "Delete", "Cancel"))
//                                    {
//                                        DeleteLandmark(i);
//                                    }
//                                }
//                                else
//                                {
//                                    EditorUtility.DisplayDialog("Cannot Delete", "You must remove, or move, all tasks in the Landmark " + scene.landmarks[i].title + " before you can remove it", "Ok");
//                                }
//                                exists = false;
//                            }
//                            GUILayout.Label("Delete", s, GUILayout.Width(screenWidth / 2 - bSize - 32), GUILayout.Height(bSize));
//                        }
//                        GUILayout.EndHorizontal();
//                    }
//                    GUILayout.EndVertical();
//                    #endregion
//                }
//            }
//            GUILayout.EndHorizontal();
//            GUILayout.EndHorizontal();
//            return exists;
//        }
//        static Rect rButton;
//        static void LoadViewLandmarkDetailButtons(int i)
//        {

//            GUIStyle s = StyleCheck();
//            GUILayout.BeginHorizontal();
//            {
//                s = StyleCheck();
//                s.fontSize = 20;
//                s.fontStyle = FontStyle.Bold;
//                s.alignment = TextAnchor.MiddleCenter;

//                s.normal.textColor = WhiteOrBlack(scene.landmarks[i].GizmoColor);
//                s.normal.background = scene.landmarks[i].tGizmoColor50;

//                GUILayout.BeginHorizontal(s);
//                GUILayout.FlexibleSpace();

//                text = scene.landmarks[i].title;
//                if (text.Length > 25) text = text.Substring(0, 15);

//                s.normal.background = Core.tTransparent;
//                GUILayout.Label(text, s);
//                GUILayout.FlexibleSpace();

//                GUILayout.EndHorizontal();

//                GUILayout.EndHorizontal();
//                GUILayout.BeginHorizontal();

//                GUILayout.Space(2);
//                thumbnailSize = (screenWidth / 2);
//                float bSize = 26;//(thumbnailSize * 0.7f) / 5;
//                s = StyleCheck();
//                s.fontStyle = FontStyle.Bold;
//                s.fontSize = (int)(bSize * 0.5);
//                s.alignment = TextAnchor.MiddleLeft;

//                sBackground = new GUIStyle();

//                if (scene.landmarkDetailState == TA.LandmarkDetailState.general)
//                {
//                    GUILayout.BeginVertical();
//                }
//                else
//                {
//                    sBackground.normal.background = Core.tGrey;

//                    GUILayout.BeginVertical(sBackground);
//                }
//                {
//                    GUILayout.Space(((bSize * 1.2f) - bSize) / 2);
//                    GUILayout.BeginHorizontal(GUILayout.Height(bSize * 1.2f));
//                    {
//                        GUILayout.Space(4);
//                        if (GUILayout.Button(Core.tEditPen, GUILayout.Width(bSize), GUILayout.Height(bSize)))
//                        {
//                            scene.selectedLandmark = i;
//                            scene.landmarkDetailOpen = true;
//                            scene.landmarkDetailState = TA.LandmarkDetailState.general;
//                        }

//                        GUILayout.Label("Details", s, GUILayout.Width(screenWidth / 3 - bSize - 32), GUILayout.Height(bSize));
//                    }
//                    GUILayout.EndHorizontal();
//                }
//                GUILayout.EndVertical();

//                if (scene.landmarkDetailState == TA.LandmarkDetailState.tasks)
//                {
//                    GUILayout.BeginVertical();
//                }
//                else
//                {
//                    sBackground.normal.background = Core.tGrey;
//                    GUILayout.BeginVertical(sBackground);
//                }
//                {
//                    GUILayout.Space(((bSize * 1.2f) - bSize) / 2);
//                    GUILayout.BeginHorizontal(GUILayout.Height(bSize * 1.2f));
//                    {
//                        GUILayout.Space(4);
//                        if (GUILayout.Button(Core.tIconTask, GUILayout.Width(bSize), GUILayout.Height(bSize)))
//                        {
//                            scene.selectedLandmark = i;
//                            scene.landmarkDetailOpen = true;
//                            scene.landmarkDetailState = TA.LandmarkDetailState.tasks;
//                        }
//                        GUILayout.Label("Tasks", s, GUILayout.Width(screenWidth / 3 - bSize - 32), GUILayout.Height(bSize));
//                    }
//                    GUILayout.EndHorizontal();
//                }
//                GUILayout.EndVertical();

//                GUILayout.BeginVertical(sBackground);
//                {
//                    GUILayout.Space(((bSize * 1.2f) - bSize) / 2);
//                    GUILayout.BeginHorizontal(GUILayout.Height(bSize * 1.2f));
//                    {
//                        GUILayout.Space(4);
//                        if (GUILayout.Button(Core.tRefresh, GUILayout.Width(bSize), GUILayout.Height(bSize)))
//                        {
//                            scene.selectedLandmark = i;
//                            scene.landmarkDetailOpen = true;
//                            scene.landmarkDetailState = TA.LandmarkDetailState.gallery;
//                        }
//                        GUILayout.Label("Gallery", s, GUILayout.Width(screenWidth / 3 - bSize - 32), GUILayout.Height(bSize));
//                    }
//                    GUILayout.EndHorizontal();
//                }
//                GUILayout.EndVertical();

//                // sBackground.normal.background = Core.tRed;
//                // GUILayout.BeginVertical(sBackground);

//                // {
//                //     GUILayout.Space(((bSize * 1.2f) - bSize) / 2);
//                //     GUILayout.BeginHorizontal(GUILayout.Height(bSize * 1.2f));
//                //     {
//                //         GUILayout.Space(4);
//                //         if (GUILayout.Button(Core.tArrange, GUILayout.Width(bSize), GUILayout.Height(bSize)))
//                //         {
//                //             scene.selectedLandmark = i;
//                //             scene.landmarkDetailOpen = true;
//                //             scene.landmarkDetailState = Scene.LandmarkDetailState.delete;
//                //         }
//                //         GUILayout.Label("Sort", s, GUILayout.Width(screenWidth / 3 - bSize - 32), GUILayout.Height(bSize));
//                //     }
//                //     GUILayout.EndHorizontal();
//                // }
//                // GUILayout.EndVertical();
//            }
//            GUILayout.EndHorizontal();
//        }

//        enum TagEditMode { none, edit, delete }
//        static TagEditMode tagEditMode = TagEditMode.none;
//        static void LoadViewLandmarkDetail()
//        {
//            int detailThumbnailSize = 160 - 24;
//            int optionSize = (int)(detailThumbnailSize) / 3;
//            var localData = scene.landmarks[scene.selectedLandmark];
//            Rect lastRect, headerRect;
//            string headerText = "";
//            Texture2D headerColor = Core.tTransparent;
//            Color headerColorText = new Color();
//            GUIStyle s;

//            GUILayout.BeginArea(new Rect(screenWidthMargin, 0, screenWidth - 6, screenHeight));
//            {
//                GUILayout.BeginHorizontal();
//                {
//                    GUISkin skin = ScriptableObject.CreateInstance<GUISkin>();
//                    skin.button = new GUIStyle("Button");

//                    if (GUILayout.Button("Back to Landmark List", skin.button, GUILayout.Height(18)))
//                    {
//                        scene.landmarkDetailOpen = false;
//                        tagEditMode = TagEditMode.none;
//                    }
//                    DrawTagFilterMask();
//                }
//                GUILayout.EndHorizontal();
//                GUILayout.Space(4);

//                LoadViewLandmarkCarousel();

//                LoadViewLandmarkDetailButtons(scene.selectedLandmark);
//                sBackground.normal.background = Core.tGreen;
//                GUILayout.BeginHorizontal();
//                {

//                    localWidth = screenWidth - 80;

//                    // HorizontalLine(Color.black);

//                    scene.scrollPosLandmarkDetail = GUILayout.BeginScrollView(scene.scrollPosLandmarkDetail, GUIStyle.none, GUILayout.Height(screenHeight - 60));

//                    GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
//                    {
//                        localWidth -= 32;

//                        switch (scene.landmarkDetailState)
//                        {
//                            #region GeneralState
//                            case TA.LandmarkDetailState.general:
//                                LoadViewLandmarkDetailGeneral(scene.selectedLandmark);
//                                break;
//                            #endregion

//                            #region GalleryState
//                            case TA.LandmarkDetailState.gallery:

//                                // GUILayout.BeginHorizontal();
//                                // {
//                                //     #region NewTask
//                                //     s = StyleCheck("button");
//                                //     s.fontSize = 20;
//                                //     s.alignment = TextAnchor.MiddleCenter;
//                                //     if (GUILayout.Button(new GUIContent("   New Image", Core.tPlus), s, GUILayout.Height(24), GUILayout.Width(screenWidth - 12)))
//                                //     {
//                                //         localData.gallery.Add(new Scene.Landmark.Gallery("New Image"));
//                                //         Core.SaveData();
//                                //     }
//                                //     s = StyleCheck();
//                                //     #endregion
//                                // }
//                                // GUILayout.EndHorizontal();

//                                var gallery = localData.gallery;
//                                float h = 20;
//                                if (gallery.currentImage < 0) gallery.currentImage = 0;

//                                if (gallery.images.Count == 0) h = 100;
//                                GUILayout.BeginHorizontal();
//                                {
//                                    if (GUILayout.Button(new GUIContent("Add Image", Core.tPlus), GUILayout.Height(h)))
//                                    {
//                                        gallery.images.Add(new Landmark.Gallery.Image((Texture2D)AssetDatabase.LoadAssetAtPath(Core.taPath + "/Screenshots/TaskAtlas_Default.png", typeof(Texture2D))));
//                                        gallery.currentImage = gallery.images.Count - 1;
//                                    }
//                                }
//                                GUILayout.EndHorizontal();

//                                HorizontalLine(Color.black);
//                                if (gallery.images.Count > 0)
//                                {
//                                    scene.scrollPosLandmarkDetailGalleryThumbnails = GUILayout.BeginScrollView(scene.scrollPosLandmarkDetailGalleryThumbnails, GUILayout.Height(100));
//                                    {
//                                        GUILayout.BeginHorizontal();
//                                        {
//                                            if (gallery.images.Count > 0)
//                                            {
//                                                GUILayout.FlexibleSpace();
//                                                for (int i = 0; i < gallery.images.Count; i++)
//                                                {
//                                                    GUILayout.BeginVertical();
//                                                    {
//                                                        if (i == gallery.currentImage)
//                                                        {
//                                                            GUILayout.Space(-1);
//                                                            GUILayout.Box(Core.tGreen, GUILayout.Width(64), GUILayout.Height(64));
//                                                            Rect r = GUILayoutUtility.GetLastRect();
//                                                            r = new Rect(r.x + 3, r.y + 3, r.width - 6, r.height - 6);
//                                                            if (GUI.Button(r, gallery.images[i].image))
//                                                            {
//                                                                gallery.currentImage = i;
//                                                            }

//                                                            GUILayout.Space(1);
//                                                        }
//                                                        else
//                                                        {
//                                                            if (GUILayout.Button(gallery.images[i].image, GUILayout.Width(64), GUILayout.Height(64)))
//                                                            {
//                                                                gallery.currentImage = i;
//                                                            }
//                                                        }
//                                                        GUILayout.BeginHorizontal(GUILayout.Width(64));
//                                                        {
//                                                            GUILayout.Space(14);
//                                                            if (GUILayout.Button(Core.tArrowLeft, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(12)))
//                                                            {
//                                                                if (i > 0)
//                                                                {
//                                                                    gallery.images.Move(i, i - 1);
//                                                                    if (i == gallery.currentImage) gallery.currentImage--;
//                                                                    else if (i - 1 == gallery.currentImage) gallery.currentImage++;
//                                                                }
//                                                            }
//                                                            if (GUILayout.Button(Core.tTrashCan, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(12)))
//                                                            {
//                                                                if (EditorUtility.DisplayDialog("Delete Image", "Are you sure you want to delete this image?  Cannot be undone!  Please note that the image file itself is not being deleted and can still be found in your project.", "Delete", "Cancel"))
//                                                                {
//                                                                    gallery.images.RemoveAt(i);
//                                                                    gallery.currentImage--;
//                                                                    GUILayout.EndHorizontal();
//                                                                    GUILayout.EndVertical();
//                                                                    GUILayout.EndHorizontal();
//                                                                    GUILayout.EndScrollView();
//                                                                    goto endGallery;
//                                                                }
//                                                            }
//                                                            if (GUILayout.Button(Core.tArrowRight, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(12)))
//                                                            {
//                                                                if (i < gallery.images.Count)
//                                                                {
//                                                                    gallery.images.Move(i, i + 1);
//                                                                    if (i == gallery.currentImage) gallery.currentImage++;
//                                                                    else if (i + 1 == gallery.currentImage) gallery.currentImage--;
//                                                                }
//                                                            }
//                                                            GUILayout.FlexibleSpace();
//                                                        }
//                                                        GUILayout.EndHorizontal();

//                                                    }
//                                                    GUILayout.EndVertical();
//                                                }

//                                            }

//                                            GUILayout.FlexibleSpace();
//                                        }
//                                        GUILayout.EndHorizontal();
//                                    }
//                                    GUILayout.EndScrollView();
//                                }
//                                s = new GUIStyle("button");

//                                if (gallery.images.Count > 0)
//                                {
//                                    GUILayout.BeginHorizontal();
//                                    {
//                                        GUILayout.Label((gallery.images[gallery.currentImage].scaleFactor * 100).ToString("F0") + "%", GUILayout.Width(80));
//                                        if (gallery.images[gallery.currentImage].scaleHeight) GUI.enabled = false;
//                                        var prev = gallery.images[gallery.currentImage].scaleFactor;
//                                        gallery.images[gallery.currentImage].scaleFactor = GUILayout.HorizontalSlider(gallery.images[gallery.currentImage].scaleFactor, 0.05f, 10.0f);
//                                        if (prev != gallery.images[gallery.currentImage].scaleFactor)
//                                        {
//                                            Core.SaveData();
//                                            // scene.scrollPosLandmarkDetailGallery.x = prev.x / (gallery.images[gallery.currentImage].width * gallery.scaleFactor) / scene.scrollPosLandmarkDetailGallery.x;
//                                            // scene.scrollPosLandmarkDetailGallery.y = prev.y / (gallery.images[gallery.currentImage].height * gallery.scaleFactor) / scene.scrollPosLandmarkDetailGallery.y;
//                                        }
//                                        if (GUILayout.Button("100%", GUILayout.Width(48)))
//                                        {
//                                            gallery.images[gallery.currentImage].scaleFactor = 1.0f;
//                                            Core.SaveData();
//                                        }
//                                        if (gallery.images[gallery.currentImage].scaleHeight) GUI.enabled = true;
//                                        var prev2 = gallery.images[gallery.currentImage].scaleHeight;
//                                        gallery.images[gallery.currentImage].scaleHeight = GUILayout.Toggle(gallery.images[gallery.currentImage].scaleHeight, "Fit", GUILayout.Width(48));//, s);
//                                        if (prev2 != gallery.images[gallery.currentImage].scaleHeight)
//                                            Core.SaveData();
//                                    }
//                                    GUILayout.EndHorizontal();
//                                    HorizontalLine(Color.black);

//                                    gallery.images[gallery.currentImage].scrollPos = GUILayout.BeginScrollView(gallery.images[gallery.currentImage].scrollPos, GUIStyle.none, GUILayout.Height(screenHeight - 380));
//                                    {
//                                        if (gallery.images.Count == 0)
//                                        {
//                                            GUILayout.Label("No Images, Add One By Clicking Above!");
//                                            GUILayout.EndScrollView();
//                                            goto endGallery;
//                                        }

//                                        if (gallery.currentImage > gallery.images.Count - 1) gallery.currentImage = gallery.images.Count - 1;

//                                        GUILayout.BeginVertical();
//                                        {
//                                            // if (gallery.images[gallery.currentImage] == null)
//                                            // {
//                                            //     gallery.currentImage = gallery.images.Count;
//                                            // }
//                                            if (gallery.currentImage > gallery.images.Count) gallery.currentImage = gallery.images.Count - 1;
//                                            if (gallery.currentImage < 0) gallery.currentImage = 0;
//                                            var image = gallery.images[gallery.currentImage].image;

//                                            if (image == null)
//                                            {
//                                                gallery.images.RemoveAt(gallery.currentImage);
//                                                gallery.currentImage--;
//                                                GUILayout.EndVertical();
//                                                GUILayout.EndVertical();
//                                                GUILayout.EndScrollView();

//                                                goto endGallery;
//                                            }

//                                            // float x = screenWidth - 32, y = ((screenWidth - 32) / image.width) * image.height;
//                                            float x = image.width, y = image.height;

//                                            // Debug.Log(r + "|" + screenHeight + "|" + (screenHeight - r.y));
//                                            if (gallery.images[gallery.currentImage].scaleHeight)
//                                            {
//                                                float rS = (screenWidth - 32) / (screenHeight - 350), rI = x / y;
//                                                if (rS > rI)
//                                                {
//                                                    x = (x * (screenHeight - 380) / y);
//                                                    y = (screenHeight - 380);
//                                                }
//                                                else
//                                                {
//                                                    x = (screenWidth - 32);
//                                                    y = (y * (screenWidth - 32) / x);
//                                                }
//                                            }
//                                            else
//                                            {
//                                                x = image.width * gallery.images[gallery.currentImage].scaleFactor;
//                                                y = image.height * gallery.images[gallery.currentImage].scaleFactor;
//                                            }

//                                            GUILayout.FlexibleSpace();
//                                            GUILayout.BeginHorizontal();
//                                            GUILayout.FlexibleSpace();

//                                            var prev3 = gallery.images[gallery.currentImage].image;
//                                            gallery.images[gallery.currentImage].image = (Texture2D)EditorGUILayout.ObjectField(image, typeof(Texture2D), false,
//                                                                                                                          GUILayout.Width(x), GUILayout.Height(y));
//                                            if (prev3 != gallery.images[gallery.currentImage].image)
//                                                Core.SaveData();

//                                            GUILayout.FlexibleSpace();
//                                            GUILayout.EndHorizontal();
//                                            GUILayout.FlexibleSpace();
//                                        }
//                                        GUILayout.EndVertical();


//                                    }
//                                    GUILayout.EndScrollView();
//                                    GUILayout.BeginHorizontal();
//                                    {
//                                        if (GUILayout.Button("Replace with Current View"))
//                                        {
//                                            Camera svc = SceneView.lastActiveSceneView.camera;
//                                            RenderTexture currentRT = new RenderTexture(1024, 1024, 24);
//                                            svc.targetTexture = currentRT;
//                                            svc.Render();

//                                            RenderTexture.active = currentRT;

//                                            Texture2D image = new Texture2D(1024, 1024, TextureFormat.RGB24, false);
//                                            image.ReadPixels(new Rect(0, 0, 1024, 1024), 0, 0);
//                                            image.Apply();

//                                            svc.targetTexture = null;
//                                            RenderTexture.active = null;

//                                            if (gallery.images == null) gallery.images = new List<Landmark.Gallery.Image>();
//                                            gallery.images[gallery.currentImage].image = (Texture2D)AssetDatabase.LoadAssetAtPath(SaveTexture(image), typeof(Texture2D));
//                                            DestroyImmediate(image);

//                                            // gallery.images.Add(Core.tEditPen);
//                                            gallery.currentImage = gallery.images.Count - 1;
//                                        }

//                                    }
//                                    GUILayout.EndHorizontal();
//                                }
//                            endGallery:;
//                                break;
//                            #endregion

//                            #region TaskState
//                            case TA.LandmarkDetailState.tasks:
//                                GUILayout.BeginHorizontal();
//                                {
//                                    #region NewTask
//                                    s = StyleCheck("button");
//                                    s.fontSize = 20;
//                                    s.alignment = TextAnchor.MiddleCenter;
//                                    // sBackground.normal.background = Core.tRed;
//                                    // GUILayout.BeginVertical(sBackground);

//                                    // {
//                                    //     GUILayout.Space(((bSize * 1.2f) - bSize) / 2);
//                                    //     GUILayout.BeginHorizontal(GUILayout.Height(bSize * 1.2f));
//                                    //     {
//                                    //         GUILayout.Space(4);
//                                    //         if (GUILayout.Button(Core.tArrange, GUILayout.Width(bSize), GUILayout.Height(bSize)))
//                                    //         {
//                                    //             scene.selectedLandmark = i;
//                                    //             scene.landmarkDetailOpen = true;
//                                    //             scene.landmarkDetailState = Scene.LandmarkDetailState.delete;
//                                    //         }
//                                    //         GUILayout.Label("Sort", s, GUILayout.Width(screenWidth / 3 - bSize - 32), GUILayout.Height(bSize));
//                                    //     }
//                                    //     GUILayout.EndHorizontal();
//                                    // }
//                                    // GUILayout.EndVertical();



//                                    if (GUILayout.Button(new GUIContent("  New Task", Core.tPlus), s, GUILayout.Height(32), GUILayout.Width((screenWidth - 12) / 2)))
//                                    {
//                                        localData.tasks.Add(new Task("New Task"));
//                                        Core.SaveData();
//                                    }

//                                    if (GUILayout.Button(new GUIContent("  Arrange", Core.tArrange), s, GUILayout.Height(32), GUILayout.Width((screenWidth - 12) / 2)))
//                                    {
//                                        // scene.selectedLandmark = i;
//                                        scene.landmarkDetailOpen = true;
//                                        scene.landmarkDetailState = TA.LandmarkDetailState.delete;
//                                    }
//                                    s = StyleCheck();
//                                    #endregion
//                                }
//                                GUILayout.EndHorizontal();

//                                lastRect = GUILayoutUtility.GetLastRect();
//                                headerRect = GUILayoutUtility.GetLastRect();

//                                scene.scrollPosLandmarkDetailTags = GUILayout.BeginScrollView(scene.scrollPosLandmarkDetailTags, GUIStyle.none, GUILayout.Height(screenHeight - 128 - 120));
//                                {
//                                    GUILayout.BeginVertical(StyleBack(s, Core.tGreyBright));
//                                    {

//                                        if (localData.tasks.Count == 0)
//                                        {
//                                            GUILayout.Label("No Tasks, Add One By Clicking Above!");
//                                            goto end;
//                                        }
//                                        for (int i = 0; i < localData.tasks.Count; i++)
//                                        {
//                                            //string checksum = "",checksum2="";
//                                            EditorGUI.BeginChangeCheck();
//                                            var prev = localData.tasks[i].Copy();
//                                            var tasks = localData.tasks[i];
//                                            HorizontalLine(Color.black);
//                                            Rect rID = GUILayoutUtility.GetLastRect();

//                                            s = StyleCheck();

//                                            s.alignment = TextAnchor.LowerCenter;
//                                            s.fontSize = 20;
//                                            s.fontStyle = FontStyle.Bold;

//                                            if (tasks.color.r > .5f || tasks.color.g > .5f || tasks.color.b > .5f)
//                                            {
//                                                s.normal.textColor = Color.black;
//                                            }
//                                            else
//                                            {
//                                                s.normal.textColor = Color.white;
//                                            }

//                                            sBackground = StyleBack(sBackground, tasks.tColor);
//                                            GUILayout.Space(48);

//                                            lastRect = GUILayoutUtility.GetLastRect();
//                                            GUILayout.BeginVertical(sBackground);
//                                            {
//                                                GUILayout.Space(32);
//                                                GUILayout.BeginHorizontal();
//                                                {
//                                                    text = tasks.name;
//                                                    if (text.Length > 20) text = text.Substring(0, 17) + "...";
//                                                    GUILayout.Label(text, s);
//                                                }
//                                                GUILayout.EndHorizontal();
//                                            }
//                                            GUILayout.EndVertical();
//                                            DrawTexture(new Rect(screenWidth / 2 - 32 - 16, lastRect.y + 16, 64, 64), Core.tWhite);
//                                            if (tasks.isSticky)
//                                            {
//                                                DrawTexture(new Rect(screenWidth / 2 - 24 - 16, lastRect.y + 24, 48, 48), Core.tTaskSticky);
//                                            }
//                                            else
//                                            {
//                                                DrawTexture(new Rect(screenWidth / 2 - 24 - 16, lastRect.y + 24, 48, 48), Core.tIconTask);
//                                            }

//                                            lastRect = GUILayoutUtility.GetLastRect();
//                                            if (scene.scrollPosLandmarkDetailTags.y > lastRect.y + lastRect.height - 20)
//                                            {
//                                                sTaskHeader = sBackground;
//                                                headerText = text;
//                                                headerColor = tasks.tColor;
//                                                if (tasks.color.r > .5f || tasks.color.g > .5f || tasks.color.b > .5f)
//                                                {
//                                                    headerColorText = Color.black;
//                                                }
//                                                else
//                                                {
//                                                    headerColorText = Color.white;
//                                                }
//                                            }

//                                            GUILayout.Space(8);
//                                            #region ZoomToTaskOnSelection
//                                            if (Event.current.type == EventType.Repaint && scene.zoomToTaskID == tasks.createdDate)
//                                            {
//                                                scene.scrollPosLandmarkDetailTags = new Vector2(0, rID.y);
//                                                scene.zoomToTaskID = "";
//                                            }
//                                            #endregion

//                                            #region Progress Stage Priority
//                                            if (EditorGUIUtility.isProSkin)
//                                            {
//                                                sBackground = StyleBack(sBackground, Core.tGreyDark);
//                                            }
//                                            else
//                                            {
//                                                sBackground = StyleBack(sBackground, Core.tGreyBright);
//                                            }
//                                            GUILayout.BeginHorizontal(sBackground);
//                                            {
//                                                GUILayout.Label("Progress: ", GUILayout.Width(64));
//                                                if (tasks.subTasksAutoProgress) GUI.enabled = false;

//                                                //checksum += tasks.progress.ToString();
//                                                tasks.progress = EditorGUILayout.IntSlider(tasks.progress, 0, 100);
//                                                //checksum2 += tasks.progress.ToString();
//                                                if (tasks.subTasksAutoProgress) GUI.enabled = true;

//                                                GUILayout.Label("Color:", GUILayout.Width(50));
//                                                //checksum += tasks.color;
//                                                tasks.SetColor(EditorGUILayout.ColorField(tasks.color, GUILayout.Width(50)));
//                                                //checksum2 += tasks.color;
//                                                GUILayout.FlexibleSpace();

//                                                { GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(sBackground); }

//                                                GUILayout.Label("Stage: ", GUILayout.Width(64));
//                                                //
//                                                //checksum += tasks.stage.ToString();
//                                                tasks.stage = (Task.Stage)EditorGUILayout.EnumPopup(tasks.stage, GUILayout.Width(80));
//                                                //checksum2 += tasks.stage.ToString();
//                                                GUILayout.FlexibleSpace();
//                                                //if (screenWidth < 300) { GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(sBackground); }

//                                                GUILayout.Label("Priority: ", GUILayout.Width(64));
//                                                //checksum += tasks.priority.ToString();
//                                                tasks.priority = (Task.Priority)EditorGUILayout.EnumPopup(tasks.priority, GUILayout.Width(80));
//                                                //checksum2 += tasks.priority.ToString();
//                                            }
//                                            GUILayout.EndHorizontal();
//                                            #endregion
//                                            string arrow;
//                                            if (tasks.isFolded) arrow = "▼"; else arrow = "▲";
//                                            if (GUILayout.Button(arrow))
//                                            {
//                                                tasks.isFolded = !tasks.isFolded;
//                                            }
//                                            if (!tasks.isFolded)
//                                            {

//                                                GUILayout.Label("Task Label (Keep Short): ");
//                                                GUILayout.BeginHorizontal(sBackground);
//                                                {
//                                                    // s = GetTextAreaStyle(tasks.name, screenWidth - 32);
//                                                    //checksum += tasks.name;
//                                                    tasks.name = EditorGUILayout.TextField(tasks.name, GUILayout.Width(120));
//                                                    //checksum2 += tasks.name;
//                                                }
//                                                GUILayout.EndHorizontal();

//                                                GUILayout.Label("Description: ");
//                                                GUILayout.BeginHorizontal(sBackground);
//                                                {
//                                                    s = GetTextAreaStyle(tasks.description, screenWidth - 32);
//                                                    //checksum += tasks.description;
//                                                    tasks.description = EditorGUILayout.TextField(tasks.description, s, GUILayout.Width(screenWidth - 32), GUILayout.Height(s.fixedHeight));
//                                                    //checksum2 += tasks.description;
//                                                }
//                                                GUILayout.EndHorizontal();

//                                                #region Alerts
//                                                string n = "Enable";
//                                                if (tasks.alert) n = "Disable";
//                                                Texture2D icon = Core.tDate;
//                                                if (tasks.alert && tasks.dueDateDT < DateTime.Now.Ticks) icon = Core.tIconExclaim;
//                                                if (DrawTaskHeader("Due Date", icon, Core.tGreen, true, 160, 18, new GUIContent(n)))
//                                                {
//                                                    //checksum += tasks.alert.ToString();
//                                                    tasks.alert = !tasks.alert;
//                                                    //checksum2 += tasks.alert.ToString();
//                                                }

//                                                if (tasks.alert)
//                                                {
//                                                    sBackground = StyleBack(sBackground, Core.tGreyDark);
//                                                    GUILayout.BeginHorizontal(sBackground);
//                                                    {
//                                                        EditorGUI.BeginChangeCheck();
//                                                        //checksum += tasks.month.ToString();
//                                                        tasks.month = (Task.Month)EditorGUILayout.EnumPopup(tasks.month);
//                                                        //checksum2 += tasks.month.ToString();
//                                                        List<int> o = new List<int>();
//                                                        List<string> slist = new List<string>();
//                                                        if (tasks.year == 0) tasks.year = DateTime.Now.Year;
//                                                        int x = (int)tasks.month + 1;
//                                                        if (x > 12) x = 1;
//                                                        for (int ii = 0; ii < DateTime.DaysInMonth(tasks.year, x); ii++)
//                                                        {
//                                                            o.Add(ii + 1);
//                                                            slist.Add((ii + 1).ToString());
//                                                        }
//                                                        //checksum += tasks.day.ToString();
//                                                        tasks.day = EditorGUILayout.IntPopup(tasks.day, slist.ToArray(), o.ToArray());
//                                                        //checksum2 += tasks.day.ToString();
//                                                        o = new List<int>();
//                                                        slist = new List<string>();
//                                                        for (int ii = DateTime.Now.Year; ii < DateTime.Now.Year + 10; ii++)
//                                                        {
//                                                            o.Add(ii);
//                                                            slist.Add(ii.ToString());
//                                                        }
//                                                        //checksum += tasks.year.ToString();
//                                                        tasks.year = EditorGUILayout.IntPopup(tasks.year, slist.ToArray(), o.ToArray());
//                                                        //checksum2 += tasks.year.ToString();
//                                                        if (EditorGUI.EndChangeCheck())
//                                                        {
//                                                            tasks.UpdateDueDate();
//                                                        }
//                                                    }
//                                                    GUILayout.EndHorizontal();
//                                                }
//                                                #endregion

//                                                #region SubTasks
//                                                if (DrawTaskHeader("Subtasks", Core.tIconTask, Core.tTeal, true, 160, 18, new GUIContent("Add Subtask")))
//                                                {
//                                                    tasks.subTasks.Add(new SubTask("New Subtask"));
//                                                }

//                                                sBackground = StyleBack(sBackground, Core.tGreyDark);
//                                                GUILayout.BeginHorizontal(sBackground);
//                                                {
//                                                    for (int st = 0; st < tasks.subTasks.Count; st++)
//                                                    {
//                                                        if (tasks.subTasks[st].complete)
//                                                        {
//                                                            sBackground = StyleBack(sBackground, Core.tTealDark);
//                                                        }
//                                                        else
//                                                        {
//                                                            sBackground = StyleBack(sBackground, Core.tGreyDark);
//                                                        }
//                                                        if (st == 0)
//                                                        {
//                                                            //checksum += tasks.subTasksAutoProgress.ToString();
//                                                            tasks.subTasksAutoProgress = GUILayout.Toggle(tasks.subTasksAutoProgress, "Auto Progress", "Button");
//                                                            //checksum2 += tasks.subTasksAutoProgress.ToString();
//                                                            GUILayout.EndHorizontal();
//                                                            GUILayout.BeginHorizontal();
//                                                        }
//                                                        GUILayout.BeginVertical(sBackground, GUILayout.Height(24));
//                                                        {
//                                                            GUILayout.Space(4);
//                                                            GUILayout.BeginHorizontal();
//                                                            {
//                                                                var prevComplete = tasks.subTasks[st].complete;
//                                                                tasks.subTasks[st].complete = GUILayout.Toggle(tasks.subTasks[st].complete, "", GUILayout.Width(24));
//                                                                if (prevComplete != tasks.subTasks[st].complete)
//                                                                {
//                                                                    scene.UpdateAllProgress(scene.stickyFont);
//                                                                    QueueRepaint();
//                                                                    Core.SaveData();
//                                                                }
//                                                                tasks.subTasks[st].name = EditorGUILayout.DelayedTextField(tasks.subTasks[st].name);
//                                                                if (GUILayout.Button(Core.tTrashCan, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(16)))
//                                                                {
//                                                                    tasks.subTasks.RemoveAt(st);
//                                                                    scene.UpdateAllProgress(scene.stickyFont);
//                                                                    QueueRepaint();
//                                                                    Core.SaveData();
//                                                                }
//                                                            }
//                                                            GUILayout.EndHorizontal();
//                                                        }
//                                                        GUILayout.EndVertical();
//                                                        GUILayout.EndHorizontal();
//                                                        GUILayout.BeginHorizontal();
//                                                    }
//                                                    if (EditorGUI.EndChangeCheck())
//                                                    {
//                                                        Core.SaveData();
//                                                    }
//                                                }
//                                                GUILayout.EndHorizontal();
//                                                #endregion

//                                                #region Sticky
//                                                sBackground = StyleBack(sBackground, Core.tGreyBright);
//                                                string dt = tasks.createdDate;

//                                                void StickyTaskReposition()
//                                                {
//                                                    tasks.position = Core.GetSVCameraPosition();
//                                                    if (SceneView.lastActiveSceneView.in2DMode) tasks.position = new Vector3(tasks.position.x, tasks.position.y, tasks.position.z + 10);
//                                                }

//                                                if (!tasks.isSticky)
//                                                {
//                                                    if (DrawTaskHeader("Sticky Task", Core.tPosition, Core.tBlue, true, 160, 18, new GUIContent("Add To Scene")))
//                                                    {
//                                                        tasks.isSticky = true;
//                                                        StickyTaskReposition();
//                                                    }
//                                                    #endregion
//                                                }
//                                                else
//                                                {

//                                                    #region RemoveFromScene
//                                                    if (DrawTaskHeader("Sticky Task", Core.tPosition, Core.tBlue, true, 160, 18, new GUIContent("Remove from Scene")))
//                                                    {
//                                                        tasks.isSticky = false;
//                                                    }
//                                                    #endregion

//                                                }

//                                                if (tasks.isSticky)
//                                                {
//                                                    sBackground = StyleBack(sBackground, Core.tBlueDark);
//                                                    GUILayout.BeginHorizontal(sBackground);
//                                                    {
//                                                        GUILayout.Label("Position: " + tasks.position);
//                                                        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
//                                                        if (localData.viewState != Landmark.ViewState.is2D)
//                                                        {
//                                                            GUILayout.Label("Scale Factor");
//                                                            GUILayout.FlexibleSpace();
//                                                            GUILayout.Label("Max:");
//                                                            scene.stickyScaleMax = EditorGUILayout.FloatField(scene.stickyScaleMax, GUILayout.Width(48));
//                                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();


//                                                            tasks.scale = EditorGUILayout.Slider(tasks.scale, 1, scene.stickyScaleMax);

//                                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
//                                                            s = new GUIStyle("button");

//                                                            tasks.useDefaultFadeDistance = GUILayout.Toggle(tasks.useDefaultFadeDistance, "Use Landmark Fade Distance");
//                                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
//                                                            if (tasks.useDefaultFadeDistance)
//                                                            {
//                                                                GUILayout.FlexibleSpace();
//                                                                GUILayout.Label("Important: Set Task Fade Distance in Details Tab");
//                                                                GUILayout.FlexibleSpace();
//                                                                GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
//                                                                GUI.enabled = false;
//                                                                EditorGUILayout.MinMaxSlider(ref localData.taskFadeStart, ref localData.taskFadeEnd, 0f, 1000f);
//                                                                GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
//                                                                s = new GUIStyle("textfield");
//                                                                GUILayout.Label("Fade Start at:");
//                                                                GUILayout.Label(localData.taskFadeStart.ToString("F1"), s);
//                                                                GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
//                                                                GUILayout.Label("Fade End at:");
//                                                                GUILayout.Label(localData.taskFadeEnd.ToString("F1"), s);
//                                                                GUI.enabled = true;
//                                                            }
//                                                            else
//                                                            {
//                                                                GUILayout.Space(8);
//                                                                EditorGUILayout.MinMaxSlider(ref tasks.fadeStart, ref tasks.fadeEnd, 0f, localData.taskFadeMax);
//                                                                GUILayout.Label("Max:");
//                                                                localData.taskFadeMax = EditorGUILayout.FloatField(localData.taskFadeMax, GUILayout.Width(48));
//                                                                GUILayout.Space(8);
//                                                                GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
//                                                                s = new GUIStyle("textfield");
//                                                                GUILayout.Label("Fade Start at:");
//                                                                if (GUILayout.Button("Set Here"))
//                                                                {
//                                                                    tasks.fadeStart = Vector3.Distance(Core.GetSVCameraPosition(), tasks.position);
//                                                                    if (tasks.fadeStart > tasks.fadeEnd) tasks.fadeStart = tasks.fadeEnd;
//                                                                }
//                                                                GUILayout.Label(tasks.fadeStart.ToString("F1"), s);
//                                                                GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
//                                                                //GUILayout.FlexibleSpace();
//                                                                GUILayout.Label("Fade End at:");
//                                                                if (GUILayout.Button("Set Here"))
//                                                                {
//                                                                    tasks.fadeEnd = Vector3.Distance(Core.GetSVCameraPosition(), tasks.position);
//                                                                    if (tasks.fadeEnd < tasks.fadeStart) tasks.fadeStart = tasks.fadeEnd;
//                                                                    if (tasks.fadeEnd > localData.taskFadeMax)
//                                                                    {
//                                                                        if (EditorUtility.DisplayDialog("Fade Distance bigger than Max", "Distance from task is " + tasks.fadeEnd + " but Max Distance is " + localData.taskFadeMax + ".  Set to Max Distance, or cancel?", "Set to " + localData.taskFadeMax, "Cancel"))
//                                                                        {
//                                                                            tasks.fadeEnd = localData.taskFadeMax;
//                                                                        }
//                                                                    }
//                                                                }
//                                                                GUILayout.Label(tasks.fadeEnd.ToString("F1"), s);
//                                                            }
//                                                        }

//                                                        s = new GUIStyle("button");
//                                                        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(sBackground);
//                                                        tasks.moveMode = GUILayout.Toggle(tasks.moveMode, new GUIContent("Move Sticky Task in Scene", "Manually Move the Sticky with Position Handles"), s);
//                                                        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(sBackground);
//                                                        if (GUILayout.Button(new GUIContent("Move Sticky Task to Current View", "Reposition Sticky in front of you")))
//                                                        {
//                                                            StickyTaskReposition();
//                                                        }
//                                                        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(sBackground);
//                                                        if (GUILayout.Button(new GUIContent("Go to Sticky Task in Scene", "Move SceneView to be in front of this Sticky")))
//                                                        {
//                                                            if (localData.viewState == Landmark.ViewState.is2D)
//                                                            {
//                                                                scene.atlas.prevPosition = tasks.position;
//                                                                scene.atlas.landmarkSelected = -1;

//                                                                TaskAtlasSceneView.EndAtlasMode(tasks.position, tasks.position, false, false);
//                                                                ZoomToLocation(scene.selectedLandmark, i);
//                                                            }
//                                                            else
//                                                            {
//                                                                scene.atlas.prevPosition = tasks.position + (Vector3.forward * -10);
//                                                                scene.atlas.landmarkSelected = -1;

//                                                                TaskAtlasSceneView.EndAtlasMode(tasks.position + (Vector3.forward * ((tasks.scale / 4))), tasks.position, false);
//                                                            }
//                                                        }


//                                                    }

//                                                    GUILayout.EndHorizontal();
//                                                    GUILayout.FlexibleSpace();
//                                                    #endregion
//                                                    #region Timers
//                                                    n = "Enable";
//                                                    if (tasks.autoTimer) n = "Disable";
//                                                    if (DrawTaskHeader("Auto Timer", Core.tSortDate, Core.tGreen, true, 160, 18, new GUIContent(n)))
//                                                    {
//                                                        tasks.autoTimer = !tasks.autoTimer;
//                                                    }

//                                                    sBackground = StyleBack(sBackground, Core.tGreyDark);

//                                                    GUILayout.BeginVertical(sBackground);
//                                                    {
//                                                        GUI.enabled = tasks.autoTimer;

//                                                        if (tasks.autoTimer)
//                                                        {
//                                                            GUILayout.BeginHorizontal();
//                                                            {
//                                                                GUILayout.FlexibleSpace();
//                                                                GUILayout.Label("Active Time");
//                                                                GUILayout.FlexibleSpace();
//                                                            }
//                                                            GUILayout.EndHorizontal();
//                                                            GUILayout.BeginHorizontal();
//                                                            {
//                                                                s = StyleCheck();
//                                                                s.fontSize = 32;
//                                                                s.fontStyle = FontStyle.Bold;
//                                                                long am = tasks.activeMinutes;

//                                                                if (am < 10080) text = am / 1440 + "d " + am % 1440 / 60 + "h " + am % 60 + "m";
//                                                                if (am < 1440) text = am / 60 + "h " + am % 60 + "m";
//                                                                if (am < 60) text = am + "m";

//                                                                GUILayout.FlexibleSpace();
//                                                                GUILayout.Label(text, s);
//                                                                GUILayout.FlexibleSpace();
//                                                            }
//                                                            GUILayout.EndHorizontal();
//                                                            sBackground = new GUIStyle();
//                                                            if (tasks.isTracking)
//                                                            {
//                                                                sBackground.normal.background = Core.tTeal;
//                                                                text = "TRACKING";
//                                                            }
//                                                            else
//                                                            {
//                                                                text = "OUT OF RANGE";
//                                                                sBackground.normal.background = Core.tGrey;
//                                                            }
//                                                            GUILayout.BeginHorizontal(sBackground);
//                                                            {
//                                                                s = StyleCheck();
//                                                                s.fontSize = 14;
//                                                                s.fontStyle = FontStyle.Bold;
//                                                                GUILayout.FlexibleSpace();
//                                                                GUILayout.Label(text, s);
//                                                                GUILayout.FlexibleSpace();

//                                                                GUILayout.EndHorizontal();
//                                                                GUILayout.BeginHorizontal(sBackground);

//                                                                GUILayout.FlexibleSpace();
//                                                                if (localData.viewState != Landmark.ViewState.is2D)
//                                                                {
//                                                                    GUILayout.Label("Current Distance: " + tasks.timeTrackingDistanceCurrent);
//                                                                }
//                                                                else
//                                                                {
//                                                                    if (tasks.isTracking)
//                                                                    {
//                                                                        GUILayout.Label("Sticky Task is within view!");
//                                                                    }
//                                                                    else
//                                                                    {
//                                                                        GUILayout.Label("Sticky Task off screen...");
//                                                                    }
//                                                                }
//                                                                GUILayout.FlexibleSpace();
//                                                            }
//                                                            GUILayout.EndHorizontal();
//                                                            if (localData.viewState != Landmark.ViewState.is2D)
//                                                            {
//                                                                GUILayout.BeginHorizontal();
//                                                                {
//                                                                    GUILayout.Label("Tracking Distance:");
//                                                                    GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
//                                                                    tasks.timeTrackingDistance = EditorGUILayout.Slider(tasks.timeTrackingDistance, 0.1f, tasks.timeTrackingDistanceScale);
//                                                                    GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
//                                                                    GUILayout.Label("Scale:");
//                                                                    tasks.timeTrackingDistanceScale = EditorGUILayout.FloatField(tasks.timeTrackingDistanceScale, GUILayout.Width(48));
//                                                                    GUILayout.FlexibleSpace();
//                                                                    s = new GUIStyle("button");
//                                                                    s.fontSize = 10;
//                                                                    if (tasks.showDistanceSphere)
//                                                                    {
//                                                                        s.normal.background = Core.tGreenBright;
//                                                                        s.normal.textColor = Color.black;
//                                                                    }
//                                                                    tasks.showDistanceSphere = GUILayout.Toggle(tasks.showDistanceSphere, "Show Time Ball", s, GUILayout.Height(20));
//                                                                }
//                                                                GUILayout.EndHorizontal();
//                                                            }
//                                                        }

//                                                        GUILayout.BeginHorizontal();
//                                                        {
//                                                            if (GUILayout.Button("Reset Timers"))
//                                                            {
//                                                                if (EditorUtility.DisplayDialog("Reset Time Tracked?", "This will set all timers for '" + tasks.name + "' to zero. This cannot be undone.", "Reset", "Cancel"))
//                                                                {
//                                                                    tasks.activeMinutes = tasks.idleMinutes = tasks.sleepMinutes = 0;
//                                                                }
//                                                            }
//                                                            s = new GUIStyle("button");
//                                                        }
//                                                        GUILayout.EndHorizontal();

//                                                        GUILayout.BeginHorizontal();
//                                                        {
//                                                            tasks.editTimers = GUILayout.Toggle(tasks.editTimers, "Manually Adjust", s);
//                                                        }
//                                                        GUILayout.EndHorizontal();

//                                                        if (tasks.editTimers)
//                                                        {
//                                                            GUILayout.BeginHorizontal();
//                                                            {
//                                                                GUILayout.Label("Active: ", GUILayout.Width(48));
//                                                                s = new GUIStyle("textfield");
//                                                                s.alignment = TextAnchor.MiddleRight;
//                                                                long am = tasks.activeMinutes;

//                                                                text = am / 1440 + "d " + am % 1440 / 60 + "h " + am % 60 + "m";
//                                                                if (am < 1440) text = am / 60 + "h " + am % 60 + "m";
//                                                                if (am < 60) text = am + "m";

//                                                                GUILayout.Label(text, s, GUILayout.Width(80));
//                                                                s = new GUIStyle("button");
//                                                                if (GUILayout.Button("+", s, GUILayout.Width(32), GUILayout.Height(20))) tasks.activeMinutes += Core.timerManualMinutes;
//                                                                if (GUILayout.Button("-", s, GUILayout.Width(32), GUILayout.Height(20))) if (tasks.activeMinutes > 0) tasks.activeMinutes -= Core.timerManualMinutes;
//                                                                s = new GUIStyle("textfield");
//                                                                s.alignment = TextAnchor.MiddleRight;
//                                                                Core.timerManualMinutes = EditorGUILayout.IntField(Core.timerManualMinutes, s, GUILayout.Width(32));
//                                                                GUILayout.Label("min");
//                                                                GUILayout.FlexibleSpace();
//                                                                s = new GUIStyle("button");
//                                                                if (GUILayout.Button("Reset", s, GUILayout.Width(64), GUILayout.Height(20)))
//                                                                {
//                                                                    if (EditorUtility.DisplayDialog("Reset Time Tracked?", "This will set Active Timer for '" + tasks.name + "' to zero. This cannot be undone.", "Reset", "Cancel"))
//                                                                    {
//                                                                        tasks.activeMinutes = 0;
//                                                                    }
//                                                                }
//                                                            }
//                                                            GUILayout.EndHorizontal();
//                                                            GUILayout.BeginHorizontal();
//                                                            {
//                                                                GUILayout.Label("Idle: ", GUILayout.Width(48));
//                                                                s = new GUIStyle("textfield");
//                                                                s.alignment = TextAnchor.MiddleRight;
//                                                                long am = tasks.idleMinutes;

//                                                                text = am / 1440 + "d " + am % 1440 / 60 + "h " + am % 60 + "m";
//                                                                if (am < 1440) text = am / 60 + "h " + am % 60 + "m";
//                                                                if (am < 60) text = am + "m";
//                                                                GUILayout.Label(text, s, GUILayout.Width(80));
//                                                                s = new GUIStyle("button");
//                                                                if (GUILayout.Button("+", s, GUILayout.Width(32), GUILayout.Height(20))) tasks.idleMinutes += Core.timerManualMinutes;
//                                                                if (GUILayout.Button("-", s, GUILayout.Width(32), GUILayout.Height(20)))
//                                                                    if (tasks.idleMinutes > 0) tasks.idleMinutes -= Core.timerManualMinutes;
//                                                                s = new GUIStyle("textfield");
//                                                                s.alignment = TextAnchor.MiddleRight;
//                                                                Core.timerManualMinutes = EditorGUILayout.IntField(Core.timerManualMinutes, s, GUILayout.Width(32));
//                                                                GUILayout.Label("min");
//                                                                GUILayout.FlexibleSpace();
//                                                                s = new GUIStyle("button");
//                                                                if (GUILayout.Button("Reset", s, GUILayout.Width(64), GUILayout.Height(20)))
//                                                                {
//                                                                    if (EditorUtility.DisplayDialog("Reset Time Tracked?", "This will set Idle Timer for '" + tasks.name + "' to zero. This cannot be undone.", "Reset", "Cancel"))
//                                                                    {
//                                                                        tasks.idleMinutes = 0;
//                                                                    }
//                                                                }
//                                                            }
//                                                            GUILayout.EndHorizontal();
//                                                            GUILayout.BeginHorizontal();
//                                                            {
//                                                                GUILayout.Label("Sleep: ", GUILayout.Width(48));
//                                                                s = new GUIStyle("textfield");
//                                                                s.alignment = TextAnchor.MiddleRight;
//                                                                long am = tasks.sleepMinutes;

//                                                                text = am / 1440 + "d " + am % 1440 / 60 + "h " + am % 60 + "m";
//                                                                if (am < 1440) text = am / 60 + "h " + am % 60 + "m";
//                                                                if (am < 60) text = am + "m";
//                                                                GUILayout.Label(text, s, GUILayout.Width(80));
//                                                                s = new GUIStyle("button");
//                                                                if (GUILayout.Button("+", s, GUILayout.Width(32), GUILayout.Height(20))) tasks.sleepMinutes += Core.timerManualMinutes;
//                                                                if (GUILayout.Button("-", s, GUILayout.Width(32), GUILayout.Height(20)))
//                                                                    if (tasks.sleepMinutes > 0) tasks.sleepMinutes -= Core.timerManualMinutes;

//                                                                s = new GUIStyle("textfield");
//                                                                s.alignment = TextAnchor.MiddleRight;
//                                                                Core.timerManualMinutes = EditorGUILayout.IntField(Core.timerManualMinutes, s, GUILayout.Width(32));
//                                                                GUILayout.Label("min");
//                                                                GUILayout.FlexibleSpace();
//                                                                s = new GUIStyle("button");
//                                                                if (GUILayout.Button("Reset", s, GUILayout.Width(64), GUILayout.Height(20)))
//                                                                {
//                                                                    if (EditorUtility.DisplayDialog("Reset Time Tracked?", "This will set Sleep Timer for '" + tasks.name + "' to zero. This cannot be undone.", "Reset", "Cancel"))
//                                                                    {
//                                                                        tasks.sleepMinutes = 0;
//                                                                    }
//                                                                }
//                                                            }
//                                                            GUILayout.EndHorizontal();

//                                                        }
//                                                    }
//                                                    GUILayout.EndVertical();
//                                                    GUI.enabled = true;
//                                                    #endregion
//                                                }

//                                                if (DrawTaskHeader("Delete", Core.tTrashCan, Core.tRed, true, 160, 18, new GUIContent("Remove Task")))
//                                                {
//                                                    if (EditorUtility.DisplayDialog("Delete Task?", "Are you sure you want to delete the task " + tasks.name + " in the Landmark called " + localData.title + "?  This cannot be undone!", "Delete", "Cancel"))
//                                                    {
//                                                        localData.tasks.RemoveAt(i);
//                                                        Core.SaveData();
//                                                    }
//                                                }
//                                                GUILayout.Space(16);
//                                            }

//                                            #region GUIChangedTasks
//                                            if (tasks.CheckChanged(prev))
//                                            {
//                                                localData.UpdateProgress(scene.stickyFont);
//                                                Core.SaveData();
//                                            }
//                                            #endregion

//                                        }
//                                    end:;
//                                    }
//                                    GUILayout.EndVertical();

//                                }
//                                GUILayout.EndScrollView();


//                                #region DrawTaskHeader
//                                s = StyleCheck();
//                                s.alignment = TextAnchor.LowerCenter;
//                                s.fontSize = 20;
//                                s.fontStyle = FontStyle.Bold;
//                                s.normal.background = headerColor;

//                                s.normal.textColor = headerColorText;

//                                GUI.Label(new Rect(0, headerRect.y + headerRect.height, screenWidth - 20, 20), headerText, s);

//                                #endregion
//                                break;
//                            case TA.LandmarkDetailState.delete:
//                                List<String> landmarkNames = new List<string>();
//                                for (int i = 0; i < scene.landmarks.Count; i++)
//                                {
//                                    landmarkNames.Add((i + 1) + ". " + scene.landmarks[i].title);
//                                }

//                                if (GUILayout.Button("Back"))
//                                {
//                                    scene.landmarkDetailState = TA.LandmarkDetailState.tasks;
//                                }

//                                scene.scrollPosLandmarkDetailDelete = GUILayout.BeginScrollView(scene.scrollPosLandmarkDetailDelete, GUIStyle.none, GUILayout.Height(screenHeight - 128 - 112));
//                                {
//                                    for (int i = 0; i < localData.tasks.Count; i++)
//                                    {
//                                        var tasks = localData.tasks[i];

//                                        s = StyleCheck();
//                                        s.fontSize = 24;
//                                        s.fontStyle = FontStyle.Bold;
//                                        s.alignment = TextAnchor.MiddleLeft;

//                                        s.normal.textColor = WhiteOrBlack(tasks.color);
//                                        s.normal.background = tasks.tColor;

//                                        GUILayout.BeginVertical();
//                                        {
//                                            GUILayout.Space(8);
//                                            GUILayout.BeginHorizontal(s, GUILayout.Height(32));
//                                            GUILayout.Space(4);
//                                            text = tasks.name;
//                                            if (text.Length > 25) text = text.Substring(0, 15);

//                                            text = (i + 1) + ". " + text;
//                                            GUILayout.BeginVertical();
//                                            {
//                                                GUILayout.Space(3);
//                                                GUILayout.BeginHorizontal();
//                                                GUILayout.Label(text, s);


//                                                GUILayout.FlexibleSpace();
//                                                if (GUILayout.Button(Core.tTrashCan, GUILayout.Width(24), GUILayout.Height(24)))
//                                                {
//                                                    if (EditorUtility.DisplayDialog("Delete Task?", "Are you sure you want to delete " + tasks.name + " inside the landmark named " + localData.title + "?  This cannot be undone!", "Delete", "Cancel"))
//                                                    {
//                                                        localData.tasks.RemoveAt(i);
//                                                    }
//                                                }
//                                                if (GUILayout.Button(Core.tArrowUp, GUILayout.Width(24), GUILayout.Height(24)))
//                                                {
//                                                    localData.tasks.Move(i, i - 1);
//                                                }

//                                                if (GUILayout.Button(Core.tArrowDown, GUILayout.Width(24), GUILayout.Height(24)))
//                                                {
//                                                    localData.tasks.Move(i, i + 1);

//                                                }
//                                                GUILayout.EndHorizontal();
//                                            }
//                                            GUILayout.EndVertical();

//                                            s.normal.background = tasks.tColorDark;


//                                            GUILayout.Space(8);
//                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(s, GUILayout.Height(32));
//                                            GUILayout.Space(4);
//                                            s = StyleCheck();
//                                            s.normal.background = Core.tTransparent;

//                                            s.normal.textColor = WhiteOrBlack(tasks.tColorDark.GetPixel(1, 1));

//                                            s.fontSize = 14;

//                                            GUILayout.BeginVertical();
//                                            {
//                                                GUILayout.Space(4);
//                                                GUILayout.BeginHorizontal();
//                                                GUILayout.Label("Choose Landmark:", s);

//                                                localData.landmarkMoveTo = EditorGUILayout.Popup(localData.landmarkMoveTo, landmarkNames.ToArray());

//                                                if (GUILayout.Button("Copy"))
//                                                {
//                                                    scene.landmarks[localData.landmarkMoveTo].tasks.Add(tasks.Copy());
//                                                }

//                                                if (GUILayout.Button("Move"))
//                                                {
//                                                    scene.landmarks[localData.landmarkMoveTo].tasks.Add(tasks.Copy());
//                                                    localData.tasks.RemoveAt(i);
//                                                }

//                                                GUILayout.Space(4);
//                                                GUILayout.EndHorizontal();
//                                                HorizontalLine(Color.black);
//                                                GUILayout.EndHorizontal();
//                                            }
//                                            GUILayout.EndVertical();
//                                        }
//                                        GUILayout.EndVertical();
//                                    }
//                                }
//                                GUILayout.EndScrollView();
//                                break;

//                        }
//                    }
//                    GUILayout.EndVertical();
//                    GUILayout.EndScrollView();
//                }
//                GUILayout.EndHorizontal();
//            }
//            GUILayout.EndArea();
//        }

//        static void DrawTagFilterMask()
//        {
//            #region TagFilterMask
//            GUILayout.Space(6);
//            sTemp = StyleCheck();
//            var method = typeof(EditorGUI).GetMethod("MaskFieldInternal", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null, new System.Type[] { typeof(Rect), typeof(GUIContent), typeof(int), typeof(string[]), typeof(int[]), typeof(GUIStyle) }, null);
//            RefreshTaglist();
//            int[] array = new int[tagList.Count];
//            for (int i = 0; i < array.Length; i++)
//            {
//                if (i == 0) { array[i] = 0; continue; }
//                if (i == array.Length - 1) { array[i] = -1; continue; }
//                array[i] = 1 << (i - 1);
//            }

//            sTemp.padding = new RectOffset(0, 0, 4, 0);
//            GUILayout.Label("Tag Filter:", sTemp);
//            GUILayout.Space(10);
//            GUILayout.Label("", GUILayout.Width(104));
//            Rect position = GUILayoutUtility.GetLastRect();
//            var prev = tagSelected;
//            tagSelected = (int)method.Invoke(null, new object[] { position, GUIContent.none, tagSelected, tagList.ToArray(), array, EditorStyles.popup });
//            if (prev != tagSelected)
//            {
//                tags = new List<string>();
//                for (int i = 0; i < Core.dataV2.tags.Count; i++)
//                {
//                    int tag = 1 << i;
//                    if ((tagSelected & tag) != 0)
//                    {
//                        tags.Add(Core.dataV2.tags[i].name);
//                        Core.dataV2.tags[i].active = true;
//                    }
//                    else
//                    {
//                        Core.dataV2.tags[i].active = false;
//                    }
//                }
//                Core.SaveData();
//            }
//            GUILayout.FlexibleSpace();
//            #endregion
//        }

//        static GUIStyle sTemp;
//        static Vector2 scrollLandmarkCarousel;
//        static void LoadViewLandmarkCarousel()
//        {
//            int bCount = 0;
//            sTemp = new GUIStyle();
//            scrollLandmarkCarousel = GUILayout.BeginScrollView(scrollLandmarkCarousel, GUILayout.Height(106));
//            GUILayout.BeginHorizontal();
//            {
//                GUILayout.FlexibleSpace();

//                for (int i = 0; i < scene.landmarks.Count; i++)
//                {

//                    #region CheckActiveTags
//                    bool noTag = true;
//                    if (tags != null)
//                        for (int t = 0; t < tags.Count; t++)
//                        {
//                            for (int r = 0; r < scene.landmarks[i].tags.Count; r++)
//                            {
//                                if (scene.landmarks[i].tags[r].name == tags[t])
//                                {
//                                    if (scene.landmarks[i].tags[r].active)
//                                    {
//                                        noTag = false;
//                                    }
//                                }
//                            }
//                        }
//                    if (tagSelected == 0 & noTag) goto skipcheck;
//                    if (noTag) continue;
//                    skipcheck:
//                    #endregion
//                    #region bLandmarkThumbnail
//                    bCount++;
//                    if (i == scene.selectedLandmark)
//                    {
//                        GUILayout.Box(Core.tWhite, GUIStyle.none, GUILayout.Width(86), GUILayout.Height(86));
//                        Rect r = GUILayoutUtility.GetLastRect();
//                        if (scene.landmarks[i].tGizmoColor == null)
//                        {
//                            // scene.landmarks[i].GizmoColor = Color.white;
//                            // scene.landmarks[i].SetColor(Color.white);
//                        }

//                        DrawTexture(r, scene.landmarks[i].tGizmoColor);

//                        if (GUI.Button(new Rect(r.x + 4, r.y + 4, r.width - 3, r.height - 9), new GUIContent(scene.landmarks[i].tScreenshot), GUIStyle.none))
//                        {
//                            ZoomToLocation(i);
//                        }
//                    }
//                    else
//                    {
//                        GUILayout.Box(Core.tWhite, GUIStyle.none, GUILayout.Width(86), GUILayout.Height(86));
//                        Rect r = GUILayoutUtility.GetLastRect();
//                        if (scene.landmarks[i].tGizmoColor == null) scene.landmarks[i].SetColor(scene.landmarks[i].GizmoColor);
//                        DrawTexture(new Rect(r.x + 2, r.y + 2, r.width - 5, r.height - 6), scene.landmarks[i].tGizmoColor);


//                        if (GUI.Button(new Rect(r.x + 4, r.y + 4, r.width - 3, r.height - 9), new GUIContent(scene.landmarks[i].tScreenshot), GUIStyle.none))
//                        {
//                            scene.selectedLandmark = i;
//                        }
//                    }
//                    Rect pos = GUILayoutUtility.GetLastRect();
//                    sTemp.fontSize = 8;
//                    sTemp.alignment = TextAnchor.MiddleCenter;
//                    sTemp.normal.textColor = WhiteOrBlack(scene.landmarks[i].GizmoColor);
//                    sTemp.normal.background = scene.landmarks[i].tGizmoColor50;
//                    string text = scene.landmarks[i].title;
//                    if (text.Length > 15) text = text.Substring(0, 15);
//                    GUI.Label(new Rect(pos.x + 6, pos.y + 6, pos.width - 12, 18f), text, sTemp);
//                    if (scene.landmarks[i].tasksHasAlert)
//                    {
//                        DrawAlert(pos.x, pos.y, 24, 24, Core.tIconExclaim);
//                    }
//                    ProgressBar(new Rect(pos.x + 8, pos.y + pos.height - 20, pos.width - 16, 8f), scene.landmarks[i].progress, Core.tGreyDark, Core.tGreenBright);
//                    #endregion
//                    GUILayout.Space(16);
//                }
//                GUILayout.Space(-16);
//                GUILayout.FlexibleSpace();
//            }
//            if (bCount == 0)
//            {
//                GUILayout.BeginVertical();
//                GUILayout.FlexibleSpace();
//                GUILayout.BeginHorizontal();
//                GUILayout.FlexibleSpace();
//                GUILayout.Label("No other Landmarks with tags selected found...");
//                GUILayout.FlexibleSpace();
//                GUILayout.EndHorizontal();
//                GUILayout.FlexibleSpace();
//                GUILayout.EndVertical();
//            }
//            GUILayout.EndHorizontal();
//            GUILayout.EndScrollView();
//        }

//        static void LoadViewLandmarkDetailGeneral(int landmark)
//        {
//            var landmarks = scene.landmarks[landmark];
//            scene.scrollPosLandmarkDetailGeneral = GUILayout.BeginScrollView(scene.scrollPosLandmarkDetailGeneral, GUIStyle.none, GUILayout.Height(screenHeight - 128 - 80));
//            {
//                GUILayout.BeginVertical();
//                {
//                    DrawTaskHeader("Details", Core.tEditPen, Core.tGreen);
//                    GUILayout.BeginHorizontal();
//                    {
//                        GUILayout.Label("Name:", GUILayout.Width(48));
//                        landmarks.title = GUILayout.TextField(landmarks.title, GUILayout.Width(128 + 48));
//                    }
//                    GUILayout.EndHorizontal();

//                    HorizontalLine(Color.black);

//                    GUILayout.Label("Description:");
//                    s = GetTextAreaStyle(landmarks.description, screenWidth - 26);
//                    landmarks.description = GUILayout.TextArea(landmarks.description, s, GUILayout.Width(screenWidth - 26), GUILayout.Height(s.fixedHeight));

//                    #region Tags
//                    DrawTaskHeader("Tags", Core.tIconTag, Core.tTeal);
//                    GUILayout.BeginHorizontal();
//                    {
//                        #region EditTagButtons
//                        switch (tagEditMode)
//                        {
//                            case TagEditMode.none:
//                                #region AddNewTag
//                                GUILayout.Label("Add New Tag:");
//                                if (screenWidth < 230) { GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(); }
//                                newTag = GUILayout.TextField(newTag, GUILayout.Width(96));
//                                if (GUILayout.Button(Core.tPlus, GUIStyle.none, GUILayout.Width(16)))
//                                {
//                                    Core.dataV2.tags.Add(new Tags(newTag));
//                                    Core.dataV2.tags = Core.dataV2.tags.Distinct().ToList();
//                                    Core.dataV2.tags = Core.dataV2.tags.OrderBy(x => x.name).ToList();
//                                    for (int i = 0; i < scene.landmarks.Count; i++)
//                                    {
//                                        scene.landmarks[i].tags.Add(new Tags(newTag));
//                                        scene.landmarks[i].tags = scene.landmarks[i].tags.Distinct().ToList();
//                                        scene.landmarks[i].tags = scene.landmarks[i].tags.OrderBy(x => x.name).ToList();
//                                    }
//                                    Core.SaveData();
//                                    newTag = "";
//                                }
//                                #endregion
//                                GUILayout.FlexibleSpace();
//                                #region EditTags
//                                if (GUILayout.Button(new GUIContent("  Edit"), GUILayout.Width(64), GUILayout.Height(18)))
//                                {
//                                    tagEditMode = TagEditMode.edit;
//                                    tagListCopy = new List<string>();
//                                    for (int i = 0; i < Core.dataV2.tags.Count; i++)
//                                    {
//                                        tagListCopy.Add(Core.dataV2.tags[i].name);
//                                    }
//                                }
//                                #endregion
//                                #region DeleteTags
//                                if (GUILayout.Button(new GUIContent("  Delete"), GUILayout.Width(64), GUILayout.Height(18))) tagEditMode = TagEditMode.delete;
//                                GUILayout.Space(16);
//                                #endregion
//                                break;
//                            case TagEditMode.edit:
//                                if (screenWidth >= 212)
//                                {
//                                    GUILayout.Space(screenWidth - 96 - 96 - 16);
//                                }

//                                #region SaveTags
//                                if (GUILayout.Button("Save Tags", GUILayout.Width(96)))
//                                {
//                                    tagEditMode = TagEditMode.none;
//                                    for (int i = 0; i < tagListCopy.Count; i++)
//                                    {
//                                        if (Core.dataV2.tags[i].name != tagListCopy[i])
//                                        {
//                                            Core.dataV2.tags[i].name = tagListCopy[i];
//                                        }
//                                    }
//                                    for (int i = 0; i < scene.landmarks.Count; i++)
//                                    {
//                                        for (int x = 0; x < scene.landmarks[i].tags.Count; x++)
//                                        {
//                                            scene.landmarks[i].tags[x].name = tagListCopy[x];
//                                        }
//                                        scene.landmarks[i].tags = scene.landmarks[i].tags.Distinct().ToList();
//                                        scene.landmarks[i].tags = scene.landmarks[i].tags.OrderBy(x => x.name).ToList();

//                                    }
//                                    for (int x = 0; x < Core.dataV2.tags.Count; x++)
//                                    {
//                                        Core.dataV2.tags[x].name = tagListCopy[x];
//                                    }
//                                    Core.dataV2.tags = Core.dataV2.tags.Distinct().ToList();
//                                    Core.dataV2.tags = Core.dataV2.tags.OrderBy(x => x.name).ToList();
//                                    Core.SaveData();
//                                }
//                                #endregion
//                                #region CancelEdit
//                                if (screenWidth < 210) { GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(); }
//                                if (GUILayout.Button("Cancel", GUILayout.Width(96)))
//                                {
//                                    tagEditMode = TagEditMode.none;
//                                }
//                                #endregion
//                                break;
//                            case TagEditMode.delete:
//                                if (screenWidth >= 212)
//                                {
//                                    GUILayout.Space(screenWidth - 128 - 16);
//                                }
//                                #region ExitDeleteMode
//                                if (GUILayout.Button("Exit Delete Mode", GUILayout.Width(128))) tagEditMode = TagEditMode.none;
//                                #endregion
//                                break;
//                        }
//                        #endregion
//                    }
//                    GUILayout.EndHorizontal();
//                    GUILayout.BeginHorizontal();
//                    {
//                        #region ShowTags


//                        switch (tagEditMode)
//                        {
//                            case TagEditMode.none:
//                                #region SelectionMode

//                                for (int i = 0; i < landmarks.tags.Count; i++)
//                                {
//                                    GUILayout.EndHorizontal(); GUILayout.Space(4); GUILayout.BeginHorizontal();
//                                    s = new GUIStyle("button");
//                                    Color prevColor = GUI.backgroundColor;
//                                    if (landmarks.tags[i].active)
//                                    {
//                                        GUI.backgroundColor = Core.cTeal;
//                                    }
//                                    landmarks.tags[i].active = GUILayout.Toggle(landmarks.tags[i].active, landmarks.tags[i].name, s);
//                                    GUI.backgroundColor = prevColor;
//                                }
//                                #endregion
//                                break;
//                            case TagEditMode.edit:
//                                #region EditMode
//                                for (int i = 0; i < tagListCopy.Count; i++)
//                                {
//                                    GUILayout.EndHorizontal(); GUILayout.Space(4); GUILayout.BeginHorizontal();
//                                    s = new GUIStyle("textfield");
//                                    s.alignment = TextAnchor.MiddleCenter;
//                                    tagListCopy[i] = GUILayout.TextField(tagListCopy[i], s);
//                                }
//                                #endregion
//                                break;
//                            case TagEditMode.delete:
//                                #region DeleteMode
//                                var p = GUI.backgroundColor;
//                                GUI.backgroundColor = Color.red;
//                                for (int i = 0; i < landmarks.tags.Count; i++)
//                                {
//                                    GUILayout.EndHorizontal(); GUILayout.Space(4); GUILayout.BeginHorizontal();
//                                    if (GUILayout.Button(landmarks.tags[i].name))
//                                    {
//                                        if (EditorUtility.DisplayDialog("Delete Tag?", "Are you sure you want to delete the tag " + landmarks.tags[i].name + "?  This cannot be undone and affects all landmarks.", "Delete Tag"))
//                                        {
//                                            for (int ii = 0; ii < scene.landmarks.Count; ii++)
//                                            {
//                                                scene.landmarks[ii].tags.RemoveAt(i);

//                                            }
//                                            Core.dataV2.tags.RemoveAt(i);
//                                            Core.dataV2.tags = Core.dataV2.tags.Distinct().ToList();
//                                            Core.dataV2.tags = Core.dataV2.tags.OrderBy(x => x.name).ToList();
//                                            Core.SaveData();
//                                            RefreshSettings();
//                                        }
//                                    };
//                                }
//                                GUI.backgroundColor = p;
//                                #endregion
//                                break;
//                        }
//                        #endregion
//                    }
//                    GUILayout.EndHorizontal();
//                    HorizontalLine(Color.black);
//                    text = "Hide Label in Scene";
//                    if (!scene.landmarks[scene.selectedLandmark].showGizmo) text = "Show Label in Scene";
//                    if (DrawTaskHeader("Landmark Label", Core.tIconPin, Core.tBlue, true, 180, 18, new GUIContent(text)))
//                    {
//                        if (scene.landmarks[scene.selectedLandmark].showGizmo)
//                        {
//                            scene.landmarks[scene.selectedLandmark].showGizmo = false;
//                        }
//                        else
//                        {
//                            scene.landmarks[scene.selectedLandmark].showGizmo = true;
//                        }
//                    }

//                    GUILayout.BeginHorizontal();
//                    {
//                        GUILayout.Label("Color");
//                        scene.landmarks[scene.selectedLandmark].SetColor(EditorGUILayout.ColorField(scene.landmarks[scene.selectedLandmark].GizmoColor, GUILayout.Width(48)));
//                        GUILayout.FlexibleSpace();
//                        if (scene.landmarks[scene.selectedLandmark].showGizmo)
//                        {
//                            s = new GUIStyle("button");
//                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
//                            scene.landmarks[scene.selectedLandmark].wsOffset = EditorGUILayout.Vector3Field("Label Offset: ", scene.landmarks[scene.selectedLandmark].wsOffset);
//                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
//                            scene.landmarks[scene.selectedLandmark].fadeGizmo = GUILayout.Toggle(scene.landmarks[scene.selectedLandmark].fadeGizmo, "Fade with Distance", s);
                            
//                            if (!scene.landmarks[scene.selectedLandmark].fadeGizmo) GUI.enabled = false;

//                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
//                            GUILayout.Label("Fade Distance:");
//                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
//                            var prev = scene.landmarks[scene.selectedLandmark].fadeStart + scene.landmarks[scene.selectedLandmark].fadeEnd;
//                            EditorGUILayout.MinMaxSlider(ref scene.landmarks[scene.selectedLandmark].fadeStart,
//                                                         ref scene.landmarks[scene.selectedLandmark].fadeEnd,
//                                                         0, 1000);
//                            if (prev != scene.landmarks[scene.selectedLandmark].fadeStart + scene.landmarks[scene.selectedLandmark].fadeEnd)
//                                Core.SaveData();
//                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
//                            s = new GUIStyle("textfield");
//                            GUILayout.Space(16);
//                            GUILayout.Label("Fade Start at:");
//                            GUILayout.Label(scene.landmarks[scene.selectedLandmark].fadeStart.ToString("F1"), s);
//                            GUILayout.FlexibleSpace();
//                            GUILayout.Label("Fade End at:");
//                            GUILayout.Label(scene.landmarks[scene.selectedLandmark].fadeEnd.ToString("F1"), s);
//                            GUILayout.Space(16);
//                            GUI.enabled = true;
//                        }
//                    }
//                    GUILayout.EndHorizontal();
//                    GUILayout.Space(16);
//                }
//                DrawTaskHeader("Task Defaults", Core.tIconTask, Core.tBlue);
//                EditorGUILayout.HelpBox("Only applies for 3D Stickies:", MessageType.Info);

//                GUILayout.Label("Fade Distance:");
//                GUILayout.BeginHorizontal();
//                {
//                    var prev = landmarks.taskFadeStart + landmarks.taskFadeEnd;
//                    EditorGUILayout.MinMaxSlider(ref landmarks.taskFadeStart, ref landmarks.taskFadeEnd, 0f, 1000f);
//                    if (prev != landmarks.taskFadeStart + landmarks.taskFadeEnd) Core.SaveData();
//                    GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
//                    s = new GUIStyle("textfield");
//                    GUILayout.Space(16);
//                    GUILayout.Label("Fade Start at:");
//                    GUILayout.Label(landmarks.taskFadeStart.ToString("F1"), s);
//                    GUILayout.FlexibleSpace();
//                    GUILayout.Label("Fade End at:");
//                    GUILayout.Label(landmarks.taskFadeEnd.ToString("F1"), s);
//                    GUILayout.Space(16);
//                }
//                GUILayout.EndHorizontal();
//                GUILayout.Space(16);

//                GUILayout.EndVertical();
//            }
//            GUILayout.EndScrollView();
//            #endregion
//        }




//        static public void AddCurrentViewToLandmarks()
//        {
//            if (Core.rtSS == null) Core.RefreshHelperCams();
//            Core.EnableLandmarkCamera();
//            if (Core.GetSceneView().in2DMode)
//            {
//                Core.LandmarkCamera.transform.position = new Vector3(Core.LandmarkCamera.transform.position.x, Core.LandmarkCamera.transform.position.y, -1000f);
//            }
//            else
//            {
//                Core.LandmarkCamera.transform.position = Core.GetSVCameraPosition();
//            }

//            Core.LandmarkCamera.transform.rotation = Core.GetSVCameraRotation();
//            Core.LandmarkCamera.GetComponent<Camera>().orthographicSize = Core.GetSVCOrthographicSize();



//            scene.landmarks.Add(
//                new Landmark(
//                    Core.rtSS,
//                    Core.LandmarkCamera.transform.position,
//                    Core.LandmarkCamera.transform.rotation,
//                    Core.GetSVCOrthographicSize())
//            );
//            scene.selectedLandmark = scene.landmarks.Count - 1;
//            scene.landmarks[scene.selectedLandmark].tags = new List<Tags>();
//            for (int i = 0; i < Core.dataV2.tags.Count; i++)
//            {
//                scene.landmarks[scene.selectedLandmark].tags.Add(new Tags(Core.dataV2.tags[i].name));
//            }
//            Core.DisableLandmarkCamera();
//            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
//        }

//        static void DeleteLandmark(int index)
//        {
//            scene.landmarks.RemoveAt(index);
//            scene.selectedLandmark = 0;
//            scene.landmarkDetailOpen = false;
//            Core.SaveData();
//        }

//        static void RefreshLandmarkTags()
//        {
//            Core.dataV2.tags = Core.dataV2.tags.Distinct().ToList();
//            Core.dataV2.tags = Core.dataV2.tags.OrderBy(x => x.name).ToList();
//            for (int i = 0; i < scene.landmarks.Count; i++)
//            {
//                var oldList = scene.landmarks[i].tags;
//                scene.landmarks[i].tags = new List<Tags>();

//                for (int y = 0; y < Core.dataV2.tags.Count; y++)
//                {
//                    scene.landmarks[i].tags.Add(new Tags(Core.dataV2.tags[y].name));
//                    for (int x = 0; x < oldList.Count; x++)
//                    {
//                        if (oldList[x].name == Core.dataV2.tags[y].name)
//                        {
//                            int lastIndex = scene.landmarks[i].tags.Count - 1;
//                            scene.landmarks[i].tags[lastIndex].active = oldList[x].active;
//                        }
//                        else
//                        {
//                        }
//                    }
//                }
//            }
//            Core.SaveData();
//        }

//        static public void RefreshLandmarkThumbnail(int index)
//        {
//            Core.EnableLandmarkCamera();

//            if (scene.landmarks[index].viewState == Landmark.ViewState.is2D)
//                Core.LandmarkCamera.GetComponent<Camera>().orthographic = true;
//            if (scene.landmarks[index].viewState == Landmark.ViewState.is3D)
//                Core.LandmarkCamera.GetComponent<Camera>().orthographic = false;

//            if (scene.landmarks[index].isFloating && scene.landmarks[index].floatingPositionObject != null)
//            {
//                Core.LandmarkCamera.transform.position = scene.landmarks[index].floatingPositionObject.transform.position;
//                Core.LandmarkCamera.transform.rotation = scene.landmarks[index].floatingPositionObject.transform.rotation;
//            }
//            else
//            {
//                Core.LandmarkCamera.transform.position = scene.landmarks[index].position;
//                Core.LandmarkCamera.transform.rotation = scene.landmarks[index].rotation;
//            }

//            scene.landmarks[index].RefreshThumbnail(Core.LandmarkCamera.GetComponent<Camera>(), scene.landmarks[index].orthographicSize, Core.LandmarkCamera.GetComponent<Camera>().orthographic);
//            Core.SaveData();
//            Core.DisableLandmarkCamera();
//        }
//        static void RefreshLandmarkPosition(int index)
//        {
//            scene.landmarks[index].isFloating = false;
//            scene.landmarks[index].position = Core.GetSVCameraPosition();
//            if (Core.GetSceneView().in2DMode) scene.landmarks[index].position = new Vector3(scene.landmarks[index].position.x, scene.landmarks[index].position.y, -1000f);
//            scene.landmarks[index].rotation = Core.GetSVCameraRotation();
//            scene.landmarks[index].orthographicSize = Core.GetSVCOrthographicSize();
//            RefreshLandmarkThumbnail(index);
//        }

//        static GUIStyle GetTextAreaStyle(string text, float w)
//        {
//            GUIStyle s = new GUIStyle(GUI.skin.textArea);

//            s.fixedHeight = 0;
//            s.fixedHeight = s.CalcHeight(new GUIContent(text), w);
//            s.wordWrap = true;
//            s.stretchWidth = false;

//            return s;
//        }

//        static GUIStyle StyleCheck()
//        {
//            GUIStyle style = new GUIStyle();
//            style.margin = style.padding = new RectOffset(0, 0, 0, 0);
//            if (EditorGUIUtility.isProSkin)
//            {
//                style.normal.textColor = Color.white;
//            }
//            return style;
//        }
//        static GUIStyle StyleCheck(GUIStyle gs)
//        {
//            GUIStyle style = new GUIStyle(gs);
//            if (EditorGUIUtility.isProSkin)
//            {
//                style.normal.textColor = Color.white;
//            }
//            return style;
//        }

//        static GUIStyle StyleBack(GUIStyle style, Texture2D texture)
//        {
//            if (style == null) style = new GUIStyle();
//            style.normal.background = texture;
//            return style;
//        }
//        static void ZoomToLocation(int i, int t = -1)
//        {
//            SceneView sv = Core.GetSceneView();
//            if (TaskAtlasSceneView.atlas.enabled)
//            {
//                if (scene.landmarks[i].viewState == Landmark.ViewState.is2D)
//                {
//                    sv.in2DMode = true;
//                }
//                else
//                {
//                    sv.in2DMode = false;
//                    if (scene.landmarks[i].viewState == Landmark.ViewState.isOrthographic)
//                    {
//                        sv.orthographic = true;
//                    }
//                    else
//                    {
//                        sv.orthographic = false;
//                    }
//                }

//                if (i == TaskAtlasSceneView.atlas.landmarkFocus && TaskAtlasSceneView.atlas.landmarkSelected == -1)
//                {
//                    TaskAtlasSceneView.atlas.landmarkSelected = i;
//                    TaskAtlasSceneView.atlas.taskFocus = 0;
//                    TaskAtlasSceneView.AtlasSpawnTaskPoints(TaskAtlasSceneView.atlas.landmarkSelected);
//                    TaskAtlasSceneView.AtlasTasksZoom(TaskAtlasSceneView.atlas.landmarkSelected, 2);
//                }
//                else
//                {
//                    TaskAtlasSceneView.atlas.landmarkSelected = -1;

//                    TaskAtlasSceneView.atlas.landmarkFocus = i;
//                    TaskAtlasSceneView.atlas.taskFocus = 0;
//                    TaskAtlasSceneView.AtlasSpawnLandmarkPoints();
//                    TaskAtlasSceneView.AtlasLandmarksZoom();
//                    TaskAtlasSceneView.StartAtlasMode(false);
//                }
//            }
//            else
//            {
//                Core.EnableLandmarkCamera();

//                if (t < 0)
//                {
//                    if (scene.landmarks[i].isFloating && scene.landmarks[i].floatingPositionObject != null)
//                    {
//                        Core.LandmarkCamera.transform.position = scene.landmarks[i].floatingPositionObject.transform.position;
//                        Core.LandmarkCamera.transform.rotation = scene.landmarks[i].floatingPositionObject.transform.rotation;
//                    }
//                    else
//                    {
//                        Core.LandmarkCamera.transform.position = scene.landmarks[i].position;
//                        Core.LandmarkCamera.transform.rotation = scene.landmarks[i].rotation;
//                    }
//                }
//                else
//                {
//                    Core.LandmarkCamera.transform.position = new Vector3(scene.landmarks[i].tasks[t].position.x,
//                                                                         scene.landmarks[i].tasks[t].position.y,
//                                                                         scene.landmarks[i].position.z);
//                }

//                //sv.AlignViewToObject(Core.LandmarkCamera.transform);
//                // sv.pivot = scene.landmarks[i].position;

//                if (scene.landmarks[i].viewState == Landmark.ViewState.is2D)
//                {
//                    sv.in2DMode = true;
//                    sv.orthographic = true;
//                    Core.LandmarkCamera.transform.rotation = Quaternion.identity;
//                }
//                else
//                {
//                    sv.in2DMode = false;
//                    if (scene.landmarks[i].viewState == Landmark.ViewState.isOrthographic)
//                    {
//                        sv.orthographic = true;
//                    }
//                    else
//                    {
//                        sv.orthographic = false;
//                    }
//                }
//                sv.AlignViewToObject(Core.LandmarkCamera.transform);
//                //sv.pivot = Core.LandmarkCamera.transform.position;// scene.landmarks[i].position + (scene.landmarks[i].position - sv.camera.transform.position);
//                sv.size = scene.landmarks[i].orthographicSize;

//                sv.Repaint();
//                sv.AlignViewToObject(Core.LandmarkCamera.transform);
//                //Core.DisableLandmarkCamera();
//            }
//        }

//        static bool DrawTaskHeader(string text, Texture2D icon, Texture2D color, bool hasButton = false, int bWidth = 0, int bHeight = 0, GUIContent bContent = null, GUIStyle bStyle = null)
//        {
//            bool ret = false;
//            GUILayout.Space(8);
//            GUIStyle s = StyleCheck();
//            s.alignment = TextAnchor.MiddleLeft;
//            s.fontSize = 14;
//            s.fontStyle = FontStyle.Bold;
//            sBackground = StyleBack(sBackground, color);
//            GUILayout.BeginHorizontal(sBackground, GUILayout.Height(48));
//            {
//                GUILayout.Space(4);
//                GUILayout.BeginVertical();
//                GUILayout.Space(10);
//                GUILayout.Box(icon, GUIStyle.none, GUILayout.Width(32), GUILayout.Height(32));
//                GUILayout.EndVertical();
//                GUILayout.BeginVertical();
//                GUILayout.Space(16);
//                GUILayout.BeginHorizontal();
//                GUILayout.Space(16);
//                GUILayout.Label(text, s, GUILayout.Width(128));
//                GUILayout.EndHorizontal();
//                GUILayout.EndVertical();
//                if (hasButton)
//                {
//                    GUILayout.BeginVertical();
//                    GUILayout.Space(16);
//                    if (bStyle == null)
//                    {
//                        if (GUILayout.Button(bContent, GUILayout.Width(bWidth), GUILayout.Height(bHeight)))
//                        {
//                            ret = true;
//                        }
//                    }
//                    else
//                    {
//                        if (GUILayout.Button(bContent, bStyle, GUILayout.Width(bWidth), GUILayout.Height(bHeight)))
//                        {
//                            ret = true;
//                        }
//                    }
//                    GUILayout.EndVertical();
//                }
//                else
//                {
//                    // GUILayout.FlexibleSpace();
//                }
//                GUILayout.Space(8);
//            }
//            GUILayout.EndHorizontal();
//            GUILayout.Space(8);
//            return ret;
//        }

//        static public void DrawAlert(float x, float y, float width, float height, Texture2D tex)
//        {
//            int xx = Mathf.FloorToInt(x + ((width - (width * lerp)) / 2));
//            int yy = Mathf.FloorToInt(y + ((height - (height * lerp)) / 2));
//            DrawTexture(new Rect(xx, yy, width * lerp, height * lerp), Core.tIconExclaim);
//        }

//        static void DrawBox(Rect position, Color color)
//        {
//            Color oldColor = GUI.color;

//            GUI.color = color;
//            GUI.Box(position, "");

//            GUI.color = oldColor;
//        }

//        static void ProgressBar(Rect rect, float progress, Texture2D background, Texture2D foreground)
//        {
//            if (background == null) return;
//            DrawTexture(rect, background);
//            DrawTexture(new Rect(rect.x, rect.y, rect.width * progress, rect.height), foreground);
//        }
//        static Color WhiteOrBlack(Color col)
//        {
//            float H, S, V;
//            Color.RGBToHSV(col, out H, out S, out V);
//            col = Color.white;
//            if (V > .5f)
//            {
//                if (V > .8f) col = Color.black;
//                if (S < .5f) col = Color.black;
//            }
//            return (col);
//        }

//        static private string SaveTexture(Texture2D texture)
//        {
//            byte[] bytes = texture.EncodeToPNG();
//            var dirPath = Core.taPath + "Screenshots/";
//            if (!System.IO.Directory.Exists(dirPath))
//            {
//                Debug.Log("Doesn't exist: " + dirPath);
//                System.IO.Directory.CreateDirectory(dirPath);
//            }
//            UnityEditor.AssetDatabase.Refresh();
//            dirPath = dirPath + "TaskAtlas_" + SceneManager.GetActiveScene().name + "_" + System.DateTime.Now + ".png";
//            dirPath = dirPath.Replace(":", "-");
//            System.IO.File.WriteAllBytes(dirPath, bytes);
//            Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + dirPath);
//#if UNITY_EDITOR
//            UnityEditor.AssetDatabase.Refresh();
//#endif
//            return dirPath;
//        }


//        static void HorizontalLine(Color color)
//        {
//            GUIStyle horizontalLine;
//            horizontalLine = new GUIStyle();
//            horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
//            horizontalLine.margin = new RectOffset(0, 0, 4, 4);
//            horizontalLine.fixedHeight = 1;
//            var c = GUI.color;
//            GUI.color = color;
//            GUILayout.Box(GUIContent.none, horizontalLine);
//            GUI.color = c;
//        }
//        static void DrawTexture(Rect r, Texture2D texture)
//        {
//            if (texture == null) return;
//            GUI.DrawTexture(r, texture);
//        }
    }
}