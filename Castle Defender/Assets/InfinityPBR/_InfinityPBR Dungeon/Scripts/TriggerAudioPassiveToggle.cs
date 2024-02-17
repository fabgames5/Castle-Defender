using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InfinityPBR
{
    /// <summary>
    /// This is meant for an "on/off" audio source -- basically turning a looped audio source on/off
    /// </summary>
    public class TriggerAudioPassiveToggle : MonoBehaviour, IInteractable
    {
        public AudioSource audioSource;
        public GameObject triggerObject;

        public float toggleOnDelay = 0f;
        public float toggleOffDelay = 0f;

        public float volumeFadeSpeed = 0.3f;
        public float maxVolume = 1f;
        private float desiredVolume = 0f;

        public bool isOn = false;
        public bool canInteract = false;
        public bool unlocked = true;
        public bool canBeLocked = false;

        public void Awake()
        {
            if (isOn)
                desiredVolume = 1f;
        }

        public void Interact()
        {
            if (isOn)
            {
                if (toggleOffDelay > 0)
                    StartCoroutine(ToggleOffAfterDelay(toggleOffDelay));
                else
                    ToggleOff();
            }
            else
            {
                if (toggleOnDelay > 0)
                    StartCoroutine(ToggleOnAfterDelay(toggleOnDelay));
                else
                    ToggleOn();
            }

            TriggerOther();
            
            isOn = !isOn;
        }

        public void TryInteract()
        {
            if (canInteract)
                Interact();
        }
        
        public void TryToggleLock()
        {
            if (canBeLocked)
            {
                unlocked = !unlocked;
            }
        }

        public void TryTrigger()
        {
            TryInteract();
        }

        IEnumerator FadeVolume(float targetVolume)
        {
            if (audioSource)
            {
                if (targetVolume > 0)
                    audioSource.Play();
                
                while (audioSource.volume != targetVolume)
                {
                    audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume,
                        Time.deltaTime / volumeFadeSpeed);
                    yield return null;
                }

                if (targetVolume == 0)
                    audioSource.Stop();
            }
            else
            {
                Debug.LogWarning("Warning (" + gameObject.name + "): No Audio Source component is attached but we are trying to modify it!");
            }
        }
        
        IEnumerator ToggleOnAfterDelay(float delay)
        {
            audioSource.Stop();
            yield return new WaitForSeconds(delay);
            audioSource.Play();
            ToggleOn();
        }
        
        IEnumerator ToggleOffAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            ToggleOff();
        }

        private void SetVolume(float volume)
        {
            desiredVolume = volume;
            StartCoroutine(FadeVolume(desiredVolume));
        }

        private void ToggleOff()
        {
            if (audioSource)
            {
                SetVolume(0f);
            }
            else
            {
                Debug.LogWarning("Warning (" + gameObject.name + "): No Audio Source component is attached but we are trying to modify it!");
            }
        }

        private void ToggleOn()
        {
            if (audioSource)
            {
                SetVolume(1f);
            }
            else
            {
                Debug.LogWarning("Warning (" + gameObject.name + "): No Audio Source component is attached but we are trying to modify it!");
            }
        }

        private void TriggerOther()
        {
            if (triggerObject)
            {
                triggerObject.GetComponent<IInteractable>().TryInteract();
            }
        }
        
    }
}

