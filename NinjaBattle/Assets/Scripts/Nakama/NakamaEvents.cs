using UnityEngine;
using UnityEngine.Events;

namespace Nakama.Helpers
{
    public class NakamaEvents : MonoBehaviour
    {
        #region EVENTS

        public UnityEvent onConnecting = null;
        public UnityEvent onConnected = null;
        public UnityEvent onDisconnected = null;
        public UnityEvent onLoginSuccess = null;
        public UnityEvent onLoginFail = null;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            NakamaManager.Instance.onConnecting += OnConnecting;
            NakamaManager.Instance.onConnected += OnConnected;
            NakamaManager.Instance.onDisconnected += OnDisconnected;
            NakamaManager.Instance.onLoginSuccess += OnLoginSuccess;
            NakamaManager.Instance.onLoginFail += OnLoginFail;
        }

        private void OnDestroy()
        {
            NakamaManager.Instance.onConnecting -= OnConnecting;
            NakamaManager.Instance.onConnected -= OnConnected;
            NakamaManager.Instance.onDisconnected -= OnDisconnected;
            NakamaManager.Instance.onLoginSuccess -= OnLoginSuccess;
            NakamaManager.Instance.onLoginFail -= OnLoginFail;
        }

        private void OnConnecting()
        {
            onConnecting?.Invoke();
        }

        private void OnConnected()
        {
            onConnected?.Invoke();
        }

        private void OnDisconnected()
        {
            onDisconnected?.Invoke();
        }

        private void OnLoginSuccess()
        {
            onLoginSuccess?.Invoke();
        }

        private void OnLoginFail()
        {
            onLoginFail?.Invoke();
        }

        #endregion
    }
}
