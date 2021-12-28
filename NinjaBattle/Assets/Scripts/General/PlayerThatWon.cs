using Nakama.Helpers;
using NinjaBattle.Game;
using UnityEngine;
using UnityEngine.UI;

namespace NinjaBattle.General
{
    public class PlayerThatWon : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private PlayerPortrait portrait = null;
        [SerializeField] private Image winnerPortrait = null;
        [SerializeField] private Sprite[] playersPortrait = null;
        [SerializeField] private NakamaCollectionObject nakamaCollectionObject = null;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            portrait.PlayerNumber = GameManager.Instance.Winner.Value;
            winnerPortrait.sprite = playersPortrait[GameManager.Instance.Winner.Value];
            if (PlayersManager.Instance.CurrentPlayerNumber == GameManager.Instance.Winner.Value)
            {
                TrophiesData trophiesData = nakamaCollectionObject.GetValue<TrophiesData>();
                trophiesData = new TrophiesData(trophiesData.Amount + 1);
                nakamaCollectionObject.SetValue(trophiesData.Serialize());
            }
        }

        #endregion
    }
}
