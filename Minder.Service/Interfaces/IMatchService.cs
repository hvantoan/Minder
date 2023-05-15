using Minder.Service.Models.Match;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface IMatchService {

        Task SwipeCard(CreateMatchReq req);
    }
}