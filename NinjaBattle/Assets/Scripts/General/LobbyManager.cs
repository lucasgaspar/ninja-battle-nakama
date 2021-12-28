using System.Collections.Generic;
using NinjaBattle.Game;
using UnityEngine;

namespace NinjaBattle.General
{
    public class LobbyManager : MonoBehaviour
    {
        #region FIELDS

        private PlayersManager playersManager = null;

        [SerializeField] private GameObject waitingText = null;
        [SerializeField] private Timer timer = null;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            playersManager = PlayersManager.Instance;
            playersManager.onPlayerJoined += PlayerJoined;
            playersManager.onPlayerLeft += PlayerLeft;
            playersManager.onPlayersReceived += PlayersReceived;
            UpdateStatus();
        }

        private void OnDestroy()
        {
            playersManager.onPlayerJoined -= PlayerJoined;
            playersManager.onPlayerLeft -= PlayerLeft;
            playersManager.onPlayersReceived -= PlayersReceived;
        }

        private void PlayersReceived(List<PlayerData> players)
        {
            UpdateStatus();
        }

        private void PlayerLeft(PlayerData player)
        {
            UpdateStatus();
        }

        private void PlayerJoined(PlayerData player)
        {
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            bool gameStarting = playersManager.PlayersCount > 1;
            waitingText.SetActive(!gameStarting);
            timer.gameObject.SetActive(gameStarting);
            if (gameStarting)
                timer.ResetTimer();
        }

        #endregion
    }
}
