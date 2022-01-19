using TMPro;
using UnityEngine;
using Nakama.Helpers;

namespace NinjaBattle.General
{
    public class SetDisplayName : MonoBehaviour
    {
        #region FIELDS

        private const float delay = 1f;

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
            inputField.onValueChanged.AddListener(ValueChanged);
            if (nakamaUserManager.LoadingFinished)
                ObtainName();
        }

        private void OnDestroy()
        {
            inputField.onValueChanged.RemoveListener(ValueChanged);
            nakamaUserManager.onLoaded -= ObtainName;
        }

        private void ObtainName()
        {
            if (string.IsNullOrEmpty(nakamaUserManager.DisplayName))
                inputField.text = firstPart[Random.Range(0, firstPart.Length)] + secondPart[Random.Range(0, secondPart.Length)];
            else
                inputField.text = nakamaUserManager.DisplayName;
        }

        private void ValueChanged(string newValue)
        {
            CancelInvoke(nameof(UpdateName));
            Invoke(nameof(UpdateName), delay);
        }

        private void UpdateName()
        {
            if (inputField.text != nakamaUserManager.DisplayName)
                nakamaUserManager.UpdateDisplayName(inputField.text);
        }

        #endregion
    }
}
