using Minder.Service.Models.Team;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface ITeamService {

        Task<TeamDto?> Get(string teamId);

        Task<ListTeamRes> List(ListTeamReq req);

        Task<string> CreateOrUpdate(TeamDto model);

        Task Delete(string teamId);

        Task Invite(InviteDto model);

        Task<ListInviteRes> ListInvite(ListInviteReq req);

        Task ConfirmInvite(ConfirmInviteReq req);

        Task Leave(string teamId);

        Task Kick(string userId);
    }
}