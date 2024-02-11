using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JBR_Health_Part: MonoBehaviour
{
    [Tooltip("Dynamically set, THe master health script")]
    public JBR_Health_Master masterHealth;

    [Tooltip("The percentage of health a hit will take, compared to the damage amount")]
    public float healthPercentage = .50f;


    public void HitDamage(float damage)
    {
        masterHealth.SetHealth(-damage * healthPercentage);   
    }

   
}
