using Minder.Services.Models;

namespace Minder.Service.Models.Team {

    public class ConfirmInviteReq : BaseReq {
        public bool IsJoin { get; set; }
    }
}