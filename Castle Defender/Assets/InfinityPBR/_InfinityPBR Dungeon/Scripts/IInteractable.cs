using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InfinityPBR
{
    public interface IInteractable
    {
        void Interact();
        void TryInteract();
        void TryToggleLock();
        void TryTrigger();
    }
}

