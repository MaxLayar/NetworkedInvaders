using System;
using Newtonsoft.Json;

namespace NetworkedInvaders.Network
{
    [Serializable]
    public class ClientMessage<T>
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

        [JsonIgnore]
        public bool IsSuccess => eventName != "error";
    }
}