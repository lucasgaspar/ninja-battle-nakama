using System.Collections.Generic;
using NinjaBattle.Game;
using UnityEngine;
using UnityEngine.UI;

namespace NinjaBattle.General
{
    public class PlayerStars : MonoBehaviour
    {
        #region FIELDS

        private PlayersManager playersManager = null;

        [SerializeField] private PlayerPortrait portrait = null;
        [SerializeField] private Image[] stars = null;

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
            int playersWins = GameManager.Instance.PlayersWins[playerNumber];
            for (int i = 0; i < GameManager.VictoriesRequiredToWin; i++)
                if (i < playersWins)
                    stars[i].color = Color.white;
        }

        #endregion
    }
}
