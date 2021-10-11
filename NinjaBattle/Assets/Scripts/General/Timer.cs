using System;
using UnityEngine;
using UnityEngine.Events;

namespace NinjaBattle.General
{
    public class Timer : MonoBehaviour
    {
        #region FIELDS

        private const float OneSecond = 1f;

        [SerializeField] private int duration = 5;
        [SerializeField] private bool autoStart = true;

        #endregion

        #region EVENTS

        public UnityEvent<int> onSecondElapsed = null;
        public UnityEvent onTimerEnd = null;

        #endregion

        #region PROPERTIES

        public int TimeRemaining { get; private set; } = 0;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            TimeRemaining = duration;
        }

        private void Start()
        {
            if (autoStart)
                StartTimer();
        }

        private void SecondElapsed()
        {
            TimeRemaining--;
            onSecondElapsed?.Invoke(TimeRemaining);
            if (TimeRemaining > 0)
                Invoke(nameof(SecondElapsed), OneSecond);
            else
                onTimerEnd?.Invoke();
        }

        public void StartTimer()
        {
            Invoke(nameof(SecondElapsed), OneSecond);
        }

        public void PauseTimer()
        {
            CancelInvoke(nameof(SecondElapsed));
        }

        public void StopTimer()
        {
            TimeRemaining = duration;
            CancelInvoke(nameof(SecondElapsed));
        }

        public void ResetTimer()
        {
            TimeRemaining = duration;
            CancelInvoke(nameof(SecondElapsed));
            Invoke(nameof(SecondElapsed), OneSecond);
        }

        #endregion
    }
}
