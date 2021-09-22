using UnityEngine;
using UnityEngine.SceneManagement;

namespace NinjaBattle.General
{
    public class SceneChanger : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private Scenes scene;

        #endregion

        #region BEHAVIORS

        public void ChangeScene()
        {
            SceneManager.LoadScene((int)scene);
        }

        #endregion
    }
}
