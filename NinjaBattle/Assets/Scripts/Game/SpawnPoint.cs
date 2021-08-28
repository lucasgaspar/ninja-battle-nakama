using System;
using UnityEngine;

[Serializable]
public class SpawnPoint
{
    [SerializeField] private Vector2Int coordinates = Vector2Int.zero;
    [SerializeField] private Direction direction = Direction.West;

    public Vector2Int Coordinates { get => coordinates; }
    public Direction Direction { get => direction; }
}
