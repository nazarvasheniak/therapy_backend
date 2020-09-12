using System;
using Newtonsoft.Json;

namespace TherapyAPI.WebSocketManager.Models
{
    public class SocketMessage
    {
        [JsonProperty("userID")]
        public long UserID { get; set; }
    }
}
