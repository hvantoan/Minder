using Minder.Service.Models.Stadium;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface IStadiumService {

        Task<ListStadiumRes> List(ListStadiumReq req);

        Task<StadiumDto?> Get(GetStadiumReq req);

        Task<string> CreateOrUpdate(StadiumDto model);

        Task Delete(DeleteStadiumReq req);

        Task InitialData();
    }
}