using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TaskAtlasNamespace
{
    [InitializeOnLoad]
    [ExecuteInEditMode]
    [CustomEditor(typeof(TaskAtlasType))]
    public class Core : Editor
    {
        #region Variables
        static public int timerManualMinutes = 30;

        #region Reference
        static public TaskAtlasData dataV1;
        static public TaskAtlasDataV2 dataV2;
        static public bool isV2 = false, isActive = false;
        #endregion

        #region Icons
        static public Texture2D
            tTrashCan, tEditPen, tSortAZ, tSortDist, tSortDate, tDate, tArrange,
            tArrowLeft, tArrowRight, tArrowUp, tArrowDown, tAdd, tRefresh, tPosition, tGallery,
            tPlus, tMinus,
            tBackgroundPanels, tBackgroundMain, tTransparent,
            tGreenP, tGreen, tTeal, tBlue, tRed, tGrey, tGreenBright, tTealBright, tBlueBright, tRedBright, tGreyBright, tGreenDark, tTealDark, tBlueDark, tRedDark, tGreyDark,
            tWhite, tBlack, tYellow,
            tIconCheck, tIconX, tIconExclaim, tIconPin, tIconTask, tIconTag,
            tAtlasEnable, tAtlasDisable, tAtlasRotLeft, tAtlasRotRight, tAtlasTop, tAtlasSide,
            tAtlasArrowIn, tAtlasArrowOut, tAtlasArrowLeft, tAtlasArrowRight,
            tAtlasEditPen, tAtlasGO, tAtlasSettings,
            tTaskSticky,
            tGradGrey2Clear, tGradClear2Grey,
            tIcon2D, tIcon3D, tIconISO;

        static public Color32
            cBackgroundPanels = new Color32(128, 0, 0, 255),
                        cGreenP = new Color32(0xAB, 0xC8, 0x37, 255),
            cGreen = new Color32(0xAB, 0xC8, 0x37, 64),
            cTeal = new Color32(0x37, 0xC8, 0x9D, 64),
            cBlue = new Color32(0x54, 0x37, 0xC8, 64),
            cRed = new Color32(0xC8, 0x37, 0x62, 64),
            cGrey = new Color32(0xFF, 0xFF, 0xFF, 64);
        #endregion

        #region HelperCams
        static public RenderTexture rtSS, rtStickyText;
        static public GameObject TaskAtlasRoot, StickyTextCam, LandmarkCamera, StickyTextTransform, GizmoHelper;

        static public TaskAtlasGizmos gizmos;
        #endregion
        #endregion

        #region Editor
        static Core()
        {
            EditorSceneManager.sceneSaved += SceneSaved;
            EditorSceneManager.newSceneCreated += NewSceneCreated;
            EditorSceneManager.sceneOpened += SceneOpened;

            EditorApplication.wantsToQuit += EditorQuitting;
        }

        static bool EditorQuitting()
        {
            if (dataV2 != null)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            return true;
        }

        //private static void OnCompile(object obj)
        //{
        //    Core.RemoveHelperCams();
        //    Core.SaveData();
        //}

        static public int CheckSceneExists()
        {
            if (isV2) { return 0; }
            else
            {
                var data = dataV1;
                for (int i = 0; i < data.scene.Count; i++)
                {
                    if (data.scene[i].name == SceneManager.GetActiveScene().path)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        static int CheckSceneExists(string name)
        {
            if (isV2) { return 0; }
            else
            {
                var data = dataV1;
                for (int i = 0; i < data.scene.Count; i++)
                {
                    if (data.scene[i].name == name)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        static void SceneSaved(Scene scene)
        {
            if (isV2) { }
            //else
            //{
            //    var data = dataV1;
            //    if (CheckSceneExists() < 0 && CheckSceneExists(data.scene[data.sceneIndex].name) > -1)
            //    {
            //        List<TaskAtlasData.Scene.Landmark> newLandmarks = new List<TaskAtlasData.Scene.Landmark>();
            //        var oldLandmarks = data.scene[CheckSceneExists(data.scene[data.sceneIndex].name)].landmarks;

            //        for (int i = 0; i < data.scene[data.sceneIndexPrev].landmarks.Count; i++)
            //        {
            //            newLandmarks.Add(new TaskAtlasData.Scene.Landmark());
            //            newLandmarks[i].bScreenshot = oldLandmarks[i].bScreenshot;
            //            newLandmarks[i].bTotalTasks = oldLandmarks[i].bTotalTasks;
            //            newLandmarks[i].bOpenTasks = oldLandmarks[i].bOpenTasks;
            //            newLandmarks[i].bOverdue = oldLandmarks[i].bOverdue;
            //            newLandmarks[i].bUrgent = oldLandmarks[i].bUrgent;
            //            newLandmarks[i].bGoToTask = oldLandmarks[i].bGoToTask;
            //            newLandmarks[i].bCompleted = oldLandmarks[i].bCompleted;
            //            newLandmarks[i].bMoveSticky = oldLandmarks[i].bMoveSticky;

            //            newLandmarks[i].tScreenshot = oldLandmarks[i].tScreenshot;
            //            newLandmarks[i].tTotalTasks = oldLandmarks[i].tTotalTasks;
            //            newLandmarks[i].tOpenTasks = oldLandmarks[i].tOpenTasks;
            //            newLandmarks[i].tOverdue = oldLandmarks[i].tOverdue;
            //            newLandmarks[i].tUrgent = oldLandmarks[i].tUrgent;
            //            newLandmarks[i].tGoToTask = oldLandmarks[i].tGoToTask;
            //            newLandmarks[i].tCompleted = oldLandmarks[i].tCompleted;
            //            newLandmarks[i].tMoveSticky = oldLandmarks[i].tMoveSticky;

            //            newLandmarks[i].position = oldLandmarks[i].position;
            //            newLandmarks[i].floorPosition = oldLandmarks[i].floorPosition;
            //            newLandmarks[i].wsPosition = oldLandmarks[i].wsPosition;

            //            newLandmarks[i].rotation = oldLandmarks[i].rotation;

            //            newLandmarks[i].currentDistance = oldLandmarks[i].currentDistance;
            //            newLandmarks[i].orthographicSize = oldLandmarks[i].orthographicSize;

            //            newLandmarks[i].title = oldLandmarks[i].title;
            //            newLandmarks[i].description = oldLandmarks[i].description;

            //            newLandmarks[i].landmarkMoveTo = oldLandmarks[i].landmarkMoveTo;

            //            newLandmarks[i].showGizmo = oldLandmarks[i].showGizmo;
            //            newLandmarks[i].fadeGizmo = oldLandmarks[i].fadeGizmo;
            //            newLandmarks[i].showOptions = oldLandmarks[i].showOptions;
            //            newLandmarks[i].thumbnailLoaded = oldLandmarks[i].thumbnailLoaded;
            //            newLandmarks[i].fadeStart = oldLandmarks[i].fadeStart;
            //            newLandmarks[i].fadeEnd = oldLandmarks[i].fadeEnd;
            //            newLandmarks[i].taskFadeStart = oldLandmarks[i].taskFadeStart;
            //            newLandmarks[i].taskFadeEnd = oldLandmarks[i].taskFadeEnd;
            //            newLandmarks[i].taskFadeMax = oldLandmarks[i].taskFadeMax;

            //            newLandmarks[i].GizmoColor = oldLandmarks[i].GizmoColor;
            //            newLandmarks[i].tGizmoColor = oldLandmarks[i].tGizmoColor;
            //            newLandmarks[i].tGizmoColor50 = oldLandmarks[i].tGizmoColor50;
            //            newLandmarks[i].tGizmoColorDark = oldLandmarks[i].tGizmoColorDark;
            //            newLandmarks[i].tGizmoColorContrast = oldLandmarks[i].tGizmoColorContrast;

            //            newLandmarks[i].progress = oldLandmarks[i].progress;
            //            newLandmarks[i].tasksOpen = oldLandmarks[i].tasksOpen;
            //            newLandmarks[i].tasksUrgent = oldLandmarks[i].tasksUrgent;
            //            newLandmarks[i].tasksOverdue = oldLandmarks[i].tasksOverdue;
            //            newLandmarks[i].tasksHasAlert = oldLandmarks[i].tasksHasAlert;

            //            newLandmarks[i].timeStamp = oldLandmarks[i].timeStamp;

            //            //newLandmarks[i].tags = oldLandmarks[i].tags;

            //            newLandmarks[i].activeMinutes = oldLandmarks[i].activeMinutes;
            //            newLandmarks[i].idleMinutes = oldLandmarks[i].idleMinutes;
            //            newLandmarks[i].sleepMinutes = oldLandmarks[i].sleepMinutes;

            //            if (oldLandmarks[i].tasks != null)
            //                for (int t = 0; t < oldLandmarks[i].tasks.Count; t++)
            //                {
            //                    newLandmarks[i].tasks = (Task)new List<TaskAtlasData.Scene.Task>();
            //                    newLandmarks[i].tasks.Add(oldLandmarks[i].tasks[t].Copy());
            //                }
            //            //Copy Gallery
            //            if (oldLandmarks[i].gallery.images != null)
            //            {
            //                newLandmarks[i].gallery = new TaskAtlasData.Scene.Landmark.Gallery();
            //                if (newLandmarks[i].gallery.images == null)
            //                    newLandmarks[i].gallery.images = new List<TaskAtlasData.Scene.Landmark.Gallery.Image>();
            //                for (int img = 0; img < oldLandmarks[i].gallery.images.Count; img++)
            //                {
            //                    newLandmarks[i].gallery.images.Add(new TaskAtlasData.Scene.Landmark.Gallery.Image(oldLandmarks[i].gallery.images[img].image));
            //                    newLandmarks[i].gallery.images[img].scaleFactor = oldLandmarks[i].gallery.images[img].scaleFactor;
            //                    newLandmarks[i].gallery.images[img].scaleHeight = oldLandmarks[i].gallery.images[img].scaleHeight;
            //                    newLandmarks[i].gallery.images[img].scrollPos = oldLandmarks[i].gallery.images[img].scrollPos;
            //                }
            //            }


            //            newLandmarks[i].viewState = oldLandmarks[i].viewState;
            //        }

            //        data.scene.Add(new TaskAtlasData.Scene());

            //        data.sceneIndexPrev = data.sceneIndex;
            //        data.sceneIndex = data.scene.Count - 1;

            //        data.scene[data.sceneIndex].name = SceneManager.GetActiveScene().path;
            //        data.scene[data.sceneIndex].history = new List<TaskAtlasData.Scene.History>();
            //        data.scene[data.sceneIndex].landmarks = new List<TaskAtlasData.Scene.Landmark>();
            //        data.scene[data.sceneIndex].landmarks = newLandmarks;
            //        data.scene[data.sceneIndex].atlas = new TaskAtlasData.Scene.Atlas();
            //        if (data.scene[data.sceneIndexPrev].name == "")
            //        {
            //            data.scene.RemoveAt(data.sceneIndexPrev);
            //            data.sceneIndex = CheckSceneExists();
            //        }
            //    }
            //    else
            //    {
            //        data.sceneIndex = CheckSceneExists();
            //    }
            //}

            Init();

            TaskAtlasEditorWindowNew.scene = null;
            TaskAtlasEditorWindowNew.Init();
            TaskAtlasEditorWindowNew.isInit = true;


        }

        static void NewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            isV2 = false;
            dataV2 = null;
            Init();
            TaskAtlasEditorWindowNew.isInit = false;
            TaskAtlasSceneView.isInit = false;
        }

        static void SceneOpened(Scene scene, OpenSceneMode mode)
        {
            Init();
            TaskAtlasEditorWindowNew.isInit = false;
            TaskAtlasSceneView.isInit = false;
        }

        string m_ScriptFilePath;
        string m_ScriptFolder;
        string ReadSetupData()
        {
            MonoScript ms = MonoScript.FromScriptableObject(this);
            m_ScriptFilePath = AssetDatabase.GetAssetPath(ms);

            FileInfo fi = new FileInfo(m_ScriptFilePath);
            m_ScriptFolder = fi.Directory.ToString();

            string x = m_ScriptFolder;
            x = x.Replace("\\Editor\\Scripts", "");
            x = x.Replace("\\", "/");
            if (x.Contains("/Assets/")) x = x.Substring(x.IndexOf("Assets"));
            x = x + "//";
            return x.Replace("//", "/");//
        }

        static public string taPath = "";
        static public bool isData = false, isBuild = false, isInstalled = false;
        static public void Init()
        {


            isActive = true;

            Core c = ScriptableObject.CreateInstance<Core>();
            taPath = c.ReadSetupData();
            isV2 = true;

            //string[] allPaths;

            //isData = false; isBuild = false;

            //if (isV2)
            //{
            //    dataV2 = GameObject.Find("TaskAtlas").GetComponent<TaskAtlasDataV2>();

            //    Debug.Log(GameObject.Find("TaskAtlas"));
            //    Debug.Log(dataV2);
            //}
            //if (taPath == "" | !AssetDatabase.IsValidFolder(taPath))
            //{
            //    allPaths = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);
            //    for (int i = 0; i < allPaths.Count(); i++)
            //    {
            //        allPaths[i] = allPaths[i].Replace("\\", "/");
            //        if (allPaths[i].Contains("TaskAtlasDataDemo.asset"))
            //        {

            //            string p = Application.dataPath;
            //            p.Replace("Assets", "");
            //            taPath = allPaths[i].Substring(allPaths[i].IndexOf("Assets/"));
            //            taPath = taPath.Substring(0, taPath.IndexOf("/TaskAtlas/") + 11);
            //        }
            //        if (allPaths[i].Contains("TaskAtlasDataDemo.asset"))
            //        {
            //            isData = true;
            //        }
            //        if (allPaths[i].Contains("TaskAtlasBuildOn"))
            //        {
            //            isBuild = true;
            //        }
            //        if (allPaths[i].Contains("TaskAtlas")) isInstalled = true;
            //    }
            //}//

            if (File.Exists(taPath + "Data/TaskAtlasDataDemo.asset")) isData = true;
            if (File.Exists(taPath.Replace("TaskAtlas", "") + "TaskAtlasBuildOn")) isBuild = true;
            if (taPath.Contains("TaskAtlas")) isInstalled = true;

            if (!isInstalled) return;

            GameObject go = GameObject.Find("TaskAtlas");
            if (go == null) return;

            if (isV2) { }
            else
            if (!File.Exists(taPath + "Data/TaskAtlasData.asset") & File.Exists(taPath + "Data/TaskAtlasDataDemo.asset"))
            {
                Debug.Log("Setting up new Data file for Task Atlas...");
                //if (!File.Exists(Application.dataPath + taPath.Replace("Assets", "") + "Data/TaskAtlasDataDemo.asset"))
                File.Copy(Application.dataPath + taPath.Replace("Assets", "") + "Data/TaskAtlasDataDemo.asset",
                          Application.dataPath + taPath.Replace("Assets", "") + "Data/TaskAtlasData.asset");
                AssetDatabase.Refresh();
            }
            if (dataV1 == null)
            {
                dataV1 = (TaskAtlasData)AssetDatabase.LoadAssetAtPath(taPath + "Data/TaskAtlasData.asset", typeof(TaskAtlasData));
            }
            //if (dataV1 == null & !isBuild)
            //{
            //    {
            //        TaskAtlasData asset = ScriptableObject.CreateInstance<TaskAtlasData>();

            //        AssetDatabase.CreateAsset(asset, taPath + "Data/TaskAtlasData.asset");
            //        AssetDatabase.SaveAssets();

            //        dataV1 = (TaskAtlasData)AssetDatabase.LoadAssetAtPath(taPath + "Data/TaskAtlasData.asset", typeof(TaskAtlasData));
            //    }
            //}
            //if (isBuild) return;

            if (isV2) { }
            else
            {
                var data = dataV1;
                if (data.scene == null)
                {
                    {
                        data.scene = new List<TaskAtlasData.Scene>();
                        data.scene.Add(new TaskAtlasData.Scene());
                        data.sceneIndex = 0;
                        data.scene[data.sceneIndex].name = SceneManager.GetActiveScene().path;
                        data.scene[data.sceneIndex].history = new List<TaskAtlasData.Scene.History>();
                        data.scene[data.sceneIndex].landmarks = new List<TaskAtlasData.Scene.Landmark>();
                        data.scene[data.sceneIndex].atlas = new TaskAtlasData.Scene.Atlas();
                        Init();
                        TaskAtlasEditorWindowNew.isInit = false;
                        TaskAtlasSceneView.isInit = false;
                    }
                }
                else if (data.scene.Count == 0)
                {
                    {
                        data.scene = new List<TaskAtlasData.Scene>();
                        data.scene.Add(new TaskAtlasData.Scene());
                        data.sceneIndex = 0;
                        data.scene[data.sceneIndex].name = SceneManager.GetActiveScene().path;
                        data.scene[data.sceneIndex].history = new List<TaskAtlasData.Scene.History>();
                        data.scene[data.sceneIndex].landmarks = new List<TaskAtlasData.Scene.Landmark>();
                        data.scene[data.sceneIndex].atlas = new TaskAtlasData.Scene.Atlas();
                    }
                }
                else
                {
                    if (CheckSceneExists() < 0)
                    {
                        data.scene.Add(new TaskAtlasData.Scene());
                        data.sceneIndexPrev = data.sceneIndex;
                        data.sceneIndex = data.scene.Count - 1;
                        data.scene[data.sceneIndex].name = SceneManager.GetActiveScene().path;
                        data.scene[data.sceneIndex].history = new List<TaskAtlasData.Scene.History>();
                        data.scene[data.sceneIndex].landmarks = new List<TaskAtlasData.Scene.Landmark>();
                        data.scene[data.sceneIndex].atlas = new TaskAtlasData.Scene.Atlas();
                    }
                    else
                    {
                        data.sceneIndex = CheckSceneExists();
                    }
                }


                for (int i = 0; i < data.scene[data.sceneIndex].landmarks.Count; i++)
                {
                    if (data.scene[data.sceneIndex].landmarks[i].tGizmoColor == null) data.scene[data.sceneIndex].landmarks[i].SetColor(data.scene[data.sceneIndex].landmarks[i].GizmoColor);
                }
            }

            GUISkin skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
            cBackgroundPanels = EditorGUIUtility.isProSkin ?
                (Color)new Color32(56, 56, 56, 255) :
                (Color)new Color32(194, 194, 194, 255);
            tBackgroundPanels = MakeTex(1, 1, cBackgroundPanels);

            var col = EditorGUIUtility.isProSkin ?
                (Color)new Color32(68, 68, 68, 255) :
                (Color)new Color32(233, 233, 233, 255);
            tBackgroundMain = MakeTex(1, 1, col);

            tTransparent = MakeTex(1, 1, Color.clear);

            tGreenP = MakeTex(1, 1, cGreenP);
            tGreen = MakeTex(1, 1, cGreen);
            tTeal = MakeTex(1, 1, cTeal);
            tBlue = MakeTex(1, 1, cBlue);
            tRed = MakeTex(1, 1, cRed);
            tGrey = MakeTex(1, 1, cGrey);

            tGreenBright = DimBackground(1.5f, cGreen);
            tTealBright = DimBackground(1.5f, cTeal);
            tBlueBright = DimBackground(1.5f, cBlue);
            tRedBright = DimBackground(1.5f, cRed);
            tGreyBright = DimBackground(1.5f, cGrey);

            tGreenDark = DimBackground(0.5f, cGreen);
            tTealDark = DimBackground(0.5f, cTeal);
            tBlueDark = DimBackground(0.5f, cBlue);
            tRedDark = DimBackground(0.5f, cRed);
            tGreyDark = DimBackground(0.5f, cGrey);


            tWhite = MakeTex(1, 1, Color.white);
            tBlack = MakeTex(1, 1, Color.black);
            tYellow = MakeTex(1, 1, new Color32(120, 120, 0, 255));

            //ClearTaskAtlasCache();
            RefreshHelperCams();


            if (!go.TryGetComponent<TaskAtlasDataV2>(out dataV2))
            {
                go.AddComponent<TaskAtlasDataV2>();

            }
            dataV2 = go.GetComponent<TaskAtlasDataV2>();

            if (dataV2 == null || !dataV2.isMigrated) MigrateData();

            isV2 = true;
        }


        static public void SaveData()
        {
            if (isV2)
            {
                EditorUtility.SetDirty(dataV2);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            else
            {
                var data = dataV1;
                EditorUtility.SetDirty(data);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        #endregion

        #region HelperCameras
        static public void EnableLandmarkCamera()
        {
            if (LandmarkCamera)
            {
                LandmarkCamera.SetActive(true);
            }
        }
        static public void DisableLandmarkCamera()
        {
            if (LandmarkCamera)
            {
                LandmarkCamera.SetActive(false);
            }
        }

        //static public void ClearTaskAtlasCache()
        //{
        //    List<GameObject> gos = FindGameObjectsWithName("TaskAtlas");
        //    for (int i = 0; i < gos.Count; i++)
        //    {
        //        DestroyImmediate(gos[i]);
        //    }
        //}

        static List<GameObject> FindGameObjectsWithName(string name)
        {
            GameObject[] gos = GameObject.FindObjectsOfType<GameObject>();
            List<GameObject> spo = new List<GameObject>();
            for (int i = 0; i < gos.Length; i++)
            {
                if (gos[i].name.Contains(name))
                {
                    spo.Add(gos[i]);
                }
            }
            return spo;
        }


        #endregion


        #region DataLoading
        static public void CreateTaskAtlasObject()
        {

        }

        static public void RefreshHelperCams()
        {
            if (!LayerExists("TaskAtlas")) CreateLayer("TaskAtlas");

            if (TaskAtlasRoot == null)
            {
                GameObject go = GameObject.Find("TaskAtlas");
                //if (go == null)
                //TaskAtlasRoot = new GameObject("TaskAtlas");
                //else
                if (go != null) TaskAtlasRoot = go;
            }
            else
            {
                if (LandmarkCamera == null)
                {
                    var go = TaskAtlasRoot.transform.Find("TaskAtlasLandmarkCamera");
                    if (go == null)
                    {
                        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(taPath + "Prefabs/TaskAtlasLandmarkCamera.prefab");
                        if (prefab == null) return;
                        LandmarkCamera = Instantiate(prefab);
                        LandmarkCamera.name = LandmarkCamera.name.Replace("(Clone)", "");
                    }
                    else
                        LandmarkCamera = go.gameObject;

                    LandmarkCamera.layer = LayerMask.NameToLayer("TaskAtlas");
                    LandmarkCamera.transform.SetParent(TaskAtlasRoot.transform);
                    LandmarkCamera.SetActive(false);
                }

                if (StickyTextCam == null)
                {
                    var go = TaskAtlasRoot.transform.Find("TaskAtlasStickyTextCamera");
                    if (go == null)
                    {
                        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(taPath + "Prefabs/TaskAtlasStickyTextCamera.prefab");
                        if (prefab == null) return;
                        StickyTextCam = Instantiate(prefab);
                        StickyTextCam.name = StickyTextCam.name.Replace("(Clone)", "");
                    }
                    else
                        StickyTextCam = go.gameObject;

                    StickyTextCam.GetComponent<Camera>().cullingMask = 1 << LayerMask.NameToLayer("TaskAtlas");
                    StickyTextCam.layer = LayerMask.NameToLayer("TaskAtlas");
                    StickyTextCam.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("TaskAtlas");
                    StickyTextCam.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("TaskAtlas");
                    StickyTextCam.transform.position = new Vector3(10000, 10000, 10000);
                    StickyTextCam.transform.SetParent(TaskAtlasRoot.transform);
                    StickyTextCam.SetActive(false);
                }

                if (GizmoHelper == null)
                {
                    var go = TaskAtlasRoot.transform.Find("TaskAtlasGizmoHelper");
                    if (go == null)
                    {
                        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(taPath + "Prefabs/TaskAtlasGizmoHelper.prefab");
                        if (prefab == null) return;
                        GizmoHelper = Instantiate(prefab);
                        GizmoHelper.name = GizmoHelper.name.Replace("(Clone)", "");
                    }
                    else
                        GizmoHelper = go.gameObject;

                    GizmoHelper.layer = LayerMask.NameToLayer("TaskAtlas");
                    GizmoHelper.transform.position = new Vector3(10000, 10000, 10000);
                    GizmoHelper.transform.SetParent(TaskAtlasRoot.transform);
                }

                if (GizmoHelper != null && gizmos == null)
                {
                    gizmos = GizmoHelper.GetComponent<TaskAtlasGizmos>();
                }
            }

            if (rtStickyText == null) rtStickyText = (RenderTexture)AssetDatabase.LoadAssetAtPath(taPath + "UI/rtStickyText.renderTexture", typeof(RenderTexture));
            if (rtSS == null) rtSS = (RenderTexture)AssetDatabase.LoadAssetAtPath(taPath + "UI/rtSS.renderTexture", typeof(RenderTexture));
        }
        //
        //static public void RemoveHelperCams()
        //{
        //    ClearTaskAtlasCache();
        //}

        static public void RefreshIcons()
        {
            if (tGreen == null) Init();

            string s = "";
            if (EditorGUIUtility.isProSkin) s = "DarkMode";

            if (tTrashCan == null) tTrashCan = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasTrashCan" + s + ".png", typeof(Texture2D));
            if (tEditPen == null) tEditPen = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasEditPen" + s + ".png", typeof(Texture2D));
            if (tSortAZ == null) tSortAZ = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasAZ" + s + ".png", typeof(Texture2D));
            if (tSortDate == null) tSortDate = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasClock" + s + ".png", typeof(Texture2D));
            if (tSortDist == null) tSortDist = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasDist" + s + ".png", typeof(Texture2D));
            if (tDate == null) tDate = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasDate" + s + ".png", typeof(Texture2D));
            if (tArrange == null) tArrange = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasArrange" + s + ".png", typeof(Texture2D));

            if (tArrowLeft == null) tArrowLeft = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasLeftArrow" + s + ".png", typeof(Texture2D));
            if (tArrowRight == null) tArrowRight = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasRightArrow" + s + ".png", typeof(Texture2D));
            if (tArrowUp == null) tArrowUp = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasUpArrow" + s + ".png", typeof(Texture2D));
            if (tArrowDown == null) tArrowDown = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasDownArrow" + s + ".png", typeof(Texture2D));

            if (tAdd == null) tAdd = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasAdd.png", typeof(Texture2D));
            if (tRefresh == null) tRefresh = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasRefresh" + s + ".png", typeof(Texture2D));
            if (tGallery == null) tGallery = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasGallery" + ".png", typeof(Texture2D));
            if (tPosition == null) tPosition = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasPosition" + s + ".png", typeof(Texture2D));
            if (tPlus == null) tPlus = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasPlus.png", typeof(Texture2D));
            if (tMinus == null) tMinus = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasMinus.png", typeof(Texture2D));

            if (tIconCheck == null) tIconCheck = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconCheck.png", typeof(Texture2D));
            if (tIconX == null) tIconX = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconX.png", typeof(Texture2D));
            if (tIconExclaim == null) tIconExclaim = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconExclaim.png", typeof(Texture2D));
            if (tIconPin == null) tIconPin = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconPin.png", typeof(Texture2D));
            if (tIconTask == null) tIconTask = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasTask" + s + ".png", typeof(Texture2D));
            if (tIconTag == null) tIconTag = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasTag" + s + ".png", typeof(Texture2D));

            if (tAtlasEnable == null) tAtlasEnable = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconAtlasEnable.png", typeof(Texture2D));
            if (tAtlasDisable == null) tAtlasDisable = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconAtlasDisable.png", typeof(Texture2D));
            if (tAtlasRotLeft == null) tAtlasRotLeft = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconAtlasRotLeft.png", typeof(Texture2D));
            if (tAtlasRotRight == null) tAtlasRotRight = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconAtlasRotRight.png", typeof(Texture2D));
            if (tAtlasTop == null) tAtlasTop = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconAtlasTop.png", typeof(Texture2D));
            if (tAtlasSide == null) tAtlasSide = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconAtlasSide.png", typeof(Texture2D));

            if (tAtlasArrowIn == null) tAtlasArrowIn = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconAtlasArrowIn.png", typeof(Texture2D));
            if (tAtlasArrowOut == null) tAtlasArrowOut = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconAtlasArrowOut.png", typeof(Texture2D));
            if (tAtlasArrowLeft == null) tAtlasArrowLeft = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconAtlasArrowLeft.png", typeof(Texture2D));
            if (tAtlasArrowRight == null) tAtlasArrowRight = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconAtlasArrowRight.png", typeof(Texture2D));
            if (tAtlasEditPen == null) tAtlasEditPen = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconAtlasEditPen.png", typeof(Texture2D));
            if (tAtlasSettings == null) tAtlasSettings = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconSettings.png", typeof(Texture2D));
            if (tAtlasGO == null) tAtlasGO = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconAtlasGO.png", typeof(Texture2D));

            if (tTaskSticky == null) tTaskSticky = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskSticky.png", typeof(Texture2D));

            if (tGradGrey2Clear == null) tGradGrey2Clear = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasGradGrey2Clear.png", typeof(Texture2D));
            if (tGradClear2Grey == null) tGradClear2Grey = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasGradClear2Grey.png", typeof(Texture2D));

            if (tIcon2D == null) tIcon2D = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIcon2D.png", typeof(Texture2D));
            if (tIcon3D == null) tIcon3D = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIcon3D.png", typeof(Texture2D));
            if (tIconISO == null) tIconISO = (Texture2D)AssetDatabase.LoadAssetAtPath(taPath + "UI/TaskAtlasIconISO.png", typeof(Texture2D));
        }
        #endregion

        #region Utils
        static public void MigrateData()
        {
            RefreshHelperCams();
            //GameObject go = GameObject.Find("TaskAtlas");
            //if (!go.TryGetComponent<TaskAtlasDataV2>(out dataV2))
            //{
            //    go.AddComponent<TaskAtlasDataV2>();
            //    dataV2 = go.GetComponent<TaskAtlasDataV2>();
            //}

            if (dataV2.isMigrated)
            {
                return;
            }

            Debug.Log("Migrating data automatically to scene Game Object called Task Atlas, do NOT delete this object!");

            //var newTags = dataV2.tags;
            var oldTags = dataV1.tags;

            dataV2.tags = new List<Tags>();
            for (int t = 0; t < oldTags.Count; t++)
            {
                dataV2.tags.Add(new Tags(oldTags[t].name, ""));
                //dataV2.tags[t].active = oldTags[t].active;
            }


            dataV2.scene = new List<TA>();
            dataV2.scene.Add(new TA());
            if (CheckSceneExists() >= 0)
            {
                var oldData = dataV1.scene[dataV1.sceneIndex];
                var newData = dataV2.scene[0];
                newData.name = oldData.name;
                newData.sortMode = oldData.sortMode;
                newData.showLandmarkLabels = oldData.showLandmarkLabels;
                newData.showTaskGizmos = oldData.showTaskGizmos;
                newData.showStickies = oldData.showStickies;
                newData.landmarkDetailState = (TA.LandmarkDetailState)oldData.landmarkDetailState;
                newData.selectedLandmark = oldData.selectedLandmark;
                newData.zoomToTaskID = oldData.zoomToTaskID;
                newData.scrollPosLandmarkDetail = oldData.scrollPosLandmarkDetail;
                newData.scrollPosLandmarkDetailGeneral = oldData.scrollPosLandmarkDetailGeneral;
                newData.scrollPosLandmarkDetailTags = oldData.scrollPosLandmarkDetailTags;
                newData.scrollPosLandmarkDetailGallery = oldData.scrollPosLandmarkDetailGallery;
                newData.scrollPosLandmarkDetailGalleryThumbnails = oldData.scrollPosLandmarkDetailGalleryThumbnails;
                newData.scrollPosLandmarkDetailDelete = oldData.scrollPosLandmarkDetailDelete;
                newData.scrollPosLandmark = oldData.scrollPosLandmark;
                newData.scrollPosLandmarkSort = oldData.scrollPosLandmarkSort;
                newData.scrollPosLandmarkTags = oldData.scrollPosLandmarkTags;
                newData.scrollPosAtlasTasks = oldData.scrollPosAtlasTasks;
                newData.scrollPosAtlasLandmarkZoom = oldData.scrollPosAtlasLandmarkZoom;
                newData.scrollPosAtlasTaskZoom = oldData.scrollPosAtlasTaskZoom;
                newData.landmarkDetailOpen = oldData.landmarkDetailOpen;
                newData.landmarkThumbnailsLoaded = oldData.landmarkThumbnailsLoaded;
                newData.count = oldData.count;

                newData.landmarks = new List<Landmark>();
                var oldLandmarks = oldData.landmarks;
                var newLandmarks = newData.landmarks;

                for (int i = 0; i < oldLandmarks.Count; i++)
                {
                    //newLandmarks.Add(new Scene.Landmark(oldLandmarks[i].tScreenshot, oldLandmarks[i].position, oldLandmarks[i].rotation, oldLandmarks[i].orthographicSize));
                    newLandmarks.Add(new Landmark());
                    //newLandmarks. = oldLandmarks. ;
                    newLandmarks[i].bScreenshot = oldLandmarks[i].bScreenshot;
                    newLandmarks[i].bTotalTasks = oldLandmarks[i].bTotalTasks;
                    newLandmarks[i].bOpenTasks = oldLandmarks[i].bOpenTasks;
                    newLandmarks[i].bOverdue = oldLandmarks[i].bOverdue;
                    newLandmarks[i].bUrgent = oldLandmarks[i].bUrgent;
                    newLandmarks[i].bGoToTask = oldLandmarks[i].bGoToTask;
                    newLandmarks[i].bCompleted = oldLandmarks[i].bCompleted;
                    newLandmarks[i].bMoveSticky = oldLandmarks[i].bMoveSticky;
                    newLandmarks[i].tScreenshot = oldLandmarks[i].tScreenshot;
                    newLandmarks[i].tTotalTasks = oldLandmarks[i].tTotalTasks;
                    newLandmarks[i].tOpenTasks = oldLandmarks[i].tOpenTasks;
                    newLandmarks[i].tOverdue = oldLandmarks[i].tOverdue;
                    newLandmarks[i].tGoToTask = oldLandmarks[i].tGoToTask;
                    newLandmarks[i].tCompleted = oldLandmarks[i].tCompleted;
                    newLandmarks[i].tMoveSticky = oldLandmarks[i].tMoveSticky;
                    newLandmarks[i].position = oldLandmarks[i].position;
                    newLandmarks[i].floorPosition = oldLandmarks[i].floorPosition;
                    newLandmarks[i].wsPosition = oldLandmarks[i].wsPosition;
                    newLandmarks[i].rotation = oldLandmarks[i].rotation;
                    newLandmarks[i].currentDistance = oldLandmarks[i].currentDistance;
                    newLandmarks[i].orthographicSize = oldLandmarks[i].orthographicSize;
                    newLandmarks[i].title = oldLandmarks[i].title;
                    newLandmarks[i].description = oldLandmarks[i].description;
                    newLandmarks[i].landmarkMoveTo = oldLandmarks[i].landmarkMoveTo;
                    newLandmarks[i].showGizmo = oldLandmarks[i].showGizmo;
                    newLandmarks[i].fadeGizmo = oldLandmarks[i].fadeGizmo;
                    newLandmarks[i].showOptions = oldLandmarks[i].showOptions;
                    newLandmarks[i].thumbnailLoaded = oldLandmarks[i].thumbnailLoaded;
                    newLandmarks[i].fadeStart = oldLandmarks[i].fadeStart;
                    newLandmarks[i].fadeEnd = oldLandmarks[i].fadeEnd;
                    newLandmarks[i].taskFadeStart = oldLandmarks[i].taskFadeStart;
                    newLandmarks[i].taskFadeEnd = oldLandmarks[i].taskFadeEnd;
                    newLandmarks[i].taskFadeMax = oldLandmarks[i].taskFadeMax;
                    newLandmarks[i].viewState = (Landmark.ViewState)oldLandmarks[i].viewState;
                    newLandmarks[i].GizmoColor = oldLandmarks[i].GizmoColor;
                    newLandmarks[i].tGizmoColor = oldLandmarks[i].tGizmoColor;
                    newLandmarks[i].tGizmoColor50 = oldLandmarks[i].tGizmoColor50;
                    newLandmarks[i].tGizmoColorDark = oldLandmarks[i].tGizmoColorDark;
                    newLandmarks[i].tGizmoColorContrast = oldLandmarks[i].tGizmoColorContrast;
                    newLandmarks[i].progress = oldLandmarks[i].progress;
                    newLandmarks[i].tasksOpen = oldLandmarks[i].tasksOpen;
                    newLandmarks[i].tasksUrgent = oldLandmarks[i].tasksUrgent;
                    newLandmarks[i].tasksOverdue = oldLandmarks[i].tasksOverdue;
                    newLandmarks[i].tasksHasAlert = oldLandmarks[i].tasksHasAlert;
                    newLandmarks[i].timeStamp = oldLandmarks[i].timeStamp;
                    newLandmarks[i].activeMinutes = oldLandmarks[i].activeMinutes;
                    newLandmarks[i].idleMinutes = oldLandmarks[i].idleMinutes;
                    newLandmarks[i].sleepMinutes = oldLandmarks[i].sleepMinutes;
                    newLandmarks[i].taskBounds = oldLandmarks[i].taskBounds;

                    var oldLMTags = oldLandmarks[i].tags;
                    //var newLMTags = newLandmarks[i].tags;
                    //dataV2.scene[0].landmarks[i].tags = new List<Tags>();
                    //var newLMTags = newLandmarks[i].tags;

                    for (int lm = 0; lm < dataV1.tags.Count; lm++)
                    {
                        dataV2.tags.Add(new Tags(dataV1.tags[lm].name, ""));
                        //dataV2.scene[0].landmarks[i].tags[lm].active = false;
                        //for (int od = 0; od < oldLMTags.Count; od++)
                        //{
                        //    if (dataV1.tags[lm].name == oldLMTags[od].name) dataV2.scene[0].landmarks[i].tags[lm].active = oldLMTags[od].active;
                        //}
                    }


                    newLandmarks[i].tasks = new List<Task>();
                    var oldTask = oldLandmarks[i].tasks;
                    var newTask = newLandmarks[i].tasks;
                    for (int t = 0; t < oldTask.Count; t++)
                    {

                        newTask.Add(new Task(""));
                        newTask[t].bTaskName = oldTask[t].bTaskName;
                        newTask[t].bTaskDescription = oldTask[t].bTaskDescription;
                        newTask[t].bTaskNameSmall = oldTask[t].bTaskNameSmall;
                        newTask[t].bActiveTime = oldTask[t].bActiveTime;
                        newTask[t].bSubTasks = oldTask[t].bSubTasks;
                        newTask[t].bCompleted = oldTask[t].bCompleted;
                        newTask[t].bStage = oldTask[t].bStage;
                        newTask[t].bPriority = oldTask[t].bPriority;
                        newTask[t].tTaskName = oldTask[t].tTaskName;
                        newTask[t].tTaskDescription = oldTask[t].tTaskDescription;
                        newTask[t].tTaskNameSmall = oldTask[t].tTaskNameSmall;
                        newTask[t].tActiveTime = oldTask[t].tActiveTime;
                        newTask[t].tSubTasks = oldTask[t].tSubTasks;
                        newTask[t].tCompleted = oldTask[t].tCompleted;
                        newTask[t].tStage = oldTask[t].tStage;
                        newTask[t].tPriority = oldTask[t].tPriority;
                        newTask[t].name = oldTask[t].name;
                        newTask[t].description = oldTask[t].description;
                        newTask[t].activeTime = oldTask[t].activeTime;
                        newTask[t].selectedSubTask = oldTask[t].selectedSubTask;
                        newTask[t].month = (Task.Month)oldTask[t].month;
                        newTask[t].day = oldTask[t].day;
                        newTask[t].year = oldTask[t].year;
                        newTask[t].createdDateDT = oldTask[t].createdDateDT;
                        newTask[t].dueDateDT = oldTask[t].dueDateDT;
                        newTask[t].createdDate = oldTask[t].createdDate;
                        newTask[t].dueDate = oldTask[t].dueDate;
                        newTask[t].priority = (Task.Priority)oldTask[t].priority;
                        newTask[t].stage = (Task.Stage)oldTask[t].stage;
                        newTask[t].progress = oldTask[t].progress;
                        newTask[t].alert = oldTask[t].alert;
                        newTask[t].isSticky = oldTask[t].isSticky;
                        newTask[t].position = oldTask[t].position;
                        newTask[t].wsPosition = oldTask[t].wsPosition;
                        newTask[t].moveMode = oldTask[t].moveMode;
                        newTask[t].scale = oldTask[t].scale;
                        newTask[t].fadeStart = oldTask[t].fadeStart;
                        newTask[t].fadeEnd = oldTask[t].fadeEnd;
                        newTask[t].dist = oldTask[t].dist;
                        newTask[t].alpha = oldTask[t].alpha;
                        newTask[t].useDefaultFadeDistance = oldTask[t].useDefaultFadeDistance;
                        newTask[t].color = oldTask[t].color;
                        newTask[t].tColor = oldTask[t].tColor;
                        newTask[t].tColor50 = oldTask[t].tColor50;
                        newTask[t].tColorDark = oldTask[t].tColorDark;
                        newTask[t].tColorContrast = oldTask[t].tColorContrast;
                        newTask[t].activeMinutes = oldTask[t].activeMinutes;
                        newTask[t].idleMinutes = oldTask[t].idleMinutes;
                        newTask[t].sleepMinutes = oldTask[t].sleepMinutes;
                        newTask[t].timeRegionCenter = oldTask[t].timeRegionCenter;
                        newTask[t].timeRegionSize = oldTask[t].timeRegionSize;
                        newTask[t].timeTrackingDistance = oldTask[t].timeTrackingDistance;
                        newTask[t].timeTrackingDistanceCurrent = oldTask[t].timeTrackingDistanceCurrent;
                        newTask[t].timeTrackingDistanceScale = oldTask[t].timeTrackingDistanceScale;
                        newTask[t].autoTimer = oldTask[t].autoTimer;
                        newTask[t].editTimers = oldTask[t].editTimers;
                        newTask[t].isTracking = oldTask[t].isTracking;
                        newTask[t].showDistanceSphere = oldTask[t].showDistanceSphere;
                        newTask[t].isVisible = oldTask[t].isVisible;
                        newTask[t].isFolded = oldTask[t].isFolded;
                        newTask[t].timeRegion = oldTask[t].timeRegion;
                        newTask[t].subTasksAutoProgress = oldTask[t].subTasksAutoProgress;

                        newTask[t].subTasks = new List<SubTask>();
                        var oldST = oldTask[t].subTasks;
                        var newST = newTask[t].subTasks;

                        if (oldST != null)
                            for (int st = 0; st < oldST.Count; st++)
                            {
                                newST.Add(new SubTask(""));
                                newST[st].bTaskName = oldST[st].bTaskName;
                                newST[st].tTaskName = oldST[st].tTaskName;
                                newST[st].complete = oldST[st].complete;
                                newST[st].name = oldST[st].name;
                            }

                    }
                    newLandmarks[i].gallery = new Landmark.Gallery();
                    var oldGallery = oldLandmarks[i].gallery;
                    var newGallery = newLandmarks[i].gallery;

                    newGallery.currentImage = oldGallery.currentImage;
                    newGallery.position = oldGallery.position;
                    newGallery.wsPosition = oldGallery.wsPosition;
                    newGallery.moveMode = oldGallery.moveMode;
                    newGallery.scale = oldGallery.scale;
                    newGallery.fadeStart = oldGallery.fadeStart;
                    newGallery.fadeEnd = oldGallery.fadeEnd;
                    newGallery.dist = oldGallery.dist;
                    newGallery.alpha = oldGallery.alpha;
                    newGallery.useDefaultFadeDistance = oldGallery.useDefaultFadeDistance;
                    newGallery.thumbnailLoaded = oldGallery.thumbnailLoaded;

                    newGallery.images = new List<Landmark.Gallery.Image>();
                    var oldGI = oldGallery.images;
                    var newGI = newGallery.images;
                    for (int g = 0; g < oldGI.Count; g++)
                    {
                        newGI.Add(new Landmark.Gallery.Image(new Texture2D(1, 1)));
                        newGI[g].image = oldGI[g].image;
                        newGI[g].scrollPos = oldGI[g].scrollPos;
                        newGI[g].scaleHeight = oldGI[g].scaleHeight;
                        newGI[g].scaleFactor = oldGI[g].scaleFactor;
                    }
                }
            }

            dataV2.scene[0].showLandmarkLabels = dataV2.scene[0].showStickies = dataV2.scene[0].showTaskGizmos = true;

            dataV2.scene[0].landmarkDetailOpen = false;

            isV2 = true;
            dataV2.isMigrated = true;

            EditorUtility.SetDirty(dataV2);
            Init();
            TaskAtlasEditorWindowNew.Init();
            TaskAtlasSceneView.isInit = false;
            CompilationPipeline.RequestScriptCompilation();
        }

        public class ColorTexture
        {
            public Texture2D texture;
            public Color color;
        }

        static public List<ColorTexture> colorTextures;

        static public Texture2D MakeTex(int width, int height, Color col)
        {
            if (width == 0 | height == 0 | width < 0 | height < 0)
                width = height = 1;

            if (colorTextures == null) colorTextures = new List<ColorTexture>();
            for (int i = 0; i < colorTextures.Count; i++)
            {
                if (col == colorTextures[i].color && colorTextures[i].texture != null)
                {
                    return colorTextures[i].texture;
                }
            }
            //Debug.Log("w: " + width + " h: " + height);
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            colorTextures.Add(new ColorTexture() { texture = result, color = col });
            colorTextures[colorTextures.Count - 1].texture.name = "Core Texture " + col.ToString();

            return result;
        }

        public static Texture2D DimBackground(float pct, Color color, float alpha = .3f)
        {
            float r, g, b;
            r = (color.r * pct);
            g = (color.g * pct);
            b = (color.b * pct);

            return MakeTex(1, 1, new Color(r, g, b, alpha));
        }

        private static int maxLayers = 31;

        public void AddNewLayer(string name)
        {
            CreateLayer(name);
        }

        public void DeleteLayer(string name)
        {
            RemoveLayer(name);
        }

        public static bool CreateLayer(string layerName)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            if (!PropertyExists(layersProp, 0, maxLayers, layerName))
            {
                SerializedProperty sp;
                for (int i = 8, j = maxLayers; i < j; i++)
                {
                    sp = layersProp.GetArrayElementAtIndex(i);
                    if (sp.stringValue == "")
                    {
                        sp.stringValue = layerName;
                        tagManager.ApplyModifiedProperties();
                        return true;
                    }
                }
            }
            return false;
        }

        public static string NewLayer(string name)
        {
            if (name != null || name != "")
            {
                CreateLayer(name);
            }

            return name;
        }

        public static bool RemoveLayer(string layerName)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");

            if (PropertyExists(layersProp, 0, layersProp.arraySize, layerName))
            {
                SerializedProperty sp;

                for (int i = 0, j = layersProp.arraySize; i < j; i++)
                {

                    sp = layersProp.GetArrayElementAtIndex(i);

                    if (sp.stringValue == layerName)
                    {
                        sp.stringValue = "";
                        tagManager.ApplyModifiedProperties();
                        return true;
                    }
                }
            }

            return false;

        }

        public static bool LayerExists(string layerName)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            return PropertyExists(layersProp, 0, maxLayers, layerName);
        }

        private static bool PropertyExists(SerializedProperty property, int start, int end, string value)
        {
            for (int i = start; i < end; i++)
            {
                SerializedProperty t = property.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }

        public static Vector3 GetSVCameraPosition()
        {
            var cameras = SceneView.GetAllSceneCameras();
            Vector3 r = Vector3.zero;
            for (int i = 0; i < cameras.Length; i++)
            {
                if (SceneView.currentDrawingSceneView != null)
                {
                    if (SceneView.currentDrawingSceneView.camera.transform.position == cameras[i].transform.position) r = cameras[i].transform.position;
                    break;
                }

                if (SceneView.lastActiveSceneView != null)
                    if (SceneView.lastActiveSceneView.camera.transform.position == cameras[i].transform.position) r = cameras[i].transform.position;

            }
            return r;
        }

        public static Quaternion GetSVCameraRotation()
        {
            var cameras = SceneView.GetAllSceneCameras();
            Quaternion r = Quaternion.identity;
            for (int i = 0; i < cameras.Length; i++)
            {
                if (SceneView.currentDrawingSceneView != null)
                {
                    if (SceneView.currentDrawingSceneView.camera.transform.position == cameras[i].transform.position) r = cameras[i].transform.rotation;
                    break;
                }

                if (SceneView.lastActiveSceneView != null)
                    if (SceneView.lastActiveSceneView.camera.transform.position == cameras[i].transform.position) r = cameras[i].transform.rotation;
            }
            return r;
        }

        public static float GetSVCOrthographicSize()
        {
            var cameras = SceneView.GetAllSceneCameras();
            float r = 0;
            for (int i = 0; i < cameras.Length; i++)
            {
                if (SceneView.currentDrawingSceneView != null)
                {
                    if (SceneView.currentDrawingSceneView.camera.transform.position == cameras[i].transform.position) r = SceneView.currentDrawingSceneView.camera.orthographicSize;
                    break;
                }

                if (SceneView.lastActiveSceneView != null)
                    if (SceneView.lastActiveSceneView.camera.transform.position == cameras[i].transform.position) r = SceneView.lastActiveSceneView.camera.orthographicSize;
            }
            return r;
        }

        public static SceneView GetSceneView()
        {
            if (SceneView.lastActiveSceneView != null) return SceneView.lastActiveSceneView;
            if (SceneView.currentDrawingSceneView != null) return SceneView.currentDrawingSceneView;
            return null;
        }


        #endregion
    }
}