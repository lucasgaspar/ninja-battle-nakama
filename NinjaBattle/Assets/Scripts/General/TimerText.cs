using TMPro;
using UnityEngine;

namespace NinjaBattle.General
{
    public class TimerText : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private string format = "{0} seconds remaining";
        [SerializeField] private TMP_Text text = null;
        [SerializeField] private Timer timer = null;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            timer.onSecondElapsed.AddListener(UpdateText);
            UpdateText(timer.TimeRemaining);
        }

        private void OnDestroy()
        {
            timer.onSecondElapsed.RemoveListener(UpdateText);
        }

        private void UpdateText(int seconds)
        {
            text.text = string.Format(format, seconds);
        }

        #endregion
    }
}
