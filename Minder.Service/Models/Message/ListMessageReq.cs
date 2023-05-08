using Minder.Services.Models;

namespace Minder.Service.Models.Message {

    public class ListMessageReq : BaseListReq {
        public string GroupId { get; set; } = string.Empty;
    }
}