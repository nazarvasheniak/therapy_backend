using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using TherapyAPI.WebSocketManager.Models;

namespace TherapyAPI.WebSocketManager.Interfaces
{
    public interface INotificationService
    {
        List<SocketUser> SocketUsers { get; }
        Task SendMessageAsync(WebSocket socket, string message);
        Task SendMessageToAllAsync(string message);
    }
}
