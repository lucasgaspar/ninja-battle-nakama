using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using Nakama.Helpers;

namespace NinjaBattle.Game
{
    public class BattleManager : MonoBehaviour
    {
        #region FIELDS

        private const float TickRate = 4f;
        private const int Draw = -1;

        [SerializeField] private List<MapData> maps = null;
        [SerializeField] private List<string> players = new List<string>();
        [SerializeField] private Map map = null;

        private MapData currentMap = null;

        #endregion

        #region EVENTS

        public event Action<int> onTick = null;
        public event Action<int> onTickEnd = null;
        public event Action<int> onRewind = null;

        #endregion

        #region PROPERTIES

        public float TickDuration { get => 1 / TickRate; }
        public int CurrentTick { get; private set; }
        public static BattleManager Instance { get; private set; }

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            MultiplayerManager.Instance.Subscribe(MultiplayerManager.Code.PlayerWon, ReceivedPlayerWonRound);
            MultiplayerManager.Instance.Subscribe(MultiplayerManager.Code.PlayerInput, ReceivedPlayerInput);
            Initialize(players.Count);
            StartGame();
        }

        private void OnDestroy()
        {
            MultiplayerManager.Instance.Unsubscribe(MultiplayerManager.Code.PlayerWon, ReceivedPlayerWonRound);
            MultiplayerManager.Instance.Unsubscribe(MultiplayerManager.Code.PlayerInput, ReceivedPlayerInput);
        }

        private void ReceivedPlayerWonRound(MultiplayerMessage message)
        {
            PlayerWonData playerWonData = message.GetData<PlayerWonData>();
        }


        private void ReceivedPlayerInput(MultiplayerMessage message)
        {
            throw new NotImplementedException();
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
            onTickEnd += CheckWinner;
        }

        private void CheckWinner(int tick)
        {
            IEnumerable<Ninja> playersAlive = map.Ninjas.Where(ninja => ninja.IsAlive.GetLastValue(tick));
            if (playersAlive.Count() > 1)
                return;

            if (playersAlive.Count() == 0)
                MultiplayerManager.Instance.Send(MultiplayerManager.Code.Draw, new DrawData(tick));
            else
                MultiplayerManager.Instance.Send(MultiplayerManager.Code.PlayerWon, new PlayerWonData(tick, 0)); //MUST GET REAL PLAYER NUMBER
        }

        private void ProcessTick()
        {
            onTick?.Invoke(CurrentTick);
            onTickEnd?.Invoke(CurrentTick);
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
                onTickEnd?.Invoke(tick);
                tick++;
            }
        }

        #endregion
    }
}
