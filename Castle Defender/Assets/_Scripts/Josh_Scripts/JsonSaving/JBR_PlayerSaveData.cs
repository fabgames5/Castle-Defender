using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class JBR_PlayerSaveData
{

    public string partName;
    public int partID;
    public Vector3 partlocation;
    public Quaternion partRotation;


    public JBR_PlayerSaveData(string name,int id, Vector3 loc, Quaternion rot)
    {
        this.partName = name;
        this.partID = id;
        this.partlocation = loc;
        this.partRotation = rot;
    }
}

    [Serializable]
    public class JBR_Data
    {
    public float coins;
    //list of all ship pieces...
    public List<JBR_PlayerSaveData> data;
    }