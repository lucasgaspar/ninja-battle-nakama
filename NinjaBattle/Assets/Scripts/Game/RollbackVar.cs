using System.Collections.Generic;

namespace NinjaBattle.Game
{
    public class RollbackVar<T>
    {
        #region FIELDS

        private Dictionary<int, T> history = new Dictionary<int, T>();

        #endregion

        #region PROPERTIES

        public T this[int tick]
        {
            get
            {
                return history.ContainsKey(tick) ? history[tick] : default(T);
            }

            set
            {
                if (history.ContainsKey(tick))
                    history[tick] = value;
                else
                    history.Add(tick, value);
            }
        }

        #endregion

        #region BEHAVIORS

        public bool HasValue(int tick)
        {
            return history.ContainsKey(tick);
        }

        public T GetLastValue(int tick)
        {
            for (; tick >= 0; tick--)
                if (HasValue(tick))
                    return history[tick];

            return default(T);
        }

        public void EraseFuture(int tick)
        {
            List<int> keysToErase = new List<int>();
            foreach (KeyValuePair<int, T> keyValuePair in history)
                if (keyValuePair.Key > tick)
                    keysToErase.Add(keyValuePair.Key);

            foreach (int i in keysToErase)
                history.Remove(i);
        }

        #endregion
    }
}
