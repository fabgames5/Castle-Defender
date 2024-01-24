using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JBR_Projectile : MonoBehaviour
{
    [Tooltip("Dynamcially set, the aim target")]
    public Transform target;
    [Tooltip("Damage done on impact")]
    public float damagebase = 1;
    [Tooltip("The aim offset vertically, head is usually about 1.5-2 meters above the players feet")]
    public float aimHeight = 1.5f;
    [Tooltip("Physics force  used to throw projectile")]
    public float throwForce = 75.0f;
    [Tooltip("Add explosion particle prefab here")]
    public GameObject hitParticlePrefab;
    [Tooltip("Should this object be destroyed on impact")]
    public bool destroyOnImpact = true;
    [Tooltip("follow gravity will aim and shoot at target but be subject to gravity like a Arrow, " +
       "Straight line will move forward with no gravity, " +
       "follow will move towards target while updating direction to current players position, " +
       "followPredictive will do the same as follow but also try to predict where target will be rather than last position")]
    public ProjectileTypes projectileType;


    private Rigidbody rB;
    private Vector3 direction;

    private void OnEnable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if there is a target then do something
       if(target != null)
        {

        } 
    }

    /// <summary>
    /// Fire Object
    /// </summary>
    /// <param name="targetGO"></param>
    public void Fire(GameObject targetGO)
    {
        target = targetGO.transform;

        rB = this.gameObject.GetComponent<Rigidbody>();
        if (rB == null)
        {
            Debug.LogWarning(this.gameObject.name + " Has no rigidbody so it cant be thrown");
            return;
        }
        direction = ((target.position + new Vector3(0, aimHeight, 0)) - this.transform.position);
        this.transform.LookAt(target);
        rB.AddForce(direction * throwForce);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == ("Player"))
        {
            //damage code
            Debug.Log("Rack Hit Player");
            collision.gameObject.SendMessage("DamageHealth", damagebase, SendMessageOptions.DontRequireReceiver);           
        }

        if (collision.gameObject.tag == ("Enemy"))
        {
            //do nothing
        }
        else
        {
            if (hitParticlePrefab != null)
            {
                Instantiate(hitParticlePrefab, collision.GetContact(0).point, Quaternion.identity);
            }

            if (destroyOnImpact)
            {
                Destroy(this.gameObject);
            }
        }
    }


    public enum ProjectileTypes {physicsGravity, straightLine, follow, followPredictive};
}
