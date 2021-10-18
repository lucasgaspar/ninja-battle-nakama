using UnityEngine;

namespace NinjaBattle.Game
{
    [CreateAssetMenu(menuName = MenuName)]
    public class AnimationData : ScriptableObject
    {
        #region FIELDS

        private const string MenuName = "NinjaBattle/AnimationData";

        [SerializeField] private Sprite[] runAnimation = null;
        [SerializeField] private Sprite[] jumpAnimation = null;
        [SerializeField] private Sprite[] fallAnimation = null;
        [SerializeField] private Sprite[] deathAnimation = null;

        #endregion

        #region PROPERTIES

        public Sprite[] RunAnimation { get => runAnimation; }
        public Sprite[] JumpAnimation { get => jumpAnimation; }
        public Sprite[] FallAnimation { get => fallAnimation; }
        public Sprite[] DeathAnimation { get => deathAnimation; }

        #endregion
    }
}
