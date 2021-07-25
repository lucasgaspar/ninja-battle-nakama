using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private GameObject hazardPrefab = null;
    [SerializeField] private TextAsset[] maps = null;
    [SerializeField] private Tile[] tiles = null;

    private List<Vector2Int> dangerousTiles = new List<Vector2Int>();

    public static Map instance = null;

    private void Start()
    {
        instance = this;
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

    public void SetTileAsDangerous(Vector2Int coordinates)
    {
        Instantiate(hazardPrefab, (Vector2)coordinates, Quaternion.identity);
        dangerousTiles.Add(coordinates);
    }

    public bool IsDangerousTile(Vector2Int coordinates)
    {
        return dangerousTiles.Contains(coordinates);
    }

}
