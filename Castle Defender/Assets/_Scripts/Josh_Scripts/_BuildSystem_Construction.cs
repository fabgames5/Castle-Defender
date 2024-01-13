using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class _BuildSystem_Construction : MonoBehaviour
{

    public bool showbox = false;

    public bool getBuildingData = false;

    public Bounds boundingBox ;
    [Tooltip("for models that need to have a -90 degree rotation on the X axis, this will correct for bounds location")]
    public bool flip_Y_Z_axis = false;

    public GameObject placementPointPrefab;
    public Vector3 placementScale = new Vector3(.2f, .2f, .2f);
    [Space(5)]
  //  [Tooltip("offset of Bounding box for irregular shapes")]
 //   public Vector3 boundsOffset;
  //  [Space(5)]

    public bool placeFront = false;
    [Tooltip(" z offset of Placement Sphere")]   
    /// <summary>
    /// z direction
    /// </summary>
    public Vector3 offsetForward;
    [Space(5)]
    public bool placeBack = false;
    [Tooltip("-z offset of Placement Sphere")]
    /// <summary>
    /// -z direction
    /// </summary>
    public Vector3 offsetBackward;
    [Space(5)]
    public bool placeLeft = false;
    [Tooltip(" -x offset of Placement Sphere")]
    /// <summary>
    /// -x direction
    /// </summary>
    public Vector3 offsetLeft;
    [Space(5)]
    public bool placeRight = false;
    [Tooltip("x offset of Placement Sphere")]
    /// <summary>
    /// x direction
    /// </summary>
    public Vector3 offsetRight;
    [Space(5)]
 //   [Tooltip("how much to offSet the box collider bigger or smaller, for collision detection during build mode ")]
 //   public Vector3 boxColliderOffset = new Vector3(-20.0f, -20.0f, -20.0f);

    private Mesh mesh;
    public Rigidbody rb;
    public BoxCollider boxCollider;
    public MeshCollider meshCollider;

    public List<GameObject> spherePoints = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (getBuildingData)
        {
            getBuildingData = false;
            mesh = GetComponent<MeshFilter>().sharedMesh;          
            boundingBox = mesh.bounds;
            meshCollider = gameObject.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = gameObject.AddComponent<MeshCollider>();
            }

            boxCollider = gameObject.GetComponent<BoxCollider>();
            {
                if(boxCollider == null)
                {
                    boxCollider = gameObject.AddComponent<BoxCollider>();
                    boxCollider.isTrigger = true;
                }
            }
            // meshCollider.convex = true;

            //      Debug.Log("Got Bounding Box Data ... " + boundingBox.Size);

            if (spherePoints.Count > 0)
            {
                for (int i = 0; i < spherePoints.Count; i++)
                {
                    DestroyImmediate(spherePoints[i]);
                }
            }
            spherePoints.Clear();

            if (placeFront)
            {
                GameObject newSphere = Instantiate(placementPointPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
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

            //this.Actor.AddScript<_BuildSystem_Building>();
        }

        void OnDrawGizmos()
        {
            if (showbox)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(boundingBox.center, boundingBox.size);

                // Debug..DrawWireBox(Actor.Box, Color.Red, 1 / 60, true);
                // Debug.Log("Draw Bounding Box...");
            }
        }
    }
}
