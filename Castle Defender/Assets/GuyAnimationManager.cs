using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuyAnimationManager : MonoBehaviour
{

    public Animator enemyAnimator; // Reference to the enemy's Animator component

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void gothit()
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("hit"); // Play the "Hit" animation
        }
    }

    public void dieanim()
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("died"); // Play the "Hit" animation
        }
    }
}
