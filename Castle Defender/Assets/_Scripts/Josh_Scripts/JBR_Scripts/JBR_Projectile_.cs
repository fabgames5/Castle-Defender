using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace JBR {
    public class JBR_Projectile_ : MonoBehaviour
    {
        private Rigidbody rB;
        private Collider col;

        [SerializeField]
        private Vector3 velocity;
        [SerializeField]
        private Quaternion rot;
        private Vector3 pos;
        [Tooltip("Set True if this projectile should check and update its location info, note this allows arrow to update its direction, and allow for correct hit sticking")]
        public bool updateProjectile = false;
        [Tooltip("the damage this projectile will have on a hit object")]
        public float damageAmount = 10;

        // Start is called before the first frame update
        void Start()
        {
            rB = this.gameObject.GetComponent<Rigidbody>();
            col = this.gameObject.GetComponent<Collider>();
        }

        private void FixedUpdate()
        {
            if (updateProjectile)
            {
                velocity = rB.velocity;
                this.transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
                rot = this.transform.rotation;
                pos = this.transform.position;
             //   Debug.Log(rot + "Arrow Rotation..." + Time.timeSinceLevelLoad);
            }
        }



        private void OnDisable()
        {
            CancelInvoke();
        }

        public void Fire()
        {         
          //  InvokeRepeating(nameof(ProjectileUpdate), 0.25f, 0.05f);
            Debug.Log("Invoke Started...");
            updateProjectile = true;
        }

        public void ProjectileUpdate()
        {
          //  velocity = rB.velocity;
          //  this.transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
          //  rot = this.transform.rotation;
          //  pos = this.transform.position;
         //   Debug.Log(rot + "Arrow Rotation..." + Time.timeSinceLevelLoad);
        }

        private void OnCollisionEnter(Collision collision)
        {          
            updateProjectile = false;
            CancelInvoke();           
           // rB.isKinematic = true;
            Destroy(rB);

            col.enabled = false;
         
            this.transform.position = collision.contacts[0].point;        
            this.transform.SetParent(collision.transform,true);
            this.transform.rotation = rot;
            Debug.Log(collision.gameObject.name + "<<< Arrow hit");

            //add damage if available
            if(collision.gameObject.GetComponent<JBR_Health_Part>() != null )
            {
                collision.gameObject.GetComponent<JBR_Health_Part>().HitDamage(damageAmount);
            }

        }

        private void OnTriggerEnter(Collider other)
        {
          //  pos = this.transform.position;
          //  CancelInvoke();

          //  rot = this.transform.rotation;
            this.rB.isKinematic = true;
            col.enabled = false;
            this.transform.parent = other.transform;
            this.transform.position = pos;
            this.transform.rotation = rot;
        }
    }
}
