using Minder.Services.Models;

namespace Minder.Service.Models.Team {

    public class SuggessTeamReq : BaseListReq {
        public string TeamId { get; set; } = string.Empty;
    }
}