using Minder.Database.Enums;
using Minder.Service.Models.Message;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface IChatService {

        Task JoinToGroup(string groupId);

        Task SendMessageNotify(string groupId, ENotify action);

        Task SendNotify(string groupId, ENotify action);
    }
}