using System.Collections;
using UnityEngine;

public class Ninja : MonoBehaviour
{
    private const float Speed = 6.5f;

    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Vector2Int startingCoordinates = new Vector2Int();
    [SerializeField] private Direction startingDirection = Direction.East;
    [SerializeField] private KeyCode keyUp = KeyCode.None;
    [SerializeField] private KeyCode keyLeft = KeyCode.None;
    [SerializeField] private KeyCode keyRight = KeyCode.None;
    [SerializeField] private KeyCode keyDown = KeyCode.None;

    private Direction currentDirection = Direction.East;
    private Direction nextDirection = Direction.Undefined;
    private Vector2Int desiredCoordinates = new Vector2Int();
    private Vector2Int previousCoordinates = new Vector2Int();
    private Vector2 currentCoordinates = new Vector2();
    private bool isJumping = false;
    private bool beforeHalfStep = true;

    private void Start()
    {
        currentCoordinates = desiredCoordinates = previousCoordinates = startingCoordinates;
        desiredCoordinates += startingDirection.ToVector2();
        currentDirection = startingDirection;
        StartCoroutine(Step());
    }

    private IEnumerator Step()
    {
        beforeHalfStep = true;
        Invoke(nameof(HalfStep), 1f / Speed / 2f);
        while (Vector2.Distance(currentCoordinates, desiredCoordinates) > 0)
        {
            currentCoordinates = Vector2.MoveTowards(currentCoordinates, desiredCoordinates, Speed * Time.deltaTime);
            transform.position = currentCoordinates;
            yield return null;
        }

        previousCoordinates = desiredCoordinates;
        if (!isJumping && nextDirection != currentDirection.Opposite() && nextDirection != Direction.Undefined)
        {
            currentDirection = nextDirection;
            nextDirection = Direction.Undefined;
        }

        desiredCoordinates += currentDirection.ToVector2();
        StartCoroutine(Step());
        yield break;
    }

    private void HalfStep()
    {
        beforeHalfStep = false;
        ArrivedIntoNewTile(previousCoordinates, desiredCoordinates);
    }

    private void ArrivedIntoNewTile(Vector2Int previousCoordinates, Vector2Int newCoordinates)
    {
        if (Map.instance.IsWallTile(newCoordinates))
        {
            Map.instance.SetTileAsDangerous(previousCoordinates);
            KillNinja();
        }
        else if (!isJumping && Map.instance.IsDangerousTile(newCoordinates))
        {
            JumpStart();
            Map.instance.SetTileAsDangerous(previousCoordinates);
        }
        else if (isJumping)
        {
            JumpEnd();
            if (Map.instance.IsDangerousTile(newCoordinates))
                KillNinja();
        }
        else
        {
            Map.instance.SetTileAsDangerous(previousCoordinates);
            if (Map.instance.IsDangerousTile(newCoordinates))
                KillNinja();
        }
    }

    private void JumpStart()
    {
        isJumping = true;
        spriteRenderer.transform.localScale = Vector3.one * 0.55f;
        StartCoroutine(DoAFlip());
    }

    private IEnumerator DoAFlip()
    {
        float totalDuration = 1f / Speed;
        float elapsedTime = 0f;
        while (elapsedTime < totalDuration)
        {
            elapsedTime += Time.deltaTime;
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 360, elapsedTime / totalDuration));
            yield return null;
        }
        spriteRenderer.transform.rotation = Quaternion.identity;
        yield break;
    }

    private void JumpEnd()
    {
        isJumping = false;
        spriteRenderer.transform.localScale = Vector3.one * 0.4f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(keyUp))
            nextDirection = Direction.North;
        else if (Input.GetKeyDown(keyLeft))
            nextDirection = Direction.West;
        else if (Input.GetKeyDown(keyRight))
            nextDirection = Direction.East;
        else if (Input.GetKeyDown(keyDown))
            nextDirection = Direction.South;

        /*if (nextDirection != Direction.Undefined)
        {
            if (beforeHalfStep)
                desiredCoordinates = previousCoordinates + nextDirection.ToVector2();
            else
                desiredCoordinates = desiredCoordinates + nextDirection.ToVector2();

            currentDirection = nextDirection;
            nextDirection = Direction.Undefined;
        }*/
    }

    private void KillNinja()
    {
        Destroy(gameObject);
    }
}
