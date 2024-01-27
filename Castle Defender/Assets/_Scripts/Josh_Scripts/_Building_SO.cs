using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "Building_SO_Data", menuName = "Building/ScriptableObjectData", order = 1)]
public class _Building_SO : ScriptableObject
{
    [Tooltip("Add prefab here")]
    public GameObject prefabReferenceGO;
    [Tooltip("Add Thumbnail UI Image here")]
    public Sprite thumbnail;
    [Tooltip("Add Base Cost here")]
    public int cost;
}
