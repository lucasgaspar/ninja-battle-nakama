using System;
using System.Collections.Generic;
using UnityEngine;

using Nakama;
using Nakama.Helpers;

namespace NinjaBattle.Game
{
    public class PlayersManager : MonoBehaviour
    {
        #region FIELDS

        private MultiplayerManager multiplayerManager = null;

        #endregion

        #region EVENTS

        public event Action<List<IUserPresence>> onPlayersReceived;
        public event Action<IUserPresence> onPlayerJoined;
        public event Action<IUserPresence> onPlayerLeft;

        #endregion

        #region PROPERTIES

        public List<IUserPresence> Players { get; private set; }

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            multiplayerManager = MultiplayerManager.Instance;
        }

        private void Start()
        {
            NakamaManager.Instance.Socket.ReceivedMatchPresence += PlayersChanged;
            multiplayerManager.Subscribe(MultiplayerManager.Code.Players, SetPlayers);
            multiplayerManager.onMatchLeave += ResetPlayersData;
        }

        private void OnDestroy()
        {
            NakamaManager.Instance.Socket.ReceivedMatchPresence -= PlayersChanged;
            multiplayerManager.Unsubscribe(MultiplayerManager.Code.Players, SetPlayers);
            multiplayerManager.onMatchLeave -= ResetPlayersData;
        }

        private void SetPlayers(MultiplayerMessage message)
        {
            Players = message.GetData<List<IUserPresence>>();
            onPlayersReceived?.Invoke(Players);
        }

        private void PlayersChanged(IMatchPresenceEvent matchPresenceEvent)
        {
            foreach (IUserPresence userPresence in matchPresenceEvent.Leaves)
            {
                Players.ForEach(presence =>
                {
                    if (presence != null && presence.SessionId == userPresence.SessionId)
                        presence = null;
                });

                onPlayerLeft?.Invoke(userPresence);
            }

            foreach (IUserPresence userPresence in matchPresenceEvent.Joins)
            {
                int index = Players.IndexOf(null);
                if (index > -1)
                    Players[index] = userPresence;
                else
                    Players.Add(userPresence);

                onPlayerJoined?.Invoke(userPresence);
            }
        }

        private void ResetPlayersData()
        {
            Players = null;
        }

        #endregion
    }
}
