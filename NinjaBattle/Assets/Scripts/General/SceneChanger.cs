using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NinjaBattle.General
{
    public class SceneChanger : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private float delay = 0f;
        [SerializeField] private Scenes scene;

        #endregion

        #region BEHAVIORS

        public void ChangeScene()
        {
            StartCoroutine(ChangeSceneCoroutine());
        }

        private IEnumerator ChangeSceneCoroutine()
        {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene((int)scene);
        }

        #endregion
    }
}
