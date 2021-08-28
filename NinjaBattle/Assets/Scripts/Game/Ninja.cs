using System.Collections.Generic;
using UnityEngine;

public class Ninja : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private List<Color> ninjaColors = new List<Color>();

    private Direction currentDirection = Direction.East;
    private Queue<Direction> nextDirections = new Queue<Direction>();
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
    }

    public void SetNextDirection(Direction direction)
    {
        nextDirections.Enqueue(direction);
    }

    public void ProcessTick()
    {
        if (isDead)
            return;

        if (!isJumping)
        {
            while (nextDirections.Count > 0)
            {
                var nextDirection = nextDirections.Dequeue();
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
    }

    private void JumpStart()
    {
        isJumping = true;
        spriteRenderer.transform.localScale = Vector3.one * 0.6f;
    }

    private void JumpEnd()
    {
        isJumping = false;
        spriteRenderer.transform.localScale = Vector3.one * 0.4f;
    }

    private void KillNinja()
    {
        gameObject.SetActive(false);
        isDead = true;
    }
}
