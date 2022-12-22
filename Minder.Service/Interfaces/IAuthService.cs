using Minder.Services.Models.Auth;
using System.Threading.Tasks;

namespace Minder.Services.Interfaces {
    public interface IAuthService {
        Task<LoginResponse> WebLogin(LoginRequest request);
        Task<LoginResponse> WebLoginGoogle(LoginGoogleRequest request);
        Task<LoginResponse> Refresh();
    }
}