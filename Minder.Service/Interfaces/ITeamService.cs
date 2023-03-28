using Minder.Service.Models.Team;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface ITeamService {

        Task<TeamDto?> Get(string teamId);

        Task<ListTeamRes> List(ListTeamReq req);

        Task<string> CreateOrUpdate(TeamDto model);

        Task Delete(string teamId);
    }
}