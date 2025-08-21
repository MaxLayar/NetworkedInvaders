using System;
using NetworkedInvaders.Manager;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NetworkedInvaders.Network
{
    public static class NetworkRegistry
    {
        public static event Action<string, string> OnServerConnected;
        public static event Action<bool, string> OnLoginResult;
        public static event Action<string, int, int> OnScoresReceived;
        public static event Action<int> OnTimerReceived;
        public static event Action OnRoundEndedReceived;

        public static void InitHandlers()
        {
            NetworkEvents.RegisterHandler("ws:open", msg =>
            {
                if (msg.data is JObject obj)
                {
                    string id = obj["clientID"]?.ToString();
                    string welcome = obj["message"]?.ToString();

                    Debug.Log($"Connected as {id}: {welcome}");

                    OnServerConnected?.Invoke(id, welcome);
                }
            });

            NetworkEvents.RegisterHandler("client:login", msg =>
            {
                bool success = false;
                string message = "";
                if (msg.data is JObject obj)
                {
                    success = obj["success"]?.ToObject<bool>() ?? false;
                    message = obj["message"]?.ToString();

                    Debug.Log($"Login response {(success ? "success" : "failure")}: {message}");
                }

                OnLoginResult?.Invoke(success, message);
            });

            NetworkEvents.RegisterHandler("client:scoreUpdate", msg =>
            {
                if (msg.data is JObject obj)
                {
                    if (!obj["success"]?.ToObject<bool>() ?? false)
                    {
                        Debug.LogError("Score submission failed");
                        return;
                    }

                    string username = obj["username"]?.ToString();
                    int score = obj["score"]?.ToObject<int>() ?? 0;
                    int highscore = obj["highscore"]?.ToObject<int>() ?? 0;

                    OnScoresReceived?.Invoke(username, score, highscore);
                }
            });

            NetworkEvents.RegisterHandler("server:countdown", msg =>
            {
                if (msg.data is JObject obj)
                {
                    int remainingTime = obj["remainingTime"]?.ToObject<int>() ?? 0;

                    OnTimerReceived?.Invoke(remainingTime);
                }
            });

            NetworkEvents.RegisterHandler("server:roundEnded", msg =>
            {
                OnRoundEndedReceived?.Invoke();
            });
        }

        #region Emitters

        public static void InitEmitters()
        {
            UIManager.OnLoginSubmission += Login;
            GameManager.OnScoreChanged += OnScoreChanged;
        }
        
        private static void Login(string username)
        {
            var data = new { username = username };
            NetworkEvents.Send("client:login", data);
        }

        private static void OnScoreChanged(int newScore)
        {
            var data = new { score = newScore };
            NetworkEvents.Send("client:scoreUpdate", data);
        }
        
        #endregion Emitters
    }
}