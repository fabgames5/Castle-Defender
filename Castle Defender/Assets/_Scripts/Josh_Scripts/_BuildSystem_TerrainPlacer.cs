using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class _BuildSystem_TerrainPlacer : MonoBehaviour
{

    [Tooltip("Set this to True, for placing objects ")]
    private bool placeobjectMousePos = false;
    [Space(5)]
    [Tooltip("Add Terrain here")]
    public Terrain terrain;
   // [Tooltip("Add NavMesh Setup script here")]
  //  public NavmeshSetup navMeshSetup;
    [Tooltip("Select all layers that should be ignored while placing buildings, Note: Terrain and buildings must not be checked")]
    public LayerMask layersMaskIgnore = new LayerMask();
    [Tooltip("Select only the Placing Layer")]
    public LayerMask placingLayer = new LayerMask();
    public int placingLayerRef = -1;
    public int buildingLayerRef = 5;

    [Space(5)]
    [Tooltip("Dynamically sets a list of placeable prefabs based on current  building style")]
    public List<GameObject> stylePrefabBuildings = new List<GameObject>();
    private List<int> jsonRefID = new List<int>();
  //  [Tooltip("Add All building Jsons here")]
  //  public JsonAsset[] buildingsJsons;
    [Tooltip("Reference ID to the current prefab that is selected to be placed")]
    public int prefabRef;
    public MeshCollider mC;
    public Vector3 placementOffset = Vector3.zero;
    public Rigidbody rb;
    public BoxCollider boxCollider;
  //  public JBR_Enums.buildingStyle currentStyleBuilding = JBR_Enums.buildingStyle.Building;

    [Space(5)]
    public bool isConnected;
    public bool isContacted;
    public int refColor = 1;
    public Color[] colors = { Color.white, Color.green, Color.yellow, Color.red };
    [Space(5)]
    [Tooltip("the mouse location in Screen Space")]
    public float2 mouseLocation;
    [Tooltip("the hit position in 3d Space")]
    public Vector3 hitPosition;
    [Tooltip("the current Rotation of the placing prefab")]
    public float prefabRot = 0;

    [Space(5)]
   // public UIControl uiControl;
    public GameObject uiThumbnailPrefab;
  //  public UIControl uiVerticalRight;

    //private
    private GameObject newPrefab;
    private Material[] slots = new Material[0];
    private Material[] materialInstances;
    private int count = 0;
    private int jsonRef = -1;

    [Tooltip("List of all building placed, during game")]
    public List<_BuildSystem_Construction> placedBuildings = new List<_BuildSystem_Construction>();
    [Space(5)]
    public bool startSession = false;
    public bool stopSession = true;
    [Space(5)]
    // current build session
    private List<_BuildSystem_Construction> sessionBuildings = new List<_BuildSystem_Construction>();
    public List<BuildingSaveLocation> buildingSaveLocations = new List<BuildingSaveLocation>();
    // all the buildin corners for regenerating Navmesh locations
    private List<Vector3> buildingVectorPoints = new List<Vector3>();
    //navmesh locations to regenerate
    private List<int> navRegions = new List<int>();

    [Space(5)]
    [Header("Saving")]
    public string DataSaveName = "PlacerSave.txt";
    
    [Tooltip("Dynamically set, the generated path to put the save text file")]
    public string textAssetPath = "";
    private string Text_Data;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetConnection(bool canConnect)
    {
        Debug.Log("Connected >>>>>>>>>>>>>");
        isConnected = canConnect;
    }

    [System.Serializable]
    public class BuildingSaveLocation
    {
        public int buildingReference;
        public float buildingRotation;
        public Vector3 buildingPosition;
    }
}
