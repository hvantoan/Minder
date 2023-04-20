using Minder.Services.Models;

namespace Minder.Service.Models.Message {

    public class ListMessageReq : BaseListReq {
        public string ConversationId { get; set; } = string.Empty;
    }
}