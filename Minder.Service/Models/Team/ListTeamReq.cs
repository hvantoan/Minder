using Minder.Services.Models;

namespace Minder.Service.Models.Team {

    public class ListTeamReq : BaseListReq {
        public bool IsMyTeam { get; set; }
    }
}