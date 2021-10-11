using UnityEngine;

namespace NinjaBattle.Game
{
    public class Hazard : MonoBehaviour
    {
        #region FIELDS

        private RollbackVar<bool> wasCreated = new RollbackVar<bool>();
        private SpriteRenderer spriteRenderer = null;
        private Map map = null;

        #endregion

        #region PROPERTIES

        public Vector2Int Coordinates { get; private set; } = new Vector2Int();

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(int tick, Vector2Int coordinates, Color color, Map map)
        {
            this.map = map;
            spriteRenderer.color = color;
            Coordinates = coordinates;
            wasCreated[default(int)] = false;
            wasCreated[tick] = true;
            BattleManager.Instance.onRewind += Rewind;
        }

        private void OnDestroy()
        {
            BattleManager.Instance.onRewind -= Rewind;
        }

        private void Rewind(int tick)
        {
            tick--;
            if (wasCreated.GetLastValue(tick) == true)
                return;

            map.RemoveHazard(this);
            Destroy(this.gameObject);
        }

        #endregion
    }
}
