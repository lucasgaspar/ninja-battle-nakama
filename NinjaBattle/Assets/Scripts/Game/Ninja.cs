using System.Collections.Generic;
using UnityEngine;

public class Ninja : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private List<Color> ninjaColors = new List<Color>();

    private Direction currentDirection = Direction.East;
    private RollbackVar<List<Direction>> nextDirections = new RollbackVar<List<Direction>>();
    private RollbackVar<Vector2Int> positions = new RollbackVar<Vector2Int>();
    private RollbackVar<Direction> directions = new RollbackVar<Direction>();
    private Vector2Int desiredCoordinates = new Vector2Int();
    private Vector2Int currentCoordinates = new Vector2Int();
    private RollbackVar<bool> isJumping = new RollbackVar<bool>();
    private RollbackVar<bool> isDead = new RollbackVar<bool>();
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
        isJumping[0] = false;
        isDead[0] = false;
        positions[0] = currentCoordinates;
        directions[0] = currentDirection;
    }

    private void OnDestroy()
    {
        GameManager.Instance.onTick -= ProcessTick;
        GameManager.Instance.onRewind -= Rewind;
    }

    public void SetInput(Direction direction, int tick)
    {
        if (!nextDirections.HasValue(tick))
            nextDirections[tick] = new List<Direction>();

        nextDirections[tick].Add(direction);
    }

    public void ProcessTick(int tick)
    {
        if (isDead.GetLastValue(tick))
            return;

        if (!isJumping.GetLastValue(tick))
        {
            if (!nextDirections.HasValue(tick))
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
            map.SetHazard(tick, spriteRenderer.color, currentCoordinates);
            KillNinja(tick);
        }
        else if (!isJumping.GetLastValue(tick) && map.IsDangerousTile(newCoordinates))
        {
            JumpStart(tick);
            map.SetHazard(tick, spriteRenderer.color, currentCoordinates);
        }
        else if (isJumping.GetLastValue(tick))
        {
            JumpEnd(tick);
            if (map.IsDangerousTile(newCoordinates))
                KillNinja(tick);
        }
        else
        {
            map.SetHazard(tick, spriteRenderer.color, currentCoordinates);
            if (map.IsDangerousTile(newCoordinates))
                KillNinja(tick);
        }

        currentCoordinates = newCoordinates;
        spriteRenderer.transform.position = ((Vector3Int)currentCoordinates);

        positions[tick] = currentCoordinates;
        directions[tick] = currentDirection;
    }

    private void Rewind(int tick)
    {
        tick--;
        if (positions.HasValue(tick))
            currentCoordinates = positions[tick];

        if (directions.HasValue(tick))
            currentDirection = directions[tick];

        isJumping.EraseFuture(tick);
        spriteRenderer.transform.localScale = Vector3.one * (isJumping[tick] ? 0.6f : 0.4f);
        isDead.EraseFuture(tick);
    }

    private void JumpStart(int tick)
    {
        isJumping[tick] = true;
        spriteRenderer.transform.localScale = Vector3.one * 0.6f;
    }

    private void JumpEnd(int tick)
    {
        isJumping[tick] = false;
        spriteRenderer.transform.localScale = Vector3.one * 0.4f;
    }

    private void KillNinja(int tick)
    {
        isDead[tick] = true;
    }
}
