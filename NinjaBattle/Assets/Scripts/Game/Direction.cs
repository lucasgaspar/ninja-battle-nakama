using UnityEngine;

namespace NinjaBattle.Game
{
    public enum Direction
    {
        North,
        West,
        East,
        South
    }

    static class DirectionMethods
    {
        public static Vector2Int ToVector2(this Direction direction)
        {
            switch (direction)
            {
                case Direction.North: return Vector2Int.up;
                case Direction.West: return Vector2Int.left;
                case Direction.East: return Vector2Int.right;
                case Direction.South: return Vector2Int.down;
                default: return Vector2Int.zero;
            }
        }

        public static Direction Opposite(this Direction direction)
        {
            switch (direction)
            {
                case Direction.North: return Direction.South;
                case Direction.West: return Direction.East;
                case Direction.East: return Direction.West;
                case Direction.South: return Direction.North;
                default: return Direction.West;
            }
        }
    }
}
