
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JBR;

public class JBR_Trajectory : MonoBehaviour
{
    public bool fire = false;
  //  public bool repeatFire = false;
  
    [Tooltip("Select a ignoring layer for collsion calculation")]
    public LayerMask ignoreLayer;

    [Range(0, 90)]
    public float shootAngle;
    public float shootMaxangle = 75;
    public float shootMaxRange = 200;
    public float heightOffset = 0.0f;

    [Tooltip("Required")]
    public GameObject shootPoint;
    public GameObject pivotPoint;
    public GameObject actor;
    public GameObject projectile;

    private float spdVec;
    private Vector3 shootVec;
    public Transform target;
    public Vector3 hitPosition;

    public float distance;

    public void CheckVector(Vector3 hitPos)
    {
       
           spdVec = CalculateVectorFromAngle(hitPos, shootAngle);
           if (spdVec <= 0.0f)
           {
               Debug.Log("impossible hit point");
               return;
           }

           shootVec = ConvertVectorToVector3(spdVec, shootAngle, hitPos);      
    }

    private float CalculateVectorFromAngle(Vector3 pos, float angle)
    {
        Vector2 shootPos = new Vector2(shootPoint.transform.position.x,
            shootPoint.transform.position.z);
        Vector2 hitPos = new Vector2(pos.x, pos.z);
        float x = Vector2.Distance(shootPos, hitPos);
        float g = Physics.gravity.y;
        float y0 = shootPoint.transform.position.y;
        float y = pos.y;
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float tan = Mathf.Tan(rad);

        float v0Sq = g * x * x / (2 * cos * cos * (y - y0 - x * tan));
        if (v0Sq <= 0.0f)
        {
            return 0.0f;
        }
        return Mathf.Sqrt(v0Sq);
    }

    private Vector3 ConvertVectorToVector3(float spdVec, float angle, Vector3 pos)
    {
        Vector3 shootPos = shootPoint.transform.position;
        Vector3 hitPos = pos;
        shootPos.y = 0f;
        hitPos.y = 0f;

        Vector3 dir = (hitPos - shootPos).normalized;
        Quaternion Rot3D = Quaternion.FromToRotation(Vector3.right, dir);
        Vector3 vec = spdVec * Vector3.right;
        vec = Rot3D * Quaternion.AngleAxis(angle, Vector3.forward) * vec;

        return vec;
    }

    public void ShootObject(GameObject shootObj, Vector3 hitPos)
    {
        int layerMask = -1;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(hitPos);
        Vector3 markPos = new Vector3();
        if (Physics.Raycast(ray, out hit, shootMaxRange, layerMask))
        {
            markPos = Camera.main.ScreenToWorldPoint(new Vector3(hitPos.x, hitPos.y, hit.distance));
        }
        CheckVector(markPos);
        Debug.Log(markPos);

        if (shootObj == null)
        {
            throw new System.NullReferenceException("shootObj");
        }
        if (shootPoint == null)
        {
            throw new System.NullReferenceException("shootPoint");
        }

        GameObject obj = Instantiate(shootObj, shootPoint.transform.position, Quaternion.identity);
        Rigidbody rig = obj.GetComponent<Rigidbody>();
        Vector3 force = shootVec * rig.mass;
        rig.AddForce(force, ForceMode.Impulse);

    }

    public void ShootObjectByDirection(GameObject shootObj, Vector3 dir, float force)
    {
        
        GameObject obj = Instantiate(shootObj, shootPoint.transform.position, shootPoint.transform.rotation);
        Rigidbody rig = obj.GetComponent<Rigidbody>();
        Vector3 forceDir = dir * rig.mass * force;
        rig.AddForce(forceDir, ForceMode.Impulse);
    }

    public void ShootAtObject(GameObject shootObj, Vector3 hitPos)
    {
        CheckVector(hitPos);
        Debug.Log(hitPos);

        if (shootObj == null)
        {
            throw new System.NullReferenceException("shootObj");
        }
        if (shootPoint == null)
        {
            throw new System.NullReferenceException("shootPoint");
        }

        GameObject obj = Instantiate(shootObj, shootPoint.transform.position, pivotPoint.transform.rotation);
        Rigidbody rig = obj.GetComponent<Rigidbody>();
        Vector3 force = shootVec * rig.mass;
        rig.AddForce(force, ForceMode.Impulse);

        obj.GetComponent<JBR.JBR_Projectile>().Fire();

    }



    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //get distance
        distance = Vector3.Distance(actor.transform.position, target.transform.position);
        //percentage of max distance away
        float percentage = distance / shootMaxRange;
        if(percentage > 1)
        {
            percentage = 1;
        }
        shootAngle = percentage * shootMaxangle;

        //testing code only
        pivotPoint.transform.localEulerAngles = new Vector3(-shootAngle, pivotPoint.transform.localEulerAngles.y, pivotPoint.transform.localEulerAngles.z );
        // looks at target
        Vector3 targetPostition = new Vector3(target.position.x, actor.transform.position.y, target.position.z);
        actor.transform.LookAt(targetPostition);

        if (fire)
        {
            fire = false;
            ShootAtObject(projectile, new Vector3(target.transform.position.x,target.transform.position.y + heightOffset, target.transform.position.z));
        }
    }
}
