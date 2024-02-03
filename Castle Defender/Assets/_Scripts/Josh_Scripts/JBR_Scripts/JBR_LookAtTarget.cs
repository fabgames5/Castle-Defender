using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using StarterAssets;

public class JBR_LookAtTarget : MonoBehaviour
{
    public float lookDistance = 20;
    public Transform cameraTarget;
   // public Vector3 lookForward;
  //  public Vector3 playerPosition;
   // public GameObject playerHead;
    public ThirdPersonController controller;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        cameraTarget = controller.CinemachineCameraTargetCurrent.transform;

        this.gameObject.transform.position = cameraTarget.position + (cameraTarget.transform.forward * lookDistance);
        this.gameObject.transform.rotation = cameraTarget.transform.rotation;
        
    }
}
