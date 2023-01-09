using Minder.Service.Models.Auth;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface ICacheManager {
        string VerifyOTP(Verify verify);

        Task<string> CreateOrUpdate<T>(string username, T data);
    }
}