using UnityEngine;

namespace NinjaBattle.Game
{
    [CreateAssetMenu(menuName = MenuName)]
    public class AnimationData : ScriptableObject
    {
        #region FIELDS

        private const string MenuName = "NinjaBattle/AnimationData";

        [SerializeField] private Animation runAnimation = null;
        [SerializeField] private Animation jumpAnimation = null;
        [SerializeField] private Animation deathAnimation = null;

        #endregion

        #region PROPERTIES

        public Animation RunAnimation { get => runAnimation; }
        public Animation JumpAnimation { get => jumpAnimation; }
        public Animation DeathAnimation { get => deathAnimation; }

        #endregion
    }
}
