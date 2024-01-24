using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _BuildSystem_Connection : MonoBehaviour
{
    [Tooltip("checked Layers are ignored, leave Connection layer unchecked for castle, road layer unchecked for buildings")]
    public LayerMask connectionLayer;
    public Collider _collider;
    public GameObject parentBuilding;

    public bool isConnected = false;

    private Vector3 goPos;
    private Vector3 offset;


    public List<Collider> cols = new List<Collider>();

    public void pingPosition(_BuildSystem_TerrainPlacer tP)
    {
        cols.Clear();
        Collider[] hit;

        hit = Physics.OverlapSphere(gameObject.transform.position, 0.5f, layerMask: ~(connectionLayer));
        if (hit.Length > 0)
        {

            for (int i = 0; i < hit.Length; i++)
            {
              //  Debug.Log(hit[i].gameObject.name + ".......");

                cols.Add(hit[i]);
                for (int j = 0; j < parentBuilding.GetComponent<_BuildSystem_Construction>().spherePoints.Count; j++)
                {
                    if (hit[i] == parentBuilding.GetComponent<_BuildSystem_Construction>().spherePoints[j].GetComponent<Collider>())
                    {
                        cols.Remove(hit[i]);
                        //                Debug.Log("Remove Collision....");
                        // continue;
                    }
                }
            }

            if (cols.Count > 0)
            {
                for (int i = 0; i < cols.Count; i++)
                {
                    //      Debug.Log(cols[i].Name + " -OverlapBox");
                    SnapPosition(cols[i]);
                    tP.SetConnection(true);
                    break;
                }
            }
        }

    }

    //offset and place building
    public void SnapPosition( Collider col)
    {
        Vector3 newPos = col.transform.position;
        _collider = col;
        goPos = parentBuilding.transform.position;
        offset = this.gameObject.transform.position - newPos;
        parentBuilding.transform.position = goPos - offset;
        
    }
}
