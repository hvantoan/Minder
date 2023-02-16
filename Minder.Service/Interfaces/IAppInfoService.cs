using Minder.Service.Models.AppInfo;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface IAppInfoService {

        Task<AppVer> GetVer();

        Task<AppAbout> GetAbout();

        Task Set(string? ver, string? aboutUs, string? hotline);
    }
}