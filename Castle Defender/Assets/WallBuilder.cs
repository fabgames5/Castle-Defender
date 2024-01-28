using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;


public class WallBuilder : MonoBehaviour
{
    // Prefab of the wall segment to be instantiated
    public GameObject wallSegmentPrefab;

    // Wall height
    public float wallHeight = 2f;

    // Whether wall should follow terrain normals
    public bool followTerrainNormals = false;
    //public GameObject navMeshGameObject;



    // Starting point for wall during click-drag
    private Vector3 startPoint;

    // List of instantiated wall segments
    private List<GameObject> wallSegments = new List<GameObject>();
    private void Start()
    {
        //NavMeshSurface navMeshSurface = navMeshGameObject.GetComponent<NavMeshSurface>();

    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Start building wall on click
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Found ground intersection point
                startPoint = hit.point;
                startPoint = new Vector3(
                    Mathf.RoundToInt(startPoint.x / 3.6f) * 3.6f,
                    startPoint.y,  // Assuming no rounding needed for Y-axis
                    Mathf.RoundToInt(startPoint.z / 3.6f) * 3.6f
                );

            }


            
            BuildWallSegment(startPoint);
        }
        else if (Input.GetMouseButton(0) && startPoint != Vector3.zero)
        {
            // Update wall while dragging
            Vector3 endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //ExtendWall(startPoint, endPoint);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Stop building wall on release
            startPoint = Vector3.zero;
            //ClearWallSegments(); // Optionally, only clear if wall isn't valid
        }
    }

    private void BuildWallSegment(Vector3 point)
    {
        // Create a new wall segment at the point
        GameObject segment = Instantiate(wallSegmentPrefab, point, Quaternion.identity);
        //NavMeshSurface.UpdateNavMeshAsync(new Bounds(point, segment.GetComponent<Collider>().bounds.size));
        segment.transform.rotation = new Quaternion(-Mathf.Sqrt(0.5f),0,0,Mathf.Sqrt(0.5f));
        wallSegments.Add(segment);
    }

    private void ExtendWall(Vector3 start, Vector3 end)
    {
        // Calculate direction and distance of drag
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        // Create new segments based on distance and segment size
        float segmentSize = wallSegmentPrefab.transform.localScale.x;
        int numSegments = Mathf.CeilToInt(distance / segmentSize);
        for (int i = 1; i < numSegments; i++)
        {
            Vector3 segmentPoint = start + direction * (i / (float)numSegments);
            BuildWallSegment(segmentPoint);
        }

        // Update last segment position and scale
        if (wallSegments.Count > 0)
        {
            GameObject lastSegment = wallSegments[wallSegments.Count - 1];
            lastSegment.transform.position = end;
            lastSegment.transform.localScale = new Vector3(distance, wallHeight, 1f);
        }
    }

    private void ClearWallSegments()
    {
        // Destroy all previously instantiated wall segments
        foreach (GameObject segment in wallSegments)
        {
            Destroy(segment);
        }
        wallSegments.Clear();
    }
}
