using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class JBR_Weapon_Hands : JBR_Weapon_Base
{
    [Tooltip("Dynamically set to true if this uses triggers")]
    public bool useTrigger = true;
    public SphereCollider col;
    [Tooltip("How big the sphere collider should be..")]
    public float colliderSize = .1f;
    [Tooltip("the cool down only allows damage to be sent once per time allowed")]
    public float cooldown = .5f;
    [Tooltip("Add a projectile prefab here (GUN ,Throw , Spell)")]
    public GameObject projectilePrefab;
    [Tooltip("a location transforn the projectile will fore from, its position and rotation are used for a gun, if nothing is set then this object is used ")]
    public Transform fireTransform;

    //internal
    private float timer = 0;

    public override void Initialize(JBR_AI_ControllerSystem mainSystem, JBR_Attack_Behavior ai_Attack, Animator ai_Animator, AudioSource ai_AudioSource)
    {
        Debug.Log("Weapon Hands Initalize Colliders...");
        SetupColliders();
        
        base.Initialize(mainSystem, ai_Attack, ai_Animator, ai_AudioSource);
       
    }

    private void OnCollisionEnter(Collision collision)
    {

    }

    private void OnCollisionExit(Collision collision)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(type == WeaponTypes.handsTrigger)
        if (timer == 0)
        {
            for (int i = 0; i < m_AI_Controller.targetTags.Length; i++)
            {
                if (other.CompareTag(m_AI_Controller.targetTags[i]))
                {
                    other.SendMessage(DamageMessage, damage);
                    timer = cooldown;
                    //stop once target is found
                    return;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    /// <summary>
    /// Sets up colliders
    /// </summary>
    private void SetupColliders()
    {
        if (type == WeaponTypes.handsTrigger || type == WeaponTypes.handCollider)
        {


            if (col == null)
            {
                //make sure we have a collider and if not add one and set size
                if (this.gameObject.GetComponent<SphereCollider>() != null)
                {
                    col = this.gameObject.GetComponent<SphereCollider>();
                }
                else
                {
                    if (this.gameObject.GetComponent<SphereCollider>() == null)
                    {
                        col = this.gameObject.AddComponent<SphereCollider>();
                    }
                }
            }
            col.radius = colliderSize;          
            col.enabled = false;
            if (type == WeaponTypes.handsTrigger)
            {
                col.isTrigger = true;
                useTrigger = true;
            }
        }
    }


    /// <summary>
    /// On Enter Behavior
    /// </summary>
    public override void OnEnterAbility()
    {
        if (type == WeaponTypes.handsTrigger || type == WeaponTypes.handCollider)
        {
            col.enabled = true;
        }

        if(type == WeaponTypes.gun || type == WeaponTypes.throwing || type == WeaponTypes.spell)
        {
            //set fire location if nothing was set
            if(fireTransform == null)
            {
                fireTransform = this.transform;
            }
            if (projectilePrefab != null)
            {
                GameObject go = Instantiate(projectilePrefab, fireTransform.position, Quaternion.LookRotation(fireTransform.forward)) as GameObject;
                // Projectile will handle movement and damage, we just initalize it and give it a target
                go.SendMessage("Fire", m_AI_Controller.currentTarget, SendMessageOptions.DontRequireReceiver);
            }
        }

        base.OnEnterAbility();
    }

    /// <summary>
    /// Exit Ability 
    /// </summary>
    public override void OnExitAbility()
    {
        col.enabled = false;
        Debug.Log("*****Exit Weapon Hands " );
        base.OnExitAbility();
    }

    /// <summary>
    /// Main Update for each ability. It's updated in the controller only updates while Attack is active
    /// </summary>
    public override void UpdateState()
    {
        base.UpdateState();
        // add all code that need update here....

        //timer for cooldown
        if(timer > 0 || timer < 0)
        {
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                timer = 0;
            }
        }
    }
}
