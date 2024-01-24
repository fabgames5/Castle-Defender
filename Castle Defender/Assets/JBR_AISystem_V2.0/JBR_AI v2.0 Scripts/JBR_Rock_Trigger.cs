using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JBR_Rock_Trigger : MonoBehaviour
{
    public float rockThrowTime = 5.0f;
    private float timer;

    public void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (timer <= 0)
        {
            Debug.Log(" throwable trigger area hit " + other.gameObject.name);
            if (other.gameObject.CompareTag("Throwable"))
            {
                other.gameObject.SendMessage("ThrowARock", SendMessageOptions.DontRequireReceiver);
                timer = rockThrowTime;
            }
        }
    }
}
