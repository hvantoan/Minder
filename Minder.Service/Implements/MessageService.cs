using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Minder.Service.Hubs;
using Minder.Service.Interfaces;
using Minder.Service.Models.Chat;
using Minder.Service.Models.Message;
using Minder.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minder.Service.Implements {

    public class MessageService : BaseService, IMessageService {
        private readonly IHubContext<ChatService> hub;

        public MessageService(IDictionary<string, Connection> connections, IServiceProvider serviceProvider, IHubContext<ChatService> hub) : base(serviceProvider) {
            this.hub = hub;
        }

        public async Task CreateMessage(MessageDto model) {
            var isExit = await this.db.Participants.AnyAsync(o => o.ConversationId == model.ConversationId && o.UserId == this.current.UserId);
            if (!isExit) return;
            var message = model.ToEntity();
            message.SenderId = this.current.UserId;
            await this.db.Messages.AddAsync(message);
            await this.db.SaveChangesAsync();
            await hub.Clients.Group(message.ConversationId).SendAsync("ReceiveMessage", MessageDto.FromEntity(message));
        }

        public async Task<ListMessageData> List(ListMessageReq req) {
            if (req.ConversationId == null) return new();
            var query = this.db.Messages.AsNoTracking().Where(o => o.ConversationId == req.ConversationId);
            var listResponse = await query.OrderByDescending(o => o.CreateAt).Take(req.Take).Skip(req.Skip).Select(o => MessageDto.FromEntity(o)).ToListAsync();
            return new ListMessageData() {
                Count = await query.CountAsync(),
                Items = listResponse.OrderBy(o => o!.CreateAt).ToList(),
            };
        }
    }
}