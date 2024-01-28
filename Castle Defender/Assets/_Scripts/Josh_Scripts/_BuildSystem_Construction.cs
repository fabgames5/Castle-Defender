using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Castle Defender - Josh 2024
[ExecuteInEditMode]
public class _BuildSystem_Construction : MonoBehaviour
{
    [Header("This Script sets up a building for the runtime placement system, Note: model position must be Vector3.Zero ")]
    [Space(5)]
    [Tooltip("Check this in Editor to setup Building specs, with connection points for each place Direction checked")]
    [SerializeField]
    private bool setBuildingData = false;
    [Space(5)]
    [Tooltip("for models that need to have a -90 degree rotation on the X axis, this will correct for bounds location")]
    [SerializeField]
    private bool flip_Y_Z_axis = false;
    [Tooltip(" Add a Connection point prefab here !")]
    [SerializeField]
    private GameObject placementPointPrefab;
    [Tooltip(" Scale of the connection point prefab")]
    [SerializeField]
    private Vector3 placementScale = new Vector3(.2f, .2f, .2f);
    [Space(15)]
    [Header("Placement Directions to use")]
    [Tooltip(" Check this to place a connection point on this side, during setup process")]
    [SerializeField]
    private bool placeFront = false;
    [Tooltip(" z offset of Placement Sphere")]
    /// <summary>
    /// z direction
    /// </summary>
    [SerializeField]
    private Vector3 offsetForward;
    [Space(10)]
    [Tooltip(" Check this to place a connection point on this side, during setup process")]
    [SerializeField]
    private bool placeBack = false;
    [Tooltip("-z offset of Placement Sphere")]
    /// <summary>
    /// -z direction
    /// </summary>
    [SerializeField]
    private Vector3 offsetBackward;
    [Space(10)]
    [Tooltip(" Check this to place a connection point on this side, during setup process")]
    [SerializeField]
    private bool placeLeft = false;
    [Tooltip(" -x offset of Placement Sphere")]
    /// <summary>
    /// -x direction
    /// </summary>
    [SerializeField]
    private Vector3 offsetLeft;
    [Space(10)]
    [Tooltip(" Check this to place a connection point on this side, during setup process")]
    [SerializeField]
    private bool placeRight = false;
    [Tooltip("x offset of Placement Sphere")]
    /// <summary>
    /// x direction
    /// </summary>
    [SerializeField]
    private Vector3 offsetRight;
    [Space(10)]
    [Tooltip("Added Dynamically, The BoxCollider attached to this gameobject, Note: all boxcollider settings are reset when building is setup")]
    public BoxCollider boxCollider;
    [Tooltip("Check to make boxCollider a Trigger, True by default")]
    [SerializeField]
    private bool boxColliderIsTrigger = true;
    [Tooltip("how much to offSet the box collider Size, for collision detection during build mode ")]
    [SerializeField]
    private Vector3 boxColliderSize = new Vector3(-0.1f, -0.1f, -0.1f);
    [Tooltip("how much to offSet the box collider center, for collision detection during build mode ")]
    [SerializeField]
    private Vector3 boxColliderCenter = new Vector3(0, 0, 0);
    [Space(5)]
    [Tooltip(" List of active connection points assigned to this building")]
    public List<GameObject> spherePoints = new List<GameObject>();

    [HideInInspector]
    public Rigidbody rb;
    [Tooltip("Check this to add a rigidbody to this gameobject, during setup process ")]
    [SerializeField]
    private bool addRigidbody = false;
    [HideInInspector]
    public MeshCollider meshCollider;
    [Tooltip("Check this to add a Mesh Collider to this gameobject, during setup process ")]
    [SerializeField]
    private bool addMeshCollider = false;
    private bool showbox = false;
    private Bounds boundingBox;
    private Mesh mesh;

    [HideInInspector]
    public Vector3 localEulerRotations;

    [HideInInspector]
    public bool isPlacing = false;
    public _BuildSystem_TerrainPlacer terrainPlacer;
    public int prefabRefID = -1;
    //  private _BuildSystem_Building building;
    
    // Update is called once per frame
    void Update()
    {
        if (setBuildingData)
        {
            setBuildingData = false;
            mesh = this.gameObject.GetComponent<MeshFilter>().sharedMesh;
            mesh.RecalculateBounds();
            boundingBox = mesh.bounds;

            //get or add mesh collider          
            meshCollider = this.gameObject.GetComponent<MeshCollider>();
            if (meshCollider == null && addMeshCollider)
            {
                meshCollider = this.gameObject.AddComponent<MeshCollider>();
            }
            
            //get or add boxcollider
            boxCollider = this.gameObject.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                DestroyImmediate(boxCollider);
            }

            
            boxCollider = this.gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            
            boxCollider.center = boxColliderCenter + boxCollider.center;
            boxCollider.size = boxColliderSize + boxCollider.size;
            if(boxColliderIsTrigger)
            {
                boxCollider.isTrigger = true;
            }
            else
            {
                boxCollider.isTrigger = false;
            }
            
            //get or add rigidbody          
            rb = this.gameObject.GetComponent<Rigidbody>();
            if(rb == null && addRigidbody)
            {
                rb= this.gameObject.AddComponent<Rigidbody>();
            }

            //remove old placement points from scene
            if (spherePoints.Count > 0)
            {
                for (int i = 0; i < spherePoints.Count; i++)
                {
                    DestroyImmediate(spherePoints[i]);
                }
            }

            //clear list 
            spherePoints.Clear();

            if(placementPointPrefab == null)
            {
                Debug.LogError("NO placementPoint Prefab on " + this.gameObject.name + "for build system construction, Please add one !!!");
                return;
            }

            if (placeFront)
            {
                GameObject newSphere = Instantiate(placementPointPrefab, this.gameObject.transform.position, Quaternion.identity) as GameObject;
                //  newSphere.BreakPrefabLink();
                if (flip_Y_Z_axis)
                {
                    newSphere.transform.position = new Vector3(boundingBox.center.x + offsetForward.x, offsetForward.y + boundingBox.center.y , offsetForward.z + boundingBox.center.y + (boundingBox.size.y / 2));
                }
                else
                {
                    newSphere.transform.position = new Vector3(boundingBox.center.x + offsetForward.x, offsetForward.y + boundingBox.center.y - (boundingBox.size.y / 2), offsetForward.z + boundingBox.center.z + (boundingBox.size.z / 2));
                }
                newSphere.transform.SetParent(gameObject.transform);
                newSphere.transform.localScale = placementScale;
                newSphere.GetComponent<_BuildSystem_Connection>().parentBuilding = this.gameObject;
                spherePoints.Add(newSphere);
            }

            if (placeBack)
            {
                GameObject newSphere = Instantiate(placementPointPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
                if (flip_Y_Z_axis)
                {
                    newSphere.transform.position = new Vector3(boundingBox.center.x + offsetBackward.x, offsetBackward.y + boundingBox.center.y, offsetBackward.z - boundingBox.center.y - (boundingBox.size.y / 2));
                }
                else
                {
                    newSphere.transform.position = new Vector3(boundingBox.center.x + offsetBackward.x, offsetBackward.y + boundingBox.center.y - (boundingBox.size.y / 2), offsetBackward.z + boundingBox.center.z - (boundingBox.size.z / 2));
                }
                newSphere.transform.SetParent(gameObject.transform);
                newSphere.transform.localScale = placementScale;
                newSphere.GetComponent<_BuildSystem_Connection>().parentBuilding = this.gameObject;
                spherePoints.Add(newSphere);
            }

            if (placeRight)
            {
                GameObject newSphere = Instantiate(placementPointPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
                if (flip_Y_Z_axis)
                {
                    newSphere.transform.position = new Vector3(boundingBox.center.x + offsetRight.x + (boundingBox.size.x / 2), offsetRight.y + boundingBox.center.y, offsetRight.z + boundingBox.center.y);
                }
                else
                {
                    newSphere.transform.position = new Vector3(boundingBox.center.x + offsetRight.x + (boundingBox.size.x / 2), offsetRight.y + boundingBox.center.y - (boundingBox.size.y / 2), offsetRight.z + boundingBox.center.z);
                }
                newSphere.transform.SetParent(gameObject.transform);
                newSphere.transform.localScale = placementScale;
                newSphere.GetComponent<_BuildSystem_Connection>().parentBuilding = this.gameObject;
                spherePoints.Add(newSphere);
            }

            if (placeLeft)
            {
                GameObject newSphere = Instantiate(placementPointPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
                if (flip_Y_Z_axis)
                {
                    newSphere.transform.position = new Vector3(boundingBox.center.x + offsetLeft.x - (boundingBox.size.x / 2), offsetLeft.y + boundingBox.center.y, offsetLeft.z + boundingBox.center.y);
                }
                else
                {
                    newSphere.transform.position = new Vector3(boundingBox.center.x + offsetLeft.x - (boundingBox.size.x / 2), offsetLeft.y + boundingBox.center.y - (boundingBox.size.y / 2), offsetLeft.z + boundingBox.center.z);
                }
                newSphere.transform.SetParent(gameObject.transform);
                newSphere.transform.localScale = placementScale;
                newSphere.GetComponent<_BuildSystem_Connection>().parentBuilding = this.gameObject;
                spherePoints.Add(newSphere);
            }

            localEulerRotations = this.transform.localEulerAngles;

          //  if (building == null)
          // {
          //      building = this.gameObject.AddComponent<_BuildSystem_Building>();
          //  }
        }       
    }

    void OnDrawGizmosSelected()
    {
        if (showbox)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(boundingBox.center, boundingBox.size);

            // Debug..DrawWireBox(Actor.Box, Color.Red, 1 / 60, true);
            Debug.Log("Draw Bounding Box...");
        }
    }

    public void OnTriggerEnter(Collider other)
    {
       // Debug.Log("OnTriggerEnter >" + other.gameObject.name + " <");
       if(isPlacing && terrainPlacer != null)
        {
            Debug.Log("OnTriggerEnter >" + other.gameObject.name + " <" + this.gameObject.name + " "+ Time.timeSinceLevelLoad);
            terrainPlacer.OnTriggerEnters(other, this.gameObject);
        } 
    }

    public void OnTriggerExit(Collider other)
    {
        if (isPlacing && terrainPlacer != null)
        {
            Debug.Log("OnTriggerExit >" + other.gameObject.name + " <" + this.gameObject.name + " " + Time.timeSinceLevelLoad);
            terrainPlacer.OnTriggerExits(other, this.gameObject);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (isPlacing && terrainPlacer != null)
        {
            Debug.Log("OnTriggerStay > " + Time.timeSinceLevelLoad);
            terrainPlacer.OnTriggerEnters(other, this.gameObject);
        }
    }
}
