using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData")]
public class MapData : ScriptableObject
{
    [SerializeField] private int width = default(int);
    [SerializeField] private int height = default(int);

    [SerializeField] private List<Vector2Int> walls = new List<Vector2Int>();

    public int Width { get => width; }
    public int Height { get => height; }
    public List<Vector2Int> Walls { get => walls; }
}
