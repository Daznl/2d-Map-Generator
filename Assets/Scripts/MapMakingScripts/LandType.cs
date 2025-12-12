using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LandType", menuName = "ScriptableObjects/LandType", order = 1)]
public class LandType : ScriptableObject
{
    public string landTypeName;
    public Sprite sprite;
    public int altitude;
    // Add any other properties specific to land types
}