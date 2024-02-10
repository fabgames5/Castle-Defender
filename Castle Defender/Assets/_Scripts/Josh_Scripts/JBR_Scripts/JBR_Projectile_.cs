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

        public bool updateProjectile = false;

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
            velocity = rB.velocity;
            this.transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
            rot = this.transform.rotation;
            pos = this.transform.position;
         //   Debug.Log(rot + "Arrow Rotation..." + Time.timeSinceLevelLoad);
        }

        private void OnCollisionEnter(Collision collision)
        {          
            updateProjectile = false;
            CancelInvoke();           
            this.rB.isKinematic = true;
            col.enabled = false;
         
            this.transform.position = collision.contacts[0].point;        
            this.transform.SetParent(collision.transform,true);
            this.transform.rotation = rot;
            Debug.Log(collision.gameObject.name + "<<< Arrow hit");
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
