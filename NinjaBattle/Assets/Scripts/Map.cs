using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    private const int MapWidth = 34;
    private const int MapHeight = 20;

    [SerializeField] private GameObject hazardPrefab = null;
    [SerializeField] private GameObject wallPrefab = null;

    private List<Vector2Int> dangerousTiles = new List<Vector2Int>();

    public static Map instance = null;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        int halfWidth = MapWidth / 2;
        int halfHeight = MapHeight / 2;
        for (int x = -halfWidth; x <= halfWidth; x++)
        {
            SetTileAsDangerous(new Vector2Int(x, halfHeight));
            SetTileAsDangerous(new Vector2Int(x, -halfHeight));
        }

        for (int y = -halfHeight; y <= halfHeight; y++)
        {
            SetTileAsDangerous(new Vector2Int(-halfWidth, y));
            SetTileAsDangerous(new Vector2Int(halfWidth, y));
        }
    }

    public void SetTileAsDangerous(Vector2Int coordinates)
    {
        if (IsDangerousTile(coordinates))
            return;

        Instantiate(hazardPrefab, (Vector2)coordinates, Quaternion.identity);
        dangerousTiles.Add(coordinates);
    }

    public bool IsDangerousTile(Vector2Int coordinates)
    {
        return dangerousTiles.Contains(coordinates);
    }

}
