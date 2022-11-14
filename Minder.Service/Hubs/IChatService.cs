using Minder.Service.Models.Chat;
using System.Threading.Tasks;

namespace Minder.Service.Hubs {
    public interface IChatService {
        Task JoinToRoom(UserRoom userRoom);
        Task SendMessageToRoom(UserRoom userRoom, string message);
        Task SendUserConnected(string roomId);
    }
}
