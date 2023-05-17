using Minder.Service.Models.Team;
using System.Threading.Tasks;
using static Minder.Service.Enums;

namespace Minder.Service.Interfaces {

    public interface ITeamService {

        Task<TeamDto?> Get(string teamId);

        Task<ListTeamRes> List(ListTeamReq req);

        Task<string> CreateOrUpdate(TeamDto model);

        Task Delete(string teamId);

        Task Leave(string teamId);

        Task Kick(string userId);

        Task<ListTeamRes> Find(FindTeamReq req);

        Task<ListTeamRes> Suggession(SuggessTeamReq req);

        Task<object?> Automation(string teamId, EAutoMation type); 
    }
}