using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NetworkedInvaders.Manager
{
	public class NetworkManager : Singleton<NetworkManager>
	{
		[SerializeField] private string serverUrl = "ws://localhost:4444";
		private WebSocket websocket;

		private string clientID = "";

		public static event Action OnWebsocketOpen;
		public static event Action OnWebsocketClosed;
		public static event Action<ServerMessage> OnLoggedIn;
		
		private Dictionary<string, Action<ServerMessage>> pendingRequests = new();

		[Serializable]
		private class PlayerData
		{
			public string username;
		}
		
		[Serializable]
		class ClientMessage<T>
		{
			public string eventName;
			public string requestId;
			public T data;
		}
		
		[Serializable]
		public class ServerMessage
		{
			public string eventName;
			public string requestId;
			public object data;

			public bool IsSuccess() => eventName != "error";
		}
		
		async void Start()
		{
			websocket = new WebSocket(serverUrl);

			websocket.OnOpen += () =>
			{
				Debug.Log("WebSocket connected!");
				OnWebsocketOpen?.Invoke();
			};

			websocket.OnError += (e) =>
			{
				Debug.LogWarning("WebSocket Error: " + e);
			};

			websocket.OnClose += (e) =>
			{
				Debug.Log("WebSocket closed with code: " + e);
				OnWebsocketClosed?.Invoke();
			};

			websocket.OnMessage += (bytes) =>
			{
				string message = Encoding.UTF8.GetString(bytes);

				HandleServerMessage(message);
			};
			
			await websocket.Connect();
		}
		
		void Update()
		{
			websocket.DispatchMessageQueue();
		}
		
		private void OnApplicationQuit()
		{
			websocket?.Close();
		}
		
		internal void Login(string username)
		{
			Send("client:login", new PlayerData { username = username }, response =>
			{
				OnLoggedIn?.Invoke(response);
				if (!response.IsSuccess())
				{
					Debug.LogError("Error while logging in! " + response.data);
				}
				Debug.Log("Login Successful: " + response.data);
				
			});
		}
		
		private void Send<T>(string eventName, T data, Action<ServerMessage> callback)
		{
			if (websocket.State != WebSocketState.Open)
			{
				Debug.LogWarning("WebSocket not connected!");
				return;
			}
		
			string requestId = Guid.NewGuid().ToString();
			pendingRequests[requestId] = callback;
			
			ClientMessage<T> msg = new()
			{
				eventName = eventName,
				requestId = requestId,
				data = data
			};
			
			string jsonString = JsonConvert.SerializeObject(msg);
			websocket.SendText(jsonString);
		}

		private void HandleServerMessage(string message)
		{
			ServerMessage serverMsg;
			try
			{
				serverMsg = JsonConvert.DeserializeObject<ServerMessage>(message);
			}
			catch (Exception e)
			{
				Debug.LogWarning("Failed to parse server message: " + e.Message);
				return;
			}

			if (serverMsg.eventName == "server:connection")
			{
				var typed = JsonConvert.DeserializeObject<ServerMessage>(message);
				if (typed.data is JObject obj)
				{
					string msg = obj["message"]?.ToString();
					string clientId = obj["clientID"]?.ToString();
					Debug.Log($"Welcome {clientId} → {msg}");

					clientID = clientId;
				}
			}
			else if (serverMsg.eventName == "client:login")
			{
				var typed = JsonConvert.DeserializeObject<ServerMessage>(message);
				if (typed.data is JObject obj)
				{
					string msg = obj["message"]?.ToString();
					Debug.Log($"Hello → {msg}");
				}
				Debug.Log("User: " + typed.data);
			}
			
			if (!string.IsNullOrEmpty(serverMsg.requestId) && pendingRequests.TryGetValue(serverMsg.requestId, out var callback))
			{
				callback?.Invoke(serverMsg);
				pendingRequests.Remove(serverMsg.requestId);
			}
			else
			{
				Debug.LogError("TODO: Broadcast or untracked message: " + message);
			}
		}
	}
}
