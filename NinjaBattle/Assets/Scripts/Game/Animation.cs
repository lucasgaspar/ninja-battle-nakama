using System;
using UnityEngine;

namespace NinjaBattle.Game
{
    [Serializable]
    public class Animation
    {
        #region FIELDS

        [SerializeField] private Sprite[] frames = null;
        [SerializeField] private int framesPerSecond = 12;
        [SerializeField] private bool loop = true;

        private float frame = 0;

        #endregion

        #region PROPERTIES

        private int CurrentFrame { get => Mathf.FloorToInt(frame); }

        #endregion

        #region BEHAVIORS

        public void Reset()
        {
            frame = 0;
        }

        public Sprite GetCurrentFrame(float deltaTime)
        {
            frame += deltaTime * framesPerSecond;
            if (frame >= frames.Length)
                frame = loop ? 0 : frames.Length - 1;

            return frames[CurrentFrame];
        }

        #endregion
    }
}
