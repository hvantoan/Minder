using Minder.Services.Models;

namespace Minder.Service.Models.Team {

    public class ConfirmInviteReq : BaseGetReq {
        public bool IsJoin { get; set; }
    }
}