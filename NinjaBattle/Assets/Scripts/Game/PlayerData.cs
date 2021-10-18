using UnityEngine;

using Newtonsoft.Json;

namespace NinjaBattle.Game
{
    public class PlayerData
    {
        #region FIELDS

        private const string PresenceKey = "presence";
        private const string DisplayNameKey = "displayName";

        #endregion

        #region PROPERTIES

        [JsonProperty(PresenceKey)] public PresenceData Presence { get; private set; }
        [JsonProperty(DisplayNameKey)] public string DisplayName { get; private set; }

        #endregion

        #region CONSTRUCTORS

        public PlayerData(PresenceData presence, string displayName)
        {
            Presence = presence;
            DisplayName = displayName;
        }

        #endregion
    }
}
