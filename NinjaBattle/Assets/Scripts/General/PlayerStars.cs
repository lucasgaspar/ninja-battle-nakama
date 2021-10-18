using System.Collections.Generic;
using UnityEngine;

using NinjaBattle.Game;

namespace NinjaBattle.General
{
    public class PlayerStars : MonoBehaviour
    {
        #region FIELDS

        private PlayersManager playersManager = null;

        [SerializeField] private PlayerPortrait portrait = null;
        [SerializeField] private Transform starsContainer = null;
        [SerializeField] private GameObject starPrefab = null;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            playersManager = PlayersManager.Instance;
            SetPortrait(playersManager.Players);
        }

        private void SetPortrait(List<PlayerData> players)
        {
            int playerNumber = portrait.PlayerNumber;
            bool hasPlayer = players.Count > playerNumber && players[playerNumber] != null;
            portrait.gameObject.SetActive(hasPlayer);
            for (int i = 0; i < GameManager.Instance.PlayersWins[playerNumber]; i++)
                Instantiate(starPrefab, starsContainer);
        }

        #endregion
    }
}
