using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InfinityPBR
{
    /// <summary>
    /// This is intended to be used passively. This means the player can not directly interact with this, but rather
    /// it is triggered by other interactions, perhaps events in the game, or other interactable triggers.
    /// </summary>
    public class TriggerPassiveToggle : MonoBehaviour, IInteractable
    {
        public Animation animation;
        public AudioSource audioSource;

        public AnimationClip openAnimation;
        public AnimationClip closeAnimation;

        public AudioClip openAudioClip;
        public AudioClip closeAudioClip;

        public GameObject triggerObject;

        public bool playAudio = true;
        public bool playAnimation = true;

        public bool isOpen = false;
        public bool canInteract = false;
        public bool canBeLocked = false;
        public bool unlocked = true;

        public float delay = 0f;

        public void Interact()
        {
            PlayAnimation();
            PlayAudio();
            TriggerOther();

            isOpen = !isOpen;
        }

        public void TryInteract()
        {
            if (canInteract)
            {
                if (delay > 0)
                    StartCoroutine(InteractAfterDelay(delay));
                else
                    Interact();
            }
            else
                Debug.LogWarning("(" + gameObject.name + ") Trying to interact, but this object is set canInteract = false");
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
            throw new System.NotImplementedException();
        }

        IEnumerator InteractAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Interact();
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
                    audioSource.clip = isOpen ? closeAudioClip : openAudioClip;
                    audioSource.Play();
                }
                else
                {
                    Debug.LogWarning("Warning (" + gameObject.name + "): No Audio Source is attached but we are trying to play audio!");
                }
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