using System;
using UnityEngine;

namespace NinjaBattle.Game
{
    [Serializable]
    public class SpawnPoint
    {
        #region FIELDS

        [SerializeField] private Vector2Int coordinates = Vector2Int.zero;
        [SerializeField] private Direction direction = Direction.West;

        #endregion

        #region PROPERTIES

        public Vector2Int Coordinates { get => coordinates; }
        public Direction Direction { get => direction; }

        #endregion
    }
}
