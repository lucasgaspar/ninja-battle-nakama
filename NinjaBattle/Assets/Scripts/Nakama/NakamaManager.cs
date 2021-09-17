using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Nakama.Helpers
{
    public class NakamaManager : MonoBehaviour
    {
        #region FIELDS

        private IClient client = null;
        private ISession session = null;
        private ISocket socket = null;

        #endregion

        #region EVENTS

        public event Action onConnecting = null;
        public event Action onConnected = null;
        public event Action onDisconnected = null;
        public event Action onLoginSuccess = null;
        public event Action onLoginFail = null;

        #endregion

        #region PROPERTIES

        public static NakamaManager Instance { get; private set; } = null;
        public string Username { get => session == null ? string.Empty : session.Username; }
        public bool IsLoggedIn { get => socket != null && socket.IsConnected; }
        public ISocket Socket { get => socket; }
        public ISession Session { get => session; }
        public IClient Client { get => client; }

        #endregion

        #region BEHAVIORS

        private void Awake()
        {
            Instance = this;
        }

        private void OnApplicationQuit()
        {
            if (socket != null)
                socket.CloseAsync();
        }

        public void LoginWithDevice(NakamaConnectionData connectionData)
        {
            client = new Client(connectionData.Scheme, connectionData.Host, connectionData.Port, connectionData.ServerKey);
            LoginAsync(connectionData, client.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier));
        }

        public void LoginWithCustomId(NakamaConnectionData connectionData, string customId)
        {
            client = new Client(connectionData.Scheme, connectionData.Host, connectionData.Port, connectionData.ServerKey);
            LoginAsync(connectionData, client.AuthenticateCustomAsync(customId));
        }

        private async void LoginAsync(NakamaConnectionData connectionData, Task<ISession> sessionTask)
        {
            onConnecting?.Invoke();
            try
            {
                session = await sessionTask;
                Debug.Log(session);
                socket = client.NewSocket();
                socket.Connected += Connected;
                socket.Closed += Disconnected;
                await socket.ConnectAsync(session);
                UnityMainThread.AddJob(() => onLoginSuccess?.Invoke());
            }
            catch (Exception exception)
            {
                Debug.Log(exception);
                UnityMainThread.AddJob(() => onLoginFail?.Invoke());
            }
        }

        public void LogOut()
        {
            socket.CloseAsync();
        }

        private void Connected()
        {
            UnityMainThread.AddJob(() => onConnected?.Invoke());
        }

        private void Disconnected()
        {
            UnityMainThread.AddJob(() => onDisconnected?.Invoke());
        }

        public async Task<IApiRpc> SendRPC(string rpc, string payload = "{}")
        {
            if (client == null || session == null)
                return null;

            return await client.RpcAsync(session, rpc, payload);
        }

        #endregion
    }
}
