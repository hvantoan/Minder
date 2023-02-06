using Minder.Service.Models.Auth;
using Minder.Service.Models.User;
using Minder.Services.Models.Auth;
using Minder.Services.Models.User;
using System.Threading.Tasks;

namespace Minder.Services.Interfaces {

    public interface IAuthService {

        Task<LoginResponse> WebLogin(LoginRequest request);

        Task Register(UserDto model);

        Task ForgotPassword(ForgotPasswordReq request);

        Task<LoginResponse> Refresh();

        Task<VerifyResponse> Verify(Verify verify);

        Task<UserNameValidate> CheckUser(string userName);
    }
}