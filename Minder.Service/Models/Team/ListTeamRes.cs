using Minder.Services.Models;

namespace Minder.Service.Models.Team {

    public class ListTeamRes : BaseListRes<TeamDto> {
        public bool HasCreateTeam { get; set; } = false;
    }
}