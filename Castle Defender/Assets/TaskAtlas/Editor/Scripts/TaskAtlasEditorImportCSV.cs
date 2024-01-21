using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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

    public class TaskAtlasEditorImportCSV : EditorWindow
    {
        //        public static EditorWindow window;
        //        static int queueRepaint = 0;
        //        public static TA scene;

        //        //public GUIStyle s;

        //        static float localWidth;

        //        static GUIStyle sBackgroundPanels, sBackground, sTaskHeader, s;
        //        static float screenWidth, screenHeight, screenWidthMargin;
        //        //static int panelHeight = 128;

        //        public static TextAsset csvFile; // Reference of CSV file

        //        [MenuItem("Window/ShrinkRay Entertainment/TaskAtlas/Import CSV")]
        //        private static void OpenLandmarksWindow()
        //        {
        //            window = GetWindow<TaskAtlasEditorImportCSV>();

        //            window.minSize = new Vector2(400, 600);
        //            // window.maxSize = new Vector2(640, 4000);
        //            window.Show();
        //            window.titleContent.text = "Task Atlas Data CSV";
        //        }

        //        public static void QueueRepaint()
        //        {
        //            if (window == null) window = GetWindow<TaskAtlasEditorWindowNew>();
        //            window.minSize = new Vector2(380, 600);
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

        //        public class LandmarkData
        //        {
        //            public string name;
        //            public string task;
        //            public string taskText;
        //            public int lmIndex, tIndex;
        //            public int priority = 0;
        //            public int stage = 0;
        //            public float lm_x = 0, lm_y = 0, lm_z = 0, t_x = 0, t_y = 0, t_z = 0;
        //            public int sticky = 0;

        //            public LandmarkData(string n, string t, string tt, int p, int s, float lx, float ly, float lz, float tx, float ty, float tz, int st)
        //            {
        //                name = n;
        //                task = t;
        //                taskText = tt;
        //                priority = p;
        //                stage = s;
        //                lm_x = lx;
        //                lm_y = ly;
        //                lm_z = lz;
        //                t_x = tx;
        //                t_y = ty;
        //                t_z = tz;
        //                sticky = st;
        //                lmIndex = 0;
        //                tIndex = 0;
        //            }
        //        }

        //        static public List<LandmarkData> newLandmarks = new List<LandmarkData>();
        //        static public List<LandmarkData> existingLandmarks = new List<LandmarkData>();

        //        void OnGUI()
        //        {
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

        //            if (scene == null)
        //                scene = Core.dataV2.scene[Core.dataV2.sceneIndex];

        //            GUILayout.BeginArea(new Rect(screenWidthMargin, 8, screenWidth, screenHeight));
        //            {
        //                if (DrawTaskHeader("Load CSV", Core.tAdd, Core.tGreen, true, 130, 24, new GUIContent("Load CSV")))
        //                {
        //                    existingLandmarks = new List<LandmarkData>();
        //                    newLandmarks = new List<LandmarkData>();

        //                    readData();

        //                    Debug.Log("Headers: " + csvLandmarkImportWithTasks.ncol());
        //                    for (int r = 0; r < csvLandmarkImportWithTasks.nrow(); r++)
        //                    {
        //                        Debug.Log("ROW " + r + "/" + csvLandmarkImportWithTasks.nrow());
        //                        String[] content = csvLandmarkImportWithTasks.row[r].ToArray();
        //                        string landmarkName = "";
        //                        string taskName = "";
        //                        string taskText = "";
        //                        int priority = 0;
        //                        int stage = 0;
        //                        float lm_x = 0, lm_y = 0, lm_z = 0, t_x = 0, t_y = 0, t_z = 0;
        //                        int sticky = 0;
        //                        for (int h = 0; h < content.Length; h++)
        //                        {
        //                            Debug.Log("Header: " + csvLandmarkImportWithTasks.headers[h]);
        //                            Debug.Log(" | Content: " + content[h]);

        //                            string header = csvLandmarkImportWithTasks.headers[h];
        //                            Debug.LogError(header);
        //                            switch (header)
        //                            {
        //                                case "LandmarkName":
        //                                    landmarkName = content[h];
        //                                    Debug.LogError("Landmark Name is " + landmarkName);
        //                                    break;
        //                                case "TaskName":
        //                                    taskName = content[h];
        //                                    break;
        //                                case "TaskText":
        //                                    taskText = content[h];
        //                                    break;
        //                                case "Stage":
        //                                    int.TryParse(content[h], out stage);
        //                                    break;
        //                                case "Priority":
        //                                    int.TryParse(content[h], out priority);
        //                                    break;
        //                                case "Sticky":
        //                                    int.TryParse(content[h], out sticky);
        //                                    break;
        //                                case "lm_x":
        //                                    float.TryParse(content[h], out lm_x);
        //                                    break;
        //                                case "lm_y":
        //                                    float.TryParse(content[h], out lm_y);
        //                                    break;
        //                                case "lm_z":
        //                                    float.TryParse(content[h], out lm_z);
        //                                    break;
        //                                case "t_x":
        //                                    float.TryParse(content[h], out t_x);
        //                                    break;
        //                                case "t_y":
        //                                    float.TryParse(content[h], out t_y);
        //                                    break;
        //                                case "t_z":
        //                                    float.TryParse(content[h], out t_z);
        //                                    break;

        //                            }
        //                        }

        //                        Debug.Log("Adding Data...");

        //                        bool foundLandmark = false, foundTask = false;
        //                        int indexLandmark = 0, indexTask = 0, indexList = 0;
        //                        for (int i = 0; i < scene.landmarks.Count; i++)
        //                        {
        //                            Debug.LogWarning("scene.landmarks[i].title == landmarkName || " + scene.landmarks[i].title + "==" + landmarkName);
        //                            if (scene.landmarks[i].title == landmarkName) { foundLandmark = true; indexLandmark = i; }

        //                        }


        //                        if (foundLandmark)
        //                        {
        //                            existingLandmarks.Add(new LandmarkData(landmarkName, taskName, taskText, priority, stage, lm_x, lm_y, lm_z, t_x, t_y, t_z, sticky));
        //                            indexList = existingLandmarks.Count - 1;
        //                            for (int i = 0; i < scene.landmarks[indexLandmark].tasks.Count; i++)
        //                            {
        //                                if (scene.landmarks[indexLandmark].tasks[i].name == taskName) { foundTask = true; indexTask = i; }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            newLandmarks.Add(new LandmarkData(landmarkName, taskName, taskText, priority, stage, lm_x, lm_y, lm_z, t_x, t_y, t_z, sticky));
        //                            indexLandmark = ImportLandmark(new Vector3(lm_x, lm_y, lm_z), Quaternion.identity);
        //                            indexList = newLandmarks.Count - 1;
        //                            Landmark l = scene.landmarks[indexLandmark];
        //                            l.title = landmarkName;
        //                        }



        //                        if (!foundTask)
        //                        {
        //                            Landmark l = scene.landmarks[indexLandmark];
        //                            l.tasks.Add(new Task(taskName));
        //                            indexTask = l.tasks.Count - 1;

        //                        }

        //                        Task t = scene.landmarks[indexLandmark].tasks[indexTask];
        //                        t.description = taskText;
        //                        if (sticky == 0) t.isSticky = false; else t.isSticky = true;
        //                        t.position = new Vector3(t_x, t_y, t_z);
        //                        t.stage = (Task.Stage)stage;
        //                        t.priority = (Task.Priority)priority;

        //                        if (foundLandmark)
        //                        {
        //                            existingLandmarks[indexList].lmIndex = indexLandmark;
        //                            existingLandmarks[indexList].tIndex = indexTask;
        //                        }
        //                        else
        //                        {
        //                            newLandmarks[indexList].lmIndex = indexLandmark;
        //                            newLandmarks[indexList].tIndex = indexTask;
        //                        }

        //                        csvLandmarkImportWithTasks.showData = true;
        //                    }
        //                }

        //                //
        //                csvFile = (TextAsset)EditorGUILayout.ObjectField(csvFile, typeof(TextAsset), false);

        //                if (csvLandmarkImportWithTasks.showData)
        //                {
        //                    int indexLandmark = 0, indexList = 0;
        //                    if (csvLandmarkImportWithTasks.row.Count == 0) return;

        //                    s = new GUIStyle("label");
        //                    s.fontSize = 20;
        //                    s.fontStyle = FontStyle.Bold;
        //                    GUILayout.BeginHorizontal();
        //                    GUILayout.Label("New Landmarks", s);
        //                    if (GUILayout.Button("Import All Below", GUILayout.Width(120)))
        //                    {

        //                    }
        //                    GUILayout.EndHorizontal();

        //                    for (int r = 0; r < newLandmarks.Count; r++)
        //                    {
        //                        HorizontalLine(Color.white);
        //                        s = StyleCheck("button");
        //                        GUILayout.Label(newLandmarks[r].name, s);
        //                        GUILayout.Label("Task Name:" + newLandmarks[r].task);
        //                        GUILayout.Label(newLandmarks[r].taskText);
        //                        if (GUILayout.Button("Import this Task Only"))
        //                        {
        //                            indexLandmark = ImportLandmark(new Vector3(newLandmarks[r].lm_x, newLandmarks[r].lm_y, newLandmarks[r].lm_z), Quaternion.identity);
        //                            indexList = newLandmarks.Count - 1;
        //                            Landmark l = scene.landmarks[indexLandmark];
        //                            l.title = newLandmarks[r].name;
        //                        }
        //                    }

        //                    if (newLandmarks.Count == 0) GUILayout.Label("None");

        //                    HorizontalLine(Color.black);
        //                    HorizontalLine(Color.black);
        //                    HorizontalLine(Color.black);
        //                    s = new GUIStyle("label");
        //                    s.fontSize = 20;
        //                    s.fontStyle = FontStyle.Bold;
        //                    GUILayout.BeginHorizontal();
        //                    GUILayout.Label("Update Landmarks", s);
        //                    if (GUILayout.Button("Import All Below", GUILayout.Width(120)))
        //                    {

        //                    }
        //                    GUILayout.EndHorizontal();
        //                    for (int r = 0; r < existingLandmarks.Count; r++)
        //                    {
        //                        HorizontalLine(Color.white);
        //                        s = StyleCheck("button");
        //                        GUILayout.Label(existingLandmarks[r].name, s);
        //                        GUILayout.Label("Task Name:" + existingLandmarks[r].task);
        //                        GUILayout.Label(existingLandmarks[r].taskText);
        //                        if (GUILayout.Button("Import this Task Only"))
        //                        {
        //                            Debug.Log("Landmark #: " + existingLandmarks[r].lmIndex);
        //                            Debug.Log("Landmarks: " + scene.landmarks.Count);
        //                            Debug.Log(scene.landmarks[existingLandmarks[r].lmIndex]);
        //                            Debug.Log("Task #: " + existingLandmarks[r].tIndex);
        //                        }
        //                    }


        //                }
        //            }
        //            GUILayout.EndArea();

        //            queueRepaint++;
        //        }




        //        static public InputField rollNoInputField;// Reference of rollno input field
        //        static public InputField nameInputField; // Reference of name input filed
        //        static public Text contentArea; // Reference of contentArea where records are displayed

        //        static private char lineSeperater = '\n'; // It defines line seperate character
        //        static private char fieldSeperator = ','; // It defines field seperate chracter

        //        class CSVLandmarkImportWithTasks
        //        {
        //            public List<String> headers = new List<string>();
        //            public List<List<string>> row = new List<List<string>>();
        //            public List<bool> rowSelect = new List<bool>();

        //            public bool showData = false;

        //            public int ncol() { return headers.Count; }
        //            public int nrow() { return row.Count - 1; }
        //        }
        //        static CSVLandmarkImportWithTasks csvLandmarkImportWithTasks = new CSVLandmarkImportWithTasks();
        //        static

        //        void Start()
        //        {
        //            readData();
        //        }
        //        // Read data from CSV file
        //        static private void readData()
        //        {
        //            csvLandmarkImportWithTasks = new CSVLandmarkImportWithTasks();
        //            string[] records = csvFile.text.Split(lineSeperater);
        //            //string record;

        //            for (int r = 0; r < records.Length; r++)
        //            {
        //                string[] fields = records[r].Split(fieldSeperator);
        //                string t;
        //                List<string> content = new List<string>();

        //                foreach (string field in fields)
        //                {
        //                    t = field;
        //                    Debug.Log(t);
        //                    if (r == 0)
        //                    {
        //                        csvLandmarkImportWithTasks.headers.Add(t);
        //                    }
        //                    else
        //                    {
        //                        content.Add(t);
        //                    }
        //                    //contentArea.text += t;
        //                }
        //                if (r > 0)
        //                {
        //                    csvLandmarkImportWithTasks.row.Add(content);
        //                    csvLandmarkImportWithTasks.rowSelect.Add(true);
        //                }
        //                //contentArea.text += '\n';
        //            }
        //        }
        //        // Add data to CSV file
        //        public void addData()
        //        {
        //            // Following line adds data to CSV file
        //            File.AppendAllText(getPath() + "/Assets/StudentData.csv", lineSeperater + rollNoInputField.text + fieldSeperator + nameInputField.text);
        //            // Following lines refresh the edotor and print data
        //            rollNoInputField.text = "";
        //            nameInputField.text = "";
        //            contentArea.text = "";
        //#if UNITY_EDITOR
        //            UnityEditor.AssetDatabase.Refresh();
        //#endif
        //            readData();
        //        }

        //        // Get path for given CSV file
        //        private static string getPath()
        //        {
        //#if UNITY_EDITOR
        //            return Application.dataPath;
        //#elif UNITY_ANDROID
        //return Application.persistentDataPath;// +fileName;
        //#elif UNITY_IPHONE
        //return GetiPhoneDocumentsPath();// +"/"+fileName;
        //#else
        //return Application.dataPath;// +"/"+ fileName;
        //#endif
        //        }
        //        // Get the path in iOS device
        //        private static string GetiPhoneDocumentsPath()
        //        {
        //            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
        //            path = path.Substring(0, path.LastIndexOf('/'));
        //            return path + "/Documents";
        //        }

        //        static bool DrawTaskHeader(string text, Texture2D icon, Texture2D color, bool hasButton = false, int bWidth = 0, int bHeight = 0, GUIContent bContent = null, GUIStyle bStyle = null, int tWidth = 128, int iSize = 32)
        //        {
        //            bool ret = false;
        //            GUILayout.Space(8);
        //            GUIStyle s = StyleCheck();
        //            s.alignment = TextAnchor.MiddleLeft;
        //            s.fontSize = 14;
        //            s.fontStyle = FontStyle.Bold;
        //            sBackground = StyleBack(sBackground, color);
        //            GUILayout.BeginHorizontal(sBackground, GUILayout.Height(iSize));
        //            {
        //                GUILayout.Space(4);
        //                GUILayout.BeginVertical();
        //                GUILayout.Space(10);
        //                GUILayout.Box(icon, GUIStyle.none, GUILayout.Width(iSize), GUILayout.Height(iSize));
        //                GUILayout.EndVertical();
        //                GUILayout.BeginVertical();
        //                GUILayout.Space(16);
        //                GUILayout.BeginHorizontal();
        //                GUILayout.Space(16);
        //                GUILayout.Label(text, s, GUILayout.Width(tWidth));
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

        //        static public int ImportLandmark(Vector3 position, Quaternion rotation)
        //        {
        //            if (Core.rtSS == null) Core.RefreshHelperCams();
        //            Core.EnableLandmarkCamera();
        //            if (Core.GetSceneView().in2DMode)
        //            {
        //                Core.LandmarkCamera.transform.position = new Vector3(position.x, position.y, -1000f);
        //            }
        //            else
        //            {
        //                Core.LandmarkCamera.transform.position = position;
        //            }

        //            Core.LandmarkCamera.transform.rotation = rotation;
        //            Core.LandmarkCamera.GetComponent<Camera>().orthographicSize = Core.GetSVCOrthographicSize();



        //            scene.landmarks.Add(
        //                new Landmark(
        //                    Core.rtSS,
        //                    Core.LandmarkCamera.transform.position,
        //                    Core.LandmarkCamera.transform.rotation,
        //                    Core.GetSVCOrthographicSize())
        //            );

        //            //scene.landmarks[scene.selectedLandmark].tags = new List<Tags>();
        //            //for (int i = 0; i < Core.dataV2.tags.Count; i++)
        //            //{
        //            //    scene.landmarks[scene.selectedLandmark].tags.Add(new Tags(Core.dataV2.tags[i].name));
        //            //}
        //            Core.DisableLandmarkCamera();
        //            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        //            return scene.landmarks.Count - 1;
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
    }


}