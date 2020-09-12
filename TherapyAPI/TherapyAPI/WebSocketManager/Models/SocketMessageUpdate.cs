using System;
using Newtonsoft.Json;
using TherapyAPI.WebSocketManager.Enums;

namespace TherapyAPI.WebSocketManager.Models
{
    public class SocketMessageUpdate
    {
        [JsonProperty("type")]
        public SocketMessageType Type { get; set; }

        [JsonProperty("message")]
        public object Message { get; set; }

        public SocketMessageUpdate(SocketMessageType type, object message)
        {
            Type = type;
            Message = message;
        }
    }
}
