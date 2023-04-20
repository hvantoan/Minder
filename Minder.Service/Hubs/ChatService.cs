using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minder.Database;
using Minder.Service.Interfaces;
using Minder.Service.Models;
using Minder.Service.Models.Chat;
using Minder.Service.Models.Conversation;
using Minder.Service.Models.Message;
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
        private readonly IMessageService messageService;

        public ChatService(IDictionary<string, Connection> connections, IServiceProvider serviceProvider) {
            this.connections = connections;
            this.db = serviceProvider.GetRequiredService<MinderContext>();
            this.currentUser = serviceProvider.GetRequiredService<CurrentUser>();
            this.conversationService = serviceProvider.GetRequiredService<IConversationService>();
            this.messageService = serviceProvider.GetRequiredService<IMessageService>();
        }

        public override async Task OnConnectedAsync() {
            connections[Context.ConnectionId] = new Connection() { UserId = currentUser.UserId, ConversationId = null};
            await base.OnConnectedAsync();
            await SendConversations();
            await SendLastConversationId();
        }

        public override Task OnDisconnectedAsync(Exception exception) {
            if (connections.TryGetValue(Context.ConnectionId, out Connection? user)) {
                connections.Remove(Context.ConnectionId);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinToRoom(string conversationId) {
            var isInRoom = await this.db.Participants.AnyAsync(o => o.UserId == this.currentUser.UserId && o.ConversationId == conversationId);

            if (isInRoom) {
                await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
                connections[Context.ConnectionId] = new Connection() { UserId = this.currentUser.UserId, ConversationId = conversationId };
                await SendListMessage(conversationId);
            }
        }

        private async Task SendLastConversationId() {
            var lastConversationId = await this.db.Messages.Where(o => o.SenderId == this.currentUser.UserId).OrderBy(o => o.CreateAt).Select(o => o.ConversationId).FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(lastConversationId)) {
                lastConversationId = await this.db.Participants.Where(o => o.UserId == this.currentUser.UserId).OrderBy(o => o.JoinAt).Select(o => o.ConversationId).FirstOrDefaultAsync();
            }

            if (!string.IsNullOrEmpty(lastConversationId)) {
                await Clients.Client(Context.ConnectionId).SendAsync("LastConversationId", lastConversationId);
            }
        }

        private async Task SendListMessage(string coversationId, string? searchText = null, int pageSize = 5, int pageIndex = 0) {
            var req = new ListMessageReq {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SearchText = searchText,
                ConversationId = coversationId
            };

            var messages = await this.messageService.List(req);
            await Clients.Client(Context.ConnectionId).SendAsync("ReciveListMessage", new { Messages = messages.Items });
        }

        private async Task SendConversations(string? searchText = null, int pageSize = 20, int pageIndex = 0) {
            var req = new ListConversationReq {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SearchText = searchText
            };
            var conversations = await conversationService.List(req);
            foreach (var item in conversations.Items) {
                if (item == null) continue;
                item.Online = connections.Values.Any(o => o.UserId != this.currentUser.UserId && item.ParticipantIds != null && item.ParticipantIds.Contains(o.UserId));
            }
            await Clients.Client(Context.ConnectionId).SendAsync("Conversations", new { Conversations = conversations.Items });
        }
    }
}