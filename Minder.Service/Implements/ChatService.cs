using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minder.Database.Enums;
using Minder.Service.Extensions;
using Minder.Service.Hubs;
using Minder.Service.Interfaces;
using Minder.Service.Models.Chat;
using Minder.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minder.Service.Implements {

    public class ChatService : BaseService, IChatService {
        private readonly IHubContext<ChatHub> hubContext;
        private readonly IDictionary<string, Connection> connections;

        public ChatService(IServiceProvider serviceProvider) : base(serviceProvider) {
            this.hubContext = serviceProvider.GetRequiredService<IHubContext<ChatHub>>();
            this.connections = serviceProvider.GetRequiredService<IDictionary<string, Connection>>();
        }

        public async Task SendNotify(string groupId, ENotify action, object? prarams = null) {
            var userIds = await this.db.Participants.Where(o => o.GroupId == groupId && o.UserId != this.current.UserId).Select(o => o.UserId).ToListAsync();
            var connectionIds = connections.Values.Where(o => userIds.Contains(o.UserId)).Select(o => o.Id).ToList();

            if (prarams != null) {
                await hubContext.Clients.Clients(connectionIds).SendAsync(action.Description(), JsonConvert.SerializeObject(prarams));
            } else await hubContext.Clients.Clients(connectionIds).SendAsync(action.Description());
        }

        public async Task JoinToGroup(string groupId) {
            var isInRoom = await this.db.Participants.AnyAsync(o => o.UserId == this.current.UserId && o.GroupId == groupId);
            var conection = connections.Values.FirstOrDefault(o => o.UserId == this.current.UserId);
            if (isInRoom && conection != null) {
                await hubContext.Groups.AddToGroupAsync(conection.Id, groupId);
                conection.GroupId = groupId;
                connections[conection.Id] = conection;
                await SendNotify(groupId, ENotify.RecieveMessage);
            }
        }
    }
}