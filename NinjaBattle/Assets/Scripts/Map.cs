using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private TextAsset[] maps = null;
    [SerializeField] private Tile[] tiles = null;

    private void Start()
    {
        TextAsset map = GetRandomMap();
        GenerateMap(map);
    }

    private TextAsset GetRandomMap()
    {
        return maps[Random.Range(0, maps.Length)];
    }

    private void GenerateMap(TextAsset map)
    {
        
    }
}
