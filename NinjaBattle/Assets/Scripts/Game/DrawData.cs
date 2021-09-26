using Newtonsoft.Json;

namespace NinjaBattle.Game
{
    public class DrawData
    {
        #region FIELDS

        private const string TickKey = "tick";

        #endregion

        #region PROPERTIES

        [JsonProperty(TickKey)] public int Tick { get; private set; }

        #endregion

        #region CONSTRUCTORS

        public DrawData(int tick)
        {
            Tick = tick;
        }

        #endregion
    }
}
