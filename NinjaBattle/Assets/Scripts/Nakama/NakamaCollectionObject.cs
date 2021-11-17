using System;
using UnityEngine;

namespace Nakama.Helpers
{
    [CreateAssetMenu(menuName = MenuName)]
    public class NakamaCollectionObject : ScriptableObject
    {
        #region FIELDS

        private const string MenuName = "Nakama/Collection object";

        [SerializeField] private string collection;
        [SerializeField] private string key;

        #endregion

        #region EVENTS

        public event Action onUpdated = null;

        #endregion

        #region PROPERTIES

        public string Collection { get => collection; }
        public string Key { get => key; }
        public string Value { get; private set; }
        public string Version { get; private set; }

        #endregion

        #region BEHAVIORS

        public void ResetData()
        {
            Value = default(string);
            Version = default(string);
        }

        public void SetValue(string newValue)
        {
            Value = newValue;
        }

        internal void SetDatabaseValue(string newValue, string newVersion)
        {
            Value = newValue;
            Version = newVersion;
            onUpdated?.Invoke();
        }

        public T GetValue<T>()
        {
            return Value == null ? default(T) : Value.Deserialize<T>();
        }

        #endregion
    }
}
