using Minder.Database.Enums;
using Minder.Service.Models.Auth;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface ICacheManager {

        (string, EVerifyType) VerifyOTP(string otp);

        Task<string> CreateOrUpdate<T>(EVerifyType type, T data);
    }
}