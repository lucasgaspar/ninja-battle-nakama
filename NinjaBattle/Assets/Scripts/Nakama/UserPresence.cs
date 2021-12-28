using Newtonsoft.Json;

namespace Nakama.Helpers
{
    public class UserPresence : IUserPresence
    {
        #region FIELDS

        private const string PersistenceKey = "persistence";
        private const string SessionIdKey = "sessionId";
        private const string StatusKey = "status";
        private const string UsernameKey = "username";
        private const string UserIdKey = "userId";

        #endregion

        #region PROPERTIES

        [JsonProperty(PersistenceKey)] public bool Persistence { get; private set; }
        [JsonProperty(SessionIdKey)] public string SessionId { get; private set; }
        [JsonProperty(StatusKey)] public string Status { get; private set; }
        [JsonProperty(UsernameKey)] public string Username { get; private set; }
        [JsonProperty(UserIdKey)] public string UserId { get; private set; }

        #endregion

        #region CONSTRUCTORS

        public UserPresence(bool persistence, string sessionId, string status, string username, string userId)
        {
            Persistence = persistence;
            SessionId = sessionId;
            Status = status;
            Username = username;
            UserId = userId;
        }

        #endregion
    }
}
