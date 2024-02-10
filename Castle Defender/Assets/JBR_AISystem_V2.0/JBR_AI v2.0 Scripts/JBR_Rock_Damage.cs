using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JBR_Rock_Damage : MonoBehaviour
{

    public float damagebase = 25f;
    public float throwForce = 75.0f;
    public float aimHeight = 4.5f;
    private Rigidbody rB;
    private Vector3 direction;
    public bool throwRock = false;
    [Tooltip("Add explosion particle prefab here")]
    public GameObject hitParticlePrefab;
    public bool destroyOnImpact = false;


    public void Start()
    {
      //  float random1 = Random.Range(1.5f, 10.0f);
      //  float random2 = Random.Range(3.5f, 5.0f);

      //  InvokeRepeating("ThrowARock", random1, random2);
    }

    public void Thrown(Transform player)
    {
        rB = this.gameObject.GetComponent<Rigidbody>();
        if(rB == null)
        {
            Debug.LogWarning(this.gameObject.name + " Has no rigidbody so it cant be thrown");
            return;
        }
        direction = ((player.transform.position + new Vector3(0,aimHeight,0)) - this.transform.position);

        rB.AddForce(direction * throwForce);
    }
   


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == ("Player"))
        {
            //damage code
            Debug.Log("Rack Hit Player");
            collision.gameObject.SendMessage("DamageHealth", damagebase, SendMessageOptions.DontRequireReceiver);
            if (hitParticlePrefab != null)
            {
                Instantiate(hitParticlePrefab, collision.GetContact(0).point, Quaternion.identity);
            }
            if (destroyOnImpact)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            if(collision.gameObject.tag == ("Enemy"))
            {
                //damage code Friendly canFire
                Debug.Log("Rock Hit Enemy");
                if (hitParticlePrefab != null)
                {
                    Instantiate(hitParticlePrefab, collision.GetContact(0).point, Quaternion.identity);
                }
                if (destroyOnImpact)
                {
                    Destroy(this.gameObject);
                }
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
    }


    public void Update()
    {
        if (throwRock)
        {
            throwRock = false;
            Thrown(GameObject.FindGameObjectWithTag("Player").transform);
        }       
    }

    public void ThrowARock()
    {
        throwRock = true;
    }
}
