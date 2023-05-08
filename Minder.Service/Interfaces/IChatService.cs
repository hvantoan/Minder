using Minder.Database.Enums;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface IChatService {

        Task JoinToGroup(string groupId);

        Task SendNotify(string groupId, ENotify action, object? prarams = null);
    }
}