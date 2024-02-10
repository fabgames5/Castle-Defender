
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JBR_Trajectory : MonoBehaviour
{
    [Header("Select Aiming Type")]
    [Tooltip("ShootAtTarget is for AI, SHootByDirection is for player aiming")]
    public ShootType shootType = ShootType.ShootAtTarget;
    [SerializeField]
    [Tooltip("Select all layers the Arrow should use for collsion calculation")]
    private LayerMask hitLayers;

    [Space(5)]
    [Header("Universal Settings")]

    [Tooltip("The Shooting force applied to the projectile prefab")]
    public float shootForce = 20;
    [Range(0, 90)]
    [Tooltip("Dynamcally Set, The current Angle the Projectile will shoot from")]
    public float shootAngle;
    [Tooltip("if true Projectile will shoot")]
    public bool canFire = false;
    [Tooltip("THe maximum Rate at which a projectile can be shot at")]
    [SerializeField]
    private float fireRate = 3.0f;
    [Tooltip("Add the Projectile Prefab Here")]
    [SerializeField]
    private GameObject projectile;
    [Tooltip("Add the projectile spawn point here, should be on front of shooting Weapon")]
    public GameObject shootPoint;
    [Tooltip("Add the < Aiming Look At Target > Scene Object here")]
    public GameObject AimingLookAtTarget;

    [Space(5)]
    [Header("Dynamic Settings (Shoot At Target) ")]
   
    [Tooltip("Dynamically Set, Distance to the Target for the AI")]
    [SerializeField]
    private float distance;
    [Tooltip("The Max Range the target can be away")]
    public float shootMaxRange = 200;
    [Tooltip("The max angle the projectile can shoot at")]
    public float shootMaxangle = 75;
    [Tooltip("The height Offset for arrow projectile spawning")]
    public float heightOffset = 0.0f;
    [Tooltip("The Target for the AI to shoot at, Set by other Scripts or drag and drop for testing")]
    [SerializeField]
    private Transform target;

    [Space(5)]
    [Header("Required for (Shoot by Direction) ")] 
    
    [Tooltip("For matching Actual Arrow path a Gravity multiplier to Adjust, for Predictive Path")]
    [SerializeField]
    private float gravityOffset = .2f;
    [Tooltip("For matching Actual Arrow path offset of Fire Angle to adjust, for Predictive Path")]
    [SerializeField]
    private float shootAngleOffset = 2.0f;
    [Tooltip("Required when showTrajectory is true, just a point indicator on the projectile arch")]
    public GameObject trajectoryPointPrefab;//Used to visualize arch vertices
    [Tooltip("Add the hit Indicator Scene Object here,(Note: should be in scene not prefab) for predictive arrow path")]
    public GameObject hitMarkerSceneObject;
    [Tooltip("Resolution for the Trajectory Calculation, small is more accurate, but uses more resources")]
    [SerializeField]
    private float archCalcInterval = 0.05f;//Resolution of trajectory arch
    [Tooltip("This is the limit of how many arch points the Predictive trajectory can have")]
    [SerializeField]
    private float archCountLimit = 80;

    [Space(5)]
    [Header("Dynamic Settings (Shoot By Direction) ")]
      
    [SerializeField]
    [Tooltip("Dynamically Set, a list of points the arrow will follow in its predicted path")]
    private List<Vector3> archVerts = new List<Vector3>();
    // [Tooltip("the arch points of a trajectory")]
    private List<GameObject> archObj = new List<GameObject>();
    [Tooltip("if checked, Shows the projectile path")]
    public bool showProjectilePath = false;
    [Tooltip("Dynamically Set, was the Fire button Released")]
    [SerializeField]
    private bool fireReleased;
    [Tooltip("Dynamically Set, was the Fire button Pressed")]
    [SerializeField]
    private bool firePressed;

    //  public int archLineCount;//Count of of points. Starting from hit position.
    //  public Vector3 hitPosition;

    // [Tooltip("Dynamically Set, The Current active Camera Target for aiming")]
    //  [SerializeField]
    private GameObject currentCameraTarget;
    //  [SerializeField]
    private bool checkDirection = true;
    //dynamcially set, forward direction
    private Vector3 forwardDirection;
   // [Tooltip("Add the point to aim at here")]
    private GameObject actor;
    //timers
    private float timer;
    private float fireTimer;

    //shoot at target calculations
    private float spdVec;
    private Vector3 shootVec;

    private StarterAssets.StarterAssetsInputs _Inputs;
    //  Line Renderer Component
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _Inputs = this.gameObject.GetComponent<StarterAssets.StarterAssetsInputs>();
        lineRenderer = this.gameObject.GetComponent<LineRenderer>();
        actor = this.gameObject;
    }

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
        //Debug.Log(markPos);

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
        
        GameObject obj = Instantiate(shootObj, shootPoint.transform.position, shootPoint.transform.rotation) as GameObject;
        Rigidbody rig = obj.GetComponent<Rigidbody>();
        Vector3 newDir = new Vector3(this.transform.forward.x,dir.y,this.transform.forward.z);

        Vector3 forceDir = newDir * rig.mass * force;
        rig.AddForce(forceDir, ForceMode.Impulse);

        obj.GetComponent<JBR.JBR_Projectile_>().Fire();
    }

    public void ShootAtObject(GameObject shootObj, Vector3 hitPos)
    {
        CheckVector(hitPos);
        //Debug.Log(hitPos);

        if (shootObj == null)
        {
            throw new System.NullReferenceException("shootObj");
        }
        if (shootPoint == null)
        {
            throw new System.NullReferenceException("shootPoint");
        }

        GameObject obj = Instantiate(shootObj, shootPoint.transform.position, AimingLookAtTarget.transform.rotation);
        Rigidbody rig = obj.GetComponent<Rigidbody>();
        Vector3 force = shootVec * rig.mass;
        rig.AddForce(force, ForceMode.Impulse);

        obj.GetComponent<JBR.JBR_Projectile_>().Fire();

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
            canFire = true;
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
            AimingLookAtTarget.transform.localEulerAngles = new Vector3(-shootAngle, AimingLookAtTarget.transform.localEulerAngles.y, AimingLookAtTarget.transform.localEulerAngles.z);
            // looks at target
            Vector3 targetPostition = new Vector3(target.position.x, actor.transform.position.y, target.position.z);
            actor.transform.LookAt(targetPostition);

            if (canFire)
            {
                canFire = false;
                fireTimer = fireRate;
                ShootAtObject(projectile, new Vector3(target.transform.position.x, target.transform.position.y + heightOffset, target.transform.position.z));
            }
        }

        if(shootType ==ShootType.ShootByDirection)
        {
            if(timer >= 0)
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
            //    AimingLookAtTarget.transform.localEulerAngles = new Vector3(-shootAngle, AimingLookAtTarget.transform.localEulerAngles.y, AimingLookAtTarget.transform.localEulerAngles.z);
            //    Debug.Log(360 - AimingLookAtTarget.transform.localEulerAngles.x);
                currentCameraTarget = this.gameObject.GetComponent<ThirdPersonController>().CinemachineCameraTargetCurrent;
                forwardDirection = this.transform.forward;
               // shootPoint.transform.forward = new Vector3(forwardDirection.x, shootPoint.transform.forward.y, forwardDirection.z);
                shootAngle = 360 - AimingLookAtTarget.transform.localEulerAngles.x;
               
                DisplayTrajectoryByDirection(AimingLookAtTarget.transform.forward, shootForce);
                
                timer = .1f;
                checkDirection = false;
            }

            if (fireReleased == true && canFire == true)
            {
                canFire = false;
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
        hitMarkerSceneObject.SetActive(false);

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
                    if (Physics.Linecast(archVerts[i - 1], archVerts[i], out hit, hitLayers))
                    {
                        archVerts[i] = hit.point;
                        //      Debug.Log("Hit " + hit.collider.gameObject.name);
                        if (showProjectilePath)
                        {
                            GameObject mark = Instantiate(trajectoryPointPrefab, hit.point, Quaternion.identity) as GameObject;
                            archObj.Add(mark);
                        }
                        hitMarkerSceneObject.SetActive(true);
                        hitMarkerSceneObject.transform.position = hit.point + (hit.normal * .05f);
                        hitMarkerSceneObject.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                        //  hitMarkerSceneObject.transform.LookAt(currentCameraTarget.transform);
                        //   archObj[i].transform.parent = shootPoint.transform;
                        break;
                    }
                }
                if (showProjectilePath)
                {
                    GameObject mark = Instantiate(trajectoryPointPrefab, archVerts[i], Quaternion.identity) as GameObject;
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
