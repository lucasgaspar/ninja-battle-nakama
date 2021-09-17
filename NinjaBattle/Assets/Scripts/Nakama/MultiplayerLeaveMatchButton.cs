using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Nakama.Helpers
{
    public class MultiplayerLeaveMatchButton : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private Button button = null;
        [SerializeField] private string sceneName = "";

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            button.onClick.AddListener(LeaveMatch);
            MultiplayerManager.Instance.onMatchLeave += ChangeScene;
        }

        private void OnDestroy()
        {
            MultiplayerManager.Instance.onMatchLeave -= ChangeScene;
        }

        private void LeaveMatch()
        {
            button.interactable = false;
            MultiplayerManager.Instance.LeaveMatchAsync();
        }

        private void ChangeScene()
        {
            SceneManager.LoadScene(sceneName);
        }

        #endregion
    }
}
