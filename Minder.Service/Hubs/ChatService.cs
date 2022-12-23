using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Minder.Database;
using Minder.Service.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minder.Service.Hubs {
    public class ChatService : Hub{
        private readonly string botUser;
        private readonly IDictionary<string, UserRoom> connections;
        private readonly MinderContext db;
        public ChatService(IDictionary<string, UserRoom> connections, IServiceProvider serviceProvider) {
 
            botUser = "My chat bot....";
            this.connections = connections;
            this.db = serviceProvider.GetRequiredService<MinderContext>();
        }

        public override Task OnDisconnectedAsync(Exception exception) {
            if (connections.TryGetValue(Context.ConnectionId, out UserRoom? user)) {
                connections.Remove(Context.ConnectionId);
                Clients.Group(user.RoomId).SendAsync("ReceiveMessage", botUser, $"{user.UserId} has left");
                SendUsersConnected(user.RoomId);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinToRoom(UserRoom userRoom) {
            await Groups.AddToGroupAsync(Context.ConnectionId, userRoom.RoomId);
            connections[Context.ConnectionId] = userRoom;
            await Clients.Group(userRoom.RoomId).SendAsync("ReceiveMessage", botUser, $"{userRoom.UserId} has joined {userRoom.RoomId}");
            await SendUsersConnected(userRoom.RoomId);
        }

        public async Task SendMessageToRoom(UserRoom userRoom, string message) {
            if (connections.TryGetValue(Context.ConnectionId, out UserRoom? User)) {
                await Clients.Group(userRoom.RoomId).SendAsync("ReceiveMessage", User.UserId, message);
            }
        }

        public Task SendUsersConnected(string roomId) {
            var users = connections.Values
                .Where(c => c.RoomId == roomId)
                .Select(c => c.UserId);

            return Clients.Group(roomId).SendAsync("UsersInRoom", users);
        }
    }
}
