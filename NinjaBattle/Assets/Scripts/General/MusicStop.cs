using UnityEngine;

namespace NinjaBattle.General
{
    public class MusicStop : MonoBehaviour
    {
        #region BEHAVIORS

        private void Start()
        {
            AudioManager.Instance.StopMusic();
        }

        #endregion
    }
}
