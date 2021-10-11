using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Nakama;

using NinjaBattle.Game;

namespace NinjaBattle.General
{
    public class PlayerPortrait : MonoBehaviour
    {
        #region FIELDS

        private PlayersManager playersManager = null;

        [SerializeField] private int playerNumber = 0;
        [SerializeField] private Image portrait = null;
        [SerializeField] private Color noPlayerColor = Color.white;
        [SerializeField] private Color connectedPlayerColor = Color.white;
        [SerializeField] private TMP_Text displayName = null;
        [SerializeField] private Color youColor = Color.white;
        [SerializeField] private Color othersColor = Color.white;

        #endregion

        #region PROPERTIES

        public int PlayerNumber { get => playerNumber; set => playerNumber = value; }

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            playersManager = PlayersManager.Instance;
            playersManager.onPlayerJoined += PlayerJoined;
            playersManager.onPlayerLeft += PlayerLeft;
            playersManager.onPlayersReceived += PlayersReceived;
            SetPortrait(playersManager.Players);
        }

        private void OnDestroy()
        {
            playersManager.onPlayerJoined -= PlayerJoined;
            playersManager.onPlayerLeft -= PlayerLeft;
            playersManager.onPlayersReceived -= PlayersReceived;
        }

        private void PlayersReceived(List<IUserPresence> players)
        {
            SetPortrait(players);
        }

        private void PlayerLeft(IUserPresence player)
        {
            SetPortrait(playersManager.Players);
        }

        private void PlayerJoined(IUserPresence player)
        {
            SetPortrait(playersManager.Players);
        }

        private void SetPortrait(List<IUserPresence> players)
        {
            bool hasPlayer = players.Count > playerNumber && players[playerNumber] != null;
            portrait.color = hasPlayer ? connectedPlayerColor : noPlayerColor;
            displayName.text = hasPlayer ? "Player " + (playerNumber + 1) : string.Empty;
            displayName.color = playersManager.CurrentPlayerNumber == playerNumber ? youColor : othersColor;
        }

        #endregion
    }
}
