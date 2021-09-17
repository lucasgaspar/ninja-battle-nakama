using UnityEngine;

namespace Nakama.Helpers
{
    public class NakamaAutoLogin : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private float retryTime = 5f;
        [SerializeField] private NakamaConnectionData nakamaConnectionData = null;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            NakamaManager.Instance.onLoginFail += LoginFailed;
            TryLogin();
        }

        private void OnDestroy()
        {
            NakamaManager.Instance.onLoginFail -= LoginFailed;
        }

        private void TryLogin()
        {
            NakamaManager.Instance.LoginWithDevice(nakamaConnectionData);
        }

        private void LoginFailed()
        {
            Invoke(nameof(TryLogin), retryTime);
        }

        #endregion
    }
}
