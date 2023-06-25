using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct FurDataStruct
{
    public Color furColor;
}
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/FurData", order = 1)]
public class FurData : ScriptableObject
{
    [SerializeField]
    public List<FurDataStruct> furDataList = new List<FurDataStruct>();
}
