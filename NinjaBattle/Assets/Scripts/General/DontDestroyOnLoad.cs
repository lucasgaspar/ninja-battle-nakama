using UnityEngine;

namespace NinjaBattle.General
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        #region BEHAVIORS

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        #endregion
    }
}
