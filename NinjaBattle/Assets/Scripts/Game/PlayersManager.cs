using System;
using System.Collections.Generic;
using System.Linq;
using Nakama;
using Nakama.Helpers;
using UnityEngine;

namespace NinjaBattle.Game
{
    public class PlayersManager : MonoBehaviour
    {
        #region FIELDS

        private NakamaManager nakamaManager = null;
        private MultiplayerManager multiplayerManager = null;
        private bool blockJoinsAndLeaves = false;

        #endregion

        #region EVENTS

        public event Action<List<PlayerData>> onPlayersReceived;
        public event Action<PlayerData> onPlayerJoined;
        public event Action<PlayerData> onPlayerLeft;
        public event Action<PlayerData, int> onLocalPlayerObtained;

        #endregion

        #region PROPERTIES

        public static PlayersManager Instance { get; private set; } = null;
        public List<PlayerData> Players { get; private set; } = new List<PlayerData>();
        public int PlayersCount { get => Players.Count(player => player != null); }
        public PlayerData CurrentPlayer { get; private set; } = null;
        public int CurrentPlayerNumber { get; private set; } = -1;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            multiplayerManager = MultiplayerManager.Instance;
            nakamaManager = NakamaManager.Instance;
            multiplayerManager.onMatchJoin += MatchJoined;
            multiplayerManager.onMatchLeave += ResetLeaved;
            multiplayerManager.Subscribe(MultiplayerManager.Code.Players, SetPlayers);
            multiplayerManager.Subscribe(MultiplayerManager.Code.PlayerJoined, PlayerJoined);
            multiplayerManager.Subscribe(MultiplayerManager.Code.ChangeScene, MatchStarted);
        }

        private void OnDestroy()
        {
            multiplayerManager.onMatchJoin -= MatchJoined;
            multiplayerManager.onMatchLeave -= ResetLeaved;
            multiplayerManager.Unsubscribe(MultiplayerManager.Code.Players, SetPlayers);
            multiplayerManager.Unsubscribe(MultiplayerManager.Code.PlayerJoined, PlayerJoined);
            multiplayerManager.Unsubscribe(MultiplayerManager.Code.ChangeScene, MatchStarted);
        }

        private void SetPlayers(MultiplayerMessage message)
        {
            Players = message.GetData<List<PlayerData>>();
            onPlayersReceived?.Invoke(Players);
            GetCurrentPlayer();
        }

        private void PlayerJoined(MultiplayerMessage message)
        {
            PlayerData player = message.GetData<PlayerData>();
            int index = Players.IndexOf(null);
            if (index > -1)
                Players[index] = player;
            else
                Players.Add(player);

            onPlayerJoined?.Invoke(player);
        }

        private void PlayersChanged(IMatchPresenceEvent matchPresenceEvent)
        {
            if (blockJoinsAndLeaves)
                return;

            foreach (IUserPresence userPresence in matchPresenceEvent.Leaves)
            {
                for (int i = 0; i < Players.Count(); i++)
                {
                    if (Players[i] != null && Players[i].Presence.SessionId == userPresence.SessionId)
                    {
                        onPlayerLeft?.Invoke(Players[i]);
                        Players[i] = null;
                    }
                }
            }
        }

        private void MatchJoined()
        {
            nakamaManager.Socket.ReceivedMatchPresence += PlayersChanged;
            GetCurrentPlayer();
        }

        private void GetCurrentPlayer()
        {
            if (Players == null)
                return;

            if (multiplayerManager.Self == null)
                return;

            if (CurrentPlayer != null)
                return;

            CurrentPlayer = Players.Find(player => player.Presence.SessionId == multiplayerManager.Self.SessionId);
            CurrentPlayerNumber = Players.IndexOf(CurrentPlayer);
            onLocalPlayerObtained?.Invoke(CurrentPlayer, CurrentPlayerNumber);
        }

        private void ResetLeaved()
        {
            nakamaManager.Socket.ReceivedMatchPresence -= PlayersChanged;
            blockJoinsAndLeaves = false;
            Players = null;
            CurrentPlayer = null;
            CurrentPlayerNumber = -1;
        }

        public void MatchStarted(MultiplayerMessage message)
        {
            blockJoinsAndLeaves = true;
        }

        #endregion
    }
}
