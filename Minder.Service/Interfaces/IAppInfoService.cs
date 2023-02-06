using Minder.Service.Models.AppInfo;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface IAppInfoService {

        Task<AppInfo> Get();

        Task Set(string ver);
    }
}