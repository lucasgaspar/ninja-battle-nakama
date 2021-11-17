using Newtonsoft.Json;

namespace NinjaBattle.General
{
    public class TrophiesData
    {
        #region FIELDS

        private const string AmountKey = "amount";

        #endregion

        #region PROPERTIES

        [JsonProperty(AmountKey)] public int Amount { get; private set; }

        #endregion

        #region CONSTRUCTORS

        public TrophiesData(int amount = 0)
        {
            Amount = amount;
        }

        #endregion
    }
}
