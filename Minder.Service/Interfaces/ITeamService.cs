using Minder.Service.Models;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface ITeamService {

        Task<TeamDto?> Get(string teamId);

        Task<string> CreateOrUpdate(TeamDto model);

        Task Delete(string teamId);
    }
}