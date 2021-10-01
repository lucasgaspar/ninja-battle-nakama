using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using Nakama.Helpers;
using Nakama;

namespace NinjaBattle.Game
{
    public class BattleManager : MonoBehaviour
    {
        #region FIELDS

        private const float TickRate = 4f;

        [SerializeField] private List<MapData> maps = null;
        [SerializeField] private Map map = null;

        private List<IUserPresence> players = null;
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
            MultiplayerManager.Instance.Subscribe(MultiplayerManager.Code.PlayerInput, ReceivedPlayerInput);
            players = PlayersManager.Instance.Players;
            Initialize(players.Count);
            StartGame();
        }

        private void OnDestroy()
        {
            MultiplayerManager.Instance.Unsubscribe(MultiplayerManager.Code.PlayerInput, ReceivedPlayerInput);
        }

        private void ReceivedPlayerInput(MultiplayerMessage message)
        {
            InputData inputData = message.GetData<InputData>();
            SetPlayerInput(GetPlayerNumber(message.SessionId), inputData.Tick, (Direction)inputData.Direction);
        }

        private int GetPlayerNumber(string sessionId)
        {
            for (int i = 0; i < players.Count; i++)
                if (players[i].SessionId == sessionId)
                    return i;

            return -1;
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
            //else
            //    MultiplayerManager.Instance.Send(MultiplayerManager.Code.PlayerWon, new PlayerWonData(tick, GetPlayerNumber(playersAlive.First().SessionId)));
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
            {
                onRewind?.Invoke(tick);
                while (tick < CurrentTick)
                {
                    onTick?.Invoke(tick);
                    onTickEnd?.Invoke(tick);
                    tick++;
                }
            }

            if (tick > CurrentTick)
            {
                CancelInvoke(nameof(ProcessTick));
                InvokeRepeating(nameof(ProcessTick), TickDuration, TickDuration);
                while (tick > CurrentTick)
                {
                    onTick?.Invoke(tick);
                    onTickEnd?.Invoke(tick);
                    tick++;
                }
            }
        }

        #endregion
    }
}
