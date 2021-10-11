using UnityEngine;
using UnityEngine.UI;

using NinjaBattle.Game;

namespace NinjaBattle.General
{
    public class PlayerThatWon : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private PlayerPortrait portrait = null;
        [SerializeField] private Image winnerPortrait = null;
        [SerializeField] private Sprite[] playersPortrait = null;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            portrait.PlayerNumber = GameManager.Instance.Winner;
            winnerPortrait.sprite = playersPortrait[GameManager.Instance.Winner];
        }

        #endregion
    }
}
