using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JBR_Lazer_Aim : MonoBehaviour
{
    [Tooltip("Set to true to start")]
    public bool aim = false;
    [Tooltip("The width of the object start size scale")]
    public float startSize = 1.0f;
    [Tooltip("based off of 1 second Time frame, this is a multipier, so .5 would be half the speed, or 2 seconds ")]
    public float timerSpeed = .5f;

    private Vector3 startSizeVect;
    private Vector3 finishSizeVect;
    [Tooltip("Dynamically set, the timer for the size change, 0 to 1 is the range")]
    public float timer = 0;

    [Tooltip("how often the lazor damage applys to player in it")]
    public float lazorDamgeRate = .25f;
    public float lazorDamage = 1.0f;
    private float lazorTimer = 0;

    public GameObject projectilePrefab;
    public Transform firePoint;
    [Tooltip("The precent the aim column has narrowed before the metero starts its drop")]
    public float startMeteorDropPercent = 75;
    private bool meteorDropped = false;
    private float randomtiming;
    public AudioSource aS;
    public AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void OnEnable()
    {
        startSizeVect = new Vector3(startSize, this.transform.localScale.y, startSize);
        finishSizeVect = new Vector3(.0001f, this.transform.localScale.y, .00001f);
        timer = 0;

        if(aS == null)
        {
            aS = this.GetComponent<AudioSource>();
        }
        if (aS == null)
        {
            Debug.LogWarning("No AuidoSource for JBR+ Lazor Aim");
        }
       
        //meteor wont drop if the lazor finishes first so set it at same time if its too long
        if(startMeteorDropPercent > 100 )
        {
            startMeteorDropPercent = 100 ;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (aim)
        {
        timer += Time.deltaTime * timerSpeed;
        Vector3 newSize = Vector3.Lerp(startSizeVect, finishSizeVect, timer);

        this.transform.localScale = newSize;

            if(timer >= startMeteorDropPercent *(.01f + randomtiming)  && meteorDropped == false)
            {
             //   Debug.Log("Drop Meteor");
                meteorDropped = true;
                GameObject meteor = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(firePoint.transform.forward)) as GameObject;
            }

            if(timer >= 1)
            {
                aim = false;
              
            }
        }
        //reset lazor damage timer before lazor can cause damage again
        if(lazorTimer > 0)
        {
            lazorTimer -= Time.deltaTime;
        }
    }

    public void StartAim()
    {
        timer = 0;
        aim = true;
        meteorDropped = false;
        //add a small amount of variance for the timing of the drop
        randomtiming = Random.Range(-.005f, +.005f);

        if ( aS != null && clip != null)
        {
            aS.PlayOneShot(clip);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && lazorTimer <= 0 && aim == true)
        {
            other.gameObject.SendMessage("DamageHealth", lazorDamage, SendMessageOptions.DontRequireReceiver);
            lazorTimer = lazorDamgeRate;
            Debug.Log("Player taken damage -1");
        }
    }
}
