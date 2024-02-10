
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JBR_Trajectory : MonoBehaviour
{
    [Tooltip("ShootAtTarget is for AI, SHootByDirection is for player aiming")]
    public ShootType shootType = ShootType.ShootAtTarget;

    public bool checkDirection = false;
    public bool fire = false;

  
    [Tooltip("Select all hit layers for collsion calculation")]
    public LayerMask hitlayers;

    [Range(0, 90)]
    public float shootAngle;
    public float shootMaxangle = 75;
    public float shootMaxRange = 200;
    public float heightOffset = 0.0f;
    public float fireRate = 3.0f;

    [Tooltip("Required")]
    public GameObject shootPoint;
    public GameObject pivotPoint;
    public GameObject actor;
    public GameObject projectile;
    public LineRenderer lineRenderer;

    private float spdVec;
    private Vector3 shootVec;

    [Tooltip("the arch points of a trajectory")]
    [SerializeField]
    private List<GameObject> archObj;
    public int archLineCount;//Count of of points. Starting from hit position.
    public float archCalcInterval;//Resolution of trajectory arch
    public float archCountLimit = 20;
    public List<Vector3> archVerts = new List<Vector3>();
    [Tooltip("if checked, Shows the projectile path")]
    public bool showProjectilePath = false;
    public float shootForce = 20;
    public float gravityOffset = .2f;
    public float shootAngleOffset = 2.0f;
         
    [Tooltip("Required when showTrajectory is true, just a point on the projectile arch")]
    public GameObject markPref;//Used to visualize arch vertices

    public GameObject hitMarker;
    public GameObject currentCamera;

    public Transform target;
    public Vector3 hitPosition;
    public Vector3 forwardDirection;

    [Tooltip("Distance to the Target for the AI")]
    public float distance;

    private float timer;
    private float fireTimer;

    private StarterAssets.StarterAssetsInputs _Inputs;
    public bool fireReleased;
    public bool firePressed;

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
        
        GameObject obj = Instantiate(shootObj, shootPoint.transform.position, pivotPoint.transform.rotation) as GameObject;
        Rigidbody rig = obj.GetComponent<Rigidbody>();
        Vector3 newDir = new Vector3(this.transform.forward.x,dir.y,this.transform.forward.z);

        Vector3 forceDir = newDir * rig.mass * force;
        rig.AddForce(forceDir, ForceMode.Impulse);

        obj.GetComponent<JBR.JBR_Projectile_>().Fire();
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

        obj.GetComponent<JBR.JBR_Projectile_>().Fire();

    }



    // Start is called before the first frame update
    void Start()
    {
        _Inputs = this.gameObject.GetComponent<StarterAssets.StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        if(fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
        }
        if(fireTimer<=0)
        {
            fire = true;
        }

        if (shootType == ShootType.ShootAtTarget)
        {
            //get distance
            distance = Vector3.Distance(actor.transform.position, target.transform.position);
            //percentage of max distance away
            float percentage = distance / shootMaxRange;
            if (percentage > 1)
            {
                percentage = 1;
            }
            shootAngle = percentage * shootMaxangle;

            //testing code only
            pivotPoint.transform.localEulerAngles = new Vector3(-shootAngle, pivotPoint.transform.localEulerAngles.y, pivotPoint.transform.localEulerAngles.z);
            // looks at target
            Vector3 targetPostition = new Vector3(target.position.x, actor.transform.position.y, target.position.z);
            actor.transform.LookAt(targetPostition);

            if (fire)
            {
                fire = false;
                fireTimer = fireRate;
                ShootAtObject(projectile, new Vector3(target.transform.position.x, target.transform.position.y + heightOffset, target.transform.position.z));
            }
        }

        if(shootType ==ShootType.ShootByDirection)
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
                if(timer <= 0)
                {
                    checkDirection = true;
                }
            }

            //inputs
            if(_Inputs.fire == true)
            {
                firePressed = true;
                fireReleased = false;
            }

            if(_Inputs.fire == false && firePressed == true)
            {
                firePressed = false;
                fireReleased = true;
            }


            if(checkDirection)
            {
            //    pivotPoint.transform.localEulerAngles = new Vector3(-shootAngle, pivotPoint.transform.localEulerAngles.y, pivotPoint.transform.localEulerAngles.z);
            //    Debug.Log(360 - pivotPoint.transform.localEulerAngles.x);
                currentCamera = this.gameObject.GetComponent<ThirdPersonController>().CinemachineCameraTargetCurrent;
                forwardDirection = this.transform.forward;
               // shootPoint.transform.forward = new Vector3(forwardDirection.x, shootPoint.transform.forward.y, forwardDirection.z);
                shootAngle = 360 - pivotPoint.transform.localEulerAngles.x;
               
                    DisplayTrajectoryByDirection(pivotPoint.transform.forward, shootForce);
                
                timer = .1f;
                checkDirection = false;
            }

            if (fireReleased == true && fire == true)
            {
                fire = false;
                fireReleased = false;
                fireTimer = fireRate;
                ShootObjectByDirection(projectile, shootPoint.transform.forward, shootForce);
            }

        }
    }

    public void DisplayTrajectoryByDirection(Vector3 dir, float force)
    {
        //remove old objects
        if (archObj.Count > 0)
        {
            foreach (GameObject obj in archObj)
            {
                Destroy(obj, 0f);
            }
            archObj.Clear();
        }
        archVerts.Clear();
        lineRenderer.positionCount = 0;
        hitMarker.SetActive(false);

        if (showProjectilePath)
        {

            Vector3 dirNor = dir.normalized;

            float x;
            float y = shootPoint.transform.position.y;
            float y0 = y;
            float g = Physics.gravity.y + gravityOffset;
            float rad = (shootAngle + shootAngleOffset )* Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            float time;
            //   Debug.Log("Cos " + cos + " Sin " + sin);

            Vector3 shootPos3 = shootPoint.transform.position;
            float spd = force;

            Quaternion yawRot = Quaternion.FromToRotation(this.transform.forward, this.transform.forward);
            RaycastHit hit;

            for (int i = 0; i < archCountLimit; i++)
            {
                //   Debug.Log("ArchHeightLimit " + i);
                time = archCalcInterval * i;
                x = spd * cos * time;
                y = spd * sin * time + y0 + g * time * time / 2;
                archVerts.Add(new Vector3(x, y, x));
                archVerts[i] = new Vector3(forwardDirection.x * x, archVerts[i].y, forwardDirection.z * x);
                archVerts[i] = new Vector3(archVerts[i].x + shootPos3.x, archVerts[i].y, archVerts[i].z + shootPos3.z);

                if (i > 0)
                {
                    if (Physics.Linecast(archVerts[i - 1], archVerts[i], out hit, hitlayers))
                    {
                        archVerts[i] = hit.point;
                        //      Debug.Log("Hit " + hit.collider.gameObject.name);
                        if (showProjectilePath)
                        {
                            GameObject mark = Instantiate(markPref, hit.point, Quaternion.identity) as GameObject;
                            archObj.Add(mark);
                        }
                        hitMarker.SetActive(true);
                        hitMarker.transform.position = hit.point + (hit.normal * .05f);
                        hitMarker.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                        //  hitMarker.transform.LookAt(currentCamera.transform);
                        //   archObj[i].transform.parent = shootPoint.transform;

                        break;
                    }
                }
                if (showProjectilePath)
                {
                    GameObject mark = Instantiate(markPref, archVerts[i], Quaternion.identity) as GameObject;
                    archObj.Add(mark);
                    //   archObj[i].transform.parent = shootPoint.transform;
                }
            }
            //  int lineLength = archLineCount;
            // archVerts.Reverse();
            //   if (archVerts.Count < lineLength)
            //   {
            //       lineLength = archVerts.Count;
            //   }



            if (lineRenderer != null)
            {
                if (showProjectilePath && archVerts.Count >= 2)
                {
                    Vector3[] positions = new Vector3[archVerts.Count];
                    for (int i = 0; i < archVerts.Count; i++)
                    {
                        positions[i] = archVerts[i];
                    }

                    lineRenderer.enabled = true;
                    lineRenderer.positionCount = positions.Length;
                    lineRenderer.SetPositions(positions);

                }
                else
                {
                    lineRenderer.enabled = false;
                }
            }
        }
    }

    public enum ShootType
    {
        
        ShootAtTarget,
        ShootByDirection
    }
}
