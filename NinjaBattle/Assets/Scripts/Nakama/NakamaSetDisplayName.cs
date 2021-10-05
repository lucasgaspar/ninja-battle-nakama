using UnityEngine;

using TMPro;

namespace Nakama.Helpers
{
    public class NakamaSetDisplayName : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private TMP_InputField inputField = null;
        [SerializeField] private string[] firstPart = null;
        [SerializeField] private string[] secondPart = null;

        private NakamaUserManager nakamaUserManager = null;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            nakamaUserManager = NakamaUserManager.Instance;
            nakamaUserManager.onLoaded += ObtainName;
            inputField.onValueChanged.AddListener(UpdateName);
            if (nakamaUserManager.LoadingFinished)
                ObtainName();
        }

        private void OnDestroy()
        {
            inputField.onValueChanged.RemoveListener(UpdateName);
            nakamaUserManager.onLoaded -= ObtainName;
        }

        private void ObtainName()
        {
            if (string.IsNullOrEmpty(nakamaUserManager.DisplayName))
                inputField.text = firstPart[Random.Range(0, firstPart.Length)] + secondPart[Random.Range(0, secondPart.Length)];
            else
                inputField.text = nakamaUserManager.DisplayName;
        }

        private void UpdateName(string newDisplayName)
        {
            if (newDisplayName != nakamaUserManager.DisplayName)
                nakamaUserManager.UpdateDisplayName(newDisplayName);
        }

        #endregion
    }
}
