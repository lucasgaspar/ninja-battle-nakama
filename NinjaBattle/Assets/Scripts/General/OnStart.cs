using UnityEngine;
using UnityEngine.Events;

namespace NinjaBattle.General
{
    public class OnStart : MonoBehaviour
    {
        #region EVENTS

        public UnityEvent onStart = null;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            onStart?.Invoke();
        }

        #endregion
    }
}
