using Minder.Services.Models;

namespace Minder.Service.Models.Team {

    public class ListInviteReq : BaseListReq {
        public string? TeamId { get; set; }
    }
}