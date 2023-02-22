using Minder.Service.Models.Chat;
using System.Threading.Tasks;

namespace Minder.Service.Hubs {
    public interface IChatService {
        Task JoinToRoom(Connection userRoom);
        Task SendMessageToRoom(Connection userRoom, string message);
        Task SendUserConnected(string roomId);
    }
}
