using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JBR {
    public class JBR_Projectile : MonoBehaviour
    {
        private Rigidbody rB;

        public Quaternion rot;

        // Start is called before the first frame update
        void Start()
        {
            rB = this.gameObject.GetComponent<Rigidbody>();
        }

        private void OnDisable()
        {
            CancelInvoke();
        }

        public void Fire()
        {
            InvokeRepeating("ProjectileUpdate", .5f, .5f);
        }

        private void ProjectileUpdate()
        {
            Vector3 velocity = rB.velocity;
            this.transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
        }

        private void OnCollisionEnter(Collision collision)
        {
            rot = this.transform.rotation;
            this.rB.isKinematic = true;

            this.transform.parent = collision.transform;
            this.transform.position = collision.contacts[0].point;
            this.transform.rotation = rot;


        }
    }
}
