using Minder.Database.Models;
using Minder.Service.Models;
using Minder.Services.Models;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface ITeamService {
        Task<string> CreateOrUpdate(TeamDto model);
    }
}