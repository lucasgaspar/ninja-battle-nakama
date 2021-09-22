using System.Collections.Generic;
using UnityEngine;

namespace NinjaBattle.Game
{
    [CreateAssetMenu(fileName = "MapData")]
    public class MapData : ScriptableObject
    {
        [SerializeField] private int width = default(int);
        [SerializeField] private int height = default(int);
        [SerializeField] private List<Vector2Int> walls = new List<Vector2Int>();
        [SerializeField] private List<Vector2Int> jumpables = new List<Vector2Int>();
        [SerializeField] private List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
        [SerializeField] private int minimumPlayers = default(int);
        [SerializeField] private int maximumPlayers = default(int);

        public int Width { get => width; }
        public int Height { get => height; }
        public List<Vector2Int> Walls { get => walls; }
        public List<Vector2Int> Jumpables { get => jumpables; }
        public List<SpawnPoint> SpawnPoints { get => spawnPoints; }
        public int MinimumPlayers { get => minimumPlayers; }
        public int MaximumPlayers { get => maximumPlayers; }
    }
}
