using System;
using UnityEngine;

namespace Nakama.Helpers
{
    public class NakamaUserManager : MonoBehaviour
    {
        #region FIELDS

        private IApiAccount account = null;

        #endregion

        #region EVENTS

        public event Action onLoaded = null;

        #endregion

        #region PROPERTIES

        public static NakamaUserManager Instance { get; private set; } = null;
        public bool LoadingFinished { get; private set; } = false;
        public IApiUser User { get => account.User; }
        public string Wallet { get => account.Wallet; }
        public string DisplayName { get => account.User.DisplayName; }

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            NakamaManager.Instance.onLoginSuccess += AutoLoad;
        }

        private void OnDestroy()
        {
            NakamaManager.Instance.onLoginSuccess -= AutoLoad;
        }

        private async void AutoLoad()
        {
            account = await NakamaManager.Instance.Client.GetAccountAsync(NakamaManager.Instance.Session);
            LoadingFinished = true;
            onLoaded?.Invoke();
        }

        public async void UpdateDisplayName(string displayName)
        {
            await NakamaManager.Instance.Client.UpdateAccountAsync(NakamaManager.Instance.Session, null, displayName);
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
