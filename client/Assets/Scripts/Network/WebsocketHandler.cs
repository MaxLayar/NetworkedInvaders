using System;
using System.Text;
using UnityEngine;
using NativeWebSocket;
using NetworkedInvaders.Manager;

namespace NetworkedInvaders.Network
{
    public class WebsocketHandler : Singleton<WebsocketHandler>
    {
        [SerializeField] private string serverUrl = "ws://localhost:4444";
        private static WebSocket websocket;

        public static event Action OnConnected;
        public static event Action OnDisconnected;
        public static event Action<string> OnRawMessage;

        async void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            NetworkEvents.Init();
            
            websocket = new WebSocket(serverUrl);

            websocket.OnOpen += () => OnConnected?.Invoke();
            websocket.OnClose += (e) => OnDisconnected?.Invoke();
            websocket.OnError += (e) => Debug.LogWarning("WebSocket Error: " + e);

            websocket.OnMessage += (bytes) =>
            {
                string msg = Encoding.UTF8.GetString(bytes);
                OnRawMessage?.Invoke(msg);
            };

            await websocket.Connect();
        }

        void Update()
        {
            websocket?.DispatchMessageQueue();
        }

        private void OnApplicationQuit()
        {
            websocket?.Close();
        }

        public static void Send(string json)
        {
            if (websocket.State == WebSocketState.Open)
                websocket.SendText(json);
            else
                Debug.LogWarning("WebSocket not connected!");
        }
    }
}