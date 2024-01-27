using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsToggle : MonoBehaviour
{
    private GameObject uiElement;
    public bool toggle;

    private void Start()
    {
        uiElement = this.gameObject;
    }

    public void changeToggle(GameObject ob)
    {
        if (ob.GetComponent<Toggle>().isOn == false)
        {
            uiElement.SetActive(false);
        }
    }
}
