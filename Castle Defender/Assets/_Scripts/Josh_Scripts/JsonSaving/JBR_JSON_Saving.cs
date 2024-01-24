using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System;

[Serializable]
public class JBR_JSON_Saving : MonoBehaviour
{
    public JBR_Data saveData;
    [Space]
    public string path = "";
    public string persistantPath = "";
    [Space]
    public bool setPaths;
    public int savePathRef;
    public bool save;
    public bool load;
    [Space]
    public bool isBuilding = false;


    private void OnEnable()
    {
     //   SetPaths(savePathRef);
    }
    // Start is called before the first frame update
    void Start()
    {
        //testing only
     //   saveData = this.GetComponent<JBR_PlayerSaveData>();
    }

    // Update is called once per frame
    void Update()
    {
        if (setPaths)
        {
            setPaths = false;
            SetPaths(savePathRef);
        }

        if (save)
        {
            save = false;
            SavePlayerData();
        }

        if (load)
        {
            load = false;
            LoadPlayerData();
        }
    }

   public void SavePlayerData()
    {
        string json = JsonUtility.ToJson(saveData);
        Debug.Log(json);

        using StreamWriter writer = new StreamWriter(path);
        writer.Write(json);
    }

    public void LoadPlayerData()
    {
        using StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        Debug.Log(json.ToString());

        if (json != "")
        {
            JBR_Data newData = JsonUtility.FromJson<JBR_Data>(json);
         //  Debug.Log("Loading.... " + "\n" + newData.ToString());
            saveData = newData;
            LoadGameObjectFromList();
        }
        else
        {
            Debug.LogError("Loading ...... " +"\n"+ "No Data Found at Path !!!!");
        }


    }

    public void SetPaths(int saveFileRef)
    {
        path = (Application.dataPath + Path.AltDirectorySeparatorChar + "SaveFile" + saveFileRef + ".json");
        persistantPath = (Application.persistentDataPath +Path.AltDirectorySeparatorChar + "SaveFile" + saveFileRef + ".json");
    }

    /// <summary>
    /// Give both the prefab reference and scene gameobject reference
    /// </summary>
    /// <param name="prefabGO"></param>
    /// <param name="sceneGO"></param>
    public void AddGameobjectToList(GameObject prefabGO, int id, GameObject sceneGO )
    {
        JBR_PlayerSaveData newData = new JBR_PlayerSaveData(prefabGO.name,id,sceneGO.transform.position,sceneGO.transform.rotation);
        saveData.data.Add(newData);
    }

    public void LoadGameObjectFromList()
    {
        for (int i = 0; i < saveData.data.Count; i++)
        {
            if (Resources.Load(saveData.data[i].partName))
            {
             GameObject part = Instantiate (Resources.Load(saveData.data[i].partName), saveData.data[i].partlocation, saveData.data[i].partRotation) as GameObject;
                
             //       for (int b = 0; b < part.GetComponent<JBR_SpaceShipDesignStats>().buildPoints.Count; b++)
            //        {
            //            if (isBuilding)
            //            {
            //                part.GetComponent<JBR_SpaceShipDesignStats>().buildPoints[b].gameObject.SetActive(true);
            //            }
             //           else
              //          {
               //             part.GetComponent<JBR_SpaceShipDesignStats>().buildPoints[b].gameObject.SetActive(false);
              //          }
             //       }
                
                Debug.Log("Loading... " + saveData.data[i].partName);
            }
        }
    }
}
