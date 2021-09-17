using UnityEngine;

namespace Nakama.Helpers
{
    public partial class MultiplayerIdentity : MonoBehaviour
    {
        #region FIELDS

        private static int currentId = 0;

        #endregion

        #region PROPERTIES

        public string Id { get; private set; }
        public bool IsLocalPlayer { get => MultiplayerManager.Instance.Self != null && MultiplayerManager.Instance.Self.SessionId == Id; }

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            AssignIdentity();
        }

        private void AssignIdentity()
        {
            Id = currentId++.ToString();
        }

        public void SetId(string id)
        {
            Id = id;
        }

        public static void ResetIds()
        {
            currentId = default(int);
        }

        #endregion
    }
}
