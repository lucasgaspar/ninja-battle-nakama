using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData")]
public class MapData : ScriptableObject
{
    [SerializeField] int width = default(int);
    [SerializeField] int height = default(int);
}
