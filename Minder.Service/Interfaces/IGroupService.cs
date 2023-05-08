using Minder.Service.Models.Group;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface IGroupService {

        Task<GroupDto?> Get(string id);

        Task<ListGroupRes> List(ListGroupReq req);

        Task<string> Create(GroupDto model);

        Task Update(GroupDto model);

        Task Delete(string id);
    }
}