using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NetworkedInvaders.Network
{
    public static class NetworkRegistry
    {
        public static event Action<string, string> OnServerConnected;
        public static event Action<bool, string> OnLoginResult;

        public static void InitHandlers()
        {
            NetworkEvents.RegisterHandler("error", msg =>
            {
                
            });
            
            NetworkEvents.RegisterHandler("server:connection", msg =>
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
                    message = obj["username"]?.ToString();
                    success = msg.IsSuccess;

                    Debug.Log($"Login response success = {success}: {message}");
                }
                
                OnLoginResult?.Invoke(success, message);
            });
        }
        
        #region Emitters
        
        public static void Login(string username)
        {
            var data = new { username = username };
            NetworkEvents.Send("client:login", data);
        }
        
        #endregion Emitters
    }
}