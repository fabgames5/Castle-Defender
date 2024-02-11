using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JBR_Health_Master : MonoBehaviour
{
    [Space(10)]
    [Header("Health")]
    public float cur_Health;

    [Tooltip("the starting base Health before any level or bonus add ons")]
    public float baseHealth = 20;
    [Space(10)]

    [Tooltip("Max Health is (based + Bonus) * multipier")]
    [SerializeField]
    private float _MaxHealth;
    public float max_Health
    {
        get
        {
            return _MaxHealth = ((baseHealth + bonusMaxHealth) * maxHealthMultipier);
        }
    }
    [Tooltip("Max Health bonus Amount")]
    public float bonusMaxHealth;
    [Tooltip("Max Health Multipier >>> Note: Multipies after bonus is added")]
    public float maxHealthMultipier = 1;

    public bool isDead;

    [Tooltip("All body parts that have a health element")]
    public List<JBR_Health_Part> parts = new List<JBR_Health_Part>();


    /// <summary>
    /// Add or Minus health from Current Health
    /// </summary>
    /// <param name="addHealth"></param>
    public void SetHealth(float addHealth)
    {
        cur_Health += addHealth;
        if (cur_Health > max_Health)
        {
            cur_Health = max_Health;
        }
        if (cur_Health <= 0)
        {
            isDead = true;
        }
    }

    // called when an object becomes enabled or active
    private void OnEnable()
    {
        //set current health to max
        float checkHealth = max_Health;
        cur_Health = checkHealth;

        //add all health elements
        JBR_Health_Part[] partsArray;
        partsArray = GetComponentsInChildren<JBR_Health_Part>();

        for (int i = 0; i < partsArray.Length; i++)
        {
            parts.Add(partsArray[i]);
            partsArray[i].masterHealth = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
