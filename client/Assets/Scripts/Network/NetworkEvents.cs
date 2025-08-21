using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NetworkedInvaders.Network
{
    public static class NetworkEvents
    {
        private static readonly Dictionary<string, Action<ServerMessage>> pendingRequests = new();
        private static readonly Dictionary<string, Action<ServerMessage>> handlers = new();

        public static event Action OnWebsocketOpen;
        public static event Action OnWebsocketClosed;

        public static void Init()
        {
            WebsocketHandler.OnConnected += () => OnWebsocketOpen?.Invoke();
            WebsocketHandler.OnDisconnected += () => OnWebsocketClosed?.Invoke();
            WebsocketHandler.OnRawMessage += HandleServerMessage;

            NetworkRegistry.InitHandlers();
        }

        public static void Send<T>(string eventName, T data, Action<ServerMessage> callback = null)
        {
            string requestId = Guid.NewGuid().ToString();
            if (callback != null) pendingRequests[requestId] = callback;

            var msg = new ClientMessage<T>
            {
                eventName = eventName,
                requestId = requestId,
                data = data
            };

            string json = JsonConvert.SerializeObject(msg);
            WebsocketHandler.Send(json);
        }

        private static void HandleServerMessage(string raw)
        {
            ServerMessage serverMsg;
            try
            {
                serverMsg = JsonConvert.DeserializeObject<ServerMessage>(raw);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to parse server message: " + e.Message);
                return;
            }

            if (!string.IsNullOrEmpty(serverMsg.requestId) && pendingRequests.TryGetValue(serverMsg.requestId, out var cb))
            {
                cb?.Invoke(serverMsg);
                pendingRequests.Remove(serverMsg.requestId);
                return;
            }

            if (handlers.TryGetValue(serverMsg.eventName, out var handler))
            {
                handler?.Invoke(serverMsg);
            }
            else
            {
                Debug.Log($"Unhandled message: {serverMsg.eventName} -> {raw}");
            }
        }

        public static void RegisterHandler(string eventName, Action<ServerMessage> handler)
        {
            handlers[eventName] = handler;
        }
    }
}
