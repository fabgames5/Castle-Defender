using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InfinityPBR
{
    public class AudioPlayerTrigger : MonoBehaviour
    {
        public AudioSource audioSource;
        public GameObject player;

        public void Awake()
        {
            audioSource = transform.parent.gameObject.GetComponent<AudioSource>();
            if (!audioSource)
                Debug.LogWarning("Warning: No AudioSource found on parent!");

            player = GameObject.FindWithTag("Player");
        }
        
        public void OnTriggerEnter(Collider other)
        {
            if (audioSource)
            {
                if (other.gameObject == player)
                {
                    audioSource.enabled = true;
                }
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (audioSource)
            {
                if (other.gameObject == player)
                {
                    audioSource.enabled = false;
                }
            }
        }
    }

}

