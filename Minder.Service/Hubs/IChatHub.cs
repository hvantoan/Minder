using Minder.Database.Enums;
using System.Threading.Tasks;

namespace Minder.Service.Hubs {

    public interface IChatHub {
        Task JoinToGroup(string groupId);
        Task SendNotify(string groupId, ENotify action);
    }
}