using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Minder.Service.Models;
using Minder.Service.Models.Chat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Minder.Service.Hubs {

    [Authorize]
    public class ChatHub : Hub {
        private readonly IDictionary<string, Connection> connections;
        private readonly CurrentUser currentUser;

        public ChatHub(IDictionary<string, Connection> connections, IServiceProvider serviceProvider) {
            this.connections = connections;
            this.currentUser = serviceProvider.GetRequiredService<CurrentUser>();
        }

        public override async Task OnConnectedAsync() {
            connections[Context.ConnectionId] = new Connection() { Id = Context.ConnectionId, UserId = currentUser.UserId, GroupId = null };
            var connectionIds = new StringBuilder();
            int i = 0;
            foreach (var connection in connections.Values) {
                connectionIds.AppendLine($"{++i} UserId: {connection.UserId} - {connection.Id}");
            }

            await Console.Out.WriteLineAsync(connectionIds.ToString());
            await base.OnConnectedAsync();
        }

        public override  Task OnDisconnectedAsync(Exception exception) {
             Console.Out.WriteLine(Context.ConnectionId);
            if (connections.TryGetValue(Context.ConnectionId, out Connection? user)) {
                connections.Remove(Context.ConnectionId);
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}