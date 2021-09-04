using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private Hazard hazardPrefab = null;
    [SerializeField] private GameObject waterPrefab = null;
    [SerializeField] private GameObject wallPrefab = null;
    [SerializeField] private Ninja ninjaPrefab = null;

    private List<Vector2Int> mapWallTiles = new List<Vector2Int>();
    private List<Vector2Int> mapDangerousTiles = new List<Vector2Int>();
    private List<Vector2Int> ninjaDangerousTiles = new List<Vector2Int>();

    public List<Ninja> Ninjas { get; private set; } = new List<Ninja>();

    public void Initialize(MapData mapData, List<string> players)
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

        foreach (Vector2Int obstacle in mapData.Jumpables)
            SetTileAsWater(obstacle);

        for (int playerNumber = 0; playerNumber < players.Count; playerNumber++)
            InstantiateNinja(playerNumber, mapData.SpawnPoints[playerNumber]);
    }

    public void SetTileAsWall(Vector2Int coordinates)
    {
        if (IsWallTile(coordinates))
            return;

        Instantiate(wallPrefab, (Vector2)coordinates, Quaternion.identity);
        mapWallTiles.Add(coordinates);
    }

    public void SetTileAsWater(Vector2Int coordinates)
    {
        if (IsDangerousTile(coordinates))
            return;

        Instantiate(waterPrefab, (Vector2)coordinates, Quaternion.identity);
        mapDangerousTiles.Add(coordinates);
    }

    public void SetHazard(int tick, Color color, Vector2Int coordinates)
    {
        if (IsDangerousTile(coordinates))
            return;

        Hazard dangerousTile = Instantiate(hazardPrefab, (Vector2)coordinates, Quaternion.identity);
        dangerousTile.Initialize(tick, coordinates, color, this);
        ninjaDangerousTiles.Add(coordinates);
    }

    public void RemoveHazard(Hazard hazard)
    {
        if (!ninjaDangerousTiles.Contains(hazard.Coordinates))
            return;

        ninjaDangerousTiles.Remove(hazard.Coordinates);
    }

    public bool IsWallTile(Vector2Int coordinates)
    {
        return mapWallTiles.Contains(coordinates);
    }

    public bool IsDangerousTile(Vector2Int coordinates)
    {
        return mapDangerousTiles.Contains(coordinates) || ninjaDangerousTiles.Contains(coordinates);
    }

    public void InstantiateNinja(int playerNumber, SpawnPoint spawnPoint)
    {
        Ninja ninja = Instantiate(ninjaPrefab);
        ninja.Initialize(spawnPoint, playerNumber, this);
        Ninjas.Add(ninja);
    }

    public Ninja GetNinja(int playerNumber)
    {
        return Ninjas[playerNumber];
    }
}
