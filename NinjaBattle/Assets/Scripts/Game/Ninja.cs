using System.Collections.Generic;
using NinjaBattle.General;
using UnityEngine;

namespace NinjaBattle.Game
{
    public class Ninja : MonoBehaviour
    {
        #region FIELDS

        private const float JumpScale = 1.5f;
        private const float NormalScale = 1f;

        [SerializeField] private SpriteRenderer spriteRenderer = null;
        [SerializeField] private List<Color> ninjaColors = new List<Color>();
        [SerializeField] private SpriteRenderer ninjaSpriteRenderer = null;
        [SerializeField] private List<AnimationData> ninjaAnimations = new List<AnimationData>();
        [SerializeField] private AudioClip jumpSound = null;
        [SerializeField] private AudioClip deathSound = null;

        private Direction currentDirection = Direction.East;
        private RollbackVar<List<Direction>> nextDirections = new RollbackVar<List<Direction>>();
        private RollbackVar<Vector2Int> positions = new RollbackVar<Vector2Int>();
        private RollbackVar<Direction> directions = new RollbackVar<Direction>();
        private Vector2Int desiredCoordinates = new Vector2Int();
        private Vector2Int currentCoordinates = new Vector2Int();
        private RollbackVar<bool> isJumping = new RollbackVar<bool>();
        private Map map = null;
        private int playerNumber = 0;
        private Vector3 currentVelocity = Vector3.zero;
        private Animation currentAnimation = null;

        #endregion

        #region PROPERTIES

        public RollbackVar<bool> IsAlive { get; private set; } = new RollbackVar<bool>();
        public string SessionId { get; private set; } = string.Empty;

        #endregion

        #region BEHAVIORS

        public void Initialize(SpawnPoint spawnPoint, int playerNumber, Map map, string sessionId)
        {
            this.playerNumber = playerNumber;
            currentCoordinates = desiredCoordinates = spawnPoint.Coordinates;
            desiredCoordinates += spawnPoint.Direction.ToVector2();
            currentDirection = spawnPoint.Direction;
            this.map = map;
            spriteRenderer.transform.position = ninjaSpriteRenderer.transform.position = ((Vector3Int)currentCoordinates);
            spriteRenderer.color = ninjaColors[playerNumber];
            currentAnimation = ninjaAnimations[playerNumber].RunAnimation;
            BattleManager.Instance.onTick += ProcessTick;
            BattleManager.Instance.onRewind += Rewind;
            isJumping[0] = false;
            IsAlive[0] = true;
            positions[0] = currentCoordinates;
            directions[0] = currentDirection;
            SessionId = sessionId;
            if (currentDirection.ToVector2().x > 0)
                ninjaSpriteRenderer.flipX = true;
            else if (currentDirection.ToVector2().x < 0)
                ninjaSpriteRenderer.flipX = false;
        }

        private void OnDestroy()
        {
            BattleManager.Instance.onTick -= ProcessTick;
            BattleManager.Instance.onRewind -= Rewind;
        }

        private void Update()
        {
            ninjaSpriteRenderer.transform.position = Vector3.SmoothDamp(ninjaSpriteRenderer.transform.position, spriteRenderer.transform.position, ref currentVelocity, 0.175f);
            if (currentAnimation != null)
                ninjaSpriteRenderer.sprite = currentAnimation.GetCurrentFrame(Time.deltaTime);
        }

        public void SetInput(Direction direction, int tick)
        {
            if (!nextDirections.HasValue(tick))
                nextDirections[tick] = new List<Direction>();

            nextDirections[tick].Add(direction);
        }

        public void ProcessTick(int tick)
        {
            if (BattleManager.Instance.RoundEnded.GetLastValue(tick))
                return;

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
            if (currentDirection.ToVector2().x > 0)
                ninjaSpriteRenderer.flipX = true;
            else if (currentDirection.ToVector2().x < 0)
                ninjaSpriteRenderer.flipX = false;

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
            IsAlive.EraseFuture(tick);
            spriteRenderer.transform.localScale = ninjaSpriteRenderer.transform.localScale = Vector3.one * (isJumping[tick] ? JumpScale : NormalScale);
            if (!IsAlive.GetLastValue(tick))
                currentAnimation = ninjaAnimations[playerNumber].DeathAnimation;
            else if (isJumping[tick])
                currentAnimation = ninjaAnimations[playerNumber].JumpAnimation;
            else
                currentAnimation = ninjaAnimations[playerNumber].RunAnimation;
        }

        private void JumpStart(int tick)
        {
            isJumping[tick] = true;
            spriteRenderer.transform.localScale = ninjaSpriteRenderer.transform.localScale = Vector3.one * JumpScale;
            currentAnimation = ninjaAnimations[playerNumber].JumpAnimation;
            currentAnimation.Reset();
            AudioManager.Instance.PlaySound(jumpSound);
        }

        private void JumpEnd(int tick)
        {
            isJumping[tick] = false;
            spriteRenderer.transform.localScale = ninjaSpriteRenderer.transform.localScale = Vector3.one * NormalScale;
            currentAnimation = ninjaAnimations[playerNumber].RunAnimation;
        }

        private void KillNinja(int tick)
        {
            IsAlive[tick] = false;
            currentAnimation = ninjaAnimations[playerNumber].DeathAnimation;
            currentAnimation.Reset();
            AudioManager.Instance.PlaySound(deathSound);
        }

        #endregion
    }
}
