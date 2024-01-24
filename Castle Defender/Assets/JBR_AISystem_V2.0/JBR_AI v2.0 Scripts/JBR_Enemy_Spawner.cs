using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JBR_Enemy_Spawner : MonoBehaviour
{
    [Tooltip("Add Enemy prefab here")]
    public GameObject enemyPrefab;
    [Tooltip("Spawning Distance range of the enemies")]
    [Range(1, 50)]
    public float distanceRange = 10;
    [Tooltip("Total  Enemy count to be spawned")]
    public int total = 25; 

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private List<bool> enemyDead = new List<bool>();
    [Tooltip("the transform of the one that  set this spawner")]
    public Transform caster;
    [Tooltip("if checked true, the caster location is the center point of the spawning, other this spawning object location is")]
    public bool useCasterLocation = true;
    [Tooltip("If this is a boss fight stage, then add the new stage to move too")]
    public int stage;

    public void OnEnable()
    {
        for (int i = 0; i < total; i++)
        {
            GameObject spawned = Instantiate(enemyPrefab, this.transform.position, Quaternion.identity) as GameObject;
            spawnedEnemies.Add(spawned);
            spawned.SetActive(false);
            SpawnEnemyLocation();
        }
    }
    /// <summary>
    /// Sets the caster of the spawner
    /// </summary>
    /// <param name="newCaster"></param>
    public void SetCaster(Transform newCaster)
    {
        caster = newCaster;
    }

    public void StageClearDeaths()
    {
        for (int i = 0; i < spawnedEnemies.Count; i++)
        {
            if (spawnedEnemies[i].GetComponent<JBR_AI_ControllerSystem>().isDead)
            {
                enemyDead[i] = true;
            }
        }

       
        for (int i = 0; i < enemyDead.Count; i++)
        {
            if(enemyDead[i] == false)
            {
                return;
            }
        }

        StageComplete();
    }

    private void StageComplete()
    {
        // code to move to next stage
        caster.gameObject.SendMessage("UpdateStage", stage, SendMessageOptions.DontRequireReceiver);
    }


    public void SpawnEnemyLocation()
    {
        for (int i = 0; i < spawnedEnemies.Count; i++)
        {
            spawnedEnemies[i].SetActive(true);
            float random1 = Random.Range(-distanceRange, distanceRange);
            float random2 = Random.Range(-distanceRange, distanceRange);
            if (useCasterLocation)
            {
                spawnedEnemies[i].transform.position = new Vector3(caster.transform.position.x + random1, this.transform.position.y, caster.transform.position.z + random2);
            }
            else
            {
                spawnedEnemies[i].transform.position = new Vector3(this.transform.position.x + random1, this.transform.position.y, this.transform.position.z + random2);
            }

            spawnedEnemies[i].GetComponent<JBR_AI_ControllerSystem>().OnControllerDied.AddListener(StageClearDeaths);
        } 
        
    }
}
