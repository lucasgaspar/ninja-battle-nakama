using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData")]
public class MapData : ScriptableObject
{
    [SerializeField] private int width = default(int);
    [SerializeField] private int height = default(int);

    [SerializeField] private List<Tile> tiles = new List<Tile>();
}
