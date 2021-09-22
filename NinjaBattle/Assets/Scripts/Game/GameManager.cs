using System;
using System.Collections.Generic;
using UnityEngine;

namespace NinjaBattle.Game
{
    public class GameManager : MonoBehaviour
    {
        private const float TickRate = 4f;

        [SerializeField] private List<MapData> maps = null;
        [SerializeField] private List<string> players = new List<string>();
        [SerializeField] private Map map = null;

        private MapData currentMap = null;

        public event Action<int> onTick = null;
        public event Action<int> onRewind = null;

        public float TickDuration { get => 1 / TickRate; }
        public int CurrentTick { get; private set; }
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Initialize(players.Count);
            StartGame();
        }

        private void Initialize(int playersAmount)
        {
            List<MapData> possibleMaps = maps.FindAll(map => playersAmount >= map.MinimumPlayers && playersAmount <= map.MaximumPlayers);
            currentMap = possibleMaps[UnityEngine.Random.Range(0, possibleMaps.Count)];
            map.Initialize(currentMap, players);
        }

        private void StartGame()
        {
            InvokeRepeating(nameof(ProcessTick), TickDuration, TickDuration);
        }

        private void ProcessTick()
        {
            onTick?.Invoke(CurrentTick);
            CurrentTick++;
        }

        public void SetPlayerInput(int playerNumber, int tick, Direction direction)
        {
            if (tick <= default(int))
                return;

            map.GetNinja(playerNumber).SetInput(direction, tick);
            if (tick < CurrentTick)
                onRewind?.Invoke(tick);

            while (tick < CurrentTick)
            {
                onTick?.Invoke(tick);
                tick++;
            }
        }
    }
}
