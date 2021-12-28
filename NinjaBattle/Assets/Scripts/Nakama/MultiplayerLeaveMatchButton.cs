using UnityEngine;
using UnityEngine.UI;

namespace Nakama.Helpers
{
    public class MultiplayerLeaveMatchButton : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private Button button = null;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            button.onClick.AddListener(LeaveMatch);
        }

        private void LeaveMatch()
        {
            button.interactable = false;
            MultiplayerManager.Instance.LeaveMatchAsync();
        }

        #endregion
    }
}
