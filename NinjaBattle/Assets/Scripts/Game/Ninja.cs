using System;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private List<Color> ninjaColors = new List<Color>();

    private Direction currentDirection = Direction.East;
    private Dictionary<int, List<Direction>> nextDirections = new Dictionary<int, List<Direction>>();
    private Dictionary<int, Vector2Int> positions = new Dictionary<int, Vector2Int>();
    private Dictionary<int, Direction> directions = new Dictionary<int, Direction>();
    private Vector2Int desiredCoordinates = new Vector2Int();
    private Vector2Int currentCoordinates = new Vector2Int();
    private bool isJumping = false;
    private bool isDead = false;
    private Map map = null;

    public void Initialize(SpawnPoint spawnPoint, int playerNumber, Map map)
    {
        currentCoordinates = desiredCoordinates = spawnPoint.Coordinates;
        desiredCoordinates += spawnPoint.Direction.ToVector2();
        currentDirection = spawnPoint.Direction;
        this.map = map;
        spriteRenderer.transform.position = ((Vector3Int)currentCoordinates);
        spriteRenderer.color = ninjaColors[playerNumber];
        GameManager.Instance.onTick += ProcessTick;
        GameManager.Instance.onRewind += Rewind;
    }

    private void OnDestroy()
    {
        GameManager.Instance.onTick -= ProcessTick;
        GameManager.Instance.onRewind -= Rewind;
    }

    public void SetInput(Direction direction, int tick)
    {
        if (!nextDirections.ContainsKey(tick))
            nextDirections[tick] = new List<Direction>();

        nextDirections[tick].Add(direction);
    }

    public void ProcessTick(int tick)
    {
        //if (isDead)
        //    return;

        if (!isJumping)
        {
            if (!nextDirections.ContainsKey(tick))
                nextDirections[tick] = new List<Direction>();

            for (int i = 0; i < nextDirections[tick].Count; i++)
            {
                var nextDirection = nextDirections[tick][i];
                if (nextDirection == currentDirection)
                    continue;

                if (nextDirection == currentDirection.Opposite())
                    continue;

                currentDirection = nextDirection;
                break;
            }
        }

        Vector2Int newCoordinates = currentCoordinates + currentDirection.ToVector2();
        if (map.IsWallTile(newCoordinates))
        {
            map.SetTileAsDangerous(spriteRenderer.color, currentCoordinates);
            KillNinja();
        }
        else if (!isJumping && map.IsDangerousTile(newCoordinates))
        {
            JumpStart();
            map.SetTileAsDangerous(spriteRenderer.color, currentCoordinates);
        }
        else if (isJumping)
        {
            JumpEnd();
            if (map.IsDangerousTile(newCoordinates))
                KillNinja();
        }
        else
        {
            map.SetTileAsDangerous(spriteRenderer.color, currentCoordinates);
            if (map.IsDangerousTile(newCoordinates))
                KillNinja();
        }

        currentCoordinates = newCoordinates;
        spriteRenderer.transform.position = ((Vector3Int)currentCoordinates);

        if (positions.ContainsKey(tick))
            positions[tick] = currentCoordinates;
        else
            positions.Add(tick, currentCoordinates);

        if (directions.ContainsKey(tick))
            directions[tick] = currentDirection;
        else
            directions.Add(tick, currentDirection);

        Debug.Log(gameObject.name + " " + tick + " " + currentCoordinates + " " + currentDirection);
    }

    private void Rewind(int tick)
    {
        tick--;
        if (positions.ContainsKey(tick))
            currentCoordinates = positions[tick];

        if (directions.ContainsKey(tick))
            currentDirection = directions[tick];

        Debug.Log(gameObject.name + " " + tick + " " + currentCoordinates + " " + currentDirection);
    }

    private void JumpStart()
    {
        //isJumping = true;
        spriteRenderer.transform.localScale = Vector3.one * 0.6f;
    }

    private void JumpEnd()
    {
        //isJumping = false;
        spriteRenderer.transform.localScale = Vector3.one * 0.4f;
    }

    private void KillNinja()
    {
        //gameObject.SetActive(false);
        isDead = true;
    }
}
