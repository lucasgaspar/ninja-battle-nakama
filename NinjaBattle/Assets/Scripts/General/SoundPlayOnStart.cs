using UnityEngine;

namespace NinjaBattle.General
{
    public class SoundPlayOnStart : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private AudioClip sound = null;

        #endregion

        #region BEHAVIORS

        private void Start()
        {
            AudioManager.Instance.PlaySound(sound);
        }

        #endregion
    }
}
