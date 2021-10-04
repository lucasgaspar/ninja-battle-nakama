using System.Collections.Generic;
using UnityEngine;

using Nakama;

using NinjaBattle.Game;

namespace NinjaBattle.General
{
    public class PlayerThatWon : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private PlayerPortrait portrait = null;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            portrait.PlayerNumber = GameManager.Instance.Winner;
        }

        #endregion
    }
}
