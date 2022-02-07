using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nakama.Helpers
{
    public partial class MultiplayerManager : MonoBehaviour
    {
        #region FIELDS

        private const int TickRate = 5;
        private const float SendRate = 1f / (float)TickRate;
        private const string JoinOrCreateMatchRpc = "JoinOrCreateMatchRpc";
        private const string LogFormat = "{0} with code {1}:\n{2}";
        private const string SendingDataLog = "Sending data";
        private const string ReceivedDataLog = "Received data";

        [SerializeField] private bool enableLog = false;

        private Dictionary<Code, Action<MultiplayerMessage>> onReceiveData = new Dictionary<Code, Action<MultiplayerMessage>>();
        private IMatch match = null;

        #endregion

        #region EVENTS

        public event Action onMatchJoin = null;
        public event Action onMatchLeave = null;
        public event Action onLocalTick = null;

        #endregion

        #region PROPERTIES

        public static MultiplayerManager Instance { get; private set; } = null;
        public IUserPresence Self { get => match == null ? null : match.Self; }
        public bool IsOnMatch { get => match != null; }

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            InvokeRepeating(nameof(LocalTickPassed), SendRate, SendRate);
        }

        private void LocalTickPassed()
        {
            onLocalTick?.Invoke();
        }

        public async void JoinMatchAsync()
        {
            NakamaManager.Instance.Socket.ReceivedMatchState -= Receive;
            NakamaManager.Instance.Socket.ReceivedMatchState += Receive;
            NakamaManager.Instance.onDisconnected += Disconnected;
            IApiRpc rpcResult = await NakamaManager.Instance.SendRPC(JoinOrCreateMatchRpc);
            string matchId = rpcResult.Payload;
            match = await NakamaManager.Instance.Socket.JoinMatchAsync(matchId);
            onMatchJoin?.Invoke();
        }

        private void Disconnected()
        {
            NakamaManager.Instance.onDisconnected -= Disconnected;
            NakamaManager.Instance.Socket.ReceivedMatchState -= Receive;
            match = null;
            onMatchLeave?.Invoke();
        }

        public async void LeaveMatchAsync()
        {
            NakamaManager.Instance.onDisconnected -= Disconnected;
            NakamaManager.Instance.Socket.ReceivedMatchState -= Receive;
            await NakamaManager.Instance.Socket.LeaveMatchAsync(match);
            match = null;
            onMatchLeave?.Invoke();
        }

        public void Send(Code code, object data = null)
        {
            if (match == null)
                return;

            string json = data != null ? data.Serialize() : string.Empty;
            if (enableLog)
                LogData(SendingDataLog, (long)code, json);

            NakamaManager.Instance.Socket.SendMatchStateAsync(match.Id, (long)code, json);
        }

        public void Send(Code code, byte[] bytes)
        {
            if (match == null)
                return;

            if (enableLog)
                LogData(SendingDataLog, (long)code, String.Empty);

            NakamaManager.Instance.Socket.SendMatchStateAsync(match.Id, (long)code, bytes);
        }

        private void Receive(IMatchState newState)
        {
            if (enableLog)
            {
                var encoding = System.Text.Encoding.UTF8;
                var json = encoding.GetString(newState.State);
                LogData(ReceivedDataLog, newState.OpCode, json);
            }

            MultiplayerMessage multiplayerMessage = new MultiplayerMessage(newState);
            if (onReceiveData.ContainsKey(multiplayerMessage.DataCode))
                onReceiveData[multiplayerMessage.DataCode]?.Invoke(multiplayerMessage);
        }

        public void Subscribe(Code code, Action<MultiplayerMessage> action)
        {
            if (!onReceiveData.ContainsKey(code))
                onReceiveData.Add(code, null);

            onReceiveData[code] += action;
        }

        public void Unsubscribe(Code code, Action<MultiplayerMessage> action)
        {
            if (onReceiveData.ContainsKey(code))
                onReceiveData[code] -= action;
        }

        private void LogData(string description, long dataCode, string json)
        {
            Debug.Log(string.Format(LogFormat, description, (Code)dataCode, json));
        }

        #endregion
    }
}
