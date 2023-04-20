using Minder.Database.Models;
using Minder.Service.Models.Message;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface IMessageService {

        Task<ListMessageData> List(ListMessageReq req);

        Task CreateMessage(MessageDto model);
    }
}