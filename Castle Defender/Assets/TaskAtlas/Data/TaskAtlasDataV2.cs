#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;


namespace TaskAtlasNamespace
{
    [ExecuteInEditMode]
    public class TaskAtlasDataV2 : MonoBehaviour
    {
        [SerializeField]
        public string version = "3.1.0";

        [SerializeField]
        public bool isMigrated = false;



        [SerializeField]
        public int sceneIndex = 0, sceneIndexPrev = 0;



        [SerializeField]
        public List<TA> scene;

        [SerializeField]
        public List<Tags> tags;

        [SerializeField]
        public string newTag;








    }



    [Serializable]
    public class ColorTexture
    {
        [NonSerialized]
        public Texture2D texture;
        [SerializeField]
        public Color color;
    }


    [Serializable]
    public class LandmarkTag
    {
        [SerializeField]
        public string id;
        public bool active;

        public LandmarkTag(string _id, bool _active)
        {
            id = _id;
            active = _active;
        }
    }

    [Serializable]
    public class Tags
    {
        [SerializeField]
        public string name, newName;
        //[SerializeField]
        //public bool active;
        public List<LandmarkTag> landmarkTag = new List<LandmarkTag>();

        public Tags(string n, string id)// = "")
        {
            name = n;
            if (id != "")
                landmarkTag.Add(new LandmarkTag(id, false));
            //active = false;
        }

        public bool GetActive(string id)
        {
            for (int i = 0; i < landmarkTag.Count; i++)
            {
                if (landmarkTag[i].id == id)
                {
                    return landmarkTag[i].active;
                }
            }
            return false;
        }

        public void SetActive(string id, bool _active)
        {
            //Debug.Log("Setting active on Tag " + name + " on landmark id: " + id);
            bool found = false;
            for (int i = 0; i < landmarkTag.Count; i++)
            {
                if (landmarkTag[i].id == id)
                {
                    //Debug.Log("Found it!");
                    landmarkTag[i].active = _active;
                    found = true;
                }

            }
            if (!found)
            {
                //Debug.Log("Not found, adding it!)");
                landmarkTag.Add(new LandmarkTag(id, _active));
            }
        }
    }

    [SerializeField]
    public enum AtlasState { landmarks, tasks };

    [SerializeField]
    public enum StickyBackground { note, modern };

    [SerializeField]
    public enum StickyFont { normal, rpg, scifi, western, toon, custom };

    [Serializable]
    public class TA
    {
        [SerializeField]
        public string name = "";
        [SerializeField]
        public bool sortMode = false;
        [SerializeField]
        public bool showLandmarkLabels = true, showTaskGizmos = true, showStickies = true, sceneSettings = false;

        [SerializeField]
        public StickyBackground stickyBackground = StickyBackground.modern;

        [SerializeField]
        public string stickyFontPath = "", stickyFontDefaultPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF";
        public TMP_FontAsset stickyFont;

        [SerializeField]
        public enum LandmarkDetailState { general, tasks, delete, gallery }
        [SerializeField]
        public LandmarkDetailState landmarkDetailState;
        [SerializeField]
        public int selectedLandmark = 0;
        [SerializeField]
        public string zoomToTaskID;
        [SerializeField]
        public Vector2 scrollPosLandmarkDetail,
        scrollPosLandmarkDetailGeneral,
        scrollPosLandmarkDetailTags,
        scrollPosLandmarkDetailGallery,
        scrollPosLandmarkDetailGalleryThumbnails,
        scrollPosLandmarkDetailDelete,
        scrollPosLandmark,
        scrollPosLandmarkSort,
        scrollPosLandmarkTags,
        scrollPosAtlasTasks,
        scrollPosAtlasLandmarkZoom,
        scrollPosAtlasTaskZoom;
        [SerializeField]
        public bool landmarkDetailOpen = false, landmarkThumbnailsLoaded = false;
        public float stickyScaleMax = 100f;

        public TA()
        {
        }


        //[SerializeField]
        //public List<History> history;

        //[SerializeField]
        public List<Landmark> landmarks;


        public void UpdateAllProgress(TMP_FontAsset stickyFont, bool forceRefresh = false)
        {
            for (int i = 0; i < landmarks.Count; i++)
            {
                landmarks[i].UpdateProgress(stickyFont, forceRefresh);
            }
        }

        public void UpdateAllThumbnails()
        {
            for (int i = 0; i < landmarks.Count; i++)
            {
                if (!landmarks[i].thumbnailLoaded)
                {
                    landmarks[i].tScreenshot = landmarks[i].GetThumbnail();
                }
            }
        }

        public void CheckAllThumbnailsLoaded()
        {
            int count = 0;
            landmarkThumbnailsLoaded = false;
            for (int i = 0; i < landmarks.Count; i++)
            {
                if (!landmarks[i].thumbnailLoaded)
                    count++;
            }
            if (count == landmarks.Count) landmarkThumbnailsLoaded = true;
        }

        [SerializeField]
        public int count = 0;

        //[Serializable]
        //public class History
        //{
        //    [SerializeField]
        //    public byte[] bytes;
        //    [SerializeField]
        //    public Texture2D thumbnail;
        //    [SerializeField]
        //    public Vector3 position;
        //    [SerializeField]
        //    public Quaternion rotation;
        //    [SerializeField]
        //    public string timeStamp;
        //    [SerializeField]
        //    public DateTime timeStampTrue;

        //    public History(RenderTexture rt, Vector3 p, Quaternion r)
        //    {
        //        RenderTexture currentRT = RenderTexture.active;

        //        RenderTexture rtNew = RenderTexture.GetTemporary(rt.width, rt.height, 0, RenderTextureFormat.ARGB32);
        //        Graphics.Blit(rt, rtNew);
        //        RenderTexture.active = rtNew;

        //        Texture2D image = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        //        image.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        //        image.Apply();
        //        RenderTexture.active = currentRT;
        //        RenderTexture.ReleaseTemporary(rtNew);

        //        bytes = image.EncodeToPNG();
        //        DestroyImmediate(image);

        //        position = p;
        //        rotation = r;
        //        thumbnail = GetThumbnail();
        //        timeStamp = DateTime.Now.ToString("MM-dd hh:mm tt");
        //        timeStampTrue = DateTime.Now;
        //    }

        //    public History() { }

        //    public Texture2D GetThumbnail()
        //    {
        //        Texture2D tex = new Texture2D(2, 2);
        //        tex.LoadImage(bytes);
        //        thumbnail = tex;
        //        return tex;
        //    }
        //}


        [SerializeField]
        public Atlas atlas;

        //static public Texture2D MakeTex(int width, int height, Color col)
        //{
        //    if (colorTextures == null) colorTextures = new List<ColorTexture>();
        //    for (int i = 0; i < colorTextures.Count; i++)
        //    {
        //        if (col == colorTextures[i].color && colorTextures[i].texture != null)
        //        {
        //            return colorTextures[i].texture;
        //        }
        //    }
        //    Color[] pix = new Color[width * height];

        //    for (int i = 0; i < pix.Length; i++)
        //        pix[i] = col;

        //    Texture2D result = new Texture2D(width, height);
        //    result.SetPixels(pix);
        //    result.Apply();

        //    colorTextures.Add(new ColorTexture() { texture = result, color = col });

        //    return result;
        //}
    }

    [Serializable]
    public class Atlas
    {
        [SerializeField]
        public List<byte[]> bytes;
        [SerializeField]
        public List<Texture2D> thumbnail = null;

        [SerializeField]
        public int landmarkSelected;

        [SerializeField]
        public AtlasState state;
        [SerializeField]
        public Vector3[,,] viewPointsLandmarks;
        [SerializeField]
        public float[] viewPointSizesLandmarks;
        [SerializeField]
        public List<Vector3> focusPositions, viewPointsTasks;
        [SerializeField]
        public float[] viewPointSizesTasks;
        [SerializeField]
        public Vector3 prevPosition, legitPosition;
        [SerializeField]
        public Quaternion prevRotation;
        [SerializeField]
        public float prevSize;
        [SerializeField]
        public int tiltAngles = 8, rotAngles = 8, zoomLevels = 50,
        tiltCurrent, rotCurrent, zoomCurrent,
        landmarkFocus = 0, landmarkFocusCount = 0,
        taskFocus = 0, taskFocusCount = 0, taskZoom = 0, taskZoomLevels = 50;
        [SerializeField]
        public bool enabled, isZooming = false, settings = false;

        public Atlas()
        {
            enabled = false;
            state = AtlasState.landmarks;
            landmarkSelected = -1;
            focusPositions = new List<Vector3>();
        }

        public void AddThumbnail(RenderTexture rt)
        {
            RenderTexture currentRT = RenderTexture.active;

            RenderTexture rtNew = RenderTexture.GetTemporary(rt.width, rt.height, 0, RenderTextureFormat.Default);
            Graphics.Blit(rt, rtNew);
            RenderTexture.active = rtNew;

            Texture2D image = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
            image.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            image.Apply();

            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(rtNew);

            bytes.Add(image.EncodeToPNG());
            //DestroyImmediate(image);

            thumbnail.Add(GetThumbnail(bytes.Count - 1));
        }

        public void RefreshThumbnail(RenderTexture rt, int index)
        {
            RenderTexture currentRT = RenderTexture.active;

            RenderTexture rtNew = RenderTexture.GetTemporary(rt.width, rt.height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(rt, rtNew);
            RenderTexture.active = rtNew;

            Texture2D image = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
            image.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            image.Apply();
            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(rtNew);

            bytes[index] = image.EncodeToPNG();
            //DestroyImmediate(image);

            thumbnail[index] = GetThumbnail(index);
        }

        public Texture2D GetThumbnail(int index)
        {
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes[index]);
            return tex;
        }

    }


    [Serializable]
    public class Landmark
    {
        static GameObject parentObject, StickyTextCam;

        [SerializeField]
        static public List<ColorTexture> colorTextures;
        [SerializeField]
        public byte[] bScreenshot, bTotalTasks, bOpenTasks, bOverdue, bUrgent, bGoToTask, bCompleted, bMoveSticky;
        [NonSerialized]
        public Texture2D tScreenshot = null, tTotalTasks, tOpenTasks, tOverdue, tUrgent, tGoToTask, tCompleted, tMoveSticky;
        [SerializeField]
        public Vector3 position, floorPosition, wsPosition, wsOffset;
        [SerializeField]
        public GameObject floatingPositionObject;
        [SerializeField]
        public bool isFloating = false;
        [SerializeField]
        public Quaternion rotation;
        [SerializeField]
        public float currentDistance, orthographicSize;
        [SerializeField]
        public string title, description;
        [SerializeField]
        public Vector2 scrollbarDescription, scrollbarEditTags, scrollbarOptions, scrollbarTasks, scrollbarGallery;
        public enum EditorState { General, Tags, Tasks, Gallery, Options };
        [SerializeField]
        public EditorState editorState = EditorState.General;
        [SerializeField]
        public int landmarkMoveTo;
        [SerializeField]
        public bool showGizmo = true, fadeGizmo = true, showOptions = false, thumbnailLoaded = false;
        [SerializeField]
        public float fadeStart = 100, fadeEnd = 200;
        [SerializeField]
        public float taskFadeStart = 0, taskFadeEnd = 10, taskFadeMax = 100;
        [Serializable]
        public enum ViewState { is3D, is2D, isOrthographic };
        [SerializeField]
        public ViewState viewState = ViewState.is3D;
        //public string newTag;
        [SerializeField]
        public bool hideInactiveTags = false, editTitle = false;

        [SerializeField]
        public Color GizmoColor = Color.white;
        [NonSerialized]
        public Texture2D tGizmoColor, tGizmoColor50, tGizmoColorDark, tGizmoColorContrast;

        public Landmark()
        {
            title = "New Landmark";
        }

        public void SetColor(Color c)
        {
            GizmoColor = c;
            tGizmoColor = MakeTex(1, 1, c);
            tGizmoColor50 = MakeTex(1, 1, new Color(c.r, c.g, c.b, 0.5f));
            tGizmoColorDark = MakeTex(1, 1, new Color(c.r, c.g, c.b, 0.25f));

            Color.RGBToHSV(c, out float H, out float S, out float V);
            float negativeH = (H + 0.75f) % 1f;
            Color negativeColor = Color.HSVToRGB(negativeH, S, V);

            tGizmoColorContrast = MakeTex(1, 1, negativeColor);
        }

        [SerializeField]
        public float progress;
        [SerializeField]
        public int tasksOpen, tasksUrgent, tasksOverdue,
            _tasksOpen, _tasksUrgent, _tasksOverdue;
        [SerializeField]
        public bool tasksHasAlert;

        [SerializeField]
        public string timeStamp;

        //[SerializeField]
        //public List<Tags> tags, tagsTemp;

        [SerializeField]
        public long activeMinutes, idleMinutes, sleepMinutes;

        [SerializeField]
        public List<Task> tasks;
        public Bounds taskBounds;

        IEnumerable<char> CharsToTitleCase(string s)
        {
            bool newWord = true;
            foreach (char c in s)
            {
                if (newWord) { yield return Char.ToUpper(c); newWord = false; }
                else yield return Char.ToLower(c);
                if (c == ' ') newWord = true;
            }
        }
        public string toProper(string s)
        {
            var asTitleCase = new string(CharsToTitleCase(s).ToArray());
            return asTitleCase;
        }


        static RenderTexture rt;
        public void UpdateProgress(TMP_FontAsset stickyFont, bool forceRefresh = false)
        {
            long now = DateTime.Now.Ticks;
            TMP_FontAsset normalFont = (TMP_FontAsset)AssetDatabase.LoadAssetAtPath("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF", typeof(TMP_FontAsset));
            progress = 0;
            tasksOpen = tasksUrgent = tasksOverdue = 0;
            activeMinutes = idleMinutes = sleepMinutes = 0;

            //if (rt == null) rt = Resources.Load<RenderTexture>("rtStickyText");//new RenderTexture(512,512,24); //(RenderTexture)Type.GetType("TaskAtlasNamespace.Core, Assembly-CSharp-Editor").GetField("rtStickyText").GetValue(null);
            if (rt == null) rt = new RenderTexture(512, 512, 0, RenderTextureFormat.Default); //(RenderTexture)Type.GetType("TaskAtlasNamespace.Core, Assembly-CSharp-Editor").GetField("rtStickyText").GetValue(null);
            rt.autoGenerateMips = true;
            rt.antiAliasing = 4;
            rt.wrapMode = TextureWrapMode.Clamp;
            rt.filterMode = FilterMode.Bilinear;
            rt.anisoLevel = 0;

            if (tasks == null) tasks = new List<Task>();
            for (int t = 0; t < tasks.Count; t++)
            {
                activeMinutes += tasks[t].activeMinutes;
                idleMinutes += tasks[t].idleMinutes;
                sleepMinutes += tasks[t].sleepMinutes;

                if (tasks[t].stage != Task.Stage.complete)
                {
                    tasksOpen++;
                    if (tasks[t].priority == Task.Priority.urgent) tasksUrgent++;
                    if (tasks[t].alert && tasks[t].dueDateDT < now) tasksOverdue++;
                }
                string text = tasks[t].name, _text = tasks[t]._name, text2 = text;

                if (text.Length > 20) text2 = text.Substring(0, 17) + "...";
                if (tasks[t].tTaskName == null || text != _text || forceRefresh)
                {
                    CreateStickyText(text2, rt, out tasks[t].tTaskName, out tasks[t].bTaskName, Color.black, TextAlignmentOptions.Top, stickyFont, 1f);
                    tasks[t]._name = tasks[t].name;
                }

                if (text.Length > 20) text = text.Substring(0, 17) + "...";
                if (tasks[t].tExit == null)
                {
                    CreateStickyText("Exit Subtasks", rt, out tasks[t].tExit, out tasks[t].bTaskNameSmall, Color.white, TextAlignmentOptions.Top, normalFont, .5f);
                }

                text = tasks[t].description; _text = tasks[t]._description;
                if (tasks[t].tTaskDescription == null || text != _text || forceRefresh)
                {
                    CreateStickyText(text, rt, out tasks[t].tTaskDescription, out tasks[t].bTaskDescription, Color.black, TextAlignmentOptions.MidlineGeoAligned, stickyFont, 0, true);
                    tasks[t]._description = tasks[t].description;
                }

                if (tasks[t].tStage == null || toProper(tasks[t].stage.ToString()) != toProper(tasks[t]._stage.ToString()))
                {
                    CreateStickyText(toProper(tasks[t].stage.ToString()), rt, out tasks[t].tStage, out tasks[t].bStage, Color.black, TextAlignmentOptions.Top, normalFont, .5f);
                    tasks[t]._stage = tasks[t].stage;
                }

                if (tasks[t].tPriority == null || toProper(tasks[t].priority.ToString()) != toProper(tasks[t]._priority.ToString()))
                {
                    CreateStickyText(toProper(tasks[t].priority.ToString()), rt, out tasks[t].tPriority, out tasks[t].bPriority, Color.black, TextAlignmentOptions.Top, normalFont, .5f);
                    tasks[t]._priority = tasks[t].priority;
                }

                long am = tasks[t].activeMinutes;

                text = am / 1440 + "d " + am % 1440 / 60 + "h " + am % 60 + "m";
                if (am < 1440) text = am / 60 + "h " + am % 60 + "m";
                if (am < 60) text = am + "m";

                if (tasks[t].tActiveTime == null)
                {
                    CreateStickyText(text, rt, out tasks[t].tActiveTime, out tasks[t].bActiveTime, Color.black, TextAlignmentOptions.Top, normalFont, .5f);
                }

                if (tasks[t].tSubTasks == null || tasks[t].subTaskCount != tasks[t].subTasks.Count)
                {
                    CreateStickyText("View " + tasks[t].subTasks.Count + " Subtasks", rt, out tasks[t].tSubTasks, out tasks[t].bSubTasks, Color.white, TextAlignmentOptions.Top, normalFont, .5f);
                    tasks[t].subTaskCount = tasks[t].subTasks.Count;

                }

                int subComplete = 0;
                for (int st = 0; st < tasks[t].subTasks.Count; st++)
                {
                    if (tasks[t].subTasks[st].tTaskName == null || tasks[t].subTasks[st].name != tasks[t].subTasks[st]._name || forceRefresh)
                    {
                        CreateStickyText(tasks[t].subTasks[st].name, rt, out tasks[t].subTasks[st].tTaskName, out tasks[t].subTasks[st].bTaskName, Color.black, TextAlignmentOptions.MidlineGeoAligned, stickyFont, 0, true);
                        tasks[t].subTasks[st]._name = tasks[t].subTasks[st].name;
                    }

                    if (tasks[t].subTasksAutoProgress)
                    {

                        if (tasks[t].subTasks[st].complete) subComplete++;
                        tasks[t].progress = Mathf.FloorToInt(((float)subComplete / (float)tasks[t].subTasks.Count) * 100);
                    }

                }
                progress += tasks[t].progress;
            }

            progress = (float)((float)progress / ((float)tasks.Count * 100));

            tasksHasAlert = false;
            if (tasksUrgent + tasksOverdue > 0) tasksHasAlert = true;
            if (tOverdue == null)
                CreateStickyText("Overdue: " + tasksOverdue, rt, out tOverdue, out bOverdue, Color.black, TextAlignmentOptions.Top, normalFont, .5f);
            if (tUrgent == null)
                CreateStickyText("Urgent: " + tasksUrgent, rt, out tUrgent, out bUrgent, Color.black, TextAlignmentOptions.Top, normalFont, .5f);
            if (tOpenTasks == null)
                CreateStickyText("Open: " + tasksOpen, rt, out tOpenTasks, out bOpenTasks, Color.black, TextAlignmentOptions.Top, normalFont, .5f);

            if (tCompleted == null)
                CreateStickyText("Completed", rt, out tCompleted, out bCompleted, Color.black, TextAlignmentOptions.Top, normalFont, .5f);
            if (tGoToTask == null)
                CreateStickyText("Go To Task", rt, out tGoToTask, out bGoToTask, Color.white, TextAlignmentOptions.Top, normalFont, .5f);
            if (tMoveSticky == null)
                CreateStickyText("Move Sticky", rt, out tMoveSticky, out bMoveSticky, Color.white, TextAlignmentOptions.Top, normalFont, .5f);
        }

        void CreateStickyText(string t, RenderTexture rt, out Texture2D texture, out byte[] bytes, Color col, TextAlignmentOptions ta, TMP_FontAsset stickyFont, float fontSize = 0, bool wrapping = false)
        {
            //texture = new Texture2D(1,1);
            //bytes = new byte[10];
            //if (Core.StickyTextCam == null) Core.RefreshHelperCams();

            //var StickyTextCam = (GameObject)Type.GetType("TaskAtlasNamespace.Core, Assembly-CSharp-Editor").GetField("StickyTextCam").GetValue(null);
            //Debug.Log("Creting Sticky for " + t);

            if (StickyTextCam == null)
            {
                parentObject = GameObject.Find("TaskAtlas");
                if (parentObject == null)
                {
                    texture = MakeTex(1, 1, Color.black);
                    bytes = texture.EncodeToPNG();

                    return;
                }
                StickyTextCam = parentObject.transform.Find("TaskAtlasStickyTextCamera").gameObject;
            }

            if (StickyTextCam == null)
            {
                texture = MakeTex(1, 1, Color.black);
                bytes = texture.EncodeToPNG();

                return;
            }

            StickyTextCam.SetActive(true);
            StickyTextCam.GetComponent<Camera>().enabled = true;
            StickyTextCam.GetComponent<Camera>().targetTexture = rt;
            var tmp = StickyTextCam.GetComponentInChildren<TextMeshProUGUI>();
            tmp.enableWordWrapping = wrapping;
            tmp.alignment = ta;
            //switch(stickyFont)
            //{
            //    case StickyFont.normal: 
            //        tmp.font = (TMP_FontAsset)AssetDatabase.LoadAssetAtPath("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF", typeof(TMP_FontAsset));
            //        break;
            //    case StickyFont.rpg:
            //        tmp.font = (TMP_FontAsset)AssetDatabase.LoadAssetAtPath("Assets/Task Atlas/Fonts/RPG SDF.asset", typeof(TMP_FontAsset));
            //        break;
            //    case StickyFont.scifi:
            //        break;
            //    case StickyFont.western:
            //        break;
            //    case StickyFont.toon:
            //        break;
            //    case StickyFont.custom:
            //        break;
            //}
            tmp.font = stickyFont;
            tmp.color = col;
            if (fontSize > 0)
            {
                tmp.enableAutoSizing = false;
                tmp.fontSize = fontSize;
            }
            else
            {
                tmp.enableAutoSizing = true;
                tmp.fontSizeMin = 0f;
                tmp.fontSizeMax = 72f;
            }
            tmp.text = t;

            StickyTextCam.GetComponent<Camera>().Render();
            StickyTextCam.GetComponent<Camera>().enabled = false;

            RenderTexture currentRT = RenderTexture.active;

            if (rt == null) rt = new RenderTexture(1, 1, 24);
            RenderTexture rtNew = RenderTexture.GetTemporary(rt.width, rt.height, 0, RenderTextureFormat.Default);
            Graphics.Blit(rt, rtNew);
            RenderTexture.active = rtNew;
            RenderTexture.ReleaseTemporary(rtNew);


            texture = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            texture.Apply();

            RenderTexture.active = currentRT;

            bytes = texture.EncodeToPNG();
            StickyTextCam.SetActive(false);
        }

        public void UpdateTaskDistances(int type)
        {
            //Vector3 scenePos = Core.GetSVCameraPosition();

            //for (int i = 0; i < tasks.Count; i++)
            //{
            //    if (!tasks[i].autoTimer) continue;

            //    tasks[i].timeTrackingDistanceCurrent = Vector3.Distance(scenePos, tasks[i].position);
            //    if (viewState == ViewState.is2D)
            //    {
            //        tasks[i].isTracking = tasks[i].isVisible;
            //    }
            //    else
            //    {
            //        if (tasks[i].timeTrackingDistanceCurrent <= tasks[i].timeTrackingDistance) tasks[i].isTracking = true; else tasks[i].isTracking = false;
            //    }
            //    if (tasks[i].stage != Task.Stage.complete && tasks[i].isTracking == true)
            //    {
            //        switch (type)
            //        {
            //            case 0://ACTIVE
            //                tasks[i].activeMinutes++;
            //                break;
            //            case 1://IDLE
            //                tasks[i].idleMinutes++;
            //                break;
            //            case 2://SLEEPING
            //                tasks[i].sleepMinutes++;
            //                break;
            //        }

            //    }

            //}
        }

        [Serializable]
        public class Gallery
        {

            [SerializeField]
            public int currentImage = 0;


            [SerializeField]
            public Vector3 position, wsPosition;
            [SerializeField]
            public bool moveMode;

            [SerializeField]
            public float scale = 10f, fadeStart = 0f, fadeEnd = 10f, dist, alpha;
            [SerializeField]
            public bool useDefaultFadeDistance = true;

            public bool thumbnailLoaded;

            [Serializable]
            public class Image
            {
                [NonSerialized]
                public Texture2D image;

                [SerializeField]
                public Vector2 scrollPos;

                [SerializeField]
                public string imagePath = "";

                [SerializeField]
                public bool scaleHeight = false;

                [SerializeField]
                public float scaleFactor = 1.0f;

                [SerializeField]
                public float height = 0f;

                public Image(Texture2D texture)
                {
                    image = texture;
                }
            }



            [SerializeField]
            public List<Image> images = new List<Image>();

        }

        [SerializeField]
        public Gallery gallery;


        public Landmark(RenderTexture rt, Vector3 p, Quaternion r, float o)
        {
            thumbnailLoaded = false;
            Camera svc = SceneView.lastActiveSceneView.camera;
            RenderTexture currentRT = new RenderTexture(512, 512, 24);
            svc.targetTexture = currentRT;
            svc.Render();

            RenderTexture.active = currentRT;

            Texture2D image = new Texture2D(512, 512, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
            image.Apply();

            svc.targetTexture = null;
            RenderTexture.active = null;

            bScreenshot = image.EncodeToPNG();
            //DestroyImmediate(image);

            position = p;
            rotation = r;
            orthographicSize = o;

            RaycastHit hit;

            if (Physics.Raycast(p, Vector3.down, out hit, 10000))
            {
                floorPosition = hit.point;
            }

            timeStamp = DateTime.Now.Ticks.ToString();

            title = p.ToString();
            description = "Place Notes Here";

            showGizmo = true;
            showOptions = true;
            if (SceneView.lastActiveSceneView.in2DMode)
            {
                viewState = ViewState.is2D;
                fadeStart = 30;
                fadeEnd = 100;
                taskFadeEnd = 100;
            }
            if (!SceneView.lastActiveSceneView.in2DMode) viewState = ViewState.is3D;
            if (!SceneView.lastActiveSceneView.in2DMode & SceneView.lastActiveSceneView.orthographic) viewState = ViewState.isOrthographic;


            tScreenshot = GetThumbnail();
            tScreenshot.name = "Landmark Screenshot";

            GizmoColor = new Color(UnityEngine.Random.Range(0.1f, 0.9f), UnityEngine.Random.Range(0.1f, 0.9f), UnityEngine.Random.Range(0.1f, 0.9f));

            tasks = new List<Task>();
        }

        //public Landmark(Texture2D t2D, Vector3 p, Quaternion r, float o)
        //{
        //    thumbnailLoaded = false;
        //    Camera svc = SceneView.lastActiveSceneView.camera;
        //    RenderTexture currentRT = new RenderTexture(1024, 1024, 24);
        //    svc.targetTexture = currentRT;
        //    svc.Render();

        //    RenderTexture.active = currentRT;

        //    Texture2D image = new Texture2D(1024, 1024, TextureFormat.RGB24, false);
        //    image.ReadPixels(new Rect(0, 0, 1024, 1024), 0, 0);
        //    image.Apply();

        //    svc.targetTexture = null;
        //    RenderTexture.active = null;

        //    bScreenshot = image.EncodeToPNG();
        //    DestroyImmediate(image);

        //    position = p;
        //    rotation = r;
        //    orthographicSize = o;

        //    RaycastHit hit;

        //    if (Physics.Raycast(p, Vector3.down, out hit, 10000))
        //    {
        //        floorPosition = hit.point;
        //    }

        //    timeStamp = DateTime.Now.Ticks.ToString();

        //    title = p.ToString();
        //    description = "Place Notes Here";

        //    showGizmo = true;
        //    showOptions = true;
        //    if (SceneView.lastActiveSceneView.in2DMode)
        //    {
        //        viewState = ViewState.is2D;
        //        fadeStart = 30;
        //        fadeEnd = 100;
        //        taskFadeEnd = 100;
        //    }
        //    if (!SceneView.lastActiveSceneView.in2DMode) viewState = ViewState.is3D;
        //    if (!SceneView.lastActiveSceneView.in2DMode & SceneView.lastActiveSceneView.orthographic) viewState = ViewState.isOrthographic;


        //    tScreenshot = t2D;

        //    tasks = new List<Task>();
        //}

        //public Landmark() { }

        public void RefreshThumbnail(Camera c, float os = -1f, bool orthographic = false)
        {
            Vector3 pos = c.transform.position;
            Quaternion rot = c.transform.rotation;

            c.CopyFrom(SceneView.lastActiveSceneView.camera);
            if (os > 0) c.orthographicSize = os;

            c.transform.position = pos;
            c.transform.rotation = rot;
            c.orthographicSize = os;
            if (os > 0) c.orthographic = orthographic;

            RenderTexture currentRT = new RenderTexture(1024, 1024, 24);
            c.targetTexture = currentRT;
            c.Render();

            RenderTexture.active = c.targetTexture;

            Texture2D image = new Texture2D(1024, 1024, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 1024, 1024), 0, 0);
            image.Apply();

            RenderTexture.active = null;

            bScreenshot = image.EncodeToPNG();
            //DestroyImmediate(image);

            tScreenshot = GetThumbnail();
        }

        public Texture2D GetThumbnail()
        {
            Texture2D tex = new Texture2D(512, 512, TextureFormat.DXT5Crunched, false);
            tex.LoadImage(bScreenshot);
            tScreenshot = tex;
            tScreenshot.name = "Internal";
            if (tScreenshot == null)
            {
                thumbnailLoaded = false;
            }
            else
            {
                thumbnailLoaded = true;
            }

            return tex;
        }


        static public Texture2D MakeTex(int width, int height, Color col)
        {
            if (colorTextures == null) colorTextures = new List<ColorTexture>();
            for (int i = 0; i < colorTextures.Count; i++)
            {
                if (col == colorTextures[i].color && colorTextures[i].texture != null)
                {
                    return colorTextures[i].texture;
                }
            }
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            colorTextures.Add(new ColorTexture() { texture = result, color = col });
            colorTextures[colorTextures.Count - 1].texture.name = "Landmark Texture " + col.ToString();

            return result;
        }
    }

    [Serializable]
    public class Task
    {
        [SerializeField]
        static public List<ColorTexture> colorTextures;
        [SerializeField]
        public byte[] bTaskName, bTaskDescription, bTaskNameSmall, bActiveTime, bSubTasks, bCompleted, bStage, bPriority;
        [NonSerialized]
        public Texture2D tTaskName, tTaskDescription, tTaskNameSmall, tActiveTime, tSubTasks, tCompleted, tStage, tPriority, tExit;
        [SerializeField]
        public string name, _name = "_", description, _description = "_", activeTime;

        [SerializeField]
        public int selectedSubTask;

        [Serializable]
        public enum Month { Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec }

        [SerializeField]
        public Month month;
        [SerializeField]
        public int day, year;

        [SerializeField]
        public long createdDateDT, dueDateDT, selectedMonth = 10;
        [SerializeField]
        public string createdDate, dueDate;
        public DateTime selectedDate = DateTime.Now;

        [Serializable]
        public enum Priority { none, low, normal, high, urgent }

        [SerializeField]
        public Priority priority, _priority = Priority.urgent;
        [Serializable]
        public enum Stage { planned, active, testing, complete }

        [Serializable]
        public enum SelectedMenu { notes, subtasks, duedate, sticky, timer }
        [SerializeField]
        public SelectedMenu selectedMenu;
        public Vector2 selectedMenuScrollBar, selectedMenuNotesScrollBar, selectedMenuSubTasksScrollBar, selectedMenuDueDateScrollBar, selectedMenuStickyScrollBar, selectedMenuTimerScrollBar;

        public Stage stage, _stage = Stage.testing;
        [SerializeField]
        public int progress, _progress = 99999;

        [SerializeField]
        public bool alert, isSticky;

        [SerializeField]
        public Vector3 position, wsPosition;
        [SerializeField]
        public bool moveMode;

        [SerializeField]
        public float scale = 10f, fadeStart = 0f, fadeEnd = 10f, dist, alpha;
        [SerializeField]
        public bool useDefaultFadeDistance = true;

        [SerializeField]
        public Color color;
        [NonSerialized]
        public Texture2D tColor, tColor50, tColorDark, tColorContrast;

        public void SetColor(Color c)
        {
            color = c;
            tColor = MakeTex(1, 1, c);
            tColor50 = MakeTex(1, 1, new Color(c.r, c.g, c.b, 0.5f));
            tColorDark = MakeTex(1, 1, new Color(c.r * 0.3f, c.g * 0.3f, c.b * 0.3f, 1f));
            tColorContrast = MakeTex(1, 1, new Color(1 - c.r, 1 - c.g, 1 - c.b, 1f));
        }

        [SerializeField]
        public long activeMinutes, idleMinutes, sleepMinutes;
        [SerializeField]
        public Vector3 timeRegionCenter, timeRegionSize;
        [SerializeField]
        public float timeTrackingDistance, timeTrackingDistanceCurrent, timeTrackingDistanceScale = 10f;
        [SerializeField]
        public bool autoTimer, editTimers, isTracking, showDistanceSphere, isVisible, isFolded;
        public Bounds timeRegion;

        [SerializeField]
        public List<SubTask> subTasks = new List<SubTask>();
        public int subTaskCount = -1;
        [SerializeField]
        public bool subTasksAutoProgress;

        public Task(string t)
        {
            name = t;
            createdDateDT = DateTime.Now.Ticks;
            createdDate = createdDateDT.ToString();
            dueDateDT = DateTime.Now.Ticks;
            dueDate = dueDateDT.ToString();
            priority = Priority.normal;
            stage = Stage.planned;
            alert = false;
            month = (Month)DateTime.Now.Month;
            day = DateTime.Now.Day;
            year = DateTime.Now.Year;
            isSticky = false;
            position = new Vector3();
            moveMode = false;
            SetColor(new Color(UnityEngine.Random.Range(0.6f, 1f), UnityEngine.Random.Range(0.6f, 1f), UnityEngine.Random.Range(0.6f, 1f)));
            subTasks = new List<SubTask>();
            progress = 0;
            position = Vector3.zero;
            wsPosition = Vector3.zero;

        }

        public Task() { }

        public int GetMonth()
        {
            return (int)month + 1;
        }

        public void UpdateDueDate()
        {
            string m = ((int)month + 1).ToString();
            if ((int)month < 9) m = "0" + m;
            string d = day.ToString();
            if (day < 10) d = "0" + d;
            string date = year + "-" + m + "-" + d;
            dueDateDT = DateTime.Parse(date).Ticks;
            dueDate = dueDateDT.ToString();
        }

        public void UpdateDueDate(DateTime date)
        {
            dueDateDT = date.Ticks;
            dueDate = dueDateDT.ToString();
        }

        public Task Copy()
        {
            Task t = new Task(name);
            t.description = description;
            t.bTaskName = bTaskName; t.bTaskNameSmall = bTaskNameSmall; t.bActiveTime = bActiveTime; t.bSubTasks = bSubTasks; t.bCompleted = bCompleted; t.bStage = bStage; t.bPriority = bPriority;
            t.tTaskName = tTaskName; t.tTaskNameSmall = tTaskNameSmall; t.tActiveTime = tActiveTime; t.tSubTasks = tSubTasks; t.tCompleted = tCompleted; t.tStage = tStage; t.tPriority = tPriority;

            t.activeTime = activeTime;

            t.selectedSubTask = selectedSubTask;

            t.month = month;
            t.year = year;
            t.day = day;
            t.createdDateDT = createdDateDT; t.dueDateDT = dueDateDT;
            t.createdDate = createdDate; t.dueDate = dueDate;

            t.progress = progress;
            t.priority = priority;
            t.position = position;
            t.stage = stage;

            t.alert = alert; t.isSticky = isSticky;
            t.position = position; t.wsPosition = wsPosition;
            t.moveMode = moveMode;
            t.scale = scale; t.fadeStart = fadeStart; t.fadeEnd = fadeEnd;
            t.useDefaultFadeDistance = useDefaultFadeDistance;
            t.SetColor(color);

            t.activeMinutes = activeMinutes; t.idleMinutes = idleMinutes; t.sleepMinutes = sleepMinutes;
            t.timeRegionCenter = timeRegionCenter; t.timeRegionSize = timeRegionSize;
            t.timeTrackingDistance = timeTrackingDistance; t.timeTrackingDistanceCurrent = timeTrackingDistanceCurrent;
            t.autoTimer = autoTimer; t.editTimers = editTimers; t.isTracking = isTracking; t.showDistanceSphere = showDistanceSphere;
            t.timeRegion = timeRegion;

            t.subTasksAutoProgress = subTasksAutoProgress;

            for (int st = 0; st < subTasks.Count; st++)
            {
                t.subTasks.Add(new SubTask(subTasks[st].name));
                t.subTasks[st].complete = subTasks[st].complete;
                t.subTasks[st].bTaskName = subTasks[st].bTaskName;
                t.subTasks[st].tTaskName = subTasks[st].tTaskName;
            }

            return t;
        }

        public bool CheckChanged(Task prev)
        {
            bool ret = false;

            if (prev.progress != progress) ret = true;
            if (prev.priority != priority) ret = true;
            if (prev.position != position) ret = true;
            if (prev.stage != stage) ret = true;
            if (prev.month != month) ret = true;
            if (prev.day != day) ret = true;
            if (prev.year != year) ret = true;
            if (prev.color != color) ret = true;
            if (prev.isSticky != isSticky) ret = true;
            if (prev.dueDate != dueDate) ret = true;
            if (prev.name != name) ret = true;
            if (prev.description != description) ret = true;
            if (prev.subTasks.Count != subTasks.Count)
            {
                ret = true;
            }
            else
            {
                for (int st = 0; st < subTasks.Count; st++)
                {
                    if (prev.subTasks[st].complete != subTasks[st].complete) ret = true;
                    if (prev.subTasks[st].name != subTasks[st].name) ret = true;
                }
            }

            return ret;
        }
        static public Texture2D MakeTex(int width, int height, Color col)
        {
            if (colorTextures == null) colorTextures = new List<ColorTexture>();
            for (int i = 0; i < colorTextures.Count; i++)
            {
                if (col == colorTextures[i].color && colorTextures[i].texture != null)
                {
                    return colorTextures[i].texture;
                }
            }
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            colorTextures.Add(new ColorTexture() { texture = result, color = col });
            colorTextures[colorTextures.Count - 1].texture.name = "Task Texture " + col.ToString();

            return result;
        }
    }

    [Serializable]
    public class SubTask
    {
        [SerializeField]
        public byte[] bTaskName;
        [NonSerialized]
        public Texture2D tTaskName;
        [SerializeField]
        public bool complete = false;
        [SerializeField]
        public string name = "", _name = "_";

        public SubTask(string n)
        {
            name = n;
        }
    }

}//
#endif