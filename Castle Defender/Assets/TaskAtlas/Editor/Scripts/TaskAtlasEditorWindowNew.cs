using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TaskAtlasNamespace
{
    [InitializeOnLoad]

    public class TaskAtlasEditorWindowNew : EditorWindow
    {
        public static EditorWindow window;

        static private ReorderableList ROLLandmarks;
        static List<string> landmarkNames;

        public static TA scene;

        static List<string> tagFilter;
        static List<string> tagList, tagListCopy;
        static int tagSelected;

        //static string newTag;

        static GUIStyle sBackgroundPanels, sBackground, sTaskHeader, s;

        static float thumbnailSize = 128 + 32;
        static string text;
        static int panelHeight = 128;
        static float screenWidth, screenHeight, screenWidthMargin;

        static float lerp = 1.0f, lerpMin = 1f, lerpMax = 1.5f, lerpTarget = 1.2f;

        static float localWidth;

        static int queueRepaint = 0;
        public static bool sceneFound = false;
        static GameObject go;

        public static TextAsset csvFile; // Reference of CSV file

        [MenuItem("Window/ShrinkRay Entertainment/TaskAtlas/Landmark Editor")]
        private static void OpenLandmarksWindow()
        {
            window = GetWindow<TaskAtlasEditorWindowNew>();

            window.minSize = new Vector2(400, 600);
            // window.maxSize = new Vector2(640, 4000);
            window.Show();
            window.titleContent.text = "Task Atlas";

            //if (File.Exists(Core.taPath + "Data/TaskAtlasDataDemo.asset")) Core.isData = true;
            //if (File.Exists(Core.taPath.Replace("TaskAtlas", "") + "TaskAtlasBuildOn")) Core.isBuild = true;
            //Core.isInstalled = true;
            //Debug.Log(Core.taPath);

            go = GameObject.Find("TaskAtlas");
            //Debug.Log(go);
            if (go != null)
            {
                sceneFound = true;
                Core.Init();
                Init();
                RefreshSettings();
            }
            else
            {
                sceneFound = false;
            }
        }

        [MenuItem("Window/ShrinkRay Entertainment/Discord Server")]
        private static void Discord()
        {
            Application.OpenURL("https://discord.gg/W8MKrRH");
        }

        [MenuItem("Window/ShrinkRay Entertainment/Email Us")]
        private static void email()
        {
            Application.OpenURL("mailto:shrinkrayentertainment@gmail.com?subject=Scene%20Pilot%20Question&body=Questions?%20Comments?%20Issues?");
        }

        [MenuItem("Window/ShrinkRay Entertainment/TaskAtlas/Rate TaskAtlas")]
        private static void Rate()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/task-atlas-ultimate-task-manager-sticky-notes-bookmarking-refere-185959#reviews");
        }

        [MenuItem("Window/ShrinkRay Entertainment/Get all ShrinkRay Assets at a HUGE discount")]
        private static void MoreAssets()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/better-editor-deluxe-192015?aid=1011lf9gY&pubref=taskatlas");
        }

        public static void QueueRepaint()
        {
            if (window == null) window = GetWindow<TaskAtlasEditorWindowNew>();
            window.minSize = new Vector2(380, 600);
            window.maxSize = new Vector2(600, 196);
            window.titleContent.text = "Task Atlas";
            queueRepaint = 1;
        }

        public void OnInspectorUpdate()
        {
            if (queueRepaint > 0 && queueRepaint < 10)
            {
                Repaint();
            }
        }

        private void OnFocus()
        {

            go = GameObject.Find("TaskAtlas");

            if (go != null)
            {
                sceneFound = true;
                Core.Init();
                Init();
                RefreshSettings();
            }
            else
            {
                sceneFound = false;
            }
        }

        static public void RefreshSettings()
        {

            RefreshTaglist();
        }

        static void RefreshTaglist()
        {
            if (Core.dataV2 == null || Core.dataV2.tags == null) return; //Core.Init();
            if (tagFilter == null) tagFilter = new List<string>();
            if (tagList == null) { tagList = new List<string>(); } else { tagList.Clear(); }
            tagList.Add("Show All");
            for (int i = 0; i < Core.dataV2.tags.Count; i++)
            {
                tagList.Add(Core.dataV2.tags[i].name);
            }
            tagList.Add("Has Any Tag");
        }


        public static bool isInit = false;
        static public void Init()
        {
            if (Core.isBuild) return;
            if (!Core.isActive) return;

            Core.RefreshIcons();
            Core.RefreshHelperCams();

            sBackgroundPanels = new GUIStyle();
            sBackgroundPanels.normal.background = Core.tBackgroundMain;

            scene = Core.dataV2.scene[Core.dataV2.sceneIndex];
            RefreshSettings();
            scene.UpdateAllProgress(scene.stickyFont);
            scene.UpdateAllThumbnails();

            //for (int i = 0; i < scene.history.Count; i++)
            //{
            //    var d = scene.history[i];
            //    d.GetThumbnail();
            //}
            for (int i = 0; i < scene.landmarks.Count; i++)
            {
                var d = scene.landmarks[i];
                d.GetThumbnail();

                for (int iii = 0; iii < scene.landmarks[i].gallery.images.Count; iii++)
                {
                    //if (scene.landmarks[i].gallery.images[iii].image == null)
                    if (scene.landmarks[i].gallery.images[iii].imagePath != "")
                    {
                        //Debug.Log(iii + ": " + scene.landmarks[i].gallery.images[iii].imagePath);
                        scene.landmarks[i].gallery.images[iii].image = (Texture2D)AssetDatabase.LoadAssetAtPath(scene.landmarks[i].gallery.images[iii].imagePath, typeof(Texture2D));
                        //Debug.Log(iii + ": " + scene.landmarks[i].gallery.images[iii].image);
                    }
                }

            }
            for (int x = 0; x < scene.landmarks.Count; x++)
            {
                for (int y = 0; y < scene.landmarks[x].tasks.Count; y++)
                {
                    var d = scene.landmarks[x].tasks[y];
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
            isInit = true;
        }

        static Vector2 scrollviewNoData;

        void OnGUI()
        {
            lerp = Mathf.Lerp(lerp, lerpTarget, 0.05f);
            if (lerp >= lerpTarget - 0.1f) lerpTarget = lerpMin;
            if (lerp <= lerpTarget + 0.1f) lerpTarget = lerpMax;


            if (window == null)
            {
                screenWidth = 0;
                screenHeight = 0;
                QueueRepaint();
            }
            else
            {
                screenWidth = window.position.width;
                screenHeight = window.position.height;
            }

            if (screenWidth > 600)
            {
                screenWidthMargin = (screenWidth - 600) / 2;
                screenWidth = 600;
            }
            else
            {
                screenWidthMargin = 0;
            }

            if (!sceneFound)
            {
                //if (Core.dataV2 == null) Debug.Log($"Core.dataV2==null");
                sceneFound = false;

                GUILayout.BeginArea(new Rect(0, 0, screenWidth, screenHeight));
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("Task Atlas Scene Object not found, would you like to put your scene data in this scene?");
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("If you would like to use Task Atlas here, choose which scene to store the data on:");
                        GUILayout.FlexibleSpace();

                        //scrollviewNoData = GUILayout.BeginScrollView(scrollviewNoData, GUILayout.Width(screenWidth), GUILayout.Height(200));
                        {
                            //Debug.Log($"Scenes: {SceneManager.sceneCount}");
                            for (int i = 0; i < SceneManager.sceneCount; i++)
                            {
                                GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                GUILayout.FlexibleSpace();
                                var sn = SceneManager.GetSceneAt(i).name;
                                if (sn == "") sn = $"[Unnamed Scene]";
                                if (GUILayout.Button(sn))
                                {
                                    SceneManager.SetActiveScene(SceneManager.GetSceneAt(i));

                                    GameObject go = GameObject.Find("TaskAtlas");
                                    if (go == null) go = new GameObject("TaskAtlas");

                                    TaskAtlasDataV2 dataV2 = new TaskAtlasDataV2();
                                    if (!go.TryGetComponent<TaskAtlasDataV2>(out dataV2))
                                    {
                                        go.AddComponent<TaskAtlasDataV2>();
                                        dataV2 = go.GetComponent<TaskAtlasDataV2>();
                                    }
                                    sceneFound = true;
                                    Core.Init();
                                    Init();
                                    RefreshSettings();
                                }

                                GUILayout.FlexibleSpace();
                            }
                        }
                        // GUILayout.EndScrollView();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();

                queueRepaint++;
                return;
            }

            //Debug.Log($"isbuild: {Core.isBuild}  isInstalled: {Core.isInstalled}  isInit: {isInit}");

            if (Core.isBuild || !Core.isInstalled) return;

            if (!isInit)
            {
                //Debug.Log("Initalizing");
                Core.Init();
                Init();
            }

            #region Landmarks
            if (Core.TaskAtlasRoot == null) return;
            if (!scene.landmarkDetailOpen)
            {
                LoadViewLandmarks();
            }
            else
            {
                LoadViewLandmarkDetail();
            }

            #endregion
            queueRepaint++;
        }

        static void LoadViewLandmarkSort()
        {
            #region LoadViewLandmarkSort
            GUIStyle s = StyleCheck();
            sBackgroundPanels.normal.background = Core.tBackgroundMain;
            GUILayout.BeginArea(new Rect(0, 8, screenWidth, panelHeight));
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    #region bAddToLandmarks
                    if (GUILayout.Button(Core.tAdd, GUILayout.Width(48), GUILayout.Height(48)))
                    {
                        AddCurrentViewToLandmarks();
                    }
                    #endregion
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                HorizontalLine(Color.black);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    s.fontSize = 8;
                    #region bSortDist
                    if (GUILayout.Button("Sort Mode", GUILayout.Width(24), GUILayout.Height(24)))
                    {
                        scene.sortMode = !scene.sortMode;
                    }
                    #endregion
                    #region bSortDist
                    if (GUILayout.Button(Core.tSortDist, GUILayout.Width(24), GUILayout.Height(24)))
                    {
                        for (int i = 0; i < scene.landmarks.Count; i++)
                        {
                            scene.landmarks[i].currentDistance = Vector3.Distance(Core.GetSVCameraPosition(), scene.landmarks[i].position);
                        }
                        scene.landmarks = scene.landmarks.OrderByDescending(x => x.currentDistance).ToList();
                    }
                    #endregion
                    #region bSortAZ
                    if (GUILayout.Button(Core.tSortAZ, GUILayout.Width(24), GUILayout.Height(24)))
                    {
                        scene.landmarks = scene.landmarks.OrderBy(x => x.title).ToList();
                    }
                    #endregion
                    #region bSortDate
                    if (GUILayout.Button(Core.tSortDate, GUILayout.Width(24), GUILayout.Height(24)))
                    {
                        scene.landmarks = scene.landmarks.OrderByDescending(x => x.timeStamp).ToList();
                    }
                    #endregion
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                HorizontalLine(Color.black);
            }
            GUILayout.EndArea();

            thumbnailSize = (screenWidth / 2);

            Rect parentRect = new Rect(0, panelHeight, screenWidth, screenHeight - panelHeight - 24);

            GUILayout.BeginArea(parentRect);
            {
                scene.scrollPosLandmarkSort = GUILayout.BeginScrollView(scene.scrollPosLandmarkSort, GUILayout.Width(screenWidth - 4));
                GUILayout.BeginVertical();
                {

                }
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();
            #endregion

        }

        static void LoadViewLandmarks()
        {
            #region LoadViewLandmarks
            GUIStyle s = StyleCheck(), s2 = StyleCheck();
            sBackgroundPanels.normal.background = Core.tBackgroundMain;
            int offset = 0;
            if (screenWidth < 400) offset = 24;

            var extra = 0;
            if (scene.sceneSettings) extra = 100;
            GUILayout.BeginArea(new Rect(screenWidthMargin, 8, screenWidth, panelHeight + offset + extra));
            {
                //if (DrawTaskHeader("Test", Core.tAdd, Core.tGreen, true, 130, 24, new GUIContent("Test")))
                //{
                //    readData();

                //    Debug.Log("Headers: " + csvLandmarkImportWithTasks.ncol());
                //    for (int r = 0; r < csvLandmarkImportWithTasks.nrow(); r++)
                //    {
                //        Debug.Log("ROW " + r + "/" + csvLandmarkImportWithTasks.nrow());
                //        String[] content = csvLandmarkImportWithTasks.row[r].ToArray();
                //        string landmarkName = "";
                //        string taskName = "";
                //        string taskText = "";
                //        int priority = 0;
                //        int stage = 0;
                //        float lm_x = 0, lm_y = 0, lm_z = 0, t_x = 0, t_y = 0, t_z = 0;
                //        int sticky = 0;
                //        for (int h = 0; h < content.Length; h++)
                //        {
                //            Debug.Log("Header: " + csvLandmarkImportWithTasks.headers[h]);
                //            Debug.Log(" | Content: " + content[h]);

                //            string header = csvLandmarkImportWithTasks.headers[h];
                //            Debug.LogError(header);
                //            switch (header)
                //            {
                //                case "LandmarkName":
                //                    landmarkName = content[h];
                //                    Debug.LogError("Landmark Name is " + landmarkName);
                //                    break;
                //                case "TaskName":
                //                    taskName = content[h];
                //                    break;
                //                case "TaskText":
                //                    taskText = content[h];
                //                    break;
                //                case "Stage":
                //                    int.TryParse(content[h], out stage);
                //                    break;
                //                case "Priority":
                //                    int.TryParse(content[h], out priority);
                //                    break;
                //                case "Sticky":
                //                    int.TryParse(content[h], out sticky);
                //                    break;
                //                case "lm_x":
                //                    float.TryParse(content[h], out lm_x);
                //                    break;
                //                case "lm_y":
                //                    float.TryParse(content[h], out lm_y);
                //                    break;
                //                case "lm_z":
                //                    float.TryParse(content[h], out lm_z);
                //                    break;
                //                case "t_x":
                //                    float.TryParse(content[h], out t_x);
                //                    break;
                //                case "t_y":
                //                    float.TryParse(content[h], out t_y);
                //                    break;
                //                case "t_z":
                //                    float.TryParse(content[h], out t_z);
                //                    break;

                //            }
                //        }

                //        Debug.Log("Adding Data...");

                //        bool foundLandmark = false, foundTask = false;
                //        int indexLandmark = 0, indexTask = 0;
                //        for (int i = 0; i < scene.landmarks.Count; i++)
                //        {
                //            Debug.LogWarning("scene.landmarks[i].title == landmarkName || " + scene.landmarks[i].title + "==" + landmarkName);
                //            if (scene.landmarks[i].title == landmarkName) { foundLandmark = true; indexLandmark = i; }

                //        }

                //        if (foundLandmark)
                //        {
                //            for (int i = 0; i < scene.landmarks[indexLandmark].tasks.Count; i++)
                //            {
                //                if (scene.landmarks[indexLandmark].tasks[i].name == taskName) { foundTask = true; indexTask = i; }
                //            }
                //        }
                //        else
                //        {
                //            indexLandmark = ImportLandmark(new Vector3(lm_x, lm_y, lm_z), Quaternion.identity);

                //            Landmark l = scene.landmarks[indexLandmark];
                //            l.title = landmarkName;
                //        }


                //        if (!foundTask)
                //        {
                //            Landmark l = scene.landmarks[indexLandmark];
                //            l.tasks.Add(new Task(taskName));
                //            indexTask = l.tasks.Count - 1;
                //        }

                //        Task t = scene.landmarks[indexLandmark].tasks[indexTask];
                //        t.description = taskText;
                //        if (sticky == 0) t.isSticky = false; else t.isSticky = true;
                //        t.position = new Vector3(t_x, t_y, t_z);
                //        t.stage = (Task.Stage)stage;
                //        t.priority = (Task.Priority)priority;


                //        csvLandmarkImportWithTasks.showData = true;
                //    }
                //}


                //csvFile = (TextAsset)EditorGUILayout.ObjectField(csvFile, typeof(TextAsset), false);

                //if (csvLandmarkImportWithTasks.showData)
                //{
                //    if (csvLandmarkImportWithTasks.row.Count == 0) return;

                //    for (int r = 0; r < csvLandmarkImportWithTasks.row.Count; r++)
                //    {

                //    }
                //}


                if (DrawTaskHeader("Add Landmark Here", Core.tAdd, Core.tGreen, true, 130, 24, new GUIContent("New Landmark")))
                {
                    AddCurrentViewToLandmarks();
                }
                HorizontalLine(Color.black);

                #region SortBy
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Sort By:");

                    s = StyleCheck();
                    s.fontSize = 8;
                    if (screenWidth > 470)
                    {
                        #region Dist
                        if (GUILayout.Button(new GUIContent("    Distance", Core.tSortDist), GUILayout.Height(18), GUILayout.Width(96)))
                        {
                            for (int i = 0; i < scene.landmarks.Count; i++)
                            {
                                scene.landmarks[i].currentDistance = Vector3.Distance(Core.GetSVCameraPosition(), scene.landmarks[i].position);
                            }
                            scene.landmarks = scene.landmarks.OrderBy(x => x.currentDistance).ToList();
                        }
                        #endregion
                        #region AZ
                        if (GUILayout.Button(new GUIContent("    ABC", Core.tSortAZ), GUILayout.Height(18), GUILayout.Width(96)))
                        {
                            scene.landmarks = scene.landmarks.OrderBy(x => x.title).ToList();
                        }
                        #endregion
                        #region tDate
                        if (GUILayout.Button(new GUIContent("    Created", Core.tSortDate), GUILayout.Height(18), GUILayout.Width(96)))
                        {
                            scene.landmarks = scene.landmarks.OrderByDescending(x => x.timeStamp).ToList();
                        }
                        #endregion
                        #region Manual
                        GUILayout.FlexibleSpace();
                        s = new GUIStyle("button");

                        scene.sortMode = GUILayout.Toggle(scene.sortMode, "Manual Sort", s, GUILayout.Height(18), GUILayout.Width(112));

                        #endregion
                        GUILayout.Space(16);
                    }
                    else
                    {
                        #region Dist
                        if (GUILayout.Button(new GUIContent(Core.tSortDist), GUILayout.Height(18), GUILayout.Width(32)))
                        {
                            for (int i = 0; i < scene.landmarks.Count; i++)
                            {
                                scene.landmarks[i].currentDistance = Vector3.Distance(Core.GetSVCameraPosition(), scene.landmarks[i].position);
                            }
                            scene.landmarks = scene.landmarks.OrderBy(x => x.currentDistance).ToList();
                        }
                        #endregion
                        #region AZ
                        if (GUILayout.Button(new GUIContent(Core.tSortAZ), GUILayout.Height(18), GUILayout.Width(32)))
                        {
                            scene.landmarks = scene.landmarks.OrderBy(x => x.title).ToList();
                        }
                        #endregion
                        #region tDate
                        if (GUILayout.Button(new GUIContent(Core.tSortDate), GUILayout.Height(18), GUILayout.Width(32)))
                        {
                            scene.landmarks = scene.landmarks.OrderByDescending(x => x.timeStamp).ToList();
                        }
                        #endregion
                        #region Manual
                        GUILayout.FlexibleSpace();
                        s = new GUIStyle("button");

                        scene.sortMode = GUILayout.Toggle(scene.sortMode, "Manual Sort", s, GUILayout.Height(18), GUILayout.Width(112));

                        #endregion
                        GUILayout.Space(16);
                    }
                }
                GUILayout.EndHorizontal();
                #endregion

                HorizontalLine(Color.black);
                GUILayout.BeginHorizontal();
                {
                    DrawTagFilterMask();
                    GUILayout.FlexibleSpace();
                    string text = "Show Scene Settings";
                    if (scene.sceneSettings == true) text = "Hide Scene Settings";

                    if (GUILayout.Button(text))
                    {
                        scene.sceneSettings = !scene.sceneSettings;
                    }


                }
                GUILayout.EndHorizontal();

                if (scene.sceneSettings)
                {
                    GUILayout.BeginHorizontal();
                    {
                        HorizontalLine(Color.black);
                        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                        GUILayout.Label("Show in SceneView:", GUILayout.Width(120));
                        s = new GUIStyle("toggle");
                        if (screenWidth < 400) { s.fontSize = 8; }
                        scene.showLandmarkLabels = GUILayout.Toggle(scene.showLandmarkLabels, "Labels", s, GUILayout.Height(18));
                        scene.showTaskGizmos = GUILayout.Toggle(scene.showTaskGizmos, "Gizmos", s, GUILayout.Height(18));
                        scene.showStickies = GUILayout.Toggle(scene.showStickies, "Sticky", s, GUILayout.Height(18));
                        GUILayout.FlexibleSpace();

                        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                        HorizontalLine(Color.black);
                        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();

                        GUILayout.Label("Sticky Notes:", GUILayout.Width(120));
                        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                        GUILayout.Label("Background:", GUILayout.Width(120));
                        if (GUILayout.Button("Note")) { scene.stickyBackground = StickyBackground.note; scene.UpdateAllProgress(scene.stickyFont, true); }
                        if (GUILayout.Button("Modern")) { scene.stickyBackground = StickyBackground.modern; scene.UpdateAllProgress(scene.stickyFont, true); }
                        GUILayout.FlexibleSpace();


                        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                        GUILayout.Label("Sticky Font:", GUILayout.Width(120));

                        //if (GUILayout.Button("Normal")) { scene.stickyFont = StickyFont.normal; scene.UpdateAllProgress(scene.stickyFont); }
                        //if (GUILayout.Button("RPG")) { scene.stickyFont = StickyFont.rpg; scene.UpdateAllProgress(scene.stickyFont); }
                        //if (GUILayout.Button("SciFi")) { scene.stickyFont = StickyFont.scifi; scene.UpdateAllProgress(scene.stickyFont); }
                        //if (GUILayout.Button("Western")) { scene.stickyFont = StickyFont.western; scene.UpdateAllProgress(scene.stickyFont); }
                        //if (GUILayout.Button("Toon")) { scene.stickyFont = StickyFont.toon; scene.UpdateAllProgress(scene.stickyFont); }
                        //

                        var prev = scene.stickyFont;
                        scene.stickyFont = (TMP_FontAsset)EditorGUILayout.ObjectField(scene.stickyFont, typeof(TMP_FontAsset), false);
                        if (prev != scene.stickyFont) scene.UpdateAllProgress(scene.stickyFont, true);

                        GUILayout.FlexibleSpace();
                        HorizontalLine(Color.black);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndArea();

            thumbnailSize = (screenWidth / 2);

            Rect parentRect = new Rect(screenWidthMargin, panelHeight + offset + extra, screenWidth, screenHeight - panelHeight - 12 - offset - extra);

            GUILayout.BeginArea(parentRect);
            {
                scene.scrollPosLandmark = GUILayout.BeginScrollView(scene.scrollPosLandmark, GUILayout.Width(screenWidth));
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Space(2);

                        for (int i = 0, ii = 1; i < scene.landmarks.Count; i++, ii++)
                        {
                            var landmarks = scene.landmarks;
                            if (landmarks[i].tGizmoColor == null) scene.landmarks[i].SetColor(landmarks[i].GizmoColor);
                            ////Debug.Log("gallery.images.Count: " + landmarks[i].gallery.images[landmarks[i].gallery.currentImage].image +" || "+landmarks[i].gallery.images.Count);
                            //
                            if (!scene.sortMode)
                            {
                                #region CheckActiveTags
                                bool noTag = true;
                                if (tagFilter != null)
                                    for (int t = 0; t < Core.dataV2.tags.Count; t++)
                                    {
                                        for (int tt = 0; tt < tagFilter.Count; tt++)
                                        {
                                            if (Core.dataV2.tags[t].name == tagFilter[tt])
                                                if (Core.dataV2.tags[t].GetActive(landmarks[i].timeStamp)) noTag = false;
                                        }

                                        //for (int r = 0; r < scene.landmarks[i].tags.Count; r++)
                                        //{
                                        //    if (scene.landmarks[i].tags[r].name == tags[t])
                                        //    {
                                        //        if (scene.landmarks[i].tags[r].active)
                                        //        {
                                        //            noTag = false;
                                        //        }
                                        //    }
                                        //}
                                    }
                                if (tagSelected == 0 & noTag) goto skipcheck;
                                if (noTag) continue;
                                skipcheck:
                                #endregion

                                if (i < scene.landmarks.Count - 1) HorizontalLine(Color.black);
                                if (!LoadViewLandmarkButtons(i))
                                {
                                    //Debug.Log("gallery.images.Count: " + landmarks[i].gallery.images.Count);

                                    continue;
                                }

                                GUILayout.Space(16);


                                GUILayout.Space(16);
                                //text = landmarks[i].description;
                                //s = new GUIStyle();
                                //s = GetTextAreaStyle(text, screenWidth - 28);
                                //GUILayout.Label(text, s, GUILayout.Width(screenWidth - 28), GUILayout.Height(s.fixedHeight));
                                //GUILayout.Space(16);

                                //s = StyleCheck();
                                //GUILayout.BeginHorizontal();
                                //{
                                //    GUILayout.Label("Open Tasks:  " + landmarks[i].tasksOpen.ToString(), s);
                                //    GUILayout.FlexibleSpace();
                                //    if (landmarks[i].tasksUrgent > 0)
                                //    {
                                //        s.normal.textColor = Color.red;
                                //    }
                                //    GUILayout.Label("Urgent:  " + landmarks[i].tasksUrgent.ToString(), s);
                                //    GUILayout.FlexibleSpace();
                                //    if (landmarks[i].tasksOverdue > 0)
                                //    {
                                //        s.normal.textColor = Color.red;
                                //    }
                                //    GUILayout.Label("Past Due:  " + landmarks[i].tasksOverdue.ToString(), s);
                                //    GUILayout.FlexibleSpace();
                                //    if (landmarks[i].tasksOverdue > 0)
                                //    {
                                //        s.normal.textColor = Color.red;
                                //    }
                                //    GUILayout.Space(9);
                                //}
                                //GUILayout.EndHorizontal();
                                //GUILayout.Space(16);

                                //int activeCount = 0;
                                //for (int t = 0; t < landmarks[i].tags.Count; t++)
                                //    if (landmarks[i].tags[t].active) activeCount++;

                                //if (activeCount > 0)
                                //{
                                //    GUILayout.BeginHorizontal();
                                //    {
                                //        DrawTaskHeader("Tags", Core.tIconTag, Core.tTeal);

                                //        GUILayout.EndHorizontal();
                                //        s2 = StyleCheck();
                                //        s2.normal.background = Core.tTeal;
                                //        s2.margin = new RectOffset(9, 9, 0, 0);
                                //        GUILayout.BeginHorizontal(s2);

                                //        for (int t = 0; t < landmarks[i].tags.Count; t++)
                                //        {

                                //            if (landmarks[i].tags[t].active)
                                //            {
                                //                GUILayout.EndHorizontal();
                                //                GUILayout.BeginHorizontal(s2);
                                //                s = StyleCheck("button");
                                //                GUILayout.Label(landmarks[i].tags[t].name, s);
                                //            }
                                //        }
                                //    }
                                //    GUILayout.EndHorizontal();
                                //    GUILayout.Space(24);
                                //}
                            }
                            else
                            {
                                //Debug.Log("gallery.images.Count: " + landmarks[i].gallery.images[landmarks[i].gallery.currentImage].image + " || " + landmarks[i].gallery.images.Count);

                                s = StyleCheck();
                                s.fontSize = 20;
                                s.fontStyle = FontStyle.Bold;
                                s.alignment = TextAnchor.MiddleLeft;

                                s.normal.textColor = WhiteOrBlack(landmarks[i].GizmoColor);
                                s.normal.background = landmarks[i].tGizmoColor;
                                GUILayout.BeginVertical(GUILayout.Height(32));
                                {
                                    //Debug.Log("gallery.images.Count: " + landmarks[i].gallery.images[landmarks[i].gallery.currentImage].image + " || " + landmarks[i].gallery.images.Count);

                                    GUILayout.BeginHorizontal(s);
                                    GUILayout.Space(8);
                                    text = landmarks[i].title;
                                    if (text.Length > 25) text = text.Substring(0, 15);

                                    text = (i + 1) + ". " + text;

                                    GUILayout.BeginVertical(); GUILayout.FlexibleSpace();
                                    GUILayout.Label(text, s);
                                    GUILayout.FlexibleSpace(); GUILayout.EndVertical();

                                    GUILayout.FlexibleSpace();

                                    GUILayout.BeginVertical(); GUILayout.FlexibleSpace();
                                    if (GUILayout.Button(Core.tArrowUp, GUILayout.Height(24), GUILayout.Width(24)))
                                    {
                                        landmarks.Move(i, i - 1);
                                    }
                                    GUILayout.FlexibleSpace(); GUILayout.EndVertical();

                                    GUILayout.BeginVertical(); GUILayout.FlexibleSpace();
                                    if (GUILayout.Button(Core.tArrowDown, GUILayout.Height(24), GUILayout.Width(24)))
                                    {
                                        landmarks.Move(i, i + 1);

                                    }
                                    GUILayout.FlexibleSpace(); GUILayout.EndVertical();

                                    GUILayout.Space(8);

                                    GUILayout.EndHorizontal();
                                    HorizontalLine(Color.black);
                                    //Debug.Log("gallery.images.Count: " + landmarks[i].gallery.images[landmarks[i].gallery.currentImage].image + " || " + landmarks[i].gallery.images.Count);

                                }
                                GUILayout.EndVertical();
                            }
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();
            ////Debug.Log("gallery.images.Count: " + gallery.images[gallery.currentImage].image +" || "+landmarks[i].gallery.images.Count);
            //
            #endregion
        }

        static bool LoadViewLandmarkButtons(int i)
        {
            GUIStyle s = StyleCheck();
            bool exists = true;
            var landmarks = scene.landmarks[i];



            GUILayout.BeginHorizontal();
            {
                //GUILayout.BeginVertical();
                {



                    s.normal.textColor = WhiteOrBlack(landmarks.GizmoColor);
                    s.normal.background = landmarks.tGizmoColor;
                    GUILayout.BeginHorizontal(s);
                    s = StyleCheck("button");
                    //s.margin = new RectOffset(0, 0, 0, 0);
                    //s.padding = new RectOffset(0, 0, 0, 0);
                    if (GUILayout.Button(Core.tTrashCan, s, GUILayout.Width(24), GUILayout.Height(24)))
                    {
                        if (scene.landmarks[i].tasks.Count() == 0)
                        {
                            if (EditorUtility.DisplayDialog("Delete Landmark?", "Delete Landmark " + scene.landmarks[i].title + "? This cannot be undone!", "Delete", "Cancel"))
                            {
                                DeleteLandmark(i);
                            }
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Cannot Delete", "You must remove, or move, all tasks in the Landmark " + scene.landmarks[i].title + " before you can remove it", "Ok");
                        }
                        exists = false;
                    }
                    GUILayout.FlexibleSpace();
                    //GUILayout.Space(64);

                    text = landmarks.title;
                    if (text.Length > 25) text = text.Substring(0, 20);

                    s = StyleCheck("label");
                    s.fontSize = 20;
                    s.fontStyle = FontStyle.Bold;
                    s.alignment = TextAnchor.MiddleCenter;
                    s.normal.textColor = WhiteOrBlack(landmarks.GizmoColor);

                    if (!landmarks.editTitle)
                    {
                        if (GUILayout.Button(text, s))
                        {
                            landmarks.editTitle = true;
                        }
                    }
                    else
                    {
                        if (Event.current.type == EventType.KeyDown)
                        {
                            if (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return)
                            {
                                landmarks.editTitle = false;
                                window.Repaint();
                            }
                            if (Event.current.keyCode == KeyCode.Escape)
                            {
                                landmarks.editTitle = false;
                                window.Repaint();
                            }
                        }
                        s.alignment = TextAnchor.MiddleLeft;
                        s.normal.textColor = Color.white;
                        s.normal.background = Core.tBlack;
                        landmarks.title = GUILayout.TextField(landmarks.title, s, GUILayout.Width(screenWidth - 160));
                    }


                    //GUILayout.Label(text, s);
                    GUILayout.FlexibleSpace();
                    scene.landmarks[i].SetColor(EditorGUILayout.ColorField(scene.landmarks[i].GizmoColor, GUILayout.Width(48)));

                    GUILayout.EndHorizontal();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                //GUILayout.EndVertical();

                GUILayout.BeginVertical();
                {
                    #region bLandmarkThumbnail            
                    // if (GUILayout.Button(new GUIContent(Core.tRefresh, "Refresh Image"), GUILayout.Height(20)))
                    // {
                    //     RefreshLandmarkThumbnail(i);
                    // }
                    if (GUILayout.Button(landmarks.tScreenshot, GUIStyle.none, GUILayout.Width(thumbnailSize), GUILayout.Height(thumbnailSize)))
                    {
                        scene.selectedLandmark = i;
                        ZoomToLocation(i);
                    }
                    Rect pos = GUILayoutUtility.GetLastRect();
                    // if (GUI.Button(new Rect(pos.x + 4, pos.y + 4, 32, 32), new GUIContent(Core.tRefresh, "Refresh Image")))
                    // {
                    //     RefreshLandmarkThumbnail(i);
                    // }
                    float p = ((float)landmarks.progress) * screenWidth;
                    if (p < 0) p = 0;

                    GUI.DrawTexture(new Rect(pos.x, pos.y + pos.height, screenWidth, 20f), landmarks.tGizmoColor50);
                    GUI.DrawTexture(new Rect(pos.x, pos.y + pos.height, p, 20f), landmarks.tGizmoColor);

                    s = new GUIStyle();
                    s.fontStyle = FontStyle.Bold;
                    s.fontSize = 20;
                    s.alignment = TextAnchor.MiddleCenter;
                    s.normal.textColor = WhiteOrBlack(landmarks.GizmoColor);
                    var v = (landmarks.progress * 100);
                    if (float.IsNaN(v)) v = 0;
                    if (float.IsInfinity(v)) v = 0;
                    GUI.Label(new Rect(pos.x, pos.y + pos.height, screenWidth, 20f), v.ToString("F0") + "%", s);

                    //EditorGUI.ProgressBar(new Rect(pos.x, pos.y + pos.height, screenWidth - 20, 8f), landmarks.progress, "");
                    Color prevGUI = GUI.color;
                    GUI.color = new Color(prevGUI.r, prevGUI.g, prevGUI.b, 0.5f);
                    if (landmarks.viewState == Landmark.ViewState.is2D)
                        DrawTexture(new Rect(pos.x + pos.width - 50, pos.y + pos.height - 26, 48, 24), Core.tIcon2D);
                    else if (landmarks.viewState == Landmark.ViewState.is3D)
                        DrawTexture(new Rect(pos.x + pos.width - 50, pos.y + pos.height - 26, 48, 24), Core.tIcon3D);
                    else if (landmarks.viewState == Landmark.ViewState.isOrthographic)
                        DrawTexture(new Rect(pos.x + pos.width - 50, pos.y + pos.height - 26, 48, 24), Core.tIconISO);
                    GUI.color = prevGUI;
                    #endregion
                }
                GUILayout.EndVertical();

                GUILayout.Space(2);
                thumbnailSize = (screenWidth / 2);
                float bSize = (thumbnailSize * 0.7f) / 6;
                s = StyleCheck();
                s.fontStyle = FontStyle.Bold;
                s.fontSize = (int)(bSize * 0.5);
                s.alignment = TextAnchor.MiddleLeft;
                sBackground = new GUIStyle();
                //GUILayout.FlexibleSpace();
                if (Event.current.type == EventType.Layout)
                    localWidth = screenWidth / 2 - 16;
                GUILayout.BeginVertical(GUILayout.Width(localWidth));
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.EndHorizontal();

                    sBackground.normal.background = Core.tGrey;
                    GUILayout.BeginHorizontal(sBackground);
                    {
                        if (GUILayout.Button(Core.tIconPin, GUILayout.Width(screenWidth / 2 / 5 - 8), GUILayout.Height(bSize)))
                        {
                            landmarks.editorState = Landmark.EditorState.General;
                        }
                        if (GUILayout.Button(Core.tIconTag, GUILayout.Width(screenWidth / 2 / 5 - 8), GUILayout.Height(bSize)))
                        {
                            landmarks.editorState = Landmark.EditorState.Tags;
                        }
                        if (GUILayout.Button(Core.tIconTask, GUILayout.Width(screenWidth / 2 / 5 - 8), GUILayout.Height(bSize)))
                        {
                            landmarks.editorState = Landmark.EditorState.Tasks;
                        }
                        if (GUILayout.Button(Core.tGallery, GUILayout.Width(screenWidth / 2 / 5 - 8), GUILayout.Height(bSize)))
                        {
                            landmarks.editorState = Landmark.EditorState.Gallery;
                        }
                        if (GUILayout.Button(Core.tAtlasSettings, GUILayout.Width(screenWidth / 2 / 5 - 8), GUILayout.Height(bSize)))
                        {
                            landmarks.editorState = Landmark.EditorState.Options;
                        }

                    }
                    GUILayout.EndHorizontal();
                    switch (landmarks.editorState)
                    {
                        case Landmark.EditorState.General:
                            {
                                var prevFloat = landmarks.isFloating;
                                GUILayout.Space(8);
                                GUILayout.BeginHorizontal();
                                landmarks.isFloating = GUILayout.Toggle(landmarks.isFloating, "Lock Position To GameObject");
                                if (landmarks.isFloating) if (GUILayout.Button("Refresh")) RefreshLandmarkThumbnail(i);
                                GUILayout.EndHorizontal();

                                //Debug.Log("gallery.images.Count: " + landmarks.gallery.images.Count);

                                if (prevFloat != landmarks.isFloating)
                                {
                                    RefreshLandmarkThumbnail(i);
                                }
                                if (landmarks.isFloating)
                                {
                                    //Debug.Log("gallery.images.Count: " + landmarks.gallery.images.Count);

                                    var prevObj = landmarks.floatingPositionObject;
                                    landmarks.floatingPositionObject = (GameObject)EditorGUILayout.ObjectField(landmarks.floatingPositionObject, typeof(GameObject), true);
                                    if (prevObj != landmarks.floatingPositionObject && landmarks.isFloating)
                                    {
                                        RefreshLandmarkThumbnail(i);
                                    }
                                }
                                else
                                {
                                    //Debug.Log("gallery.images.Count: " + landmarks.gallery.images.Count);

                                    if (GUILayout.Button("Reposition At Scene Location"))
                                    {
                                        RefreshLandmarkPosition(i);
                                    }
                                }

                                if (GUILayout.Button("Refresh Landmark Image"))
                                {
                                    RefreshLandmarkThumbnail(i);
                                }

                                sBackground.normal.background = Core.tBlue;
                                GUILayout.BeginHorizontal(sBackground);
                                {
                                    //GUILayout.Button(new GUIContent("  Details", Core.tEditPen));
                                    //GUILayout.Button("Task Preview");
                                    //GUILayout.Button("Options");
                                    GUILayout.Label("Description");
                                }
                                GUILayout.EndHorizontal();

                                landmarks.scrollbarDescription = GUILayout.BeginScrollView(landmarks.scrollbarDescription, GUILayout.Height(thumbnailSize - bSize - 100));
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        s = GetTextAreaStyle(landmarks.description, screenWidth - 32);
                                        float h = s.fixedHeight;
                                        if (h < thumbnailSize - bSize - 20 - 10) h = thumbnailSize - bSize - 110;
                                        landmarks.description = GUILayout.TextArea(landmarks.description, GUILayout.Height(h));
                                    }
                                    GUILayout.EndHorizontal();
                                }
                                GUILayout.EndScrollView();

                                //var prevFloat = landmarks.isFloating;
                                //GUILayout.BeginHorizontal();
                                //landmarks.isFloating = GUILayout.Toggle(landmarks.isFloating, "Lock Position To GameObject");
                                //if (landmarks.isFloating) if (GUILayout.Button("Refresh")) RefreshLandmarkThumbnail(i);
                                //GUILayout.EndHorizontal();
                                //if (prevFloat != landmarks.isFloating)
                                //{
                                //    RefreshLandmarkThumbnail(i);
                                //}
                                //if (landmarks.isFloating)
                                //{
                                //    var prevObj = landmarks.floatingPositionObject;
                                //    landmarks.floatingPositionObject = (GameObject)EditorGUILayout.ObjectField(landmarks.floatingPositionObject, typeof(GameObject), true);
                                //    if (prevObj != landmarks.floatingPositionObject && landmarks.isFloating)
                                //    {
                                //        RefreshLandmarkThumbnail(i);
                                //    }
                                //}
                                //else
                                //{
                                //    if (GUILayout.Button("Reposition Here"))
                                //    {
                                //        RefreshLandmarkPosition(i);
                                //    }
                                //}

                                //sBackground.normal.background = Core.tBlue;
                                //GUILayout.BeginHorizontal(sBackground);
                                //{
                                //    s = StyleCheck();
                                //    s.fontSize = (int)(bSize * 0.5);
                                //    s.alignment = TextAnchor.MiddleLeft;
                                //    GUILayout.Label(new GUIContent("  Tags", Core.tIconTag), s, GUILayout.Height(16));
                                //    GUILayout.FlexibleSpace();

                                //    s = StyleCheck("button");
                                //    s.fontSize = 8;
                                //    s.alignment = TextAnchor.MiddleLeft;
                                //}
                                //GUILayout.EndHorizontal();

                                //GUILayout.BeginHorizontal();
                                //{
                                //    //GUILayout.EndHorizontal();
                                //    s = StyleCheck();
                                //    s.normal.background = Core.tTeal;
                                //    s.margin = new RectOffset(9, 9, 0, 0);
                                //    //GUILayout.BeginHorizontal(s);

                                //    float labelSize = 0f;

                                //    for (int t = 0; t < landmarks.tags.Count; t++)
                                //    {
                                //        s = StyleCheck("button");
                                //        s.fontSize = 10;
                                //        labelSize += s.CalcSize(new GUIContent(landmarks.tags[t].name)).x;
                                //        if (labelSize > localWidth)
                                //        {
                                //            labelSize = 0;
                                //            GUILayout.EndHorizontal();
                                //            GUILayout.BeginHorizontal();
                                //        }

                                //        if (landmarks.tags[t].active)
                                //        {
                                //            //GUILayout.EndHorizontal();
                                //            //GUILayout.BeginHorizontal(s);

                                //            GUILayout.Label(landmarks.tags[t].name, s);
                                //        }
                                //    }

                                //}
                                //GUILayout.EndHorizontal();
                                break;
                            }
                        case Landmark.EditorState.Tasks:
                            {
                                if (GUILayout.Button("Edit Tasks"))
                                {
                                    scene.selectedLandmark = i;
                                    scene.landmarkDetailOpen = true;
                                    scene.landmarkDetailState = TA.LandmarkDetailState.tasks;
                                }


                                landmarks.scrollbarTasks = GUILayout.BeginScrollView(landmarks.scrollbarTasks, false, true, GUILayout.Height(thumbnailSize - bSize - 32));
                                {
                                    GUILayout.BeginVertical();
                                    {
                                        for (int ii = 0; ii < landmarks.tasks.Count; ii++)
                                        {
                                            var tasks = landmarks.tasks[ii];
                                            //float p = ((float)tasks.progress / 100) * localWidth - 8 + 4 - 48;
                                            //if (p < 0) p = 0;

                                            sBackground = StyleBack(sBackground, landmarks.tasks[ii].tColor);
                                            //GUILayout.BeginHorizontal();

                                            GUILayout.BeginHorizontal(); GUILayout.EndHorizontal();

                                            var lastRect = GUILayoutUtility.GetLastRect();


                                            var xx = 20f;

                                            if (tasks.tColor50 != null && tasks.tColor != null)
                                            {
                                                ProgressBar(new Rect(xx, lastRect.y, localWidth - 8 + 4 - 48, 44), (float)tasks.progress / 100, tasks.tColor50, tasks.tColor);
                                                //GUI.DrawTexture(new Rect(xx, lastRect.y, localWidth - 8 + 4 - 48, 44), tasks.tColor50);
                                                //GUI.DrawTexture(new Rect(xx, lastRect.y, p, 44), tasks.tColor);
                                            }

                                            if (GUI.Button(new Rect(xx, lastRect.y, localWidth - 16 - 8 + 4, 44), "", GUIStyle.none))
                                            {
                                                scene.landmarkDetailOpen = true;
                                                scene.selectedLandmark = i;
                                                scene.landmarkDetailState = TA.LandmarkDetailState.tasks;
                                                scene.zoomToTaskID = tasks.createdDate;
                                            }

                                            string text = tasks.name;
                                            if (text.Length > 25) text = text.Substring(0, 22) + "...";
                                            s = StyleCheck();
                                            s.alignment = TextAnchor.MiddleCenter;
                                            s.normal.textColor = Color.black;
                                            s.fontStyle = FontStyle.Bold;
                                            s.fontSize = 14;
                                            GUILayout.Label(text, s);

                                            lastRect = GUILayoutUtility.GetLastRect();

                                            s.normal.textColor = Color.black;//Core.cGrey;//tasks.tColorDark.GetPixel(0,0);
                                            s.fontSize = 14;
                                            GUI.Label(new Rect(lastRect.x + 1, lastRect.y + 1, lastRect.width, lastRect.height), text, s);
                                            GUI.Label(new Rect(lastRect.x, lastRect.y + 1, lastRect.width, lastRect.height), text, s);
                                            GUI.Label(new Rect(lastRect.x + 1, lastRect.y, lastRect.width, lastRect.height), text, s);
                                            GUI.Label(new Rect(lastRect.x - 1, lastRect.y - 1, lastRect.width, lastRect.height), text, s);
                                            GUI.Label(new Rect(lastRect.x, lastRect.y - 1, lastRect.width, lastRect.height), text, s);
                                            GUI.Label(new Rect(lastRect.x - 1, lastRect.y, lastRect.width, lastRect.height), text, s);

                                            s = StyleCheck();
                                            //if (tasks.color.r > .5f || tasks.color.g > .5f || tasks.color.b > .5f)
                                            //{
                                            //    s.normal.textColor = Color.black;
                                            //}
                                            //else
                                            //{
                                            //    s.normal.textColor = Color.white;
                                            //}
                                            s.alignment = TextAnchor.MiddleCenter;
                                            s.normal.textColor = Color.white;
                                            s.fontStyle = FontStyle.Bold;
                                            s.fontSize = 14;
                                            GUI.Label(lastRect, text, s);

                                            GUILayout.BeginHorizontal();
                                            {
                                                s = StyleCheck();
                                                float bGap = ((localWidth - 26) / 32);
                                                float bWidth = ((localWidth - 26) - (bGap * 2)) / 2;
                                                s.alignment = TextAnchor.MiddleCenter;
                                                s.fontSize = 8;
                                                s.fontStyle = FontStyle.Bold;
                                                //GUILayout.Space(8);
                                                GUILayout.FlexibleSpace();
                                                GUILayout.BeginVertical();
                                                {
                                                    s.normal.background = Core.tBlack;
                                                    //GUILayout.Label("Stage", GUILayout.Width(64));
                                                    GUILayout.Label(tasks.stage.ToString().ToUpper(), s, GUILayout.Width(bWidth));
                                                }
                                                GUILayout.EndVertical();

                                                GUILayout.Space(bGap);

                                                GUILayout.BeginVertical();
                                                {
                                                    s.normal.background = Core.tBlack;
                                                    //GUILayout.Label("Priority", GUILayout.Width(64));
                                                    if (tasks.priority == Task.Priority.urgent) s.normal.background = Core.tRedDark;
                                                    GUILayout.Label(tasks.priority.ToString().ToUpper(), s, GUILayout.Width(bWidth));
                                                }
                                                GUILayout.EndVertical();

                                                GUILayout.FlexibleSpace();
                                                GUILayout.EndHorizontal();
                                                GUILayout.Space(4);
                                                GUILayout.BeginHorizontal();
                                                GUILayout.FlexibleSpace();

                                                GUILayout.BeginVertical();
                                                {
                                                    s.normal.background = Core.tBlack;
                                                    double dd = Math.Floor(DateTime.Now.Subtract(new DateTime(tasks.dueDateDT)).TotalDays);
                                                    string sDue = "NOT SET";
                                                    if (dd < 0)
                                                    {
                                                        sDue = Math.Abs(dd).ToString() + " DAYS";
                                                    }
                                                    else if (dd > 0)
                                                    {
                                                        s.normal.background = Core.DimBackground(1, Color.red, .75f);// Core.tRedDark;
                                                        sDue = "OVERDUE";
                                                    }
                                                    else
                                                    {
                                                        sDue = "TODAY";
                                                    }

                                                    //GUILayout.Label("Due in", GUILayout.Width(64));
                                                    GUILayout.Label(sDue, s, GUILayout.Width(bWidth));
                                                }
                                                GUILayout.EndVertical();

                                                GUILayout.Space(bGap);

                                                GUILayout.BeginVertical();
                                                {
                                                    s.normal.background = Core.tBlack;
                                                    //GUILayout.Label("Timer", GUILayout.Width(64));
                                                    text = "0m";
                                                    if (tasks.autoTimer)
                                                    {
                                                        long am = tasks.activeMinutes;

                                                        if (am < 10080) text = am / 1440 + "d " + am % 1440 / 60 + "h " + am % 60 + "m";
                                                        if (am < 1440) text = am / 60 + "h " + am % 60 + "m";
                                                        if (am < 60) text = am + "m";
                                                    }

                                                    GUILayout.Label(text, s, GUILayout.Width(bWidth));
                                                }
                                                GUILayout.EndVertical();
                                                GUILayout.FlexibleSpace();
                                            }

                                            GUILayout.EndHorizontal();

                                            GUILayout.Space(20);
                                        }
                                    }
                                    GUILayout.EndVertical();
                                }
                                GUILayout.EndScrollView();
                                break;
                            }
                        case Landmark.EditorState.Tags:
                            {
                                //void RefreshTags()
                                //{
                                //    //for (int lm = 0; lm < scene.landmarks.Count; lm++)
                                //    //{

                                //    //    var landmark = scene.landmarks[lm];
                                //    //    Debug.Log("---" + landmark.title);
                                //    //    landmark.tagsTemp.Clear();
                                //    //    //dedupe
                                //    //    for (int v2 = 0; v2 < Core.dataV2.tags.Count; v2++)
                                //    //    {
                                //    //        int c = 0;
                                //    //        bool a = false;
                                //    //        landmark.tagsTemp.Add(Core.dataV2.tags[v2]);
                                //    //        Debug.Log("----" + Core.dataV2.tags[v2].name + "/" + landmark.tagsTemp[v2].name);
                                //    //        for (int tc = 0; tc < landmark.tags.Count; tc++)
                                //    //        {
                                //    //            Debug.LogError(lm + "/" + v2 + "/" + tc + "   " + landmark.tags[tc].name + " | " + landmark.tags[tc].active);
                                //    //            landmark.tagsTemp[v2].active = false;
                                //    //            if (Core.dataV2.tags[v2].name == landmark.tags[tc].name)
                                //    //            {
                                //    //                Debug.LogError(landmark.tags[tc].active + " | " + landmark.tagsTemp[v2].active);


                                //    //            }
                                //    //        }
                                //    //    }
                                //    //}
                                //    for (int lm = 0; lm < scene.landmarks.Count; lm++)
                                //    {
                                //        var landmark = scene.landmarks[lm];
                                //        //landmark.tagsTemp.Clear();
                                //        //dedupe
                                //        for (int v2 = 0; v2 < Core.dataV2.tags.Count; v2++)
                                //        {

                                //            landmark.tagsTemp.Add(Core.dataV2.tags[v2]);
                                //            for (int tc = 0; tc < landmark.tags.Count; tc++)
                                //            {
                                //                landmark.tagsTemp[v2].active = false;
                                //                //Debug.Log(lm + "/" + v2 + "/" + tc + "   " + landmark.title + " | " + landmark.tags[tc].name + " | " + landmark.tags[tc].active + " | " + landmark.tagsTemp[v2].name + " | " + landmark.tagsTemp[v2].active);
                                //                if (Core.dataV2.tags[v2].name == landmark.tags[tc].name)
                                //                {
                                //                    if (landmark.tags[tc].active)// || landmark.tagsTemp[v2].active)
                                //                    {
                                //                        //Debug.LogError(lm + "/" + v2 + "/" + tc + "   " + landmark.title + " | " + landmark.tags[tc].name + " | " + landmark.tags[tc].active + " | " + landmark.tagsTemp[v2].name + " | " + landmark.tagsTemp[v2].active);
                                //                        landmark.tagsTemp[v2].active = true;
                                //                        //Debug.LogError(lm + "/" + v2 + "/" + tc + "   " + landmark.title + " | " + landmark.tags[tc].name + " | " + landmark.tags[tc].active + " | " + landmark.tagsTemp[v2].name + " | " + landmark.tagsTemp[v2].active);
                                //                    }
                                //                    else
                                //                    {
                                //                        //Debug.LogWarning(lm + "/" + v2 + "/" + tc + "   " + landmark.title + " | " + landmark.tags[tc].name + " | " + landmark.tags[tc].active + " | " + landmark.tagsTemp[v2].name + " | " + landmark.tagsTemp[v2].active);
                                //                    }
                                //                    //if (landmark.tags[tc].active) landmark.tagsTemp[v2].active = true;

                                //                    //if (landmark.tags[tc].active)


                                //                }
                                //            }
                                //        }
                                //        //for (int tt = 0; tt < landmark.tagsTemp.Count; tt++)
                                //        //{
                                //        //    //Debug.Log(lm + "/" + tt + "    " + landmark.tagsTemp[tt].name + "    " + landmark.tagsTemp[tt].active);
                                //        //    //if (landmark.tags[tc].name == landmark.tagsTemp[tt].name)
                                //        //    //{
                                //        //    //    Debug.Log(lm + "/" + v2 + "/" + tc + "/" + tt + "    " + landmark.tagsTemp[tt].name);
                                //        //    //    landmark.tagsTemp[tt].active = true;
                                //        //    //}
                                //        //}

                                //        landmark.tags = landmark.tagsTemp;
                                //        landmark.tagsTemp = new List<Tags>();
                                //    }
                                //}

                                //DrawTaskHeader("Tags", Core.tIconTag, Core.tTeal);
                                GUILayout.BeginHorizontal();
                                {
                                    #region EditTagButtons
                                    switch (tagEditMode)
                                    {
                                        case TagEditMode.none:
                                            #region AddNewTag
                                            //GUILayout.Label("New:");
                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();

                                            Core.dataV2.newTag = GUILayout.TextField(Core.dataV2.newTag);
                                            if (GUILayout.Button(Core.tPlus, GUIStyle.none, GUILayout.Width(16)))
                                            {
                                                Core.dataV2.tags.Add(new Tags(Core.dataV2.newTag, ""));
                                                Core.dataV2.tags = Core.dataV2.tags.Distinct().ToList();
                                                Core.dataV2.newTag = "";
                                                //bool found = false;
                                                //for (int ii = 0; ii < Core.dataV2.tags.Count; ii++)
                                                //{
                                                //    if (Core.dataV2.tags[ii].name.ToUpper() == landmarks.newTag.ToUpper())
                                                //    {
                                                //        found = true;
                                                //    }
                                                //}
                                                //if (!found)
                                                //{
                                                //    Core.dataV2.tags.Add(new Tags(landmarks.newTag));
                                                //    Core.dataV2.tags = Core.dataV2.tags.Distinct().ToList();
                                                //    Core.dataV2.tags = Core.dataV2.tags.OrderBy(x => x.name).ToList();
                                                //    for (int ii = 0; ii < scene.landmarks.Count; ii++)
                                                //    {
                                                //        scene.landmarks[ii].tags.Add(new Tags(landmarks.newTag));
                                                //    }
                                                //    //landmarks.tags.Add(new Tags(landmarks.newTag));
                                                //    //for (int ii = 0; ii < scene.landmarks.Count; ii++)
                                                //    //{
                                                //    //    landmarks.tags.Add(new Tags(newTag));
                                                //    //    landmarks.tags = landmarks.tags.Distinct().ToList();
                                                //    //    landmarks.tags = landmarks.tags.OrderBy(x => x.name).ToList();
                                                //    //}
                                                //    Core.SaveData();
                                                //}
                                                //landmarks.newTag = "";
                                                //RefreshTags();

                                            }
                                            #endregion
                                            //GUILayout.FlexibleSpace();
                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                            #region EditTags
                                            if (GUILayout.Button(Core.tEditPen, GUILayout.Width(32), GUILayout.Height(18)))
                                            {
                                                tagEditMode = TagEditMode.edit;
                                                tagListCopy = new List<string>();
                                                for (int ii = 0; ii < Core.dataV2.tags.Count; ii++)
                                                {
                                                    tagListCopy.Add(Core.dataV2.tags[ii].name);
                                                }
                                            }
                                            #endregion
                                            #region DeleteTags
                                            if (GUILayout.Button(Core.tTrashCan, GUILayout.Width(32), GUILayout.Height(18))) tagEditMode = TagEditMode.delete;
                                            text = "Hide Inactive Tags";
                                            if (landmarks.hideInactiveTags) text = "Show Inactive Tags";
                                            if (GUILayout.Button(text))
                                            {
                                                landmarks.hideInactiveTags = !landmarks.hideInactiveTags;
                                            }
                                            //GUILayout.Space(16);

                                            #endregion
                                            break;
                                        case TagEditMode.edit:
                                            //if (screenWidth >= 212)
                                            //{
                                            //    GUILayout.Space(screenWidth - 96 - 96 - 16);
                                            //}

                                            #region SaveTags
                                            if (GUILayout.Button("Save Tags", GUILayout.Width(96)))
                                            {

                                                tagEditMode = TagEditMode.none;
                                                ////RefreshTags();
                                                ////landmarks.tagsTemp = landmarks.tags;


                                                for (int ii = 0; ii < tagListCopy.Count; ii++)
                                                {
                                                    Core.dataV2.tags[ii].name = tagListCopy[ii];
                                                }

                                                //for (int ii = 0; ii < tagListCopy.Count; ii++)
                                                //{
                                                //    if (Core.dataV2.tags[ii].name != tagListCopy[ii])
                                                //    {
                                                //        Core.dataV2.tags[ii].name = tagListCopy[ii];
                                                //    }
                                                //}
                                                //for (int ii = 0; ii < scene.landmarks.Count; ii++)
                                                //{
                                                //    for (int iii = 0; iii < scene.landmarks[ii].tags.Count; iii++)
                                                //    {
                                                //        scene.landmarks[ii].tags[iii].name = tagListCopy[ii];
                                                //    }
                                                //    scene.landmarks[ii].tags = scene.landmarks[ii].tags.Distinct().ToList();
                                                //    scene.landmarks[ii].tags = scene.landmarks[ii].tags.OrderBy(x => x.name).ToList();

                                                //}
                                                //for (int x = 0; x < Core.dataV2.tags.Count; x++)
                                                //{
                                                //    Core.dataV2.tags[x].name = tagListCopy[x];
                                                //    Debug.Log(Core.dataV2.tags[x].name + " // " + Core.dataV2.tags[x].active+"=="+ tagListCopy[x]);
                                                //}
                                                //Core.dataV2.tags = Core.dataV2.tags.Distinct().ToList();
                                                //Core.dataV2.tags = Core.dataV2.tags.OrderBy(x => x.name).ToList();
                                                //RefreshTags();
                                                Core.SaveData();
                                            }
                                            #endregion
                                            #region CancelEdit
                                            if (screenWidth < 210) { GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(); }
                                            if (GUILayout.Button("Cancel", GUILayout.Width(96)))
                                            {
                                                tagEditMode = TagEditMode.none;
                                                tagListCopy = null;
                                                //RefreshTags();
                                            }
                                            #endregion
                                            break;
                                        case TagEditMode.delete:
                                            //if (screenWidth >= 212)
                                            //{
                                            //    GUILayout.Space(screenWidth - 128 - 16);
                                            //}
                                            #region ExitDeleteMode
                                            if (GUILayout.Button("Exit Delete Mode", GUILayout.Width(128)))
                                            {
                                                tagEditMode = TagEditMode.none;
                                                //RefreshTags();
                                            }
                                            #endregion
                                            break;
                                    }

                                    #endregion
                                }
                                GUILayout.EndHorizontal();

                                var landmarkId = landmarks.timeStamp;

                                landmarks.scrollbarEditTags = GUILayout.BeginScrollView(landmarks.scrollbarEditTags);
                                {
                                    GUILayout.BeginHorizontal(GUILayout.Height(200));
                                    {
                                        #region ShowTags


                                        switch (tagEditMode)
                                        {
                                            case TagEditMode.none:
                                                #region SelectionMode
                                                //landmarks.tags = landmarks.tags.Distinct().ToList();
                                                //for (int ii= 0; ii < landmarks.tags.Count; ii++) {
                                                //    for (int iii = 0; iii < landmarks.tags.Count; iii++) {
                                                //        if (landmarks.tags[ii].name == landmarks.tags[iii].name)
                                                //        {
                                                //            landmarks.tags.RemoveAt(ii);
                                                //        }
                                                //    }
                                                //}

                                                for (int ii = 0; ii < Core.dataV2.tags.Count; ii++)
                                                {
                                                    bool isActive = Core.dataV2.tags[ii].GetActive(landmarkId);
                                                    if (landmarks.hideInactiveTags && !isActive) continue;

                                                    //Debug.Log(landmarks.tags[ii].name);
                                                    GUILayout.EndHorizontal(); GUILayout.Space(4); GUILayout.BeginHorizontal();
                                                    s = new GUIStyle("button");
                                                    Color prevColor = GUI.backgroundColor;
                                                    if (isActive)
                                                    {
                                                        GUI.backgroundColor = landmarks.tGizmoColor.GetPixel(0, 0);//Core.cTeal;
                                                        //s = new GUIStyle("toggle");
                                                        s.fontStyle = FontStyle.BoldAndItalic;
                                                        s.alignment = TextAnchor.MiddleCenter;
                                                    }
                                                    if (GUILayout.Button(Core.dataV2.tags[ii].name, s))
                                                    {
                                                        //Debug.Log(landmarks.title + " / " + landmarks.tags[ii].name + " / " + landmarks.tags[ii].active);
                                                        Core.dataV2.tags[ii].SetActive(landmarkId, !isActive);
                                                    }
                                                    //landmarks.tags[ii].active = GUILayout.Toggle(landmarks.tags[ii].active, landmarks.tags[ii].name, s);
                                                    GUI.backgroundColor = prevColor;
                                                }
                                                #endregion
                                                break;
                                            case TagEditMode.edit:
                                                #region EditMode
                                                for (int ii = 0; ii < tagListCopy.Count; ii++)
                                                {
                                                    GUILayout.EndHorizontal(); GUILayout.Space(4); GUILayout.BeginHorizontal();
                                                    s = new GUIStyle("textfield");
                                                    s.alignment = TextAnchor.MiddleCenter;
                                                    tagListCopy[ii] = GUILayout.TextField(tagListCopy[ii], s);
                                                }
                                                #endregion
                                                break;
                                            case TagEditMode.delete:
                                                #region DeleteMode
                                                var p = GUI.backgroundColor;
                                                GUI.backgroundColor = Color.red;
                                                for (int ii = 0; ii < Core.dataV2.tags.Count; ii++)
                                                {
                                                    GUILayout.EndHorizontal(); GUILayout.Space(4); GUILayout.BeginHorizontal();
                                                    if (GUILayout.Button(Core.dataV2.tags[ii].name))
                                                    {
                                                        if (EditorUtility.DisplayDialog("Delete Tag?", "Are you sure you want to delete the tag " + Core.dataV2.tags[i].name + "?  This cannot be undone and affects all landmarks.", "Delete Tag", "ABORT!!!"))
                                                        {
                                                            //for (int iii = 0; iii < scene.landmarks.Count; iii++)
                                                            //{
                                                            //    scene.landmarks[iii].tags.RemoveAt(ii);

                                                            //}
                                                            Core.dataV2.tags.RemoveAt(ii);
                                                            Core.dataV2.tags = Core.dataV2.tags.Distinct().ToList();
                                                            Core.dataV2.tags = Core.dataV2.tags.OrderBy(x => x.name).ToList();
                                                            Core.SaveData();
                                                            RefreshSettings();
                                                        }
                                                    };
                                                }
                                                GUI.backgroundColor = p;
                                                #endregion
                                                break;
                                        }

                                        #endregion
                                    }
                                    GUILayout.EndHorizontal();
                                }
                                GUILayout.EndScrollView();
                                break;
                            }
                        case Landmark.EditorState.Gallery:
                            {
                                var localData = scene.landmarks[i];

                                landmarks.scrollbarGallery = GUILayout.BeginScrollView(landmarks.scrollbarGallery);
                                {

                                    //GUILayout.BeginHorizontal();
                                    {
                                        var gallery = localData.gallery;
                                        float h = 20;
                                        if (gallery.currentImage < 0) gallery.currentImage = 0;

                                        if (gallery.images.Count == 0) h = 240;

                                        GUILayout.BeginHorizontal();
                                        {
                                            if (GUILayout.Button(new GUIContent("Add Image", Core.tPlus), GUILayout.Height(h)))
                                            {
                                                gallery.images.Add(new Landmark.Gallery.Image((Texture2D)AssetDatabase.LoadAssetAtPath(Core.taPath + "/Screenshots/TaskAtlas_Default.png", typeof(Texture2D))));

                                                gallery.currentImage = gallery.images.Count - 1;
                                                gallery.images[gallery.currentImage].scaleHeight = true;

                                                gallery.images[gallery.currentImage].imagePath = AssetDatabase.GetAssetPath(gallery.images[gallery.currentImage].image);
                                            }
                                        }
                                        GUILayout.EndHorizontal();

                                        HorizontalLine(Color.black);

                                        if (gallery.images.Count > 0)
                                        {



                                            scene.scrollPosLandmarkDetailGalleryThumbnails = GUILayout.BeginScrollView(scene.scrollPosLandmarkDetailGalleryThumbnails, GUILayout.Height(100));
                                            {

                                                GUILayout.BeginHorizontal();
                                                {
                                                    if (gallery.images.Count > 0)
                                                    {

                                                        GUILayout.FlexibleSpace();
                                                        for (int iii = 0; iii < gallery.images.Count; iii++)
                                                        {

                                                            GUILayout.BeginVertical();
                                                            {
                                                                if (iii == gallery.currentImage)
                                                                {
                                                                    GUILayout.Space(-1);
                                                                    GUILayout.Box(Core.tGreen, GUILayout.Width(64), GUILayout.Height(64));
                                                                    Rect r = GUILayoutUtility.GetLastRect();
                                                                    r = new Rect(r.x + 3, r.y + 3, r.width - 6, r.height - 6);
                                                                    if (GUI.Button(r, gallery.images[iii].image))
                                                                    {
                                                                        gallery.currentImage = iii;
                                                                    }
                                                                    //
                                                                    GUILayout.Space(1);
                                                                }
                                                                else
                                                                {

                                                                    if (GUILayout.Button(gallery.images[iii].image, GUILayout.Width(64), GUILayout.Height(64)))
                                                                    {
                                                                        gallery.currentImage = iii;
                                                                    }

                                                                }
                                                                GUILayout.BeginHorizontal(GUILayout.Width(64));
                                                                {
                                                                    //if (gallery.images[gallery.currentImage].image == null) Debug.Log("gallery.images is NULL!!");
                                                                    GUILayout.Space(14);
                                                                    if (GUILayout.Button(Core.tArrowLeft, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(12)))
                                                                    {
                                                                        if (iii > 0)
                                                                        {
                                                                            gallery.images.Move(iii, iii - 1);
                                                                            if (iii == gallery.currentImage) gallery.currentImage--;
                                                                            else if (iii - 1 == gallery.currentImage) gallery.currentImage++;
                                                                        }
                                                                    }
                                                                    if (GUILayout.Button(Core.tTrashCan, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(12)))
                                                                    {

                                                                        if (EditorUtility.DisplayDialog("Delete Image", "Are you sure you want to delete this image?  Cannot be undone!  Please note that the image file itself is not being deleted and can still be found in your project.", "Delete", "Cancel"))
                                                                        {

                                                                            gallery.images.RemoveAt(iii);
                                                                            gallery.currentImage--;
                                                                            GUILayout.EndHorizontal();
                                                                            GUILayout.EndVertical();
                                                                            GUILayout.EndHorizontal();
                                                                            GUILayout.EndScrollView();
                                                                            //Debug.Log("gallery.images.Count: [" + gallery.currentImage + "] " + gallery.images[gallery.currentImage].image + " || " + gallery.images.Count);
                                                                            goto endGallery;
                                                                        }
                                                                    }
                                                                    if (GUILayout.Button(Core.tArrowRight, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(12)))
                                                                    {

                                                                        if (iii < gallery.images.Count)
                                                                        {
                                                                            gallery.images.Move(iii, iii + 1);
                                                                            if (iii == gallery.currentImage) gallery.currentImage++;
                                                                            else if (iii + 1 == gallery.currentImage) gallery.currentImage--;
                                                                        }
                                                                    }
                                                                    GUILayout.FlexibleSpace();
                                                                }
                                                                GUILayout.EndHorizontal();

                                                            }
                                                            GUILayout.EndVertical();
                                                        }
                                                    }

                                                    GUILayout.FlexibleSpace();
                                                }
                                                GUILayout.EndHorizontal();
                                            }
                                            GUILayout.EndScrollView();
                                        }
                                        s = new GUIStyle("button");

                                        if (gallery.images.Count > 0)
                                        {

                                            GUILayout.BeginHorizontal();
                                            {
                                                GUILayout.Label((gallery.images[gallery.currentImage].scaleFactor * 100).ToString("F0") + "%", GUILayout.Width(36));
                                                if (gallery.images[gallery.currentImage].scaleHeight) GUI.enabled = false;
                                                var prev = gallery.images[gallery.currentImage].scaleFactor;
                                                gallery.images[gallery.currentImage].scaleFactor = GUILayout.HorizontalSlider(gallery.images[gallery.currentImage].scaleFactor, 0.05f, 2.0f);
                                                if (prev != gallery.images[gallery.currentImage].scaleFactor)
                                                {
                                                    Core.SaveData();
                                                    // scene.scrollPosLandmarkDetailGallery.x = prev.x / (gallery.images[gallery.currentImage].width * gallery.scaleFactor) / scene.scrollPosLandmarkDetailGallery.x;
                                                    // scene.scrollPosLandmarkDetailGallery.y = prev.y / (gallery.images[gallery.currentImage].height * gallery.scaleFactor) / scene.scrollPosLandmarkDetailGallery.y;
                                                }
                                                if (GUILayout.Button("100%", GUILayout.Width(48)))
                                                {
                                                    gallery.images[gallery.currentImage].scaleFactor = 1.0f;
                                                    Core.SaveData();
                                                }
                                                if (gallery.images[gallery.currentImage].scaleHeight) GUI.enabled = true;
                                                var prev2 = gallery.images[gallery.currentImage].scaleHeight;
                                                gallery.images[gallery.currentImage].scaleHeight = GUILayout.Toggle(gallery.images[gallery.currentImage].scaleHeight, "Fit", GUILayout.Width(48));//, s);
                                                if (prev2 != gallery.images[gallery.currentImage].scaleHeight)
                                                    Core.SaveData();
                                            }
                                            GUILayout.EndHorizontal();
                                            HorizontalLine(Color.black);


                                            gallery.images[gallery.currentImage].scrollPos = GUILayout.BeginScrollView(gallery.images[gallery.currentImage].scrollPos, GUIStyle.none, GUILayout.Height(gallery.images[gallery.currentImage].height));// 600));  //
                                            {

                                                if (gallery.images.Count == 0)
                                                {
                                                    GUILayout.Label("No Images, Add One By Clicking Above!");
                                                    GUILayout.EndScrollView();
                                                    goto endGallery;
                                                }

                                                if (gallery.currentImage > gallery.images.Count - 1) gallery.currentImage = gallery.images.Count - 1;

                                                GUILayout.BeginVertical();
                                                {

                                                    // if (gallery.images[gallery.currentImage] == null)
                                                    // {
                                                    //     gallery.currentImage = gallery.images.Count;
                                                    // }
                                                    if (gallery.currentImage > gallery.images.Count) gallery.currentImage = gallery.images.Count - 1;
                                                    if (gallery.currentImage < 0) gallery.currentImage = 0;
                                                    var image = gallery.images[gallery.currentImage].image;



                                                    if (gallery.images[gallery.currentImage].image == null)
                                                    {
                                                        //Debug.Log("1. Null, path is " + gallery.images[gallery.currentImage].imagePath);

                                                        if (gallery.images[gallery.currentImage].image == null)
                                                            if (gallery.images[gallery.currentImage].imagePath != "")
                                                            {
                                                                //Debug.Log(gallery.currentImage + ": " + gallery.images[gallery.currentImage].imagePath);
                                                                gallery.images[gallery.currentImage].image = (Texture2D)AssetDatabase.LoadAssetAtPath(gallery.images[gallery.currentImage].imagePath, typeof(Texture2D));
                                                            }
                                                        //Debug.Log("2. Null, path is " + gallery.images[gallery.currentImage].imagePath);
                                                        if (gallery.images[gallery.currentImage].image == null)
                                                        {
                                                            gallery.images.RemoveAt(gallery.currentImage);
                                                            gallery.currentImage--;

                                                            GUILayout.EndVertical();
                                                            GUILayout.EndVertical();
                                                            GUILayout.EndScrollView();


                                                            goto endGallery;
                                                        }
                                                    }

                                                    // float x = screenWidth - 32, y = ((screenWidth - 32) / image.width) * image.height;
                                                    float x = image.width, y = image.height;

                                                    // Debug.Log(r + "|" + screenHeight + "|" + (screenHeight - r.y));
                                                    if (gallery.images[gallery.currentImage].scaleHeight)
                                                    {

                                                        float rS = (screenWidth / 2 - 16 - 32) / (screenHeight - 350), rI = x / y;
                                                        if (rS > rI)
                                                        {
                                                            x = (x * (screenHeight - 380) / y);
                                                            y = (screenHeight - 380);
                                                        }
                                                        else
                                                        {
                                                            x = (screenWidth / 2 - 16 - 32);
                                                            y = image.height * (x / image.width);
                                                            //y = (y * (screenWidth / 2 - 16 - 32) / x);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        x = image.width * gallery.images[gallery.currentImage].scaleFactor;
                                                        y = image.height * gallery.images[gallery.currentImage].scaleFactor;
                                                    }


                                                    gallery.images[gallery.currentImage].height = y;
                                                    //Debug.Log(y);

                                                    GUILayout.FlexibleSpace();
                                                    GUILayout.BeginHorizontal();
                                                    GUILayout.FlexibleSpace();

                                                    var prev3 = gallery.images[gallery.currentImage].image;
                                                    gallery.images[gallery.currentImage].image = (Texture2D)EditorGUILayout.ObjectField(image, typeof(Texture2D), false,
                                                                                                                                  GUILayout.Width(x), GUILayout.Height(y));

                                                    if (prev3 != gallery.images[gallery.currentImage].image)
                                                    {
                                                        //Debug.Log("AssetDatabase.GetAssetPath: " + AssetDatabase.GetAssetPath(gallery.images[gallery.currentImage].image));
                                                        gallery.images[gallery.currentImage].imagePath = AssetDatabase.GetAssetPath(gallery.images[gallery.currentImage].image);
                                                        Core.SaveData();
                                                        //
                                                    }

                                                    GUILayout.FlexibleSpace();
                                                    GUILayout.EndHorizontal();
                                                    GUILayout.FlexibleSpace();
                                                }
                                                GUILayout.EndVertical();


                                            }
                                            GUILayout.EndScrollView();
                                            GUILayout.BeginHorizontal();
                                            {
                                                if (GUILayout.Button("Replace with Current View"))
                                                {
                                                    Camera svc = SceneView.lastActiveSceneView.camera;
                                                    RenderTexture currentRT = new RenderTexture(1024, 1024, 24);
                                                    svc.targetTexture = currentRT;
                                                    svc.Render();

                                                    RenderTexture.active = currentRT;

                                                    Texture2D image = new Texture2D(1024, 1024, TextureFormat.RGB24, false);
                                                    image.ReadPixels(new Rect(0, 0, 1024, 1024), 0, 0);
                                                    image.Apply();

                                                    svc.targetTexture = null;
                                                    RenderTexture.active = null;

                                                    if (gallery.images == null) gallery.images = new List<Landmark.Gallery.Image>();
                                                    gallery.images[gallery.currentImage].image = (Texture2D)AssetDatabase.LoadAssetAtPath(SaveTexture(image), typeof(Texture2D));
                                                    gallery.images[gallery.currentImage].imagePath = AssetDatabase.GetAssetPath(gallery.images[gallery.currentImage].image);
                                                    DestroyImmediate(image);

                                                    // gallery.images.Add(Core.tEditPen);
                                                    //gallery.currentImage = gallery.images.Count - 1;
                                                    Core.SaveData();
                                                }

                                            }
                                            GUILayout.EndHorizontal();

                                        }
                                    endGallery:;
                                    }
                                    //GUILayout.EndHorizontal();
                                    //
                                }
                                GUILayout.EndScrollView();
                                //Debug.Log("gallery.images.Count: " + landmarks.gallery.images.Count);
                                break;
                            }
                        case Landmark.EditorState.Options:
                            {
                                //Debug.Log("gallery.images.Count: " + landmarks.gallery.images.Count);

                                landmarks.scrollbarOptions = GUILayout.BeginScrollView(landmarks.scrollbarOptions);
                                {
                                    scene.landmarks[scene.selectedLandmark].showGizmo = GUILayout.Toggle(scene.landmarks[scene.selectedLandmark].showGizmo, "Show Label in Scene");
                                    //if (GUILayout.Button("Landmark Label"))
                                    //{
                                    //    if (scene.landmarks[scene.selectedLandmark].showGizmo)
                                    //    {
                                    //        scene.landmarks[scene.selectedLandmark].showGizmo = false;
                                    //    }
                                    //    else
                                    //    {
                                    //        scene.landmarks[scene.selectedLandmark].showGizmo = true;
                                    //    }
                                    //}
                                    //
                                    GUILayout.BeginHorizontal();
                                    {
                                        //Debug.Log("gallery.images.Count: " + landmarks.gallery.images.Count);

                                        //GUILayout.FlexibleSpace();
                                        if (scene.landmarks[scene.selectedLandmark].showGizmo)
                                        {
                                            EditorGUILayout.HelpBox("Onscreen Labels", MessageType.None);
                                            s = new GUIStyle("button");
                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                            scene.landmarks[scene.selectedLandmark].wsOffset = EditorGUILayout.Vector3Field("Label Offset: ", scene.landmarks[scene.selectedLandmark].wsOffset, GUILayout.Width(localWidth - 24));
                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                            scene.landmarks[scene.selectedLandmark].fadeGizmo = GUILayout.Toggle(scene.landmarks[scene.selectedLandmark].fadeGizmo, "Fade with Distance", s, GUILayout.Width(localWidth - 24));

                                            if (!scene.landmarks[scene.selectedLandmark].fadeGizmo) GUI.enabled = false;

                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                            GUILayout.Label("Fade Distance:");
                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                            var prev = scene.landmarks[scene.selectedLandmark].fadeStart + scene.landmarks[scene.selectedLandmark].fadeEnd;
                                            EditorGUILayout.MinMaxSlider(ref scene.landmarks[scene.selectedLandmark].fadeStart,
                                                                                                             ref scene.landmarks[scene.selectedLandmark].fadeEnd,
                                                                                                             0, 1000, GUILayout.Width(localWidth - 24));
                                            if (prev != scene.landmarks[scene.selectedLandmark].fadeStart + scene.landmarks[scene.selectedLandmark].fadeEnd)
                                                Core.SaveData();
                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                            s = new GUIStyle("textfield");
                                            //GUILayout.Space(16);
                                            GUILayout.Label("Fade Start at:");
                                            GUILayout.Label(scene.landmarks[scene.selectedLandmark].fadeStart.ToString("F1"), s); GUILayout.FlexibleSpace();
                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                            GUILayout.Label("Fade End at:");
                                            GUILayout.Label(scene.landmarks[scene.selectedLandmark].fadeEnd.ToString("F1"), s); GUILayout.FlexibleSpace();
                                            GUILayout.Space(16);
                                            GUI.enabled = true;
                                        }
                                    }
                                    GUILayout.EndHorizontal();
                                    GUILayout.Space(16);

                                    //DrawTaskHeader("Task Defaults", Core.tIconTask, Core.tBlue);
                                    EditorGUILayout.HelpBox("3D Stickies", MessageType.None);

                                    GUILayout.Label("Fade Distance:");
                                    GUILayout.BeginHorizontal();
                                    {
                                        //Debug.Log("gallery.images.Count: " + landmarks.gallery.images.Count);

                                        var prev = landmarks.taskFadeStart + landmarks.taskFadeEnd;
                                        EditorGUILayout.MinMaxSlider(ref landmarks.taskFadeStart, ref landmarks.taskFadeEnd, 0f, 1000f, GUILayout.Width(localWidth - 24));
                                        if (prev != landmarks.taskFadeStart + landmarks.taskFadeEnd) Core.SaveData();
                                        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                        s = new GUIStyle("textfield");
                                        //GUILayout.Space(16);
                                        GUILayout.Label("Fade Start at:");
                                        GUILayout.Label(landmarks.taskFadeStart.ToString("F1"), s); GUILayout.FlexibleSpace();
                                        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();

                                        GUILayout.Label("Fade End at:");
                                        GUILayout.Label(landmarks.taskFadeEnd.ToString("F1"), s); GUILayout.FlexibleSpace();
                                        GUILayout.Space(16);
                                    }
                                    GUILayout.EndHorizontal();
                                }
                                GUILayout.EndScrollView();
                                break;
                            }
                    }


                }

            }

            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            //Debug.Log("gallery.images.Count: " + landmarks.gallery.images.Count);

            //var prevFloat = landmarks.isFloating;
            //GUILayout.Space(8);
            //GUILayout.BeginHorizontal();
            //landmarks.isFloating = GUILayout.Toggle(landmarks.isFloating, "Lock Position To GameObject");
            //if (landmarks.isFloating) if (GUILayout.Button("Refresh")) RefreshLandmarkThumbnail(i);
            //GUILayout.EndHorizontal();

            ////Debug.Log("gallery.images.Count: " + landmarks.gallery.images.Count);

            //if (prevFloat != landmarks.isFloating)
            //{
            //    RefreshLandmarkThumbnail(i);
            //}
            //if (landmarks.isFloating)
            //{
            //    //Debug.Log("gallery.images.Count: " + landmarks.gallery.images.Count);

            //    var prevObj = landmarks.floatingPositionObject;
            //    landmarks.floatingPositionObject = (GameObject)EditorGUILayout.ObjectField(landmarks.floatingPositionObject, typeof(GameObject), true);
            //    if (prevObj != landmarks.floatingPositionObject && landmarks.isFloating)
            //    {
            //        RefreshLandmarkThumbnail(i);
            //    }
            //}
            //else
            //{
            //    //Debug.Log("gallery.images.Count: " + landmarks.gallery.images.Count);

            //    if (GUILayout.Button("Reposition Here"))
            //    {
            //        RefreshLandmarkPosition(i);
            //    }
            //}
            //Debug.Log("gallery.images.Count: " + landmarks.gallery.images.Count);


            return exists;
        }
        static Rect rButton;
        static void LoadViewLandmarkDetailButtons(int i)
        {
            var landmarks = scene.landmarks[i];
            GUIStyle s = StyleCheck();
            GUILayout.BeginHorizontal();
            {
                s = StyleCheck();
                s.fontSize = 20;
                s.fontStyle = FontStyle.Bold;
                s.alignment = TextAnchor.MiddleCenter;

                s.normal.textColor = WhiteOrBlack(landmarks.GizmoColor);
                s.normal.background = landmarks.tGizmoColor50;

                GUILayout.BeginHorizontal(s);
                GUILayout.FlexibleSpace();

                text = landmarks.title;
                if (text.Length > 25) text = text.Substring(0, 15);

                s.normal.background = Core.tTransparent;
                GUILayout.Label(text, s);
                GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();

                GUILayout.Space(2);
                thumbnailSize = (screenWidth / 2);
                float bSize = 26;//(thumbnailSize * 0.7f) / 5;
                s = StyleCheck();
                s.fontStyle = FontStyle.Bold;
                s.fontSize = (int)(bSize * 0.5);
                s.alignment = TextAnchor.MiddleLeft;

                sBackground = new GUIStyle();

                //if (scene.landmarkDetailState == TA.LandmarkDetailState.general)
                //{
                //    GUILayout.BeginVertical();
                //}
                //else
                //{
                //    sBackground.normal.background = Core.tGrey;

                //    GUILayout.BeginVertical(sBackground);
                //}
                //{
                //    GUILayout.Space(((bSize * 1.2f) - bSize) / 2);
                //    GUILayout.BeginHorizontal(GUILayout.Height(bSize * 1.2f));
                //    {
                //        GUILayout.Space(4);
                //        if (GUILayout.Button(Core.tEditPen, GUILayout.Width(bSize), GUILayout.Height(bSize)))
                //        {
                //            scene.selectedLandmark = i;
                //            scene.landmarkDetailOpen = true;
                //            scene.landmarkDetailState = TA.LandmarkDetailState.general;
                //        }

                //        GUILayout.Label("Details", s, GUILayout.Width(screenWidth / 3 - bSize - 32), GUILayout.Height(bSize));
                //    }
                //    GUILayout.EndHorizontal();
                //}
                //GUILayout.EndVertical();

                //if (scene.landmarkDetailState == TA.LandmarkDetailState.tasks)
                //{
                //    GUILayout.BeginVertical();
                //}
                //else
                //{
                //    sBackground.normal.background = Core.tGrey;
                //    GUILayout.BeginVertical(sBackground);
                //}
                //{
                //    GUILayout.Space(((bSize * 1.2f) - bSize) / 2);
                //    GUILayout.BeginHorizontal(GUILayout.Height(bSize * 1.2f));
                //    {
                //        GUILayout.FlexibleSpace();
                //        s.fontSize = 24;
                //        GUILayout.Label("Task List", s, GUILayout.Width(screenWidth / 3 - bSize - 32), GUILayout.Height(bSize));
                //        GUILayout.FlexibleSpace();
                //    }
                //    GUILayout.EndHorizontal();
                //}
                //GUILayout.EndVertical();

                //if (scene.landmarkDetailState == TA.LandmarkDetailState.gallery)
                //{
                //    GUILayout.BeginVertical();
                //}
                //else
                //{
                //    sBackground.normal.background = Core.tGrey;
                //    GUILayout.BeginVertical(sBackground);
                //}
                //{
                //    GUILayout.Space(((bSize * 1.2f) - bSize) / 2);
                //    GUILayout.BeginHorizontal(GUILayout.Height(bSize * 1.2f));
                //    {
                //        GUILayout.Space(4);
                //        if (GUILayout.Button(Core.tRefresh, GUILayout.Width(bSize), GUILayout.Height(bSize)))
                //        {
                //            scene.selectedLandmark = i;
                //            scene.landmarkDetailOpen = true;
                //            scene.landmarkDetailState = TA.LandmarkDetailState.gallery;
                //        }
                //        GUILayout.Label("Gallery", s, GUILayout.Width(screenWidth / 3 - bSize - 32), GUILayout.Height(bSize));
                //    }
                //    GUILayout.EndHorizontal();
                //}
                //GUILayout.EndVertical();

                // sBackground.normal.background = Core.tRed;
                // GUILayout.BeginVertical(sBackground);

                // {
                //     GUILayout.Space(((bSize * 1.2f) - bSize) / 2);
                //     GUILayout.BeginHorizontal(GUILayout.Height(bSize * 1.2f));
                //     {
                //         GUILayout.Space(4);
                //         if (GUILayout.Button(Core.tArrange, GUILayout.Width(bSize), GUILayout.Height(bSize)))
                //         {
                //             scene.selectedLandmark = i;
                //             scene.landmarkDetailOpen = true;
                //             scene.landmarkDetailState = Scene.LandmarkDetailState.delete;
                //         }
                //         GUILayout.Label("Sort", s, GUILayout.Width(screenWidth / 3 - bSize - 32), GUILayout.Height(bSize));
                //     }
                //     GUILayout.EndHorizontal();
                // }
                // GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        enum TagEditMode { none, edit, delete }
        static TagEditMode tagEditMode = TagEditMode.none;
        static void LoadViewLandmarkDetail()
        {
            int detailThumbnailSize = 160 - 24;
            int optionSize = (int)(detailThumbnailSize) / 3;
            var localData = scene.landmarks[scene.selectedLandmark];
            var landmarks = localData;
            Rect lastRect, headerRect;
            string headerText = "";
            Texture2D headerColor = Core.tTransparent;
            Color headerColorText = new Color();
            GUIStyle s;

            GUILayout.BeginArea(new Rect(screenWidthMargin, 0, screenWidth - 6, screenHeight));
            {
                GUILayout.BeginHorizontal();
                {
                    GUISkin skin = ScriptableObject.CreateInstance<GUISkin>();
                    skin.button = new GUIStyle("Button");

                    if (GUILayout.Button("Back to Landmark List", skin.button, GUILayout.Height(18)))
                    {
                        scene.landmarkDetailOpen = false;
                        tagEditMode = TagEditMode.none;
                    }
                    DrawTagFilterMask();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(4);

                LoadViewLandmarkCarousel();

                LoadViewLandmarkDetailButtons(scene.selectedLandmark);
                sBackground.normal.background = Core.tGreen;
                GUILayout.BeginHorizontal();
                {

                    localWidth = screenWidth - 80;

                    // HorizontalLine(Color.black);

                    scene.scrollPosLandmarkDetail = GUILayout.BeginScrollView(scene.scrollPosLandmarkDetail, GUIStyle.none, GUILayout.Height(screenHeight - 60));

                    GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                    {
                        localWidth -= 32;

                        switch (scene.landmarkDetailState)
                        {
                            //#region GeneralState
                            //case TA.LandmarkDetailState.general:
                            //    LoadViewLandmarkDetailGeneral(scene.selectedLandmark);
                            //    break;
                            //#endregion

                            #region GalleryState
                            case TA.LandmarkDetailState.gallery:

                                // GUILayout.BeginHorizontal();
                                // {
                                //     #region NewTask
                                //     s = StyleCheck("button");
                                //     s.fontSize = 20;
                                //     s.alignment = TextAnchor.MiddleCenter;
                                //     if (GUILayout.Button(new GUIContent("   New Image", Core.tPlus), s, GUILayout.Height(24), GUILayout.Width(screenWidth - 12)))
                                //     {
                                //         localData.gallery.Add(new Scene.Landmark.Gallery("New Image"));
                                //         Core.SaveData();
                                //     }
                                //     s = StyleCheck();
                                //     #endregion
                                // }
                                // GUILayout.EndHorizontal();

                                //    var gallery = localData.gallery;
                                //    float h = 20;
                                //    if (gallery.currentImage < 0) gallery.currentImage = 0;

                                //    if (gallery.images.Count == 0) h = 100;
                                //    GUILayout.BeginHorizontal();
                                //    {
                                //        if (GUILayout.Button(new GUIContent("Add Image", Core.tPlus), GUILayout.Height(h)))
                                //        {
                                //            gallery.images.Add(new Landmark.Gallery.Image((Texture2D)AssetDatabase.LoadAssetAtPath(Core.taPath + "/Screenshots/TaskAtlas_Default.png", typeof(Texture2D))));
                                //            gallery.currentImage = gallery.images.Count - 1;
                                //        }
                                //    }
                                //    GUILayout.EndHorizontal();

                                //    HorizontalLine(Color.black);
                                //    if (gallery.images.Count > 0)
                                //    {
                                //        scene.scrollPosLandmarkDetailGalleryThumbnails = GUILayout.BeginScrollView(scene.scrollPosLandmarkDetailGalleryThumbnails, GUILayout.Height(100));
                                //        {
                                //            GUILayout.BeginHorizontal();
                                //            {
                                //                if (gallery.images.Count > 0)
                                //                {
                                //                    GUILayout.FlexibleSpace();
                                //                    for (int i = 0; i < gallery.images.Count; i++)
                                //                    {
                                //                        GUILayout.BeginVertical();
                                //                        {
                                //                            if (i == gallery.currentImage)
                                //                            {
                                //                                GUILayout.Space(-1);
                                //                                GUILayout.Box(Core.tGreen, GUILayout.Width(64), GUILayout.Height(64));
                                //                                Rect r = GUILayoutUtility.GetLastRect();
                                //                                r = new Rect(r.x + 3, r.y + 3, r.width - 6, r.height - 6);
                                //                                if (GUI.Button(r, gallery.images[i].image))
                                //                                {
                                //                                    gallery.currentImage = i;
                                //                                }

                                //                                GUILayout.Space(1);
                                //                            }
                                //                            else
                                //                            {
                                //                                if (GUILayout.Button(gallery.images[i].image, GUILayout.Width(64), GUILayout.Height(64)))
                                //                                {
                                //                                    gallery.currentImage = i;
                                //                                }
                                //                            }
                                //                            GUILayout.BeginHorizontal(GUILayout.Width(64));
                                //                            {
                                //                                GUILayout.Space(14);
                                //                                if (GUILayout.Button(Core.tArrowLeft, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(12)))
                                //                                {
                                //                                    if (i > 0)
                                //                                    {
                                //                                        gallery.images.Move(i, i - 1);
                                //                                        if (i == gallery.currentImage) gallery.currentImage--;
                                //                                        else if (i - 1 == gallery.currentImage) gallery.currentImage++;
                                //                                    }
                                //                                }
                                //                                if (GUILayout.Button(Core.tTrashCan, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(12)))
                                //                                {
                                //                                    if (EditorUtility.DisplayDialog("Delete Image", "Are you sure you want to delete this image?  Cannot be undone!  Please note that the image file itself is not being deleted and can still be found in your project.", "Delete", "Cancel"))
                                //                                    {
                                //                                        gallery.images.RemoveAt(i);
                                //                                        gallery.currentImage--;
                                //                                        GUILayout.EndHorizontal();
                                //                                        GUILayout.EndVertical();
                                //                                        GUILayout.EndHorizontal();
                                //                                        GUILayout.EndScrollView();
                                //                                        goto endGallery;
                                //                                    }
                                //                                }
                                //                                if (GUILayout.Button(Core.tArrowRight, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(12)))
                                //                                {
                                //                                    if (i < gallery.images.Count)
                                //                                    {
                                //                                        gallery.images.Move(i, i + 1);
                                //                                        if (i == gallery.currentImage) gallery.currentImage++;
                                //                                        else if (i + 1 == gallery.currentImage) gallery.currentImage--;
                                //                                    }
                                //                                }
                                //                                GUILayout.FlexibleSpace();
                                //                            }
                                //                            GUILayout.EndHorizontal();

                                //                        }
                                //                        GUILayout.EndVertical();
                                //                    }

                                //                }

                                //                GUILayout.FlexibleSpace();
                                //            }
                                //            GUILayout.EndHorizontal();
                                //        }
                                //        GUILayout.EndScrollView();
                                //    }
                                //    s = new GUIStyle("button");

                                //    if (gallery.images.Count > 0)
                                //    {
                                //        GUILayout.BeginHorizontal();
                                //        {
                                //            GUILayout.Label((gallery.images[gallery.currentImage].scaleFactor * 100).ToString("F0") + "%", GUILayout.Width(80));
                                //            if (gallery.images[gallery.currentImage].scaleHeight) GUI.enabled = false;
                                //            var prev = gallery.images[gallery.currentImage].scaleFactor;
                                //            gallery.images[gallery.currentImage].scaleFactor = GUILayout.HorizontalSlider(gallery.images[gallery.currentImage].scaleFactor, 0.05f, 10.0f);
                                //            if (prev != gallery.images[gallery.currentImage].scaleFactor)
                                //            {
                                //                Core.SaveData();
                                //                // scene.scrollPosLandmarkDetailGallery.x = prev.x / (gallery.images[gallery.currentImage].width * gallery.scaleFactor) / scene.scrollPosLandmarkDetailGallery.x;
                                //                // scene.scrollPosLandmarkDetailGallery.y = prev.y / (gallery.images[gallery.currentImage].height * gallery.scaleFactor) / scene.scrollPosLandmarkDetailGallery.y;
                                //            }
                                //            if (GUILayout.Button("100%", GUILayout.Width(48)))
                                //            {
                                //                gallery.images[gallery.currentImage].scaleFactor = 1.0f;
                                //                Core.SaveData();
                                //            }
                                //            if (gallery.images[gallery.currentImage].scaleHeight) GUI.enabled = true;
                                //            var prev2 = gallery.images[gallery.currentImage].scaleHeight;
                                //            gallery.images[gallery.currentImage].scaleHeight = GUILayout.Toggle(gallery.images[gallery.currentImage].scaleHeight, "Fit", GUILayout.Width(48));//, s);
                                //            if (prev2 != gallery.images[gallery.currentImage].scaleHeight)
                                //                Core.SaveData();
                                //        }
                                //        GUILayout.EndHorizontal();
                                //        HorizontalLine(Color.black);

                                //        gallery.images[gallery.currentImage].scrollPos = GUILayout.BeginScrollView(gallery.images[gallery.currentImage].scrollPos, GUIStyle.none, GUILayout.Height(screenHeight - 480));
                                //        {
                                //            if (gallery.images.Count == 0)
                                //            {
                                //                GUILayout.Label("No Images, Add One By Clicking Above!");
                                //                GUILayout.EndScrollView();
                                //                goto endGallery;
                                //            }

                                //            if (gallery.currentImage > gallery.images.Count - 1) gallery.currentImage = gallery.images.Count - 1;

                                //            GUILayout.BeginVertical();
                                //            {
                                //                // if (gallery.images[gallery.currentImage] == null)
                                //                // {
                                //                //     gallery.currentImage = gallery.images.Count;
                                //                // }
                                //                if (gallery.currentImage > gallery.images.Count) gallery.currentImage = gallery.images.Count - 1;
                                //                if (gallery.currentImage < 0) gallery.currentImage = 0;
                                //                var image = gallery.images[gallery.currentImage].image;

                                //                if (image == null)
                                //                {
                                //                    gallery.images.RemoveAt(gallery.currentImage);
                                //                    gallery.currentImage--;
                                //                    GUILayout.EndVertical();
                                //                    GUILayout.EndVertical();
                                //                    GUILayout.EndScrollView();

                                //                    goto endGallery;
                                //                }

                                //                // float x = screenWidth - 32, y = ((screenWidth - 32) / image.width) * image.height;
                                //                float x = image.width, y = image.height;

                                //                // Debug.Log(r + "|" + screenHeight + "|" + (screenHeight - r.y));
                                //                if (gallery.images[gallery.currentImage].scaleHeight)
                                //                {
                                //                    float rS = (screenWidth - 32) / (screenHeight - 350), rI = x / y;
                                //                    if (rS > rI)
                                //                    {
                                //                        x = (x * (screenHeight - 380) / y);
                                //                        y = (screenHeight - 380);
                                //                    }
                                //                    else
                                //                    {
                                //                        x = (screenWidth - 32);
                                //                        y = (y * (screenWidth - 32) / x);
                                //                    }
                                //                }
                                //                else
                                //                {
                                //                    x = image.width * gallery.images[gallery.currentImage].scaleFactor;
                                //                    y = image.height * gallery.images[gallery.currentImage].scaleFactor;
                                //                }

                                //                GUILayout.FlexibleSpace();
                                //                GUILayout.BeginHorizontal();
                                //                GUILayout.FlexibleSpace();

                                //                var prev3 = gallery.images[gallery.currentImage].image;
                                //                gallery.images[gallery.currentImage].image = (Texture2D)EditorGUILayout.ObjectField(image, typeof(Texture2D), false,
                                //                                                                                              GUILayout.Width(x), GUILayout.Height(y));
                                //                if (prev3 != gallery.images[gallery.currentImage].image)
                                //                    Core.SaveData();

                                //                GUILayout.FlexibleSpace();
                                //                GUILayout.EndHorizontal();
                                //                GUILayout.FlexibleSpace();
                                //            }
                                //            GUILayout.EndVertical();


                                //        }
                                //        GUILayout.EndScrollView();
                                //        GUILayout.BeginHorizontal();
                                //        {
                                //            if (GUILayout.Button("Replace with Current View"))
                                //            {
                                //                Camera svc = SceneView.lastActiveSceneView.camera;
                                //                RenderTexture currentRT = new RenderTexture(1024, 1024, 24);
                                //                svc.targetTexture = currentRT;
                                //                svc.Render();

                                //                RenderTexture.active = currentRT;

                                //                Texture2D image = new Texture2D(1024, 1024, TextureFormat.RGB24, false);
                                //                image.ReadPixels(new Rect(0, 0, 1024, 1024), 0, 0);
                                //                image.Apply();

                                //                svc.targetTexture = null;
                                //                RenderTexture.active = null;

                                //                if (gallery.images == null) gallery.images = new List<Landmark.Gallery.Image>();
                                //                gallery.images[gallery.currentImage].image = (Texture2D)AssetDatabase.LoadAssetAtPath(SaveTexture(image), typeof(Texture2D));
                                //                DestroyImmediate(image);

                                //                // gallery.images.Add(Core.tEditPen);
                                //                gallery.currentImage = gallery.images.Count - 1;
                                //            }

                                //        }
                                //        GUILayout.EndHorizontal();
                                //    }
                                //endGallery:;
                                break;
                            #endregion

                            #region TaskState
                            case TA.LandmarkDetailState.tasks:
                                GUILayout.BeginHorizontal();
                                {
                                    #region NewTask
                                    s = StyleCheck("button");
                                    s.fontSize = 20;
                                    s.alignment = TextAnchor.MiddleCenter;
                                    // sBackground.normal.background = Core.tRed;
                                    // GUILayout.BeginVertical(sBackground);

                                    // {
                                    //     GUILayout.Space(((bSize * 1.2f) - bSize) / 2);
                                    //     GUILayout.BeginHorizontal(GUILayout.Height(bSize * 1.2f));
                                    //     {
                                    //         GUILayout.Space(4);
                                    //         if (GUILayout.Button(Core.tArrange, GUILayout.Width(bSize), GUILayout.Height(bSize)))
                                    //         {
                                    //             scene.selectedLandmark = i;
                                    //             scene.landmarkDetailOpen = true;
                                    //             scene.landmarkDetailState = Scene.LandmarkDetailState.delete;
                                    //         }
                                    //         GUILayout.Label("Sort", s, GUILayout.Width(screenWidth / 3 - bSize - 32), GUILayout.Height(bSize));
                                    //     }
                                    //     GUILayout.EndHorizontal();
                                    // }
                                    // GUILayout.EndVertical();



                                    if (GUILayout.Button(new GUIContent("  New Task", Core.tPlus), s, GUILayout.Height(32), GUILayout.Width((screenWidth - 12) / 2)))
                                    {
                                        localData.tasks.Add(new Task("New Task"));
                                        Core.SaveData();
                                    }

                                    if (GUILayout.Button(new GUIContent("  Arrange", Core.tArrange), s, GUILayout.Height(32), GUILayout.Width((screenWidth - 12) / 2)))
                                    {
                                        // scene.selectedLandmark = i;
                                        scene.landmarkDetailOpen = true;
                                        scene.landmarkDetailState = TA.LandmarkDetailState.delete;
                                    }
                                    s = StyleCheck();
                                    #endregion
                                }
                                GUILayout.EndHorizontal();

                                lastRect = GUILayoutUtility.GetLastRect();
                                headerRect = GUILayoutUtility.GetLastRect();

                                //s = new GUIStyle(GUI.skin.horizontalScrollbar);
                                //s = StyleCheck("horizontalScrollBar");
                                //scene.scrollPosLandmarkDetailTags = GUILayout.BeginScrollView(scene.scrollPosLandmarkDetailTags, false, true, GUIStyle.none, GUIStyle.none, GUILayout.Height(screenHeight - 300));
                                scene.scrollPosLandmarkDetailTags = GUILayout.BeginScrollView(scene.scrollPosLandmarkDetailTags, false, true, GUILayout.Height(screenHeight - 300));
                                {
                                    GUILayout.BeginVertical(StyleBack(s, Core.tGreyBright));
                                    {

                                        if (localData.tasks.Count == 0)
                                        {
                                            GUILayout.Label("No Tasks, Add One By Clicking Above!");
                                            goto end;
                                        }
                                        for (int i = 0; i < localData.tasks.Count; i++)
                                        {
                                            //string checksum = "",checksum2="";
                                            EditorGUI.BeginChangeCheck();
                                            var prev = localData.tasks[i].Copy();
                                            var tasks = localData.tasks[i];
                                            HorizontalLine(Color.black);
                                            Rect rID = GUILayoutUtility.GetLastRect();

                                            GUIStyle sNone = null;
                                            sNone = StyleBack(sNone, Core.MakeTex(1, 1, new Color(0, 0, 0, 0)));
                                            float p = ((float)tasks.progress / 100) * (screenWidth - 32);
                                            if (tasks.isFolded)
                                            {


                                                sBackground = StyleBack(sBackground, tasks.tColor);
                                                //GUILayout.BeginHorizontal();
                                                //GUILayout.Space(8);
                                                GUILayout.BeginHorizontal(); GUILayout.EndHorizontal();

                                                lastRect = GUILayoutUtility.GetLastRect();
                                                //GUILayout.EndHorizontal();

                                                GUI.DrawTexture(new Rect(lastRect.x + 8, lastRect.y, screenWidth - 32, 96), tasks.tColor50);
                                                GUI.DrawTexture(new Rect(lastRect.x + 8, lastRect.y, p, 96), tasks.tColor);



                                                GUILayout.BeginHorizontal(GUILayout.Width(screenWidth - 24));
                                                {
                                                    GUILayout.Space(8);
                                                    //GUILayout.BeginVertical(sBackground, GUILayout.Width(64), GUILayout.Height(64));
                                                    //{
                                                    //    GUILayout.Space(8);
                                                    //    GUILayout.BeginHorizontal();
                                                    //    {
                                                    //        GUILayout.FlexibleSpace();
                                                    //        if (tasks.isSticky)
                                                    //        {
                                                    //            GUILayout.Button(Core.tTaskSticky, GUIStyle.none, GUILayout.Width(48), GUILayout.Height(48));
                                                    //        }
                                                    //        else
                                                    //        {
                                                    //            GUILayout.Button(Core.tIconTask, GUIStyle.none, GUILayout.Width(48), GUILayout.Height(48));
                                                    //        }
                                                    //        GUILayout.FlexibleSpace();
                                                    //    }
                                                    //    GUILayout.EndHorizontal();
                                                    //    //GUILayout.Label("Bottom");
                                                    //}
                                                    //GUILayout.EndVertical();
                                                    //GUILayout.Space(8);


                                                    GUILayout.BeginVertical(sNone, GUILayout.Height(96));
                                                    {
                                                        GUILayout.FlexibleSpace();
                                                        GUILayout.BeginHorizontal();
                                                        {
                                                            s = StyleCheck();

                                                            s.alignment = TextAnchor.MiddleLeft;
                                                            s.fontSize = 32;
                                                            s.fontStyle = FontStyle.Bold;

                                                            if (tasks.color.r > .5f || tasks.color.g > .5f || tasks.color.b > .5f)
                                                            {
                                                                s.normal.textColor = Color.black;
                                                            }
                                                            else
                                                            {
                                                                s.normal.textColor = Color.white;
                                                            }

                                                            text = tasks.name;
                                                            if (text.Length > 20) s.fontSize = 20;//text = text.Substring(0, 27) + "...";
                                                            if (text.Length > 30) s.fontSize = 14;//text = text.Substring(0, 27) + "...";
                                                            if (text.Length > 65) text = text.Substring(0, 62) + "...";
                                                            GUILayout.Space(16);
                                                            GUILayout.Label(" " + text, s);
                                                            //GUILayout.FlexibleSpace();
                                                        }
                                                        GUILayout.EndHorizontal();
                                                        GUILayout.FlexibleSpace();
                                                    }
                                                    GUILayout.EndVertical();
                                                    GUILayout.Space(8);
                                                    //GUILayout.Space(80);
                                                }
                                                GUILayout.EndHorizontal();



                                                lastRect = GUILayoutUtility.GetLastRect();

                                                //GUILayout.Space(4);
                                                //GUILayout.BeginHorizontal();
                                                //{
                                                //    GUILayout.Space(8);
                                                //    s = StyleCheck("label");
                                                //    s.normal.background = tasks.tColorDark;
                                                //    s.padding = new RectOffset(10, 10, 10, 10);
                                                //    s.wordWrap = true;
                                                //    GUILayout.TextArea(tasks.description,s);
                                                //    GUILayout.Space(4);
                                                //}
                                                //GUILayout.EndHorizontal();
                                                //GUILayout.Space(8);

                                                GUILayout.BeginHorizontal();
                                                {
                                                    s = StyleCheck();

                                                    s.alignment = TextAnchor.MiddleCenter;
                                                    s.fontSize = 10;
                                                    s.fontStyle = FontStyle.Bold;
                                                    GUILayout.FlexibleSpace();
                                                    GUILayout.BeginVertical();
                                                    {
                                                        s.normal.background = Core.tBlue;
                                                        GUILayout.Label("Stage", GUILayout.Width(64));
                                                        GUILayout.Label(tasks.stage.ToString().ToUpper(), s, GUILayout.Width(64));
                                                    }
                                                    GUILayout.EndVertical();

                                                    GUILayout.Space(8);

                                                    GUILayout.BeginVertical();
                                                    {
                                                        s.normal.background = Core.tBlue;
                                                        GUILayout.Label("Priority", GUILayout.Width(64));
                                                        if (tasks.priority == Task.Priority.urgent) s.normal.background = Core.tRedDark;
                                                        GUILayout.Label(tasks.priority.ToString().ToUpper(), s, GUILayout.Width(64));
                                                    }
                                                    GUILayout.EndVertical();

                                                    GUILayout.Space(8);

                                                    GUILayout.BeginVertical();
                                                    {
                                                        s.normal.background = Core.tBlue;
                                                        double dd = Math.Floor(DateTime.Now.Subtract(new DateTime(tasks.dueDateDT)).TotalDays);
                                                        string sDue = "NOT SET";
                                                        if (dd < 0)
                                                        {
                                                            sDue = Math.Abs(dd).ToString() + " DAYS";
                                                        }
                                                        else if (dd > 0)
                                                        {
                                                            s.normal.background = Core.tRedDark;
                                                            sDue = "OVERDUE";
                                                        }
                                                        else
                                                        {
                                                            sDue = "TODAY";
                                                        }

                                                        GUILayout.Label("Due in", GUILayout.Width(64));
                                                        GUILayout.Label(sDue, s, GUILayout.Width(64));
                                                    }
                                                    GUILayout.EndVertical();

                                                    GUILayout.Space(8);

                                                    GUILayout.BeginVertical();
                                                    {
                                                        s.normal.background = Core.tBlue;
                                                        GUILayout.Label("Timer", GUILayout.Width(64));
                                                        text = "0m";
                                                        if (tasks.autoTimer)
                                                        {
                                                            long am = tasks.activeMinutes;

                                                            if (am < 10080) text = am / 1440 + "d " + am % 1440 / 60 + "h " + am % 60 + "m";
                                                            if (am < 1440) text = am / 60 + "h " + am % 60 + "m";
                                                            if (am < 60) text = am + "m";
                                                        }

                                                        GUILayout.Label(text, s, GUILayout.Width(64));
                                                    }
                                                    GUILayout.EndVertical();
                                                    GUILayout.FlexibleSpace();
                                                }
                                                GUILayout.EndHorizontal();

                                                GUILayout.Space(4);
                                                GUILayout.BeginHorizontal();
                                                {
                                                    GUILayout.Space(8);
                                                    s = StyleCheck("label");
                                                    s.normal.background = tasks.tColorDark;
                                                    s.padding = new RectOffset(10, 10, 10, 10);
                                                    s.wordWrap = true;
                                                    GUILayout.TextArea(tasks.description, s);
                                                    GUILayout.Space(4);
                                                }
                                                GUILayout.EndHorizontal();
                                                GUILayout.Space(8);

                                                s = StyleCheck();


                                                s.normal.background = Core.MakeTex(1, 1, new Color32(255, 255, 255, 0));
                                                s.fontSize = 20;
                                                s.normal.textColor = Core.cBackgroundPanels;
                                                s.hover.textColor = Color.black;
                                                s.alignment = TextAnchor.MiddleCenter;

                                                if (GUI.Button(new Rect(lastRect.x + lastRect.width - 36, lastRect.y + lastRect.height - 100, 32, 32), "▶", s))
                                                {
                                                    tasks.isFolded = !tasks.isFolded;
                                                }

                                                //if (GUI.Button(new Rect(lastRect.x + lastRect.width - 40, lastRect.y + lastRect.height - 24, 32, 32), Core.tEditPen))
                                                //{
                                                //    tasks.isFolded = !tasks.isFolded;
                                                //}
                                            }
                                            else
                                            {
                                                s = StyleCheck();

                                                s.alignment = TextAnchor.MiddleLeft;
                                                s.fontSize = 16;
                                                s.fontStyle = FontStyle.Bold;

                                                if (tasks.color.r > .5f || tasks.color.g > .5f || tasks.color.b > .5f)
                                                {
                                                    s.normal.textColor = Color.black;
                                                }
                                                else
                                                {
                                                    s.normal.textColor = Color.white;
                                                }

                                                sBackground = StyleBack(sBackground, tasks.tColor50);

                                                GUILayout.BeginHorizontal(GUILayout.Height(96));
                                                {
                                                    GUILayout.Space(8);
                                                    //GUILayout.BeginVertical(sBackground, GUILayout.Width(64), GUILayout.Height(64));
                                                    //{
                                                    //    GUILayout.Space(8);
                                                    //    GUILayout.BeginHorizontal();
                                                    //    {
                                                    //        GUILayout.FlexibleSpace();
                                                    //        if (tasks.isSticky)
                                                    //        {
                                                    //            GUILayout.Button(Core.tTaskSticky, GUIStyle.none, GUILayout.Width(48), GUILayout.Height(48));
                                                    //        }
                                                    //        else
                                                    //        {
                                                    //            GUILayout.Button(Core.tIconTask, GUIStyle.none, GUILayout.Width(48), GUILayout.Height(48));
                                                    //        }
                                                    //        GUILayout.FlexibleSpace();
                                                    //    }
                                                    //    GUILayout.EndHorizontal();
                                                    //    //GUILayout.Label("Bottom");
                                                    //}
                                                    //GUILayout.EndVertical();
                                                    //GUILayout.Space(8);

                                                    GUILayout.BeginHorizontal(); GUILayout.EndHorizontal();

                                                    lastRect = GUILayoutUtility.GetLastRect();

                                                    GUI.DrawTexture(new Rect(lastRect.x, lastRect.y, screenWidth - 32, 96), tasks.tColor50);
                                                    GUI.DrawTexture(new Rect(lastRect.x, lastRect.y, p, 96), tasks.tColor);

                                                    GUILayout.BeginVertical(sNone);
                                                    {

                                                        //GUILayout.FlexibleSpace();
                                                        GUIStyle ss = StyleCheck();
                                                        ss.normal.background = tasks.tColor50;
                                                        GUILayout.BeginHorizontal(ss, GUILayout.Height(32));
                                                        {
                                                            text = tasks.name;
                                                            if (text.Length > 65) text = text.Substring(0, 63) + "...";
                                                            GUILayout.Label(" " + text, s);


                                                            sBackground = StyleBack(sBackground, tasks.tColor50);

                                                            { GUILayout.EndHorizontal(); GUILayout.FlexibleSpace(); GUILayout.BeginHorizontal(); }

                                                            #region ZoomToTaskOnSelection
                                                            if (Event.current.type == EventType.Repaint && scene.zoomToTaskID == tasks.createdDate)
                                                            {
                                                                scene.scrollPosLandmarkDetailTags = new Vector2(0, rID.y);
                                                                scene.zoomToTaskID = "";
                                                            }
                                                            #endregion

                                                            #region Progress Stage Priority
                                                            s.fontSize = 12;
                                                            s.fontStyle = FontStyle.Normal;

                                                            //if (EditorGUIUtility.isProSkin)
                                                            //{
                                                            //    sBackground = StyleBack(sBackground, Core.tGreyDark);
                                                            //}
                                                            //else
                                                            //{
                                                            //    sBackground = StyleBack(sBackground, Core.tGreyBright);
                                                            //}

                                                            //GUILayout.Label( s);
                                                            if (tasks.subTasksAutoProgress) GUI.enabled = false;
                                                            GUILayout.Space(8);

                                                            //checksum += tasks.progress.ToString();
                                                            GUILayout.Label("Progress: ", s);
                                                            if (tasks.subTasksAutoProgress)
                                                            {
                                                                GUILayout.Label("Subtask Auto Progress Enabled", s);
                                                                GUILayout.FlexibleSpace();
                                                            }
                                                            else
                                                            {
                                                                var prevProg = tasks.progress;
                                                                tasks.progress = EditorGUILayout.IntSlider(tasks.progress, 0, 100);
                                                                if (prevProg != tasks.progress)
                                                                {
                                                                    scene.UpdateAllProgress(scene.stickyFont);
                                                                    //QueueRepaint();
                                                                    Core.SaveData();
                                                                    //window.Repaint();
                                                                }
                                                            }
                                                            //GUILayout.FlexibleSpace();
                                                            //checksum2 += tasks.progress.ToString();
                                                            if (tasks.subTasksAutoProgress) GUI.enabled = true;
                                                            GUILayout.Space(8);

                                                            { GUILayout.EndHorizontal(); GUILayout.Space(8); GUILayout.BeginHorizontal(GUILayout.Height(16)); }
                                                            GUILayout.Space(8);
                                                            GUILayout.Label("Stage: ", s);//, GUILayout.Width(64));

                                                            //checksum += tasks.stage.ToString();
                                                            var prevStage = tasks.stage;
                                                            tasks.stage = (Task.Stage)EditorGUILayout.EnumPopup(tasks.stage, GUILayout.Width(80));
                                                            if (prevStage != tasks.stage) landmarks.UpdateProgress(scene.stickyFont);
                                                            //checksum2 += tasks.stage.ToString();
                                                            //
                                                            //if (screenWidth < 300) { GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(sBackground); }
                                                            GUILayout.FlexibleSpace();
                                                            GUILayout.Label("Priority: ", s);//, GUILayout.Width(64));
                                                                                             //checksum += tasks.priority.ToString();
                                                            var prevPriority = tasks.priority;
                                                            tasks.priority = (Task.Priority)EditorGUILayout.EnumPopup(tasks.priority, GUILayout.Width(80));
                                                            if (prevPriority != tasks.priority) landmarks.UpdateProgress(scene.stickyFont);
                                                            //checksum2 += tasks.priority.ToString();
                                                            //GUILayout.Space(32);
                                                            GUILayout.FlexibleSpace();
                                                            GUILayout.Label("Color:", s);
                                                            //checksum += tasks.color;
                                                            var prevColor = tasks.color;
                                                            tasks.SetColor(EditorGUILayout.ColorField(tasks.color, GUILayout.Width(50)));
                                                            if (prevColor != tasks.color) landmarks.UpdateProgress(scene.stickyFont);
                                                            GUILayout.Space(8);
                                                            //GUILayout.FlexibleSpace();
                                                            #endregion


                                                        }
                                                        GUILayout.EndHorizontal();
                                                        GUILayout.FlexibleSpace();
                                                    }
                                                    GUILayout.EndVertical();

                                                    GUILayout.Space(8);


                                                }
                                                GUILayout.EndHorizontal();
                                                Rect foldRect = GUILayoutUtility.GetLastRect();

                                                GUILayout.Space(8);
                                                GUIStyle menuStyle = null;

                                                GUILayout.BeginHorizontal(GUILayout.Height(64));
                                                {
                                                    GUILayout.Space(8);

                                                    GUILayout.BeginVertical(sBackground, GUILayout.Width(64));
                                                    {
                                                        tasks.selectedMenuScrollBar = GUILayout.BeginScrollView(tasks.selectedMenuScrollBar, GUIStyle.none, GUIStyle.none);//, GUILayout.Height(232), GUILayout.Width(screenWidth-116));
                                                        {
                                                            //GUILayout.Space(8);
                                                            if (tasks.selectedMenu == Task.SelectedMenu.notes) { menuStyle = StyleBack(menuStyle, Core.tBackgroundMain); } else { menuStyle = StyleBack(menuStyle, tasks.tColorDark); }
                                                            GUILayout.BeginVertical(menuStyle, GUILayout.Height(46));
                                                            GUILayout.FlexibleSpace();
                                                            GUILayout.BeginHorizontal();
                                                            {
                                                                GUILayout.FlexibleSpace();
                                                                if (GUILayout.Button(Core.tTaskSticky, GUIStyle.none, GUILayout.Width(32), GUILayout.Height(32)))
                                                                {
                                                                    tasks.selectedMenu = Task.SelectedMenu.notes;
                                                                }
                                                                GUILayout.FlexibleSpace();
                                                            }
                                                            GUILayout.EndHorizontal();
                                                            GUILayout.FlexibleSpace();
                                                            GUILayout.EndVertical();
                                                            //GUILayout.Space(8);
                                                            if (tasks.selectedMenu == Task.SelectedMenu.subtasks) { menuStyle = StyleBack(menuStyle, Core.tBackgroundMain); } else { menuStyle = StyleBack(menuStyle, tasks.tColorDark); }
                                                            GUILayout.BeginVertical(menuStyle, GUILayout.Height(46));
                                                            GUILayout.FlexibleSpace();
                                                            GUILayout.BeginHorizontal();
                                                            {
                                                                GUILayout.FlexibleSpace();
                                                                if (GUILayout.Button(Core.tIconTask, GUIStyle.none, GUILayout.Width(32), GUILayout.Height(32)))
                                                                {
                                                                    tasks.selectedMenu = Task.SelectedMenu.subtasks;
                                                                }
                                                                GUILayout.FlexibleSpace();
                                                            }
                                                            GUILayout.EndHorizontal();
                                                            GUILayout.FlexibleSpace();
                                                            GUILayout.EndVertical();
                                                            //GUILayout.Space(8);
                                                            if (tasks.selectedMenu == Task.SelectedMenu.duedate) { menuStyle = StyleBack(menuStyle, Core.tBackgroundMain); } else { menuStyle = StyleBack(menuStyle, tasks.tColorDark); }
                                                            GUILayout.BeginVertical(menuStyle, GUILayout.Height(46));
                                                            GUILayout.FlexibleSpace();
                                                            GUILayout.BeginHorizontal();
                                                            {
                                                                GUILayout.FlexibleSpace();
                                                                if (GUILayout.Button(Core.tDate, GUIStyle.none, GUILayout.Width(32), GUILayout.Height(32)))
                                                                {
                                                                    tasks.selectedMenu = Task.SelectedMenu.duedate;
                                                                }
                                                                GUILayout.FlexibleSpace();
                                                            }
                                                            GUILayout.EndHorizontal();
                                                            GUILayout.FlexibleSpace();
                                                            GUILayout.EndVertical();
                                                            //GUILayout.Space(8);
                                                            if (tasks.selectedMenu == Task.SelectedMenu.sticky) { menuStyle = StyleBack(menuStyle, Core.tBackgroundMain); } else { menuStyle = StyleBack(menuStyle, tasks.tColorDark); }
                                                            GUILayout.BeginVertical(menuStyle, GUILayout.Height(46));
                                                            GUILayout.FlexibleSpace();
                                                            GUILayout.BeginHorizontal();
                                                            {
                                                                GUILayout.FlexibleSpace();
                                                                if (GUILayout.Button(Core.tIconPin, GUIStyle.none, GUILayout.Width(32), GUILayout.Height(32)))
                                                                {
                                                                    tasks.selectedMenu = Task.SelectedMenu.sticky;
                                                                }
                                                                GUILayout.FlexibleSpace();
                                                            }
                                                            GUILayout.EndHorizontal();
                                                            GUILayout.FlexibleSpace();
                                                            GUILayout.EndVertical();
                                                            //GUILayout.Space(8);
                                                            if (tasks.selectedMenu == Task.SelectedMenu.timer) { menuStyle = StyleBack(menuStyle, Core.tBackgroundMain); } else { menuStyle = StyleBack(menuStyle, tasks.tColorDark); }
                                                            GUILayout.BeginVertical(menuStyle, GUILayout.Height(46));
                                                            GUILayout.FlexibleSpace();
                                                            GUILayout.BeginHorizontal();
                                                            {
                                                                GUILayout.FlexibleSpace();
                                                                if (GUILayout.Button(Core.tSortDate, GUIStyle.none, GUILayout.Width(32), GUILayout.Height(32)))
                                                                {
                                                                    tasks.selectedMenu = Task.SelectedMenu.timer;
                                                                }
                                                                GUILayout.FlexibleSpace();
                                                            }
                                                            GUILayout.EndHorizontal();
                                                            GUILayout.FlexibleSpace();
                                                            GUILayout.EndVertical();
                                                            //GUILayout.Space(8);
                                                        }
                                                        GUILayout.EndScrollView();
                                                    }
                                                    GUILayout.EndVertical();

                                                    sBackground = StyleBack(sBackground, Core.tBackgroundMain);
                                                    GUILayout.BeginVertical(sBackground, GUILayout.Height(230));
                                                    {
                                                        switch (tasks.selectedMenu)
                                                        {
                                                            case Task.SelectedMenu.notes:

                                                                //GUILayout.Label("Task Label (Keep Short): ");
                                                                //GUILayout.BeginHorizontal(sBackground);
                                                                //{
                                                                //    // s = GetTextAreaStyle(tasks.name, screenWidth - 32);
                                                                //    //checksum += tasks.name;
                                                                //    tasks.name = EditorGUILayout.TextField(tasks.name, GUILayout.Width(120));
                                                                //    //checksum2 += tasks.name;
                                                                //}
                                                                //GUILayout.EndHorizontal();
                                                                tasks.selectedMenuNotesScrollBar = GUILayout.BeginScrollView(tasks.selectedMenuNotesScrollBar, GUIStyle.none);//, GUILayout.Height(232), GUILayout.Width(screenWidth-116));
                                                                {
                                                                    GUILayout.Label("Task Label (Keep Short): ");
                                                                    GUILayout.BeginHorizontal(sBackground);
                                                                    {
                                                                        // s = GetTextAreaStyle(tasks.name, screenWidth - 32);
                                                                        //checksum += tasks.name;
                                                                        string prev1 = tasks.name;
                                                                        tasks.name = EditorGUILayout.TextField(tasks.name, GUILayout.Width(180));
                                                                        if (prev1 != tasks.name) landmarks.UpdateProgress(scene.stickyFont);
                                                                        //checksum2 += tasks.name;
                                                                    }
                                                                    GUILayout.EndHorizontal();

                                                                    GUILayout.Label("Description: ");
                                                                    GUILayout.BeginHorizontal(sBackground);
                                                                    {
                                                                        s = GetTextAreaStyle(tasks.description, screenWidth - 128);
                                                                        //checksum += tasks.description;
                                                                        string prev1 = tasks.description;
                                                                        tasks.description = EditorGUILayout.TextField(tasks.description, s, GUILayout.Width(screenWidth - 132), GUILayout.Height(s.fixedHeight));
                                                                        if (prev1 != tasks.description) landmarks.UpdateProgress(scene.stickyFont);
                                                                        //checksum2 += tasks.description;
                                                                    }
                                                                    GUILayout.EndHorizontal();
                                                                }
                                                                GUILayout.EndScrollView();

                                                                break;
                                                            case Task.SelectedMenu.subtasks:
                                                                var prevAuto = tasks.subTasksAutoProgress;
                                                                tasks.subTasksAutoProgress = GUILayout.Toggle(tasks.subTasksAutoProgress, "Auto Progress", "Button");
                                                                if (prevAuto != tasks.subTasksAutoProgress) scene.UpdateAllProgress(scene.stickyFont);

                                                                if (GUILayout.Button("New Subtask"))
                                                                {
                                                                    tasks.subTasks.Add(new SubTask("New Subtask"));
                                                                    scene.UpdateAllProgress(scene.stickyFont);
                                                                }
                                                                tasks.selectedMenuSubTasksScrollBar = GUILayout.BeginScrollView(tasks.selectedMenuSubTasksScrollBar, GUIStyle.none);//, GUILayout.Height(232), GUILayout.Width(screenWidth-116));
                                                                {
                                                                    sBackground = StyleBack(sBackground, Core.tGreyDark);

                                                                    GUILayout.BeginHorizontal();
                                                                    {
                                                                        for (int st = 0; st < tasks.subTasks.Count; st++)
                                                                        {
                                                                            //if (tasks.subTasks[st].complete)
                                                                            //{
                                                                            //    sBackground = StyleBack(sBackground, Core.tTealDark);
                                                                            //}
                                                                            //else
                                                                            //{
                                                                            //    sBackground = StyleBack(sBackground, Core.tGreyDark);
                                                                            //}
                                                                            s = StyleCheck("textfield");
                                                                            s = GetTextAreaStyle(tasks.subTasks[st].name, screenWidth - 180);
                                                                            GUILayout.BeginVertical(GUILayout.Height(s.fixedHeight));
                                                                            {
                                                                                GUILayout.Space(4);
                                                                                GUILayout.BeginHorizontal();
                                                                                {
                                                                                    var prevComplete = tasks.subTasks[st].complete;
                                                                                    GUILayout.Space(8);
                                                                                    tasks.subTasks[st].complete = GUILayout.Toggle(tasks.subTasks[st].complete, "", GUILayout.Width(24));
                                                                                    if (prevComplete != tasks.subTasks[st].complete)
                                                                                    {
                                                                                        scene.UpdateAllProgress(scene.stickyFont);
                                                                                        QueueRepaint();
                                                                                        //landmarks.UpdateProgress(scene.stickyFont);
                                                                                        Core.SaveData();
                                                                                        window.Repaint();
                                                                                    }
                                                                                    //s = GetTextAreaStyle(tasks.description, screenWidth - 32);
                                                                                    //checksum += tasks.description;
                                                                                    //tasks.description = EditorGUILayout.TextField(tasks.description, s, GUILayout.Width(screenWidth - 32), GUILayout.Height(s.fixedHeight));

                                                                                    var prevST = tasks.subTasks[st].name;
                                                                                    tasks.subTasks[st].name = EditorGUILayout.TextField(tasks.subTasks[st].name, s, GUILayout.Width(screenWidth - 180), GUILayout.Height(s.fixedHeight));
                                                                                    if (prevST != tasks.subTasks[st].name) landmarks.UpdateProgress(scene.stickyFont);

                                                                                    GUILayout.Space(4);
                                                                                    if (GUILayout.Button(Core.tTrashCan, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(16)))
                                                                                    {
                                                                                        tasks.subTasks.RemoveAt(st);
                                                                                        scene.UpdateAllProgress(scene.stickyFont);
                                                                                        landmarks.UpdateProgress(scene.stickyFont);
                                                                                        QueueRepaint();
                                                                                        Core.SaveData();
                                                                                    }
                                                                                    GUILayout.Space(8);
                                                                                }
                                                                                GUILayout.EndHorizontal();
                                                                            }
                                                                            GUILayout.EndVertical();
                                                                            GUILayout.EndHorizontal();
                                                                            HorizontalLine(new Color(0, 0, 0, 1));
                                                                            GUILayout.BeginHorizontal();
                                                                        }
                                                                        //if (EditorGUI.EndChangeCheck())
                                                                        //{
                                                                        //    Core.SaveData();
                                                                        //}
                                                                    }
                                                                    GUILayout.EndHorizontal();


                                                                }
                                                                GUILayout.EndScrollView();
                                                                break;
                                                            case Task.SelectedMenu.duedate:
                                                                //Debug.Log(tasks.selectedDate.Month);
                                                                DateTime prevMonth;
                                                                int firstDay, daysPrevMonth, daysMonth;
                                                                float buttonWidth = (screenWidth - 160) / 7;
                                                                void UpdatePrev()
                                                                {
                                                                    int d = tasks.selectedDate.Day;
                                                                    int m = tasks.selectedDate.Month;
                                                                    int pm = tasks.selectedDate.Month - 1;
                                                                    int y = tasks.selectedDate.Year;
                                                                    firstDay = (int)new DateTime(y, m, 1).DayOfWeek;
                                                                    daysMonth = DateTime.DaysInMonth(y, m);

                                                                    if (pm < 1) { pm = 12; y--; }
                                                                    prevMonth = new DateTime(y, pm, 1);
                                                                    daysPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

                                                                }

                                                                if (tasks.selectedDate.Month < 1 || tasks.selectedDate.Month > 12) tasks.selectedDate = DateTime.Now;
                                                                UpdatePrev();



                                                                tasks.selectedMenuDueDateScrollBar = GUILayout.BeginScrollView(tasks.selectedMenuDueDateScrollBar, GUIStyle.none);//, GUILayout.Height(232), GUILayout.Width(screenWidth-116));
                                                                {

                                                                    //if (tasks.alert)
                                                                    //DateTime selectedDate = DateTime.Now;
                                                                    double dd = Math.Floor(DateTime.Now.Subtract(new DateTime(tasks.dueDateDT)).TotalDays);
                                                                    GUILayout.BeginHorizontal();
                                                                    {
                                                                        s = StyleCheck("Label");
                                                                        s.fontSize = 16; s.fontStyle = FontStyle.Bold;
                                                                        GUILayout.FlexibleSpace();
                                                                        GUILayout.Space(16);
                                                                        GUILayout.Label(tasks.selectedDate.ToString("yyyy"), s);
                                                                        GUILayout.FlexibleSpace();
                                                                        s.alignment = TextAnchor.MiddleCenter;
                                                                        GUILayout.Label(tasks.selectedDate.ToString("MMMM"), s, GUILayout.Width(128));
                                                                        GUILayout.FlexibleSpace();
                                                                        if (GUILayout.Button("◄"))
                                                                        {
                                                                            int y, m, d;
                                                                            y = tasks.selectedDate.Year;
                                                                            m = tasks.selectedDate.Month;
                                                                            d = tasks.selectedDate.Day;

                                                                            m--;
                                                                            //Debug.Log(y + "/" + m + "/" + d);
                                                                            if (m < 1) { m = 12; y--; }
                                                                            tasks.selectedDate = new DateTime(y, m, tasks.selectedDate.Day);
                                                                            UpdatePrev();
                                                                        }
                                                                        if (GUILayout.Button("►"))
                                                                        {
                                                                            int y, m, d;
                                                                            y = tasks.selectedDate.Year;
                                                                            m = tasks.selectedDate.Month;
                                                                            d = tasks.selectedDate.Day;

                                                                            m++;
                                                                            //Debug.Log(y + "/" + m + "/" + d);
                                                                            if (m > 12) { m = 1; y++; }
                                                                            tasks.selectedDate = new DateTime(y, m, tasks.selectedDate.Day);
                                                                            UpdatePrev();
                                                                        }
                                                                        GUILayout.Space(16);
                                                                        GUILayout.FlexibleSpace();
                                                                    }
                                                                    GUILayout.EndHorizontal();

                                                                    GUILayout.BeginHorizontal();
                                                                    {
                                                                        GUILayout.FlexibleSpace();
                                                                        s = StyleCheck();
                                                                        s.alignment = TextAnchor.MiddleCenter;
                                                                        GUILayout.Label("Su", s, GUILayout.Width(buttonWidth), GUILayout.Height(24));
                                                                        GUILayout.Space(3);
                                                                        GUILayout.Label("Mo", s, GUILayout.Width(buttonWidth), GUILayout.Height(24));
                                                                        GUILayout.Space(3);
                                                                        GUILayout.Label("Tu", s, GUILayout.Width(buttonWidth), GUILayout.Height(24));
                                                                        GUILayout.Space(3);
                                                                        GUILayout.Label("We", s, GUILayout.Width(buttonWidth), GUILayout.Height(24));
                                                                        GUILayout.Space(3);
                                                                        GUILayout.Label("Th", s, GUILayout.Width(buttonWidth), GUILayout.Height(24));
                                                                        GUILayout.Space(3);
                                                                        GUILayout.Label("Fr", s, GUILayout.Width(buttonWidth), GUILayout.Height(24));
                                                                        GUILayout.Space(3);
                                                                        GUILayout.Label("Sa", s, GUILayout.Width(buttonWidth), GUILayout.Height(24));
                                                                        GUILayout.FlexibleSpace();
                                                                    }
                                                                    GUILayout.EndHorizontal();

                                                                    GUILayout.BeginHorizontal();
                                                                    int count = daysPrevMonth - firstDay + 1;
                                                                    int month = tasks.selectedDate.Month - 1;
                                                                    if (month == 0) month = 12;
                                                                    int year = tasks.selectedDate.Year - 1;
                                                                    for (int week = 0; week < 6; week++)
                                                                    {
                                                                        GUILayout.FlexibleSpace();
                                                                        for (int day = 0; day < 7; day++, count++)
                                                                        {
                                                                            //(day + (week * 7)).ToString()
                                                                            if (count > daysPrevMonth || (count > daysMonth && daysPrevMonth == 99)) { count = 1; daysPrevMonth = 99; month++; if (month == 13) month = 1; }
                                                                            if (month > 1 && year == tasks.selectedDate.Year - 1) { year++; }
                                                                            s = StyleCheck("button");
                                                                            //Debug.Log(prevMonth.ToString("MMMM") + "  " + daysPrevMonth + " ---  " + tasks.selectedDate.Year + "/" + month + "/" + count);
                                                                            long ticks = new DateTime(year, month, count).Ticks;
                                                                            //Debug.Log(new DateTime(ticks) + "  /  " + new DateTime(tasks.dueDateDT));
                                                                            s.normal.background = Core.tGreyDark;
                                                                            if (tasks.alert)
                                                                            {
                                                                                double daysdiff = 255 - Math.Abs(new DateTime(ticks).Subtract(new DateTime(tasks.dueDateDT)).TotalDays);
                                                                                //Debug.Log(255 - Math.Abs(daysdiff));
                                                                                if (ticks < tasks.dueDateDT && new DateTime(year, month, count).Ticks >= DateTime.Now.Ticks) s.normal.background = Core.MakeTex(1, 1, new Color(0, Mathf.Lerp((float)daysdiff / 510, 1, 0.01f), 0));
                                                                                if (ticks > tasks.dueDateDT) s.normal.background = Core.MakeTex(1, 1, new Color(Mathf.Lerp((float)daysdiff / 510, 1, 0.01f), 0, 0));
                                                                                if (ticks == tasks.dueDateDT)
                                                                                {
                                                                                    s.normal.background = Core.tBlack;
                                                                                }
                                                                            }
                                                                            if (month != tasks.selectedDate.Month)
                                                                            {
                                                                                GUI.enabled = false;
                                                                            }
                                                                            if (GUILayout.Button(count.ToString(), s, GUILayout.Width(buttonWidth), GUILayout.Height(24)))
                                                                            {
                                                                                DateTime date = DateTime.Now;
                                                                                if (daysPrevMonth < 99) { date = new DateTime(prevMonth.Year, month, count); } else { date = new DateTime(tasks.selectedDate.Year, month, count); }
                                                                                if (daysPrevMonth == 99 && count > daysMonth)
                                                                                {
                                                                                    if (month == 12)
                                                                                    {
                                                                                        month = 1; year++;
                                                                                    }//Debug.Log("Count: " + count); }
                                                                                    date = new DateTime(year, month, count);
                                                                                }
                                                                                if (date.Ticks == tasks.dueDateDT) tasks.alert = !tasks.alert; else tasks.alert = true;
                                                                                tasks.UpdateDueDate(date);
                                                                                landmarks.UpdateProgress(scene.stickyFont);
                                                                            }
                                                                            if (month != tasks.selectedDate.Month) GUI.enabled = true;
                                                                        }
                                                                        GUILayout.FlexibleSpace();
                                                                        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                                                    }
                                                                    GUILayout.EndHorizontal();
                                                                    GUILayout.BeginHorizontal();
                                                                    GUILayout.FlexibleSpace();
                                                                    s = StyleCheck("button");
                                                                    if (tasks.alert)
                                                                    {
                                                                        if (dd < 0)
                                                                        {
                                                                            s.normal.background = Core.tGreenBright;
                                                                            GUILayout.Label(Math.Abs(dd).ToString() + " DAYS REMAINING", s, GUILayout.Width(buttonWidth * 7 + 16), GUILayout.Height(16));
                                                                        }
                                                                        else if (dd > 0)
                                                                        {
                                                                            s.normal.background = Core.tRedDark;
                                                                            GUILayout.Label(Math.Abs(dd).ToString() + " DAYS OVERDUE", s, GUILayout.Width(buttonWidth * 7 + 8), GUILayout.Height(16));
                                                                        }
                                                                        else
                                                                        {
                                                                            s.normal.background = Core.tBlack;
                                                                            GUILayout.Label("DUE TODAY", s, GUILayout.Width(buttonWidth * 7 + 8), GUILayout.Height(16));
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        s.normal.background = Core.tBlack;
                                                                        GUILayout.Label("CHOOSE A DATE TO ENABLE", s, GUILayout.Width(buttonWidth * 7 + 8), GUILayout.Height(16));
                                                                    }
                                                                    GUILayout.FlexibleSpace();
                                                                    GUILayout.EndHorizontal();

                                                                    //EditorGUI.BeginChangeCheck();
                                                                    ////checksum += tasks.month.ToString();
                                                                    //tasks.month = (Task.Month)EditorGUILayout.EnumPopup(tasks.month);
                                                                    ////checksum2 += tasks.month.ToString();
                                                                    //List<int> o = new List<int>();
                                                                    //List<string> slist = new List<string>();
                                                                    //if (tasks.year == 0) tasks.year = DateTime.Now.Year;
                                                                    //int x = (int)tasks.month + 1;
                                                                    //if (x > 12) x = 1;
                                                                    //for (int ii = 0; ii < DateTime.DaysInMonth(tasks.year, x); ii++)
                                                                    //{
                                                                    //    o.Add(ii + 1);
                                                                    //    slist.Add((ii + 1).ToString());
                                                                    //}
                                                                    ////checksum += tasks.day.ToString();
                                                                    //tasks.day = EditorGUILayout.IntPopup(tasks.day, slist.ToArray(), o.ToArray());
                                                                    ////checksum2 += tasks.day.ToString();
                                                                    //o = new List<int>();
                                                                    //slist = new List<string>();
                                                                    //for (int ii = DateTime.Now.Year; ii < DateTime.Now.Year + 10; ii++)
                                                                    //{
                                                                    //    o.Add(ii);
                                                                    //    slist.Add(ii.ToString());
                                                                    //}
                                                                    ////checksum += tasks.year.ToString();
                                                                    //tasks.year = EditorGUILayout.IntPopup(tasks.year, slist.ToArray(), o.ToArray());
                                                                    ////checksum2 += tasks.year.ToString();
                                                                    //if (EditorGUI.EndChangeCheck())
                                                                    //{
                                                                    //tasks.UpdateDueDate();
                                                                    //}

                                                                }
                                                                GUILayout.EndScrollView();
                                                                break;
                                                            case Task.SelectedMenu.sticky:
                                                                void StickyTaskReposition()
                                                                {
                                                                    tasks.position = Core.GetSVCameraPosition();
                                                                    if (SceneView.lastActiveSceneView.in2DMode) tasks.position = new Vector3(tasks.position.x, tasks.position.y, tasks.position.z + 10);
                                                                }
                                                                //
                                                                if (!tasks.isSticky)
                                                                {
                                                                    if (GUILayout.Button("Show Sticky Task"))
                                                                    {
                                                                        tasks.isSticky = true;
                                                                        if (tasks.position == Vector3.zero)
                                                                            StickyTaskReposition();
                                                                        landmarks.UpdateProgress(scene.stickyFont);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (GUILayout.Button("Hide Sticky Task"))
                                                                    {
                                                                        tasks.isSticky = false;
                                                                        landmarks.UpdateProgress(scene.stickyFont);
                                                                    }
                                                                }
                                                                tasks.selectedMenuStickyScrollBar = GUILayout.BeginScrollView(tasks.selectedMenuStickyScrollBar, GUIStyle.none);//, GUILayout.Height(232), GUILayout.Width(screenWidth-116));
                                                                {
                                                                    sBackground = StyleBack(sBackground, Core.tGreyBright);
                                                                    string dt = tasks.createdDate;

                                                                    if (!tasks.isSticky)
                                                                    {
                                                                        GUI.enabled = false;
                                                                    }
                                                                    sBackground = StyleBack(sBackground, Core.tBlueDark);
                                                                    GUILayout.BeginHorizontal(sBackground);
                                                                    {
                                                                        s = StyleCheck("button");
                                                                        s.fontSize = 8;
                                                                        if (GUILayout.Button("RESET", s, GUILayout.Width(64))) { StickyTaskReposition(); }
                                                                        GUILayout.FlexibleSpace();
                                                                        GUILayout.Label("Position: " + tasks.position);
                                                                        GUILayout.FlexibleSpace();
                                                                        if (GUILayout.Button("GO", s, GUILayout.Width(24)))
                                                                        {
                                                                            if (localData.viewState == Landmark.ViewState.is2D)
                                                                            {
                                                                                scene.atlas.prevPosition = tasks.position;
                                                                                scene.atlas.landmarkSelected = -1;

                                                                                TaskAtlasSceneView.EndAtlasMode(tasks.position, tasks.position, false, false);
                                                                                ZoomToLocation(scene.selectedLandmark, i);
                                                                            }
                                                                            else
                                                                            {
                                                                                scene.atlas.prevPosition = tasks.position + (Vector3.forward * -10);
                                                                                scene.atlas.landmarkSelected = -1;

                                                                                TaskAtlasSceneView.EndAtlasMode(tasks.position + (Vector3.forward * ((tasks.scale / 4))), tasks.position, false);
                                                                            }
                                                                        }

                                                                        //GUILayout.FlexibleSpace();


                                                                        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                                                        if (localData.viewState != Landmark.ViewState.is2D)
                                                                        {
                                                                            GUILayout.Label("Scale Factor");
                                                                            GUILayout.FlexibleSpace();
                                                                            GUILayout.Label("Max:");
                                                                            scene.stickyScaleMax = EditorGUILayout.FloatField(scene.stickyScaleMax, GUILayout.Width(48));
                                                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();


                                                                            tasks.scale = EditorGUILayout.Slider(tasks.scale, 1, scene.stickyScaleMax);

                                                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                                                            s = new GUIStyle("button");

                                                                            tasks.useDefaultFadeDistance = GUILayout.Toggle(tasks.useDefaultFadeDistance, "Use Default Pop In/Out Distance");
                                                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                                                            if (tasks.useDefaultFadeDistance)
                                                                            {
                                                                                s = StyleCheck("label");
                                                                                s.fontSize = 10;
                                                                                GUILayout.Label("(Default Pop In/Out set in Landmark Tab)", s);

                                                                                //GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                                                                //GUI.enabled = false;
                                                                                //EditorGUILayout.MinMaxSlider(ref localData.taskFadeStart, ref localData.taskFadeEnd, 0f, 1000f);
                                                                                //GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                                                                //s = new GUIStyle("textfield");
                                                                                //GUILayout.Label("Fade Start at:");
                                                                                //GUILayout.Label(localData.taskFadeStart.ToString("F1"), s);
                                                                                //GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                                                                //GUILayout.Label("Fade End at:");
                                                                                //GUILayout.Label(localData.taskFadeEnd.ToString("F1"), s);
                                                                                //GUI.enabled = true;
                                                                            }
                                                                            else
                                                                            {
                                                                                GUILayout.Space(8);
                                                                                EditorGUILayout.MinMaxSlider(ref tasks.fadeStart, ref tasks.fadeEnd, 0f, localData.taskFadeMax);
                                                                                GUILayout.Label("Max:");
                                                                                localData.taskFadeMax = EditorGUILayout.FloatField(localData.taskFadeMax, GUILayout.Width(48));
                                                                                GUILayout.Space(8);
                                                                                GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                                                                s = new GUIStyle("textfield");
                                                                                GUILayout.Label("Fade Start at:");
                                                                                if (GUILayout.Button("Set Here"))
                                                                                {
                                                                                    tasks.fadeStart = Vector3.Distance(Core.GetSVCameraPosition(), tasks.position);
                                                                                    if (tasks.fadeStart > tasks.fadeEnd) tasks.fadeStart = tasks.fadeEnd;
                                                                                }
                                                                                GUILayout.Label(tasks.fadeStart.ToString("F1"), s);
                                                                                GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                                                                //GUILayout.FlexibleSpace();
                                                                                GUILayout.Label("Fade End at:");
                                                                                if (GUILayout.Button("Set Here"))
                                                                                {
                                                                                    tasks.fadeEnd = Vector3.Distance(Core.GetSVCameraPosition(), tasks.position);
                                                                                    if (tasks.fadeEnd < tasks.fadeStart) tasks.fadeStart = tasks.fadeEnd;
                                                                                    if (tasks.fadeEnd > localData.taskFadeMax)
                                                                                    {
                                                                                        if (EditorUtility.DisplayDialog("Fade Distance bigger than Max", "Distance from task is " + tasks.fadeEnd + " but Max Distance is " + localData.taskFadeMax + ".  Set to Max Distance, or cancel?", "Set to " + localData.taskFadeMax, "Cancel"))
                                                                                        {
                                                                                            tasks.fadeEnd = localData.taskFadeMax;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                GUILayout.Label(tasks.fadeEnd.ToString("F1"), s);
                                                                            }


                                                                            s = new GUIStyle("button");
                                                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(sBackground);

                                                                            if (!tasks.isSticky)
                                                                            {
                                                                                GUI.enabled = true;
                                                                            }

                                                                        }

                                                                        GUILayout.EndHorizontal();
                                                                        GUILayout.FlexibleSpace();

                                                                    }
                                                                }
                                                                GUILayout.EndScrollView();
                                                                break;
                                                            case Task.SelectedMenu.timer:
                                                                string n = "Enable";
                                                                if (tasks.autoTimer) n = "Disable";
                                                                if (GUILayout.Button(n))
                                                                {
                                                                    tasks.autoTimer = !tasks.autoTimer;
                                                                }

                                                                tasks.selectedMenuTimerScrollBar = GUILayout.BeginScrollView(tasks.selectedMenuTimerScrollBar, GUIStyle.none);//, GUILayout.Height(232), GUILayout.Width(screenWidth-116));
                                                                {


                                                                    sBackground = StyleBack(sBackground, Core.tGreyDark);

                                                                    GUILayout.BeginVertical(sBackground);
                                                                    {
                                                                        GUI.enabled = tasks.autoTimer;

                                                                        if (tasks.autoTimer)
                                                                        {
                                                                            GUILayout.BeginHorizontal();
                                                                            {
                                                                                GUILayout.FlexibleSpace();
                                                                                GUILayout.Label("Active Time");
                                                                                GUILayout.FlexibleSpace();
                                                                            }
                                                                            GUILayout.EndHorizontal();
                                                                            GUILayout.BeginHorizontal();
                                                                            {
                                                                                s = StyleCheck();
                                                                                s.fontSize = 32;
                                                                                s.fontStyle = FontStyle.Bold;
                                                                                long am = tasks.activeMinutes;

                                                                                if (am < 10080) text = am / 1440 + "d " + am % 1440 / 60 + "h " + am % 60 + "m";
                                                                                if (am < 1440) text = am / 60 + "h " + am % 60 + "m";
                                                                                if (am < 60) text = am + "m";

                                                                                GUILayout.FlexibleSpace();
                                                                                GUILayout.Label(text, s);
                                                                                GUILayout.FlexibleSpace();
                                                                            }
                                                                            GUILayout.EndHorizontal();
                                                                            sBackground = new GUIStyle();
                                                                            if (tasks.isTracking)
                                                                            {
                                                                                sBackground.normal.background = Core.tTeal;
                                                                                text = "TRACKING";
                                                                            }
                                                                            else
                                                                            {
                                                                                text = "OUT OF RANGE";
                                                                                sBackground.normal.background = Core.tGrey;
                                                                            }
                                                                            GUILayout.BeginHorizontal(sBackground);
                                                                            {
                                                                                s = StyleCheck();
                                                                                s.fontSize = 14;
                                                                                s.fontStyle = FontStyle.Bold;
                                                                                GUILayout.FlexibleSpace();
                                                                                GUILayout.Label(text, s);
                                                                                GUILayout.FlexibleSpace();

                                                                                GUILayout.EndHorizontal();
                                                                                GUILayout.BeginHorizontal(sBackground);

                                                                                GUILayout.FlexibleSpace();
                                                                                if (localData.viewState != Landmark.ViewState.is2D)
                                                                                {
                                                                                    GUILayout.Label("Current Distance: " + tasks.timeTrackingDistanceCurrent);
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (tasks.isTracking)
                                                                                    {
                                                                                        GUILayout.Label("Sticky Task is within view!");
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        GUILayout.Label("Sticky Task off screen...");
                                                                                    }
                                                                                }
                                                                                GUILayout.FlexibleSpace();
                                                                            }
                                                                            GUILayout.EndHorizontal();
                                                                            if (localData.viewState != Landmark.ViewState.is2D)
                                                                            {
                                                                                GUILayout.BeginHorizontal();
                                                                                {
                                                                                    GUILayout.Label("Tracking Distance:");
                                                                                    GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                                                                    tasks.timeTrackingDistance = EditorGUILayout.Slider(tasks.timeTrackingDistance, 0.1f, tasks.timeTrackingDistanceScale);
                                                                                    GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                                                                                    GUILayout.Label("Scale:");
                                                                                    tasks.timeTrackingDistanceScale = EditorGUILayout.FloatField(tasks.timeTrackingDistanceScale, GUILayout.Width(48));
                                                                                    GUILayout.FlexibleSpace();
                                                                                    s = new GUIStyle("button");
                                                                                    s.fontSize = 10;
                                                                                    if (tasks.showDistanceSphere)
                                                                                    {
                                                                                        s.normal.background = Core.tGreenBright;
                                                                                        s.normal.textColor = Color.black;
                                                                                    }
                                                                                    tasks.showDistanceSphere = GUILayout.Toggle(tasks.showDistanceSphere, "Show Time Ball", s, GUILayout.Height(20));
                                                                                }
                                                                                GUILayout.EndHorizontal();
                                                                            }
                                                                        }

                                                                        GUILayout.BeginHorizontal();
                                                                        {
                                                                            if (GUILayout.Button("Reset Timers"))
                                                                            {
                                                                                if (EditorUtility.DisplayDialog("Reset Time Tracked?", "This will set all timers for '" + tasks.name + "' to zero. This cannot be undone.", "Reset", "Cancel"))
                                                                                {
                                                                                    tasks.activeMinutes = tasks.idleMinutes = tasks.sleepMinutes = 0;
                                                                                }
                                                                            }
                                                                            s = new GUIStyle("button");
                                                                        }
                                                                        GUILayout.EndHorizontal();

                                                                        GUILayout.BeginHorizontal();
                                                                        {
                                                                            tasks.editTimers = GUILayout.Toggle(tasks.editTimers, "Manually Adjust", s);
                                                                        }
                                                                        GUILayout.EndHorizontal();

                                                                        if (tasks.editTimers)
                                                                        {
                                                                            GUILayout.BeginHorizontal();
                                                                            {
                                                                                GUILayout.FlexibleSpace();
                                                                                GUILayout.Label("Active: ", GUILayout.Width(48));
                                                                                s = new GUIStyle("textfield");
                                                                                s.alignment = TextAnchor.MiddleRight;
                                                                                long am = tasks.activeMinutes;

                                                                                text = am / 1440 + "d " + am % 1440 / 60 + "h " + am % 60 + "m";
                                                                                if (am < 1440) text = am / 60 + "h " + am % 60 + "m";
                                                                                if (am < 60) text = am + "m";

                                                                                GUILayout.Label(text, s, GUILayout.Width(80));
                                                                                s = new GUIStyle("button");
                                                                                if (GUILayout.Button("+", s, GUILayout.Width(24), GUILayout.Height(20))) tasks.activeMinutes += Core.timerManualMinutes;
                                                                                if (GUILayout.Button("-", s, GUILayout.Width(24), GUILayout.Height(20))) if (tasks.activeMinutes > 0) tasks.activeMinutes -= Core.timerManualMinutes;
                                                                                s = new GUIStyle("textfield");
                                                                                s.alignment = TextAnchor.MiddleRight;
                                                                                Core.timerManualMinutes = EditorGUILayout.IntField(Core.timerManualMinutes, s, GUILayout.Width(32));
                                                                                GUILayout.Label("min");

                                                                                s = new GUIStyle("button");
                                                                                if (GUILayout.Button("R", s, GUILayout.Width(24), GUILayout.Height(20)))
                                                                                {
                                                                                    if (EditorUtility.DisplayDialog("Reset Time Tracked?", "This will set Active Timer for '" + tasks.name + "' to zero. This cannot be undone.", "Reset", "Cancel"))
                                                                                    {
                                                                                        tasks.activeMinutes = 0;
                                                                                    }
                                                                                }
                                                                                GUILayout.FlexibleSpace();
                                                                            }
                                                                            GUILayout.EndHorizontal();
                                                                            GUILayout.BeginHorizontal();
                                                                            {
                                                                                GUILayout.FlexibleSpace();
                                                                                GUILayout.Label("Idle: ", GUILayout.Width(48));
                                                                                s = new GUIStyle("textfield");
                                                                                s.alignment = TextAnchor.MiddleRight;
                                                                                long am = tasks.idleMinutes;

                                                                                text = am / 1440 + "d " + am % 1440 / 60 + "h " + am % 60 + "m";
                                                                                if (am < 1440) text = am / 60 + "h " + am % 60 + "m";
                                                                                if (am < 60) text = am + "m";
                                                                                GUILayout.Label(text, s, GUILayout.Width(80));
                                                                                s = new GUIStyle("button");
                                                                                if (GUILayout.Button("+", s, GUILayout.Width(24), GUILayout.Height(20))) tasks.idleMinutes += Core.timerManualMinutes;
                                                                                if (GUILayout.Button("-", s, GUILayout.Width(24), GUILayout.Height(20)))
                                                                                    if (tasks.idleMinutes > 0) tasks.idleMinutes -= Core.timerManualMinutes;
                                                                                s = new GUIStyle("textfield");
                                                                                s.alignment = TextAnchor.MiddleRight;
                                                                                Core.timerManualMinutes = EditorGUILayout.IntField(Core.timerManualMinutes, s, GUILayout.Width(32));
                                                                                GUILayout.Label("min");
                                                                                //GUILayout.FlexibleSpace();
                                                                                s = new GUIStyle("button");
                                                                                if (GUILayout.Button("R", s, GUILayout.Width(24), GUILayout.Height(20)))
                                                                                {
                                                                                    if (EditorUtility.DisplayDialog("Reset Time Tracked?", "This will set Idle Timer for '" + tasks.name + "' to zero. This cannot be undone.", "Reset", "Cancel"))
                                                                                    {
                                                                                        tasks.idleMinutes = 0;
                                                                                    }
                                                                                }
                                                                                GUILayout.FlexibleSpace();
                                                                            }
                                                                            GUILayout.EndHorizontal();
                                                                            GUILayout.BeginHorizontal();
                                                                            {
                                                                                GUILayout.FlexibleSpace();
                                                                                GUILayout.Label("Sleep: ", GUILayout.Width(48));
                                                                                s = new GUIStyle("textfield");
                                                                                s.alignment = TextAnchor.MiddleRight;
                                                                                long am = tasks.sleepMinutes;

                                                                                text = am / 1440 + "d " + am % 1440 / 60 + "h " + am % 60 + "m";
                                                                                if (am < 1440) text = am / 60 + "h " + am % 60 + "m";
                                                                                if (am < 60) text = am + "m";
                                                                                GUILayout.Label(text, s, GUILayout.Width(80));
                                                                                s = new GUIStyle("button");
                                                                                if (GUILayout.Button("+", s, GUILayout.Width(24), GUILayout.Height(20))) tasks.sleepMinutes += Core.timerManualMinutes;
                                                                                if (GUILayout.Button("-", s, GUILayout.Width(24), GUILayout.Height(20)))
                                                                                    if (tasks.sleepMinutes > 0) tasks.sleepMinutes -= Core.timerManualMinutes;

                                                                                s = new GUIStyle("textfield");
                                                                                s.alignment = TextAnchor.MiddleRight;
                                                                                Core.timerManualMinutes = EditorGUILayout.IntField(Core.timerManualMinutes, s, GUILayout.Width(32));
                                                                                GUILayout.Label("min");
                                                                                //GUILayout.FlexibleSpace();
                                                                                s = new GUIStyle("button");
                                                                                if (GUILayout.Button("R", s, GUILayout.Width(24), GUILayout.Height(20)))
                                                                                {
                                                                                    if (EditorUtility.DisplayDialog("Reset Time Tracked?", "This will set Sleep Timer for '" + tasks.name + "' to zero. This cannot be undone.", "Reset", "Cancel"))
                                                                                    {
                                                                                        tasks.sleepMinutes = 0;
                                                                                    }
                                                                                }
                                                                                GUILayout.FlexibleSpace();
                                                                            }
                                                                            GUILayout.EndHorizontal();

                                                                        }
                                                                    }
                                                                    GUILayout.EndVertical();
                                                                    GUI.enabled = true;
                                                                }
                                                                GUILayout.EndScrollView();
                                                                break;
                                                        }
                                                    }
                                                    GUILayout.EndVertical();
                                                    GUILayout.Space(8);
                                                }
                                                GUILayout.EndHorizontal();

                                                GUILayout.Space(8);
                                                GUILayout.BeginHorizontal();
                                                {
                                                    GUILayout.Space(8);
                                                    s = StyleCheck("label");
                                                    s.normal.background = Core.tBlack;
                                                    s.hover.background = Core.tRed;
                                                    s.fontSize = 16;
                                                    s.hover.textColor = Color.black;
                                                    s.alignment = TextAnchor.MiddleCenter;
                                                    if (GUILayout.Button("Remove Task", s))
                                                    {
                                                        if (EditorUtility.DisplayDialog("Delete Task?", "Are you sure you want to delete the task " + tasks.name + " in the Landmark called " + localData.title + "?  This cannot be undone!", "Delete", "Cancel"))
                                                        {
                                                            localData.tasks.RemoveAt(i);
                                                            Core.SaveData();
                                                        }
                                                    }
                                                    GUILayout.Space(8);
                                                }
                                                GUILayout.EndHorizontal();
                                                s = StyleCheck();
                                                s.normal.background = Core.MakeTex(1, 1, new Color32(255, 255, 255, 0));
                                                s.fontSize = 16;
                                                s.normal.textColor = Core.cBackgroundPanels;
                                                s.hover.textColor = Color.black;
                                                s.alignment = TextAnchor.MiddleCenter;

                                                if (GUI.Button(new Rect(foldRect.x + foldRect.width - 44 - 0, foldRect.y + foldRect.height - 100, 32, 32), "▼", s))
                                                {
                                                    tasks.isFolded = !tasks.isFolded;
                                                }


                                            }





                                            GUILayout.Space(16);


                                            //#region GUIChangedTasks
                                            //if (tasks.CheckChanged(prev))
                                            //{
                                            //    localData.UpdateProgress(scene.stickyFont);
                                            //    Core.SaveData();
                                            //}
                                            //#endregion

                                        }
                                    end:;
                                    }
                                    GUILayout.EndVertical();

                                }
                                GUILayout.EndScrollView();


                                #region DrawTaskHeader
                                s = StyleCheck();
                                s.alignment = TextAnchor.LowerCenter;
                                s.fontSize = 20;
                                s.fontStyle = FontStyle.Bold;
                                s.normal.background = headerColor;

                                s.normal.textColor = headerColorText;

                                GUI.Label(new Rect(0, headerRect.y + headerRect.height, screenWidth - 20, 20), headerText, s);

                                #endregion
                                break;
                            case TA.LandmarkDetailState.delete:
                                List<String> landmarkNames = new List<string>();
                                for (int i = 0; i < scene.landmarks.Count; i++)
                                {
                                    landmarkNames.Add((i + 1) + ". " + landmarks.title);
                                }

                                if (GUILayout.Button("Back"))
                                {
                                    scene.landmarkDetailState = TA.LandmarkDetailState.tasks;
                                }

                                scene.scrollPosLandmarkDetailDelete = GUILayout.BeginScrollView(scene.scrollPosLandmarkDetailDelete, GUIStyle.none, GUILayout.Height(screenHeight - 128 - 112));
                                {
                                    for (int i = 0; i < localData.tasks.Count; i++)
                                    {
                                        var tasks = localData.tasks[i];

                                        s = StyleCheck();
                                        s.fontSize = 24;
                                        s.fontStyle = FontStyle.Bold;
                                        s.alignment = TextAnchor.MiddleLeft;

                                        s.normal.textColor = WhiteOrBlack(tasks.color);
                                        s.normal.background = tasks.tColor;

                                        GUILayout.BeginVertical();
                                        {
                                            GUILayout.Space(8);
                                            GUILayout.BeginHorizontal(s, GUILayout.Height(32));
                                            GUILayout.Space(4);
                                            text = tasks.name;
                                            if (text.Length > 25) text = text.Substring(0, 15);

                                            text = (i + 1) + ". " + text;
                                            GUILayout.BeginVertical();
                                            {
                                                GUILayout.Space(3);
                                                GUILayout.BeginHorizontal();
                                                GUILayout.Label(text, s);


                                                GUILayout.FlexibleSpace();
                                                if (GUILayout.Button(Core.tTrashCan, GUILayout.Width(24), GUILayout.Height(24)))
                                                {
                                                    if (EditorUtility.DisplayDialog("Delete Task?", "Are you sure you want to delete " + tasks.name + " inside the landmark named " + localData.title + "?  This cannot be undone!", "Delete", "Cancel"))
                                                    {
                                                        localData.tasks.RemoveAt(i);
                                                    }
                                                }
                                                if (GUILayout.Button(Core.tArrowUp, GUILayout.Width(24), GUILayout.Height(24)))
                                                {
                                                    localData.tasks.Move(i, i - 1);
                                                }

                                                if (GUILayout.Button(Core.tArrowDown, GUILayout.Width(24), GUILayout.Height(24)))
                                                {
                                                    localData.tasks.Move(i, i + 1);

                                                }
                                                GUILayout.EndHorizontal();
                                            }
                                            GUILayout.EndVertical();
                                            s.normal.background = tasks.tColorDark;


                                            GUILayout.Space(8);
                                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(s, GUILayout.Height(32));
                                            GUILayout.Space(4);
                                            s = StyleCheck();
                                            s.normal.background = Core.tTransparent;

                                            s.normal.textColor = WhiteOrBlack(tasks.tColorDark.GetPixel(1, 1));

                                            s.fontSize = 14;

                                            GUILayout.BeginVertical();
                                            {
                                                GUILayout.Space(4);
                                                GUILayout.BeginHorizontal();
                                                GUILayout.Label("Choose Landmark:", s);

                                                localData.landmarkMoveTo = EditorGUILayout.Popup(localData.landmarkMoveTo, landmarkNames.ToArray());

                                                if (GUILayout.Button("Copy"))
                                                {
                                                    scene.landmarks[localData.landmarkMoveTo].tasks.Add(tasks.Copy());
                                                }

                                                if (GUILayout.Button("Move"))
                                                {
                                                    scene.landmarks[localData.landmarkMoveTo].tasks.Add(tasks.Copy());
                                                    localData.tasks.RemoveAt(i);
                                                }

                                                GUILayout.Space(4);
                                                GUILayout.EndHorizontal();
                                                HorizontalLine(Color.black);
                                                GUILayout.EndHorizontal();
                                            }
                                            GUILayout.EndVertical();
                                        }
                                        GUILayout.EndVertical();
                                    }
                                }
                                GUILayout.EndScrollView();
                                break;

                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndScrollView();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }

        static void DrawTagFilterMask()
        {
            #region TagFilterMask
            GUILayout.Space(6);
            sTemp = StyleCheck();
            var method = typeof(EditorGUI).GetMethod("MaskFieldInternal", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null, new System.Type[] { typeof(Rect), typeof(GUIContent), typeof(int), typeof(string[]), typeof(int[]), typeof(GUIStyle) }, null);
            RefreshTaglist();
            int[] array = new int[tagList.Count];
            for (int i = 0; i < array.Length; i++)
            {
                if (i == 0) { array[i] = 0; continue; }
                if (i == array.Length - 1) { array[i] = -1; continue; }
                array[i] = 1 << (i - 1);
            }

            sTemp.padding = new RectOffset(0, 0, 4, 0);
            GUILayout.Label("Tag Filter:", sTemp);
            GUILayout.Space(10);
            GUILayout.Label("", GUILayout.Width(104));
            Rect position = GUILayoutUtility.GetLastRect();
            var prev = tagSelected;
            tagSelected = (int)method.Invoke(null, new object[] { position, GUIContent.none, tagSelected, tagList.ToArray(), array, EditorStyles.popup });
            if (prev != tagSelected)
            {
                tagFilter = new List<string>();
                for (int i = 0; i < Core.dataV2.tags.Count; i++)
                {
                    int tag = 1 << i;
                    if ((tagSelected & tag) != 0)
                    {
                        tagFilter.Add(Core.dataV2.tags[i].name);
                        //Core.dataV2.tags[i].active = true;
                    }
                    else
                    {
                        //Core.dataV2.tags[i].active = false;
                    }
                }
                //for (int i = 0; i < tagFilter.Count; i++)
                //{
                //    Debug.Log(tagFilter[i]);
                //}
                Core.SaveData();
            }
            GUILayout.FlexibleSpace();
            #endregion
        }

        static GUIStyle sTemp;
        static Vector2 scrollLandmarkCarousel;
        static void LoadViewLandmarkCarousel()
        {
            int bCount = 0;
            sTemp = new GUIStyle();
            scrollLandmarkCarousel = GUILayout.BeginScrollView(scrollLandmarkCarousel, GUILayout.Height(192));
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                for (int i = 0; i < scene.landmarks.Count; i++)
                {
                    var landmarks = scene.landmarks[i];
                    #region CheckActiveTags
                    bool noTag = true;
                    //if (tags != null)
                    for (int t = 0; t < Core.dataV2.tags.Count; t++)
                    {
                        for (int tt = 0; tt < tagFilter.Count; tt++)
                        {
                            if (Core.dataV2.tags[t].name == tagFilter[tt])
                                if (Core.dataV2.tags[t].GetActive(landmarks.timeStamp)) noTag = false;
                        }
                        //for (int r = 0; r < landmarks.tags.Count; r++)
                        //{
                        //    if (landmarks.tags[r].name == tags[t])
                        //    {
                        //        if (landmarks.tags[r].active)
                        //        {
                        //            noTag = false;
                        //        }
                        //    }
                        //}
                    }
                    if (tagSelected == 0 & noTag) goto skipcheck;
                    if (noTag) continue;
                    skipcheck:
                    #endregion
                    #region bLandmarkThumbnail
                    bCount++;
                    if (i == scene.selectedLandmark)
                    {
                        GUILayout.Box(Core.tWhite, GUIStyle.none, GUILayout.Width(176), GUILayout.Height(176));
                        Rect r = GUILayoutUtility.GetLastRect();
                        if (landmarks.tGizmoColor == null)
                        {
                            // landmarks.GizmoColor = Color.white;
                            // landmarks.SetColor(Color.white);
                        }

                        DrawTexture(r, landmarks.tGizmoColor);

                        if (GUI.Button(new Rect(r.x + 4, r.y + 4, r.width - 3, r.height - 9), new GUIContent(landmarks.tScreenshot), GUIStyle.none))
                        {
                            ZoomToLocation(i);
                        }
                    }
                    else
                    {
                        GUILayout.Box(Core.tWhite, GUIStyle.none, GUILayout.Width(176), GUILayout.Height(176));
                        Rect r = GUILayoutUtility.GetLastRect();
                        if (landmarks.tGizmoColor == null) landmarks.SetColor(landmarks.GizmoColor);
                        DrawTexture(new Rect(r.x + 2, r.y + 2, r.width - 5, r.height - 6), landmarks.tGizmoColor);


                        if (GUI.Button(new Rect(r.x + 4, r.y + 4, r.width - 3, r.height - 9), new GUIContent(landmarks.tScreenshot), GUIStyle.none))
                        {
                            scene.selectedLandmark = i;
                        }
                    }
                    Rect pos = GUILayoutUtility.GetLastRect();
                    sTemp.fontSize = 12;
                    sTemp.alignment = TextAnchor.MiddleCenter;
                    sTemp.normal.textColor = WhiteOrBlack(landmarks.GizmoColor);
                    sTemp.normal.background = landmarks.tGizmoColor50;
                    string text = landmarks.title;
                    if (text.Length > 21) text = text.Substring(0, 21);
                    GUI.Label(new Rect(pos.x + 6, pos.y + 6, pos.width - 12, 24f), text, sTemp);
                    if (landmarks.tasksHasAlert)
                    {
                        DrawAlert(pos.x, pos.y, 24, 24, Core.tIconExclaim);
                    }
                    ProgressBar(new Rect(pos.x + 8, pos.y + pos.height - 20, pos.width - 16, 8f), landmarks.progress, Core.tGreyDark, Core.tGreenBright);
                    #endregion
                    GUILayout.Space(16);
                }
                GUILayout.Space(-16);
                GUILayout.FlexibleSpace();
            }
            if (bCount == 0)
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("No other Landmarks with tags selected found...");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }

        static void LoadViewLandmarkDetailGeneral(int landmark)
        {
            var landmarks = scene.landmarks[landmark];
            scene.scrollPosLandmarkDetailGeneral = GUILayout.BeginScrollView(scene.scrollPosLandmarkDetailGeneral, GUIStyle.none, GUILayout.Height(screenHeight - 128 - 80));
            {
                GUILayout.BeginVertical();
                {
                    DrawTaskHeader("Details", Core.tEditPen, Core.tGreen);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Name:", GUILayout.Width(48));
                        landmarks.title = GUILayout.TextField(landmarks.title, GUILayout.Width(128 + 48));
                    }
                    GUILayout.EndHorizontal();

                    HorizontalLine(Color.black);

                    GUILayout.Label("Description:");
                    s = GetTextAreaStyle(landmarks.description, screenWidth - 26);
                    landmarks.description = GUILayout.TextArea(landmarks.description, s, GUILayout.Width(screenWidth - 26), GUILayout.Height(s.fixedHeight));

                    #region Tags
                    DrawTaskHeader("Tags", Core.tIconTag, Core.tTeal);
                    GUILayout.BeginHorizontal();
                    {
                        //#region EditTagButtons
                        //switch (tagEditMode)
                        //{
                        //    case TagEditMode.none:
                        //        #region AddNewTag
                        //        GUILayout.Label("Add New Tag:");
                        //        if (screenWidth < 230) { GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(); }
                        //        newTag = GUILayout.TextField(newTag, GUILayout.Width(96));
                        //        if (GUILayout.Button(Core.tPlus, GUIStyle.none, GUILayout.Width(16)))
                        //        {
                        //            Core.dataV2.tags.Add(new Tags(newTag));
                        //            Core.dataV2.tags = Core.dataV2.tags.Distinct().ToList();
                        //            Core.dataV2.tags = Core.dataV2.tags.OrderBy(x => x.name).ToList();
                        //            for (int i = 0; i < scene.landmarks.Count; i++)
                        //            {
                        //                landmarks.tags.Add(new Tags(newTag));
                        //                landmarks.tags = landmarks.tags.Distinct().ToList();
                        //                landmarks.tags = landmarks.tags.OrderBy(x => x.name).ToList();
                        //            }
                        //            Core.SaveData();
                        //            newTag = "";
                        //        }
                        //        #endregion
                        //        GUILayout.FlexibleSpace();
                        //        #region EditTags
                        //        if (GUILayout.Button(new GUIContent("  Edit"), GUILayout.Width(64), GUILayout.Height(18)))
                        //        {
                        //            tagEditMode = TagEditMode.edit;
                        //            tagListCopy = new List<string>();
                        //            for (int i = 0; i < Core.dataV2.tags.Count; i++)
                        //            {
                        //                tagListCopy.Add(Core.dataV2.tags[i].name);
                        //            }
                        //        }
                        //        #endregion
                        //        #region DeleteTags
                        //        if (GUILayout.Button(new GUIContent("  Delete"), GUILayout.Width(64), GUILayout.Height(18))) tagEditMode = TagEditMode.delete;
                        //        GUILayout.Space(16);
                        //        #endregion
                        //        break;
                        //    case TagEditMode.edit:
                        //        if (screenWidth >= 212)
                        //        {
                        //            GUILayout.Space(screenWidth - 96 - 96 - 16);
                        //        }

                        //        #region SaveTags
                        //        if (GUILayout.Button("Save Tags", GUILayout.Width(96)))
                        //        {
                        //            tagEditMode = TagEditMode.none;
                        //            for (int i = 0; i < tagListCopy.Count; i++)
                        //            {
                        //                if (Core.dataV2.tags[i].name != tagListCopy[i])
                        //                {
                        //                    Core.dataV2.tags[i].name = tagListCopy[i];
                        //                }
                        //            }
                        //            for (int i = 0; i < scene.landmarks.Count; i++)
                        //            {
                        //                for (int x = 0; x < landmarks.tags.Count; x++)
                        //                {
                        //                    landmarks.tags[x].name = tagListCopy[x];
                        //                }
                        //                landmarks.tags = landmarks.tags.Distinct().ToList();
                        //                landmarks.tags = landmarks.tags.OrderBy(x => x.name).ToList();

                        //            }
                        //            for (int x = 0; x < Core.dataV2.tags.Count; x++)
                        //            {
                        //                Core.dataV2.tags[x].name = tagListCopy[x];
                        //            }
                        //            Core.dataV2.tags = Core.dataV2.tags.Distinct().ToList();
                        //            Core.dataV2.tags = Core.dataV2.tags.OrderBy(x => x.name).ToList();
                        //            Core.SaveData();
                        //        }
                        //        #endregion
                        //        #region CancelEdit
                        //        if (screenWidth < 210) { GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(); }
                        //        if (GUILayout.Button("Cancel", GUILayout.Width(96)))
                        //        {
                        //            tagEditMode = TagEditMode.none;
                        //        }
                        //        #endregion
                        //        break;
                        //    case TagEditMode.delete:
                        //        if (screenWidth >= 212)
                        //        {
                        //            GUILayout.Space(screenWidth - 128 - 16);
                        //        }
                        //        #region ExitDeleteMode
                        //        if (GUILayout.Button("Exit Delete Mode", GUILayout.Width(128))) tagEditMode = TagEditMode.none;
                        //        #endregion
                        //        break;
                        //}
                        //#endregion
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {
                        #region ShowTags


                        switch (tagEditMode)
                        {
                            case TagEditMode.none:
                                #region SelectionMode

                                for (int i = 0; i < Core.dataV2.tags.Count; i++)
                                {
                                    GUILayout.EndHorizontal(); GUILayout.Space(4); GUILayout.BeginHorizontal();
                                    s = new GUIStyle("button");
                                    Color prevColor = GUI.backgroundColor;
                                    bool isActive = Core.dataV2.tags[i].GetActive(landmarks.timeStamp);
                                    if (isActive)
                                    {
                                        GUI.backgroundColor = Core.cTeal;
                                    }
                                    isActive = GUILayout.Toggle(isActive, Core.dataV2.tags[i].name, s);
                                    if (isActive != Core.dataV2.tags[i].GetActive(landmarks.timeStamp)) Core.dataV2.tags[i].SetActive(landmarks.timeStamp, isActive);
                                    GUI.backgroundColor = prevColor;
                                }
                                #endregion
                                break;
                            case TagEditMode.edit:
                                #region EditMode
                                //for (int i = 0; i < tagListCopy.Count; i++)
                                //{
                                //    GUILayout.EndHorizontal(); GUILayout.Space(4); GUILayout.BeginHorizontal();
                                //    s = new GUIStyle("textfield");
                                //    s.alignment = TextAnchor.MiddleCenter;
                                //    tagListCopy[i] = GUILayout.TextField(tagListCopy[i], s);
                                //}
                                #endregion
                                break;
                            case TagEditMode.delete:
                                #region DeleteMode
                                //var p = GUI.backgroundColor;
                                //GUI.backgroundColor = Color.red;
                                //for (int i = 0; i < landmarks.tags.Count; i++)
                                //{
                                //    GUILayout.EndHorizontal(); GUILayout.Space(4); GUILayout.BeginHorizontal();
                                //    if (GUILayout.Button(landmarks.tags[i].name))
                                //    {
                                //        if (EditorUtility.DisplayDialog("Delete Tag?", "Are you sure you want to delete the tag " + landmarks.tags[i].name + "?  This cannot be undone and affects all landmarks.", "Delete Tag"))
                                //        {
                                //            for (int ii = 0; ii < scene.landmarks.Count; ii++)
                                //            {
                                //                scene.landmarks[ii].tags.RemoveAt(i);

                                //            }
                                //            Core.dataV2.tags.RemoveAt(i);
                                //            Core.dataV2.tags = Core.dataV2.tags.Distinct().ToList();
                                //            Core.dataV2.tags = Core.dataV2.tags.OrderBy(x => x.name).ToList();
                                //            Core.SaveData();
                                //            RefreshSettings();
                                //        }
                                //    };
                                //}
                                //GUI.backgroundColor = p;
                                #endregion
                                break;
                        }
                        #endregion
                    }
                    GUILayout.EndHorizontal();
                    HorizontalLine(Color.black);
                    text = "Hide Label in Scene";
                    if (!scene.landmarks[scene.selectedLandmark].showGizmo) text = "Show Label in Scene";
                    if (DrawTaskHeader("Landmark Label", Core.tIconPin, Core.tBlue, true, 180, 18, new GUIContent(text)))
                    {
                        if (scene.landmarks[scene.selectedLandmark].showGizmo)
                        {
                            scene.landmarks[scene.selectedLandmark].showGizmo = false;
                        }
                        else
                        {
                            scene.landmarks[scene.selectedLandmark].showGizmo = true;
                        }
                    }

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Color");
                        scene.landmarks[scene.selectedLandmark].SetColor(EditorGUILayout.ColorField(scene.landmarks[scene.selectedLandmark].GizmoColor, GUILayout.Width(48)));
                        GUILayout.FlexibleSpace();
                        if (scene.landmarks[scene.selectedLandmark].showGizmo)
                        {
                            s = new GUIStyle("button");
                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                            scene.landmarks[scene.selectedLandmark].wsOffset = EditorGUILayout.Vector3Field("Label Offset: ", scene.landmarks[scene.selectedLandmark].wsOffset);
                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                            scene.landmarks[scene.selectedLandmark].fadeGizmo = GUILayout.Toggle(scene.landmarks[scene.selectedLandmark].fadeGizmo, "Fade with Distance", s);

                            if (!scene.landmarks[scene.selectedLandmark].fadeGizmo) GUI.enabled = false;

                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                            GUILayout.Label("Fade Distance:");
                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                            var prev = scene.landmarks[scene.selectedLandmark].fadeStart + scene.landmarks[scene.selectedLandmark].fadeEnd;
                            EditorGUILayout.MinMaxSlider(ref scene.landmarks[scene.selectedLandmark].fadeStart,
                                                         ref scene.landmarks[scene.selectedLandmark].fadeEnd,
                                                         0, 1000);
                            if (prev != scene.landmarks[scene.selectedLandmark].fadeStart + scene.landmarks[scene.selectedLandmark].fadeEnd)
                                Core.SaveData();
                            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                            s = new GUIStyle("textfield");
                            GUILayout.Space(16);
                            GUILayout.Label("Fade Start at:");
                            GUILayout.Label(scene.landmarks[scene.selectedLandmark].fadeStart.ToString("F1"), s);
                            GUILayout.FlexibleSpace();
                            GUILayout.Label("Fade End at:");
                            GUILayout.Label(scene.landmarks[scene.selectedLandmark].fadeEnd.ToString("F1"), s);
                            GUILayout.Space(16);
                            GUI.enabled = true;
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(16);
                }
                DrawTaskHeader("Task Defaults", Core.tIconTask, Core.tBlue);
                EditorGUILayout.HelpBox("Only applies for 3D Stickies:", MessageType.Info);

                GUILayout.Label("Fade Distance:");
                GUILayout.BeginHorizontal();
                {
                    var prev = landmarks.taskFadeStart + landmarks.taskFadeEnd;
                    EditorGUILayout.MinMaxSlider(ref landmarks.taskFadeStart, ref landmarks.taskFadeEnd, 0f, 1000f);
                    if (prev != landmarks.taskFadeStart + landmarks.taskFadeEnd) Core.SaveData();
                    GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
                    s = new GUIStyle("textfield");
                    GUILayout.Space(16);
                    GUILayout.Label("Fade Start at:");
                    GUILayout.Label(landmarks.taskFadeStart.ToString("F1"), s);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Fade End at:");
                    GUILayout.Label(landmarks.taskFadeEnd.ToString("F1"), s);
                    GUILayout.Space(16);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(16);

                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
            #endregion
        }




        static public void AddCurrentViewToLandmarks()
        {
            if (Core.rtSS == null) Core.RefreshHelperCams();
            Core.EnableLandmarkCamera();
            if (Core.GetSceneView().in2DMode)
            {
                Core.LandmarkCamera.transform.position = new Vector3(Core.LandmarkCamera.transform.position.x, Core.LandmarkCamera.transform.position.y, -1000f);
            }
            else
            {
                Core.LandmarkCamera.transform.position = Core.GetSVCameraPosition();
            }

            Core.LandmarkCamera.transform.rotation = Core.GetSVCameraRotation();
            Core.LandmarkCamera.GetComponent<Camera>().orthographicSize = Core.GetSVCOrthographicSize();



            scene.landmarks.Add(
                new Landmark(
                    Core.rtSS,
                    Core.LandmarkCamera.transform.position,
                    Core.LandmarkCamera.transform.rotation,
                    Core.GetSVCOrthographicSize())
            );
            scene.selectedLandmark = scene.landmarks.Count - 1;
            //scene.landmarks[scene.selectedLandmark].tags = new List<Tags>();
            //for (int i = 0; i < Core.dataV2.tags.Count; i++)
            //{
            //    scene.landmarks[scene.selectedLandmark].tags.Add(new Tags(Core.dataV2.tags[i].name));
            //}
            Core.DisableLandmarkCamera();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        static public int ImportLandmark(Vector3 position, Quaternion rotation)
        {
            if (Core.rtSS == null) Core.RefreshHelperCams();
            Core.EnableLandmarkCamera();
            if (Core.GetSceneView().in2DMode)
            {
                Core.LandmarkCamera.transform.position = new Vector3(position.x, position.y, -1000f);
            }
            else
            {
                Core.LandmarkCamera.transform.position = position;
            }

            Core.LandmarkCamera.transform.rotation = rotation;
            Core.LandmarkCamera.GetComponent<Camera>().orthographicSize = Core.GetSVCOrthographicSize();



            scene.landmarks.Add(
                new Landmark(
                    Core.rtSS,
                    Core.LandmarkCamera.transform.position,
                    Core.LandmarkCamera.transform.rotation,
                    Core.GetSVCOrthographicSize())
            );

            //scene.landmarks[scene.selectedLandmark].tags = new List<Tags>();
            //for (int i = 0; i < Core.dataV2.tags.Count; i++)
            //{
            //    scene.landmarks[scene.selectedLandmark].tags.Add(new Tags(Core.dataV2.tags[i].name));
            //}
            Core.DisableLandmarkCamera();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            return scene.landmarks.Count - 1;
        }

        static void DeleteLandmark(int index)
        {
            scene.landmarks.RemoveAt(index);
            scene.selectedLandmark = 0;
            scene.landmarkDetailOpen = false;
            Core.SaveData();
        }

        //static void RefreshLandmarkTags()
        //{
        //    Core.dataV2.tags = Core.dataV2.tags.Distinct().ToList();
        //    Core.dataV2.tags = Core.dataV2.tags.OrderBy(x => x.name).ToList();
        //    for (int i = 0; i < scene.landmarks.Count; i++)
        //    {
        //        var landmarks = scene.landmarks[i];
        //        var oldList = landmarks.tags;
        //        landmarks.tags = new List<Tags>();

        //        for (int y = 0; y < Core.dataV2.tags.Count; y++)
        //        {
        //            landmarks.tags.Add(new Tags(Core.dataV2.tags[y].name));
        //            for (int x = 0; x < oldList.Count; x++)
        //            {
        //                if (oldList[x].name == Core.dataV2.tags[y].name)
        //                {
        //                    int lastIndex = landmarks.tags.Count - 1;
        //                    landmarks.tags[lastIndex].active = oldList[x].active;
        //                }
        //                else
        //                {
        //                }
        //            }
        //        }
        //    }
        //    Core.SaveData();
        //}

        static public void RefreshLandmarkThumbnail(int index)
        {
            Core.EnableLandmarkCamera();

            if (scene.landmarks[index].viewState == Landmark.ViewState.is2D)
                Core.LandmarkCamera.GetComponent<Camera>().orthographic = true;
            if (scene.landmarks[index].viewState == Landmark.ViewState.is3D)
                Core.LandmarkCamera.GetComponent<Camera>().orthographic = false;

            if (scene.landmarks[index].isFloating && scene.landmarks[index].floatingPositionObject != null)
            {
                Core.LandmarkCamera.transform.position = scene.landmarks[index].floatingPositionObject.transform.position;
                Core.LandmarkCamera.transform.rotation = scene.landmarks[index].floatingPositionObject.transform.rotation;
            }
            else
            {
                Core.LandmarkCamera.transform.position = scene.landmarks[index].position;
                Core.LandmarkCamera.transform.rotation = scene.landmarks[index].rotation;
            }

            scene.landmarks[index].RefreshThumbnail(Core.LandmarkCamera.GetComponent<Camera>(), scene.landmarks[index].orthographicSize, Core.LandmarkCamera.GetComponent<Camera>().orthographic);
            Core.SaveData();
            Core.DisableLandmarkCamera();
        }
        static void RefreshLandmarkPosition(int index)
        {
            scene.landmarks[index].isFloating = false;
            scene.landmarks[index].position = Core.GetSVCameraPosition();
            if (Core.GetSceneView().in2DMode) scene.landmarks[index].position = new Vector3(scene.landmarks[index].position.x, scene.landmarks[index].position.y, -1000f);
            scene.landmarks[index].rotation = Core.GetSVCameraRotation();
            scene.landmarks[index].orthographicSize = Core.GetSVCOrthographicSize();
            RefreshLandmarkThumbnail(index);
        }

        static GUIStyle GetTextAreaStyle(string text, float w)
        {
            GUIStyle s = new GUIStyle(GUI.skin.textArea);

            s.fixedHeight = 0;
            s.fixedHeight = s.CalcHeight(new GUIContent(text), w);
            s.wordWrap = true;
            s.stretchWidth = false;

            return s;
        }

        static GUIStyle StyleCheck()
        {
            GUIStyle style = new GUIStyle();
            style.margin = style.padding = new RectOffset(0, 0, 0, 0);
            if (EditorGUIUtility.isProSkin)
            {
                style.normal.textColor = Color.white;
            }
            return style;
        }
        static GUIStyle StyleCheck(GUIStyle gs)
        {
            GUIStyle style = new GUIStyle(gs);
            if (EditorGUIUtility.isProSkin)
            {
                style.normal.textColor = Color.white;
            }
            return style;
        }

        static GUIStyle StyleBack(GUIStyle style, Texture2D texture)
        {
            if (style == null) style = new GUIStyle();
            style.normal.background = texture;
            return style;
        }
        static void ZoomToLocation(int i, int t = -1)
        {
            SceneView sv = Core.GetSceneView();
            var landmarks = scene.landmarks[i];
            if (TaskAtlasSceneView.atlas.enabled)
            {
                if (landmarks.viewState == Landmark.ViewState.is2D)
                {
                    sv.in2DMode = true;
                }
                else
                {
                    sv.in2DMode = false;
                    if (landmarks.viewState == Landmark.ViewState.isOrthographic)
                    {
                        sv.orthographic = true;
                    }
                    else
                    {
                        sv.orthographic = false;
                    }
                }

                if (i == TaskAtlasSceneView.atlas.landmarkFocus && TaskAtlasSceneView.atlas.landmarkSelected == -1)
                {
                    TaskAtlasSceneView.atlas.landmarkSelected = i;
                    TaskAtlasSceneView.atlas.taskFocus = 0;
                    TaskAtlasSceneView.AtlasSpawnTaskPoints(TaskAtlasSceneView.atlas.landmarkSelected);
                    TaskAtlasSceneView.AtlasTasksZoom(TaskAtlasSceneView.atlas.landmarkSelected, 2);
                }
                else
                {
                    TaskAtlasSceneView.atlas.landmarkSelected = -1;

                    TaskAtlasSceneView.atlas.landmarkFocus = i;
                    TaskAtlasSceneView.atlas.taskFocus = 0;
                    TaskAtlasSceneView.AtlasSpawnLandmarkPoints();
                    TaskAtlasSceneView.AtlasLandmarksZoom();
                    TaskAtlasSceneView.StartAtlasMode(false);
                }
            }
            else
            {
                Core.EnableLandmarkCamera();

                if (t < 0)
                {
                    if (landmarks.isFloating && landmarks.floatingPositionObject != null)
                    {
                        Core.LandmarkCamera.transform.position = landmarks.floatingPositionObject.transform.position;
                        Core.LandmarkCamera.transform.rotation = landmarks.floatingPositionObject.transform.rotation;
                    }
                    else
                    {
                        Core.LandmarkCamera.transform.position = landmarks.position;
                        Core.LandmarkCamera.transform.rotation = landmarks.rotation;
                    }
                }
                else
                {
                    Core.LandmarkCamera.transform.position = new Vector3(landmarks.tasks[t].position.x,
                                                                         landmarks.tasks[t].position.y,
                                                                         landmarks.position.z);
                }

                //sv.AlignViewToObject(Core.LandmarkCamera.transform);
                // sv.pivot = landmarks.position;

                if (landmarks.viewState == Landmark.ViewState.is2D)
                {
                    sv.in2DMode = true;
                    sv.orthographic = true;
                    Core.LandmarkCamera.transform.rotation = Quaternion.identity;
                }
                else
                {
                    sv.in2DMode = false;
                    if (landmarks.viewState == Landmark.ViewState.isOrthographic)
                    {
                        sv.orthographic = true;
                    }
                    else
                    {
                        sv.orthographic = false;
                    }
                }
                sv.AlignViewToObject(Core.LandmarkCamera.transform);
                //sv.pivot = Core.LandmarkCamera.transform.position;// landmarks.position + (landmarks.position - sv.camera.transform.position);
                sv.size = landmarks.orthographicSize;

                sv.Repaint();

                if (!sv.in2DMode)
                    sv.AlignViewToObject(Core.LandmarkCamera.transform);
                //Core.DisableLandmarkCamera();
            }
        }

        static bool DrawTaskHeader(string text, Texture2D icon, Texture2D color, bool hasButton = false, int bWidth = 0, int bHeight = 0, GUIContent bContent = null, GUIStyle bStyle = null, int tWidth = 128, int iSize = 48)
        {
            bool ret = false;
            GUILayout.Space(8);
            GUIStyle s = StyleCheck();
            s.alignment = TextAnchor.MiddleLeft;
            s.fontSize = 14;
            s.fontStyle = FontStyle.Bold;
            sBackground = StyleBack(sBackground, color);
            GUILayout.BeginHorizontal(sBackground, GUILayout.Height(iSize));
            {
                GUILayout.Space(4);
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.Box(icon, GUIStyle.none, GUILayout.Width(iSize), GUILayout.Height(iSize));
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.Space(16);
                GUILayout.Label(text, s, GUILayout.Width(tWidth));
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
                if (hasButton)
                {
                    GUILayout.BeginVertical();
                    GUILayout.FlexibleSpace();
                    if (bStyle == null)
                    {
                        if (GUILayout.Button(bContent, GUILayout.Width(bWidth), GUILayout.Height(bHeight)))
                        {
                            ret = true;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(bContent, bStyle, GUILayout.Width(bWidth), GUILayout.Height(bHeight)))
                        {
                            ret = true;
                        }
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();
                }
                else
                {
                    // GUILayout.FlexibleSpace();
                }
                GUILayout.Space(8);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(8);
            return ret;
        }

        static public void DrawAlert(float x, float y, float width, float height, Texture2D tex)
        {
            int xx = Mathf.FloorToInt(x + ((width - (width * lerp)) / 2));
            int yy = Mathf.FloorToInt(y + ((height - (height * lerp)) / 2));
            DrawTexture(new Rect(xx, yy, width * lerp, height * lerp), Core.tIconExclaim);
        }

        static void DrawBox(Rect position, Color color)
        {
            Color oldColor = GUI.color;

            GUI.color = color;
            GUI.Box(position, "");

            GUI.color = oldColor;
        }

        static void ProgressBar(Rect rect, float progress, Texture2D background, Texture2D foreground)
        {
            if (background == null) return;
            DrawTexture(rect, background);
            DrawTexture(new Rect(rect.x, rect.y, rect.width * progress, rect.height), foreground);
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

        static private string SaveTexture(Texture2D texture)
        {
            byte[] bytes = texture.EncodeToPNG();
            var dirPath = Core.taPath + "Screenshots/";
            if (!System.IO.Directory.Exists(dirPath))
            {
                Debug.Log("Doesn't exist: " + dirPath);
                System.IO.Directory.CreateDirectory(dirPath);
            }
            UnityEditor.AssetDatabase.Refresh();
            dirPath = dirPath + "TaskAtlas_" + SceneManager.GetActiveScene().name + "_" + System.DateTime.Now + ".png";
            dirPath = dirPath.Replace(":", "-");
            System.IO.File.WriteAllBytes(dirPath, bytes);
            Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + dirPath);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            return dirPath;
        }


        static void HorizontalLine(Color color)
        {
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
        static void DrawTexture(Rect r, Texture2D texture)
        {
            if (texture == null) return;
            GUI.DrawTexture(r, texture);
        }


        static public InputField rollNoInputField;// Reference of rollno input field
        static public InputField nameInputField; // Reference of name input filed
        static public Text contentArea; // Reference of contentArea where records are displayed

        static private char lineSeperater = '\n'; // It defines line seperate character
        static private char fieldSeperator = ','; // It defines field seperate chracter

        class CSVLandmarkImportWithTasks
        {
            public List<String> headers = new List<string>();
            public List<List<string>> row = new List<List<string>>();
            public List<bool> rowSelect = new List<bool>();

            public bool showData = false;

            public int ncol() { return headers.Count; }
            public int nrow() { return row.Count - 1; }
        }
        static CSVLandmarkImportWithTasks csvLandmarkImportWithTasks = new CSVLandmarkImportWithTasks();
        static

        void Start()
        {
            readData();
        }
        // Read data from CSV file
        static private void readData()
        {
            csvLandmarkImportWithTasks = new CSVLandmarkImportWithTasks();
            string[] records = csvFile.text.Split(lineSeperater);
            //string record;

            for (int r = 0; r < records.Length; r++)
            {
                string[] fields = records[r].Split(fieldSeperator);
                string t;
                List<string> content = new List<string>();

                foreach (string field in fields)
                {
                    t = field;
                    //Debug.Log(t);
                    if (r == 0)
                    {
                        csvLandmarkImportWithTasks.headers.Add(t);
                    }
                    else
                    {
                        content.Add(t);
                    }
                    //contentArea.text += t;
                }
                if (r > 0)
                    csvLandmarkImportWithTasks.row.Add(content);
                csvLandmarkImportWithTasks.rowSelect.Add(true);
                //contentArea.text += '\n';
            }
        }
        // Add data to CSV file
        public void addData()
        {
            // Following line adds data to CSV file
            File.AppendAllText(getPath() + "/Assets/StudentData.csv", lineSeperater + rollNoInputField.text + fieldSeperator + nameInputField.text);
            // Following lines refresh the edotor and print data
            rollNoInputField.text = "";
            nameInputField.text = "";
            contentArea.text = "";
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            readData();
        }

        // Get path for given CSV file
        private static string getPath()
        {
#if UNITY_EDITOR
            return Application.dataPath;
#elif UNITY_ANDROID
return Application.persistentDataPath;// +fileName;
#elif UNITY_IPHONE
return GetiPhoneDocumentsPath();// +"/"+fileName;
#else
return Application.dataPath;// +"/"+ fileName;
#endif
        }
        // Get the path in iOS device
        private static string GetiPhoneDocumentsPath()
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
            path = path.Substring(0, path.LastIndexOf('/'));
            return path + "/Documents";
        }

    }


    #endregion
}