using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Nakama.Helpers
{
    public class MultiplayerJoinMatchButton : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private Button button = null;
        [SerializeField] private string sceneName = "";

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            button.onClick.AddListener(FindMatch);
        }

        private void Start()
        {
            MultiplayerManager.Instance.onMatchJoin += ChangeScene;
        }

        private void OnDestroy()
        {
            MultiplayerManager.Instance.onMatchJoin -= ChangeScene;
        }

        private void FindMatch()
        {
            button.interactable = false;
            MultiplayerManager.Instance.JoinMatchAsync();
        }

        private void ChangeScene()
        {
            SceneManager.LoadScene(sceneName);
        }

        #endregion
    }
}
