using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nakama.Helpers
{
    public class NakamaDisconnectedChangeScene : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private string sceneName = null;

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            NakamaManager.Instance.onDisconnected += Disconnected;
        }

        private void OnDestroy()
        {
            NakamaManager.Instance.onDisconnected -= Disconnected;
        }

        private void Disconnected()
        {
            SceneManager.LoadScene(sceneName);
        }

        #endregion
    }
}
