namespace Nakama.Helpers
{
    public class MultiplayerMessage
    {
        #region FIELDS

        private string json = null;
        private byte[] bytes = null;

        #endregion

        #region PROPERTIES

        public MultiplayerManager.Code DataCode { get; private set; }
        public string SessionId { get; private set; }
        public string UserId { get; private set; }
        public string Username { get; private set; }

        #endregion

        #region BEHAVIORS

        public MultiplayerMessage(IMatchState matchState)
        {
            DataCode = (MultiplayerManager.Code)matchState.OpCode;
            if (matchState.UserPresence != null)
            {
                UserId = matchState.UserPresence.UserId;
                SessionId = matchState.UserPresence.SessionId;
                Username = matchState.UserPresence.Username;
            }

            var encoding = System.Text.Encoding.UTF8;
            json = encoding.GetString(matchState.State);
            bytes = matchState.State;
        }

        public T GetData<T>()
        {
            return json.Deserialize<T>();
        }

        public byte[] GetBytes()
        {
            return bytes;
        }

        #endregion
    }
}
