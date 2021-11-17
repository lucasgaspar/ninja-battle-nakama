using UnityEngine;

using Nakama.Helpers;
using TMPro;

using NinjaBattle.General;

namespace NinjaBattle.Game
{
    public class BattleWinnerFeedback : MonoBehaviour
    {
        #region FIELDS

        public const string VictoryText = "VICTORY";
        public const string DefeatText = "DEFEAT";
        public const string DrawText = "DRAW";

        [SerializeField] private TMP_Text text = null;
        [SerializeField] private Color victoryColor = Color.white;
        [SerializeField] private Color defeatColor = Color.white;
        [SerializeField] private Color drawColor = Color.white;
        [SerializeField] private AudioClip victorySound = null;
        [SerializeField] private AudioClip defeatSound = null;
        [SerializeField] private AudioClip drawSound = null;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            MultiplayerManager.Instance.Subscribe(MultiplayerManager.Code.PlayerWon, ReceivedPlayerWonRound);
            MultiplayerManager.Instance.Subscribe(MultiplayerManager.Code.Draw, ReceivedDrawRound);
        }

        private void OnDestroy()
        {
            MultiplayerManager.Instance.Unsubscribe(MultiplayerManager.Code.PlayerWon, ReceivedPlayerWonRound);
            MultiplayerManager.Instance.Unsubscribe(MultiplayerManager.Code.Draw, ReceivedDrawRound);
        }

        private void ReceivedPlayerWonRound(MultiplayerMessage message)
        {
            AudioManager.Instance.StopMusic();
            PlayerWonData data = message.GetData<PlayerWonData>();
            bool playerWon = PlayersManager.Instance.CurrentPlayerNumber == data.PlayerNumber;
            AudioManager.Instance.PlaySound(playerWon ? victorySound : defeatSound);
            text.text = playerWon ? VictoryText : DefeatText;
            text.color = playerWon ? victoryColor : defeatColor;
        }

        private void ReceivedDrawRound(MultiplayerMessage message)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlaySound(drawSound);
            text.text = DrawText;
            text.color = drawColor;
        }

        #endregion
    }
}
