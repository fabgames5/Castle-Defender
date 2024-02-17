using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InfinityPBR
{
    /// <summary>
    /// This is meant for an "on/off" toggle/lever that the player can interact with.
    /// </summary>
    public class TriggerToggle : MonoBehaviour, IInteractable
    {
        public Animation animation;
        public AudioSource audioSource;
        public LayerMask interactionLayers;
        
        public AnimationClip openAnimation;
        public AnimationClip closeAnimation;

        public AudioClip[] openAudioClips;
        public AudioClip[] closeAudioClips;
        public AudioClip[] unlockAudioClips;
        public AudioClip[] lockAudioClips;

        public GameObject[] triggerObjects;
        public GameObject[] unlockObjects;

        public bool playAudio = true;
        public bool playAnimation = true;

        public bool isOpen = false;
        public bool canInteract = false;
        public bool canBeLocked = false;
        public bool unlocked = true;
        
        public float lockDelay = 0f;
        public float toggleDelay = 0f;

        private bool canBeInteractedWith => CanBeInteractedWith();
        private bool canBeOpened => CanBeOpened();

        public void Update()
        {
            if (canInteract)
            {
                if (Input.GetMouseButtonDown(0))
                    TryInteract();
            }
        }
        public void Interact()
        {
            PlayAnimation();
            PlayAudio();
            TriggerOther();
            UnlockOther();

            isOpen = !isOpen;
        }

        public void TryTrigger()
        {
            if (canBeOpened)
            {
                if (toggleDelay > 0)
                    StartCoroutine(InteractAfterDelay(toggleDelay));
                else
                    Interact();
            }
        }

        public void TryInteract()
        {
            if (canBeInteractedWith)
            {
                if (toggleDelay > 0)
                    StartCoroutine(InteractAfterDelay(toggleDelay));
                else
                    Interact();
            }
        }

        IEnumerator InteractAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Interact();
        }

        public void TryToggleLock()
        {
            if (canBeLocked)
            {
                if (lockDelay > 0)
                    StartCoroutine(ToggleLockAfterDelay(lockDelay));
                else
                    ToggleLock();
            }
        }
        
        IEnumerator ToggleLockAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            ToggleLock();
        }

        private void ToggleLock()
        {
            AudioClip[] clips = unlocked ? lockAudioClips : unlockAudioClips;
            if (clips.Length > 0)
            {
                audioSource.clip = clips[Random.Range(0, clips.Length)];
                audioSource.Play();
            }
            unlocked = !unlocked;
        }

        public bool CanBeInteractedWith()
        {
            if (canInteract && unlocked)
                return true;
            return false;
        }
        
        public bool CanBeOpened()
        {
            if (unlocked)
                return true;
            return false;
        }

        private void PlayAnimation()
        {
            if (playAnimation)
            {
                if (animation)
                {
                    animation.clip = isOpen ? closeAnimation : openAnimation;
                    animation.Play(); 
                }
                else
                {
                    Debug.LogWarning("Warning (" + gameObject.name + "): No Animation component is attached but we are trying to play animation!");
                }
            }
        }

        private void PlayAudio()
        {
            if (playAudio)
            {
                if (audioSource)
                {
                    AudioClip[] clips = isOpen ? closeAudioClips : openAudioClips;
                    if (clips.Length > 0)
                    {
                        audioSource.clip = clips[Random.Range(0, clips.Length)];
                        audioSource.Play();
                    }
                    
                }
                else
                {
                    Debug.LogWarning("Warning (" + gameObject.name + "): No Audio Source is attached but we are trying to play audio!");
                }
            }
        }

        private void TriggerOther()
        {
            for (int i = 0; i < triggerObjects.Length; i++)
            {
                triggerObjects[i].GetComponent<IInteractable>().TryTrigger();
            }
        }

        private void UnlockOther()
        {
            for (int i = 0; i < unlockObjects.Length; i++)
            {
                unlockObjects[i].GetComponent<IInteractable>().TryToggleLock();
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            int layer = other.gameObject.layer;
            if (interactionLayers == (interactionLayers | (1 << layer)))
                canInteract = true;
        }

        public void OnTriggerExit(Collider other)
        {
            int layer = other.gameObject.layer;
            if (interactionLayers == (interactionLayers | (1 << layer)))
                canInteract = false;
        }
    }
}

