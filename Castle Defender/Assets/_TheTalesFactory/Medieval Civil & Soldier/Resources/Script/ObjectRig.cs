using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRig : MonoBehaviour
{

    public GameObject childObj;
    public GameObject parentObj;

    // Start is called before the first frame update
    void Start()
    {
        childObj.transform.parent = parentObj.transform;
    }

}
