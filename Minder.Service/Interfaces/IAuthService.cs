using Minder.Services.Models.Auth;
using Minder.Services.Models.User;
using System.Threading.Tasks;

namespace Minder.Services.Interfaces {

    public interface IAuthService {

        Task<string> Register(UserDto model);

        Task<bool> VerifyUser(UserDto model);

        Task<LoginResponse> WebLogin(LoginRequest request);

        Task<LoginResponse> WebLoginGoogle(LoginGoogleRequest request);

        Task<LoginResponse> Refresh();
    }
}