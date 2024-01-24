using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UIElements;


public class _BuildSystem_TerrainPlacer : MonoBehaviour
{

    [Tooltip("Set this to True, for placing objects ")]
    public bool placeobjectMousePos = false;
    [Tooltip("if true rotating buildings is based on mouse wheel else its based on Q,E")]
    public bool useMouseScrollToRotate = false;
    [Space(5)]
    [Tooltip("Add Terrain here")]
    public Terrain terrain;
   // [Tooltip("Add NavMesh Setup script here")]
  //  public NavmeshSetup navMeshSetup;
    [Tooltip("Select the Terrain layer")]
    public LayerMask terrainLayer = new LayerMask();
    [Tooltip("Select only the Placing Layer, only one layer can be selected")]
    public LayerMask placingLayer = new LayerMask();
    private int placingLayerRef = -1;
    [Space(5)]
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
    [Tooltip("Dynamically Set, true if connected ")]
    public bool isConnected;
    [Tooltip("Dynamically Set, true if contact with another object ")]
    public bool isContacted;
    [Tooltip("the maximum distance the snapping effect can work from")]
    [Range(.25f,3.0f)]
    public float maxConnectionDistance = 1.0f;
    [Tooltip("Dynamically Set, color reference id ")]
    private int refColor = 1;
    //Colors depending on collison and placement
    private Color[] colors = { Color.white, Color.green, Color.yellow, Color.red };
    [Space(5)]
    [Tooltip("the mouse location in Screen Space")]
    public Vector3 mouseLocation;
    [Tooltip("the hit position in 3d Space")]
    public Vector3 hitPosition;

    [Tooltip("the current Rotation of the placing prefab")]
    public float prefabRot = 0;
    [Tooltip("the Rotation change allowed")]
    [SerializeField]
    private float rotatonAmount = 45;

    [Space(5)]
   // public UIControl uiControl;
    public GameObject uiThumbnailPrefab;
    //  public UIControl uiVerticalRight;

    //private
    [SerializeField]
    private GameObject newPrefab;
   // private Material[] slots = new Material[0];
   // private Material[] materialInstances;
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

    private Vector3 rots;

    // Start is called before the first frame update
    void Start()
    {       
        //get layer by number         
        int p = ToLayer(placingLayer);
        placingLayerRef = p;
        Debug.Log(placingLayer.ToString() + " " + p);
      
        bool isloaded = LoadListData();
        if (isloaded && buildingSaveLocations.Count > 0)
        {
            for (int i = 0; i < buildingSaveLocations.Count; i++)
            {
                // spawn prefab and add to placed building list..
            }
        }
    }

    //trigger enter
    public void OnTriggerEnters(Collider physicsActor, GameObject go)
    {
        if (physicsActor != terrain && go == newPrefab)
        {
            Debug.Log(newPrefab.name + " << TriggerEnter >> " + physicsActor);
            isContacted = true;
            refColor = 3;
        }
    }

    //trigger exit
    public void OnTriggerExits(Collider physicsActor, GameObject go)
    {
        if (physicsActor != terrain && go == newPrefab)
        {
            Debug.Log("TriggerExit " + physicsActor);
            isContacted = false;
            refColor = 1;

            if (newPrefab.GetComponent<_BuildSystem_Construction>().spherePoints.Count > 0)
            {
                refColor = 2;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //  Debug.Log(Input.MouseScrollDelta + " Mouse Scroll Delta");

        //mouse scroll middle only y is used
        if (useMouseScrollToRotate)
        {
            if (Input.mouseScrollDelta.y < 0)
            {
                prefabRot += rotatonAmount;
                if (prefabRot >= 360)
                {
                    prefabRot = -360 + prefabRot;
                }
            }

            //mouse scroll middle only y is used
            if (Input.mouseScrollDelta.y > 0)
            {
                prefabRot -= rotatonAmount;
                if (prefabRot <= -360)
                {
                    prefabRot = 360 + prefabRot;
                }
            }
        }
        //building rotation based on Q,E
        else
        {
            if (Input.GetKeyUp(KeyCode.Q))
            {
                prefabRot += rotatonAmount;
                if (prefabRot >= 360)
                {
                    prefabRot = -360 + prefabRot;
                }
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                prefabRot -= rotatonAmount;
                if (prefabRot <= -360)
                {
                    prefabRot = 360 + prefabRot;
                }
            }
        }

        // mouse 2 is middle button
        if (Input.GetMouseButtonDown(2))
        {
            prefabRef += 1;
            if (prefabRef >= stylePrefabBuildings.Count)
            {
                prefabRef = 0;
            }
        }

        if (placeobjectMousePos)
        {
            if (terrain != null)
            {
                //On first frame mouse button is down
                if (Input.GetMouseButtonDown(1))
                {
                    mouseLocation = Input.mousePosition;                                    
                    var ray = Camera.main.ScreenPointToRay(mouseLocation);
                    RaycastHit hit;
                    if (Physics.Raycast(ray.origin, ray.direction,out hit,2000, terrainLayer ,QueryTriggerInteraction.Ignore))
                    {
                        if (hit.collider == terrain.GetComponent<Collider>())
                        {
                            Debug.Log("Hit Terrain");
                            // get Json asset..
                            //var building = (JBR_Building_Resources.BuildingResources)buildingsJsons[prefabRef].Instance;

                            hitPosition = hit.point;
                            newPrefab = Instantiate(stylePrefabBuildings[prefabRef], hitPosition + placementOffset, Quaternion.identity) as GameObject; 
                         //   placementOffset = newPrefab.GetScript<JBR_Building>()._buildingResources.placingOffset;
                            
                            newPrefab.name = (newPrefab.name + "_Placed_" + count);

                            newPrefab.GetComponent<_BuildSystem_Construction>().isPlacing = true;
                            newPrefab.GetComponent<_BuildSystem_Construction>().terrainPlacer = this;

                            rots = newPrefab.GetComponent<_BuildSystem_Construction>().localEulerRotations;
                            newPrefab.transform.localEulerAngles = new Vector3(rots.x, rots.y + prefabRot, rots.z);
                            count++;
                            // newPrefab.BreakPrefabLink();
                            //check if already has a rigidbody if not add one
                            rb = newPrefab.GetComponent<Rigidbody>();
                            if (rb == null)
                            {
                                Debug.Log("Setting Up Trigger System...");
                                rb = newPrefab.AddComponent<Rigidbody>();
                            }
                            rb.useGravity = false;
                            rb.constraints = RigidbodyConstraints.FreezeRotationX;
                            rb.constraints = RigidbodyConstraints.FreezeRotationZ;
                            newPrefab.GetComponent<_BuildSystem_Construction>().rb = rb;

                            //check if already has a boxCollider if not add one
                            boxCollider = newPrefab.GetComponent<BoxCollider>();                        
                            if (boxCollider == null)
                            {
                                Debug.Log("No Box Collider was added ... do better...");
                            }
                            
                            boxCollider.isTrigger = true;
                            newPrefab.layer = placingLayerRef;

                            // get mesh collider
                            mC = newPrefab.GetComponent<MeshCollider>();                         
                            mC.enabled = false;

                            // set all connecetion spheres active
                            if (placedBuildings.Count > 0)
                            {
                                for (int i = 0; i < placedBuildings.Count; i++)
                                {
                                    placedBuildings[i].GetComponent<MeshCollider>().enabled = false;
                                    
                                    placedBuildings[i].GetComponent<BoxCollider>().enabled = true;
                                    if (placedBuildings[i].GetComponent<_BuildSystem_Construction>().spherePoints.Count > 0)
                                    {
                                        for (int s = 0; s < placedBuildings[i].GetComponent<_BuildSystem_Construction>().spherePoints.Count; s++)
                                        {
                                            placedBuildings[i].GetComponent<_BuildSystem_Construction>().spherePoints[s].SetActive(true);
                                        }
                                    }
                                }
                            }

                            for (int i = 0;i < newPrefab.GetComponent<MeshRenderer>().materials.Length; i++)
                            {
                                newPrefab.GetComponent<MeshRenderer>().materials[i].color = colors[refColor];
                            }

                            if (newPrefab.GetComponent<_BuildSystem_Construction>().spherePoints.Count > 0)
                            {
                                refColor = 2;
                            }
                            else
                            {
                                refColor = 1;
                            }
                        }
                    }
                }

                // while mouse button is still down 
                if (Input.GetMouseButton(1) && newPrefab != null)
                {
                    mouseLocation = Input.mousePosition;
                    var ray = Camera.main.ScreenPointToRay(mouseLocation);
                    RaycastHit hit;
                    if (Physics.Raycast(ray.origin, ray.direction, out hit, 2000, terrainLayer, QueryTriggerInteraction.Ignore))
                    {
                        if (hit.collider == terrain.GetComponent<Collider>())
                        {
                         //   Debug.Log("Hit Terrain");
                            // get Json asset..
                            //var building = (JBR_Building_Resources.BuildingResources)buildingsJsons[prefabRef].Instance;

                            hitPosition = hit.point;

                         //   for (int i = 0; i < slots.Length; i++)
                         //   {
                         //       slots[i].Material.SetParameterValue("Color", colors[refColor]);
                         //   }

                            //DebugDraw.DrawWireBox(newPrefab.Box, Color.Aqua, (1 / (float)Engine.FramesPerSecond));
                            
                            if (!isConnected)
                            {
                                newPrefab.transform.position = new Vector3(hitPosition.x + placementOffset.x, hitPosition.y + placementOffset.y, hitPosition.z + placementOffset.z);
                                newPrefab.transform.localEulerAngles = new Vector3(rots.x, rots.y + prefabRot, rots.z);
                                //
                                for (int i = 0; i < newPrefab.GetComponent<_BuildSystem_Construction>().spherePoints.Count; i++)
                                {
                                    newPrefab.GetComponent<_BuildSystem_Construction>().spherePoints[i].GetComponent<_BuildSystem_Connection>().pingPosition(this);
                                }
                            }

                            if (isConnected && !isContacted)
                            {
                                refColor = 1;
                            }

                            if (isConnected && Vector3.Distance(newPrefab.transform.position, hitPosition) > maxConnectionDistance)
                            {
                                isConnected = false;
                                refColor = 2;
                            }
                            if (isContacted)
                            {
                                isConnected = false;
                                refColor = 3;
                            }

                            for (int i = 0; i < newPrefab.GetComponent<MeshRenderer>().materials.Length; i++)
                            {
                                newPrefab.GetComponent<MeshRenderer>().materials[i].color = colors[refColor];
                            }
                        }
                        else
                        {

                        }
                    }
                }

                if (Input.GetMouseButtonUp(1) && newPrefab != null)
                {
                    if (isConnected || placedBuildings.Count <= 0)
                    {
                        refColor = 0;
                        for (int i = 0; i < newPrefab.GetComponent<MeshRenderer>().materials.Length; i++)
                        {
                            newPrefab.GetComponent<MeshRenderer>().materials[i].color = colors[refColor];
                        }
                        newPrefab.GetComponent<_BuildSystem_Construction>().isPlacing = false;
                        placedBuildings.Add(newPrefab.GetComponent<_BuildSystem_Construction>());
                    }
                    else
                    {
                        refColor = 1;
                        Destroy(newPrefab);
                    }

                    //disable connection points
                    if (placedBuildings.Count > 0)
                    {
                        for (int i = 0; i < placedBuildings.Count; i++)
                        {
                            placedBuildings[i].GetComponent<MeshCollider>().enabled = true;

                            placedBuildings[i].GetComponent<BoxCollider>().enabled = false;
                            if (placedBuildings[i].GetComponent<_BuildSystem_Construction>().spherePoints.Count > 0)
                            {
                                for (int s = 0; s < placedBuildings[i].GetComponent<_BuildSystem_Construction>().spherePoints.Count; s++)
                                {
                                    placedBuildings[i].GetComponent<_BuildSystem_Construction>().spherePoints[s].SetActive(false);
                                }
                            }
                        }
                    }

                }

            }
        }


    }

    public void SetConnection(bool canConnect)
    {
    //    Debug.Log("Connected >>>>>>>>>>>>>");
        isConnected = canConnect;
    }

    /// <summary>
    /// Loads and Sets Data for all NPC Units
    /// </summary>
    public bool LoadListData()
    {
        bool loaded = false;
        if (FindTextFile(textAssetPath))
        {
            loaded = LoadTextFile(textAssetPath);
        }
        return loaded;
    }

    /// <summary>
    /// Loads from Text file data
    /// </summary>
    /// <param name="pathJ"></param>
    bool LoadTextFile(string pathJ)
    {
        Text_Data = File.ReadAllText(pathJ);
        Debug.Log("loading Text Data...");
       // JsonSerializer.Deserialize(buildingSaveLocations, Text_Data);
        return true;
    }

    /// <summary>
    /// Returns a bool true/false if a text File is found
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    bool FindTextFile(string path)
    {
        if (System.IO.File.Exists(path))
        {
            Debug.Log("Found Text File !");
            return true;
        }
        Debug.Log("NO Text File Found !!!");
        return false;
    }

    /// <summary>
    /// Returns a list of values between [0;31].
    /// </summary>
    public static List<int> GetAllLayerMaskInspectorIndex(LayerMask _mask)
    {
        List<int> layers = new List<int>();
        var bitmask = _mask.value;
        for (int i = 0; i < 32; i++)
        {
            if (((1 << i) & bitmask) != 0)
            {
                layers.Add(i);
            }
        }
        return layers;
    }

    /// <summary> Converts given bitmask to layer number </summary>
    /// <returns> layer number </returns>
    public static int ToLayer(int bitmask)
    {
        int result = bitmask > 0 ? 0 : 31;
        while (bitmask > 1)
        {
            bitmask = bitmask >> 1;
       //     Debug.Log(result);
            result++;
        }
        return result;
    }

    [System.Serializable]
    public class BuildingSaveLocation
    {
        public int buildingReference;
        public float buildingRotation;
        public Vector3 buildingPosition;
    }
}
