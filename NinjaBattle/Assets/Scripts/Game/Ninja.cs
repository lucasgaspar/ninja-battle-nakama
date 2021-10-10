using System.Collections.Generic;
using UnityEngine;

namespace NinjaBattle.Game
{
    public class Ninja : MonoBehaviour
    {
        #region FIELDS

        private const float JumpScale = 0.6f;
        private const float NormalScale = 0.4f;

        [SerializeField] private SpriteRenderer spriteRenderer = null;
        [SerializeField] private List<Color> ninjaColors = new List<Color>();
        [SerializeField] private SpriteRenderer ninjaSpriteRenderer = null;
        [SerializeField] private List<Sprite> ninjaSprites = new List<Sprite>();

        private Direction currentDirection = Direction.East;
        private RollbackVar<List<Direction>> nextDirections = new RollbackVar<List<Direction>>();
        private RollbackVar<Vector2Int> positions = new RollbackVar<Vector2Int>();
        private RollbackVar<Direction> directions = new RollbackVar<Direction>();
        private Vector2Int desiredCoordinates = new Vector2Int();
        private Vector2Int currentCoordinates = new Vector2Int();
        private RollbackVar<bool> isJumping = new RollbackVar<bool>();
        private Map map = null;

        #endregion

        #region PROPERTIES

        public RollbackVar<bool> IsAlive { get; private set; } = new RollbackVar<bool>();
        public string SessionId { get; private set; } = string.Empty;

        #endregion

        #region BEHAVIORS

        public void Initialize(SpawnPoint spawnPoint, int playerNumber, Map map, string sessionId)
        {
            currentCoordinates = desiredCoordinates = spawnPoint.Coordinates;
            desiredCoordinates += spawnPoint.Direction.ToVector2();
            currentDirection = spawnPoint.Direction;
            this.map = map;
            spriteRenderer.transform.position = ninjaSpriteRenderer.transform.position = ((Vector3Int)currentCoordinates);
            spriteRenderer.color = ninjaColors[playerNumber];
            ninjaSpriteRenderer.sprite = ninjaSprites[playerNumber];
            BattleManager.Instance.onTick += ProcessTick;
            BattleManager.Instance.onRewind += Rewind;
            isJumping[0] = false;
            IsAlive[0] = true;
            positions[0] = currentCoordinates;
            directions[0] = currentDirection;
            SessionId = sessionId;
        }

        private void OnDestroy()
        {
            BattleManager.Instance.onTick -= ProcessTick;
            BattleManager.Instance.onRewind -= Rewind;
        }

        Vector3 currentVelocity = Vector3.zero;
        private void Update()
        {
            ninjaSpriteRenderer.transform.position = Vector3.SmoothDamp(ninjaSpriteRenderer.transform.position, spriteRenderer.transform.position, ref currentVelocity, 0.15f);
        }

        public void SetInput(Direction direction, int tick)
        {
            if (!nextDirections.HasValue(tick))
                nextDirections[tick] = new List<Direction>();

            nextDirections[tick].Add(direction);
        }

        public void ProcessTick(int tick)
        {
            if (!IsAlive.GetLastValue(tick))
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
            spriteRenderer.transform.localScale = Vector3.one * (isJumping[tick] ? JumpScale : NormalScale);
            IsAlive.EraseFuture(tick);
        }

        private void JumpStart(int tick)
        {
            isJumping[tick] = true;
            spriteRenderer.transform.localScale = Vector3.one * JumpScale;
        }

        private void JumpEnd(int tick)
        {
            isJumping[tick] = false;
            spriteRenderer.transform.localScale = Vector3.one * NormalScale;
        }

        private void KillNinja(int tick)
        {
            IsAlive[tick] = false;
        }

        #endregion
    }
}
