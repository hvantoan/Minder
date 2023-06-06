using Minder.Service.Models.Team;
using Minder.Service.Models.User;
using System.Threading.Tasks;
using static Minder.Service.Enums;

namespace Minder.Service.Interfaces {

    public interface ITeamService {

        Task<TeamDto?> Get(string teamId);

        Task<ListTeamRes> List(ListTeamReq req);

        Task<TeamDto?> CreateOrUpdate(SaveTeamRequest model);

        Task Delete(string teamId);

        Task Leave(string teamId);

        Task Kick(string userId);

        Task<ListTeamRes> Find(FindTeamReq req);

        Task<ListTeamRes> Suggestion(SuggessTeamReq req);

        Task<object?> Automation(EAutoMation type, string hostTeamId, string? opposingTeamId = null, bool getTimeChooiceModel = false);

        Task<ListUserRes> SuggestUserForTeam(ListUserSuggest req);
    }
}