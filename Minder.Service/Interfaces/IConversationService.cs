using Minder.Service.Models.Conversation;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface IConversationService {

        Task<ConversationDto?> Get(string id);

        Task<ListConversationRes> List(ListConversationReq req);

        Task<string> Create(ConversationDto model);

        Task Update(ConversationDto model);

        Task Delete(string id);
    }
}