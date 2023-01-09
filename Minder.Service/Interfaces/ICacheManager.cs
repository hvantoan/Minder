using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface ICacheManager {
        bool VerifyOtp(string username, string? code);
        Task<string> CreateOrUpdate(string username);
    }
}