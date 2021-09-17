using UnityEngine;

namespace Nakama.Helpers
{
    public class NakamaUserManager : MonoBehaviour
    {
        #region FIELDS

        private IApiAccount account = null;

        #endregion

        #region PROPERTIES

        public bool LoadingFinished { get; private set; } = false;
        public string DisplayName { get => account.User.DisplayName; }

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            NakamaManager.Instance.onLoginSuccess += LoadAsync;
        }

        private void OnDestroy()
        {
            NakamaManager.Instance.onLoginSuccess -= LoadAsync;
        }

        private async void LoadAsync()
        {
            account = await NakamaManager.Instance.Client.GetAccountAsync(NakamaManager.Instance.Session);
            LoadingFinished = true;
        }

        public T GetWallet<T>()
        {
            if (account == null || account.Wallet == null)
                return default(T);

            return account.Wallet.Deserialize<T>();
        }

        #endregion
    }
}
