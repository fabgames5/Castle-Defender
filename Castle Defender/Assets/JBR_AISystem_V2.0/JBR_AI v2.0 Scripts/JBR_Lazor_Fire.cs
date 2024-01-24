using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JBR_Lazor_Fire : MonoBehaviour
{
    public GameObject aimPrefab;
    [Range(1,50)]
    public float distanceRange = 10;
    public int total = 25;
    private List<GameObject> aimLazors = new List<GameObject>();
    public Transform caster;

    public void Start()
    {
        for (int i = 0; i < total; i++)
        {
            GameObject aimLazor = Instantiate(aimPrefab, this.transform.position, Quaternion.identity) as GameObject;
            aimLazors.Add(aimLazor);
            aimLazor.SetActive(false);
        }
    }


    public void Fire()
    {
        for (int i = 0; i < aimLazors.Count; i++)
        {
            aimLazors[i].SetActive(true);
            float random1 = Random.Range(-distanceRange, distanceRange);
            float random2 = Random.Range(-distanceRange, distanceRange);
            
            aimLazors[i].transform.position = new Vector3(caster.transform.position.x + random1, this.transform.position.y, caster.transform.position.z + random2);
            aimLazors[i].GetComponent<JBR_Lazer_Aim>().StartAim();
        }
    }
}
