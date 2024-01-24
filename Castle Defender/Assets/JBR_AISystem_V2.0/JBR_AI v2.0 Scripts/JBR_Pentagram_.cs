using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JBR_Pentagram_ : MonoBehaviour
{
    public Transform parentOB;
    private Vector3 offset;
    public float rotationBaseSpeed = 25;
    public float finalSpinSpeed;

    public float health;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
       
        offset = this.transform.localPosition;
        parentOB = this.transform.parent;
        this.transform.parent = null;
        
    }

    private void OnDisable()
    {
        this.transform.SetParent(parentOB);
    }

    // Update is called once per frame
    void Update()
    {
        

        health = parentOB.GetComponent<JBR_AI_ControllerSystem>().currentHealth;
        finalSpinSpeed  = ((parentOB.GetComponent<JBR_AI_ControllerSystem>().maxHealth / health) * rotationBaseSpeed * Time.deltaTime);

        if(health <= 0)
        {
            this.gameObject.SetActive(false);
        }

        this.transform.position = parentOB.position + offset;
        transform.Rotate(0, 0, finalSpinSpeed );

    }
}
