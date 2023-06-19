using Minder.Service.Models.Match;
using System.Threading.Tasks;
using static Minder.Service.Enums;

namespace Minder.Service.Interfaces {

    public interface IMatchService {

        Task SwipeCard(CreateMatchReq req);

        Task<ListMatchRes> List(ListMatchReq req);

        Task<MatchDto?> Get(string matchId);

        Task<MatchDto?> Update(string matchId, string teamId, object model, EUpdateType type);

        Task Void(string id);

        Task ConfirmSettingMatch(string matchId, string teamId);

        Task AddTimeOpption(AddTimeOpptionReq req);

        Task MemberConfirm(string userId, string matchId);
    }
}