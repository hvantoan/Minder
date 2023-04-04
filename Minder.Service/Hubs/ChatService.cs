using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minder.Database;
using Minder.Service.Interfaces;
using Minder.Service.Models;
using Minder.Service.Models.Chat;
using Minder.Service.Models.Conversation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minder.Service.Hubs {

    [Authorize]
    public class ChatService : Hub {
        private readonly IDictionary<string, Connection> connections;
        private readonly MinderContext db;
        private readonly CurrentUser currentUser;
        private readonly IConversationService conversationService;

        public ChatService(IDictionary<string, Connection> connections, IServiceProvider serviceProvider) {
            this.connections = connections;
            this.db = serviceProvider.GetRequiredService<MinderContext>();
            this.currentUser = serviceProvider.GetRequiredService<CurrentUser>();
            this.conversationService = serviceProvider.GetRequiredService<IConversationService>();
        }

        public override async Task OnConnectedAsync() {
            await base.OnConnectedAsync();
            var conversations = await conversationService.List(new ListConversationReq { PageIndex = 0, PageSize = 20, SearchText = "" });
            await Clients.Client(Context.ConnectionId).SendAsync("Conversations", new { Conversations = conversations.Items });
        }

        public override Task OnDisconnectedAsync(Exception exception) {
            if (connections.TryGetValue(Context.ConnectionId, out Connection? user)) {
                connections.Remove(Context.ConnectionId);
                Clients.Group(user.ConversationId).SendAsync("ReceiveMessage", $"{user.UserId} has left");
                SendUsersConnected(user.ConversationId);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task Init() {
            var conversationIds = await this.db.Participants.Where(o => o.UserId == this.currentUser.UserId).Select(o => o.ConversationId).ToListAsync();
            var conversations = await this.db.Conversations.Where(o => conversationIds.Contains(o.Id)).ToListAsync();
            await Clients.Caller.SendAsync("Init", conversations);
        }

        public async Task JoinToRoom(string roomId) {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            connections[Context.ConnectionId] = new Connection() { UserId = this.currentUser.UserId, ConversationId = roomId };
            await Clients.Group(roomId).SendAsync("ReceiveMessage", $"{this.currentUser.UserId} has joined {roomId}");
            await SendUsersConnected(roomId);
        }

        public async Task SendMessageToRoom(Connection userRoom, string message) {
            if (connections.TryGetValue(Context.ConnectionId, out Connection? User)) {
                await Clients.Group(userRoom.ConversationId).SendAsync("ReceiveMessage", User.UserId, message);
            }
        }

        public Task SendUsersConnected(string roomId) {
            var users = connections.Values
                .Where(c => c.ConversationId == roomId)
                .Select(c => c.UserId);

            return Clients.Group(roomId).SendAsync("UsersInRoom", users);
        }
    }
}