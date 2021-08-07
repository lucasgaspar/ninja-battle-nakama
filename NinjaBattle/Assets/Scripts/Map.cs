using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private MapData mapData = null;
    [SerializeField] private GameObject hazardPrefab = null;
    [SerializeField] private GameObject wallPrefab = null;

    private List<Vector2Int> wallTiles = new List<Vector2Int>();
    private List<Vector2Int> dangerousTiles = new List<Vector2Int>();

    public static Map instance = null;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        int halfWidth = mapData.Width / 2;
        int halfHeight = mapData.Height / 2;
        for (int x = -halfWidth; x <= halfWidth; x++)
        {
            SetTileAsWall(new Vector2Int(x, halfHeight));
            SetTileAsWall(new Vector2Int(x, -halfHeight));
        }

        for (int y = -halfHeight; y <= halfHeight; y++)
        {
            SetTileAsWall(new Vector2Int(-halfWidth, y));
            SetTileAsWall(new Vector2Int(halfWidth, y));
        }

        foreach (Vector2Int wallCoordinates in mapData.Walls)
            SetTileAsWall(wallCoordinates);
    }

    public void SetTileAsWall(Vector2Int coordinates)
    {
        if (IsWallTile(coordinates))
            return;

        Instantiate(wallPrefab, (Vector2)coordinates, Quaternion.identity);
        wallTiles.Add(coordinates);
    }

    public void SetTileAsDangerous(Vector2Int coordinates)
    {
        if (IsDangerousTile(coordinates))
            return;

        Instantiate(hazardPrefab, (Vector2)coordinates, Quaternion.identity);
        dangerousTiles.Add(coordinates);
    }

    public bool IsWallTile(Vector2Int coordinates)
    {
        return wallTiles.Contains(coordinates);
    }

    public bool IsDangerousTile(Vector2Int coordinates)
    {
        return dangerousTiles.Contains(coordinates);
    }

}
